using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Клиент")]
        public string? ClientName { get; set; }

        [Display(Name = "Автомобиль")]
        public string? CarDetails { get; set; }

        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        public List<SalePosition> Positions { get; set; } = new();
    }
}