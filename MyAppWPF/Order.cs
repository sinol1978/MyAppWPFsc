namespace MyAppWPF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public int Id { get; set; }

        public int ClientId { get; set; }

        [Column(TypeName = "date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [StringLength(11)]
        public string TotalS { get; set; }

        public int ManagerId { get; set; }

        [Required]
        [StringLength(11)]
        public string PaymentS { get; set; }

        [Required]
        [StringLength(11)]
        public string BalanceS { get; set; }

        public virtual Client Clients { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderLine> OrderLines { get; set; }

        public virtual User Users { get; set; }
    }
}
