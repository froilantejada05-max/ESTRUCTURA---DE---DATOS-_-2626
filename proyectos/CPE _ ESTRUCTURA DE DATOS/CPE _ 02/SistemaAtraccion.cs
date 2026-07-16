using System;
using System.Collections.Generic;

namespace SistemaAtraccionParque
{
    // Clase que representa a una persona en la cola de espera
    class Persona
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public int NumeroTicket { get; private set; }
        public DateTime HoraLlegada { get; private set; }

        public Persona(int id, string nombre, int numeroTicket)
        {
            Id = id;
            Nombre = nombre;
            NumeroTicket = numeroTicket;
            HoraLlegada = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[Ticket #{NumeroTicket}] {Nombre} - Llegada: {HoraLlegada:HH:mm:ss}";
        }
    }

    // Clase que encapsula la estructura Queue<Persona>
    class ColaDePasajeros
    {
        private Queue<Persona> _cola;

        public int Cantidad => _cola.Count;

        public ColaDePasajeros()
        {
            _cola = new Queue<Persona>();
        }

        // Inserta una persona al final de la cola (O(1))
        public void Encolar(Persona persona)
        {
            _cola.Enqueue(persona);
        }

        // Extrae la persona del frente de la cola - FIFO (O(1))
        public Persona Desencolar()
        {
            if (_cola.Count == 0)
                throw new InvalidOperationException("La cola esta vacia. No hay personas en espera.");
            return _cola.Dequeue();
        }

        public bool EstaVacia() => _cola.Count == 0;

        // Permite iterar la cola sin modificarla - para reporteria
        public IEnumerable<Persona> ObtenerPersonas() => _cola;
    }

    // Clase principal que gestiona el sistema de la atraccion
    class SistemaAtraccion
    {
        private ColaDePasajeros _cola;
        private List<Persona> _abordados;
        private const int TOTAL_ASIENTOS = 30;
        private int _asientosDisponibles;
        private int _contadorId;
        private int _contadorTicket;

        public SistemaAtraccion()
        {
            _cola = new ColaDePasajeros();
            _abordados = new List<Persona>();
            _asientosDisponibles = TOTAL_ASIENTOS;
            _contadorId = 1;
            _contadorTicket = 1000;
        }

        // Registra una nueva persona en la cola
        public void RegistrarPersona(string nombre)
        {
            var persona = new Persona(_contadorId++, nombre, _contadorTicket++);
            _cola.Encolar(persona);
            Console.WriteLine($"\n  [OK] {persona.Nombre} registrado con Ticket #{persona.NumeroTicket}.");
            Console.WriteLine($"       Posicion en cola: #{_cola.Cantidad}");
        }

        // Procesa el abordaje del siguiente en la cola (FIFO)
        public void ProcesarAbordaje()
        {
            if (_asientosDisponibles == 0)
            {
                Console.WriteLine("\n  [!] Todos los 30 asientos han sido asignados. La atraccion esta llena.");
                return;
            }

            if (_cola.EstaVacia())
            {
                Console.WriteLine("\n  [!] No hay personas en la cola de espera.");
                return;
            }

            Persona persona = _cola.Desencolar();
            int asientoAsignado = TOTAL_ASIENTOS - _asientosDisponibles + 1;
            _asientosDisponibles--;
            _abordados.Add(persona);

            Console.WriteLine($"\n  [OK] {persona.Nombre} (Ticket #{persona.NumeroTicket}) ha abordado.");
            Console.WriteLine($"       Asiento asignado: #{asientoAsignado} de {TOTAL_ASIENTOS}");
            Console.WriteLine($"       Asientos restantes: {_asientosDisponibles} | En cola: {_cola.Cantidad}");
        }

        // Muestra las personas actualmente en espera - Reporteria
        public void MostrarColaDespera()
        {
            Console.WriteLine("\n  ======= COLA DE ESPERA ACTUAL =======");
            if (_cola.EstaVacia())
            {
                Console.WriteLine("  La cola esta vacia.");
                return;
            }

            int posicion = 1;
            foreach (var p in _cola.ObtenerPersonas())
            {
                Console.WriteLine($"  {posicion++,2}. {p}");
            }
            Console.WriteLine($"  --> Total en espera: {_cola.Cantidad} persona(s)");
        }

