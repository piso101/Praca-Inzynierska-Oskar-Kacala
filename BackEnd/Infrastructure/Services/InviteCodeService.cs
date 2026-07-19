using System.Security.Cryptography;
using System.Text;

public interface IInviteCodeService
{
    string GenerateInviteCode(long surveyId);

    bool ValidateInviteCode(long surveyId, string inviteCode);

    public long? GetSurveyIdFromInviteCode(string inviteCode);
}

public class InviteCodeService : IInviteCodeService
{
    private readonly IHashKeyProvider _hashKeyProvider;

    public InviteCodeService(IHashKeyProvider hashKeyProvider)
    {
        _hashKeyProvider = hashKeyProvider;
    }

    public string GenerateInviteCode(long surveyId)
    {
        var key = _hashKeyProvider.GetHashKey();
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(surveyId.ToString()));

        var code = Convert.ToBase64String(hash)
            .Replace("+", "")
            .Replace("/", "")
            .Substring(0, 8);

        return $"{surveyId:X8}-{code}";
    }

    public long? GetSurveyIdFromInviteCode(string inviteCode)
    {
        if (string.IsNullOrEmpty(inviteCode)) return null;

        var parts = inviteCode.Split('-');
        if (parts.Length != 2) return null;

        if (!long.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out long surveyId))
            return null;

        var key = _hashKeyProvider.GetHashKey();
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(surveyId.ToString()));

        var expectedCode = Convert.ToBase64String(hash)
            .Replace("+", "")
            .Replace("/", "")
            .Substring(0, 8);

        if (parts[1] != expectedCode)
            return null;

        return surveyId;
    }

    public bool ValidateInviteCode(long surveyId, string inviteCode)
    {
        var expected = GenerateInviteCode(surveyId);
        return inviteCode == expected;
    }
}