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
using System.Threading; // Threads using

namespace PW_Lab1
{
    public partial class Form1 : Form
    {
        // Access from few threads - volatile 
        public byte[] bytesTab;
        public int[] histogramTab;
        Random random;
        Thread thread_pomocniczy, t1, t2, t3, t4;
        Thread[] threadsTab;
        int startTime, stopTime;

        public Form1()
        {
            InitializeComponent();

            // Disabling form control !
            Form1.CheckForIllegalCrossThreadCalls = false;

            this.textBox2.Text = "Threads : Time:";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bytesTab = new byte[4000000];
            random = new Random(7);
            
            // "Generating" byte's tab
            random.NextBytes(bytesTab);

            // write out the byte tab using helper thread...
            thread_pomocniczy = new Thread(showTabInPanel);
            thread_pomocniczy.Start();
            
            this.comboBox1.Enabled = true;
            this.button1.Enabled = false;

        }

        private void showTabInPanel()
        {
            for (int i = 0; i < 10; i++)
            {
                textBox1.Text += bytesTab[i] + ", ";
                //if (i % 5 == 1 && i != 1)
                  //  textBox1.Text += "\n";
            }    
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // clearing the histogram
            this.chart1.Series["Histogram"].Points.Clear();
            // table to histogram 
            histogramTab = new int[256];

            // generating histogram depends on threads amount 
            switch(comboBox1.Text.ToString())
            {
                case "sequentially":
                    
                    threadsTab = new Thread[1];
                    for (int i = 0; i < threadsTab.Length; i++ )
                    {
                        threadsTab[i] = this.getThread(1,i);
                    }

                    startTime = Environment.TickCount;
                    foreach(Thread t in threadsTab)
                    {
                        t.Start();
                    }
                    foreach (Thread t in threadsTab)
                    {
                        t.Join();
                    }
                    stopTime = Environment.TickCount;
                    this.addListing(1, stopTime - startTime);                    
                    break;

                case "2 threads":

                    threadsTab = new Thread[2];
                     for (int i = 0; i < threadsTab.Length; i++ )
                    {
                        threadsTab[i] = this.getThread(2,i);
                        threadsTab[i].Priority = ThreadPriority.Highest;
                    }

                    startTime = Environment.TickCount;
                    foreach(Thread t in threadsTab)
                    {
                        t.Start();
                    }
                    foreach (Thread t in threadsTab)
                    {
                        t.Join();
                    }
                    stopTime = Environment.TickCount;

                    this.addListing(2, stopTime - startTime);  
                    break;

                case "4 threads":
                    threadsTab = new Thread[4];
                     for (int i = 0; i < threadsTab.Length; i++ )
                    {
                        threadsTab[i] = this.getThread(4,i);
                        threadsTab[i].Priority = ThreadPriority.Highest;
                    }

                    startTime = Environment.TickCount;
                    foreach(Thread t in threadsTab)
                    {
                        t.Start();
                    }
                    foreach (Thread t in threadsTab)
                    {
                        t.Join();
                    }
                    stopTime = Environment.TickCount;

                    this.addListing(4, stopTime - startTime);  
                    break;

            }

            this.drawHistogram();
            
        }

        private void countHistogram(int _start, int _end)
        {
            for(int i= _start; i< _end; i++)
            {
                byte tmp_byte =  bytesTab[i];
                histogramTab[tmp_byte] += 1;
            }   
        }

        private void drawHistogram()
        {
            this.chart1.Palette = ChartColorPalette.BrightPastel;

            for (int i = 0; i < histogramTab.Length; i++)
            {
                // Add series
                this.chart1.Series["Histogram"].Points.AddXY(Convert.ToString(i), histogramTab[i]);
            }
        }

        private Thread getThread(int _threadQuantity, int _threadNumber)
        {
            Thread threadTmp;
            ThreadStart ts = this.getThreadStart(_threadQuantity, _threadNumber);

            threadTmp = new Thread(ts);

            return threadTmp;
        }

        private ThreadStart getThreadStart(int _threadQuantity, int _threadNumber)
        {
            int[,] compartments;
            compartments = this.getSecion(_threadQuantity);

            int begin = compartments[_threadNumber, 0];
            int end = compartments[_threadNumber, 1];

            ThreadStart ts = delegate() 
                            { 
                                countHistogram(begin, end); 
                            };

            return ts;
        }

        private int[,] getSecion(int _threadQuantity)
        {
            int[,] intTabTmp = new int[_threadQuantity,2];  

            // Cons compartments
            switch(_threadQuantity)
            {
                case 1: // Single thread 
                    intTabTmp = new int[1,2] {{0, bytesTab.Length}};
                    break;

                case 2: // TWo threads
                    intTabTmp = new int[2, 2] {{0, bytesTab.Length/2 -1},
                                               {bytesTab.Length/2, bytesTab.Length}};
                    break;

                case 4: // Four threads
                    intTabTmp = new int[4, 2] { {0, bytesTab.Length/4 -1}, 
                                               {bytesTab.Length/4, bytesTab.Length/2 -1},
                                               {bytesTab.Length/2, (bytesTab.Length - bytesTab.Length/4) -1},
                                               {bytesTab.Length - bytesTab.Length/4, bytesTab.Length}};
                    break;
            }

            return intTabTmp;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] dane = new string[histogramTab.Length];
            for(int i=0; i<histogramTab.Length;i++)
            {
                dane[i] += histogramTab[i].ToString();
            }
            
            //System.IO.File.WriteAllLines(@"C:\Users\Piotr\Desktop\4.txt", dane);
        }

        private void addListing(int _threadQuantity, int _time)
        {
            
            textBox2.Text += System.Environment.NewLine + String.Format("{0} : {1}", _threadQuantity, _time);
        }
    }
}
