using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Timers;
using System.Windows.Threading;

namespace InternetRadio
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Player _radio;
        private Einstellungen _settings;
        private Hilfe _help;
        private Info _about;
        private CSVManager csvFile;
        private DataTable table;
        private Timer reconnectTimer;

        public MainWindow()
        {
            reconnectTimer = new Timer();
            reconnectTimer.Interval = 5000;
            reconnectTimer.Elapsed += tryReconnect;
            _radio = Player.Instance;
            _radio.Volume = 0.8;
            _radio.StateChanged += new EventHandler(_radio_Changed);
            _radio.SongChanged += new EventHandler<SongEventArgs>(_radio_SongChanged);
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            csvFile = new CSVManager(appPath, "radiolist.csv");
            InitializeComponent();
            PlaylistLaden();
            Update();
            NewSelection();

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // scheint nicht ausgelöst zu werden
        }


        void _radio_Changed(object sender, EventArgs e)
        {
            Update();            
            if (_radio.State == PlayerState.Failed) reconnectTimer.Enabled = true;
            else reconnectTimer.Enabled = false;
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_radio.State == PlayerState.Playing)
                    _radio.Pause();
                else
                    _radio.Play();
                UpdateButton();
            }
            catch (Exception e3)
            {
                MessageBox.Show("Fehler beim Abspielen: "+e3.Message);
            }
        }

        private void Update()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if(stateLabel != null)
                    stateLabel.Content = _radio.State;
            }));

            slider1.Value = _radio.Volume;

            senderListe.ItemsSource = _radio.Playlist;
            senderListe.DisplayMemberPath = "Key";
            senderListe.SelectedValuePath = "Value";
            senderListe.Items.Refresh();

            if (senderListe.SelectedIndex != _radio.SelectedIndex)
            {
                senderListe.SelectedIndex = _radio.SelectedIndex;
            }
            UpdateButton();
        }

        private void UpdateButton()
        {
            play.IsEnabled = false;
            stop.IsEnabled = false;

            switch (_radio.State)
            {
                case PlayerState.Stopped:
                    play.IsEnabled = true;
                    play.Content = "Play";
                    break;
                case PlayerState.Playing:
                    play.IsEnabled = true;
                    stop.IsEnabled = true;
                    play.Content = "Pause";
                    break;
                case PlayerState.Paused:
                    play.IsEnabled = true;
                    stop.IsEnabled = true;
                    play.Content = "Play";
                    break;
                case PlayerState.Failed:
                    play.IsEnabled = false;
                    stop.IsEnabled = true;
                    play.Content = "Pause";
                    break;
                case PlayerState.Unknown:
                    break;
            }
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            _radio.Stop();
        }

        private void laden_Click(object sender, RoutedEventArgs e)
        {
            PlaylistLaden();
        }

        private void PlaylistLaden()
        {
            _radio.Unload();
            table = csvFile.GetTableFromFile();
            try
            {
                _radio.LoadPlaylist(table);            
            }
            catch (Exception e)
            {
                MessageBox.Show("Fehler beim Laden der Playlist: \n" + e.Message);
            }
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _radio.Volume = slider1.Value;
        }

        private void senderListe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NewSelection();
        }

        private void NewSelection()
        {
            try
            {
                if (senderListe.SelectedValue != null)
                {
                    // Ein neuer Sender wurde ausgewählt
                    if (senderListe.SelectedIndex != _radio.SelectedIndex)
                    {
                        _radio.SetSource(senderListe.SelectedIndex,
                            senderListe.SelectedValue.ToString());

                        _radio.Play();
                        UpdateButton();
                        //MessageBox.Show(_radio.GetCurrentTitle(senderListe.SelectedValue.ToString()));
                    }
                    // Kein neuer Sender ausgewählt
                    else
                    {
                        _radio.Stop();
                        _radio.SetSource(senderListe.SelectedIndex,
                            senderListe.SelectedValue.ToString());
                        UpdateButton();
                    }
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show("Fehler beim Laden des Streams: \n" + e2.Message);
            }
        }


        private void quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void showSettings(object sender, RoutedEventArgs e)
        {
            _settings = new Einstellungen(ref csvFile);
            _settings.Owner = this;
            _settings.ShowDialog(); // Code "stoppt" bis das Window geschlossen wird.
            _settings.SaveData();   // Änderungen werden gespeichert.
            Update();
        }

        private void showInfo(object sender, RoutedEventArgs e)
        {
            _about = new Info();
            _about.Owner = this;
            _about.ShowDialog();
        }

        private void showHelp(object sender, RoutedEventArgs e)
        {
            _help = new Hilfe();
            _help.Owner = this;
            _help.ShowDialog();
        }

        void _radio_SongChanged(object sender, SongEventArgs e)
        {
            // Radio hat neue Meta Daten empfangen
            MessageBox.Show(e.title + e.album + e.artist);
        }

        private void tryReconnect(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                NewSelection();
                _radio.Play();
            }));
        }
    }
}
