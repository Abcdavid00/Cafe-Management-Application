using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CSWBManagementApplication.Views
{
    /// <summary>
    /// Interaction logic for Cafe.xaml
    /// </summary>
    public partial class Cafe : UserControl, INotifyPropertyChanged
    {
        public Cafe()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _input;

        public string Input
        {
            get { return _input; }
            set
            {
                _input = value;
                OnPropertyChanged();
            }
        }

        private string _output;

        public string Output
        {
            get { return _output; }
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }

        public void Copy()
        {
            MessageBox.Show("Copied");
        }
    }
}