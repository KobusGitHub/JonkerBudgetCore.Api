namespace JonkerBudgetCore.Api.Auth.Encrypt
{
    public interface IEncrypter
    {
        string GenerateHash(string password, byte[] salt);
        byte[] GenerateSalt();
    }
}
