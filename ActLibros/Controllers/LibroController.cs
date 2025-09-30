using ActLibros.Models;
using ActLibros.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace ActLibros.Controllers
{
    public class LibroController : Controller
    {
        // 1. Inyección de Dependencias: Almacena el DbContext
        private readonly LibrosDbContext _contexto;

        // 2. Constructor para recibir el DbContext inyectado
        public LibroController(LibrosDbContext contexto)
        {
            _contexto = contexto;

            // Opcional: Si quieres asegurar que los autores iniciales estén en la DB la primera vez
            if (!_contexto.Autores.Any())
            {
                _contexto.Autores.AddRange(ObtenerAutoresIniciales());
                _contexto.SaveChanges();
            }
        }

        // --- Métodos Auxiliares para Autores ---

        private List<Autor> ObtenerAutoresIniciales()
        {
            var autores = new List<Autor>
    {
        // ELIMINAR: id = 1,
        new Autor { nombre = "George Orwell" }, 
        
        // ELIMINAR: id = 2,
        new Autor { nombre = "Ray Bradbury" }
    };
            return autores;
        }

        private List<Autor> ObtenerAutores()
        {
            // Obtener autores desde la base de datos
            return _contexto.Autores.ToList();
        }

        // --- Acciones del Controller ---

        public IActionResult Index()
        {
            if (TempData["ColorFondo"] != null)
            {
                ViewBag.ColorFondo = TempData["ColorFondo"].ToString();
            }
            else
            {
                ViewBag.ColorFondo = "white";
            }

            // Leer todos los libros de la DB, incluyendo la entidad Autor relacionada
            var libros = _contexto.Libros.Include(l => l.autor).ToList();

            return View(libros);
        }

        public IActionResult Detalle(int id)
        {
            ViewBag.ColorFondo = TempData["ColorFondo"] ?? "white";
            TempData.Keep("ColorFondo");

            // Buscar en la DB
            var libroSeleccionado = _contexto.Libros
                                              .Include(l => l.autor)
                                              .FirstOrDefault(l => l.id == id);

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
            TempData.Keep("ColorFondo");

            // Filtrar por AutorId en la DB
            var librosAutor = _contexto.Libros
                                        .Include(l => l.autor)
                                        .Where(l => l.autorId == id)
                                        .ToList();

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
            ViewBag.Autores = ObtenerAutores();
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Libro libro)
        {
            ModelState.Remove("autor");
            if (!ModelState.IsValid)
            {
                ViewBag.Autores = ObtenerAutores();
                return View(libro);
            }

            var autorSeleccionado = ObtenerAutores().FirstOrDefault(a => a.id == libro.autorId);
            if (autorSeleccionado == null)
            {
                ModelState.AddModelError("autorId", "El autor no existe.");
                ViewBag.Autores = ObtenerAutores();
                return View(libro);
            }

            
            libro.autor = autorSeleccionado;

            // AGREGAR a la base de datos
            _contexto.Libros.Add(libro);
            _contexto.SaveChanges();

            TempData["Mensaje"] = $"libro '{libro.titulo}' creado con éxito.";

            return RedirectToAction("Detalle", new { id = libro.id });
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            // Buscar en la base de datos
            var libro = _contexto.Libros.FirstOrDefault(l => l.id == id);
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

            // --- Lógica de Validación Original Mantenida ---
            if (string.IsNullOrEmpty(libro.titulo))
            {
                ModelState.AddModelError("titulo", "El título no puede estar vacío.");
                ViewBag.Autores = ObtenerAutores();
                return View(libro);
            }

            string patronTitulo = @"^[a-zA-Z0-9\s]+$";

            if (!Regex.IsMatch(libro.titulo, patronTitulo))
            {
                ModelState.AddModelError("titulo", "El título solo puede contener letras, números y espacios.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Autores = ObtenerAutores();
                return View(libro);
            }

            // ACTUALIZAR en la base de datos
            _contexto.Libros.Update(libro);
            _contexto.SaveChanges();

            TempData["Mensaje"] = "Libro editado correctamente";
            return RedirectToAction("Detalle", new { id = libro.id });
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            // 1. Buscar el libro en la DB
            var libro = _contexto.Libros.FirstOrDefault(l => l.id == id);

            if (libro == null)
            {
                // El libro no existe en la DB
                return NotFound();
            }

            // 2. Marcar el libro para su eliminación
            _contexto.Libros.Remove(libro);

            // 3. Persistir los cambios en la DB
            _contexto.SaveChanges();

            TempData["Mensaje"] = $"Libro con ID {id} eliminado correctamente.";

            return RedirectToAction("Index");
        }
    }





    //    public class LibroController : Controller
    //    {
    //        private static List<Libro> libros = new List<Libro> {

    //            new Libro{
    //               id = 1,
    //               titulo = "1984",
    //               anioPublicacion = 1949,
    //               autor = new Autor { id = 1, nombre = "George Orwell" },
    //               UrlImagen = "https://upload.wikimedia.org/wikipedia/en/c/c3/1984first.jpg"
    //               },

    //               new Libro {
    //               id = 2,
    //               titulo = "Fahrenheit 451",
    //               anioPublicacion = 1953,
    //               autor = new Autor { id = 2, nombre = "Ray Bradbury" },
    //               UrlImagen ="https://upload.wikimedia.org/wikipedia/en/d/db/Fahrenheit_451_1st_ed_cover.jpg"
    //               },

    //               new Libro
    //               {
    //                   id = 3,
    //                   titulo = "Rebelión en la granja",
    //                   anioPublicacion = 1945,
    //                   autor = new Autor { id = 1, nombre = "George Orwell" },
    //                   UrlImagen = "https://upload.wikimedia.org/wikipedia/en/f/fb/Animal_Farm_-_1st_edition.jpg"
    //               }
    //        };

    //        private static List<Libro> ObtenerLibros()
    //        {
    //            return libros;
    //        }

    //        public IActionResult Index()
    //        {

    //            if (TempData["ColorFondo"] != null)
    //            {
    //                ViewBag.ColorFondo = TempData["ColorFondo"].ToString();
    //            }
    //            else
    //            {
    //                ViewBag.ColorFondo = "white"; 
    //            }

    //            return View(libros); 
    //        }

    //        public IActionResult Detalle(int id)
    //        {
    //            ViewBag.ColorFondo = TempData["ColorFondo"] ?? "white";
    //            TempData.Keep("ColorFondo"); 
    //            var libroSeleccionado = libros.FirstOrDefault(l => l.id == id);
    //            if (libroSeleccionado == null)
    //            {
    //                return RedirectToAction("Index");
    //            }

    //            ViewBag.Mensaje = TempData["Mensaje"];
    //            return View(libroSeleccionado);
    //        }

    //        public IActionResult Autor(int id)
    //        {
    //            ViewBag.ColorFondo = TempData["ColorFondo"] ?? "white";
    //            TempData.Keep("ColorFondo"); 
    //            var libros = ObtenerLibros();
    //            var librosAutor = libros.Where(l => l?.autor?.id == id).ToList();

    //            if (librosAutor.Count == 0)
    //            {
    //                ViewBag.Mensaje = "Este autor no tiene libros registrados.";
    //                return View("Autor", new List<Libro>());
    //            }

    //            return View("Autor", librosAutor);
    //        }

    //        public IActionResult CambiarFondo(string color)
    //        {
    //            TempData["ColorFondo"] = color;
    //            return RedirectToAction("Index");
    //        }

    //        [HttpGet]
    //        public IActionResult Crear()
    //        {
    //            ViewBag.Autores = ObtenerAutores(); 
    //            return View();
    //        }

    //        [HttpPost]
    //        public IActionResult Crear(Libro libro)
    //        {
    //            ModelState.Remove("autor"); // Ignorar el id al validar
    //            if (!ModelState.IsValid)
    //            {
    //                ViewBag.Autores = ObtenerAutores();
    //                return View(libro);
    //            }
    //            // Simular almacenamiento del libro
    //            // libros.Add(libro);
    //            var autorSeleccionado = ObtenerAutores().FirstOrDefault(a => a.id == libro.autorId);
    //            if (autorSeleccionado == null)
    //            {
    //                ModelState.AddModelError("autorId", "El autor no existe.");
    //                ViewBag.Autores = ObtenerAutores();
    //                return View(libro);
    //            }

    //            libro.autor = autorSeleccionado;
    //            libro.id = ObtenerLibros().Any() ? ObtenerLibros().Max(l => l.id) + 1 : 1; 
    //            ObtenerLibros().Add(libro); 

    //            TempData["Mensaje"] = $"libro '{libro.titulo}' creado con éxito.";

    //            return RedirectToAction("Detalle", new { id = libro.id });
    //        }

    //        private static List<Autor> ObtenerAutores()
    //        {
    //            var autores = new List<Autor>
    //            {
    //                new Autor { id = 1, nombre = "George Orwell" },
    //                new Autor { id = 2, nombre = "Ray Bradbury" }
    //            };
    //            return autores;
    //        }

    //        [HttpGet]
    //        public IActionResult Editar(int id)
    //        {
    //            var libro = ObtenerLibros().FirstOrDefault(l => l.id == id);
    //            if (libro == null)
    //            {
    //                return NotFound();
    //            }
    //            ViewBag.Autores = ObtenerAutores();
    //            return View(libro);
    //        }
    //        [HttpPost]
    //        public IActionResult Editar(Libro libro)
    //        {
    //            ModelState.Remove("autor");

    //            if (string.IsNullOrEmpty(libro.titulo))
    //            {
    //                ModelState.AddModelError("titulo", "El título no puede estar vacío.");
    //                ViewBag.Autores = ObtenerAutores();
    //                return View(libro);
    //            }

    //            string patronTitulo = @"^[a-zA-Z0-9\s]+$";

    //            if (!Regex.IsMatch(libro.titulo, patronTitulo))
    //            {
    //                ModelState.AddModelError("titulo", "El título solo puede contener letras, números y espacios.");
    //            }


    //            if (!ModelState.IsValid)
    //            {
    //                ViewBag.Autores = ObtenerAutores();
    //                return View(libro);
    //            }


    //            var libroExistente = ObtenerLibros().FirstOrDefault(l => l.id == libro.id);
    //            if (libroExistente == null)
    //            {
    //                return NotFound();
    //            }

    //            libroExistente.titulo = libro.titulo;
    //            libroExistente.anioPublicacion = libro.anioPublicacion;
    //            libroExistente.UrlImagen = libro.UrlImagen;

    //            var autorSeleccionado = ObtenerAutores().FirstOrDefault(a => a.id == libro.autorId);
    //            if (autorSeleccionado != null)
    //            {
    //                libroExistente.autor = autorSeleccionado;
    //            }

    //            TempData["Mensaje"] = "Libro editado correctamente";
    //            return RedirectToAction("Detalle", new { id = libro.id });
    //        }

    //        [HttpPost]
    //        public IActionResult Eliminar(int id)
    //        {

    //            var libro = libros.FirstOrDefault(l => l.id == id);

    //            if (libro == null)
    //            {
    //                return NotFound(); 
    //            }


    //            libros.Remove(libro);


    //            return RedirectToAction("Index");
    //        }
    //    }
}

