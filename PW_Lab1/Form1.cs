using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Threading; // Obsluga wątków

namespace PW_Lab1
{
    public partial class Form1 : Form
    {
        // Dostep z roznych watkow - volatile 
        private byte[] tablica_bajtow;
        private volatile int[] tab_int;
        Random random;
        Thread thread_pomocniczy, t1, t2, t3, t4;
        Stopwatch sw;

        public Form1()
        {
            InitializeComponent();

            // Wyłączona kontrola !!!!!!!!
            Form1.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tablica_bajtow = new byte[4000000];
            random = new Random(7);
            
            // Generowanie tablicy bajtów
            random.NextBytes(tablica_bajtow);

            // Uruchomienie wypisania tablicy za pomocą wątku 
            thread_pomocniczy = new Thread(showTabInPanel);
            thread_pomocniczy.Start();
            
            this.comboBox1.Enabled = true;
            this.button1.Enabled = false;

        }

        private void showTabInPanel()
        {
            for (int i = 0; i < 10; i++)
            {
                textBox1.Text += tablica_bajtow[i] + ", ";
                //if (i % 5 == 1 && i != 1)
                  //  textBox1.Text += "\n";
            }    
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Tablica do histogramu
            tab_int = new int[256];

            sw = new Stopwatch();

            // Generowanie histogramu w zależności od trybu
            switch(comboBox1.Text.ToString())
            {
                    // TODO Jakiś manager wątków, żeby zwracał tablicę w zależności od parametrów 
                case "Sekwencyjnie":
                    ThreadStart ts = delegate() { licz_histogram(0, tablica_bajtow.Length); };
                    t1 = new Thread(ts);

                    sw.Start(); // Start stop watch

                    t1.Start();

                    ////////////////////this//////////////////
                    /*
                    while (t1.IsAlive)
                    {
                        Thread.Sleep(50);
                    }
                    */
                    ///////////////or this ////////////////////
                    t1.Join(); // czekam na zakończenie

                    sw.Stop(); // Stop
                    MessageBox.Show(sw.ElapsedMilliseconds.ToString());
                    break;

                case "2 wątki":
                    ThreadStart ts1 = delegate() { licz_histogram(0, tablica_bajtow.Length/2); }; //todo: klasa/metoda do zarządzania przedziałami
                    ThreadStart ts2 = delegate() { licz_histogram(tablica_bajtow.Length/2, tablica_bajtow.Length); };
                    t1 = new Thread(ts1);
                    t2 = new Thread(ts2);

                    sw.Start(); // Start stop watch

                    t1.Start();
                    t2.Start();

                    t1.Join();
                    t2.Join();

                    sw.Stop(); // Stop 
                    MessageBox.Show(sw.ElapsedMilliseconds.ToString());
                    break;

                case "4 wątki":
                    ThreadStart ts4_1 = delegate() { licz_histogram(0, tablica_bajtow.Length/4); };
                    ThreadStart ts4_2 = delegate() { licz_histogram(tablica_bajtow.Length/4, tablica_bajtow.Length/2); };
                    ThreadStart ts4_3 = delegate() { licz_histogram(tablica_bajtow.Length / 2, tablica_bajtow.Length - tablica_bajtow.Length/4); };
                    ThreadStart ts4_4 = delegate() { licz_histogram(tablica_bajtow.Length - tablica_bajtow.Length / 4, tablica_bajtow.Length); };

                    t1 = new Thread(ts4_1);
                    t2 = new Thread(ts4_2);
                    t3 = new Thread(ts4_3);
                    t4 = new Thread(ts4_4);

                    sw.Start(); // Start stop watch

                    t1.Start();
                    t2.Start();
                    t3.Start();
                    t4.Start();

                    t1.Join();
                    t2.Join();
                    t3.Join();
                    t4.Join();

                    sw.Stop(); // Stop 
                    MessageBox.Show(sw.ElapsedMilliseconds.ToString());

                    break;
            }



            this.rysuj_histogram();
            
        }

        private void licz_histogram(int _start, int _end)
        {

            for(int i= _start; i< _end; i++)
            {
                byte tmp_byte =  tablica_bajtow[i];
                tab_int[tmp_byte] += 1;
            }   
        }

        private void rysuj_histogram()
        {
            this.chart1.Palette = ChartColorPalette.BrightPastel;

            for (int i = 0; i < tab_int.Length; i++)
            {
                // Add series
                this.chart1.Series["Histogram"].Points.AddXY(Convert.ToString(i), tab_int[i]);
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] dane = new string[tab_int.Length];
            for(int i=0; i<tab_int.Length;i++)
            {
                dane[i] += tab_int[i].ToString();
            }
            
            //System.IO.File.WriteAllLines(@"C:\Users\Piotr\Desktop\4.txt", dane);
            
        }
    }
}
