using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RoomsAPIServices.Models
{
    public partial class RoomsContext : DbContext
    {

        //Нужно добавить вручную
        public RoomsContext()
        {
        }

        public virtual DbSet<Rooms> Rooms { get; set; }

        //Нужно добавить вручную
        public RoomsContext(DbContextOptions<RoomsContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=tcp:rsoi.database.windows.net,1433;Initial Catalog=Rooms;Persist Security Info=False;User ID=pkiselev;Password=0JI4DyWek;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rooms>(entity =>
            {
                entity.HasKey(e => e.RoomId);

                entity.Property(e => e.RoomId)
                    .HasColumnName("RoomID")
                    .ValueGeneratedNever();
            });
        }
    }
}
