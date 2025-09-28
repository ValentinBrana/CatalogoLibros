using Microsoft.EntityFrameworkCore;
using ActLibros.Models;
namespace ActLibros.Data
{
    public class LibrosDbContext : DbContext
    {
        public LibrosDbContext(DbContextOptions<LibrosDbContext> options)
            : base(options)
        {
        }

        // Propiedad DbSet para cada modelo que quieres mapear a una tabla
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Autor> Autores { get; set; }

        // Opcional: Configuración de la relación uno-a-muchos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Especifica que el campo autorId en Libro es la Foreign Key para Autor
            modelBuilder.Entity<Libro>()
                .HasOne(l => l.autor)
                .WithMany()
                .HasForeignKey(l => l.autorId);
        }
    }
}
