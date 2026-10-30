using System.ComponentModel.DataAnnotations;

namespace Gym.Business.ViewModels.Accounts
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email { get; set; } = default!;

        [DataType(DataType.Password)]

        public string Password { get; set; } = default!;

        public bool RememberMe { get; set; }


        public string? ReturnUrl { get; set; }


    }
}
