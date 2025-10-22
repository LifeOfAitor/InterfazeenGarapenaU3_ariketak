using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;


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

            CargarReservasJson();

            reservaVehiculo.Vehiculo = tipoVehiculo;

            AulkiakSortu();

            var asientosOcupados = ObtenerReservasParaFecha(fechaReserva);

            foreach (var child in (Content as Grid).Children)
            {
                if (child is libreriaVehiculos.SillaControl silla)
                {
                    if (asientosOcupados.Contains(silla.Asiento.NumeroAsiento))
                    {
                        silla.Asiento.Estado = libreriaVehiculos.EstadoAsiento.Ocupado;
                    }
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
                    (Content as Grid).Children.Add(silla);
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
            for (int i = 0; i < (Content as Grid).Children.Count; i++)
            {
                if ((Content as Grid).Children[i] is libreriaVehiculos.SillaControl silla)
                {
                    if (silla.Asiento.Estado == libreriaVehiculos.EstadoAsiento.Seleccionado)
                    {
                        txt_box_erreserbatutako_aulkia.Text += silla.Asiento.NumeroAsiento + " ";
                    }
                }
            }
        }

        private List<int> ObtenerAsientosSeleccionados()
        {
            var seleccionados = new List<int>();

            foreach (var child in (Content as Grid).Children)
            {
                if (child is libreriaVehiculos.SillaControl silla && silla.Asiento.Estado == libreriaVehiculos.EstadoAsiento.Seleccionado)
                {
                    seleccionados.Add(silla.Asiento.NumeroAsiento);
                }
            }

            return seleccionados;
        }
        private List<int> ObtenerReservasParaFecha(DateTime fecha)
        {
            if (!reservaVehiculo.ReservasPorFecha.ContainsKey(fecha))
            {
                int cantidadReservas = random.Next(0, 5); // entre 0 y 4 reservas aleatorias
                var reservados = new List<int>();

                while (reservados.Count < cantidadReservas)
                {
                    int asiento = random.Next(1, TOTAL_ASIENTOS + 1);
                    if (!reservados.Contains(asiento))
                    {
                        reservados.Add(asiento);
                    }
                }

                reservaVehiculo.ReservasPorFecha[fecha] = reservados;
                GuardarReservasJson();
            }

            return reservaVehiculo.ReservasPorFecha[fecha];
        }


        private void CambiarEstadoAsientos(List<int> asientos, libreriaVehiculos.EstadoAsiento nuevoEstado)
        {
            foreach (var child in (Content as Grid).Children)
            {
                if (child is libreriaVehiculos.SillaControl silla && asientos.Contains(silla.Asiento.NumeroAsiento))
                {
                    silla.Asiento.Estado = nuevoEstado;
                }
            }
        }

        private bool ConfirmarReserva(List<int> asientosSeleccionados)
        {
            string mensaje = "Aulki hauek aukeratu dituzu: " + string.Join(", ", asientosSeleccionados) + "\nBenetan erreserbatu nahi dituzu?";
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
                CambiarEstadoAsientos(asientosSeleccionados, libreriaVehiculos.EstadoAsiento.Ocupado);
                MessageBox.Show("Erreserba konfirmatua.", "Primeran", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                CambiarEstadoAsientos(asientosSeleccionados, libreriaVehiculos.EstadoAsiento.Libre);
            }

            mezuaErakutsi(); // Actualizar el TextBox con los asientos actuales
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CargarReservasJson()
        {
            if (File.Exists(RUTA_JSON))
            {
                string json = File.ReadAllText(RUTA_JSON);
                reservaVehiculo = JsonSerializer.Deserialize<ReservaVehiculo>(json);
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



    }


}
