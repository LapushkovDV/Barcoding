namespace ERP_Admin_Panel.Services.Toast
{
    public class ToastSetting
    {
        public string Heading { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string BaseClass { get; set; } = string.Empty;
        public string AdditionalClasses { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;

        public ToastSetting(string heading, string message, string baseClass, string additionalClasses, string iconClass)
        {
            Heading = heading;
            Message = message;
            BaseClass = baseClass;
            AdditionalClasses = additionalClasses;
            IconClass = iconClass;
        }
    }
}
