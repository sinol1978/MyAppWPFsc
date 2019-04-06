namespace MyAppWPF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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
