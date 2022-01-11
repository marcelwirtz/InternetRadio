using System.Windows;
using System.Data;

namespace InternetRadio
{
    /// <summary>
    /// Interaktionslogik für Einstellungen.xaml
    /// </summary>
    public partial class Einstellungen : Window
    {
        public DataTable table;
        private CSVManager csv;

        public Einstellungen(ref CSVManager csvManager)
        {
            InitializeComponent();
            table = new DataTable("Inhalt");
            csv = csvManager;
            table = csv.GetTableFromFile(); //aus radiolist.csv

            dataGrid1.ItemsSource = table.AsDataView();
        }

        public void SaveData()
        {
            // table wird durch das Binding aktualisiert und hier gespeichert
            csv.SaveFileFromTable(table);
        }
    }
}
