﻿namespace ActLibros.Models
{
    public class Libro
    {
        public int id { get; set; }
        public string titulo { get; set; }
        public int anioPublicacion { get; set; }
        public Autor autor { get; set; }

        public string UrlImagen { get; set; }
    }
}
