namespace Services
{
    public interface IPasswordService
    {
        string HashPasswordPBKDF2(string password);
        bool AuthenticatePBKDF2(string password, string loginStatusPassword);
    }
}