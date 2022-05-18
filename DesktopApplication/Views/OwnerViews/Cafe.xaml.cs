using CSWBManagementApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            set { _input = value;
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

        public void Copy() {
            MessageBox.Show("Copied");
        }
    }
}
