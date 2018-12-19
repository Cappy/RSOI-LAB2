using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Auth.Entities
{
    public partial class TokensContext : DbContext
    {
        public TokensContext()
        {
        }

        public TokensContext(DbContextOptions<TokensContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tokens> Tokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=46.254.21.136;port=3306;user=p460741_pavel;password=2M8p8B0c;database=p460741_lab");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tokens>(entity =>
            {
                entity.HasKey(e => e.Token);

                entity.Property(e => e.Token).HasColumnType("text(512)");

                entity.Property(e => e.Revoked).HasColumnType("int(16)");

                entity.Property(e => e.IssuedAt).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasColumnType("binary(16)")
                    .ValueGeneratedNever();
            });
        }
    }
}
