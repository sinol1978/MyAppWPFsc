namespace MyAppWPF
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Client
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Client()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Phone1 { get; set; }

        [StringLength(20)]
        public string Phone2 { get; set; }

        [StringLength(11)]
        public string DebtS { get; set; }

        [StringLength(100)]
        public string Mail { get; set; }

        [StringLength(250)]
        public string RegionInfo { get; set; }

        [StringLength(500)]
        public string Descr { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
