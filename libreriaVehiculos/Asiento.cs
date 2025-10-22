using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace libreriaVehiculos
{
    public class Asiento : INotifyPropertyChanged
    {
        private EstadoAsiento estado;
        private int numeroAsiento;

        public Asiento(EstadoAsiento estado, int numeroAsiento)
        {
            this.estado = estado;
            this.numeroAsiento = numeroAsiento;
        }

        public EstadoAsiento Estado
        {
            get { return estado; }
            set
            {
                if (estado != value)
                {
                    estado = value;
                    OnPropertyChanged(); // Notifica a la UI que el valor cambió
                }
            }
        }

        public int NumeroAsiento
        {
            get { return numeroAsiento; }
            set
            {
                if (numeroAsiento != value)
                {
                    numeroAsiento = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string nombrePropiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        }
    }
}
