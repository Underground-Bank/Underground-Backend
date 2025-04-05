using System.ComponentModel.DataAnnotations;

namespace UndergroundBank.AccountService.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? RedirectUri { get; set; }

        public string? ReturnUrl { get; set; }
        public bool WasFailed { get; set; } = false;
    }
}
