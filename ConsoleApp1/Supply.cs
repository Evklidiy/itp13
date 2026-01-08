using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class Supply
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Поставщик")]
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Статус")]
        public string Status { get; set; } = "Ожидается";

        public List<SupplyPosition> Positions { get; set; } = new();
    }
}