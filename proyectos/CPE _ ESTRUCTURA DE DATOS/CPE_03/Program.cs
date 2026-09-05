using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SistemaBiblioteca
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Biblioteca biblioteca = new Biblioteca();

            Console.WriteLine("=== SISTEMA DE REGISTRO DE LIBROS - BIBLIOTECA ===\n");

            // Lista de libros a registrar (10 libros distintos + 2 intentos duplicados)
            var libros = new List<Libro>
            {
                new Libro("978-84-376-0494-7", "Cien Años de Soledad", "Gabriel García Márquez", "Novela", 1967),
                new Libro("978-84-206-5786-3", "El Amor en los Tiempos del Cólera", "Gabriel García Márquez", "Novela", 1985),
                new Libro("978-84-663-2404-5", "1984", "George Orwell", "Ciencia Ficción", 1949),
                new Libro("978-84-663-1234-1", "Un Mundo Feliz", "Aldous Huxley", "Ciencia Ficción", 1932),
                new Libro("978-84-397-2077-2", "Rayuela", "Julio Cortázar", "Novela", 1963),
                new Libro("978-84-9759-234-0", "Los Miserables", "Victor Hugo", "Novela Histórica", 1862),
                new Libro("978-84-670-0625-9", "Veinte Poemas de Amor", "Pablo Neruda", "Poesía", 1924),
                new Libro("978-84-320-2957-3", "El Aleph", "Jorge Luis Borges", "Cuento", 1949),
                new Libro("978-84-204-8261-7", "Ficciones", "Jorge Luis Borges", "Cuento", 1944),
                new Libro("978-84-663-8901-2", "Fahrenheit 451", "Ray Bradbury", "Ciencia Ficción", 1953),
                // Intentos de duplicado (mismo ISBN que libros ya registrados)
                new Libro("978-84-376-0494-7", "Cien Años de Soledad (copia)", "Gabriel García Márquez", "Novela", 1967),
                new Libro("978-84-663-2404-5", "1984 (copia)", "George Orwell", "Ciencia Ficción", 1949),
            };

            Console.WriteLine("--- Registrando libros ---");
            int exitosos = 0, rechazados = 0;
            foreach (var libro in libros)
            {
                if (biblioteca.RegistrarLibro(libro)) exitosos++; else rechazados++;
            }
            Console.WriteLine($"\nResumen de registro: {exitosos} registrados / {rechazados} rechazados por duplicado.\n");

            biblioteca.MostrarCatalogoCompleto();
            biblioteca.MostrarResumenPorGenero();

            Console.WriteLine("\n=== BÚSQUEDA POR AUTOR (uso del mapa autor -> ISBN) ===");
            var librosBorges = biblioteca.ListarPorAutor("Jorge Luis Borges");
            foreach (var l in librosBorges) Console.WriteLine(l);

            Console.WriteLine("\n=== SIMULACIÓN DE PRÉSTAMOS ===");
            string[] isbnsAPrestar = { "978-84-376-0494-7", "978-84-663-2404-5", "978-84-320-2957-3" };
            foreach (var isbn in isbnsAPrestar)
            {
                biblioteca.PrestarLibro(isbn);
                Console.WriteLine($"Prestado: {biblioteca.BuscarPorIsbn(isbn).Titulo}");
            }
            biblioteca.DevolverLibro("978-84-663-2404-5");
            Console.WriteLine($"Devuelto: {biblioteca.BuscarPorIsbn("978-84-663-2404-5").Titulo}");

            Console.WriteLine($"\nTotal de libros en catálogo : {biblioteca.TotalLibros}");
            Console.WriteLine($"Total disponibles ahora      : {biblioteca.TotalDisponibles}");
            Console.WriteLine($"Total de géneros distintos   : {biblioteca.TotalGeneros}");
            Console.WriteLine($"Total de autores distintos   : {biblioteca.TotalAutores}");

            // --- Análisis de tiempo de ejecución: Dictionary (mapa) vs List (búsqueda lineal) ---
            Console.WriteLine("\n=== ANÁLISIS DE TIEMPO DE EJECUCIÓN ===");
            AnalizarTiempos(biblioteca, libros.Take(10).ToList());

            Console.WriteLine("\nPresione una tecla para salir...");
            Console.ReadKey();
        }

        // Compara el tiempo de búsqueda usando el Dictionary (mapa, O(1) promedio)
        // contra una búsqueda lineal sobre una List<Libro> (O(n)), repitiendo la
        // operación muchas veces para que la diferencia sea medible con Stopwatch.
        static void AnalizarTiempos(Biblioteca biblioteca, List<Libro> listaPlana)
        {
            const int repeticiones = 200000;
            string isbnBuscado = "978-84-663-8901-2"; // último libro de la lista

            Stopwatch cronometroMapa = Stopwatch.StartNew();
            for (int i = 0; i < repeticiones; i++)
            {
                var _ = biblioteca.BuscarPorIsbn(isbnBuscado);
            }
            cronometroMapa.Stop();

            Stopwatch cronometroLista = Stopwatch.StartNew();
            for (int i = 0; i < repeticiones; i++)
            {
                var _ = listaPlana.Find(l => l.Isbn == isbnBuscado);
            }
            cronometroLista.Stop();

            Console.WriteLine($"Búsquedas realizadas por método: {repeticiones:N0}");
            Console.WriteLine($"Tiempo total con Dictionary (mapa)  : {cronometroMapa.Elapsed.TotalMilliseconds:F2} ms");
            Console.WriteLine($"Tiempo total con List (lineal)      : {cronometroLista.Elapsed.TotalMilliseconds:F2} ms");
        }
    }
}
