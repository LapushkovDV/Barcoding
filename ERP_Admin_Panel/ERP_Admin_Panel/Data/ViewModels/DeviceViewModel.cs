using System.ComponentModel.DataAnnotations;

namespace ERP_Admin_Panel.Data.ViewModels
{
    public class DeviceViewModel
    {
        private string _deviceCode = string.Empty;

        [Required(ErrorMessage = "Код устройства является обязательным")]
        [MaxLength(50)]
        public string DeviceCode
        {
            get => _deviceCode;
            set
            {
                _deviceCode = value;
                IsDisabledButton = IsDisabled();
            }
        }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;

        public bool IsDisabledButton { get; private set; } = true;

        private bool IsDisabled() => DeviceCode == string.Empty;
    }
}
