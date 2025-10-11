public interface IValidationRule<T>
{
    bool Validate(T data, out string errorMessage);
}
