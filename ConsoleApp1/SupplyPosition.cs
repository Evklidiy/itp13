using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class SupplyPosition
    {
        [Key]
        public int Id { get; set; }
        public int SupplyId { get; set; }
        public Supply? Supply { get; set; }

        [Display(Name = "Запчасть")]
        public int PartId { get; set; }
        public Part? Part { get; set; }

        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        [Display(Name = "Цена закупки")]
        public decimal PurchasePrice { get; set; }
    }
}