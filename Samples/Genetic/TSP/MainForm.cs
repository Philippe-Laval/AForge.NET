// Traveling Salesman Problem using Genetic Algorithms
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

using AForge;
using AForge.Genetic;
using AForge.Controls;

namespace TSP
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private GroupBox groupBox1;
        private Chart mapControl;
        private Label label1;
        private TextBox citiesCountBox;
        private Button generateMapButton;
        private GroupBox groupBox2;
        private Label label2;
        private TextBox populationSizeBox;
        private Label label3;
        private ComboBox selectionBox;
        private Label label4;
        private TextBox iterationsBox;
        private Label label5;
        private GroupBox groupBox3;
        private Button startButton;
        private Button stopButton;
        private Label label6;
        private TextBox currentIterationBox;
        private Label label7;
        private TextBox pathLengthBox;
        private CheckBox greedyCrossoverBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private int citiesCount = 20;
        private int populationSize = 40;
        private int iterations = 100;
        private int selectionMethod = 0;
        private bool greedyCrossover = true;

        private double[,] map = null;

        private Thread workerThread = null;
        private volatile bool needToStop = false;

        // Constructor
        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // set up map control
            mapControl.RangeX = new AForge.Range(0, 1000);
            mapControl.RangeY = new AForge.Range(0, 1000);
            mapControl.AddDataSeries("map", Color.Red, Chart.SeriesType.Dots, 5, false);
            mapControl.AddDataSeries("path", Color.Blue, Chart.SeriesType.Line, 1, false);

            //
            selectionBox.SelectedIndex = selectionMethod;
            greedyCrossoverBox.Checked = greedyCrossover;
            UpdateSettings();
            GenerateMap();
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
            generateMapButton = new Button();
            citiesCountBox = new TextBox();
            label1 = new Label();
            mapControl = new Chart();
            groupBox2 = new GroupBox();
            greedyCrossoverBox = new CheckBox();
            label5 = new Label();
            iterationsBox = new TextBox();
            label4 = new Label();
            selectionBox = new ComboBox();
            label3 = new Label();
            populationSizeBox = new TextBox();
            label2 = new Label();
            groupBox3 = new GroupBox();
            pathLengthBox = new TextBox();
            label7 = new Label();
            currentIterationBox = new TextBox();
            label6 = new Label();
            startButton = new Button();
            stopButton = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(generateMapButton);
            groupBox1.Controls.Add(citiesCountBox);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(mapControl);
            groupBox1.Location = new System.Drawing.Point(18, 18);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(540, 628);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Map";
            // 
            // generateMapButton
            // 
            generateMapButton.Location = new System.Drawing.Point(198, 570);
            generateMapButton.Name = "generateMapButton";
            generateMapButton.Size = new Size(135, 41);
            generateMapButton.TabIndex = 3;
            generateMapButton.Text = "&Generate";
            generateMapButton.Click += generateMapButton_Click;
            // 
            // citiesCountBox
            // 
            citiesCountBox.Location = new System.Drawing.Point(90, 572);
            citiesCountBox.Name = "citiesCountBox";
            citiesCountBox.Size = new Size(90, 31);
            citiesCountBox.TabIndex = 2;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(18, 576);
            label1.Name = "label1";
            label1.Size = new Size(72, 30);
            label1.TabIndex = 1;
            label1.Text = "Cities:";
            // 
            // mapControl
            // 
            mapControl.Location = new System.Drawing.Point(18, 37);
            mapControl.Name = "mapControl";
            mapControl.RangeX = (AForge.Range)resources.GetObject("mapControl.RangeX");
            mapControl.RangeY = (AForge.Range)resources.GetObject("mapControl.RangeY");
            mapControl.Size = new Size(504, 517);
            mapControl.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(greedyCrossoverBox);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(iterationsBox);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(selectionBox);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(populationSizeBox);
            groupBox2.Controls.Add(label2);
            groupBox2.Location = new System.Drawing.Point(576, 18);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(333, 416);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Settings";
            // 
            // greedyCrossoverBox
            // 
            greedyCrossoverBox.Location = new System.Drawing.Point(18, 129);
            greedyCrossoverBox.Name = "greedyCrossoverBox";
            greedyCrossoverBox.Size = new Size(216, 45);
            greedyCrossoverBox.TabIndex = 7;
            greedyCrossoverBox.Text = "Greedy crossover";
            // 
            // label5
            // 
            label5.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new System.Drawing.Point(225, 369);
            label5.Name = "label5";
            label5.Size = new Size(104, 28);
            label5.TabIndex = 6;
            label5.Text = "( 0 - inifinity )";
            // 
            // iterationsBox
            // 
            iterationsBox.Location = new System.Drawing.Point(225, 332);
            iterationsBox.Name = "iterationsBox";
            iterationsBox.Size = new Size(90, 31);
            iterationsBox.TabIndex = 5;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(18, 336);
            label4.Name = "label4";
            label4.Size = new Size(108, 30);
            label4.TabIndex = 4;
            label4.Text = "Iterations:";
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
            // label3
            // 
            label3.Location = new System.Drawing.Point(18, 87);
            label3.Name = "label3";
            label3.Size = new Size(180, 29);
            label3.TabIndex = 2;
            label3.Text = "Selection method:";
            // 
            // populationSizeBox
            // 
            populationSizeBox.Location = new System.Drawing.Point(225, 37);
            populationSizeBox.Name = "populationSizeBox";
            populationSizeBox.Size = new Size(90, 31);
            populationSizeBox.TabIndex = 1;
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(18, 41);
            label2.Name = "label2";
            label2.Size = new Size(162, 29);
            label2.TabIndex = 0;
            label2.Text = "Population size:";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(pathLengthBox);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(currentIterationBox);
            groupBox3.Controls.Add(label6);
            groupBox3.Location = new System.Drawing.Point(576, 443);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(333, 139);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Current iteration";
            // 
            // pathLengthBox
            // 
            pathLengthBox.Location = new System.Drawing.Point(225, 83);
            pathLengthBox.Name = "pathLengthBox";
            pathLengthBox.ReadOnly = true;
            pathLengthBox.Size = new Size(90, 31);
            pathLengthBox.TabIndex = 3;
            // 
            // label7
            // 
            label7.Location = new System.Drawing.Point(18, 87);
            label7.Name = "label7";
            label7.Size = new Size(144, 29);
            label7.TabIndex = 2;
            label7.Text = "Path length:";
            // 
            // currentIterationBox
            // 
            currentIterationBox.Location = new System.Drawing.Point(225, 37);
            currentIterationBox.Name = "currentIterationBox";
            currentIterationBox.ReadOnly = true;
            currentIterationBox.Size = new Size(90, 31);
            currentIterationBox.TabIndex = 1;
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(18, 41);
            label6.Name = "label6";
            label6.Size = new Size(90, 29);
            label6.TabIndex = 0;
            label6.Text = "Iteration:";
            // 
            // startButton
            // 
            startButton.Location = new System.Drawing.Point(612, 600);
            startButton.Name = "startButton";
            startButton.Size = new Size(135, 42);
            startButton.TabIndex = 3;
            startButton.Text = "&Start";
            startButton.Click += startButton_Click;
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new System.Drawing.Point(774, 600);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(135, 42);
            stopButton.TabIndex = 4;
            stopButton.Text = "S&top";
            stopButton.Click += stopButton_Click;
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(9, 24);
            ClientSize = new Size(1005, 682);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Traveling Salesman Problem using Genetic Algorithms";
            Closing += MainForm_Closing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
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
            citiesCountBox.Text = citiesCount.ToString();
            populationSizeBox.Text = populationSize.ToString();
            iterationsBox.Text = iterations.ToString();
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
                citiesCountBox.Enabled = enable;
                populationSizeBox.Enabled = enable;
                iterationsBox.Enabled = enable;
                selectionBox.Enabled = enable;

                generateMapButton.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // Generate new map for the Traivaling Salesman problem
        private void GenerateMap()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);

            // create coordinates array
            map = new double[citiesCount, 2];

            for (int i = 0; i < citiesCount; i++)
            {
                map[i, 0] = rand.Next(1001);
                map[i, 1] = rand.Next(1001);
            }

            // set the map
            mapControl.UpdateDataSeries("map", map);
            // erase path if it is
            mapControl.UpdateDataSeries("path", null);
        }

        // On "Generate" button click - generate map
        private void generateMapButton_Click(object sender, EventArgs e)
        {
            // get cities count
            try
            {
                citiesCount = Math.Max(5, Math.Min(50, int.Parse(citiesCountBox.Text)));
            }
            catch
            {
                citiesCount = 20;
            }
            citiesCountBox.Text = citiesCount.ToString();

            // regenerate map
            GenerateMap();
        }

        // On "Start" button click
        private void startButton_Click(object sender, EventArgs e)
        {
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
            greedyCrossover = greedyCrossoverBox.Checked;

            // disable all settings controls except "Stop" button
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
            if (workerThread != null)
            {
                needToStop = true;
                while (!workerThread.Join(100))
                    Application.DoEvents();
                workerThread = null;
            }
        }

        // Worker thread
        void SearchSolution()
        {
            // create fitness function
            TSPFitnessFunction fitnessFunction = new TSPFitnessFunction(map);
            // create population
            Population population = new Population(populationSize,
                (greedyCrossover) ? new TSPChromosome(map) : new PermutationChromosome(citiesCount),
                fitnessFunction,
                (selectionMethod == 0) ? (ISelectionMethod)new EliteSelection() :
                (selectionMethod == 1) ? (ISelectionMethod)new RankSelection() :
                (ISelectionMethod)new RouletteWheelSelection()
                );
            // iterations
            int i = 1;

            // path
            double[,] path = new double[citiesCount + 1, 2];

            // loop
            while (!needToStop)
            {
                // run one epoch of genetic algorithm
                population.RunEpoch();

                // display current path
                ushort[] bestValue = ((PermutationChromosome)population.BestChromosome).Value;

                for (int j = 0; j < citiesCount; j++)
                {
                    path[j, 0] = map[bestValue[j], 0];
                    path[j, 1] = map[bestValue[j], 1];
                }
                path[citiesCount, 0] = map[bestValue[0], 0];
                path[citiesCount, 1] = map[bestValue[0], 1];

                mapControl.UpdateDataSeries("path", path);

                // set current iteration's info
                SetText(currentIterationBox, i.ToString());
                SetText(pathLengthBox, fitnessFunction.PathLength(population.BestChromosome).ToString());

                // increase current iteration
                i++;

                //
                if ((iterations != 0) && (i > iterations))
                    break;
            }

            // enable settings controls
            EnableControls(true);
        }
    }
}
