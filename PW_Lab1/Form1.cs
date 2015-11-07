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
using System.IO;
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

        int timeSeq, numSeq;
        int time2Threads, num2Threads;
        int time4Threads, num4Threads;
        int time8Threads, num8Threads;
        int time10Threads, num10Threads;
        int time12Threads, num12Threads;
        int time14Threads, num14Threads;
        int time16Threads, num16Threads;

        ThreadManager manager = new ThreadManager();

        public Form1()
        {
            // Initialiaze variables
            timeSeq = 0; numSeq = 0;
            time2Threads = 0; num2Threads = 0;
            time4Threads = 0; num4Threads = 0;
            time8Threads = 0; num8Threads = 0;
            time10Threads = 0; num10Threads = 0;
            time12Threads = 0; num12Threads = 0;
            time14Threads = 0; num14Threads = 0;
            time16Threads = 0; num16Threads = 0;

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
                    this.startProcess(1);                
                    break;

                case "2 threads":
                    this.startProcess(2);
                    break;

                case "4 threads":
                    this.startProcess(4);  
                    break;

                case "8 threads":
                    this.startProcess(8);
                    break;

                case "10 threads":
                    this.startProcess(10);
                    break;

                case "12 threads":
                    this.startProcess(12);
                    break;

                case "14 threads":
                    this.startProcess(14);
                    break;

                case "16 threads":
                    this.startProcess(16);
                    break;
            }

            this.drawHistogram();
        }

        private void startProcess(int _param)
        {
            threadsTab = new Thread[_param];
            for (int i = 0; i < threadsTab.Length; i++)
            {
                threadsTab[i] = this.getThread(_param, i);
                threadsTab[i].Priority = ThreadPriority.Highest;
            }

            startTime = Environment.TickCount;
            foreach (Thread t in threadsTab)
            {
                t.Start();
            }
            foreach (Thread t in threadsTab)
            {
                t.Join();
            }
            stopTime = Environment.TickCount;

            this.addListing(_param, stopTime - startTime);  
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
            return manager.getIntervalsTresholds(bytesTab, _threadQuantity);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            
            //System.IO.File.WriteAllLines(@"C:\Users\Piotr\Desktop\4.txt", dane);
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string[] dane = new string[histogramTab.Length];
            try
            {

                for (int i = 0; i < histogramTab.Length; i++)
                {
                    dane[i] += histogramTab[i].ToString();
                }
            }
            catch (Exception ex)
            {

            }
            // Get file name.
            string name = saveFileDialog1.FileName;
            // Write to the file name selected.
            // ... You can write the text from a TextBox instead of a string literal.
            File.WriteAllLines(name, dane);
        }

        private void addListing(int _threadQuantity, int _time)
        {
            textBox2.Text += System.Environment.NewLine + String.Format("{0} : {1}", _threadQuantity, _time);
            this.addAverage(_threadQuantity, _time);
        }

        private void addAverage(int _case, int _time)
        {
            int averageLocal;

            switch(_case)
            {
                case 1:
                    numSeq++;
                    timeSeq += _time;
                    averageLocal = timeSeq / numSeq;
                    textBox3.Text = numSeq.ToString() + ":" + averageLocal.ToString();
                    break;

                case 2:
                    num2Threads++;
                    time2Threads += _time;
                    averageLocal = time2Threads / num2Threads;
                    textBox4.Text = num2Threads.ToString() + ":" + averageLocal.ToString();
                    break;

                case 4:
                    num4Threads++;
                    time4Threads += _time;
                    averageLocal = time4Threads / num4Threads;
                    textBox5.Text = num4Threads.ToString() + ":" + averageLocal.ToString();
                    break;

                case 8:
                    num8Threads++;
                    time8Threads += _time;
                    averageLocal = time8Threads / num8Threads;
                    textBox6.Text = num8Threads.ToString() + ":" + averageLocal.ToString();
                    break;

                case 10:
                    num10Threads++;
                    time10Threads += _time;
                    averageLocal = time10Threads / num10Threads;
                    textBox7.Text = num10Threads.ToString() + ":" + averageLocal.ToString();
                    break;

                case 12:
                    num12Threads++;
                    time12Threads += _time;
                    averageLocal = time12Threads / num12Threads;
                    textBox8.Text = num12Threads.ToString() + ":" + averageLocal.ToString();
                    break;

                case 14:
                    num14Threads++;
                    time14Threads += _time;
                    averageLocal = time14Threads / num14Threads;
                    textBox9.Text = num14Threads.ToString() + ":" + averageLocal.ToString();
                    break;

                case 16:
                    num16Threads++;
                    time16Threads += _time;
                    averageLocal = time16Threads / num16Threads;
                    textBox10.Text = num16Threads.ToString() + ":" + averageLocal.ToString();
                    break;

            }
        }
    }
}
