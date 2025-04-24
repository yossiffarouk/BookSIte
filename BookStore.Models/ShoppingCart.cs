using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1, 1000, ErrorMessage = " 1 - 1000 pls")]
        public int Count { get; set; }
        public string ApplicationsUserId { get; set; }
        [ForeignKey("ApplicationsUserId")]
        [ValidateNever]
        public ApplicationsUser ApplicationsUser { get; set; }
    }
}
