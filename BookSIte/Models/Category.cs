using System.ComponentModel.DataAnnotations;

namespace BookSIte.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNumber { get; set; }
    }
}
