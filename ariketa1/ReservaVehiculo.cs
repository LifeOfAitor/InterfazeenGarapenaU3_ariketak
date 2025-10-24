using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

public class ReservaVehiculo
{
    // Tipo de vehículo (Autobús, Tren, Avión...)
    public string Vehiculo { get; set; } = "Autobus";

    // Guardamos las reservas con la fecha en formato string ISO (más seguro para JSON)
    // Ejemplo: "2025-10-24"
    public Dictionary<string, List<int>> ReservasPorFecha { get; set; } = new();

    // --- Métodos auxiliares ---

    // Devuelve la lista de asientos reservados para una fecha dada
    public List<int> ObtenerAsientosReservados(DateTime fecha)
    {
        string clave = fecha.ToString("yyyy-MM-dd");
        return ReservasPorFecha.ContainsKey(clave)
            ? new List<int>(ReservasPorFecha[clave])
            : new List<int>();
    }

    // Añade asientos a una fecha (sin duplicados)
    public void AgregarReservas(DateTime fecha, List<int> nuevosAsientos)
    {
        string clave = fecha.ToString("yyyy-MM-dd");

        if (!ReservasPorFecha.ContainsKey(clave))
        {
            ReservasPorFecha[clave] = new List<int>();
        }

        ReservasPorFecha[clave] = ReservasPorFecha[clave]
            .Union(nuevosAsientos)
            .ToList();
    }

    // Elimina una reserva concreta
    public void CancelarReserva(DateTime fecha, int numeroAsiento)
    {
        string clave = fecha.ToString("yyyy-MM-dd");

        if (ReservasPorFecha.ContainsKey(clave))
        {
            ReservasPorFecha[clave].Remove(numeroAsiento);

            // Si la lista queda vacía, puedes limpiar la fecha
            if (ReservasPorFecha[clave].Count == 0)
            {
                ReservasPorFecha.Remove(clave);
            }
        }
    }
}
