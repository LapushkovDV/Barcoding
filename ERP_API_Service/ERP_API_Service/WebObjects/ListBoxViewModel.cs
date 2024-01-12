using ERP_API_Service.Objects;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ERP_API_Service.WebObjects
{
    public class ListBoxViewModel
    {
        public RoleMenu RoleMenu { get; set; }
        public int[] SelectListItemsIds { get; set; }
        public List<SelectListItem> SelectLists { get; set; }
    }
}
