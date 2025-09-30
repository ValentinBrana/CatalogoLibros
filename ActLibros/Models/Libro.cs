using System.ComponentModel.DataAnnotations;

namespace ActLibros.Models
{
    public class Libro
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "El título solo puede contener letras, números y espacios.")]
        public string? titulo { get; set; }

        [Required(ErrorMessage = "El año de publicación es obligatorio.")]
        [Range(1800, 2025, ErrorMessage = "El año debe estar entre 1800 y 2025.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "El año debe tener exactamente 4 dígitos.")]
        public int anioPublicacion { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un autor.")]
        public Autor? autor { get; set; }

        [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]

        [Url(ErrorMessage = "Por favor, introduce una URL válida.")]

        [RegularExpression(@"^https?://.*", ErrorMessage = "La URL debe comenzar con http:// o https://.")]
        public string? UrlImagen { get; set; }
        public int autorId { get; set; }
    }
}
