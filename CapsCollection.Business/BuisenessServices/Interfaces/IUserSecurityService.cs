namespace CapsCollection.Business.BuisenessServices.Interfaces
{
    public interface IUserSecurityService
    {
        string GenerateSalt();
        string CalculatePasswordHash(string password, string salt);
        bool CheckPassword(string hashString, string saltString, string passwordToCheck);
        bool CheckPassword(byte[] hashBytes, byte[] saltBytes, string passwordToCheck);
    }
}