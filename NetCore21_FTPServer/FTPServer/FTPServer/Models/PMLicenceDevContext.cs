using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FTPServer.Models
{
    public partial class PMLicenceDevContext : DbContext
    {
        public PMLicenceDevContext()
        {
        }

        public PMLicenceDevContext(DbContextOptions<PMLicenceDevContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PmlicenceKey> PmlicenceKey { get; set; }
        public virtual DbSet<PmlicenceKeyHis> PmlicenceKeyHis { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=;Database=PMLicenceDev;User Id=;Password=;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PmlicenceKey>(entity =>
            {
                entity.HasKey(e => e.PublicKey);

                entity.ToTable("PMLicenceKey");

                entity.Property(e => e.PublicKey)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CusEmail)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CusName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CusPhone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<PmlicenceKeyHis>(entity =>
            {
                entity.HasKey(e => new { e.Hwkey, e.PublicKey });

                entity.ToTable("PMLicenceKeyHis");

                entity.Property(e => e.Hwkey)
                    .HasColumnName("HWKey")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.PublicKey)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ActivedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CurrentDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ExpiredDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.PrivateKey)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.SoftwareVersion)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValueSql("('')");

                entity.HasOne(d => d.PublicKeyNavigation)
                    .WithMany(p => p.PmlicenceKeyHis)
                    .HasForeignKey(d => d.PublicKey)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PMLicenceKeyHis_PMLicenceKey");
            });
        }
    }
}
