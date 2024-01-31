using System.ComponentModel.DataAnnotations;

namespace ERP_Admin_Panel.Data.ViewModels
{
    public class EventViewModel
    {
        [Required]
        public DateTime StartDateTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        [Required]
        public DateTime EndDateTime { get; set; } = DateTime.Now;
    }
}
