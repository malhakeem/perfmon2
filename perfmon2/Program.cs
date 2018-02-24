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
        private CheckBox checkBoxIO;
        private CheckBox checkBoxCPU;
        private Button buttonSuggest;

        private static BackgroundWorker bw = new BackgroundWorker();

        private static PerformanceCounter cpu;
        private static PerformanceCounter read;
        private static PerformanceCounter write;

        private static ArrayList samplesList = new ArrayList();
        private static ArrayList timeList = new ArrayList();
        private static ArrayList readList = new ArrayList();
        private static ArrayList writeList = new ArrayList();

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
            this.checkBoxIO = new System.Windows.Forms.CheckBox();
            this.checkBoxCPU = new System.Windows.Forms.CheckBox();
            this.buttonSuggest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.upDownInstances)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(48, 716);
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
            this.buttonStop.Location = new System.Drawing.Point(222, 716);
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
            this.comboBoxDbTypes.Location = new System.Drawing.Point(202, 82);
            this.comboBoxDbTypes.Name = "comboBoxDbTypes";
            this.comboBoxDbTypes.Size = new System.Drawing.Size(302, 39);
            this.comboBoxDbTypes.TabIndex = 3;
            // 
            // textBoxSize
            // 
            this.textBoxSize.Location = new System.Drawing.Point(202, 190);
            this.textBoxSize.Name = "textBoxSize";
            this.textBoxSize.Size = new System.Drawing.Size(302, 38);
            this.textBoxSize.TabIndex = 4;
            this.textBoxSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSize_KeyPress);
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
            // upDownInstances
            // 
            this.upDownInstances.Location = new System.Drawing.Point(349, 306);
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
            // textBoxUsage
            // 
            this.textBoxUsage.Location = new System.Drawing.Point(158, 416);
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
            this.comboBoxUsageType.Location = new System.Drawing.Point(400, 416);
            this.comboBoxUsageType.Name = "comboBoxUsageType";
            this.comboBoxUsageType.Size = new System.Drawing.Size(121, 39);
            this.comboBoxUsageType.TabIndex = 11;
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxOutput.Location = new System.Drawing.Point(657, 82);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.Size = new System.Drawing.Size(487, 685);
            this.textBoxOutput.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 531);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 32);
            this.label5.TabIndex = 13;
            this.label5.Text = "IO/month";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 617);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 32);
            this.label6.TabIndex = 14;
            this.label6.Text = "CPU %";
            // 
            // textBoxIO
            // 
            this.textBoxIO.Location = new System.Drawing.Point(202, 531);
            this.textBoxIO.Name = "textBoxIO";
            this.textBoxIO.Size = new System.Drawing.Size(302, 38);
            this.textBoxIO.TabIndex = 15;
            // 
            // textBoxCPU
            // 
            this.textBoxCPU.Location = new System.Drawing.Point(202, 617);
            this.textBoxCPU.Name = "textBoxCPU";
            this.textBoxCPU.Size = new System.Drawing.Size(302, 38);
            this.textBoxCPU.TabIndex = 16;
            // 
            // checkBoxIO
            // 
            this.checkBoxIO.AutoSize = true;
            this.checkBoxIO.Location = new System.Drawing.Point(534, 531);
            this.checkBoxIO.Name = "checkBoxIO";
            this.checkBoxIO.Size = new System.Drawing.Size(34, 33);
            this.checkBoxIO.TabIndex = 17;
            this.checkBoxIO.UseVisualStyleBackColor = true;
            // 
            // checkBoxCPU
            // 
            this.checkBoxCPU.AutoSize = true;
            this.checkBoxCPU.Location = new System.Drawing.Point(534, 613);
            this.checkBoxCPU.Name = "checkBoxCPU";
            this.checkBoxCPU.Size = new System.Drawing.Size(34, 33);
            this.checkBoxCPU.TabIndex = 18;
            this.checkBoxCPU.UseVisualStyleBackColor = true;
            // 
            // buttonSuggest
            // 
            this.buttonSuggest.Location = new System.Drawing.Point(401, 716);
            this.buttonSuggest.Name = "buttonSuggest";
            this.buttonSuggest.Size = new System.Drawing.Size(132, 51);
            this.buttonSuggest.TabIndex = 19;
            this.buttonSuggest.Text = "Suggest";
            this.buttonSuggest.UseVisualStyleBackColor = true;
            this.buttonSuggest.Click += new System.EventHandler(this.buttonSuggest_Click);
            // 
            // Program
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1278, 859);
            this.Controls.Add(this.buttonSuggest);
            this.Controls.Add(this.checkBoxCPU);
            this.Controls.Add(this.checkBoxIO);
            this.Controls.Add(this.textBoxCPU);
            this.Controls.Add(this.textBoxIO);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.comboBoxUsageType);
            this.Controls.Add(this.textBoxUsage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.upDownInstances);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSize);
            this.Controls.Add(this.comboBoxDbTypes);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Name = "Program";
            this.Text = "What is the name of this?";
            ((System.ComponentModel.ISupportInitialize)(this.upDownInstances)).EndInit();
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

            textBoxOutput.AppendText(string.Format("Finished montioring for {0} \n", comboBoxDbTypes.SelectedItem));
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
            // TO DO
            // call price estimators and suggest the best plan
            textBoxOutput.AppendText("TO DO :) ");
        }
    }
}
