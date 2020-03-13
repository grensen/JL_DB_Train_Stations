using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

// http://data.deutschebahn.com/datasets/haltestellen/
// CC BY:   DB Station & Services AG

// https://j3l7h.de/lectures/1616ss/Informatik_2/Material/03E.1_Material.zip
// Source code from Professor Jörn Loviscach https://j3l7h.de/about.html

// https://www.youtube.com/watch?v=41SdVA2aqKw
// Lecture from Professor Jörn Loviscach on German 


namespace JL_DB_Stations
{
    public partial class MainWindow : Window
    {
        Stations[] stations;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                string[] lines = File.ReadAllLines(ofd.FileName);
                stations = new Stations[lines.Length - 1]; // eins weniger, weil Spaltentitel als oberste Zeile
                string[] t = lines[0].ToLower().Split(';'); // split into input string
                int lengthIdx = Array.IndexOf(t, "laenge");
                int widthIdx = Array.IndexOf(t, "breite");
                int placeIdx = Array.IndexOf(t, "name"); // good for 2021? ^^

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Replace(',', '.').Split(';'); // double issue (2016 = '.', 2017/2020 = ',') 
                    double length = double.Parse(parts[lengthIdx], System.Globalization.CultureInfo.InvariantCulture);
                    double width = double.Parse(parts[widthIdx], System.Globalization.CultureInfo.InvariantCulture);
                    stations[i - 1] = new Stations(parts[placeIdx], length, width, parts[placeIdx].EndsWith("Hbf") ? true : false);
                }

                double minLength = stations.Min(x => x.Length), maxLength = stations.Max(x => x.Length);
                double minWidth = stations.Min(x => x.Width), maxWidth = stations.Max(x => x.Width);

                bool swap = false;
                for (int j = 1; j < 3; j++, swap = !swap) // stations, main stations...
                {
                    for (int i = 0; i < stations.Length; i++)
                    {
                        if (stations[i].IsMain == swap)  // swap to handle both cases
                        {
                            Ellipse elli = new Ellipse
                            {
                                Width = 1.3 * j,
                                Height = 1.3 * j,
                                Fill = j == 1 ? Brushes.Chartreuse : Brushes.Red,  // stations: main stations
                                ToolTip = stations[i].Place
                            };
                            artboard.Children.Add(elli);
                            Canvas.SetLeft(elli, artboard.ActualWidth / (maxLength - minLength) * (stations[i].Length - minLength) * 0.85 + 30);
                            Canvas.SetBottom(elli, artboard.ActualHeight / (maxWidth - minWidth) * (stations[i].Width - minWidth) - 50);
                        }
                    }
                }
            }
        } // bc
    } // mw

    class Stations
    {
        string place;
        double length;
        double width;
        bool main;
        public Stations(string place, double length, double width, bool main)
        {
            this.place = place;
            this.length = length;
            this.width = width;
            this.main = main;
        }
        public double Length // Property: Spezialität von C#
        {
            get { return length; }
        }
        public double Width
        {
            get { return width; }
        }
        public string Place
        {
            get { return place; }
        }
        public bool IsMain
        {
            get { return this.main; }
        }
    }
}

