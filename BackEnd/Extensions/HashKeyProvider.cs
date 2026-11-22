using Microsoft.Extensions.Configuration;

public interface IHashKeyProvider
{
    string GetHashKey();
}

public class HashKeyProvider : IHashKeyProvider
{
    private readonly IConfiguration _config;

    public HashKeyProvider(IConfiguration config)
    {
        _config = config;
    }

    public string GetHashKey()
    {
        var key = _config["HashingSystem:DefaultHashingSurveyKey"];
        if (string.IsNullOrWhiteSpace(key) || System.Text.Encoding.UTF8.GetByteCount(key) < 32)
        {
            throw new InvalidOperationException("HashingSystem:DefaultHashingSurveyKey must be configured with at least 32 bytes");
        }

        return key;
    }
}
