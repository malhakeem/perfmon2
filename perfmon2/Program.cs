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
        private Button buttonStart;
        private Button buttonStop;
        private ProgressBar progressBar1;
        private ComboBox comboBoxDbTypes;
        private TextBox textBoxSize;
        private Label label1;
        private Label label2;
        private NumericUpDown upDownInstances;
        private Label label3;
        private Label label4;
        private TextBox textBoxUsage;
        private ComboBox comboBoxUsageType;
        private TextBox textBoxOutput;
        private Label label5;
        private Label label6;
        private TextBox textBoxIO;
        private TextBox textBoxCPU;
        private Button buttonSuggest;
        private Panel panel1;
        private NumericUpDown upDownCores;
        private Label label7;
        private Button buttonClear;

        private static BackgroundWorker bw = new BackgroundWorker();

        private static PerformanceCounter cpu;
        private static PerformanceCounter read;
        private static PerformanceCounter write;

        private static ArrayList samplesList = new ArrayList();
        private static ArrayList timeList = new ArrayList();
        private static ArrayList readList = new ArrayList();
        private static ArrayList writeList = new ArrayList();

        private static Hashtable prices = new Hashtable();

        Program()
        {
            InitializeComponent();

            CreateCounters();

            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
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

            for (int i = 1; (i <= 120); i++)
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
                    //bw.ReportProgress((i * 100) / 120);
                }
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Error == null))
            {
                CalculateResults();
                buttonStop.Enabled = false;
            }

        }


        private void InitializeComponent()
        {
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.comboBoxDbTypes = new System.Windows.Forms.ComboBox();
            this.textBoxSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.upDownInstances = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxUsage = new System.Windows.Forms.TextBox();
            this.comboBoxUsageType = new System.Windows.Forms.ComboBox();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxIO = new System.Windows.Forms.TextBox();
            this.textBoxCPU = new System.Windows.Forms.TextBox();
            this.buttonSuggest = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.upDownCores = new System.Windows.Forms.NumericUpDown();
            this.buttonClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.upDownInstances)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownCores)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(248, 567);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(132, 51);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(433, 567);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(132, 51);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(222, 806);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(709, 24);
            this.progressBar1.TabIndex = 2;
            // 
            // comboBoxDbTypes
            // 
            this.comboBoxDbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDbTypes.FormattingEnabled = true;
            this.comboBoxDbTypes.Items.AddRange(new object[] {
            "MS SQL Server",
            "PostgreSQL"});
            this.comboBoxDbTypes.Location = new System.Drawing.Point(286, 39);
            this.comboBoxDbTypes.Name = "comboBoxDbTypes";
            this.comboBoxDbTypes.Size = new System.Drawing.Size(302, 39);
            this.comboBoxDbTypes.Sorted = true;
            this.comboBoxDbTypes.TabIndex = 3;
            // 
            // textBoxSize
            // 
            this.textBoxSize.Location = new System.Drawing.Point(286, 102);
            this.textBoxSize.Name = "textBoxSize";
            this.textBoxSize.Size = new System.Drawing.Size(302, 38);
            this.textBoxSize.TabIndex = 4;
            this.textBoxSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSize_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 32);
            this.label1.TabIndex = 5;
            this.label1.Text = "Database";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 32);
            this.label2.TabIndex = 6;
            this.label2.Text = "Size (GB)";
            // 
            // upDownInstances
            // 
            this.upDownInstances.Location = new System.Drawing.Point(433, 170);
            this.upDownInstances.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownInstances.Name = "upDownInstances";
            this.upDownInstances.Size = new System.Drawing.Size(155, 38);
            this.upDownInstances.TabIndex = 7;
            this.upDownInstances.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(274, 32);
            this.label3.TabIndex = 8;
            this.label3.Text = "Number of instances";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 300);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 32);
            this.label4.TabIndex = 9;
            this.label4.Text = "Usage";
            // 
            // textBoxUsage
            // 
            this.textBoxUsage.Location = new System.Drawing.Point(225, 294);
            this.textBoxUsage.Name = "textBoxUsage";
            this.textBoxUsage.Size = new System.Drawing.Size(210, 38);
            this.textBoxUsage.TabIndex = 10;
            // 
            // comboBoxUsageType
            // 
            this.comboBoxUsageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUsageType.FormattingEnabled = true;
            this.comboBoxUsageType.Items.AddRange(new object[] {
            "Hours/day",
            "Hours/Week",
            "Hours/Month",
            "%Utilized / Month"});
            this.comboBoxUsageType.Location = new System.Drawing.Point(467, 293);
            this.comboBoxUsageType.Name = "comboBoxUsageType";
            this.comboBoxUsageType.Size = new System.Drawing.Size(121, 39);
            this.comboBoxUsageType.TabIndex = 11;
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxOutput.Location = new System.Drawing.Point(695, 70);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.Size = new System.Drawing.Size(487, 685);
            this.textBoxOutput.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 405);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 32);
            this.label5.TabIndex = 13;
            this.label5.Text = "IO/month";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 474);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 32);
            this.label6.TabIndex = 14;
            this.label6.Text = "CPU %";
            // 
            // textBoxIO
            // 
            this.textBoxIO.Location = new System.Drawing.Point(286, 405);
            this.textBoxIO.Name = "textBoxIO";
            this.textBoxIO.Size = new System.Drawing.Size(302, 38);
            this.textBoxIO.TabIndex = 15;
            // 
            // textBoxCPU
            // 
            this.textBoxCPU.Location = new System.Drawing.Point(286, 474);
            this.textBoxCPU.Name = "textBoxCPU";
            this.textBoxCPU.Size = new System.Drawing.Size(302, 38);
            this.textBoxCPU.TabIndex = 16;
            // 
            // buttonSuggest
            // 
            this.buttonSuggest.Location = new System.Drawing.Point(284, 735);
            this.buttonSuggest.Name = "buttonSuggest";
            this.buttonSuggest.Size = new System.Drawing.Size(132, 51);
            this.buttonSuggest.TabIndex = 19;
            this.buttonSuggest.Text = "Suggest";
            this.buttonSuggest.UseVisualStyleBackColor = true;
            this.buttonSuggest.Click += new System.EventHandler(this.buttonSuggest_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.buttonClear);
            this.panel1.Controls.Add(this.upDownCores);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.comboBoxDbTypes);
            this.panel1.Controls.Add(this.buttonStop);
            this.panel1.Controls.Add(this.buttonStart);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxCPU);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBoxIO);
            this.panel1.Controls.Add(this.textBoxSize);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.upDownInstances);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.comboBoxUsageType);
            this.panel1.Controls.Add(this.textBoxUsage);
            this.panel1.Location = new System.Drawing.Point(36, 70);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(631, 640);
            this.panel1.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 233);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(222, 32);
            this.label7.TabIndex = 9;
            this.label7.Text = "Number of cores";
            // 
            // upDownCores
            // 
            this.upDownCores.Location = new System.Drawing.Point(433, 233);
            this.upDownCores.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownCores.Name = "upDownCores";
            this.upDownCores.Size = new System.Drawing.Size(155, 38);
            this.upDownCores.TabIndex = 10;
            this.upDownCores.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(65, 567);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(132, 51);
            this.buttonClear.TabIndex = 19;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // Program
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1278, 859);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonSuggest);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.progressBar1);
            this.Name = "Program";
            this.Text = "What is the name of this?";
            ((System.ComponentModel.ISupportInitialize)(this.upDownInstances)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownCores)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy != true)
            {
                //progressBar1.Maximum = 100;
                //progressBar1.Step = 1;
                //progressBar1.Value = 0;
                bw.RunWorkerAsync();
                buttonStart.Enabled = false;
            }

            buttonStop.Enabled = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }
            buttonStart.Enabled = true;

            double price = double.MinValue;
            WindowsCalculator calculator;
            switch (comboBoxDbTypes.SelectedIndex)
            {
                case 0:
                    calculator = new WindowsAzureCalculator(5, 5, 5, 5, 5, 5);
                    price = calculator.CalculateBestPrice();
                    if (prices.ContainsKey("Azure"))
                        prices["Azure"] = price;
                    else
                        prices.Add("Azure", price);
                    break;
                case 1:
                    calculator = new WindowsIBMCalculator(5, 5, 5);
                    price = calculator.CalculateBestPrice();
                    if (prices.ContainsKey("Azure"))
                        prices["IBM"] = price;
                    else
                        prices.Add("IBM", price);
                    break;
                default:
                    break;
            }

            textBoxOutput.AppendText(string.Format("Finished montioring for {0} \n Price: {1} \n", comboBoxDbTypes.SelectedItem, price));
        }

        private void textBoxSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void buttonSuggest_Click(object sender, EventArgs e)
        {
            if (prices.Count == 0)
            { 
                textBoxOutput.AppendText("At least one plan needs to be estimated");
                return;
            }


            double bestPrice = double.MaxValue; 
            foreach (DictionaryEntry pair in prices)
            {
                if ((double)pair.Value <= bestPrice)
                {
                    bestPrice = (double)pair.Value;
                }
                textBoxOutput.AppendText(string.Format("Bla bla {0} ", bestPrice));
            }
            
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            upDownInstances.Value = 1;
            upDownCores.Value = 1;
            textBoxUsage.Clear();
            textBoxSize.Clear();
            textBoxIO.Clear();
            textBoxCPU.Clear();
        }
    }
}
