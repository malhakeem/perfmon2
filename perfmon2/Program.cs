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
    enum DBType { PostgreSQL = 1, SQLServer, DB2 };
    class Program : System.Windows.Forms.Form
    {
        private Button buttonStart;
        private Button buttonStop;
        private ComboBox comboBoxDbTypes;
        private TextBox textBoxStorage;
        private Label label1;
        private Label label2;
        private NumericUpDown upDownInstances;
        private Label label3;
        private Label label4;
        private TextBox textBoxOutput;
        private Label label6;
        private TextBox textBoxCPU;
        private Button buttonSuggest;
        private Panel panel1;
        private NumericUpDown upDownCores;
        private Label label7;
        private Button buttonClear;
        private TextBox textBoxCPUScore;
        private Label label8;
        private CheckBox checkBoxHighAvailability;
        private Button buttonEstimate;
        private NumericUpDown upDownUsage;
        private Label label9;

        private static BackgroundWorker bw = new BackgroundWorker();

        private static PerformanceCounter cpu;
        private static PerformanceCounter read;
        private static PerformanceCounter write;

        public static double maxCPU;
        public static double avgCPU;
        public static double maxRead;
        public static double maxWrite;
        public static double avgIO;

        private static ArrayList samplesList = new ArrayList();
        private static ArrayList timeList = new ArrayList();
        private static ArrayList readList = new ArrayList();
        private static ArrayList writeList = new ArrayList();

        private static Hashtable prices = new Hashtable();

        private static WindowsAmazonCalculator AmazonCalculator = new WindowsAmazonCalculator();
        private static WindowsAzureCalculator AzureCalculator = new WindowsAzureCalculator();
        private static WindowsIBMCalculator IBMCalculator = new WindowsIBMCalculator();
        private static WindowsGoogleCalculator GoogleCalculator = new WindowsGoogleCalculator();


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

            for (int i = 0; i < samplesList.Count; i++)
            {
                var newLine = string.Format("{0},{1},{2},{3}", timeList[i], samplesList[i], readList[i], writeList[i]);
                result.AppendLine(newLine);
            }
            File.WriteAllText("output.csv", result.ToString());
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            samplesList = new ArrayList();
            timeList = new ArrayList();
            readList = new ArrayList();
            writeList = new ArrayList();

            // Maybe we can set a timeout here
            while (true)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    samplesList.Add(cpu.NextValue());
                    timeList.Add(GetTimestamp(DateTime.Now));
                    readList.Add(read.NextValue());
                    writeList.Add(write.NextValue());
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
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
            buttonStart = new Button();
            buttonStop = new Button();
            comboBoxDbTypes = new System.Windows.Forms.ComboBox();
            textBoxStorage = new TextBox();
            label1 = new Label();
            label2 = new Label();
            upDownInstances = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            textBoxOutput = new TextBox();
            label6 = new Label();
            textBoxCPU = new TextBox();
            buttonSuggest = new Button();
            panel1 = new System.Windows.Forms.Panel();
            label9 = new Label();
            upDownUsage = new NumericUpDown();
            buttonEstimate = new Button();
            checkBoxHighAvailability = new System.Windows.Forms.CheckBox();
            textBoxCPUScore = new TextBox();
            label8 = new Label();
            buttonClear = new Button();
            upDownCores = new NumericUpDown();
            label7 = new Label();
            ((System.ComponentModel.ISupportInitialize)(upDownInstances)).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(upDownUsage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(upDownCores)).BeginInit();
            SuspendLayout();
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(172, 790);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(132, 51);
            buttonStart.TabIndex = 0;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += new EventHandler(buttonStart_Click);
            // 
            // buttonStop
            // 
            buttonStop.Enabled = false;
            buttonStop.Location = new Point(326, 790);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(132, 51);
            buttonStop.TabIndex = 1;
            buttonStop.Text = "Stop";
            buttonStop.UseVisualStyleBackColor = true;
            buttonStop.Click += new EventHandler(buttonStop_Click);
            // 
            // comboBoxDbTypes
            // 
            comboBoxDbTypes.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDbTypes.FormattingEnabled = true;
            comboBoxDbTypes.Items.AddRange(new object[] {
            "--select a database--",
            "DB2",
            "MS SQL Server",
            "PostgreSQL"});
            comboBoxDbTypes.Location = new Point(286, 39);
            comboBoxDbTypes.Name = "comboBoxDbTypes";
            comboBoxDbTypes.Size = new Size(302, 39);
            comboBoxDbTypes.Sorted = true;
            comboBoxDbTypes.TabIndex = 3;
            comboBoxDbTypes.SelectedIndex = 0;
            // 
            // textBoxStorage
            // 
            textBoxStorage.Location = new Point(286, 102);
            textBoxStorage.Name = "textBoxStorage";
            textBoxStorage.Size = new Size(302, 38);
            textBoxStorage.TabIndex = 4;
            textBoxStorage.KeyPress += new KeyPressEventHandler(textBoxSize_KeyPress);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 39);
            label1.Name = "label1";
            label1.Size = new Size(137, 32);
            label1.TabIndex = 5;
            label1.Text = "Database";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(30, 102);
            label2.Name = "label2";
            label2.Size = new Size(181, 32);
            label2.TabIndex = 6;
            label2.Text = "Storage (GB)";
            // 
            // upDownInstances
            // 
            upDownInstances.Location = new Point(433, 170);
            upDownInstances.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            upDownInstances.Name = "upDownInstances";
            upDownInstances.Size = new Size(155, 38);
            upDownInstances.TabIndex = 7;
            upDownInstances.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(30, 170);
            label3.Name = "label3";
            label3.Size = new Size(274, 32);
            label3.TabIndex = 8;
            label3.Text = "Number of instances";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(30, 300);
            label4.Name = "label4";
            label4.Size = new Size(97, 32);
            label4.TabIndex = 9;
            label4.Text = "Usage";
            // 
            // textBoxOutput
            // 
            textBoxOutput.BackColor = SystemColors.ControlLightLight;
            textBoxOutput.Location = new Point(735, 70);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.Size = new Size(487, 685);
            textBoxOutput.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(30, 382);
            label6.Name = "label6";
            label6.Size = new Size(143, 32);
            label6.TabIndex = 14;
            label6.Text = "RAM (GB)";
            // 
            // textBoxCPU
            // 
            textBoxCPU.Location = new Point(286, 379);
            textBoxCPU.Name = "textBoxCPU";
            textBoxCPU.Size = new Size(302, 38);
            textBoxCPU.TabIndex = 16;
            // 
            // buttonSuggest
            // 
            buttonSuggest.Location = new Point(912, 805);
            buttonSuggest.Name = "buttonSuggest";
            buttonSuggest.Size = new Size(132, 51);
            buttonSuggest.TabIndex = 19;
            buttonSuggest.Text = "Suggest";
            buttonSuggest.UseVisualStyleBackColor = true;
            buttonSuggest.Click += new EventHandler(buttonSuggest_Click);
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(label9);
            panel1.Controls.Add(upDownUsage);
            panel1.Controls.Add(buttonEstimate);
            panel1.Controls.Add(checkBoxHighAvailability);
            panel1.Controls.Add(textBoxCPUScore);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(buttonClear);
            panel1.Controls.Add(upDownCores);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(comboBoxDbTypes);
            panel1.Controls.Add(buttonStop);
            panel1.Controls.Add(buttonStart);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(textBoxCPU);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(textBoxStorage);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(upDownInstances);
            panel1.Controls.Add(label4);
            panel1.Location = new Point(36, 70);
            panel1.Name = "panel1";
            panel1.Size = new Size(631, 875);
            panel1.TabIndex = 20;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(473, 304);
            label9.Name = "label9";
            label9.Size = new Size(85, 32);
            label9.TabIndex = 25;
            label9.Text = "h/day";
            // 
            // upDownUsage
            // 
            upDownUsage.Location = new Point(286, 298);
            upDownUsage.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            upDownUsage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            upDownUsage.Name = "upDownUsage";
            upDownUsage.Size = new Size(172, 38);
            upDownUsage.TabIndex = 24;
            upDownUsage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonEstimate
            // 
            buttonEstimate.Location = new Point(479, 790);
            buttonEstimate.Name = "buttonEstimate";
            buttonEstimate.Size = new Size(133, 51);
            buttonEstimate.TabIndex = 23;
            buttonEstimate.Text = "Estimate";
            buttonEstimate.UseVisualStyleBackColor = true;
            buttonEstimate.Click += new EventHandler(buttonEstimate_Click);
            // 
            // checkBoxHighAvailability
            // 
            checkBoxHighAvailability.AutoSize = true;
            checkBoxHighAvailability.Location = new Point(301, 524);
            checkBoxHighAvailability.Name = "checkBoxHighAvailability";
            checkBoxHighAvailability.Size = new Size(264, 36);
            checkBoxHighAvailability.TabIndex = 22;
            checkBoxHighAvailability.Text = "High Availability ";
            checkBoxHighAvailability.UseVisualStyleBackColor = true;
            // 
            // textBoxCPUScore
            // 
            textBoxCPUScore.Location = new Point(286, 702);
            textBoxCPUScore.Name = "textBoxCPUScore";
            textBoxCPUScore.Size = new Size(302, 38);
            textBoxCPUScore.TabIndex = 21;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(36, 702);
            label8.Name = "label8";
            label8.Size = new Size(150, 32);
            label8.TabIndex = 20;
            label8.Text = "CPU score";
            // 
            // buttonClear
            // 
            buttonClear.Location = new Point(12, 790);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(132, 51);
            buttonClear.TabIndex = 19;
            buttonClear.Text = "Clear";
            buttonClear.UseVisualStyleBackColor = true;
            buttonClear.Click += new EventHandler(buttonClear_Click);
            // 
            // upDownCores
            // 
            upDownCores.Location = new Point(433, 233);
            upDownCores.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            upDownCores.Name = "upDownCores";
            upDownCores.Size = new Size(155, 38);
            upDownCores.TabIndex = 10;
            upDownCores.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(30, 233);
            label7.Name = "label7";
            label7.Size = new Size(222, 32);
            label7.TabIndex = 9;
            label7.Text = "Number of cores";
            // 
            // Program
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoScroll = true;
            AutoSize = true;
            ClientSize = new Size(1278, 998);
            Controls.Add(panel1);
            Controls.Add(buttonSuggest);
            Controls.Add(textBoxOutput);
            Name = "Program";
            Text = "Terminator";
            ((System.ComponentModel.ISupportInitialize)(upDownInstances)).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(upDownUsage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(upDownCores)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
                buttonStart.Enabled = false;
                buttonEstimate.Enabled = false;
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
            buttonEstimate.Enabled = true;
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
                textBoxOutput.AppendText(Environment.NewLine);
                return;
            }


            double bestPrice = double.MaxValue;
            string bestProvider = "None";

            foreach (DictionaryEntry pair in prices)
            {
                textBoxOutput.AppendText(string.Format("Provider: {0} Price: {1}", pair.Key, pair.Value));

                if ((double)pair.Value <= bestPrice)
                {                    
                    textBoxOutput.AppendText(Environment.NewLine);
                    textBoxOutput.AppendText(Environment.NewLine);
                    bestPrice = (double)pair.Value;
                    bestProvider = (string)pair.Key;
                }
            }
            textBoxOutput.AppendText(Environment.NewLine);
            textBoxOutput.AppendText("****************************");
            textBoxOutput.AppendText(string.Format("For the specified workload running on {0} the best price in euro/month is {1} using {2}", comboBoxDbTypes.SelectedItem, bestPrice, bestProvider));
            textBoxOutput.AppendText(Environment.NewLine);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            upDownInstances.Value = 1;
            upDownCores.Value = 1;
            upDownUsage.Value = 1;
            textBoxStorage.Clear();
            textBoxCPU.Clear();
        }

        private void buttonEstimate_Click(object sender, EventArgs e)
        {
            getInputs();

            double price = double.MinValue;

            switch (comboBoxDbTypes.SelectedIndex)
            {
                case 0:
                    return;
                case 1:
                    // DB2 - only IBM
                    try
                    {
                        IBMCalculator.RAM = double.Parse(textBoxCPU.Text);
                    }
                    catch (FormatException)
                    {
                        IBMCalculator.RAM = 120;
                    }
                    IBMCalculator.MillionsOfIO = (avgIO * 60 * 60 * (int)upDownUsage.Value * 30) / 1000000;
                    Console.WriteLine("Avg IO " + avgIO + " MIL " + IBMCalculator.MillionsOfIO);
                    IBMCalculator.Storage = double.Parse(textBoxStorage.Text);
                    IBMCalculator.NoOfInstances = (int)upDownInstances.Value;
                    IBMCalculator.HighAvailability = checkBoxHighAvailability.Checked;
                    price = IBMCalculator.CalculateBestPrice(DBType.DB2);
                    price = Math.Round(price * 100) / 100;

                    if (prices.ContainsKey("IBM DB2"))
                        prices["IBM DB2"] = price;
                    else
                        prices.Add("IBM DB2", price);
                    break;
                case 2:
                    // SQL SERVER
                    // Supported on Azure, AWS
                    AzureCalculator.MaxCPU = maxCPU;
                    AzureCalculator.MaxReads = maxRead;
                    AzureCalculator.MaxWrites = maxWrite;
                    AzureCalculator.NoOfCores = (int)upDownCores.Value;
                    AzureCalculator.NoOfHours = (int)upDownUsage.Value;
                    AzureCalculator.Storage = double.Parse(textBoxStorage.Text);

                    price = AzureCalculator.CalculateBestPrice(DBType.SQLServer);
                    price = Math.Round(price * 100) / 100;

                    if (prices.ContainsKey("Azure SQL"))
                        prices["Azure SQL"] = price;
                    else
                        prices.Add("Azure SQL", price);

                    AmazonCalculator.Storage = double.Parse(textBoxStorage.Text);
                    AmazonCalculator.NoOfHours = (int)upDownUsage.Value;
                    AmazonCalculator.NoOfInstances = (int)upDownInstances.Value;
                    price = AmazonCalculator.CalculateBestPrice(DBType.SQLServer);
                    if (prices.ContainsKey("AWS SQL"))
                        prices["AWS SQL"] = price;
                    else
                        prices.Add("AWS SQL", price);
                    break;
                case 3:
                    // PostgreSQL
                    // Supported on IBM, Google, AWS
                    double storage = double.Parse(textBoxStorage.Text);
                    IBMCalculator.Storage = storage;
                    IBMCalculator.NoOfInstances = (int)upDownInstances.Value;

                    price = IBMCalculator.CalculateBestPrice(DBType.PostgreSQL);
                    if (prices.ContainsKey("IBM PostgreSQL"))
                        prices["IBM PostgreSQL"] = price;
                    else
                        prices.Add("IBM PostgreSQL", price);

                    AmazonCalculator.Storage = storage;
                    AmazonCalculator.NoOfHours = (int)upDownUsage.Value;
                    AmazonCalculator.NoOfInstances = (int)upDownInstances.Value;
                    price = AmazonCalculator.CalculateBestPrice(DBType.PostgreSQL);
                    if (prices.ContainsKey("AWS PostgreSQL"))
                        prices["AWS PostgreSQL"] = price;
                    else
                        prices.Add("AWS PostgreSQL", price);
                    break;


                default:
                    break;
            }


            textBoxOutput.AppendText(string.Format("Finished montioring for {0}", comboBoxDbTypes.SelectedItem));
            textBoxOutput.AppendText(Environment.NewLine);
        }

        
        private void getInputs()
        {
            maxCPU = 0;
            maxRead = 0;
            maxWrite = 0;
            avgCPU = 0;
            avgIO = 0;

            for (int i = 0; i < samplesList.Count; i++)
            {
                avgIO += Convert.ToDouble(readList[i]) + Convert.ToDouble(writeList[i]);
                
                avgCPU += Convert.ToDouble(samplesList[i]);

                if (Convert.ToDouble(readList[i]) > maxRead)
                {
                    maxRead = Convert.ToDouble(readList[i]);
                }

                if (Convert.ToDouble(writeList[i]) > maxWrite)
                {
                    maxWrite = Convert.ToDouble(writeList[i]);
                }

                if (Convert.ToDouble(samplesList[i]) > maxCPU)
                {
                    maxCPU = Convert.ToDouble(samplesList[i]);
                }                
            }
            avgIO /= samplesList.Count;
            avgCPU /= samplesList.Count;
        }
    }
}
