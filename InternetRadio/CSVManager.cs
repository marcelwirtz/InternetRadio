using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Data;

namespace InternetRadio
{

    // handles .csv-Files. (App-Data is stored in .csv-Files)
    // .csv Files have to be seperated by commas (',')!
    // Format: "Name,Source,Fav"

    class CSVManager
    {
        private string path;
        private string fileName;

        public CSVManager(string directoryPath, string fileName)
        {
            this.path     = directoryPath;
            this.fileName = fileName;
        }

        public CSVManager(string appName)
        {
            // Saves .csv-File to default location in %APPDATA%/"appName"/"appName.csv"
            this.path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);
            this.fileName = appName + ".csv";
        }

        //<summary>
        //This method reads a .csv-File and parses it into a DataTable-object.
        //</summary>
        public DataTable GetTableFromFile()
        {
            DataTable table = new DataTable("Inhalt");

            //
            // Adding the main-columns: Name | Source (URL/path) | Favourite?
            //
            table.Columns.Add("Name",   typeof(string));
            table.Columns.Add("Source", typeof(string));
            table.Columns.Add("Fav",    typeof(bool));

            //
            // Read .csv-File to complete the output-table
            //   
            try
            {
                StreamReader reader = new StreamReader(@Path.Combine(path, fileName));

                List<string[]> content = new List<string[]>(); // Container for all rows

                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(','); // Read row and split it into (3) strings
                    content.Add(line);                            // Add row
                }


                //
                // Adding all the rows to the table
                //
                for (int i = 0; i<content.Count; i++)
                {
                    table.Rows.Add(content.ElementAt(i)[0], content.ElementAt(i)[1], Convert.ToBoolean(content.ElementAt(i)[2]));
                }
                table.AcceptChanges();

                // Le fin
                reader.Close();

            }
            catch (DirectoryNotFoundException dirErr)
            {
                Directory.CreateDirectory(path);
            }
            catch (FileNotFoundException fileErr)
            {
                File.Create(Path.Combine(path, fileName));
                MessageBox.Show("Installiert!");
            }
            catch (IOException ioErr)
            {
                MessageBox.Show("Fehler beim Lesen von appdata.csv: \n\r" + ioErr.Message);
            }

            return table;
        }

        //<summary>
        //This method writes DataTable content into a .csv-File
        //</summary>
        public void SaveFileFromTable(DataTable table) 
        {
            StreamWriter writer = new StreamWriter(@Path.Combine(path, fileName));

            foreach (DataRow row in table.Rows)
            {
                writer.WriteLine("{0},{1},{2}", row[0], row[1], row[2].ToString() );
            }
            writer.Close();
        }
    }
}
