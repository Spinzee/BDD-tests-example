using Infrastructure.Enums;

namespace Model
{
    public class LoginStatus
    {
        public int Id { get; set; }
        public AccountStatus Status { get; set; }
        public string Password { get; set; }
        public bool IsPasswordValid { get; set; }
        public bool IsEmailValid { get; set; }
    }
}
