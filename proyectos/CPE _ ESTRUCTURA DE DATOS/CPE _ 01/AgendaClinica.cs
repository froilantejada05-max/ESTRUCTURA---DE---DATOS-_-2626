/*
 * Guia de Practicas #01 - Estructura de Datos
 * Universidad Estatal Amazonica
 * Caso: Agenda de turnos de pacientes de una clinica
 * Lenguaje: C#
 * Estructuras utilizadas: struct (Paciente, Turno) + List<Turno>
 */

using System;
using System.Collections.Generic;

namespace AgendaClinicaApp
{
    // ============================================================
    // ESTRUCTURA 1: Paciente
    // Registro con los datos personales del paciente
    // ============================================================
    struct Paciente
    {
        public long   Cedula;
        public string Nombre;
        public string Apellido;
        public int    Edad;
        public string Telefono;
    }

    // ============================================================
    // ESTRUCTURA 2: Turno
    // Registro que representa una cita medica
    // Contiene un Paciente por composicion
    // ============================================================
    struct Turno
    {
        public int      IdTurno;
        public Paciente Paciente;   // struct anidado
        public string   Fecha;
        public string   Hora;
        public string   Especialidad;
        public string   Medico;
        public string   Estado;     // "Pendiente", "Atendido", "Cancelado"
    }

    // ============================================================
    // CLASE: AgendaClinica
    // Gestiona la List<Turno> y expone las operaciones CRUD
    // ============================================================
    class AgendaClinica
    {
        private List<Turno> turnos;        // LISTA dinamica de turnos
        private int    contadorId;
        private string nombreClinica;

        public AgendaClinica(string nombre)
        {
            turnos = new List<Turno>();
            contadorId = 1;
            nombreClinica = nombre;
        }

        private void MostrarLinea(char c = '=', int n = 70)
        {
            Console.WriteLine(new string(c, n));
        }

        // -------- OPERACION 1: Agregar turno --------
        public void AgregarTurno()
        {
            Paciente p = new Paciente();
            Turno    t = new Turno();

            Console.WriteLine("\n--- Registro de Nuevo Turno ---");
            Console.Write("Cedula del paciente : "); p.Cedula = long.Parse(Console.ReadLine());
            Console.Write("Nombre              : "); p.Nombre = Console.ReadLine();
            Console.Write("Apellido            : "); p.Apellido = Console.ReadLine();
            Console.Write("Edad                : "); p.Edad = int.Parse(Console.ReadLine());
            Console.Write("Telefono            : "); p.Telefono = Console.ReadLine();
            Console.Write("Fecha (DD/MM/AAAA)  : "); t.Fecha = Console.ReadLine();
            Console.Write("Hora  (HH:MM)       : "); t.Hora = Console.ReadLine();
            Console.Write("Especialidad        : "); t.Especialidad = Console.ReadLine();
            Console.Write("Nombre del medico   : "); t.Medico = Console.ReadLine();

            t.IdTurno  = contadorId++;
            t.Paciente = p;
            t.Estado   = "Pendiente";

            turnos.Add(t);
            Console.WriteLine($"\n[OK] Turno registrado. ID asignado: {t.IdTurno}");
        }

        // -------- OPERACION 2: Listar todos los turnos --------
        public void ListarTurnos()
        {
            MostrarLinea('=', 70);
            Console.WriteLine($"  CLINICA: {nombreClinica}  |  Total turnos: {turnos.Count}");
            MostrarLinea('=', 70);

            if (turnos.Count == 0)
            {
                Console.WriteLine("  No existen turnos registrados.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-22} {2,-12} {3,-7} {4,-18} {5,-10}",
                "ID", "Paciente", "Fecha", "Hora", "Medico", "Estado");
            MostrarLinea('-', 70);

            foreach (Turno t in turnos)
            {
                string nombreCompleto = t.Paciente.Nombre + " " + t.Paciente.Apellido;
                Console.WriteLine("{0,-5} {1,-22} {2,-12} {3,-7} {4,-18} {5,-10}",
                    t.IdTurno, nombreCompleto, t.Fecha, t.Hora, t.Medico, t.Estado);
            }
            MostrarLinea('-', 70);
        }

        // -------- OPERACION 3a: Buscar por cedula --------
        public void BuscarPorCedula(long cedula)
        {
            Console.WriteLine($"\n--- Busqueda por cedula: {cedula} ---");
            bool encontrado = false;
            foreach (Turno t in turnos)
            {
                if (t.Paciente.Cedula == cedula)
                {
                    Console.WriteLine($"ID: {t.IdTurno} | {t.Paciente.Nombre} {t.Paciente.Apellido} | " +
                        $"{t.Especialidad} | Fecha: {t.Fecha}  {t.Hora} | Dr. {t.Medico} | Estado: {t.Estado}");
                    encontrado = true;
                }
            }
            if (!encontrado)
                Console.WriteLine("[!] No se encontro ningun turno para esa cedula.");
        }

        // -------- OPERACION 3b: Buscar por medico --------
        public void BuscarPorMedico(string medico)
        {
            Console.WriteLine($"\n--- Turnos del medico: {medico} ---");
            bool encontrado = false;
            foreach (Turno t in turnos)
            {
                if (t.Medico == medico)
                {
                    Console.WriteLine($"ID: {t.IdTurno} | C.I: {t.Paciente.Cedula} | " +
                        $"{t.Paciente.Nombre} {t.Paciente.Apellido} | {t.Fecha}  {t.Hora} | Estado: {t.Estado}");
                    encontrado = true;
                }
            }
            if (!encontrado)
                Console.WriteLine("[!] No hay turnos asignados a ese medico.");
        }

        public int GetTotalTurnos() => turnos.Count;
    }

    // ============================================================
    // PROGRAMA PRINCIPAL - MENU INTERACTIVO
    // ============================================================
    class Program
    {
        static void Main(string[] args)
        {
            AgendaClinica agenda = new AgendaClinica("SaludVida");
            int opcion;

            do
            {
                Console.WriteLine();
                Console.WriteLine("========================================");
                Console.WriteLine("  AGENDA DE TURNOS - CLINICA SALUDVIDA  ");
                Console.WriteLine("========================================");
                Console.WriteLine(" 1. Registrar nuevo turno");
                Console.WriteLine(" 2. Listar todos los turnos");
                Console.WriteLine(" 3. Buscar turno por cedula");
                Console.WriteLine(" 4. Buscar turnos por medico");
                Console.WriteLine(" 0. Salir");
                Console.WriteLine("----------------------------------------");
                Console.Write(" Opcion: ");
                opcion = int.Parse(Console.ReadLine());

                switch (opcion)
                {
                    case 1:
                        agenda.AgregarTurno();
                        break;
                    case 2:
                        agenda.ListarTurnos();
                        break;
                    case 3:
                        Console.Write("Ingrese la cedula: ");
                        long ced = long.Parse(Console.ReadLine());
                        agenda.BuscarPorCedula(ced);
                        break;
                    case 4:
                        Console.Write("Nombre del medico: ");
                        string med = Console.ReadLine();
                        agenda.BuscarPorMedico(med);
                        break;
                    case 0:
                        Console.WriteLine("\nCerrando el sistema. Hasta pronto.");
                        break;
                    default:
                        Console.WriteLine("[!] Opcion no valida. Intente nuevamente.");
                        break;
                }
            } while (opcion != 0);
        }
    }
}
