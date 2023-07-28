// Perceptron Classifier
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

namespace Classifier
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private GroupBox groupBox1;
        private ListView dataList;
        private Button loadButton;
        private OpenFileDialog openFileDialog;
        private Chart chart;
        private GroupBox groupBox2;
        private Label label1;
        private TextBox learningRateBox;
        private Button startButton;
        private Label noVisualizationLabel;
        private Label label2;
        private Label label3;
        private ListView weightsList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private Label label4;
        private TextBox iterationsBox;
        private Button stopButton;
        private Label label5;
        private Chart errorChart;
        private CheckBox saveFilesCheck;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private int samples = 0;
        private int variables = 0;
        private double[,] data = null;
        private int[] classes = null;

        private double learningRate = 0.1;
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

            // initialize charts
            chart.AddDataSeries("class1", Color.Red, Chart.SeriesType.Dots, 5);
            chart.AddDataSeries("class2", Color.Blue, Chart.SeriesType.Dots, 5);
            chart.AddDataSeries("classifier", Color.Gray, Chart.SeriesType.Line, 1, false);

            errorChart.AddDataSeries("error", Color.Red, Chart.SeriesType.ConnectedDots, 3, false);

            // update some controls
            saveFilesCheck.Checked = saveStatisticsToFiles;
            UpdateSettings();
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
            chart = new Chart();
            loadButton = new Button();
            dataList = new ListView();
            noVisualizationLabel = new Label();
            openFileDialog = new OpenFileDialog();
            groupBox2 = new GroupBox();
            saveFilesCheck = new CheckBox();
            errorChart = new Chart();
            label5 = new Label();
            stopButton = new Button();
            iterationsBox = new TextBox();
            label4 = new Label();
            weightsList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            label3 = new Label();
            label2 = new Label();
            startButton = new Button();
            learningRateBox = new TextBox();
            label1 = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(chart);
            groupBox1.Controls.Add(loadButton);
            groupBox1.Controls.Add(dataList);
            groupBox1.Controls.Add(noVisualizationLabel);
            groupBox1.Location = new System.Drawing.Point(18, 18);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(342, 776);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Data";
            // 
            // chart
            // 
            chart.Location = new System.Drawing.Point(18, 397);
            chart.Name = "chart";
            chart.RangeX = (AForge.Range)resources.GetObject("chart.RangeX");
            chart.RangeY = (AForge.Range)resources.GetObject("chart.RangeY");
            chart.Size = new Size(306, 314);
            chart.TabIndex = 2;
            chart.Text = "chart1";
            // 
            // loadButton
            // 
            loadButton.Location = new System.Drawing.Point(18, 720);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(135, 42);
            loadButton.TabIndex = 1;
            loadButton.Text = "&Load";
            loadButton.Click += loadButton_Click;
            // 
            // dataList
            // 
            dataList.FullRowSelect = true;
            dataList.GridLines = true;
            dataList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            dataList.Location = new System.Drawing.Point(18, 37);
            dataList.Name = "dataList";
            dataList.Size = new Size(306, 351);
            dataList.TabIndex = 0;
            dataList.UseCompatibleStateImageBehavior = false;
            dataList.View = View.Details;
            // 
            // noVisualizationLabel
            // 
            noVisualizationLabel.Location = new System.Drawing.Point(18, 397);
            noVisualizationLabel.Name = "noVisualizationLabel";
            noVisualizationLabel.Size = new Size(306, 314);
            noVisualizationLabel.TabIndex = 2;
            noVisualizationLabel.Text = "Visualization is not available.";
            noVisualizationLabel.TextAlign = ContentAlignment.MiddleCenter;
            noVisualizationLabel.Visible = false;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            openFileDialog.Title = "Select data file";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(saveFilesCheck);
            groupBox2.Controls.Add(errorChart);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(stopButton);
            groupBox2.Controls.Add(iterationsBox);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(weightsList);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(startButton);
            groupBox2.Controls.Add(learningRateBox);
            groupBox2.Controls.Add(label1);
            groupBox2.Location = new System.Drawing.Point(378, 18);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(432, 776);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Training";
            // 
            // saveFilesCheck
            // 
            saveFilesCheck.Location = new System.Drawing.Point(18, 148);
            saveFilesCheck.Name = "saveFilesCheck";
            saveFilesCheck.Size = new Size(328, 29);
            saveFilesCheck.TabIndex = 11;
            saveFilesCheck.Text = "Save weights and errors to files";
            // 
            // errorChart
            // 
            errorChart.Location = new System.Drawing.Point(18, 498);
            errorChart.Name = "errorChart";
            errorChart.RangeX = (AForge.Range)resources.GetObject("errorChart.RangeX");
            errorChart.RangeY = (AForge.Range)resources.GetObject("errorChart.RangeY");
            errorChart.Size = new Size(396, 259);
            errorChart.TabIndex = 10;
            // 
            // label5
            // 
            label5.Location = new System.Drawing.Point(18, 462);
            label5.Name = "label5";
            label5.Size = new Size(182, 27);
            label5.TabIndex = 9;
            label5.Text = "Error's dynamics:";
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new System.Drawing.Point(279, 90);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(135, 43);
            stopButton.TabIndex = 8;
            stopButton.Text = "S&top";
            stopButton.Click += stopButton_Click;
            // 
            // iterationsBox
            // 
            iterationsBox.Location = new System.Drawing.Point(162, 92);
            iterationsBox.Name = "iterationsBox";
            iterationsBox.ReadOnly = true;
            iterationsBox.Size = new Size(90, 31);
            iterationsBox.TabIndex = 7;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(18, 96);
            label4.Name = "label4";
            label4.Size = new Size(117, 30);
            label4.TabIndex = 6;
            label4.Text = "Iterations:";
            // 
            // weightsList
            // 
            weightsList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            weightsList.FullRowSelect = true;
            weightsList.GridLines = true;
            weightsList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            weightsList.Location = new System.Drawing.Point(18, 240);
            weightsList.Name = "weightsList";
            weightsList.Size = new Size(396, 203);
            weightsList.TabIndex = 5;
            weightsList.UseCompatibleStateImageBehavior = false;
            weightsList.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Weight";
            columnHeader1.Width = 70;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Value";
            columnHeader2.Width = 100;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(18, 203);
            label3.Name = "label3";
            label3.Size = new Size(202, 30);
            label3.TabIndex = 4;
            label3.Text = "Perceptron weights:";
            // 
            // label2
            // 
            label2.BorderStyle = BorderStyle.FixedSingle;
            label2.Location = new System.Drawing.Point(18, 185);
            label2.Name = "label2";
            label2.Size = new Size(396, 3);
            label2.TabIndex = 3;
            // 
            // startButton
            // 
            startButton.Enabled = false;
            startButton.Location = new System.Drawing.Point(279, 35);
            startButton.Name = "startButton";
            startButton.Size = new Size(135, 43);
            startButton.TabIndex = 2;
            startButton.Text = "&Start";
            startButton.Click += startButton_Click;
            // 
            // learningRateBox
            // 
            learningRateBox.Location = new System.Drawing.Point(162, 37);
            learningRateBox.Name = "learningRateBox";
            learningRateBox.Size = new Size(90, 31);
            learningRateBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(18, 41);
            label1.Name = "label1";
            label1.Size = new Size(135, 29);
            label1.TabIndex = 0;
            label1.Text = "Learning rate:";
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(9, 24);
            ClientSize = new Size(834, 821);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Perceptron Classifier";
            Closing += MainForm_Closing;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
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

        // On "Load" button click - load data
        private void loadButton_Click(object sender, EventArgs e)
        {
            // data file format:
            // X1, X2, ... Xn, class (0|1)

            // show file selection dialog
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = null;

                // temp buffers (for 50 samples only)
                float[,] tempData = null;
                int[] tempClasses = new int[50];

                // min and max X values
                float minX = float.MaxValue;
                float maxX = float.MinValue;

                // samples count
                samples = 0;

                try
                {
                    string str = null;

                    // open selected file
                    reader = File.OpenText(openFileDialog.FileName);

                    // read the data
                    while ((samples < 50) && ((str = reader.ReadLine()) != null))
                    {
                        // split the string
                        string[] strs = str.Split(';');
                        if (strs.Length == 1)
                            strs = str.Split(',');

                        // allocate data array
                        if (samples == 0)
                        {
                            variables = strs.Length - 1;
                            tempData = new float[50, variables];
                        }

                        // parse data
                        for (int j = 0; j < variables; j++)
                        {
                            tempData[samples, j] = float.Parse(strs[j]);
                        }
                        tempClasses[samples] = int.Parse(strs[variables]);

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

                    // clear current result
                    ClearCurrentSolution();
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

                // show chart or not
                bool showChart = (variables == 2);

                if (showChart)
                {
                    chart.RangeX = new AForge.Range(minX, maxX);
                    ShowTrainingData();
                }

                chart.Visible = showChart;
                noVisualizationLabel.Visible = !showChart;

                // enable start button
                startButton.Enabled = true;
            }
        }

        // Update settings controls
        private void UpdateSettings()
        {
            learningRateBox.Text = learningRate.ToString();
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

        // Show training data on chart
        private void ShowTrainingData()
        {
            int class1Size = 0;
            int class2Size = 0;

            // calculate number of samples in each class
            for (int i = 0, n = samples; i < n; i++)
            {
                if (classes[i] == 0)
                    class1Size++;
                else
                    class2Size++;
            }

            // allocate classes arrays
            double[,] class1 = new double[class1Size, 2];
            double[,] class2 = new double[class2Size, 2];

            // fill classes arrays
            for (int i = 0, c1 = 0, c2 = 0; i < samples; i++)
            {
                if (classes[i] == 0)
                {
                    // class 1
                    class1[c1, 0] = data[i, 0];
                    class1[c1, 1] = data[i, 1];
                    c1++;
                }
                else
                {
                    // class 2
                    class2[c2, 0] = data[i, 0];
                    class2[c2, 1] = data[i, 1];
                    c2++;
                }
            }

            // updata chart control
            chart.UpdateDataSeries("class1", class1);
            chart.UpdateDataSeries("class2", class2);
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
                loadButton.Enabled = enable;
                startButton.Enabled = enable;
                saveFilesCheck.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // Clear current solution
        private void ClearCurrentSolution()
        {
            chart.UpdateDataSeries("classifier", null);
            errorChart.UpdateDataSeries("error", null);
            weightsList.Items.Clear();
        }

        // On button "Start" - start learning procedure
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

        // On button "Stop" - stop learning procedure
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
            // prepare learning data
            double[][] input = new double[samples][];
            double[][] output = new double[samples][];

            for (int i = 0; i < samples; i++)
            {
                input[i] = new double[variables];
                output[i] = new double[1];

                // copy input
                for (int j = 0; j < variables; j++)
                    input[i][j] = data[i, j];
                // copy output
                output[i][0] = classes[i];
            }

            // create perceptron
            ActivationNetwork network = new ActivationNetwork(new ThresholdFunction(), variables, 1);
            ActivationNeuron neuron = network.Layers[0].Neurons[0] as ActivationNeuron;
            // create teacher
            PerceptronLearning teacher = new PerceptronLearning(network);
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
                        for (int i = 0; i < variables; i++)
                        {
                            weightsFile.Write(neuron.Weights[i] + ",");
                        }
                        weightsFile.WriteLine(neuron.Threshold);
                    }

                    // run epoch of learning procedure
                    double error = teacher.RunEpoch(input, output);
                    errorsList.Add(error);

                    // show current iteration
                    SetText(iterationsBox, iteration.ToString());

                    // save current error
                    if (errorsFile != null)
                    {
                        errorsFile.WriteLine(error);
                    }

                    // show classifier in the case of 2 dimensional data
                    if ((neuron.InputsCount == 2) && (neuron.Weights[1] != 0))
                    {
                        double k = -neuron.Weights[0] / neuron.Weights[1];
                        double b = -neuron.Threshold / neuron.Weights[1];

                        double[,] classifier = new double[2, 2] {
                            { chart.RangeX.Min, chart.RangeX.Min * k + b },
                            { chart.RangeX.Max, chart.RangeX.Max * k + b }
                                                                };
                        // update chart
                        chart.UpdateDataSeries("classifier", classifier);
                    }

                    // stop if no error
                    if (error == 0)
                        break;

                    iteration++;
                }

                // show perceptron's weights
                ListViewItem item = null;

                ClearList(weightsList);
                for (int i = 0; i < variables; i++)
                {
                    item = AddListItem(weightsList, string.Format("Weight {0}", i + 1));
                    AddListSubitem(item, neuron.Weights[i].ToString("F6"));
                }
                item = AddListItem(weightsList, "Threshold");
                AddListSubitem(item, neuron.Threshold.ToString("F6"));

                // show error's dynamics
                double[,] errors = new double[errorsList.Count, 2];

                for (int i = 0, n = errorsList.Count; i < n; i++)
                {
                    errors[i, 0] = i;
                    errors[i, 1] = (double)errorsList[i];
                }

                errorChart.RangeX = new AForge.Range(0, errorsList.Count - 1);
                errorChart.RangeY = new AForge.Range(0, samples);
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
