// Time Series Prediction using Multi-Layer Neural Network
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
using System.Threading;
using System.IO;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;
using System.Globalization;

namespace TimeSeries
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private GroupBox groupBox1;
        private ListView dataList;
        private ColumnHeader yColumnHeader;
        private ColumnHeader estimatedYColumnHeader;
        private Button loadDataButton;
        private GroupBox groupBox2;
        private Chart chart;
        private OpenFileDialog openFileDialog;
        private GroupBox groupBox3;
        private TextBox momentumBox;
        private Label label6;
        private TextBox alphaBox;
        private Label label2;
        private TextBox learningRateBox;
        private Label label1;
        private Label label10;
        private TextBox iterationsBox;
        private Label label9;
        private Label label8;
        private TextBox predictionSizeBox;
        private Label label7;
        private TextBox windowSizeBox;
        private Label label3;
        private Label label5;
        private Button stopButton;
        private Button startButton;
        private GroupBox groupBox4;
        private TextBox currentPredictionErrorBox;
        private Label label13;
        private TextBox currentLearningErrorBox;
        private Label label12;
        private TextBox currentIterationBox;
        private Label label11;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private double[] data = null;
        private double[,] dataToShow = null;

        private double learningRate = 0.1;
        private double momentum = 0.0;
        private double sigmoidAlphaValue = 2.0;
        private int windowSize = 5;
        private int predictionSize = 1;
        private int iterations = 1000;

        private Thread workerThread = null;
        private volatile bool needToStop = false;

        private double[,] windowDelimiter = new double[2, 2] { { 0, 0 }, { 0, 0 } };
        private double[,] predictionDelimiter = new double[2, 2] { { 0, 0 }, { 0, 0 } };

        // Constructor
        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // initializa chart control
            chart.AddDataSeries("data", Color.Red, Chart.SeriesType.Dots, 5);
            chart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.Line, 1);
            chart.AddDataSeries("window", Color.LightGray, Chart.SeriesType.Line, 1, false);
            chart.AddDataSeries("prediction", Color.Gray, Chart.SeriesType.Line, 1, false);

            // update controls
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
            dataList = new ListView();
            yColumnHeader = new ColumnHeader();
            estimatedYColumnHeader = new ColumnHeader();
            loadDataButton = new Button();
            groupBox2 = new GroupBox();
            chart = new Chart();
            openFileDialog = new OpenFileDialog();
            groupBox3 = new GroupBox();
            momentumBox = new TextBox();
            label6 = new Label();
            alphaBox = new TextBox();
            label2 = new Label();
            learningRateBox = new TextBox();
            label1 = new Label();
            label8 = new Label();
            iterationsBox = new TextBox();
            predictionSizeBox = new TextBox();
            label7 = new Label();
            windowSizeBox = new TextBox();
            label3 = new Label();
            label10 = new Label();
            label9 = new Label();
            label5 = new Label();
            stopButton = new Button();
            startButton = new Button();
            groupBox4 = new GroupBox();
            currentPredictionErrorBox = new TextBox();
            label13 = new Label();
            currentLearningErrorBox = new TextBox();
            label12 = new Label();
            currentIterationBox = new TextBox();
            label11 = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dataList);
            groupBox1.Controls.Add(loadDataButton);
            groupBox1.Location = new System.Drawing.Point(18, 18);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(324, 702);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Data";
            // 
            // dataList
            // 
            dataList.Columns.AddRange(new ColumnHeader[] { yColumnHeader, estimatedYColumnHeader });
            dataList.FullRowSelect = true;
            dataList.GridLines = true;
            dataList.Location = new System.Drawing.Point(18, 37);
            dataList.Name = "dataList";
            dataList.Size = new Size(288, 581);
            dataList.TabIndex = 3;
            dataList.UseCompatibleStateImageBehavior = false;
            dataList.View = View.Details;
            // 
            // yColumnHeader
            // 
            yColumnHeader.Text = "Y:Real";
            yColumnHeader.Width = 70;
            // 
            // estimatedYColumnHeader
            // 
            estimatedYColumnHeader.Text = "Y:Estimated";
            estimatedYColumnHeader.Width = 70;
            // 
            // loadDataButton
            // 
            loadDataButton.Location = new System.Drawing.Point(18, 637);
            loadDataButton.Name = "loadDataButton";
            loadDataButton.Size = new Size(135, 42);
            loadDataButton.TabIndex = 2;
            loadDataButton.Text = "&Load";
            loadDataButton.Click += loadDataButton_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(chart);
            groupBox2.Location = new System.Drawing.Point(360, 18);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(540, 702);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Function";
            // 
            // chart
            // 
            chart.Location = new System.Drawing.Point(18, 37);
            chart.Name = "chart";
            chart.RangeX = (AForge.Range)resources.GetObject("chart.RangeX");
            chart.RangeY = (AForge.Range)resources.GetObject("chart.RangeY");
            chart.Size = new Size(504, 646);
            chart.TabIndex = 0;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            openFileDialog.Title = "Select data file";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(momentumBox);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(alphaBox);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(learningRateBox);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(iterationsBox);
            groupBox3.Controls.Add(predictionSizeBox);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(windowSizeBox);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(label9);
            groupBox3.Controls.Add(label5);
            groupBox3.Location = new System.Drawing.Point(918, 18);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(351, 379);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Settings";
            // 
            // momentumBox
            // 
            momentumBox.Location = new System.Drawing.Point(225, 83);
            momentumBox.Name = "momentumBox";
            momentumBox.Size = new Size(108, 31);
            momentumBox.TabIndex = 9;
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(18, 87);
            label6.Name = "label6";
            label6.Size = new Size(148, 31);
            label6.TabIndex = 8;
            label6.Text = "Momentum:";
            // 
            // alphaBox
            // 
            alphaBox.Location = new System.Drawing.Point(225, 129);
            alphaBox.Name = "alphaBox";
            alphaBox.Size = new Size(108, 31);
            alphaBox.TabIndex = 11;
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(18, 133);
            label2.Name = "label2";
            label2.Size = new Size(216, 28);
            label2.TabIndex = 10;
            label2.Text = "Sigmoid's alpha value:";
            // 
            // learningRateBox
            // 
            learningRateBox.Location = new System.Drawing.Point(225, 37);
            learningRateBox.Name = "learningRateBox";
            learningRateBox.Size = new Size(108, 31);
            learningRateBox.TabIndex = 7;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(18, 41);
            label1.Name = "label1";
            label1.Size = new Size(140, 25);
            label1.TabIndex = 6;
            label1.Text = "Learning rate:";
            // 
            // label8
            // 
            label8.BorderStyle = BorderStyle.Fixed3D;
            label8.Location = new System.Drawing.Point(18, 290);
            label8.Name = "label8";
            label8.Size = new Size(315, 4);
            label8.TabIndex = 22;
            // 
            // iterationsBox
            // 
            iterationsBox.Location = new System.Drawing.Point(225, 305);
            iterationsBox.Name = "iterationsBox";
            iterationsBox.Size = new Size(108, 31);
            iterationsBox.TabIndex = 24;
            // 
            // predictionSizeBox
            // 
            predictionSizeBox.Location = new System.Drawing.Point(225, 240);
            predictionSizeBox.Name = "predictionSizeBox";
            predictionSizeBox.Size = new Size(108, 31);
            predictionSizeBox.TabIndex = 21;
            predictionSizeBox.TextChanged += predictionSizeBox_TextChanged;
            // 
            // label7
            // 
            label7.Location = new System.Drawing.Point(18, 244);
            label7.Name = "label7";
            label7.Size = new Size(162, 29);
            label7.TabIndex = 20;
            label7.Text = "Prediction size:";
            // 
            // windowSizeBox
            // 
            windowSizeBox.Location = new System.Drawing.Point(225, 194);
            windowSizeBox.Name = "windowSizeBox";
            windowSizeBox.Size = new Size(108, 31);
            windowSizeBox.TabIndex = 19;
            windowSizeBox.TextChanged += windowSizeBox_TextChanged;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(18, 198);
            label3.Name = "label3";
            label3.Size = new Size(144, 29);
            label3.TabIndex = 18;
            label3.Text = "Window size:";
            // 
            // label10
            // 
            label10.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(227, 342);
            label10.Name = "label10";
            label10.Size = new Size(104, 25);
            label10.TabIndex = 25;
            label10.Text = "( 0 - inifinity )";
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(18, 308);
            label9.Name = "label9";
            label9.Size = new Size(126, 30);
            label9.TabIndex = 23;
            label9.Text = "Iterations:";
            // 
            // label5
            // 
            label5.BorderStyle = BorderStyle.Fixed3D;
            label5.Location = new System.Drawing.Point(18, 179);
            label5.Name = "label5";
            label5.Size = new Size(315, 4);
            label5.TabIndex = 17;
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new System.Drawing.Point(1134, 665);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(135, 42);
            stopButton.TabIndex = 6;
            stopButton.Text = "S&top";
            stopButton.Click += stopButton_Click;
            // 
            // startButton
            // 
            startButton.Enabled = false;
            startButton.Location = new System.Drawing.Point(972, 665);
            startButton.Name = "startButton";
            startButton.Size = new Size(135, 42);
            startButton.TabIndex = 5;
            startButton.Text = "&Start";
            startButton.Click += startButton_Click;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(currentPredictionErrorBox);
            groupBox4.Controls.Add(label13);
            groupBox4.Controls.Add(currentLearningErrorBox);
            groupBox4.Controls.Add(label12);
            groupBox4.Controls.Add(currentIterationBox);
            groupBox4.Controls.Add(label11);
            groupBox4.Location = new System.Drawing.Point(918, 415);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(351, 185);
            groupBox4.TabIndex = 7;
            groupBox4.TabStop = false;
            groupBox4.Text = "Current iteration:";
            // 
            // currentPredictionErrorBox
            // 
            currentPredictionErrorBox.Location = new System.Drawing.Point(225, 129);
            currentPredictionErrorBox.Name = "currentPredictionErrorBox";
            currentPredictionErrorBox.ReadOnly = true;
            currentPredictionErrorBox.Size = new Size(108, 31);
            currentPredictionErrorBox.TabIndex = 5;
            // 
            // label13
            // 
            label13.Location = new System.Drawing.Point(18, 133);
            label13.Name = "label13";
            label13.Size = new Size(180, 29);
            label13.TabIndex = 4;
            label13.Text = "Prediction error:";
            // 
            // currentLearningErrorBox
            // 
            currentLearningErrorBox.Location = new System.Drawing.Point(225, 83);
            currentLearningErrorBox.Name = "currentLearningErrorBox";
            currentLearningErrorBox.ReadOnly = true;
            currentLearningErrorBox.Size = new Size(108, 31);
            currentLearningErrorBox.TabIndex = 3;
            // 
            // label12
            // 
            label12.Location = new System.Drawing.Point(18, 87);
            label12.Name = "label12";
            label12.Size = new Size(144, 29);
            label12.TabIndex = 2;
            label12.Text = "Learning error:";
            // 
            // currentIterationBox
            // 
            currentIterationBox.Location = new System.Drawing.Point(225, 37);
            currentIterationBox.Name = "currentIterationBox";
            currentIterationBox.ReadOnly = true;
            currentIterationBox.Size = new Size(108, 31);
            currentIterationBox.TabIndex = 1;
            // 
            // label11
            // 
            label11.Location = new System.Drawing.Point(18, 41);
            label11.Name = "label11";
            label11.Size = new Size(126, 29);
            label11.TabIndex = 0;
            label11.Text = "Iteration:";
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(9, 24);
            ClientSize = new Size(1302, 734);
            Controls.Add(groupBox4);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Time Series Prediction using Multi-Layer Neural Network";
            Closing += MainForm_Closing;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ResumeLayout(false);
        }
        #endregion

        // Delegates to enable async calls for setting controls properties
        private delegate void SetTextCallback(Control control, string text);
        private delegate void AddSubItemCallback(ListView control, int item, string subitemText);

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

        // Thread safe adding of subitem to list control
        private void AddSubItem(ListView control, int item, string subitemText)
        {
            if (control.InvokeRequired)
            {
                AddSubItemCallback d = new AddSubItemCallback(AddSubItem);
                Invoke(d, new object[] { control, item, subitemText });
            }
            else
            {
                control.Items[item].SubItems.Add(subitemText);
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

        // Update settings controls
        private void UpdateSettings()
        {
            learningRateBox.Text = learningRate.ToString();
            momentumBox.Text = momentum.ToString();
            alphaBox.Text = sigmoidAlphaValue.ToString();
            windowSizeBox.Text = windowSize.ToString();
            predictionSizeBox.Text = predictionSize.ToString();
            iterationsBox.Text = iterations.ToString();
        }

        // Load data
        private void loadDataButton_Click(object sender, EventArgs e)
        {
            // show file selection dialog
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = null;
                // read maximum 50 points
                double[] tempData = new double[50];

                try
                {
                    // open selected file
                    reader = File.OpenText(openFileDialog.FileName);
                    string str = null;
                    int i = 0;

                    // read the data
                    while ((i < 50) && ((str = reader.ReadLine()) != null))
                    {
                        // parse the value
                        tempData[i] = double.Parse(str, CultureInfo.InvariantCulture);

                        i++;
                    }

                    // allocate and set data
                    data = new double[i];
                    dataToShow = new double[i, 2];
                    Array.Copy(tempData, 0, data, 0, i);
                    for (int j = 0; j < i; j++)
                    {
                        dataToShow[j, 0] = j;
                        dataToShow[j, 1] = data[j];
                    }
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
                chart.RangeX = new AForge.Range(0, data.Length - 1);
                chart.UpdateDataSeries("data", dataToShow);
                chart.UpdateDataSeries("solution", null);
                // set delimiters
                UpdateDelimiters();
                // enable "Start" button
                startButton.Enabled = true;
            }
        }

        // Update delimiters on the chart
        private void UpdateDelimiters()
        {
            // window delimiter
            windowDelimiter[0, 0] = windowDelimiter[1, 0] = windowSize;
            windowDelimiter[0, 1] = chart.RangeY.Min;
            windowDelimiter[1, 1] = chart.RangeY.Max;
            chart.UpdateDataSeries("window", windowDelimiter);
            // prediction delimiter
            predictionDelimiter[0, 0] = predictionDelimiter[1, 0] = data.Length - 1 - predictionSize;
            predictionDelimiter[0, 1] = chart.RangeY.Min;
            predictionDelimiter[1, 1] = chart.RangeY.Max;
            chart.UpdateDataSeries("prediction", predictionDelimiter);
        }

        // Update data in list view
        private void UpdateDataListView()
        {
            // remove all current records
            dataList.Items.Clear();
            // add new records
            for (int i = 0, n = data.GetLength(0); i < n; i++)
            {
                dataList.Items.Add(data[i].ToString());
            }
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback(bool enable);

        // Enable/disable controls
        private void EnableControls(bool enable)
        {
            if (InvokeRequired)
            {
                EnableCallback d = new EnableCallback(EnableControls);
                Invoke(d, new object[] { enable });
            }
            else
            {
                loadDataButton.Enabled = enable;
                learningRateBox.Enabled = enable;
                momentumBox.Enabled = enable;
                alphaBox.Enabled = enable;
                windowSizeBox.Enabled = enable;
                predictionSizeBox.Enabled = enable;
                iterationsBox.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // On window size changed
        private void windowSizeBox_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowSize();
        }

        // On prediction changed
        private void predictionSizeBox_TextChanged(object sender, EventArgs e)
        {
            UpdatePredictionSize();
        }

        // Update window size
        private void UpdateWindowSize()
        {
            if (data != null)
            {
                // get new window size value
                try
                {
                    windowSize = Math.Max(1, Math.Min(15, int.Parse(windowSizeBox.Text)));
                }
                catch
                {
                    windowSize = 5;
                }
                // check if we have too few data
                if (windowSize >= data.Length)
                    windowSize = 1;
                // update delimiters
                UpdateDelimiters();
            }
        }

        // Update prediction size
        private void UpdatePredictionSize()
        {
            if (data != null)
            {
                // get new prediction size value
                try
                {
                    predictionSize = Math.Max(1, Math.Min(10, int.Parse(predictionSizeBox.Text)));
                }
                catch
                {
                    predictionSize = 1;
                }
                // check if we have too few data
                if (data.Length - predictionSize - 1 < windowSize)
                    predictionSize = 1;
                // update delimiters
                UpdateDelimiters();
            }
        }

        // On button "Start"
        private void startButton_Click(object sender, EventArgs e)
        {
            // clear previous solution
            for (int j = 0, n = data.Length; j < n; j++)
            {
                if (dataList.Items[j].SubItems.Count > 1)
                    dataList.Items[j].SubItems.RemoveAt(1);
            }

            // get learning rate
            try
            {
                learningRate = Math.Max(0.00001, Math.Min(1, double.Parse(learningRateBox.Text)));
            }
            catch
            {
                learningRate = 0.1;
            }
            // get momentum
            try
            {
                momentum = Math.Max(0, Math.Min(0.5, double.Parse(momentumBox.Text)));
            }
            catch
            {
                momentum = 0;
            }
            // get sigmoid's alpha value
            try
            {
                sigmoidAlphaValue = Math.Max(0.001, Math.Min(50, double.Parse(alphaBox.Text)));
            }
            catch
            {
                sigmoidAlphaValue = 2;
            }
            // iterations
            try
            {
                iterations = Math.Max(0, int.Parse(iterationsBox.Text));
            }
            catch
            {
                iterations = 1000;
            }
            // update settings controls
            UpdateSettings();

            // disable all settings controls except "Stop" button
            EnableControls(false);

            // run worker thread
            needToStop = false;
            workerThread = new Thread(new ThreadStart(SearchSolution));
            workerThread.Start();
        }

        // On button "Stop"
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
            // number of learning samples
            int samples = data.Length - predictionSize - windowSize;
            // data transformation factor
            double factor = 1.7 / chart.RangeY.Length;
            double yMin = chart.RangeY.Min;
            // prepare learning data
            double[][] input = new double[samples][];
            double[][] output = new double[samples][];

            for (int i = 0; i < samples; i++)
            {
                input[i] = new double[windowSize];
                output[i] = new double[1];

                // set input
                for (int j = 0; j < windowSize; j++)
                {
                    input[i][j] = (data[i + j] - yMin) * factor - 0.85;
                }
                // set output
                output[i][0] = (data[i + windowSize] - yMin) * factor - 0.85;
            }

            // create multi-layer neural network
            ActivationNetwork network = new ActivationNetwork(
                new BipolarSigmoidFunction(sigmoidAlphaValue),
                windowSize, windowSize * 2, 1);
            // create teacher
            BackPropagationLearning teacher = new BackPropagationLearning(network);
            // set learning rate and momentum
            teacher.LearningRate = learningRate;
            teacher.Momentum = momentum;

            // iterations
            int iteration = 1;

            // solution array
            int solutionSize = data.Length - windowSize;
            double[,] solution = new double[solutionSize, 2];
            double[] networkInput = new double[windowSize];

            // calculate X values to be used with solution function
            for (int j = 0; j < solutionSize; j++)
            {
                solution[j, 0] = j + windowSize;
            }

            // loop
            while (!needToStop)
            {
                // run epoch of learning procedure
                double error = teacher.RunEpoch(input, output) / samples;

                // calculate solution and learning and prediction errors
                double learningError = 0.0;
                double predictionError = 0.0;
                // go through all the data
                for (int i = 0, n = data.Length - windowSize; i < n; i++)
                {
                    // put values from current window as network's input
                    for (int j = 0; j < windowSize; j++)
                    {
                        networkInput[j] = (data[i + j] - yMin) * factor - 0.85;
                    }

                    // evalue the function
                    solution[i, 1] = (network.Compute(networkInput)[0] + 0.85) / factor + yMin;

                    // calculate prediction error
                    if (i >= n - predictionSize)
                    {
                        predictionError += Math.Abs(solution[i, 1] - data[windowSize + i]);
                    }
                    else
                    {
                        learningError += Math.Abs(solution[i, 1] - data[windowSize + i]);
                    }
                }
                // update solution on the chart
                chart.UpdateDataSeries("solution", solution);

                // set current iteration's info
                SetText(currentIterationBox, iteration.ToString());
                SetText(currentLearningErrorBox, learningError.ToString("F3"));
                SetText(currentPredictionErrorBox, predictionError.ToString("F3"));

                // increase current iteration
                iteration++;

                // check if we need to stop
                if ((iterations != 0) && (iteration > iterations))
                    break;
            }

            // show new solution
            for (int j = windowSize, k = 0, n = data.Length; j < n; j++, k++)
            {
                AddSubItem(dataList, j, solution[k, 1].ToString());
            }

            // enable settings controls
            EnableControls(true);
        }
    }
}
