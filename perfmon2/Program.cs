using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace perfmon2
{
    class Program : System.Windows.Forms.Form
    {
        private Button button1;
        private Button button2;

        private static BackgroundWorker bw = new BackgroundWorker();

        private static PerformanceCounter cpu;
        private static PerformanceCounter read;
        private static PerformanceCounter write;

        private static ArrayList samplesList = new ArrayList();
        private static ArrayList timeList = new ArrayList();
        private static ArrayList readList = new ArrayList();
        private ProgressBar progressBar1;
        private ComboBox comboBox1;
        private TextBox textBox1;
        private Label label1;
        private Label label2;
        private NumericUpDown numericUpDown1;
        private Label label3;
        private Label label4;
        private TextBox textBox2;
        private ComboBox comboBox2;
        private static ArrayList writeList = new ArrayList();

        Program()
        {
            InitializeComponent();

            CreateCounters();

            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = false;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        static void Main()
        {
            Application.EnableVisualStyles();

            Application.Run(new Program());
        }

        private static void CreateCounters()
        {            
            cpu = new PerformanceCounter("Processor Information", "% Processor Time", "_Total", true);
            read = new PerformanceCounter("PhysicalDisk", "Disk Reads/sec", "_Total", true);
            write = new PerformanceCounter("PhysicalDisk", "Disk Writes/sec", "_Total");
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private static void CollectSamples()
        {
            samplesList = new ArrayList();
            timeList = new ArrayList();
            readList = new ArrayList();
            writeList = new ArrayList();
            
            for (int j = 0; j <= 10; j++)
            {

                //int value = r.Next(1, 10);
                //Console.Write(j + " = " + value);
                //cpu.IncrementBy(value);
                samplesList.Add(cpu.NextValue());
                timeList.Add(GetTimestamp(DateTime.Now));
                readList.Add(read.NextValue());
                writeList.Add(write.NextValue());
                System.Threading.Thread.Sleep(1000);
            }

        }

        private static void CalculateResults()
        {
            var result = new StringBuilder();
            result.AppendLine("Time,CPU%,Reads/sec,Writes/sec");

            for (int i = 0; i < (samplesList.Count - 1); i++)
            {
                //Console.WriteLine(" time: "+ timeList[i]+" CPU: "+ samplesList[i] + " read: "+ readList[i]+" write: "+ writeList[i]);
                var newLine = string.Format("{0},{1},{2},{3}", timeList[i], samplesList[i], readList[i], writeList[i]);
                result.AppendLine(newLine);
            }

            /*
            double readSum = 0;
            double writeSum = 0;
            
            for (int j=0;j< (samplesList.Count - 1); j++)
            {
                readSum = readSum + readArray[j];
                writeSum = writeSum + writeArray[j];
            }
            

            Console.WriteLine("total reads= " + readSum);
            Console.WriteLine("total writes= " + writeSum);

            result.AppendLine(string.Format("Total reads={0}, total writes={1}", readSum, writeSum));
            */
            File.WriteAllText("output.csv", result.ToString());
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            samplesList = new ArrayList();
            timeList = new ArrayList();
            readList = new ArrayList();
            writeList = new ArrayList();

            for (int i = 1; (i <= 10); i++)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    samplesList.Add(cpu.NextValue());
                    timeList.Add(GetTimestamp(DateTime.Now));
                    readList.Add(read.NextValue());
                    writeList.Add(write.NextValue());
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Error == null))
            {
                CalculateResults();
                button2.Enabled = false;
            }

        }


        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(798, 79);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 42);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(798, 170);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(132, 42);
            this.button2.TabIndex = 1;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(181, 611);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(709, 24);
            this.progressBar1.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "MS SQL Server",
            "PostgreSQL"});
            this.comboBox1.Location = new System.Drawing.Point(202, 82);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(302, 39);
            this.comboBox1.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(202, 190);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(302, 38);
            this.textBox1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 32);
            this.label1.TabIndex = 5;
            this.label1.Text = "Database";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 32);
            this.label2.TabIndex = 6;
            this.label2.Text = "Size (GB)";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(349, 306);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(155, 38);
            this.numericUpDown1.TabIndex = 7;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 308);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(274, 32);
            this.label3.TabIndex = 8;
            this.label3.Text = "Number of instances";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 416);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 32);
            this.label4.TabIndex = 9;
            this.label4.Text = "Usage";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(158, 416);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(210, 38);
            this.textBox2.TabIndex = 10;
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Hours/day",
            "Hours/Week",
            "Hours/Month",
            "%Utilized / Month"});
            this.comboBox2.Location = new System.Drawing.Point(400, 416);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 39);
            this.comboBox2.TabIndex = 11;
            // 
            // Program
            // 
            this.ClientSize = new System.Drawing.Size(1074, 697);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Program";
            this.Text = "What is the name of this?";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }

            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }
        }

    }
}
