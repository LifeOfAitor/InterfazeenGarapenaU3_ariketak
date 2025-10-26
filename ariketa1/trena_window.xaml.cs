using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace ariketa1
{
    public partial class trena_window : Window
    {
        private const int TOTAL_ASIENTOS = 28;
        private const string RUTA_JSON = "trena.json";

        private ReservaVehiculo reservaVehiculo;
        private readonly Random random = new();
        private readonly DateTime fechaReserva;

        public trena_window(DateTime fechaReserva, string tipoVehiculo)
        {
            InitializeComponent();
            this.fechaReserva = fechaReserva;

            // erreserbak kargatu JSON fitxategitik
            reservaVehiculo = IbilgailuenKlaseak.CargarReservasJson(RUTA_JSON);

            reservaVehiculo.Vehiculo = tipoVehiculo;

            //aulkiak sortzeko
            IbilgailuenKlaseak.AulkiakSortu(
                lerroak: 7,
                zutabeak: 4,
                grida: grida,
                sillaEstadoCambiadoHandler: Silla_EstadoCambiado
            );

            //Ezarritako datarako erreserbatuta dauden aulkiak kargatu
            var asientosOcupados = IbilgailuenKlaseak.ObtenerReservasParaFecha(
                fechaReserva,
                reservaVehiculo,
                random,
                TOTAL_ASIENTOS,
                RUTA_JSON
            );

            IbilgailuenKlaseak.CambiarEstadoAsientos(grida, asientosOcupados, libreriaVehiculos.EstadoAsiento.Ocupado);

            ActualizarTextoAsientosSeleccionados();
        }

        private void Silla_EstadoCambiado(object sender, EventArgs e)
        {
            ActualizarTextoAsientosSeleccionados();
        }

        private void ActualizarTextoAsientosSeleccionados()
        {
            txt_box_erreserbatutako_aulkia.Text = IbilgailuenKlaseak.ObtenerAsientosSeleccionados(grida);
        }

        private void btn_erreserbatu_Click(object sender, RoutedEventArgs e)
        {
            IbilgailuenKlaseak.ProcesarReserva(
                grida,
                reservaVehiculo,
                fechaReserva,
                RUTA_JSON
            );

            ActualizarTextoAsientosSeleccionados();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}

