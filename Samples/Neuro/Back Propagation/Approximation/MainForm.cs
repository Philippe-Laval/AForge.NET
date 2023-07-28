// Approximation using Mutli-Layer Neural Network
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

namespace Approximation
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private GroupBox groupBox1;
        private ListView dataList;
        private Button loadDataButton;
        private ColumnHeader xColumnHeader;
        private ColumnHeader yColumnHeader;
        private OpenFileDialog openFileDialog;
        private GroupBox groupBox2;
        private Chart chart;
        private GroupBox groupBox3;
        private TextBox momentumBox;
        private Label label6;
        private TextBox alphaBox;
        private Label label2;
        private TextBox learningRateBox;
        private Label label1;
        private Label label8;
        private TextBox iterationsBox;
        private Label label10;
        private Label label9;
        private GroupBox groupBox4;
        private TextBox currentErrorBox;
        private Label label3;
        private TextBox currentIterationBox;
        private Label label5;
        private Button stopButton;
        private Button startButton;
        private Label label4;
        private TextBox neuronsBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private double[,] data = null;

        private double learningRate = 0.1;
        private double momentum = 0.0;
        private double sigmoidAlphaValue = 2.0;
        private int neuronsInFirstLayer = 20;
        private int iterations = 1000;

        private Thread workerThread = null;
        private volatile bool needToStop = false;

        // Constructor
        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // init chart control
            chart.AddDataSeries("data", Color.Red, Chart.SeriesType.Dots, 5);
            chart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.Line, 1);

            // init controls
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
            xColumnHeader = new ColumnHeader();
            yColumnHeader = new ColumnHeader();
            loadDataButton = new Button();
            openFileDialog = new OpenFileDialog();
            groupBox2 = new GroupBox();
            chart = new Chart();
            groupBox3 = new GroupBox();
            neuronsBox = new TextBox();
            label4 = new Label();
            momentumBox = new TextBox();
            label6 = new Label();
            alphaBox = new TextBox();
            label2 = new Label();
            learningRateBox = new TextBox();
            label1 = new Label();
            label8 = new Label();
            iterationsBox = new TextBox();
            label10 = new Label();
            label9 = new Label();
            groupBox4 = new GroupBox();
            currentErrorBox = new TextBox();
            label3 = new Label();
            currentIterationBox = new TextBox();
            label5 = new Label();
            stopButton = new Button();
            startButton = new Button();
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
            groupBox1.Size = new Size(324, 591);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Data";
            // 
            // dataList
            // 
            dataList.Columns.AddRange(new ColumnHeader[] { xColumnHeader, yColumnHeader });
            dataList.FullRowSelect = true;
            dataList.GridLines = true;
            dataList.Location = new System.Drawing.Point(18, 37);
            dataList.Name = "dataList";
            dataList.Size = new Size(288, 471);
            dataList.TabIndex = 0;
            dataList.UseCompatibleStateImageBehavior = false;
            dataList.View = View.Details;
            // 
            // xColumnHeader
            // 
            xColumnHeader.Text = "X";
            // 
            // yColumnHeader
            // 
            yColumnHeader.Text = "Y";
            // 
            // loadDataButton
            // 
            loadDataButton.Location = new System.Drawing.Point(18, 526);
            loadDataButton.Name = "loadDataButton";
            loadDataButton.Size = new Size(135, 43);
            loadDataButton.TabIndex = 1;
            loadDataButton.Text = "&Load";
            loadDataButton.Click += loadDataButton_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            openFileDialog.Title = "Select data file";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(chart);
            groupBox2.Location = new System.Drawing.Point(360, 18);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(540, 591);
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
            chart.Size = new Size(504, 535);
            chart.TabIndex = 0;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(neuronsBox);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(momentumBox);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(alphaBox);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(learningRateBox);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(iterationsBox);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(label9);
            groupBox3.Location = new System.Drawing.Point(918, 18);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(351, 360);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Settings";
            // 
            // neuronsBox
            // 
            neuronsBox.Location = new System.Drawing.Point(225, 175);
            neuronsBox.Name = "neuronsBox";
            neuronsBox.Size = new Size(108, 31);
            neuronsBox.TabIndex = 7;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(18, 179);
            label4.Name = "label4";
            label4.Size = new Size(207, 28);
            label4.TabIndex = 6;
            label4.Text = "Neurons in first layer:";
            // 
            // momentumBox
            // 
            momentumBox.Location = new System.Drawing.Point(225, 83);
            momentumBox.Name = "momentumBox";
            momentumBox.Size = new Size(108, 31);
            momentumBox.TabIndex = 3;
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(18, 87);
            label6.Name = "label6";
            label6.Size = new Size(148, 31);
            label6.TabIndex = 2;
            label6.Text = "Momentum:";
            // 
            // alphaBox
            // 
            alphaBox.Location = new System.Drawing.Point(225, 129);
            alphaBox.Name = "alphaBox";
            alphaBox.Size = new Size(108, 31);
            alphaBox.TabIndex = 5;
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(18, 133);
            label2.Name = "label2";
            label2.Size = new Size(216, 28);
            label2.TabIndex = 4;
            label2.Text = "Sigmoid's alpha value:";
            // 
            // learningRateBox
            // 
            learningRateBox.Location = new System.Drawing.Point(225, 37);
            learningRateBox.Name = "learningRateBox";
            learningRateBox.Size = new Size(108, 31);
            learningRateBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(18, 41);
            label1.Name = "label1";
            label1.Size = new Size(140, 25);
            label1.TabIndex = 0;
            label1.Text = "Learning rate:";
            // 
            // label8
            // 
            label8.BorderStyle = BorderStyle.Fixed3D;
            label8.Location = new System.Drawing.Point(18, 271);
            label8.Name = "label8";
            label8.Size = new Size(315, 4);
            label8.TabIndex = 22;
            // 
            // iterationsBox
            // 
            iterationsBox.Location = new System.Drawing.Point(225, 286);
            iterationsBox.Name = "iterationsBox";
            iterationsBox.Size = new Size(108, 31);
            iterationsBox.TabIndex = 9;
            // 
            // label10
            // 
            label10.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(227, 323);
            label10.Name = "label10";
            label10.Size = new Size(104, 26);
            label10.TabIndex = 25;
            label10.Text = "( 0 - inifinity )";
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(18, 290);
            label9.Name = "label9";
            label9.Size = new Size(126, 29);
            label9.TabIndex = 8;
            label9.Text = "Iterations:";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(currentErrorBox);
            groupBox4.Controls.Add(label3);
            groupBox4.Controls.Add(currentIterationBox);
            groupBox4.Controls.Add(label5);
            groupBox4.Location = new System.Drawing.Point(918, 388);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(351, 138);
            groupBox4.TabIndex = 6;
            groupBox4.TabStop = false;
            groupBox4.Text = "Current iteration";
            // 
            // currentErrorBox
            // 
            currentErrorBox.Location = new System.Drawing.Point(225, 83);
            currentErrorBox.Name = "currentErrorBox";
            currentErrorBox.ReadOnly = true;
            currentErrorBox.Size = new Size(108, 31);
            currentErrorBox.TabIndex = 3;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(18, 87);
            label3.Name = "label3";
            label3.Size = new Size(126, 29);
            label3.TabIndex = 2;
            label3.Text = "Error:";
            // 
            // currentIterationBox
            // 
            currentIterationBox.Location = new System.Drawing.Point(225, 37);
            currentIterationBox.Name = "currentIterationBox";
            currentIterationBox.ReadOnly = true;
            currentIterationBox.Size = new Size(108, 31);
            currentIterationBox.TabIndex = 1;
            // 
            // label5
            // 
            label5.Location = new System.Drawing.Point(18, 41);
            label5.Name = "label5";
            label5.Size = new Size(126, 29);
            label5.TabIndex = 0;
            label5.Text = "Iteration:";
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new System.Drawing.Point(1134, 563);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(135, 43);
            stopButton.TabIndex = 8;
            stopButton.Text = "S&top";
            stopButton.Click += stopButton_Click;
            // 
            // startButton
            // 
            startButton.Enabled = false;
            startButton.Location = new System.Drawing.Point(972, 563);
            startButton.Name = "startButton";
            startButton.Size = new Size(135, 43);
            startButton.TabIndex = 7;
            startButton.Text = "&Start";
            startButton.Click += startButton_Click;
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(9, 24);
            ClientSize = new Size(1294, 649);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Approximation using Multi-Layer Neural Network";
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
            neuronsBox.Text = neuronsInFirstLayer.ToString();
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
                float[,] tempData = new float[50, 2];
                float minX = float.MaxValue;
                float maxX = float.MinValue;

                try
                {
                    // open selected file
                    reader = File.OpenText(openFileDialog.FileName);
                    string str = null;
                    int i = 0;

                    // read the data
                    while ((i < 50) && ((str = reader.ReadLine()) != null))
                    {
                        string[] strs = str.Split(';');
                        if (strs.Length == 1)
                            strs = str.Split(',');
                        // parse X
                        tempData[i, 0] = float.Parse(strs[0], CultureInfo.InvariantCulture);
                        tempData[i, 1] = float.Parse(strs[1], CultureInfo.InvariantCulture);

                        // search for min value
                        if (tempData[i, 0] < minX)
                            minX = tempData[i, 0];
                        // search for max value
                        if (tempData[i, 0] > maxX)
                            maxX = tempData[i, 0];

                        i++;
                    }

                    // allocate and set data
                    data = new double[i, 2];
                    Array.Copy(tempData, 0, data, 0, i * 2);
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
                chart.RangeX = new AForge.Range(minX, maxX);
                chart.UpdateDataSeries("data", data);
                chart.UpdateDataSeries("solution", null);
                // enable "Start" button
                startButton.Enabled = true;
            }
        }

        // Update data in list view
        private void UpdateDataListView()
        {
            // remove all current records
            dataList.Items.Clear();
            // add new records
            for (int i = 0, n = data.GetLength(0); i < n; i++)
            {
                dataList.Items.Add(data[i, 0].ToString());
                dataList.Items[i].SubItems.Add(data[i, 1].ToString());
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
                loadDataButton.Enabled = enable;
                learningRateBox.Enabled = enable;
                momentumBox.Enabled = enable;
                alphaBox.Enabled = enable;
                neuronsBox.Enabled = enable;
                iterationsBox.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // On button "Start"
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
            // get neurons count in first layer
            try
            {
                neuronsInFirstLayer = Math.Max(5, Math.Min(50, int.Parse(neuronsBox.Text)));
            }
            catch
            {
                neuronsInFirstLayer = 20;
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
            int samples = data.GetLength(0);
            // data transformation factor
            double yFactor = 1.7 / chart.RangeY.Length;
            double yMin = chart.RangeY.Min;
            double xFactor = 2.0 / chart.RangeX.Length;
            double xMin = chart.RangeX.Min;

            // prepare learning data
            double[][] input = new double[samples][];
            double[][] output = new double[samples][];

            for (int i = 0; i < samples; i++)
            {
                input[i] = new double[1];
                output[i] = new double[1];

                // set input
                input[i][0] = (data[i, 0] - xMin) * xFactor - 1.0;
                // set output
                output[i][0] = (data[i, 1] - yMin) * yFactor - 0.85;
            }

            // create multi-layer neural network
            ActivationNetwork network = new ActivationNetwork(
                new BipolarSigmoidFunction(sigmoidAlphaValue),
                1, neuronsInFirstLayer, 1);
            // create teacher
            BackPropagationLearning teacher = new BackPropagationLearning(network);
            // set learning rate and momentum
            teacher.LearningRate = learningRate;
            teacher.Momentum = momentum;

            // iterations
            int iteration = 1;

            // solution array
            double[,] solution = new double[50, 2];
            double[] networkInput = new double[1];

            // calculate X values to be used with solution function
            for (int j = 0; j < 50; j++)
            {
                solution[j, 0] = chart.RangeX.Min + (double)j * chart.RangeX.Length / 49;
            }

            // loop
            while (!needToStop)
            {
                // run epoch of learning procedure
                double error = teacher.RunEpoch(input, output) / samples;

                // calculate solution
                for (int j = 0; j < 50; j++)
                {
                    networkInput[0] = (solution[j, 0] - xMin) * xFactor - 1.0;
                    solution[j, 1] = (network.Compute(networkInput)[0] + 0.85) / yFactor + yMin;
                }
                chart.UpdateDataSeries("solution", solution);
                // calculate error
                double learningError = 0.0;
                for (int j = 0, k = data.GetLength(0); j < k; j++)
                {
                    networkInput[0] = input[j][0];
                    learningError += Math.Abs(data[j, 1] - ((network.Compute(networkInput)[0] + 0.85) / yFactor + yMin));
                }

                // set current iteration's info
                SetText(currentIterationBox, iteration.ToString());
                SetText(currentErrorBox, learningError.ToString("F3"));

                // increase current iteration
                iteration++;

                // check if we need to stop
                if ((iterations != 0) && (iteration > iterations))
                    break;
            }


            // enable settings controls
            EnableControls(true);
        }
    }
}
