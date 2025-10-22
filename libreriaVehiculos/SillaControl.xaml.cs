using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace libreriaVehiculos
{
    public partial class SillaControl : UserControl
    {
        public Asiento Asiento { get; set; }
        public event EventHandler EstadoCambiado;

        private void SillaButton_Click(object sender, RoutedEventArgs e)
        {
            if (Asiento.Estado == EstadoAsiento.Libre)
                Asiento.Estado = EstadoAsiento.Seleccionado;
            else if (Asiento.Estado == EstadoAsiento.Seleccionado)
                Asiento.Estado = EstadoAsiento.Libre;

            EstadoCambiado?.Invoke(this, EventArgs.Empty);  // Notifica el cambio
        }


        public SillaControl()
        {
            InitializeComponent();
            Asiento = new Asiento(EstadoAsiento.Libre, 1);
            this.DataContext = Asiento;
        }
    }
}