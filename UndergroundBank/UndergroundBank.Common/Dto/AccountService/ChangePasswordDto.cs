namespace UndergroundBank.Common.Dto.AccountService
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
