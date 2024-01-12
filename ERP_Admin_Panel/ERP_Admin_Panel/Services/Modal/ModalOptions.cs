namespace ERP_Admin_Panel.Services.Modal
{
    public class ModalOptions
    {
        public string Position { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public bool? DisableBackgroundCancel { get; set; }
        public bool? HideHeader { get; set; }
        public bool? HideCloseButton { get; set; }
    }
}
