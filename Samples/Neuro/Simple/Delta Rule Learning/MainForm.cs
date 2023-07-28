// Classifier using Delta Rule Learning 
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Threading;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;
using System.Globalization;

namespace Classifier
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private GroupBox groupBox1;
        private Button loadButton;
        private OpenFileDialog openFileDialog;
        private ListView dataList;
        private GroupBox groupBox2;
        private TextBox learningRateBox;
        private Label label1;
        private Label label2;
        private TextBox alphaBox;
        private Label label3;
        private TextBox errorLimitBox;
        private Label label4;
        private TextBox iterationsBox;
        private Label label5;
        private Label label6;
        private TextBox neuronsBox;
        private CheckBox oneNeuronForTwoCheck;
        private Label label7;
        private Label label8;
        private TextBox currentIterationBox;
        private Button stopButton;
        private Button startButton;
        private Label label9;
        private Label label10;
        private TextBox classesBox;
        private CheckBox errorLimitCheck;
        private Label label11;
        private TextBox currentErrorBox;
        private GroupBox groupBox3;
        private Label label12;
        private ListView weightsList;
        private Label label13;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private CheckBox saveFilesCheck;
        private Chart errorChart;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private int samples = 0;
        private int variables = 0;
        private double[,] data = null;
        private int[] classes = null;
        private int classesCount = 0;
        private int[] samplesPerClass = null;
        private int neuronsCount = 0;

        private double learningRate = 0.1;
        private double sigmoidAlphaValue = 2.0;
        private double learningErrorLimit = 0.1;
        private double iterationLimit = 1000;
        private bool useOneNeuronForTwoClasses = false;
        private bool useErrorLimit = true;
        private bool saveStatisticsToFiles = false;

        private Thread workerThread = null;
        private volatile bool needToStop = false;

        // Constructor
        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // update settings controls
            UpdateSettings();

            // initialize charts
            errorChart.AddDataSeries("error", Color.Red, Chart.SeriesType.Line, 1);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
            groupBox1 = new GroupBox();
            classesBox = new TextBox();
            label10 = new Label();
            dataList = new ListView();
            loadButton = new Button();
            openFileDialog = new OpenFileDialog();
            groupBox2 = new GroupBox();
            currentErrorBox = new TextBox();
            label11 = new Label();
            label9 = new Label();
            currentIterationBox = new TextBox();
            label8 = new Label();
            label7 = new Label();
            errorLimitCheck = new CheckBox();
            oneNeuronForTwoCheck = new CheckBox();
            neuronsBox = new TextBox();
            label6 = new Label();
            label5 = new Label();
            iterationsBox = new TextBox();
            label4 = new Label();
            errorLimitBox = new TextBox();
            label3 = new Label();
            alphaBox = new TextBox();
            label2 = new Label();
            label1 = new Label();
            learningRateBox = new TextBox();
            stopButton = new Button();
            startButton = new Button();
            groupBox3 = new GroupBox();
            saveFilesCheck = new CheckBox();
            label13 = new Label();
            weightsList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            errorChart = new Chart();
            label12 = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(classesBox);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(dataList);
            groupBox1.Controls.Add(loadButton);
            groupBox1.Location = new System.Drawing.Point(18, 18);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(414, 610);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Data";
            // 
            // classesBox
            // 
            classesBox.Location = new System.Drawing.Point(342, 548);
            classesBox.Name = "classesBox";
            classesBox.ReadOnly = true;
            classesBox.Size = new Size(54, 31);
            classesBox.TabIndex = 3;
            // 
            // label10
            // 
            label10.Location = new System.Drawing.Point(252, 552);
            label10.Name = "label10";
            label10.Size = new Size(90, 22);
            label10.TabIndex = 2;
            label10.Text = "Classes:";
            // 
            // dataList
            // 
            dataList.FullRowSelect = true;
            dataList.GridLines = true;
            dataList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            dataList.Location = new System.Drawing.Point(18, 37);
            dataList.Name = "dataList";
            dataList.Size = new Size(378, 498);
            dataList.TabIndex = 0;
            dataList.UseCompatibleStateImageBehavior = false;
            dataList.View = View.Details;
            // 
            // loadButton
            // 
            loadButton.Location = new System.Drawing.Point(18, 548);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(135, 43);
            loadButton.TabIndex = 1;
            loadButton.Text = "&Load";
            loadButton.Click += loadButton_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            openFileDialog.Title = "Select data file";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(currentErrorBox);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(currentIterationBox);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(errorLimitCheck);
            groupBox2.Controls.Add(oneNeuronForTwoCheck);
            groupBox2.Controls.Add(neuronsBox);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(iterationsBox);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(errorLimitBox);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(alphaBox);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(learningRateBox);
            groupBox2.Controls.Add(stopButton);
            groupBox2.Controls.Add(startButton);
            groupBox2.Location = new System.Drawing.Point(450, 18);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(333, 610);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Training";
            // 
            // currentErrorBox
            // 
            currentErrorBox.Location = new System.Drawing.Point(225, 471);
            currentErrorBox.Name = "currentErrorBox";
            currentErrorBox.ReadOnly = true;
            currentErrorBox.Size = new Size(90, 31);
            currentErrorBox.TabIndex = 20;
            // 
            // label11
            // 
            label11.Location = new System.Drawing.Point(18, 474);
            label11.Name = "label11";
            label11.Size = new Size(218, 26);
            label11.TabIndex = 19;
            label11.Text = "Current average error:";
            // 
            // label9
            // 
            label9.BorderStyle = BorderStyle.FixedSingle;
            label9.Location = new System.Drawing.Point(18, 522);
            label9.Name = "label9";
            label9.Size = new Size(297, 4);
            label9.TabIndex = 18;
            // 
            // currentIterationBox
            // 
            currentIterationBox.Location = new System.Drawing.Point(225, 425);
            currentIterationBox.Name = "currentIterationBox";
            currentIterationBox.ReadOnly = true;
            currentIterationBox.Size = new Size(90, 31);
            currentIterationBox.TabIndex = 17;
            // 
            // label8
            // 
            label8.Location = new System.Drawing.Point(18, 428);
            label8.Name = "label8";
            label8.Size = new Size(176, 30);
            label8.TabIndex = 16;
            label8.Text = "Current iteration:";
            // 
            // label7
            // 
            label7.BorderStyle = BorderStyle.FixedSingle;
            label7.Location = new System.Drawing.Point(18, 406);
            label7.Name = "label7";
            label7.Size = new Size(297, 4);
            label7.TabIndex = 15;
            // 
            // errorLimitCheck
            // 
            errorLimitCheck.Location = new System.Drawing.Point(18, 342);
            errorLimitCheck.Name = "errorLimitCheck";
            errorLimitCheck.Size = new Size(283, 55);
            errorLimitCheck.TabIndex = 14;
            errorLimitCheck.Text = "Use error limit (checked) or iterations limit";
            // 
            // oneNeuronForTwoCheck
            // 
            oneNeuronForTwoCheck.Enabled = false;
            oneNeuronForTwoCheck.Location = new System.Drawing.Point(18, 305);
            oneNeuronForTwoCheck.Name = "oneNeuronForTwoCheck";
            oneNeuronForTwoCheck.Size = new Size(302, 27);
            oneNeuronForTwoCheck.TabIndex = 13;
            oneNeuronForTwoCheck.Text = "Use 1 neuron for 2 classes";
            oneNeuronForTwoCheck.CheckedChanged += oneNeuronForTwoCheck_CheckedChanged;
            // 
            // neuronsBox
            // 
            neuronsBox.Location = new System.Drawing.Point(225, 249);
            neuronsBox.Name = "neuronsBox";
            neuronsBox.ReadOnly = true;
            neuronsBox.Size = new Size(90, 31);
            neuronsBox.TabIndex = 12;
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(18, 253);
            label6.Name = "label6";
            label6.Size = new Size(106, 22);
            label6.TabIndex = 11;
            label6.Text = "Neurons:";
            // 
            // label5
            // 
            label5.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new System.Drawing.Point(225, 212);
            label5.Name = "label5";
            label5.Size = new Size(104, 32);
            label5.TabIndex = 10;
            label5.Text = "( 0 - inifinity )";
            // 
            // iterationsBox
            // 
            iterationsBox.Location = new System.Drawing.Point(225, 175);
            iterationsBox.Name = "iterationsBox";
            iterationsBox.Size = new Size(90, 31);
            iterationsBox.TabIndex = 9;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(18, 179);
            label4.Name = "label4";
            label4.Size = new Size(162, 24);
            label4.TabIndex = 8;
            label4.Text = "Iterations limit:";
            // 
            // errorLimitBox
            // 
            errorLimitBox.Location = new System.Drawing.Point(225, 129);
            errorLimitBox.Name = "errorLimitBox";
            errorLimitBox.Size = new Size(90, 31);
            errorLimitBox.TabIndex = 7;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(18, 133);
            label3.Name = "label3";
            label3.Size = new Size(198, 28);
            label3.TabIndex = 6;
            label3.Text = "Learning error limit:";
            // 
            // alphaBox
            // 
            alphaBox.Location = new System.Drawing.Point(225, 83);
            alphaBox.Name = "alphaBox";
            alphaBox.Size = new Size(90, 31);
            alphaBox.TabIndex = 5;
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(18, 87);
            label2.Name = "label2";
            label2.Size = new Size(216, 27);
            label2.TabIndex = 4;
            label2.Text = "Sigmoid's alpha value:";
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(18, 41);
            label1.Name = "label1";
            label1.Size = new Size(135, 29);
            label1.TabIndex = 2;
            label1.Text = "Learning rate:";
            // 
            // learningRateBox
            // 
            learningRateBox.Location = new System.Drawing.Point(225, 37);
            learningRateBox.Name = "learningRateBox";
            learningRateBox.Size = new Size(90, 31);
            learningRateBox.TabIndex = 3;
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new System.Drawing.Point(180, 548);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(135, 43);
            stopButton.TabIndex = 6;
            stopButton.Text = "S&top";
            stopButton.Click += stopButton_Click;
            // 
            // startButton
            // 
            startButton.Enabled = false;
            startButton.Location = new System.Drawing.Point(18, 548);
            startButton.Name = "startButton";
            startButton.Size = new Size(135, 43);
            startButton.TabIndex = 5;
            startButton.Text = "&Start";
            startButton.Click += startButton_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(saveFilesCheck);
            groupBox3.Controls.Add(label13);
            groupBox3.Controls.Add(weightsList);
            groupBox3.Controls.Add(errorChart);
            groupBox3.Controls.Add(label12);
            groupBox3.Location = new System.Drawing.Point(801, 18);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(396, 610);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Solution";
            // 
            // saveFilesCheck
            // 
            saveFilesCheck.Location = new System.Drawing.Point(18, 563);
            saveFilesCheck.Name = "saveFilesCheck";
            saveFilesCheck.Size = new Size(351, 28);
            saveFilesCheck.TabIndex = 4;
            saveFilesCheck.Text = "Save weights and errors to files";
            // 
            // label13
            // 
            label13.Location = new System.Drawing.Point(18, 314);
            label13.Name = "label13";
            label13.Size = new Size(180, 22);
            label13.TabIndex = 3;
            label13.Text = "Error's dynamics:";
            // 
            // weightsList
            // 
            weightsList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            weightsList.FullRowSelect = true;
            weightsList.GridLines = true;
            weightsList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            weightsList.Location = new System.Drawing.Point(18, 65);
            weightsList.Name = "weightsList";
            weightsList.Size = new Size(360, 240);
            weightsList.TabIndex = 2;
            weightsList.UseCompatibleStateImageBehavior = false;
            weightsList.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Neuron";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Weight";
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Value";
            // 
            // errorChart
            // 
            errorChart.Location = new System.Drawing.Point(18, 342);
            errorChart.Name = "errorChart";
            errorChart.RangeX = (AForge.Range)resources.GetObject("errorChart.RangeX");
            errorChart.RangeY = (AForge.Range)resources.GetObject("errorChart.RangeY");
            errorChart.Size = new Size(360, 203);
            errorChart.TabIndex = 1;
            errorChart.Text = "chart1";
            // 
            // label12
            // 
            label12.Location = new System.Drawing.Point(18, 37);
            label12.Name = "label12";
            label12.Size = new Size(180, 28);
            label12.TabIndex = 0;
            label12.Text = "Network weights:";
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(9, 24);
            ClientSize = new Size(1213, 662);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Classifier using Delta Rule Learning";
            Closing += MainForm_Closing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion



        // Delegates to enable async calls for setting controls properties
        private delegate void SetTextCallback(Control control, string text);
        private delegate void ClearListCallback(ListView control);
        private delegate ListViewItem AddListItemCallback(ListView control, string itemText);
        private delegate void AddListSubitemCallback(ListViewItem item, string subItemText);

        // Thread safe updating of control's text property
        private void SetText(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { control, text });
            }
            else
            {
                control.Text = text;
            }
        }

        // Thread safe clearing of list view
        private void ClearList(ListView control)
        {
            if (control.InvokeRequired)
            {
                ClearListCallback d = new ClearListCallback(ClearList);
                Invoke(d, new object[] { control });
            }
            else
            {
                control.Items.Clear();
            }
        }

        // Thread safe adding of item to list control
        private ListViewItem AddListItem(ListView control, string itemText)
        {
            ListViewItem item = null;

            if (control.InvokeRequired)
            {
                AddListItemCallback d = new AddListItemCallback(AddListItem);
                item = (ListViewItem)Invoke(d, new object[] { control, itemText });
            }
            else
            {
                item = control.Items.Add(itemText);
            }

            return item;
        }

        // Thread safe adding of subitem to list control
        private void AddListSubitem(ListViewItem item, string subItemText)
        {
            if (this.InvokeRequired)
            {
                AddListSubitemCallback d = new AddListSubitemCallback(AddListSubitem);
                Invoke(d, new object[] { item, subItemText });
            }
            else
            {
                item.SubItems.Add(subItemText);
            }
        }

        // On main form closing
        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            // check if worker thread is running
            if ((workerThread != null) && (workerThread.IsAlive))
            {
                needToStop = true;
                while (!workerThread.Join(100))
                    Application.DoEvents();
            }
        }

        // Load input data
        private void loadButton_Click(object sender, EventArgs e)
        {
            // data file format:
            // X1, X2, ..., Xn, class

            // load maximum 10 classes !

            // show file selection dialog
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = null;

                // temp buffers (for 200 samples only)
                double[,] tempData = null;
                int[] tempClasses = new int[200];

                // min and max X values
                double minX = double.MaxValue;
                double maxX = double.MinValue;

                // samples count
                samples = 0;
                // classes count
                classesCount = 0;
                samplesPerClass = new int[10];

                try
                {
                    string str = null;

                    // open selected file
                    reader = File.OpenText(openFileDialog.FileName);

                    // read the data
                    while ((samples < 200) && ((str = reader.ReadLine()) != null))
                    {
                        // split the string
                        string[] strs = str.Split(';');
                        if (strs.Length == 1)
                            strs = str.Split(',');

                        // allocate data array
                        if (samples == 0)
                        {
                            variables = strs.Length - 1;
                            tempData = new double[200, variables];
                        }

                        // parse data
                        for (int j = 0; j < variables; j++)
                        {
                            tempData[samples, j] = double.Parse(strs[j], CultureInfo.InvariantCulture);
                        }
                        tempClasses[samples] = int.Parse(strs[variables]);

                        // skip classes over 10, except only first 10 classes
                        if (tempClasses[samples] >= 10)
                            continue;

                        // count the amount of different classes
                        if (tempClasses[samples] >= classesCount)
                            classesCount = tempClasses[samples] + 1;
                        // count samples per class
                        samplesPerClass[tempClasses[samples]]++;

                        // search for min value
                        if (tempData[samples, 0] < minX)
                            minX = tempData[samples, 0];
                        // search for max value
                        if (tempData[samples, 0] > maxX)
                            maxX = tempData[samples, 0];

                        samples++;
                    }

                    // allocate and set data
                    data = new double[samples, variables];
                    Array.Copy(tempData, 0, data, 0, samples * variables);
                    classes = new int[samples];
                    Array.Copy(tempClasses, 0, classes, 0, samples);
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed reading the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    // close file
                    if (reader != null)
                        reader.Close();
                }

                // update list and chart
                UpdateDataListView();

                classesBox.Text = classesCount.ToString();
                oneNeuronForTwoCheck.Enabled = classesCount == 2;

                // set neurons count
                neuronsCount = ((classesCount == 2) && (useOneNeuronForTwoClasses)) ? 1 : classesCount;
                neuronsBox.Text = neuronsCount.ToString();

                ClearSolution();
                startButton.Enabled = true;
            }
        }

        // Update settings controls
        private void UpdateSettings()
        {
            learningRateBox.Text = learningRate.ToString();
            alphaBox.Text = sigmoidAlphaValue.ToString();
            errorLimitBox.Text = learningErrorLimit.ToString();
            iterationsBox.Text = iterationLimit.ToString();

            oneNeuronForTwoCheck.Checked = useOneNeuronForTwoClasses;
            errorLimitCheck.Checked = useErrorLimit;
            saveFilesCheck.Checked = saveStatisticsToFiles;
        }

        // Update data in list view
        private void UpdateDataListView()
        {
            // remove all curent data and columns
            dataList.Items.Clear();
            dataList.Columns.Clear();

            // add columns
            for (int i = 0, n = variables; i < n; i++)
            {
                dataList.Columns.Add(string.Format("X{0}", i + 1),
                    50, HorizontalAlignment.Left);
            }
            dataList.Columns.Add("Class", 50, HorizontalAlignment.Left);

            // add items
            for (int i = 0; i < samples; i++)
            {
                dataList.Items.Add(data[i, 0].ToString());

                for (int j = 1; j < variables; j++)
                {
                    dataList.Items[i].SubItems.Add(data[i, j].ToString());
                }
                dataList.Items[i].SubItems.Add(classes[i].ToString());
            }
        }

        // Use or not one neuron to classify two classes
        private void oneNeuronForTwoCheck_CheckedChanged(object sender, EventArgs e)
        {
            useOneNeuronForTwoClasses = oneNeuronForTwoCheck.Checked;
            // update neurons count box
            neuronsCount = ((classesCount == 2) && (useOneNeuronForTwoClasses)) ? 1 : classesCount;
            neuronsBox.Text = neuronsCount.ToString();
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback(bool enable);

        // Enable/disale controls (safe for threading)
        private void EnableControls(bool enable)
        {
            if (InvokeRequired)
            {
                EnableCallback d = new EnableCallback(EnableControls);
                Invoke(d, new object[] { enable });
            }
            else
            {
                learningRateBox.Enabled = enable;
                alphaBox.Enabled = enable;
                errorLimitBox.Enabled = enable;
                iterationsBox.Enabled = enable;
                oneNeuronForTwoCheck.Enabled = (enable) && (classesCount == 2);
                errorLimitCheck.Enabled = enable;
                saveFilesCheck.Enabled = enable;

                loadButton.Enabled = enable;
                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // Clear current solution
        private void ClearSolution()
        {
            errorChart.UpdateDataSeries("error", null);
            weightsList.Items.Clear();
            currentIterationBox.Text = string.Empty;
            currentErrorBox.Text = string.Empty;
        }

        // On "Start" button click
        private void startButton_Click(object sender, EventArgs e)
        {
            // get learning rate
            try
            {
                learningRate = Math.Max(0.00001, Math.Min(1, double.Parse(learningRateBox.Text)));
            }
            catch
            {
                learningRate = 0.1;
            }
            // get sigmoid's alpha value
            try
            {
                sigmoidAlphaValue = Math.Max(0.01, Math.Min(100, double.Parse(alphaBox.Text)));
            }
            catch
            {
                sigmoidAlphaValue = 2;
            }
            // get learning error limit
            try
            {
                learningErrorLimit = Math.Max(0, double.Parse(errorLimitBox.Text));
            }
            catch
            {
                learningErrorLimit = 0.1;
            }
            // get iterations limit
            try
            {
                iterationLimit = Math.Max(0, int.Parse(iterationsBox.Text));
            }
            catch
            {
                iterationLimit = 1000;
            }

            useOneNeuronForTwoClasses = oneNeuronForTwoCheck.Checked;
            useErrorLimit = errorLimitCheck.Checked;
            saveStatisticsToFiles = saveFilesCheck.Checked;

            // update settings controls
            UpdateSettings();

            // disable all settings controls
            EnableControls(false);

            // run worker thread
            needToStop = false;
            workerThread = new Thread(new ThreadStart(SearchSolution));
            workerThread.Start();
        }

        // On "Stop" button click
        private void stopButton_Click(object sender, EventArgs e)
        {
            // stop worker thread
            needToStop = true;
            while (!workerThread.Join(100))
                Application.DoEvents();
            workerThread = null;
        }

        // Worker thread
        void SearchSolution()
        {
            bool reducedNetwork = ((classesCount == 2) && (useOneNeuronForTwoClasses));

            // prepare learning data
            double[][] input = new double[samples][];
            double[][] output = new double[samples][];

            for (int i = 0; i < samples; i++)
            {
                input[i] = new double[variables];
                output[i] = new double[neuronsCount];

                // set input
                for (int j = 0; j < variables; j++)
                    input[i][j] = data[i, j];
                // set output
                if (reducedNetwork)
                {
                    output[i][0] = classes[i];
                }
                else
                {
                    output[i][classes[i]] = 1;
                }
            }

            // create perceptron
            ActivationNetwork network = new ActivationNetwork(
                new SigmoidFunction(sigmoidAlphaValue), variables, neuronsCount);
            ActivationLayer layer = network.Layers[0] as ActivationLayer;
            // create teacher
            DeltaRuleLearning teacher = new DeltaRuleLearning(network);
            // set learning rate
            teacher.LearningRate = learningRate;

            // iterations
            int iteration = 1;

            // statistic files
            StreamWriter errorsFile = null;
            StreamWriter weightsFile = null;

            try
            {
                // check if we need to save statistics to files
                if (saveStatisticsToFiles)
                {
                    // open files
                    errorsFile = File.CreateText("errors.csv");
                    weightsFile = File.CreateText("weights.csv");
                }

                // erros list
                ArrayList errorsList = new ArrayList();

                // loop
                while (!needToStop)
                {
                    // save current weights
                    if (weightsFile != null)
                    {
                        for (int i = 0; i < neuronsCount; i++)
                        {
                            weightsFile.Write("neuron" + i + ",");
                            for (int j = 0; j < variables; j++)
                                weightsFile.Write(layer.Neurons[i].Weights[j] + ",");
                            weightsFile.WriteLine(((ActivationNeuron)layer.Neurons[i]).Threshold);
                        }
                    }

                    // run epoch of learning procedure
                    double error = teacher.RunEpoch(input, output) / samples;
                    errorsList.Add(error);

                    // save current error
                    if (errorsFile != null)
                    {
                        errorsFile.WriteLine(error);
                    }

                    // show current iteration & error
                    SetText(currentIterationBox, iteration.ToString());
                    SetText(currentErrorBox, error.ToString());
                    iteration++;

                    // check if we need to stop
                    if ((useErrorLimit) && (error <= learningErrorLimit))
                        break;
                    if ((!useErrorLimit) && (iterationLimit != 0) && (iteration > iterationLimit))
                        break;
                }

                // show perceptron's weights
                ClearList(weightsList);
                for (int i = 0; i < neuronsCount; i++)
                {
                    string neuronName = string.Format("Neuron {0}", i + 1);
                    ListViewItem item = null;

                    // add all weights
                    for (int j = 0; j < variables; j++)
                    {
                        item = AddListItem(weightsList, neuronName);
                        AddListSubitem(item, string.Format("Weight {0}", j + 1));
                        AddListSubitem(item, layer.Neurons[i].Weights[0].ToString("F6"));
                    }
                    // threshold
                    item = AddListItem(weightsList, neuronName);
                    AddListSubitem(item, "Threshold");
                    AddListSubitem(item, ((ActivationNeuron)layer.Neurons[i]).Threshold.ToString("F6"));
                }

                // show error's dynamics
                double[,] errors = new double[errorsList.Count, 2];

                for (int i = 0, n = errorsList.Count; i < n; i++)
                {
                    errors[i, 0] = i;
                    errors[i, 1] = (double)errorsList[i];
                }

                errorChart.RangeX = new AForge.Range(0, errorsList.Count - 1);
                errorChart.UpdateDataSeries("error", errors);
            }
            catch (IOException)
            {
                MessageBox.Show("Failed writing file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // close files
                if (errorsFile != null)
                    errorsFile.Close();
                if (weightsFile != null)
                    weightsFile.Close();
            }

            // enable settings controls
            EnableControls(true);
        }
    }
}