        // Muestra las personas que ya abordaron - Reporteria
        public void MostrarAbordados()
        {
            Console.WriteLine("\n  ======= PERSONAS QUE YA ABORDARON =======");
            if (_abordados.Count == 0)
            {
                Console.WriteLine("  Nadie ha abordado aun.");
                return;
            }

            int asiento = 1;
            foreach (var p in _abordados)
            {
                Console.WriteLine($"  Asiento #{asiento++,2}: {p}");
            }
            Console.WriteLine($"  --> Total abordados: {_abordados.Count} de {TOTAL_ASIENTOS}");
        }

        // Reporte completo del estado del sistema - Reporteria
        public void MostrarReporteCompleto()
        {
            Console.WriteLine("\n  +---------------------------------------+");
            Console.WriteLine("  |       REPORTE COMPLETO DEL SISTEMA    |");
            Console.WriteLine("  +---------------------------------------+");
            Console.WriteLine($"  | Total de asientos:       {TOTAL_ASIENTOS,3}           |");
            Console.WriteLine($"  | Asientos ocupados:       {_abordados.Count,3}           |");
            Console.WriteLine($"  | Asientos disponibles:    {_asientosDisponibles,3}           |");
            Console.WriteLine($"  | Personas en cola:        {_cola.Cantidad,3}           |");
            Console.WriteLine($"  | Porcentaje ocupacion:    {((double)_abordados.Count / TOTAL_ASIENTOS * 100),5:F1}%        |");
            string estado = _asientosDisponibles == 0 ? "LLENA     " : "DISPONIBLE";
            Console.WriteLine($"  | Estado de la atraccion:  {estado}      |");
            Console.WriteLine("  +---------------------------------------+");

            if (_abordados.Count > 0)
                MostrarAbordados();
            if (!_cola.EstaVacia())
                MostrarColaDespera();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var sistema = new SistemaAtraccion();
            bool ejecutando = true;

            Console.WriteLine("\n  +===========================================+");
            Console.WriteLine("  |  SISTEMA DE COLA - ATRACCION PARQUE FUN  |");
            Console.WriteLine("  |  Implementacion: Cola FIFO en C# con POO  |");
            Console.WriteLine("  |  Capacidad maxima: 30 asientos            |");
            Console.WriteLine("  +===========================================+");

            while (ejecutando)
            {
                Console.WriteLine("\n  ------- MENU PRINCIPAL -------");
                Console.WriteLine("  1. Registrar persona en cola");
                Console.WriteLine("  2. Procesar abordaje (siguiente en cola)");
                Console.WriteLine("  3. Ver cola de espera actual");
                Console.WriteLine("  4. Ver personas que ya abordaron");
                Console.WriteLine("  5. Ver reporte completo");
                Console.WriteLine("  6. Salir");
                Console.Write("\n  Seleccione una opcion: ");

                string opcion = Console.ReadLine()?.Trim() ?? "";

                switch (opcion)
                {
                    case "1":
                        Console.Write("  Nombre de la persona: ");
                        string nombre = Console.ReadLine()?.Trim() ?? "";
                        if (!string.IsNullOrWhiteSpace(nombre))
                            sistema.RegistrarPersona(nombre);
                        else
                            Console.WriteLine("  [!] Nombre invalido. Intente de nuevo.");
                        break;

                    case "2":
                        sistema.ProcesarAbordaje();
                        break;

                    case "3":
                        sistema.MostrarColaDespera();
                        break;

                    case "4":
                        sistema.MostrarAbordados();
                        break;

                    case "5":
                        sistema.MostrarReporteCompleto();
                        break;

                    case "6":
                        ejecutando = false;
                        Console.WriteLine("\n  Sistema cerrado. Hasta luego!\n");
                        break;

                    default:
                        Console.WriteLine("  [!] Opcion no valida. Elija entre 1 y 6.");
                        break;
                }
            }
        }
    }
}
