using System;

namespace SistemaBiblioteca
{
    // Representa un libro dentro del catálogo de la biblioteca.
    // El ISBN se usa como identificador único para todas las estructuras
    // (conjunto de control de duplicados y mapa de acceso directo).
    public class Libro
    {
        public string Isbn { get; }
        public string Titulo { get; }
        public string Autor { get; }
        public string Genero { get; }
        public int Anio { get; }
        public bool Disponible { get; set; }

        public Libro(string isbn, string titulo, string autor, string genero, int anio)
        {
            Isbn = isbn;
            Titulo = titulo;
            Autor = autor;
            Genero = genero;
            Anio = anio;
            Disponible = true;
        }

        public override string ToString()
        {
            string estado = Disponible ? "Disponible" : "Prestado";
            return $"{Isbn} | {Titulo} | {Autor} | {Genero} | {Anio} | {estado}";
        }
    }
}
