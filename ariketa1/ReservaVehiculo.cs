using System;
using System.Collections.Generic;

public class ReservaVehiculo
{
    public string Vehiculo { get; set; } = "Autobus";

    public Dictionary<DateTime, List<int>> ReservasPorFecha { get; set; } = new Dictionary<DateTime, List<int>>();
}
