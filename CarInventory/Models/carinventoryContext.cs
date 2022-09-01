using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CarInventory.Models
{
    public partial class carinventoryContext : DbContext
    {
        public carinventoryContext()
        {
        }

        public carinventoryContext(DbContextOptions<carinventoryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;user=root;password=root123;database=carinventory", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("cars");

                entity.Property(e => e.CarId).HasColumnName("car_id");

                entity.Property(e => e.Brand)
                    .HasMaxLength(255)
                    .HasColumnName("brand");

                entity.Property(e => e.Model)
                    .HasMaxLength(255)
                    .HasColumnName("model");

                entity.Property(e => e.New).HasColumnName("new");

                entity.Property(e => e.Price)
                    .HasPrecision(8, 2)
                    .HasColumnName("price");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
