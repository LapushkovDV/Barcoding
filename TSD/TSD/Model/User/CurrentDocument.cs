namespace TSD.Model.User
{
    public class CurrentDocument
    {
        public int Id { get; set; }
        public string IdDoc { get; set; } = string.Empty;
        public bool IsAllowEditDoc { get; set; } = true;
        public string GuidStatus { get; set; } = string.Empty;
        public int StatusId { get; set; } = (int)StatusEnum.InWork;
        public string LastAction { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
    }
}
