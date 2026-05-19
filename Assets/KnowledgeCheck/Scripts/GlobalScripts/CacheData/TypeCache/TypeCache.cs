using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public static class TypeCache
{
    private static readonly Dictionary<Type, Type[]> _resolvedTypes = new();

    public static Type[] GetRelatedTypes(Type type)
    {
        if (type == null)
        {
            ErrorMessageGenerator.GenerateSimpleError(typeof(TypeCache), "Type is empty");
            return Array.Empty<Type>();
        }

        if (!_resolvedTypes.TryGetValue(type, out var types))
        {
            var resultList = new List<Type>();
            Type current = type;

            while (current != null && !current.IsInterface)
            {
                var directInterfaces = current.GetInterfaces();

                bool hasDirectSingleton = directInterfaces.Contains(typeof(IBindingSingletonComponent));
                bool hasDirectTransient = directInterfaces.Contains(typeof(IBindingTransientComponent));

                if (hasDirectSingleton || hasDirectTransient)
                {
                    resultList.Add(current);
                }
                current = current.BaseType;
            }

            foreach (var i in type.GetInterfaces())
            {
                var directInterfacesOfInterface = i.GetInterfaces();

                bool isDirectSingleton =
                    directInterfacesOfInterface.Contains(typeof(IBindingSingletonComponent))
                    && i != typeof(IBindingSingletonComponent);

                bool isDirectTransient =
                    directInterfacesOfInterface.Contains(typeof(IBindingTransientComponent))
                    && i != typeof(IBindingTransientComponent);

                if (isDirectSingleton || isDirectTransient)
                {
                    resultList.Add(i);
                }
            }

            types = resultList.ToArray();
            _resolvedTypes[type] = types;
        }
        return types;
    }

    public static void Dispose()
    {
        _resolvedTypes.Clear();
    }
}