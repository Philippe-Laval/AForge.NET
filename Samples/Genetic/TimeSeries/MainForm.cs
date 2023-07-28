// Time Series Prediction using Genetic Programming and Gene Expression Programming
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
using AForge.Genetic;
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
        private Button loadDataButton;
        private OpenFileDialog openFileDialog;
        private IContainer components;
        private GroupBox groupBox2;
        private Chart chart;
        private GroupBox groupBox3;
        private Label label1;
        private TextBox populationSizeBox;
        private Label label2;
        private ComboBox selectionBox;
        private Label label3;
        private ComboBox functionsSetBox;
        private Label label4;
        private ComboBox geneticMethodBox;
        private Label label5;
        private Label label6;
        private TextBox windowSizeBox;
        private Label label7;
        private TextBox predictionSizeBox;
        private Label label8;
        private Label label9;
        private TextBox iterationsBox;
        private Label label10;
        private Button startButton;
        private Button stopButton;
        private GroupBox groupBox4;
        private Label label11;
        private TextBox currentIterationBox;
        private Label label12;
        private TextBox currentLearningErrorBox;
        private Label label13;
        private TextBox currentPredictionErrorBox;
        private GroupBox groupBox5;
        private TextBox solutionBox;
        private ColumnHeader estimatedYColumnHeader;
        private Button moreSettingsButton;
        private ToolTip toolTip;

        private double[] data = null;
        private double[,] dataToShow = null;

        private int populationSize = 100;
        private int iterations = 1000;
        private int windowSize = 5;
        private int predictionSize = 1;
        private int selectionMethod = 0;
        private int functionsSet = 0;
        private int geneticMethod = 0;

        private int headLength = 20;

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

            //
            chart.AddDataSeries("data", Color.Red, Chart.SeriesType.Dots, 5);
            chart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.Line, 1);
            chart.AddDataSeries("window", Color.LightGray, Chart.SeriesType.Line, 1, false);
            chart.AddDataSeries("prediction", Color.Gray, Chart.SeriesType.Line, 1, false);

            selectionBox.SelectedIndex = selectionMethod;
            functionsSetBox.SelectedIndex = functionsSet;
            geneticMethodBox.SelectedIndex = geneticMethod;
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
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
            groupBox1 = new GroupBox();
            dataList = new ListView();
            yColumnHeader = new ColumnHeader();
            estimatedYColumnHeader = new ColumnHeader();
            loadDataButton = new Button();
            openFileDialog = new OpenFileDialog();
            groupBox2 = new GroupBox();
            chart = new Chart();
            groupBox3 = new GroupBox();
            moreSettingsButton = new Button();
            label10 = new Label();
            iterationsBox = new TextBox();
            label9 = new Label();
            label8 = new Label();
            predictionSizeBox = new TextBox();
            label7 = new Label();
            windowSizeBox = new TextBox();
            label6 = new Label();
            label5 = new Label();
            geneticMethodBox = new ComboBox();
            label4 = new Label();
            functionsSetBox = new ComboBox();
            label3 = new Label();
            selectionBox = new ComboBox();
            label2 = new Label();
            populationSizeBox = new TextBox();
            label1 = new Label();
            startButton = new Button();
            stopButton = new Button();
            groupBox4 = new GroupBox();
            currentPredictionErrorBox = new TextBox();
            label13 = new Label();
            currentLearningErrorBox = new TextBox();
            label12 = new Label();
            currentIterationBox = new TextBox();
            label11 = new Label();
            groupBox5 = new GroupBox();
            solutionBox = new TextBox();
            toolTip = new ToolTip(components);
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
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
            dataList.TabIndex = 1;
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
            groupBox2.Size = new Size(540, 702);
            groupBox2.TabIndex = 1;
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
            // groupBox3
            // 
            groupBox3.Controls.Add(moreSettingsButton);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(iterationsBox);
            groupBox3.Controls.Add(label9);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(predictionSizeBox);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(windowSizeBox);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(geneticMethodBox);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(functionsSetBox);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(selectionBox);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(populationSizeBox);
            groupBox3.Controls.Add(label1);
            groupBox3.Location = new System.Drawing.Point(918, 18);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(333, 444);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Settings";
            // 
            // moreSettingsButton
            // 
            moreSettingsButton.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Bold, GraphicsUnit.Point);
            moreSettingsButton.ForeColor = SystemColors.ControlText;
            moreSettingsButton.Location = new System.Drawing.Point(18, 406);
            moreSettingsButton.Name = "moreSettingsButton";
            moreSettingsButton.Size = new Size(45, 28);
            moreSettingsButton.TabIndex = 17;
            moreSettingsButton.Text = ">>";
            toolTip.SetToolTip(moreSettingsButton, "More settings");
            moreSettingsButton.Click += moreSettingsButton_Click;
            // 
            // label10
            // 
            label10.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(225, 406);
            label10.Name = "label10";
            label10.Size = new Size(104, 26);
            label10.TabIndex = 16;
            label10.Text = "( 0 - inifinity )";
            // 
            // iterationsBox
            // 
            iterationsBox.Location = new System.Drawing.Point(225, 369);
            iterationsBox.Name = "iterationsBox";
            iterationsBox.Size = new Size(90, 31);
            iterationsBox.TabIndex = 15;
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(18, 373);
            label9.Name = "label9";
            label9.Size = new Size(126, 29);
            label9.TabIndex = 14;
            label9.Text = "Iterations:";
            // 
            // label8
            // 
            label8.BorderStyle = BorderStyle.Fixed3D;
            label8.Location = new System.Drawing.Point(18, 351);
            label8.Name = "label8";
            label8.Size = new Size(297, 3);
            label8.TabIndex = 13;
            // 
            // predictionSizeBox
            // 
            predictionSizeBox.Location = new System.Drawing.Point(225, 295);
            predictionSizeBox.Name = "predictionSizeBox";
            predictionSizeBox.Size = new Size(90, 31);
            predictionSizeBox.TabIndex = 12;
            predictionSizeBox.TextChanged += predictionSizeBox_TextChanged;
            // 
            // label7
            // 
            label7.Location = new System.Drawing.Point(18, 299);
            label7.Name = "label7";
            label7.Size = new Size(162, 30);
            label7.TabIndex = 11;
            label7.Text = "Prediction size:";
            // 
            // windowSizeBox
            // 
            windowSizeBox.Location = new System.Drawing.Point(225, 249);
            windowSizeBox.Name = "windowSizeBox";
            windowSizeBox.Size = new Size(90, 31);
            windowSizeBox.TabIndex = 10;
            windowSizeBox.TextChanged += windowSizeBox_TextChanged;
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(18, 253);
            label6.Name = "label6";
            label6.Size = new Size(144, 29);
            label6.TabIndex = 9;
            label6.Text = "Window size:";
            // 
            // label5
            // 
            label5.BorderStyle = BorderStyle.Fixed3D;
            label5.Location = new System.Drawing.Point(18, 231);
            label5.Name = "label5";
            label5.Size = new Size(297, 3);
            label5.TabIndex = 8;
            // 
            // geneticMethodBox
            // 
            geneticMethodBox.DropDownStyle = ComboBoxStyle.DropDownList;
            geneticMethodBox.Items.AddRange(new object[] { "GP", "GEP" });
            geneticMethodBox.Location = new System.Drawing.Point(198, 175);
            geneticMethodBox.Name = "geneticMethodBox";
            geneticMethodBox.Size = new Size(117, 33);
            geneticMethodBox.TabIndex = 7;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(18, 179);
            label4.Name = "label4";
            label4.Size = new Size(180, 30);
            label4.TabIndex = 6;
            label4.Text = "Genetic method:";
            // 
            // functionsSetBox
            // 
            functionsSetBox.DropDownStyle = ComboBoxStyle.DropDownList;
            functionsSetBox.Items.AddRange(new object[] { "Simple", "Extended" });
            functionsSetBox.Location = new System.Drawing.Point(198, 129);
            functionsSetBox.Name = "functionsSetBox";
            functionsSetBox.Size = new Size(117, 33);
            functionsSetBox.TabIndex = 5;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(18, 133);
            label3.Name = "label3";
            label3.Size = new Size(180, 29);
            label3.TabIndex = 4;
            label3.Text = "Function set:";
            // 
            // selectionBox
            // 
            selectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
            selectionBox.Items.AddRange(new object[] { "Elite", "Rank", "Roulette" });
            selectionBox.Location = new System.Drawing.Point(198, 83);
            selectionBox.Name = "selectionBox";
            selectionBox.Size = new Size(117, 33);
            selectionBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(18, 87);
            label2.Name = "label2";
            label2.Size = new Size(180, 29);
            label2.TabIndex = 2;
            label2.Text = "Selection method:";
            // 
            // populationSizeBox
            // 
            populationSizeBox.Location = new System.Drawing.Point(225, 37);
            populationSizeBox.Name = "populationSizeBox";
            populationSizeBox.Size = new Size(90, 31);
            populationSizeBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(18, 41);
            label1.Name = "label1";
            label1.Size = new Size(180, 29);
            label1.TabIndex = 0;
            label1.Text = "Population size:";
            // 
            // startButton
            // 
            startButton.Enabled = false;
            startButton.Location = new System.Drawing.Point(963, 672);
            startButton.Name = "startButton";
            startButton.Size = new Size(135, 42);
            startButton.TabIndex = 3;
            startButton.Text = "&Start";
            startButton.Click += startButton_Click;
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new System.Drawing.Point(1116, 672);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(135, 42);
            stopButton.TabIndex = 4;
            stopButton.Text = "S&top";
            stopButton.Click += stopButton_Click;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(currentPredictionErrorBox);
            groupBox4.Controls.Add(label13);
            groupBox4.Controls.Add(currentLearningErrorBox);
            groupBox4.Controls.Add(label12);
            groupBox4.Controls.Add(currentIterationBox);
            groupBox4.Controls.Add(label11);
            groupBox4.Location = new System.Drawing.Point(918, 471);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(333, 184);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Current iteration:";
            // 
            // currentPredictionErrorBox
            // 
            currentPredictionErrorBox.Location = new System.Drawing.Point(225, 129);
            currentPredictionErrorBox.Name = "currentPredictionErrorBox";
            currentPredictionErrorBox.ReadOnly = true;
            currentPredictionErrorBox.Size = new Size(90, 31);
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
            currentLearningErrorBox.Size = new Size(90, 31);
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
            currentIterationBox.Size = new Size(90, 31);
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
            // groupBox5
            // 
            groupBox5.Controls.Add(solutionBox);
            groupBox5.Location = new System.Drawing.Point(18, 729);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(1233, 93);
            groupBox5.TabIndex = 6;
            groupBox5.TabStop = false;
            groupBox5.Text = "Solution";
            // 
            // solutionBox
            // 
            solutionBox.Location = new System.Drawing.Point(18, 37);
            solutionBox.Name = "solutionBox";
            solutionBox.ReadOnly = true;
            solutionBox.Size = new Size(1197, 31);
            solutionBox.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(9, 24);
            ClientSize = new Size(1306, 854);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Time Series Prediction using Genetic Programming and Gene Expression Programming";
            Closing += MainForm_Closing;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
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
            populationSizeBox.Text = populationSize.ToString();
            iterationsBox.Text = iterations.ToString();
            windowSizeBox.Text = windowSize.ToString();
            predictionSizeBox.Text = predictionSize.ToString();
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
                populationSizeBox.Enabled = enable;
                iterationsBox.Enabled = enable;
                selectionBox.Enabled = enable;
                functionsSetBox.Enabled = enable;
                geneticMethodBox.Enabled = enable;
                windowSizeBox.Enabled = enable;
                predictionSizeBox.Enabled = enable;

                moreSettingsButton.Enabled = enable;

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

        // Clear current solution
        private void ClearSolution()
        {
            // remove solution form chart
            chart.UpdateDataSeries("solution", null);
            // remove it from solution box
            solutionBox.Text = string.Empty;
            // remove it from data list view
            for (int i = 0, n = dataList.Items.Count; i < n; i++)
            {
                if (dataList.Items[i].SubItems.Count > 1)
                    dataList.Items[i].SubItems.RemoveAt(1);
            }
        }

        // On button "Start"
        private void startButton_Click(object sender, EventArgs e)
        {
            ClearSolution();

            // get population size
            try
            {
                populationSize = Math.Max(10, Math.Min(100, int.Parse(populationSizeBox.Text)));
            }
            catch
            {
                populationSize = 40;
            }
            // iterations
            try
            {
                iterations = Math.Max(0, int.Parse(iterationsBox.Text));
            }
            catch
            {
                iterations = 100;
            }
            // update settings controls
            UpdateSettings();

            selectionMethod = selectionBox.SelectedIndex;
            functionsSet = functionsSetBox.SelectedIndex;
            geneticMethod = geneticMethodBox.SelectedIndex;

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
            // constants
            double[] constants = new double[10] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23 };
            // create fitness function
            TimeSeriesPredictionFitness fitness = new TimeSeriesPredictionFitness(
                data, windowSize, predictionSize, constants);
            // create gene function
            IGPGene gene = (functionsSet == 0) ?
                (IGPGene)new SimpleGeneFunction(windowSize + constants.Length) :
                (IGPGene)new ExtendedGeneFunction(windowSize + constants.Length);
            // create population
            Population population = new Population(populationSize,
                (geneticMethod == 0) ?
                (IChromosome)new GPTreeChromosome(gene) :
                (IChromosome)new GEPChromosome(gene, headLength),
                fitness,
                (selectionMethod == 0) ? (ISelectionMethod)new EliteSelection() :
                (selectionMethod == 1) ? (ISelectionMethod)new RankSelection() :
                (ISelectionMethod)new RouletteWheelSelection()
                );
            // iterations
            int i = 1;
            // solution array
            int solutionSize = data.Length - windowSize;
            double[,] solution = new double[solutionSize, 2];
            double[] input = new double[windowSize + constants.Length];

            // calculate X values to be used with solution function
            for (int j = 0; j < solutionSize; j++)
            {
                solution[j, 0] = j + windowSize;
            }
            // prepare input
            Array.Copy(constants, 0, input, windowSize, constants.Length);

            // loop
            while (!needToStop)
            {
                // run one epoch of genetic algorithm
                population.RunEpoch();

                try
                {
                    // get best solution
                    string bestFunction = population.BestChromosome.ToString();

                    // calculate best function and prediction error
                    double learningError = 0.0;
                    double predictionError = 0.0;
                    // go through all the data
                    for (int j = 0, n = data.Length - windowSize; j < n; j++)
                    {
                        // put values from current window as variables
                        for (int k = 0, b = j + windowSize - 1; k < windowSize; k++)
                        {
                            input[k] = data[b - k];
                        }

                        // evalue the function
                        solution[j, 1] = PolishExpression.Evaluate(bestFunction, input);

                        // calculate prediction error
                        if (j >= n - predictionSize)
                        {
                            predictionError += Math.Abs(solution[j, 1] - data[windowSize + j]);
                        }
                        else
                        {
                            learningError += Math.Abs(solution[j, 1] - data[windowSize + j]);
                        }
                    }
                    // update solution on the chart
                    chart.UpdateDataSeries("solution", solution);

                    // set current iteration's info
                    SetText(currentIterationBox, i.ToString());
                    SetText(currentLearningErrorBox, learningError.ToString("F3"));
                    SetText(currentPredictionErrorBox, predictionError.ToString("F3"));
                }
                catch
                {
                    // remove any solutions from chart in case of any errors
                    chart.UpdateDataSeries("solution", null);
                }


                // increase current iteration
                i++;

                //
                if ((iterations != 0) && (i > iterations))
                    break;
            }

            // show solution
            SetText(solutionBox, population.BestChromosome.ToString());
            for (int j = windowSize, k = 0, n = data.Length; j < n; j++, k++)
            {
                AddSubItem(dataList, j, solution[k, 1].ToString());
            }

            // enable settings controls
            EnableControls(true);
        }

        // On "More settings" button click
        private void moreSettingsButton_Click(object sender, EventArgs e)
        {
            ExSettingsDialog settingsDlg = new ExSettingsDialog();

            // init the dialog
            settingsDlg.MaxInitialTreeLevel = GPTreeChromosome.MaxInitialLevel;
            settingsDlg.MaxTreeLevel = GPTreeChromosome.MaxLevel;
            settingsDlg.HeadLength = headLength;

            // show the dialog
            if (settingsDlg.ShowDialog() == DialogResult.OK)
            {
                GPTreeChromosome.MaxInitialLevel = settingsDlg.MaxInitialTreeLevel;
                GPTreeChromosome.MaxLevel = settingsDlg.MaxTreeLevel;
                headLength = settingsDlg.HeadLength;
            }
        }
    }
}
