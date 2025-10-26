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
            TwoMonthsFromNow = DateTime.Today.AddMonths(2); //erreserbak bakarrik egin daitezke gaurtik bi hilabetera arte egiteko errealagoa
            DataContext = this;

            datePickerReserva.SelectedDate = DateTime.Today;
        }

        private void btn_autobus_Click(object sender, RoutedEventArgs e)
        {
            checkHutsik();

            DateTime fechaSeleccionada = datePickerReserva.SelectedDate.Value;

            // ireki lehioa autobusa_window autobus motarekin eta hautatutako datarekin
            var ventanaAutobus = new autobusa_window(fechaSeleccionada, "Autobusa");
            ventanaAutobus.ShowDialog();
        }

        private void btn_tren_Click(object sender, RoutedEventArgs e)
        {
            checkHutsik();

            DateTime fechaSeleccionada = datePickerReserva.SelectedDate.Value;

            // ireki lehioa trena_window tren motarekin eta hautatutako datarekin
            var ventanaTren = new trena_window(fechaSeleccionada, "Trena");
            ventanaTren.ShowDialog();
        }

        private void btn_avion_Click(object sender, RoutedEventArgs e)
        {
            checkHutsik();

            DateTime fechaSeleccionada = datePickerReserva.SelectedDate.Value;

            // ireki lehioa hegazkina_windwow autobus motarekin eta hautatutako datarekin
            var ventanaAvion = new hegazkina_window(fechaSeleccionada, "Hegazkina");
            ventanaAvion.ShowDialog();
        }
        private void checkHutsik()
        {
            if (datePickerReserva.SelectedDate == null)
            {
                MessageBox.Show("Mesedez, hautatu data baliozko bat.", "Errorea", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
    }
}