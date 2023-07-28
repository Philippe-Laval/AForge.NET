// Fuzzy Auto Guided Vehicle Sample
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Diagnostics;
using AForge.Fuzzy;

namespace FuzzyAGV
{
    public class MainForm : System.Windows.Forms.Form
    {
        #region Private members
        private string RunLabel;
        private Point InitialPos;
        private bool FirstInference;
        private int LastX;
        private int LastY;
        private double Angle;
        private Bitmap OriginalMap, InitialMap;
        private InferenceSystem IS;
        private Thread thMovement;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private PictureBox pbTerrain;
        private Button btnRun;
        private TextBox txtInterval;
        private CheckBox cbLasers;
        private GroupBox groupBox1;
        private Label txtRight;
        private Label txtLeft;
        private Label txtFront;
        private Label lbl;
        private Label label2;
        private Label label1;
        private GroupBox groupBox2;
        private Label label3;
        private Label txtAngle;
        private Button btnStep;
        private Label label4;
        private PictureBox pbRobot;
        private Button btnReset;
        private GroupBox gbComandos;
        private Container components = null;
        private GroupBox groupBox3;
        private Label label5;
        private Button aboutButton;
        private CheckBox cbTrajeto;
        #endregion

        #region Class constructor, destructor and Main method

        public MainForm()
        {
            InitializeComponent();
            Angle = 0;
            OriginalMap = new Bitmap(pbTerrain.Image);
            InitialMap = new Bitmap(pbTerrain.Image);

            InitFuzzyEngine();
            FirstInference = true;
            pbRobot.Top = pbTerrain.Bottom - 50;
            pbRobot.Left = pbTerrain.Left + 60;
            InitialPos = pbRobot.Location;
            RunLabel = btnRun.Text;
        }

