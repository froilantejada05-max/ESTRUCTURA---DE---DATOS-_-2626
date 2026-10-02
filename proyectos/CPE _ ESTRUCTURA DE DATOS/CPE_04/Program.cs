using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace VuelosBaratosED
{
    // Representa una conexion (arista) del grafo: aeropuerto destino y precio en USD
    public class Vuelo
    {
        public string Destino { get; set; }
        public double Precio { get; set; }

        public Vuelo(string destino, double precio)
        {
            Destino = destino;
            Precio = precio;
        }
    }

    // Grafo no dirigido y ponderado, representado mediante lista de adyacencia
    public class GrafoVuelos
    {
        private Dictionary<string, List<Vuelo>> listaAdyacencia;

        public GrafoVuelos()
        {
            listaAdyacencia = new Dictionary<string, List<Vuelo>>();
        }

        // Agrega un aeropuerto (nodo) vacio si todavia no existe
        public void AgregarAeropuerto(string codigo)
        {
            if (!listaAdyacencia.ContainsKey(codigo))
                listaAdyacencia[codigo] = new List<Vuelo>();
        }

        // Agrega una ruta en ambos sentidos (grafo no dirigido)
        public void AgregarRuta(string origen, string destino, double precio)
        {
            AgregarAeropuerto(origen);
            AgregarAeropuerto(destino);
            listaAdyacencia[origen].Add(new Vuelo(destino, precio));
            listaAdyacencia[destino].Add(new Vuelo(origen, precio));
        }

        public IEnumerable<string> Aeropuertos => listaAdyacencia.Keys;

        public int NumeroAeropuertos => listaAdyacencia.Count;

        public int NumeroRutas => listaAdyacencia.Values.Sum(l => l.Count) / 2;

        // Reporteria: imprime todos los aeropuertos registrados
        public void ReportarAeropuertos()
        {
            Console.WriteLine("\n--- AEROPUERTOS REGISTRADOS ---");
            foreach (var codigo in listaAdyacencia.Keys.OrderBy(c => c))
                Console.WriteLine(" * " + codigo);
            Console.WriteLine("Total: " + NumeroAeropuertos + " aeropuertos");
        }

        // Reporteria: imprime la lista de adyacencia completa (todas las rutas)
        public void ReportarRutas()
        {
            Console.WriteLine("\n--- LISTA DE ADYACENCIA (RUTAS DISPONIBLES) ---");
            foreach (var origen in listaAdyacencia.Keys.OrderBy(c => c))
            {
                var conexiones = listaAdyacencia[origen]
                    .OrderBy(v => v.Destino)
                    .Select(v => $"{v.Destino} (${v.Precio})");
                Console.WriteLine($" {origen} -> " + string.Join(", ", conexiones));
            }
            Console.WriteLine("Total: " + NumeroRutas + " rutas (aristas no dirigidas)");
        }

        // Algoritmo de Dijkstra: retorna el costo minimo y la ruta desde origen hasta destino
        public (double costo, List<string> ruta, int nodosProcesados) CaminoMasBarato(string origen, string destino)
        {
            var distancias = new Dictionary<string, double>();
            var predecesor = new Dictionary<string, string>();
            var visitados = new HashSet<string>();

            foreach (var nodo in listaAdyacencia.Keys)
                distancias[nodo] = double.PositiveInfinity;

            if (!listaAdyacencia.ContainsKey(origen) || !listaAdyacencia.ContainsKey(destino))
                throw new ArgumentException("Aeropuerto de origen o destino no existe en el grafo.");

            distancias[origen] = 0;

            // Cola de prioridad simulada con SortedSet de tuplas (precio acumulado, codigo)
            var colaPrioridad = new SortedSet<(double precio, string nodo)>(
                Comparer<(double precio, string nodo)>.Create((a, b) =>
                {
                    int cmp = a.precio.CompareTo(b.precio);
                    return cmp != 0 ? cmp : string.Compare(a.nodo, b.nodo, StringComparison.Ordinal);
                }));

            colaPrioridad.Add((0, origen));
            int nodosProcesados = 0;

            while (colaPrioridad.Count > 0)
            {
                var actual = colaPrioridad.Min;
                colaPrioridad.Remove(actual);
                string nodoActual = actual.nodo;

                if (visitados.Contains(nodoActual)) continue;
                visitados.Add(nodoActual);
                nodosProcesados++;

                if (nodoActual == destino) break;

                foreach (var vuelo in listaAdyacencia[nodoActual])
                {
                    double nuevaDistancia = distancias[nodoActual] + vuelo.Precio;
                    if (nuevaDistancia < distancias[vuelo.Destino])
                    {
                        distancias[vuelo.Destino] = nuevaDistancia;
                        predecesor[vuelo.Destino] = nodoActual;
                        colaPrioridad.Add((nuevaDistancia, vuelo.Destino));
                    }
                }
            }

            // Reconstruccion de la ruta a partir de los predecesores
            var ruta = new List<string>();
            string paso = destino;
            while (paso != null)
            {
                ruta.Insert(0, paso);
                predecesor.TryGetValue(paso, out paso);
            }

            if (ruta.Count == 0 || ruta[0] != origen)
                return (double.PositiveInfinity, new List<string>(), nodosProcesados);

            return (distancias[destino], ruta, nodosProcesados);
        }
    }

    class Program
    {
        static GrafoVuelos grafo = new GrafoVuelos();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CargarBaseDeDatos("vuelos.txt");

            int opcion;
            do
            {
                MostrarMenu();
                opcion = LeerOpcion();

                switch (opcion)
                {
                    case 1:
                        grafo.ReportarAeropuertos();
                        break;
                    case 2:
                        grafo.ReportarRutas();
                        break;
                    case 3:
                        BuscarVueloMasBarato();
                        break;
                    case 4:
                        EjecutarPruebasDemostrativas();
                        break;
                    case 0:
                        Console.WriteLine("\nSaliendo del sistema. Buen viaje!");
                        break;
                    default:
                        Console.WriteLine("\nOpcion no valida, intente nuevamente.");
                        break;
                }
            } while (opcion != 0);
        }

        static void MostrarMenu()
        {
            Console.WriteLine("\n==============================================");
            Console.WriteLine(" SISTEMA DE BUSQUEDA DE VUELOS BARATOS - ED2026");
            Console.WriteLine("==============================================");
            Console.WriteLine("1. Listar aeropuertos disponibles");
            Console.WriteLine("2. Listar todas las rutas (lista de adyacencia)");
            Console.WriteLine("3. Buscar el vuelo mas barato entre dos aeropuertos");
            Console.WriteLine("4. Ejecutar casos de prueba demostrativos");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opcion: ");
        }

        static int LeerOpcion()
        {
            string entrada = Console.ReadLine();
            return int.TryParse(entrada, out int valor) ? valor : -1;
        }

        static void CargarBaseDeDatos(string ruta)
        {
            if (!File.Exists(ruta))
            {
                Console.WriteLine("No se encontro el archivo " + ruta);
                return;
            }

            foreach (var linea in File.ReadAllLines(ruta))
            {
                string limpia = linea.Trim();
                if (string.IsNullOrEmpty(limpia) || limpia.StartsWith("#")) continue;

                var partes = limpia.Split(',');
                string origen = partes[0].Trim();
                string destino = partes[1].Trim();
                double precio = double.Parse(partes[2].Trim());

                grafo.AgregarRuta(origen, destino, precio);
            }

            Console.WriteLine($"Base de datos cargada: {grafo.NumeroAeropuertos} aeropuertos, {grafo.NumeroRutas} rutas.");
        }

        static void BuscarVueloMasBarato()
        {
            Console.Write("\nCodigo de aeropuerto de origen (ej. UIO): ");
            string origen = Console.ReadLine().Trim().ToUpper();
            Console.Write("Codigo de aeropuerto de destino (ej. XSH): ");
            string destino = Console.ReadLine().Trim().ToUpper();

            EjecutarConsulta(origen, destino);
        }

        // Ejecuta la consulta de Dijkstra midiendo el tiempo de ejecucion con Stopwatch
        static void EjecutarConsulta(string origen, string destino)
        {
            try
            {
                var cronometro = Stopwatch.StartNew();
                var (costo, ruta, nodosProcesados) = grafo.CaminoMasBarato(origen, destino);
                cronometro.Stop();

                Console.WriteLine("\n--- RESULTADO DE LA BUSQUEDA ---");
                if (ruta.Count == 0)
                {
                    Console.WriteLine($"No existe una ruta disponible entre {origen} y {destino}.");
                }
                else
                {
                    Console.WriteLine($"Origen:  {origen}");
                    Console.WriteLine($"Destino: {destino}");
                    Console.WriteLine($"Ruta mas barata: {string.Join(" -> ", ruta)}");
                    Console.WriteLine($"Costo total: ${costo}");
                    Console.WriteLine($"Escalas: {(ruta.Count - 2 < 0 ? 0 : ruta.Count - 2)}");
                }
                Console.WriteLine($"Nodos procesados por el algoritmo: {nodosProcesados}");
                Console.WriteLine($"Tiempo de ejecucion: {cronometro.Elapsed.TotalMilliseconds:F4} ms " +
                                   $"({cronometro.ElapsedTicks} ticks)");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // Casos de prueba fijos usados para el analisis de resultados del informe
        static void EjecutarPruebasDemostrativas()
        {
            Console.WriteLine("\n=== CASOS DE PRUEBA DEMOSTRATIVOS ===");
            EjecutarConsulta("MEC", "XSH");
            EjecutarConsulta("LOJ", "LGQ");
            EjecutarConsulta("UIO", "XSH");
        }
    }
}
