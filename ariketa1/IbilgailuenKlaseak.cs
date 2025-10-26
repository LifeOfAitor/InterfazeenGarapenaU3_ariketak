using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace ariketa1
{
    internal static class IbilgailuenKlaseak
    {
        //aulkiak griean ezartzeko balio digu
        public static void AulkiakSortu(int lerroak, int zutabeak, Grid grida, EventHandler sillaEstadoCambiadoHandler)
        {
            int asientoNumero = 1;

            for (int fila = 0; fila < zutabeak; fila++)
            {
                for (int columna = 0; columna < lerroak; columna++)
                {
                    var silla = new libreriaVehiculos.SillaControl
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(39 + columna * 60, 104 + fila * 50, 0, 0)
                    };

                    silla.Asiento.NumeroAsiento = asientoNumero++;
                    silla.EstadoCambiado += sillaEstadoCambiadoHandler;

                    Grid.SetZIndex(silla, 1);
                    grida.Children.Add(silla);
                }
            }
        }

        //Aukeratuta dauden aulkiak lortzeko balio digu string batean
        public static string ObtenerAsientosSeleccionados(Grid grida)
        {
            var seleccionados = grida.Children
                .OfType<libreriaVehiculos.SillaControl>()
                .Where(s => s.Asiento.Estado == libreriaVehiculos.EstadoAsiento.Seleccionado)
                .Select(s => s.Asiento.NumeroAsiento.ToString());

            return string.Join(" ", seleccionados);
        }

        //Aulki baten egoera aldatzeko balio digu
        public static void CambiarEstadoAsientos(Grid grida, List<int> asientos, libreriaVehiculos.EstadoAsiento nuevoEstado)
        {
            foreach (var silla in grida.Children.OfType<libreriaVehiculos.SillaControl>())
            {
                if (asientos.Contains(silla.Asiento.NumeroAsiento))
                    silla.Asiento.Estado = nuevoEstado;
            }
        }

        //Data baterako erreserbak lortzeko balio digu String dictionary batean
        // Bezeroak data baz ezartzen duenen, automatikoki sortuko dira ibilgailuaren arabera erreserbak, erabilgarritasuna simulatzeko
        public static List<int> ObtenerReservasParaFecha(
        DateTime fecha,
        ReservaVehiculo reservaVehiculo,
        Random random,
        int totalAsientos,
        string rutaJson)
        {
            string clave = fecha.ToShortDateString();

            if (!reservaVehiculo.ReservasPorFecha.ContainsKey(clave))
            {
                // ibilgailuaren arabera erreserbak sortu
                int cantidad;
                if (reservaVehiculo.Vehiculo == "Autobusa")
                    cantidad = random.Next(1, 5); // 1-4 
                else if (reservaVehiculo.Vehiculo == "Trena")
                    cantidad = random.Next(3, 10); // 3-9 
                else
                    cantidad = random.Next(6, 15); // 6-14 

                var reservados = new HashSet<int>();

                while (reservados.Count < cantidad)
                {
                    reservados.Add(random.Next(1, totalAsientos + 1));
                }

                reservaVehiculo.ReservasPorFecha[clave] = reservados.ToList();
                GuardarReservasJson(rutaJson, reservaVehiculo);
            }

            return reservaVehiculo.ReservasPorFecha[clave];
        }


        // Erreserba prozesatzeko balio digu
        public static void ProcesarReserva(Grid grida, ReservaVehiculo reservaVehiculo, DateTime fechaReserva, string rutaJson)
        {
            var seleccionados = grida.Children
                .OfType<libreriaVehiculos.SillaControl>()
                .Where(s => s.Asiento.Estado == libreriaVehiculos.EstadoAsiento.Seleccionado)
                .Select(s => s.Asiento.NumeroAsiento)
                .ToList();

            if (!seleccionados.Any())
            {
                MessageBox.Show("Ez duzu aulkirik aukeratu.", "Kontuz", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string mensaje = $"Aulki hauek aukeratu dituzu: {string.Join(", ", seleccionados)}\nBenetan erreserbatu nahi dituzu?";
            if (MessageBox.Show(mensaje, "Konfirmatu erreserba", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CambiarEstadoAsientos(grida, seleccionados, libreriaVehiculos.EstadoAsiento.Ocupado);

                string clave = fechaReserva.ToShortDateString();

                // data ez bada existitzen, sortu zerrenda berria
                if (!reservaVehiculo.ReservasPorFecha.ContainsKey(clave))
                    reservaVehiculo.ReservasPorFecha[clave] = new List<int>();

                //erreserbatuta zeuden aulkiei gehitu aukeratu direnak elkarrekin gordetzeko
                reservaVehiculo.ReservasPorFecha[clave] = reservaVehiculo.ReservasPorFecha[clave]
                    .Union(seleccionados)
                    .ToList();


                GuardarReservasJson(rutaJson, reservaVehiculo);
                MessageBox.Show("Erreserba konfirmatua.", "Primeran", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                CambiarEstadoAsientos(grida, seleccionados, libreriaVehiculos.EstadoAsiento.Libre);
            }
        }

        //json fitxategitik kargatuko ditugu erreserbak bere ruta emanez
        public static ReservaVehiculo CargarReservasJson(string ruta)
        {
            try
            {
                if (File.Exists(ruta))
                {
                    string json = File.ReadAllText(ruta);
                    return JsonSerializer.Deserialize<ReservaVehiculo>(json) ?? new ReservaVehiculo();
                }
            }
            catch { }

            return new ReservaVehiculo();
        }

        //json fitxategian gordetzeko balio digu
        public static void GuardarReservasJson(string ruta, ReservaVehiculo reservaVehiculo)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(reservaVehiculo, options);
            File.WriteAllText(ruta, json);
        }
    }
}
