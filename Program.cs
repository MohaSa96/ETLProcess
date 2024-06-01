using System;
using System.Collections.Generic;// Listen generieren
using System.Drawing;//Zeichnen Tools
using System.Drawing.Imaging;//Speicherung der Grafik als Bild
using System.Net; //Internet-Zugriff
using System.IO; //Dateizugriff

/**************************************
Aufgabenvariante: A-Klimadaten
***************************************/
    class Program
    {
        static void Main(string[] args)
        {

            /*****************************************************************
            Aktuelle Daten aus einer Datenquelle aus dem Internet laden
            *****************************************************************/

            Console.WriteLine("Aufrufen des Webservices https://cbrell.de/bwi403/demo/getKlimaWS.php... \r\n");

            //Klasse Webclient ermöglicht uns Daten auf Internet zuzugreifen, aber wir brauchen zusäztlich den Namensraum System.IO zum Dateizugriff
            //und System.Net zum Internet-Zugriff
            WebClient myClient = new WebClient();

            //Methode OpenRead nimmt die Daten von einer Webseite bzw. URL Link
            Stream data = myClient.OpenRead("https://cbrell.de/bwi403/demo/getKlimaWS.php");

            Console.WriteLine("Webservice erfolgreich aufgerufen... \r\n");

            //Das Ergebnis wird in eine Stringvariable eingelesen, aus diesem Grund benötigen wir die Klasse StreamReader
            StreamReader reader = new StreamReader(data);

            //unser string ms enthält den Inhalt
            string ms = reader.ReadToEnd();
            data.Close();
            reader.Close();

            //String wird in der Console ausgegeben,damit wir eine bessere Visualisierung der Daten haben
            Console.WriteLine("Liefert: ");
            Console.WriteLine(ms);
            Console.WriteLine("Daten wurden von Internet hochgeladen... \r\n");

            /*****************************************************************
            Die geladenen Daten abspeichern.
            *****************************************************************/


            File.WriteAllText(@"ein-Mohasa96.csv", ms);
            Console.WriteLine("Daten wurden als csv abgespeichert... \r\n");


            //einlesen der Daten in einem String Array
            Console.WriteLine("Daten werden eingelesen und nach den gewünschten Werten sortiert... \r\n");

            /*****************************************************************
            Aus den Daten Werte extrahieren
            *****************************************************************/

            String[] csvLines = File.ReadAllLines(@"ein-Mohasa96.csv");

            //wir erstellen drei Listen für die benötigten, extrahierten Werten
            //wir haben uns entschieden als Datensatz den Druck und für Zeitstempel Unixtime
            var nr = new List<String>();
            var unixtime = new List<String>();
            var druck = new List<String>();


            //eine For-Schleife, die auf alle Zeile der Ausgangstabelle läuft
            for (int i = 0; i < csvLines.Length; i++)
            {
                //um die Ausgangstabelle aufzuspalten, benötigen die Methode "Split", dann alles wird in einem Array gespeichert
                String[] csvColumns = csvLines[i].Split(';');

                //wir ordnen nun alle ausgewählten Werten nach Spalten zu
                nr.Add(csvColumns[0]);    //hier speichern wir die erste Spalte der Ausgangstabelle und zwar die Nummer
                unixtime.Add(csvColumns[1]); //hier speichern wir die zweite Spalte der Ausgangstabelle und zwar die Unixtime
                druck.Add(csvColumns[5]);//hier speichern wir die dritte Spalte der Ausgangstabelle und zwar der Druck

            }

            //wir erstellen ein String Array als "Brücke", um die Zeilen der zweiten Tabelle (aus) aufzubauen
            String[] s = new string[nr.Count];

            //die For-Schleife schreibt die Zeilen in die Tabelle (aus) auf 
            for (int i = 0; i < nr.Count; i++)
            {
                s[i] = Convert.ToString(nr[i] + ";" + unixtime[i] + ";" + druck[i] + "\r\n");

                // Visualisierung der Daten in der Console
                Console.WriteLine(s[i]);
            }

            /*****************************************************************
            Die Ergebnis-Werte abspeichern
            *****************************************************************/

            File.WriteAllLines(@"aus-Mohasa96.csv", s);
            Console.WriteLine("extrahierte Daten wurden als csv abgespeichert... \r\n");

            /*****************************************************************
            Erstellung der Grafik
            *****************************************************************/

            //wie in der Vorlesung gennant, erstellen wir ein Objekt aus der Klasse Bitmap.
            //Eine Vorlage erzeugen mit 768 mal 768 Pixel
            Bitmap b = new Bitmap(768, 768);

            //erzeugt ein Bild aus dem Bitmap Objekt
            Graphics g = Graphics.FromImage(b);

            //Hintergrundfarbe auf weiß setzen
            g.Clear(Color.White);

            //aus der Klasse Pen erzeugen wir ein objekt zum Zeichnen und Schreiben auf dem Bitmap
            Pen p = new Pen(Color.Black, 2);

            //zwei Objekte Font, zum Zeichnen und Schreiben
            Font f = new Font("Arial", 10);
            Font f1 = new Font("Arial", 15);

            //StringFormat dient zur vertikalen Positienierung der Daten
            StringFormat sf = new StringFormat();
            sf.FormatFlags = StringFormatFlags.DirectionVertical;

            //SolidBrush ermöglicht Verfärbung der gezeichneten Daten,Texte etc...
            SolidBrush sb = new SolidBrush(Color.Red);
            SolidBrush sb1 = new SolidBrush(Color.Black);
            SolidBrush sb3 = new SolidBrush(Color.Green);
            SolidBrush sb4 = new SolidBrush(Color.Blue);

            /*****************************************************************
            Die Ergebnis-Werte ggf. zu Kennzahlen verdichten
            *****************************************************************/

            g.DrawString("Dr", f, sb3, 0, 20); // Dr ist eine Abkürzung von Druck auf der X-Achse der Grafik

            g.DrawString("UT", f, sb3, 740, 610); //UT ist eine Abkürzung von Unixtime auf der Y-Achse der Grafik

            g.DrawString("0", f, sb4, 20, 610);//0 ist der Ursprung des Koordinatensystems

            g.DrawString("Mohasa96", f1, sb1, 20, 710);// Beschriftung des Bildes ganz unten links

            g.DrawString("Darstellung der Veränderung des Drucks im Laufe der Zeit", f, sb4, 100, 100);// Titel der Grafik

            g.DrawString("Dr X-Axe = Druck \nUT Y-Axe = UnixTime", f, sb, 500, 710);// Legenden der Grafik

            //Zwei Objekte aus der Klasse Point, um die X-Achse zu zeichnen.
            Point point1 = new Point(32, 0);
            Point point2 = new Point(32, 700);

            //Zwei Objekte aus der Klasse Point, um die Y-Achse zu zeichnen.
            Point point3 = new Point(0, 600);
            Point point4 = new Point(768, 600);

            //Zeichnung der X-Achse
            g.DrawLine(p, point1, point2);

            //Zeichnung der Y-Achse
            g.DrawLine(p, point3, point4);


            //For-Schleife um die X-Achse mit Druck Werten zu beschriften
            for (int i = 500; i >= 100; i -= 100)
            {
                //dabei konvertieren wir die Druck Werte von Typ Int zu String,dann Zeichnung der Werten auf der X-Achse
                g.DrawString(Convert.ToString(i * 3), f, sb1, new Point(0, (600 - i)));
            }

            Console.WriteLine("Bestimmung der Veränderungen des Drucks im Lauf der Zeit... \r\n");

            //For-Schleife um die Y-Achse mit Unixtime Werten zu beschriften
            for (int i = nr.Count - 1, h = 718 / nr.Count - 1; i > 0 && h <= 718; i--, h += 718 / nr.Count - 1)
            {
                //Wenn der Wert gleich Unixtime überspringen wir ihn
                if (unixtime[i] == "UnixTime") { continue; }

                //sonst Zeichnung der Werten von UnixTime auf der Y-Achse
                g.DrawString(unixtime[i], f, sb1, h + 5, 610, sf);

                //gibt die UnixTime Werten und die entsprechenden Druck Werten auf der Konsole aus
                Console.WriteLine("im Zeitpunkt " + unixtime[i] + " war der Druck :  " + druck[i] + "\r\n");

            }

            Console.WriteLine("Die Grafik wird erstellt und als Bild abgespreichert... \r\n");
            Console.WriteLine("Drücken Sie eine beliebige Taste, um dieses Fenster zu schließen.");

            //Array vom typ float,um die Druck Werte von String zu Float zu konvertieren, damit wir nachher die Säulen der Grafik zeichnen zu können
            float[] drucknumber = new float[druck.Count];

            /*****************************************************************
            Aus den Ergebnis - Werten eine(Informations -)Grafik erstellen.
            *****************************************************************/

            //um die Werten zur Säulen in der Grafik darzustellen, verwenden wie eine For-Schleife,die auf alle Werten durchläuft
            for (int i = nr.Count - 1, h = 718 / nr.Count - 1; i > 0 && h <= 718; i--, h += 718 / nr.Count - 1)
            {
                //Wenn der Wert gleich Druck überspringen wir ihn
                if (druck[i] == "Druck") { continue; }

                else
                {
                    //hier erfolgt die Konvertierung der Druck Werte vom Typ String zu Float und werden in dem Array drucknumber gespeichert
                    drucknumber[i] = float.Parse(druck[i].Replace('.', ','));
                }

                float height = drucknumber[i] * 500 / 1500;
                /*Höhe der einzelnen Säulen, ist wie folgedem berechnet : von 0 bis 1500 haben 500 Pixel,
                zb :  1500                                ->  500 Pixel
                      drucknumber[i](Wert von Druck)      -> X (Höhe der Säule in Pixel) 
                 anhand von Dreisatz Regeln berehcnen wir X = drucknumber[i] * 500 / 1500
              */

                float width = 30;//Breite der Säulen
                float x = h;//Basis der Säulen auf X-Achse 
                float y = 600 - height; //Kopf der Säulen auf Y-Achse

                //Zeichnen der entsprechenden Druck Werten in der Mitte jeder Säule
                g.DrawString(Convert.ToString(drucknumber[i]), f, sb, new Point(h + 5, 400), sf);

                //Zeichnen von Säulen
                g.DrawRectangle(p, x, y, width, height);
            }
            /*****************************************************************
            die Grafik als Bild abspeichern
            *****************************************************************/

            //letztendlich spreichern wir die Grafik als Bild
            b.Save(@"info-Mohasa96.png", ImageFormat.Png);

            Console.ReadKey();

        }
    }