using System;

namespace SistemaBiblioteca
{
    // Excepción específica que se lanza cuando se intenta registrar
    // un libro con un ISBN que el conjunto de control ya contiene.
    public class IsbnDuplicadoException : Exception
    {
        public IsbnDuplicadoException(string isbn)
            : base($"El ISBN '{isbn}' ya existe en el catálogo. No se permiten duplicados.")
        {
        }
    }
}
