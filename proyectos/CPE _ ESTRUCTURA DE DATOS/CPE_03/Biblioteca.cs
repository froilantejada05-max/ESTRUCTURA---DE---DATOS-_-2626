using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaBiblioteca
{
    // Núcleo del sistema. Combina un conjunto (HashSet) para garantizar
    // ISBN únicos y varios mapas (Dictionary) para indexar la información
    // y permitir búsquedas rápidas por distintos criterios.
    public class Biblioteca
    {
        // CONJUNTO: guarda solo los ISBN registrados, sin duplicados.
        private readonly HashSet<string> _isbnRegistrados;

        // MAPA principal: ISBN -> Libro. Acceso directo O(1) por clave.
        private readonly Dictionary<string, Libro> _catalogo;

        // MAPA secundario: Género -> conjunto de ISBN de ese género.
        private readonly Dictionary<string, HashSet<string>> _indicePorGenero;

        // MAPA secundario: Autor -> conjunto de ISBN de ese autor.
        private readonly Dictionary<string, HashSet<string>> _indicePorAutor;

        public Biblioteca()
        {
            _isbnRegistrados = new HashSet<string>();
            _catalogo = new Dictionary<string, Libro>();
            _indicePorGenero = new Dictionary<string, HashSet<string>>();
            _indicePorAutor = new Dictionary<string, HashSet<string>>();
        }

        // Registra un libro nuevo. Si el ISBN ya existe en el conjunto,
        // se rechaza el registro y se informa el motivo.
        public bool RegistrarLibro(Libro libro)
        {
            // Add() de HashSet devuelve false si el elemento ya existía;
            // así se detecta el duplicado sin recorrer nada (operación O(1)).
            if (!_isbnRegistrados.Add(libro.Isbn))
            {
                Console.WriteLine($"[RECHAZADO] {new IsbnDuplicadoException(libro.Isbn).Message}");
                return false;
            }

            _catalogo[libro.Isbn] = libro;
            AgregarAIndice(_indicePorGenero, libro.Genero, libro.Isbn);
            AgregarAIndice(_indicePorAutor, libro.Autor, libro.Isbn);

            Console.WriteLine($"[REGISTRADO] {libro.Titulo} (ISBN {libro.Isbn})");
            return true;
        }

        private void AgregarAIndice(Dictionary<string, HashSet<string>> indice, string clave, string isbn)
        {
            if (!indice.ContainsKey(clave))
            {
                indice[clave] = new HashSet<string>();
            }
            indice[clave].Add(isbn);
        }

        // Búsqueda directa por ISBN usando el mapa principal.
        public Libro BuscarPorIsbn(string isbn)
        {
            _catalogo.TryGetValue(isbn, out Libro libro);
            return libro;
        }

        // Reportería: lista los libros de un género usando el índice de mapas.
        public List<Libro> ListarPorGenero(string genero)
        {
            var resultado = new List<Libro>();
            if (_indicePorGenero.TryGetValue(genero, out HashSet<string> isbns))
            {
                foreach (string isbn in isbns)
                {
                    resultado.Add(_catalogo[isbn]);
                }
            }
            return resultado;
        }

        // Reportería: lista los libros de un autor usando el índice de mapas.
        public List<Libro> ListarPorAutor(string autor)
        {
            var resultado = new List<Libro>();
            if (_indicePorAutor.TryGetValue(autor, out HashSet<string> isbns))
            {
                foreach (string isbn in isbns)
                {
                    resultado.Add(_catalogo[isbn]);
                }
            }
            return resultado;
        }

        // Cambia el estado de un libro a "prestado" si existe y está disponible.
        public bool PrestarLibro(string isbn)
        {
            Libro libro = BuscarPorIsbn(isbn);
            if (libro == null || !libro.Disponible) return false;
            libro.Disponible = false;
            return true;
        }

        // Cambia el estado de un libro a "disponible" nuevamente.
        public bool DevolverLibro(string isbn)
        {
            Libro libro = BuscarPorIsbn(isbn);
            if (libro == null || libro.Disponible) return false;
            libro.Disponible = true;
            return true;
        }

        public int TotalLibros => _catalogo.Count;

        public int TotalDisponibles => _catalogo.Values.Count(l => l.Disponible);

        public int TotalGeneros => _indicePorGenero.Count;

        public int TotalAutores => _indicePorAutor.Count;

        // Muestra el catálogo completo ordenado por título.
        public void MostrarCatalogoCompleto()
        {
            Console.WriteLine("\n=== CATÁLOGO COMPLETO ===");
            foreach (Libro libro in _catalogo.Values.OrderBy(l => l.Titulo))
            {
                Console.WriteLine(libro);
            }
        }

        // Muestra un resumen de cuántos libros hay por género (usa el mapa).
        public void MostrarResumenPorGenero()
        {
            Console.WriteLine("\n=== LIBROS POR GÉNERO ===");
            foreach (var par in _indicePorGenero.OrderBy(p => p.Key))
            {
                Console.WriteLine($"{par.Key}: {par.Value.Count} libro(s)");
            }
        }
    }
}
