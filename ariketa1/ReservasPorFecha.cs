using System;
using System.Collections.Generic;

public class ReservasPorFecha
{
    public Dictionary<DateTime, List<int>> Reservas { get; set; } = new Dictionary<DateTime, List<int>>();
}
