using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class Part
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Артикул обязателен")]
        [Display(Name = "Артикул")]
        public string Article { get; set; } = string.Empty;

        [Required(ErrorMessage = "Название обязательно")]
        [Display(Name = "Название")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Производитель")]
        public string Manufacturer { get; set; } = string.Empty;

        [Display(Name = "Совместимость")]
        public string? CompatibleModels { get; set; }

        [Display(Name = "Цена закупки")]
        public decimal PurchasePrice { get; set; }

        [Display(Name = "Цена продажи")]
        public decimal SalePrice { get; set; }

        public Stock? Stock { get; set; }
    }
}