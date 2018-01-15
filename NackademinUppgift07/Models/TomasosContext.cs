using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NackademinUppgift07.Models
{
    public partial class TomasosContext : DbContext
    {
        public virtual DbSet<Bestallning> Bestallning { get; set; }
        public virtual DbSet<BestallningMatratt> BestallningMatratt { get; set; }
        public virtual DbSet<Kund> Kund { get; set; }
        public virtual DbSet<Matratt> Matratt { get; set; }
        public virtual DbSet<MatrattProdukt> MatrattProdukt { get; set; }
        public virtual DbSet<MatrattTyp> MatrattTyp { get; set; }
        public virtual DbSet<Produkt> Produkt { get; set; }

	    public TomasosContext(DbContextOptions options)
			: base(options)
	    {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bestallning>(entity =>
            {
                entity.Property(e => e.BestallningId).HasColumnName("BestallningID");

                entity.Property(e => e.BestallningDatum).HasColumnType("datetime");

                entity.Property(e => e.KundId).HasColumnName("KundID");

                entity.HasOne(d => d.Kund)
                    .WithMany(p => p.Bestallning)
                    .HasForeignKey(d => d.KundId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bestallning_Kund");
            });

            modelBuilder.Entity<BestallningMatratt>(entity =>
            {
                entity.HasKey(e => new { e.MatrattId, e.BestallningId });

                entity.Property(e => e.MatrattId).HasColumnName("MatrattID");

                entity.Property(e => e.BestallningId).HasColumnName("BestallningID");

                entity.Property(e => e.Antal).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Bestallning)
                    .WithMany(p => p.BestallningMatratt)
                    .HasForeignKey(d => d.BestallningId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BestallningMatratt_Bestallning");

                entity.HasOne(d => d.Matratt)
                    .WithMany(p => p.BestallningMatratt)
                    .HasForeignKey(d => d.MatrattId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BestallningMatratt_Matratt");
            });

            modelBuilder.Entity<Kund>(entity =>
            {
                entity.Property(e => e.KundId).HasColumnName("KundID");

                entity.Property(e => e.AnvandarNamn)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gatuadress)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Losenord)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Namn)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Postnr)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Postort)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Telefon)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Matratt>(entity =>
            {
                entity.Property(e => e.MatrattId).HasColumnName("MatrattID");

                entity.Property(e => e.Beskrivning)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.MatrattNamn)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.MatrattTypNavigation)
                    .WithMany(p => p.Matratt)
                    .HasForeignKey(d => d.MatrattTyp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Matratt_MatrattTyp");
            });

            modelBuilder.Entity<MatrattProdukt>(entity =>
            {
                entity.HasKey(e => new { e.MatrattId, e.ProduktId });

                entity.Property(e => e.MatrattId).HasColumnName("MatrattID");

                entity.Property(e => e.ProduktId).HasColumnName("ProduktID");

                entity.HasOne(d => d.Matratt)
                    .WithMany(p => p.MatrattProdukt)
                    .HasForeignKey(d => d.MatrattId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatrattProdukt_Matratt");

                entity.HasOne(d => d.Produkt)
                    .WithMany(p => p.MatrattProdukt)
                    .HasForeignKey(d => d.ProduktId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatrattProdukt_Produkt");
            });

            modelBuilder.Entity<MatrattTyp>(entity =>
            {
                entity.HasKey(e => e.MatrattTyp1);

                entity.Property(e => e.MatrattTyp1).HasColumnName("MatrattTyp");

                entity.Property(e => e.Beskrivning)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Produkt>(entity =>
            {
                entity.Property(e => e.ProduktId).HasColumnName("ProduktID");

                entity.Property(e => e.ProduktNamn)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}
