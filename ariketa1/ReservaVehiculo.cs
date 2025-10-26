using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

public class ReservaVehiculo
{
    // Tipo de vehículo (Autobús, Tren, Avión...)
    public string Vehiculo { get; set; } = "Autobus";

    // Guardamos las reservas con la fecha en formato string ISO (más seguro para JSON)
    public Dictionary<string, List<int>> ReservasPorFecha { get; set; } = new();
}
