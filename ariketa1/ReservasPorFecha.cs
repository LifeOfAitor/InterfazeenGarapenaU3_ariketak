using System;
using System.Collections.Generic;

public class ReservasPorFecha
{
    // Guarda las reservas por fecha en formato string (ISO)
    public Dictionary<string, List<int>> Reservas { get; set; } = new();
}

