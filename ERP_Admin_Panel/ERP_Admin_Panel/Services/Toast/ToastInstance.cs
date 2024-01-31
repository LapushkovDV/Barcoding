namespace ERP_Admin_Panel.Services.Toast
{
    public class ToastInstance
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public ToastSetting ToastSetting { get; set; }
    }
}
