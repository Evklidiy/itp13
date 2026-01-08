using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Контакты")]
        public string? Contacts { get; set; }

        [Display(Name = "Рейтинг")]
        public int Rating { get; set; }
    }
}