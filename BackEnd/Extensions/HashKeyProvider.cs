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
        return _config["HashingSystem:DefaultHashingSurveyKey"]??"0";
    }
}
