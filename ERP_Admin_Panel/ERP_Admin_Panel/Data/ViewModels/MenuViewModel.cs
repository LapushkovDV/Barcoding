using System.ComponentModel.DataAnnotations;

namespace ERP_Admin_Panel.Data.ViewModels
{
    public class MenuViewModel
    {
        [Required(ErrorMessage = "Название меню является обязательным")]
        [StringLength(50, ErrorMessage = "Слишком длинное название меню")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Action меню является обязательным")]
        [StringLength(50, ErrorMessage = "Слишком длинное Action меню")]
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Npp { get; set; } = 0;
    }
}
