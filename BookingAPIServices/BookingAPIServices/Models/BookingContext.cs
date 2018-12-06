using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingAPIServices.Models
{
    public partial class BookingContext : DbContext
    {
        //Нужно добавить вручную
        public BookingContext()
        {
        }

        //Нужно добавить вручную
        public BookingContext(DbContextOptions<BookingContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Booking> Booking { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer(@"Server=tcp:rsoi.database.windows.net,1433;Initial Catalog=Booking;Persist Security Info=False;User ID=pkiselev;Password=pass;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"); //for Azure
                //optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Booking;Integrated Security=True");
                optionsBuilder.UseMySQL("server=46.254.21.136; port=3306; database=p460741_lab; user=p460741_pavel; password=2M8p8B0c");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.BookingId)
                    .HasColumnName("BookingID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");
            });
        }
    }
}
