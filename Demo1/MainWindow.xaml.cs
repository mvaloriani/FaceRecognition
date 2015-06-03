using Demo1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Demo1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            this.mainViewModel = new MainViewModel();
            this.mainViewModel.Initialize();
            this.DataContext = mainViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.SelectImage();
        }

        private async void Button_Click_Oxford(object sender, RoutedEventArgs e)
        {
            mainViewModel.Oxford();
        }


        private async void Button_Click_Emgu(object sender, RoutedEventArgs e)
        {
            mainViewModel.Emgu();
        }

        private async void Button_Click_Betaface(object sender, RoutedEventArgs e)
        {
            mainViewModel.Betaface();
        }
    }
}
