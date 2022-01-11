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

namespace InternetRadio.Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Player.LoadedBehavior = MediaState.Manual;
            this.Player.UnloadedBehavior = MediaState.Manual;
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            this.Player.Stop();
            this.Player.Position = TimeSpan.Zero;

            try
            {
                string source = ((Button)sender).CommandParameter as string;
                this.Player.Source = new Uri(source, UriKind.Absolute);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Player.Play:\n" + ex);
            }

            try
            {
                this.Player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Player.Play:\n" + ex);
            }
        }
    }
}
