using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartsWarehouse.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Part")]
        public int PartId { get; set; }
        public Part? Part { get; set; }

        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        [Display(Name = "Стеллаж/Ячейка")]
        public string? Location { get; set; }

        [Display(Name = "Мин. остаток")]
        public int MinQuantity { get; set; }
    }
}