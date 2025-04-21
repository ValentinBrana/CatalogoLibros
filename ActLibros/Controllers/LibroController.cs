using ActLibros.Models;
using Microsoft.AspNetCore.Mvc;

namespace ActLibros.Controllers
{
    public class LibroController : Controller
    {
        public IActionResult Index()
        {
            var autor1 = new Autor { id = 1, nombre = "George Orwell" };
            var autor2 = new Autor { id = 2, nombre = "Ray Bradbury" };

            var libros = new List<Libro>{
               new Libro {
               id = 1,
               titulo = "1984",
               anioPublicacion = 1949,
               autor = autor1,
               UrlImagen = "https://upload.wikimedia.org/wikipedia/en/c/c3/1984first.jpg"
               },
               new Libro {
               id = 2,
               titulo = "Fahrenheit 451",
               anioPublicacion = 1953,
               autor = autor2,
               UrlImagen ="https://upload.wikimedia.org/wikipedia/en/d/db/Fahrenheit_451_1st_ed_cover.jpg"
               },
               new Libro {
               id = 3,
               titulo = "Rebelión en la granja",
               anioPublicacion = 1945,
               autor = autor1,
               UrlImagen = "https://upload.wikimedia.org/wikipedia/en/f/fb/Animal_Farm_-_1st_edition.jpg"
               }
               };
            return View(libros);
        }

        public IActionResult Detalle(int id)
        {
            var autor1 = new Autor { id = 1, nombre = "George Orwell" };
            var autor2 = new Autor { id = 2, nombre = "Ray Bradbury" };

            var libros = new List<Libro>{
            new Libro {
            id = 1,
            titulo = "1984",
            anioPublicacion = 1949,
            autor = autor1,
            UrlImagen = "https://upload.wikimedia.org/wikipedia/en/c/c3/1984first.jpg"
            },
            new Libro {
            id = 2,
            titulo = "Fahrenheit 451",
            anioPublicacion = 1953,
            autor = autor2,
            UrlImagen ="https://upload.wikimedia.org/wikipedia/en/d/db/Fahrenheit_451_1st_ed_cover.jpg"
            },
            new Libro {
            id = 3,
            titulo = "Rebelión en la granja",
            anioPublicacion = 1945,
            autor = autor1,
            UrlImagen = "https://upload.wikimedia.org/wikipedia/en/f/fb/Animal_Farm_-_1st_edition.jpg"
            }
            };

            var libro = libros.FirstOrDefault(l => l.id == id);

            if (libro == null)
            {
                ViewBag.Error = "Libro no encontrado";
                return View("Detalle");
            }

            return View("Detalle", libro);
        }
    }
}

