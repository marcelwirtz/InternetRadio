using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Data;
using System.Data.Common;

namespace InternetRadio
{
    /// <summary>
    /// Interaktionslogik für Einstellungen.xaml
    /// </summary>
    public partial class Einstellungen : Window
    {
        private CSVManager csv;
        public DataTable table;

        public Einstellungen()
        {
            InitializeComponent();
            string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Simple Radio");
            csv = new CSVManager(appPath, "radiolist.csv");
            table = new DataTable("Inhalt");
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
