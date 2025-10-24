using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace ariketa1
{
    /// <summary>
    /// Interaction logic for autobusa_window.xaml
    /// </summary>
    public partial class autobusa_window : Window
    {
        private const int TOTAL_ASIENTOS = 10;
        private const string RUTA_JSON = "reservas.json";
        private ReservaVehiculo reservaVehiculo = new ReservaVehiculo();
        private Random random = new Random();
        private DateTime fechaReserva;

        public autobusa_window(DateTime fechaReserva, string tipoVehiculo)
        {
            InitializeComponent();

            this.fechaReserva = fechaReserva;

            // Cargar reservas guardadas
            CargarReservasJson();

            reservaVehiculo.Vehiculo = tipoVehiculo;

            // Crear los asientos en la interfaz
            AulkiakSortu();

            // Obtener asientos ocupados para la fecha y actualizar visualmente
            var asientosOcupados = ObtenerReservasParaFecha(fechaReserva);
            foreach (var child in grida.Children)
            {
                if (child is libreriaVehiculos.SillaControl silla &&
                    asientosOcupados.Contains(silla.Asiento.NumeroAsiento))
                {
                    silla.Asiento.Estado = libreriaVehiculos.EstadoAsiento.Ocupado;
                }
            }

            mezuaErakutsi();
        }

        private void AulkiakSortu()
        {
            int asientoNumero = 1;
            for (int fila = 0; fila < 5; fila++)
            {
                for (int columna = 0; columna < 2; columna++)
                {
                    var silla = new libreriaVehiculos.SillaControl();
                    silla.Asiento.NumeroAsiento = asientoNumero++;

                    silla.Margin = new Thickness(39 + columna * 60, 104 + fila * 50, 0, 0);
                    silla.HorizontalAlignment = HorizontalAlignment.Left;
                    silla.VerticalAlignment = VerticalAlignment.Top;

                    silla.EstadoCambiado += Silla_EstadoCambiado;

                    Grid.SetZIndex(silla, 1);
                    grida.Children.Add(silla);
                }
            }
        }

        private void Silla_EstadoCambiado(object sender, EventArgs e)
        {
            mezuaErakutsi();
        }

        public void mezuaErakutsi()
        {
            txt_box_erreserbatutako_aulkia.Text = "";
            foreach (var child in grida.Children)
            {
                if (child is libreriaVehiculos.SillaControl silla &&
                    silla.Asiento.Estado == libreriaVehiculos.EstadoAsiento.Seleccionado)
                {
                    txt_box_erreserbatutako_aulkia.Text += silla.Asiento.NumeroAsiento + " ";
                }
            }
        }

        private List<int> ObtenerAsientosSeleccionados()
        {
            var seleccionados = new List<int>();

            foreach (var child in grida.Children)
            {
                if (child is libreriaVehiculos.SillaControl silla &&
                    silla.Asiento.Estado == libreriaVehiculos.EstadoAsiento.Seleccionado)
                {
                    seleccionados.Add(silla.Asiento.NumeroAsiento);
                }
            }

            return seleccionados;
        }

        private void CambiarEstadoAsientos(List<int> asientos, libreriaVehiculos.EstadoAsiento nuevoEstado)
        {
            foreach (var child in grida.Children)
            {
                if (child is libreriaVehiculos.SillaControl silla &&
                    asientos.Contains(silla.Asiento.NumeroAsiento))
                {
                    silla.Asiento.Estado = nuevoEstado;
                }
            }
        }

        #region Gestión de reservas

        private List<int> ObtenerReservasParaFecha(DateTime fecha)
        {
            var asientosOcupados = reservaVehiculo.ObtenerAsientosReservados(fecha);

            if (asientosOcupados.Count == 0)
            {
                // Generar algunas reservas aleatorias si no hay ninguna
                int cantidadReservas = random.Next(0, 5);
                var reservados = new List<int>();

                while (reservados.Count < cantidadReservas)
                {
                    int asiento = random.Next(1, TOTAL_ASIENTOS + 1);
                    if (!reservados.Contains(asiento))
                        reservados.Add(asiento);
                }

                reservaVehiculo.AgregarReservas(fecha, reservados);
                GuardarReservasJson();
                return reservados;
            }

            return asientosOcupados;
        }

        private bool ConfirmarReserva(List<int> asientosSeleccionados)
        {
            string mensaje = "Aulki hauek aukeratu dituzu: " + string.Join(", ", asientosSeleccionados) +
                             "\nBenetan erreserbatu nahi dituzu?";
            var resultado = MessageBox.Show(mensaje, "Konfirmatu erreserba", MessageBoxButton.YesNo, MessageBoxImage.Question);

            return resultado == MessageBoxResult.Yes;
        }

        private void btn_erreserbatu_Click(object sender, RoutedEventArgs e)
        {
            var asientosSeleccionados = ObtenerAsientosSeleccionados();

            if (asientosSeleccionados.Count == 0)
            {
                MessageBox.Show("Ez duzu aulkirik aukeratu.", "Kontuz", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ConfirmarReserva(asientosSeleccionados))
            {
                // Cambiar estado visual
                CambiarEstadoAsientos(asientosSeleccionados, libreriaVehiculos.EstadoAsiento.Ocupado);

                // Guardar reservas en la clase y persistir en JSON
                reservaVehiculo.AgregarReservas(fechaReserva, asientosSeleccionados);
                GuardarReservasJson();

                MessageBox.Show("Erreserba konfirmatua.", "Primeran", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                CambiarEstadoAsientos(asientosSeleccionados, libreriaVehiculos.EstadoAsiento.Libre);
            }

            mezuaErakutsi(); // Actualizar TextBox
        }

        #endregion

        #region Guardar y cargar JSON

        private void CargarReservasJson()
        {
            if (File.Exists(RUTA_JSON))
            {
                try
                {
                    string json = File.ReadAllText(RUTA_JSON);
                    reservaVehiculo = JsonSerializer.Deserialize<ReservaVehiculo>(json) ?? new ReservaVehiculo();
                }
                catch
                {
                    reservaVehiculo = new ReservaVehiculo();
                }
            }
            else
            {
                reservaVehiculo = new ReservaVehiculo();
                GuardarReservasJson();
            }
        }

        private void GuardarReservasJson()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(reservaVehiculo, options);
            File.WriteAllText(RUTA_JSON, json);
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
