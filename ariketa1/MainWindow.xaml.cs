using libreriaVehiculos;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ariketa1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DateTime TwoMonthsFromNow { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            TwoMonthsFromNow = DateTime.Today.AddMonths(2);
            DataContext = this;

            datePickerReserva.SelectedDate = DateTime.Today;
        }

        private void btn_autobus_Click(object sender, RoutedEventArgs e)
        {
            if (datePickerReserva.SelectedDate == null)
            {
                MessageBox.Show("Mesedez, hautatu data baliozko bat.", "Errorea", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime fechaSeleccionada = datePickerReserva.SelectedDate.Value;

            // Abrir ventana de autobús pasando la fecha seleccionada
            var ventanaAutobus = new autobusa_window(fechaSeleccionada, "Autobusa");
            ventanaAutobus.ShowDialog();
        }

        private void btn_tren_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_avion_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}