namespace MyAppWPF
{
    using System.ComponentModel.DataAnnotations;

    public partial class OrderLine
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        [StringLength(11)]
        public string Quantity { get; set; }

        [Required]
        [StringLength(11)]
        public string Price { get; set; }

        [Required]
        [StringLength(11)]
        public string Total { get; set; }

        public int OrderId { get; set; }

        public virtual Order Orders { get; set; }

        public virtual Product Products { get; set; }
    }
}