        /// <summary>
        /// Stoping the movement thread
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            StopMovement();
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
            pbTerrain = new PictureBox();
            btnStep = new Button();
            btnRun = new Button();
            txtInterval = new TextBox();
            cbLasers = new CheckBox();
            groupBox1 = new GroupBox();
            txtRight = new Label();
            txtLeft = new Label();
            txtFront = new Label();
            lbl = new Label();
            label2 = new Label();
            label1 = new Label();
            groupBox2 = new GroupBox();
            label3 = new Label();
            txtAngle = new Label();
            gbComandos = new GroupBox();
            cbTrajeto = new CheckBox();
            btnReset = new Button();
            label4 = new Label();
            pbRobot = new PictureBox();
            groupBox3 = new GroupBox();
            label5 = new Label();
            aboutButton = new Button();
            ((ISupportInitialize)pbTerrain).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            gbComandos.SuspendLayout();
            ((ISupportInitialize)pbRobot).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // pbTerrain
            // 
            pbTerrain.BackColor = SystemColors.ControlText;
            pbTerrain.ErrorImage = null;
            pbTerrain.Image = (Image)resources.GetObject("pbTerrain.Image");
            pbTerrain.InitialImage = null;
            pbTerrain.Location = new Point(288, 15);
            pbTerrain.Name = "pbTerrain";
            pbTerrain.Size = new Size(500, 500);
            pbTerrain.SizeMode = PictureBoxSizeMode.AutoSize;
            pbTerrain.TabIndex = 10;
            pbTerrain.TabStop = false;
            pbTerrain.MouseDown += pbTerrain_MouseDown;
            pbTerrain.MouseMove += pbTerrain_MouseMove;
            // 
            // btnStep
            // 
            btnStep.FlatStyle = FlatStyle.Flat;
            btnStep.Location = new Point(11, 201);
            btnStep.Name = "btnStep";
            btnStep.Size = new Size(135, 43);
            btnStep.TabIndex = 14;
            btnStep.Text = "&One Step";
            btnStep.Click += button3_Click;
            // 
            // btnRun
            // 
            btnRun.FlatStyle = FlatStyle.Flat;
            btnRun.Location = new Point(11, 255);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(135, 42);
            btnRun.TabIndex = 15;
            btnRun.Text = "&Run";
            btnRun.Click += btnRun_Click;
            // 
            // txtInterval
            // 
            txtInterval.Location = new Point(11, 153);
            txtInterval.Name = "txtInterval";
            txtInterval.Size = new Size(129, 31);
            txtInterval.TabIndex = 16;
            txtInterval.Text = "10";
            txtInterval.TextAlign = HorizontalAlignment.Right;
            // 
            // cbLasers
            // 
            cbLasers.Checked = true;
            cbLasers.CheckState = CheckState.Checked;
            cbLasers.Location = new Point(14, 74);
            cbLasers.Name = "cbLasers";
            cbLasers.Size = new Size(216, 44);
            cbLasers.TabIndex = 17;
            cbLasers.Text = "&Show Beams";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtRight);
            groupBox1.Controls.Add(txtLeft);
            groupBox1.Controls.Add(txtFront);
            groupBox1.Controls.Add(lbl);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(14, 15);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(260, 133);
            groupBox1.TabIndex = 24;
            groupBox1.TabStop = false;
            groupBox1.Text = "Sensor readings::";
            // 
            // txtRight
            // 
            txtRight.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtRight.Location = new Point(187, 89);
            txtRight.Name = "txtRight";
            txtRight.RightToLeft = RightToLeft.Yes;
            txtRight.Size = new Size(58, 29);
            txtRight.TabIndex = 29;
            txtRight.Text = "0";
            txtRight.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtLeft
            // 
            txtLeft.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtLeft.Location = new Point(187, 59);
            txtLeft.Name = "txtLeft";
            txtLeft.RightToLeft = RightToLeft.Yes;
            txtLeft.Size = new Size(58, 30);
            txtLeft.TabIndex = 28;
            txtLeft.Text = "0";
            txtLeft.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtFront
            // 
            txtFront.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtFront.Location = new Point(187, 30);
            txtFront.Name = "txtFront";
            txtFront.RightToLeft = RightToLeft.Yes;
            txtFront.Size = new Size(58, 29);
            txtFront.TabIndex = 27;
            txtFront.Text = "0";
            txtFront.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbl
            // 
            lbl.Location = new Point(14, 89);
            lbl.Name = "lbl";
            lbl.Size = new Size(180, 29);
            lbl.TabIndex = 26;
            lbl.Text = "Right (pixels):";
            // 
            // label2
            // 
            label2.Location = new Point(14, 59);
            label2.Name = "label2";
            label2.Size = new Size(180, 30);
            label2.TabIndex = 25;
            label2.Text = "Left (pixels):";
            // 
            // label1
            // 
            label1.Location = new Point(14, 30);
            label1.Name = "label1";
            label1.Size = new Size(159, 29);
            label1.TabIndex = 24;
            label1.Text = "Frontal (pixels):";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(txtAngle);
            groupBox2.Location = new Point(14, 162);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(260, 74);
            groupBox2.TabIndex = 25;
            groupBox2.TabStop = false;
            groupBox2.Text = "Output:";
            // 
            // label3
            // 
            label3.Location = new Point(14, 30);
            label3.Name = "label3";
            label3.Size = new Size(159, 29);
            label3.TabIndex = 10;
            label3.Text = "Angle (degrees):";
            // 
            // txtAngle
            // 
            txtAngle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtAngle.Location = new Point(173, 30);
            txtAngle.Name = "txtAngle";
            txtAngle.Size = new Size(72, 29);
            txtAngle.TabIndex = 29;
            txtAngle.Text = "0,00";
            txtAngle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // gbComandos
            // 
            gbComandos.Controls.Add(cbTrajeto);
            gbComandos.Controls.Add(btnReset);
            gbComandos.Controls.Add(label4);
            gbComandos.Controls.Add(btnStep);
            gbComandos.Controls.Add(cbLasers);
            gbComandos.Controls.Add(btnRun);
            gbComandos.Controls.Add(txtInterval);
            gbComandos.Location = new Point(14, 251);
            gbComandos.Name = "gbComandos";
            gbComandos.Size = new Size(260, 369);
            gbComandos.TabIndex = 26;
            gbComandos.TabStop = false;
            gbComandos.Text = "Tools:";
            // 
            // cbTrajeto
            // 
            cbTrajeto.Location = new Point(14, 30);
            cbTrajeto.Name = "cbTrajeto";
            cbTrajeto.Size = new Size(216, 44);
            cbTrajeto.TabIndex = 19;
            cbTrajeto.Text = "&Track Path";
            // 
            // btnReset
            // 
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Location = new Point(11, 308);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(135, 43);
            btnReset.TabIndex = 0;
            btnReset.Text = "Rest&art";
            btnReset.Click += btnReset_Click;
            // 
            // label4
            // 
            label4.Location = new Point(11, 124);
            label4.Name = "label4";
            label4.Size = new Size(225, 24);
            label4.TabIndex = 18;
            label4.Text = "Move Interval (ms):";
            // 
            // pbRobot
            // 
            pbRobot.BackColor = Color.Transparent;
            pbRobot.Image = (Image)resources.GetObject("pbRobot.Image");
            pbRobot.Location = new Point(354, 441);
            pbRobot.Name = "pbRobot";
            pbRobot.Size = new Size(18, 19);
            pbRobot.TabIndex = 11;
            pbRobot.TabStop = false;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label5);
            groupBox3.Location = new Point(14, 631);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(260, 220);
            groupBox3.TabIndex = 27;
            groupBox3.TabStop = false;
            groupBox3.Text = "Hints:";
            // 
            // label5
            // 
            label5.Location = new Point(14, 30);
            label5.Name = "label5";
            label5.Size = new Size(225, 162);
            label5.TabIndex = 10;
            label5.Text = "Left click the image to draw passages (white), right click the image to draw walls (black).";
            // 
            // aboutButton
            // 
            aboutButton.FlatStyle = FlatStyle.Flat;
            aboutButton.Location = new Point(74, 873);
            aboutButton.Name = "aboutButton";
            aboutButton.Size = new Size(135, 43);
            aboutButton.TabIndex = 28;
            aboutButton.Text = "About";
            aboutButton.UseVisualStyleBackColor = true;
            aboutButton.Click += aboutButton_Click;
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(9, 24);
            ClientSize = new Size(1242, 949);
            Controls.Add(aboutButton);
            Controls.Add(groupBox3);
            Controls.Add(gbComandos);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(pbRobot);
            Controls.Add(pbTerrain);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Fuzzy Auto Guided Vehicle Sample";
            ((ISupportInitialize)pbTerrain).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            gbComandos.ResumeLayout(false);
            gbComandos.PerformLayout();
            ((ISupportInitialize)pbRobot).EndInit();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        // Hardcode initializing the Fuzzy Inference System
        void InitFuzzyEngine()
        {

            // Linguistic labels (fuzzy sets) that compose the distances
            FuzzySet fsNear = new FuzzySet("Near", new TrapezoidalFunction(15, 50, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsMedium = new FuzzySet("Medium", new TrapezoidalFunction(15, 50, 60, 100));
            FuzzySet fsFar = new FuzzySet("Far", new TrapezoidalFunction(60, 100, TrapezoidalFunction.EdgeType.Left));

            // Right Distance (Input)
            LinguisticVariable lvRight = new LinguisticVariable("RightDistance", 0, 120);
            lvRight.AddLabel(fsNear);
            lvRight.AddLabel(fsMedium);
            lvRight.AddLabel(fsFar);

            // Left Distance (Input)
            LinguisticVariable lvLeft = new LinguisticVariable("LeftDistance", 0, 120);
            lvLeft.AddLabel(fsNear);
            lvLeft.AddLabel(fsMedium);
            lvLeft.AddLabel(fsFar);

            // Front Distance (Input)
            LinguisticVariable lvFront = new LinguisticVariable("FrontalDistance", 0, 120);
            lvFront.AddLabel(fsNear);
            lvFront.AddLabel(fsMedium);
            lvFront.AddLabel(fsFar);

            // Linguistic labels (fuzzy sets) that compose the angle
            FuzzySet fsVN = new FuzzySet("VeryNegative", new TrapezoidalFunction(-40, -35, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsN = new FuzzySet("Negative", new TrapezoidalFunction(-40, -35, -25, -20));
            FuzzySet fsLN = new FuzzySet("LittleNegative", new TrapezoidalFunction(-25, -20, -10, -5));
            FuzzySet fsZero = new FuzzySet("Zero", new TrapezoidalFunction(-10, 5, 5, 10));
            FuzzySet fsLP = new FuzzySet("LittlePositive", new TrapezoidalFunction(5, 10, 20, 25));
            FuzzySet fsP = new FuzzySet("Positive", new TrapezoidalFunction(20, 25, 35, 40));
            FuzzySet fsVP = new FuzzySet("VeryPositive", new TrapezoidalFunction(35, 40, TrapezoidalFunction.EdgeType.Left));

            // Angle
            LinguisticVariable lvAngle = new LinguisticVariable("Angle", -50, 50);
            lvAngle.AddLabel(fsVN);
            lvAngle.AddLabel(fsN);
            lvAngle.AddLabel(fsLN);
            lvAngle.AddLabel(fsZero);
            lvAngle.AddLabel(fsLP);
            lvAngle.AddLabel(fsP);
            lvAngle.AddLabel(fsVP);

            // The database
            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvFront);
            fuzzyDB.AddVariable(lvLeft);
            fuzzyDB.AddVariable(lvRight);
            fuzzyDB.AddVariable(lvAngle);

            // Creating the inference system
            IS = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));

            // Going Straight
            IS.NewRule("Rule 1", "IF FrontalDistance IS Far THEN Angle IS Zero");
            // Going Straight (if can go anywhere)
            IS.NewRule("Rule 2", "IF FrontalDistance IS Far AND RightDistance IS Far AND LeftDistance IS Far THEN Angle IS Zero");
            // Near right wall
            IS.NewRule("Rule 3", "IF RightDistance IS Near AND LeftDistance IS Not Near THEN Angle IS LittleNegative");
            // Near left wall
            IS.NewRule("Rule 4", "IF RightDistance IS Not Near AND LeftDistance IS Near THEN Angle IS LittlePositive");
            // Near front wall - room at right
            IS.NewRule("Rule 5", "IF RightDistance IS Far AND FrontalDistance IS Near THEN Angle IS Positive");
            // Near front wall - room at left
            IS.NewRule("Rule 6", "IF LeftDistance IS Far AND FrontalDistance IS Near THEN Angle IS Negative");
            // Near front wall - room at both sides - go right
            IS.NewRule("Rule 7", "IF RightDistance IS Far AND LeftDistance IS Far AND FrontalDistance IS Near THEN Angle IS Positive");
        }

        // Run one epoch of the Fuzzy Inference System 
        private void DoInference()
        {
            // Setting inputs
            IS.SetInput("RightDistance", Convert.ToSingle(txtRight.Text));
            IS.SetInput("LeftDistance", Convert.ToSingle(txtLeft.Text));
            IS.SetInput("FrontalDistance", Convert.ToSingle(txtFront.Text));

            // Setting outputs
            try
            {
                double NewAngle = IS.Evaluate("Angle");
                txtAngle.Text = NewAngle.ToString("##0.#0");
                Angle += NewAngle;
            }
            catch (Exception)
            {
            }
        }

        // AGV's terrain drawing
        private void pbTerrain_MouseDown(object sender, MouseEventArgs e)
        {
            pbTerrain.Image = CopyImage(OriginalMap);
            LastX = e.X;
            LastY = e.Y;
        }

        // AGV's terrain drawing
        private void pbTerrain_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(pbTerrain.Image);

            Color c = Color.Yellow;

            if (e.Button == MouseButtons.Left)
                c = Color.White;
            else if (e.Button == MouseButtons.Right)
                c = Color.Black;

            if (c != Color.Yellow)
            {
                g.FillRectangle(new SolidBrush(c), e.X - 40, e.Y - 40, 80, 80);

                LastX = e.X;
                LastY = e.Y;

                g.DrawImage(pbTerrain.Image, 0, 0);
                OriginalMap = CopyImage(pbTerrain.Image as Bitmap);
                pbTerrain.Refresh();
                g.Dispose();
            }

        }

        // Getting sensors measures
        private void GetMeasures()
        {
            // Getting AGV's position
            pbTerrain.Image = CopyImage(OriginalMap);
            Bitmap b = pbTerrain.Image as Bitmap;
            Point pPos = new Point(pbRobot.Left - pbTerrain.Left + 5, pbRobot.Top - pbTerrain.Top + 5);

            // AGV on the wall
            if ((b.GetPixel(pPos.X, pPos.Y).R == 0) && (b.GetPixel(pPos.X, pPos.Y).G == 0) && (b.GetPixel(pPos.X, pPos.Y).B == 0))
            {
                if (btnRun.Text != RunLabel)
                {
                    btnRun_Click(btnRun, null);
                }
                string Msg = "The vehicle is on the solid area!";
                MessageBox.Show(Msg, "Error!");
                throw new Exception(Msg);
            }

            // Getting distances
            Point pFrontObstacle = GetObstacle(pPos, b, -1, 0);
            Point pLeftObstacle = GetObstacle(pPos, b, 1, 90);
            Point pRightObstacle = GetObstacle(pPos, b, 1, -90);

            // Showing beams
            Graphics g = Graphics.FromImage(b);
            if (cbLasers.Checked)
            {
                g.DrawLine(new Pen(Color.Green, 1), pFrontObstacle, pPos);
                g.DrawLine(new Pen(Color.Red, 1), pLeftObstacle, pPos);
                g.DrawLine(new Pen(Color.Red, 1), pRightObstacle, pPos);
            }

            // Drawing AGV
            if (btnRun.Text != RunLabel)
            {
                g.FillEllipse(new SolidBrush(Color.Navy), pPos.X - 5, pPos.Y - 5, 10, 10);
            }

            g.DrawImage(b, 0, 0);
            g.Dispose();

            pbTerrain.Refresh();

            // Updating distances texts
            txtFront.Text = GetDistance(pPos, pFrontObstacle).ToString();
            txtLeft.Text = GetDistance(pPos, pLeftObstacle).ToString();
            txtRight.Text = GetDistance(pPos, pRightObstacle).ToString();

        }

        // Calculating distances
        private int GetDistance(Point p1, Point p2)
        {
            return (Convert.ToInt32(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2))));
        }

        // Finding obstacles
        private Point GetObstacle(Point Start, Bitmap Map, int Inc, int AngleOffset)
        {
            Point p = new Point(Start.X, Start.Y);

            double rad = ((Angle + 90 + AngleOffset) * Math.PI) / 180;
            int IncX = 0;
            int IncY = 0;
            int Offset = 0;

            while ((p.X + IncX >= 0) && (p.X + IncX < Map.Width) && (p.Y + IncY >= 0) && (p.Y + IncY < Map.Height))
            {
                if ((Map.GetPixel(p.X + IncX, p.Y + IncY).R == 0) && (Map.GetPixel(p.X + IncX, p.Y + IncY).G == 0) && (Map.GetPixel(p.X + IncX, p.Y + IncY).B == 0))
                    break;
                Offset += Inc;
                IncX = Convert.ToInt32(Offset * Math.Cos(rad));
                IncY = Convert.ToInt32(Offset * Math.Sin(rad));
            }
            p.X += IncX;
            p.Y += IncY;

            return p;
        }

        // Copying bitmaps
        private Bitmap CopyImage(Bitmap Src)
        {
            return new Bitmap(Src);
        }


        // Restarting the AGVs simulation
        private void btnReset_Click(object sender, EventArgs e)
        {
            Angle = 0;
            pbTerrain.Image = new Bitmap(InitialMap);
            OriginalMap = new Bitmap(InitialMap);
            FirstInference = true;
            pbRobot.Location = InitialPos;
            txtFront.Text = "0";
            txtLeft.Text = "0";
            txtRight.Text = "0";
            txtAngle.Text = "0,00";
        }

        // Moving the AGV
        private void MoveAGV()
        {
            double rad = ((Angle + 90) * Math.PI) / 180;
            int Offset = 0;
            int Inc = -4;

            Offset += Inc;
            int IncX = Convert.ToInt32(Offset * Math.Cos(rad));
            int IncY = Convert.ToInt32(Offset * Math.Sin(rad));

            // Leaving the track 
            if (cbTrajeto.Checked)
            {
                Graphics g = Graphics.FromImage(OriginalMap);
                Point p1 = new Point(pbRobot.Left - pbTerrain.Left + pbRobot.Width / 2, pbRobot.Top - pbTerrain.Top + pbRobot.Height / 2);
                Point p2 = new Point(p1.X + IncX, p1.Y + IncY);
                g.DrawLine(new Pen(new SolidBrush(Color.Blue)), p1, p2);
                g.DrawImage(OriginalMap, 0, 0);
                g.Dispose();
            }

            pbRobot.Top = pbRobot.Top + IncY;
            pbRobot.Left = pbRobot.Left + IncX;
        }

        // Starting and stopping the AGV's moviment a
        private void btnRun_Click(object sender, EventArgs e)
        {
            Button b = (sender as Button);

            if (b.Text == RunLabel)
            {
                b.Text = "&Stop";
                btnStep.Enabled = false;
                btnReset.Enabled = false;
                txtInterval.Enabled = false;
                cbLasers.Enabled = false;
                cbTrajeto.Enabled = false;
                pbRobot.Hide();
                StartMovement();
            }
            else
            {
                StopMovement();
                b.Text = RunLabel;
                btnReset.Enabled = true;
                btnStep.Enabled = true;
                txtInterval.Enabled = true;
                cbLasers.Enabled = true;
                cbTrajeto.Enabled = true;
                pbRobot.Show();
                pbTerrain.Image = CopyImage(OriginalMap);
                pbTerrain.Refresh();
            }
        }

        // One step of the AGV
        private void button3_Click(object sender, EventArgs e)
        {
            pbRobot.Hide();
            AGVStep();
            pbRobot.Show();
        }

        // Thread for the AGVs movement
        private void StartMovement()
        {
            thMovement = new Thread(new ThreadStart(MoveCycle));
            thMovement.IsBackground = true;
            thMovement.Priority = ThreadPriority.AboveNormal;
            thMovement.Start();
        }

        // Thread main cycle
        private void MoveCycle()
        {
            try
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    MethodInvoker mi = new MethodInvoker(AGVStep);
                    this.BeginInvoke(mi);
                    Thread.Sleep(Convert.ToInt32(txtInterval.Text));
                }
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        // One step of the AGV
        private void AGVStep()
        {
            if (FirstInference) GetMeasures();

            try
            {
                DoInference();
                MoveAGV();
                GetMeasures();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        // Stop background thread
        private void StopMovement()
        {
            if (thMovement != null)
            {
                thMovement.Interrupt();
                thMovement = null;
            }
        }

        // Show About dialog
        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutForm form = new AboutForm();

            form.ShowDialog();
        }
    }
}
