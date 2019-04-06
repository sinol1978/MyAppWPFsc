namespace MyAppWPF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model11")
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<OrderLine> OrderLines { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Clients)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderLines)
                .WithRequired(e => e.Orders)
                .HasForeignKey(e => e.OrderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.OrderLines)
                .WithRequired(e => e.Products)
                .HasForeignKey(e => e.ProductId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.ManagerId);
        }
    }
}
