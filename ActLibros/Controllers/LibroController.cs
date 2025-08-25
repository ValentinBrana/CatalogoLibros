using ActLibros.Models;
using Microsoft.AspNetCore.Mvc;

namespace ActLibros.Controllers
{
    public class LibroController : Controller
    {
        private static List<Libro> libros = new List<Libro> {

            new Libro{
               id = 1,
               titulo = "1984",
               anioPublicacion = 1949,
               autor = new Autor { id = 1, nombre = "George Orwell" },
               UrlImagen = "https://upload.wikimedia.org/wikipedia/en/c/c3/1984first.jpg"
               },

               new Libro {
               id = 2,
               titulo = "Fahrenheit 451",
               anioPublicacion = 1953,
               autor = new Autor { id = 2, nombre = "Ray Bradbury" },
               UrlImagen ="https://upload.wikimedia.org/wikipedia/en/d/db/Fahrenheit_451_1st_ed_cover.jpg"
               },

               new Libro
               {
                   id = 3,
                   titulo = "Rebelión en la granja",
                   anioPublicacion = 1945,
                   autor = new Autor { id = 1, nombre = "George Orwell" },
                   UrlImagen = "https://upload.wikimedia.org/wikipedia/en/f/fb/Animal_Farm_-_1st_edition.jpg"
               }
        };

        private static List<Libro> ObtenerLibros()
        {
            return libros;
        }

        public IActionResult Index()
        {
            // *** ¡Aquí está la clave! ***
            // Recupera el color de TempData si existe y asígnalo a ViewBag.
            if (TempData["ColorFondo"] != null)
            {
                ViewBag.ColorFondo = TempData["ColorFondo"].ToString();
            }
            else
            {
                ViewBag.ColorFondo = "white"; // Color por defecto si no hay ninguno en TempData
            }

            // Aquí deberías obtener tus libros de la base de datos o servicio
            // Por ahora, usaremos la lista estática de ejemplo
            return View(libros); // Pasa tus libros a la vista
        }

        public IActionResult Detalle(int id)
        {
            ViewBag.ColorFondo = TempData["ColorFondo"] ?? "white";
            TempData.Keep("ColorFondo"); // mantiene TempData para futuras páginas
            var libroSeleccionado = libros.FirstOrDefault(l => l.id == id);
            if (libroSeleccionado == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Mensaje = TempData["Mensaje"];
            return View(libroSeleccionado);
        }

        public IActionResult Autor(int id)
        {
            ViewBag.ColorFondo = TempData["ColorFondo"] ?? "white";
            TempData.Keep("ColorFondo"); // mantiene TempData para futuras páginas
            var libros = ObtenerLibros();
            var librosAutor = libros.Where(l => l?.autor?.id == id).ToList();

            if (librosAutor.Count == 0)
            {
                ViewBag.Mensaje = "Este autor no tiene libros registrados.";
                return View("Autor", new List<Libro>());
            }

            return View("Autor", librosAutor);
        }

        public IActionResult CambiarFondo(string color)
        {
            TempData["ColorFondo"] = color;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Autores = ObtenerAutores(); // método que devuelve lista de autores
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Libro libro)
        {
            ModelState.Remove("autor"); // Ignorar el id al validar
            if (!ModelState.IsValid)
            {
                ViewBag.Autores = ObtenerAutores();
                return View(libro);
            }
            // Simular almacenamiento del libro
            // libros.Add(libro);
            var autorSeleccionado = ObtenerAutores().FirstOrDefault(a => a.id == libro.autorId);
            if (autorSeleccionado == null)
            {
                ModelState.AddModelError("autorId", "El autor no existe.");
                ViewBag.Autores = ObtenerAutores();
                return View(libro);
            }

            libro.autor = autorSeleccionado;
            libro.id = ObtenerLibros().Any() ? ObtenerLibros().Max(l => l.id) + 1 : 1; // Asignar un nuevo ID
            ObtenerLibros().Add(libro); // Simular almacenamiento

            TempData["Mensaje"] = $"libro '{libro.titulo}' creado con éxito.";

            return RedirectToAction("Detalle", new { id = libro.id });
        }

        private static List<Autor> ObtenerAutores()
        {
            var autores = new List<Autor>
            {
                new Autor { id = 1, nombre = "George Orwell" },
                new Autor { id = 2, nombre = "Ray Bradbury" }
            };
            return autores;
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var libro = ObtenerLibros().FirstOrDefault(l => l.id == id);
            if (libro == null)
            {
                return NotFound();
            }
            ViewBag.Autores = ObtenerAutores();
            return View(libro);
        }

        [HttpPost]
        public IActionResult Editar(Libro libro)
        {
            ModelState.Remove("autor");
            if (!ModelState.IsValid)
            {
                ViewBag.Autores = ObtenerAutores();

                return View(libro);
            }
            var libroExistente = ObtenerLibros().FirstOrDefault(l => l.id == libro.id);
            if (libroExistente == null)
            {
                return NotFound();
            }
            
            libroExistente.titulo = libro.titulo;
            libroExistente.anioPublicacion = libro.anioPublicacion;
            libroExistente.UrlImagen = libro.UrlImagen;
            var autorSeleccionado = ObtenerAutores().FirstOrDefault(a => a.id == libro.autorId);
            if (autorSeleccionado != null)
            {
                libroExistente.autor = autorSeleccionado;
            }
            TempData["Mensaje"] = "Libro editado correctamente";
            return RedirectToAction("Detalle", new { id = libro });
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            // 1. Encontrar el libro en la lista usando su ID
            var libro = libros.FirstOrDefault(l => l.id == id);

            if (libro == null)
            {
                return NotFound(); // El libro no fue encontrado en la lista
            }

            // 2. Eliminar el libro de la lista
            libros.Remove(libro);

            // 3. Redirigir al usuario a la página de inicio que muestra la lista de libros
            return RedirectToAction("Index");
        }
    }
}

