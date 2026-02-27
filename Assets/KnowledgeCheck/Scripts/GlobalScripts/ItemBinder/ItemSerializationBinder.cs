using System;
using Newtonsoft.Json.Serialization;

public class ItemSerializationBinder : DefaultSerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        if (typeName == SerializableItemType.ContainerItem.ToString())
        {
            return typeof(ContainerItem);
        }
        else if (typeName == SerializableItemType.EquippableItem.ToString())
        {
            return typeof(EquippableItem);
        }
        else if (typeName == SerializableItemType.ConsumableItem.ToString())
        {
            return typeof(ConsumableItem);
        }

        return base.BindToType(assemblyName, typeName);
    }
}

public enum SerializableItemType
{
    ContainerItem,
    EquippableItem,
    ConsumableItem
}