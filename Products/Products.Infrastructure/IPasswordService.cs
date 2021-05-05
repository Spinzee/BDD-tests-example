namespace Products.Infrastructure
{
    public interface IPasswordService
    {
        string HashPasswordPBKDF2(string password);
    }
}
