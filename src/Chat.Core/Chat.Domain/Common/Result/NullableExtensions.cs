namespace Chat.Domain.Common.Result;

public static class NullableExtensions
{
    public static Result<TValue> OnError<TValue>(this TValue? value, Error error) where TValue : class
    {
        return value is not null
            ? value
            : error;
    }
}
