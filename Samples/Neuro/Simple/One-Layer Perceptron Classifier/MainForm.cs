// One-Layer Perceptron Classifier
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2006-2011
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
        private Chart chart;
        private Button loadButton;
        private OpenFileDialog openFileDialog;
        private GroupBox groupBox2;
        private Label label1;
        private TextBox learningRateBox;
        private Label label2;
        private TextBox iterationsBox;
        private Button stopButton;
        private Button startButton;
        private CheckBox saveFilesCheck;
        private Label label3;
        private Label label4;
        private ListView weightsList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private GroupBox groupBox3;
        private Chart errorChart;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private int samples = 0;
        private double[,] data = null;
        private int[] classes = null;
        private int classesCount = 0;
        private int[] samplesPerClass = null;

        private double learningRate = 0.1;
        private bool saveStatisticsToFiles = false;

        private Thread workerThread = null;
        private volatile bool needToStop = false;

        // color for data series
        private static Color[] dataSereisColors = new Color[10] {
                                                                     Color.Red,     Color.Blue,
                                                                     Color.Green,   Color.DarkOrange,
                                                                     Color.Violet,  Color.Brown,
                                                                     Color.Black,   Color.Pink,
                                                                     Color.Olive,   Color.Navy };

        // Constructor
        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // update some controls
            saveFilesCheck.Checked = saveStatisticsToFiles;
            UpdateSettings();

            // initialize charts
            errorChart.AddDataSeries("error", Color.Red, Chart.SeriesType.ConnectedDots, 3);
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
            loadButton = new Button();
            chart = new Chart();
            openFileDialog = new OpenFileDialog();
            groupBox2 = new GroupBox();
            weightsList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            label4 = new Label();
            label3 = new Label();
            saveFilesCheck = new CheckBox();
            stopButton = new Button();
            startButton = new Button();
            iterationsBox = new TextBox();
            label2 = new Label();
            learningRateBox = new TextBox();
            label1 = new Label();
            groupBox3 = new GroupBox();
            errorChart = new Chart();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(loadButton);
            groupBox1.Controls.Add(chart);
            groupBox1.Location = new System.Drawing.Point(18, 18);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(396, 471);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Data";
            // 
            // loadButton
            // 
            loadButton.Location = new System.Drawing.Point(18, 415);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(135, 43);
            loadButton.TabIndex = 1;
            loadButton.Text = "&Load";
            loadButton.Click += loadButton_Click;
            // 
            // chart
            // 
            chart.Location = new System.Drawing.Point(18, 37);
            chart.Name = "chart";
            chart.RangeX = (AForge.Range)resources.GetObject("chart.RangeX");
            chart.RangeY = (AForge.Range)resources.GetObject("chart.RangeY");
            chart.Size = new Size(360, 369);
            chart.TabIndex = 0;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            openFileDialog.Title = "Select data file";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(weightsList);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(saveFilesCheck);
            groupBox2.Controls.Add(stopButton);
            groupBox2.Controls.Add(startButton);
            groupBox2.Controls.Add(iterationsBox);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(learningRateBox);
            groupBox2.Controls.Add(label1);
            groupBox2.Location = new System.Drawing.Point(432, 18);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(432, 757);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Training";
            // 
            // weightsList
            // 
            weightsList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            weightsList.FullRowSelect = true;
            weightsList.GridLines = true;
            weightsList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            weightsList.Location = new System.Drawing.Point(18, 240);
            weightsList.Name = "weightsList";
            weightsList.Size = new Size(396, 498);
            weightsList.TabIndex = 14;
            weightsList.UseCompatibleStateImageBehavior = false;
            weightsList.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Neuron";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Weigh";
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Value";
            columnHeader3.Width = 65;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(18, 203);
            label4.Name = "label4";
            label4.Size = new Size(99, 30);
            label4.TabIndex = 13;
            label4.Text = "Weights:";
            // 
            // label3
            // 
            label3.BorderStyle = BorderStyle.FixedSingle;
            label3.Location = new System.Drawing.Point(18, 185);
            label3.Name = "label3";
            label3.Size = new Size(396, 3);
            label3.TabIndex = 12;
            // 
            // saveFilesCheck
            // 
            saveFilesCheck.Location = new System.Drawing.Point(18, 148);
            saveFilesCheck.Name = "saveFilesCheck";
            saveFilesCheck.Size = new Size(270, 29);
            saveFilesCheck.TabIndex = 11;
            saveFilesCheck.Text = "Save weights and errors to files";
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new System.Drawing.Point(279, 90);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(135, 43);
            stopButton.TabIndex = 10;
            stopButton.Text = "S&top";
            stopButton.Click += stopButton_Click;
            // 
            // startButton
            // 
            startButton.Enabled = false;
            startButton.Location = new System.Drawing.Point(279, 35);
            startButton.Name = "startButton";
            startButton.Size = new Size(135, 43);
            startButton.TabIndex = 9;
            startButton.Text = "&Start";
            startButton.Click += startButton_Click;
            // 
            // iterationsBox
            // 
            iterationsBox.Location = new System.Drawing.Point(162, 92);
            iterationsBox.Name = "iterationsBox";
            iterationsBox.ReadOnly = true;
            iterationsBox.Size = new Size(90, 31);
            iterationsBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(18, 96);
            label2.Name = "label2";
            label2.Size = new Size(99, 24);
            label2.TabIndex = 2;
            label2.Text = "Iterations:";
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
            label1.Size = new Size(144, 31);
            label1.TabIndex = 0;
            label1.Text = "Learning rate:";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(errorChart);
            groupBox3.Location = new System.Drawing.Point(18, 498);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(396, 277);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Error's dynamics";
            // 
            // errorChart
            // 
            errorChart.Location = new System.Drawing.Point(18, 37);
            errorChart.Name = "errorChart";
            errorChart.RangeX = (AForge.Range)resources.GetObject("errorChart.RangeX");
            errorChart.RangeY = (AForge.Range)resources.GetObject("errorChart.RangeY");
            errorChart.Size = new Size(360, 221);
            errorChart.TabIndex = 0;
            errorChart.Text = "chart1";
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(9, 24);
            ClientSize = new Size(906, 809);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "One-Layer Perceptron Classifier";
            Closing += MainForm_Closing;
            groupBox1.ResumeLayout(false);
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
            // X1, X2, class

            // load maximum 10 classes !

            // show file selection dialog
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = null;

                // temp buffers (for 200 samples only)
                float[,] tempData = new float[200, 2];
                int[] tempClasses = new int[200];

                // min and max X values
                float minX = float.MaxValue;
                float maxX = float.MinValue;

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

                        // check tokens count
                        if (strs.Length != 3)
                            throw new ApplicationException("Invalid file format");

                        // parse tokens
                        tempData[samples, 0] = float.Parse(strs[0], System.Globalization.CultureInfo.InvariantCulture);
                        tempData[samples, 1] = float.Parse(strs[1], System.Globalization.CultureInfo.InvariantCulture);
                        tempClasses[samples] = int.Parse(strs[2]);

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
                    data = new double[samples, 2];
                    Array.Copy(tempData, 0, data, 0, samples * 2);
                    classes = new int[samples];
                    Array.Copy(tempClasses, 0, classes, 0, samples);

                    // clear current result
                    weightsList.Items.Clear();
                    errorChart.UpdateDataSeries("error", null);
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

                // update chart
                chart.RangeX = new AForge.Range(minX, maxX);
                ShowTrainingData();

                // enable start button
                startButton.Enabled = true;
            }
        }

        // Update settings controls
        private void UpdateSettings()
        {
            learningRateBox.Text = learningRate.ToString();
        }

        // Show training data on chart
        private void ShowTrainingData()
        {
            double[][,] dataSeries = new double[classesCount][,];
            int[] indexes = new int[classesCount];

            // allocate data arrays
            for (int i = 0; i < classesCount; i++)
            {
                dataSeries[i] = new double[samplesPerClass[i], 2];
            }

            // fill data arrays
            for (int i = 0; i < samples; i++)
            {
                // get sample's class
                int dataClass = classes[i];
                // copy data into appropriate array
                dataSeries[dataClass][indexes[dataClass], 0] = data[i, 0];
                dataSeries[dataClass][indexes[dataClass], 1] = data[i, 1];
                indexes[dataClass]++;
            }

            // remove all previous data series from chart control
            chart.RemoveAllDataSeries();

            // add new data series
            for (int i = 0; i < classesCount; i++)
            {
                string className = string.Format("class" + i);

                // add data series
                chart.AddDataSeries(className, dataSereisColors[i], Chart.SeriesType.Dots, 5);
                chart.UpdateDataSeries(className, dataSeries[i]);
                // add classifier
                chart.AddDataSeries(string.Format("classifier" + i), Color.Gray, Chart.SeriesType.Line, 1, false);
            }
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
            // prepare learning data
            double[][] input = new double[samples][];
            double[][] output = new double[samples][];

            for (int i = 0; i < samples; i++)
            {
                input[i] = new double[2];
                output[i] = new double[classesCount];

                // set input
                input[i][0] = data[i, 0];
                input[i][1] = data[i, 1];
                // set output
                output[i][classes[i]] = 1;
            }

            // create perceptron
            ActivationNetwork network = new ActivationNetwork(new ThresholdFunction(), 2, classesCount);
            ActivationLayer layer = network.Layers[0] as ActivationLayer;
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
                        for (int i = 0; i < classesCount; i++)
                        {
                            weightsFile.Write("neuron" + i + ",");
                            weightsFile.Write(layer.Neurons[i].Weights[0] + ",");
                            weightsFile.Write(layer.Neurons[i].Weights[1] + ",");
                            weightsFile.WriteLine(((ActivationNeuron)layer.Neurons[i]).Threshold);
                        }
                    }

                    // run epoch of learning procedure
                    double error = teacher.RunEpoch(input, output);
                    errorsList.Add(error);

                    // save current error
                    if (errorsFile != null)
                    {
                        errorsFile.WriteLine(error);
                    }

                    // show current iteration
                    SetText(iterationsBox, iteration.ToString());

                    // stop if no error
                    if (error == 0)
                        break;

                    // show classifiers
                    for (int j = 0; j < classesCount; j++)
                    {
                        double k = (layer.Neurons[j].Weights[1] != 0) ? (-layer.Neurons[j].Weights[0] / layer.Neurons[j].Weights[1]) : 0;
                        double b = (layer.Neurons[j].Weights[1] != 0) ? (-((ActivationNeuron)layer.Neurons[j]).Threshold / layer.Neurons[j].Weights[1]) : 0;

                        double[,] classifier = new double[2, 2] {
                            { chart.RangeX.Min, chart.RangeX.Min * k + b },
                            { chart.RangeX.Max, chart.RangeX.Max * k + b }
                                                                };

                        // update chart
                        chart.UpdateDataSeries(string.Format("classifier" + j), classifier);
                    }

                    iteration++;
                }

                // show perceptron's weights
                ClearList(weightsList);
                for (int i = 0; i < classesCount; i++)
                {
                    string neuronName = string.Format("Neuron {0}", i + 1);

                    // weight 0
                    ListViewItem item = AddListItem(weightsList, neuronName);
                    AddListSubitem(item, "Weight 1");
                    AddListSubitem(item, layer.Neurons[i].Weights[0].ToString("F6"));
                    // weight 1
                    item = AddListItem(weightsList, neuronName);
                    AddListSubitem(item, "Weight 2");
                    AddListSubitem(item, layer.Neurons[i].Weights[1].ToString("F6"));
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
