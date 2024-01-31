using System.ComponentModel.DataAnnotations;

namespace ERP_Admin_Panel.Data.ViewModels
{
    public class RoleViewModel
    {
        private string _name = string.Empty;

        [Required(ErrorMessage = "Название роли является обязательным")]
        [StringLength(50, ErrorMessage = "Слишком длинное название роли")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                IsDisabledButton = IsDisabled();
            }
        }
        public string Description { get; set; } = string.Empty;

        public bool IsDisabledButton { get; private set; } = true;

        private bool IsDisabled() => Name == string.Empty;
    }
}
