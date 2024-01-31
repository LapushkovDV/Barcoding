using System.ComponentModel.DataAnnotations;

namespace ERP_Admin_Panel.Data.ViewModels
{
    public class LoginViewModel
    {
        private string _login = string.Empty;

        [Required(ErrorMessage = "Логин является обязательным")]
        [StringLength(50, ErrorMessage = "Слишком длинный логин")]
        public string Login
        {
            get => _login;
            set
            {
                _login = value;

                if (_login != null && _login.Length > 0)
                {
                    IsDisabledButtonSignIn = false;
                }
                else
                {
                    IsDisabledButtonSignIn = true;
                }
            }
        }
        [Required(ErrorMessage = "Пароль является обязательным")]
        [MinLength(8, ErrorMessage = "Слишком короткий пароль")]
        public string Password { get; set; } = string.Empty;

        public bool IsDisabledButtonSignIn { get; private set; } = true;
    }
}
