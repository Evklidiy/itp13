using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class SalePosition
    {
        [Key]
        public int Id { get; set; }
        public int SaleId { get; set; }
        public Sale? Sale { get; set; }

        [Display(Name = "Запчасть")]
        public int PartId { get; set; }
        public Part? Part { get; set; }

        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        [Display(Name = "Цена продажи")]
        public decimal Price { get; set; }
    }
}