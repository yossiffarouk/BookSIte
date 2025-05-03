using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class UserMangmentVM
    {

        public ApplicationsUser ApplicationsUser { get; set; }
        public IEnumerable< SelectListItem> Roles { get; set; }
        public IEnumerable<SelectListItem> Companys { get; set; }
        
    }
}
