using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace perfmon2
{
    enum DBType { PostgreSQL = 1, SQLServer, DB2 };

    class Program : Form
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
        private TextBox textBoxRAM;
        private Button buttonSuggest;
        private Panel panel1;
        private NumericUpDown upDownCores;
        private Label label7;
        private Button buttonClear;
        private CheckBox checkBoxHighAvailability;
        private Button buttonEstimate;
        private NumericUpDown upDownUsage;
        private Label label9;
        private NumericUpDown upDownvCPU;
        private Label label5;
        private ErrorProvider errorProvider1;
        private IContainer components;

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

        Program()
        {
            // Sets up the UI
            InitializeComponent();

            // Windows Perfromance Counters
            CreateCounters();

            // Background worker that will monitor the system performance
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

        // Gets CPU usage %, Reads and Writes per second
        private static void CreateCounters()
        {
            cpu = new PerformanceCounter("Processor Information", "% Processor Time", "_Total", true);
            read = new PerformanceCounter("PhysicalDisk", "Disk Reads/sec", "_Total", true);
            write = new PerformanceCounter("PhysicalDisk", "Disk Writes/sec", "_Total");
        }

        // Converts timestamp to formatted string
        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // Rouds the price to d decimal points
        private static double RoundPrice(double price, int d = 2)
        {
            if (d < 0)
                d = 0;
            int p = (int)Math.Pow(10, d);
            return price = Math.Round(price * p) / p;
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

        // Background worker "Do Work" action
        // Collects system information from the Perfromance Counters
        // CPU%, Writes/sec, Reads/sec 
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            samplesList = new ArrayList();
            timeList = new ArrayList();
            readList = new ArrayList();
            writeList = new ArrayList();

            while (true)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    timeList.Add(GetTimestamp(DateTime.Now));
                    samplesList.Add(cpu.NextValue());
                    readList.Add(read.NextValue());
                    writeList.Add(write.NextValue());
                    // Sleep for one second
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        // Background worker "Completed" action
        // Writes the system information to a csv file
        // and calculates the necessary statistics: MaxCPU%, Max Writes, Max Reads, Average IOPS 
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Error == null))
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

        // Sets up the UI components
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.comboBoxDbTypes = new System.Windows.Forms.ComboBox();
            this.textBoxStorage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.upDownInstances = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxRAM = new System.Windows.Forms.TextBox();
            this.buttonSuggest = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.upDownvCPU = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.upDownUsage = new System.Windows.Forms.NumericUpDown();
            this.buttonEstimate = new System.Windows.Forms.Button();
            this.checkBoxHighAvailability = new System.Windows.Forms.CheckBox();
            this.upDownCores = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonClear = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.upDownInstances)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownvCPU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownUsage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownCores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(35, 772);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(151, 51);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(236, 772);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(147, 51);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // comboBoxDbTypes
            // 
            this.comboBoxDbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDbTypes.FormattingEnabled = true;
            this.comboBoxDbTypes.Items.AddRange(new object[] {
            "--select a database--",
            "DB2",
            "MS SQL Server",
            "PostgreSQL"});
            this.comboBoxDbTypes.Location = new System.Drawing.Point(286, 39);
            this.comboBoxDbTypes.Name = "comboBoxDbTypes";
            this.comboBoxDbTypes.Size = new System.Drawing.Size(302, 39);
            this.comboBoxDbTypes.Sorted = true;
            this.comboBoxDbTypes.TabIndex = 3;
            comboBoxDbTypes.SelectedIndex = 0;
            // 
            // textBoxStorage
            // 
            this.textBoxStorage.Location = new System.Drawing.Point(286, 102);
            this.textBoxStorage.Name = "textBoxStorage";
            this.textBoxStorage.Size = new System.Drawing.Size(302, 38);
            this.textBoxStorage.TabIndex = 4;
            this.textBoxStorage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
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
            this.label2.Size = new System.Drawing.Size(181, 32);
            this.label2.TabIndex = 6;
            this.label2.Text = "Storage (GB)";
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
            this.label4.Location = new System.Drawing.Point(30, 242);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 32);
            this.label4.TabIndex = 9;
            this.label4.Text = "Usage";
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxOutput.Location = new System.Drawing.Point(735, 70);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.Size = new System.Drawing.Size(487, 685);
            this.textBoxOutput.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 439);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(143, 32);
            this.label6.TabIndex = 14;
            this.label6.Text = "RAM (GB)";
            // 
            // textBoxRAM
            // 
            this.textBoxRAM.Location = new System.Drawing.Point(286, 433);
            this.textBoxRAM.Name = "textBoxRAM";
            this.textBoxRAM.Size = new System.Drawing.Size(302, 38);
            this.textBoxRAM.TabIndex = 16;
            this.textBoxRAM.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // buttonSuggest
            // 
            this.buttonSuggest.Location = new System.Drawing.Point(1027, 805);
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
            this.panel1.Controls.Add(this.upDownvCPU);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.upDownUsage);
            this.panel1.Controls.Add(this.buttonEstimate);
            this.panel1.Controls.Add(this.checkBoxHighAvailability);
            this.panel1.Controls.Add(this.upDownCores);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.comboBoxDbTypes);
            this.panel1.Controls.Add(this.buttonStop);
            this.panel1.Controls.Add(this.buttonStart);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxRAM);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBoxStorage);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.upDownInstances);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(36, 70);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(631, 875);
            this.panel1.TabIndex = 20;
            // 
            // upDownvCPU
            // 
            this.upDownvCPU.Location = new System.Drawing.Point(433, 508);
            this.upDownvCPU.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownvCPU.Name = "upDownvCPU";
            this.upDownvCPU.Size = new System.Drawing.Size(155, 38);
            this.upDownvCPU.TabIndex = 27;
            this.upDownvCPU.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 508);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 32);
            this.label5.TabIndex = 26;
            this.label5.Text = "vCPUs";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(480, 244);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 32);
            this.label9.TabIndex = 25;
            this.label9.Text = "h/day";
            // 
            // upDownUsage
            // 
            this.upDownUsage.Location = new System.Drawing.Point(341, 236);
            this.upDownUsage.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.upDownUsage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownUsage.Name = "upDownUsage";
            this.upDownUsage.Size = new System.Drawing.Size(117, 38);
            this.upDownUsage.TabIndex = 24;
            this.upDownUsage.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            // 
            // buttonEstimate
            // 
            this.buttonEstimate.Location = new System.Drawing.Point(433, 772);
            this.buttonEstimate.Name = "buttonEstimate";
            this.buttonEstimate.Size = new System.Drawing.Size(145, 51);
            this.buttonEstimate.TabIndex = 23;
            this.buttonEstimate.Text = "Estimate";
            this.buttonEstimate.UseVisualStyleBackColor = true;
            this.buttonEstimate.Click += new System.EventHandler(this.buttonEstimate_Click);
            // 
            // checkBoxHighAvailability
            // 
            this.checkBoxHighAvailability.AutoSize = true;
            this.checkBoxHighAvailability.Location = new System.Drawing.Point(301, 326);
            this.checkBoxHighAvailability.Name = "checkBoxHighAvailability";
            this.checkBoxHighAvailability.Size = new System.Drawing.Size(264, 36);
            this.checkBoxHighAvailability.TabIndex = 22;
            this.checkBoxHighAvailability.Text = "High Availability ";
            this.checkBoxHighAvailability.UseVisualStyleBackColor = true;
            // 
            // upDownCores
            // 
            this.upDownCores.Location = new System.Drawing.Point(433, 583);
            this.upDownCores.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
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
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 585);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(222, 32);
            this.label7.TabIndex = 9;
            this.label7.Text = "Number of cores";
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(812, 805);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(132, 51);
            this.buttonClear.TabIndex = 19;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // Program
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1278, 998);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonSuggest);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.buttonClear);
            this.Name = "Program";
            this.Text = "The estimator";
            ((System.ComponentModel.ISupportInitialize)(this.upDownInstances)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownvCPU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownUsage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownCores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        // Starts the performance monitoring
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
                buttonStart.Enabled = false;
                buttonEstimate.Enabled = false;
                buttonStop.Enabled = true;
            }
        }

        // Stops the performance monitoring
        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }
            buttonStart.Enabled = true;
            buttonEstimate.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
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
            StringBuilder bestProvider = new StringBuilder(); 

            foreach (DictionaryEntry pair in prices)
            {
                textBoxOutput.AppendText(string.Format("Provider: {0} Price: {1}", pair.Key, pair.Value));
                textBoxOutput.AppendText(Environment.NewLine);

                if ((double)pair.Value == bestPrice)
                {
                    bestProvider.Append(", " + (string)pair.Key);
                }

                if ((double)pair.Value < bestPrice)
                {
                    bestPrice = (double)pair.Value;
                    bestProvider.Clear();
                    bestProvider.Append((string)pair.Key);
                }

            }
            textBoxOutput.AppendText(Environment.NewLine);
            textBoxOutput.AppendText("****************************");
            textBoxOutput.AppendText(Environment.NewLine);
            textBoxOutput.AppendText(string.Format("For the specified workload the best price is {0} euro/month{1}on {2}", bestPrice, Environment.NewLine, bestProvider));
            textBoxOutput.AppendText(Environment.NewLine);
        }


        // Clears the text
        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxOutput.Clear();
        }

        // Based on database selection, provided inputs, and collected system information
        // calculates the possible prices and stores them
        private void buttonEstimate_Click(object sender, EventArgs e)
        {
            bool error = false;
            if (string.IsNullOrEmpty(textBoxRAM.Text))
            {
                errorProvider1.SetError(textBoxRAM, "Please enter RAM value.");
                error = true;
            }
            if (string.IsNullOrEmpty(textBoxStorage.Text))
            {
                errorProvider1.SetError(textBoxStorage, "Please enter Storage value.");
                error = true;
            }
            if (error)
                return;
            errorProvider1.SetError(textBoxRAM, string.Empty);
            errorProvider1.SetError(textBoxStorage, string.Empty);

            double price = double.MinValue;

            switch (comboBoxDbTypes.SelectedIndex)
            {
                case 0:
                    textBoxOutput.AppendText(string.Format("Please select a database first {0}", Environment.NewLine));
                    return;
                case 1:
                    // DB2
                    //  - IBM
                    try
                    {
                        IBMCalculator.RAM = double.Parse(textBoxRAM.Text);
                    }
                    catch (FormatException)
                    {
                        IBMCalculator.RAM = 120;
                    }
                    IBMCalculator.MillionsOfIO = (avgIO * 60 * 60 * (int)upDownUsage.Value * 30) / 1000000;
                    IBMCalculator.Storage = double.Parse(textBoxStorage.Text);
                    IBMCalculator.NoOfInstances = (int)upDownInstances.Value;
                    IBMCalculator.HighAvailability = checkBoxHighAvailability.Checked;
                    price = IBMCalculator.CalculateBestPrice(DBType.DB2);
                    price = RoundPrice(price);

                    if (checkBoxHighAvailability.Checked)
                    {
                        if (prices.ContainsKey("IBM DB2 HA"))
                            prices["IBM DB2 HA"] = price;
                        else
                            prices.Add("IBM DB2 HA", price);
                    }
                    else
                    {
                        if (prices.ContainsKey("IBM DB2 Normal"))
                            prices["IBM DB2 Normal"] = price;
                        else
                            prices.Add("IBM DB2 Normal", price);
                    }
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
                    price = RoundPrice(price);

                    if (prices.ContainsKey("Azure SQL"))
                        prices["Azure SQL"] = price;
                    else
                        prices.Add("Azure SQL", price);

                    AmazonCalculator.Storage = double.Parse(textBoxStorage.Text);
                    AmazonCalculator.NoOfHours = (int)upDownUsage.Value;
                    AmazonCalculator.NoOfInstances = (int)upDownInstances.Value;
                    AmazonCalculator.vCPUs = (int)upDownvCPU.Value;
                    AmazonCalculator.RAM = double.Parse(textBoxRAM.Text);
                    AmazonCalculator.IOPS = avgIO;

                    price = AmazonCalculator.CalculateBestPrice(DBType.SQLServer);
                    price = RoundPrice(price);

                    if (prices.ContainsKey("AWS SQL Server"))
                        prices["AWS SQL Server"] = price;
                    else
                        prices.Add("AWS SQL Server", price);
                    break;
                case 3:
                    // PostgreSQL
                    //  - IBM, AWS
                    double storage = double.Parse(textBoxStorage.Text);
                    IBMCalculator.Storage = storage;
                    IBMCalculator.NoOfInstances = (int)upDownInstances.Value;

                    price = IBMCalculator.CalculateBestPrice(DBType.PostgreSQL);
                    price = RoundPrice(price);

                    if (prices.ContainsKey("IBM PostgreSQL"))
                        prices["IBM PostgreSQL"] = price;
                    else
                        prices.Add("IBM PostgreSQL", price);

                    AmazonCalculator.Storage = storage;
                    AmazonCalculator.NoOfHours = (int)upDownUsage.Value;
                    AmazonCalculator.NoOfInstances = (int)upDownInstances.Value;
                    price = AmazonCalculator.CalculateBestPrice(DBType.PostgreSQL);
                    price = RoundPrice(price);

                    if (prices.ContainsKey("AWS PostgreSQL"))
                        prices["AWS PostgreSQL"] = price;
                    else
                        prices.Add("AWS PostgreSQL", price);
                    break;

                default:
                    textBoxOutput.AppendText(string.Format("Please select a database first {0}", Environment.NewLine));
                    break;
            }            
        }        
    }
}
