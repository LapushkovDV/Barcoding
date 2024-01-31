using System.ComponentModel.DataAnnotations;

namespace ERP_Admin_Panel.Data.ViewModels
{
    public class UserViewModel
    {
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isEdit = false;

        [Required(ErrorMessage = "Логин является обязательным")]
        [StringLength(50, ErrorMessage = "Слишком длинный логин")]
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                IsDisabledButton = IsEdit ? IsDisabledEdit() : IsDisabledAdd();
            }
        }
        [Required(ErrorMessage = "Пароль является обязательным")]
        [MinLength(8, ErrorMessage = "Слишком короткий пароль")]
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                IsDisabledButton = IsEdit ? IsDisabledEdit() : IsDisabledAdd();
            }
        }
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                IsDisabledButton = IsEdit ? IsDisabledEdit() : IsDisabledAdd();
            }
        }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public int RoleId { get; set; }
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                _isEdit = value;

                IsDisabledButton = value ? IsDisabledEdit() : IsDisabledAdd();
            }
        }

        public bool IsDisabledButton { get; private set; } = true;

        private bool IsDisabledAdd() => Login.Length < 8 ||
                                      Password.Length < 8 ||
                                      ConfirmPassword != Password;
        private bool IsDisabledEdit() => Login.Length < 8;

    }
}
