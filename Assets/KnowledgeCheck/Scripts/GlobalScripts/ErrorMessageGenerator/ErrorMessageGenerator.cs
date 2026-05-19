using System;

public static class ErrorMessageGenerator
{
    public static void GenerateErrorMessage<T>(T classWithError, string errorMessage, out string generatedErrorMessage)
    {
        generatedErrorMessage = $"[{classWithError.GetType().Name.ToUpper()}]: {errorMessage}";
    }

    public static void GenerateErrorMessage<T>(T classWithError, Exception error, out string generatedErrorMessage)
    {
        generatedErrorMessage = $"[{classWithError.GetType().Name.ToUpper()}]: {error.Message}";
    }

    public static void GenerateException<T>(T classWithException, string errorMessage, out Exception generatedException)
    {
        GenerateErrorMessage<T>(classWithException, errorMessage, out string generatedErrorMessage);
        generatedException = new Exception(
            message: generatedErrorMessage
        );
    }

    public static void GenerateException<T>(T classWithException, Exception error, out Exception generatedException)
    {
        GenerateErrorMessage<T>(classWithException, error, out string generatedErrorMessage);
        generatedException = new Exception(
            message: generatedErrorMessage
        );
    }

    public static void GenerateSimpleError<T>(T classWithException, string message)
    {
        GenerateErrorMessage(classWithException, message, out string generatedErrorMessage);
        GenerateException(classWithException, generatedErrorMessage, out Exception generatedException);
        UnityEngine.Debug.LogError(generatedErrorMessage);
        throw generatedException;
    }

    public static void GenerateSimpleError<T>(T classWithException, Exception exceptionMessage)
    {
        GenerateErrorMessage(classWithException, exceptionMessage, out string generatedErrorMessage);
        GenerateException(classWithException, generatedErrorMessage, out Exception generatedException);
        UnityEngine.Debug.LogError(generatedErrorMessage);
        throw generatedException;
    }
}