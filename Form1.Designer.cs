
namespace Featherline
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            SaveSettings();
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_beginAlg = new System.Windows.Forms.Button();
            this.btn_selectInfoFile = new System.Windows.Forms.Button();
            this.txt_infoFile = new System.Windows.Forms.TextBox();
            this.btn_infoExtraction = new System.Windows.Forms.Button();
            this.txt_initSolution = new System.Windows.Forms.RichTextBox();
            this.lbl_initSolution = new System.Windows.Forms.Label();
            this.lbl_population = new System.Windows.Forms.Label();
            this.num_population = new System.Windows.Forms.NumericUpDown();
            this.num_generations = new System.Windows.Forms.NumericUpDown();
            this.lbl_generations = new System.Windows.Forms.Label();
            this.num_survivorCount = new System.Windows.Forms.NumericUpDown();
            this.lbl_survivorCount = new System.Windows.Forms.Label();
            this.num_framecount = new System.Windows.Forms.NumericUpDown();
            this.lbl_framecount = new System.Windows.Forms.Label();
            this.grd_checkpoints = new System.Windows.Forms.DataGridView();
            this.cpL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cpU = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cpR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cpD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbl_checkpoints = new System.Windows.Forms.Label();
            this.num_simplificationProbability = new System.Windows.Forms.NumericUpDown();
            this.lbl_simplificationProbability = new System.Windows.Forms.Label();
            this.num_mutationProbability = new System.Windows.Forms.NumericUpDown();
            this.lbl_mutationProbability = new System.Windows.Forms.Label();
            this.num_crossoverProbability = new System.Windows.Forms.NumericUpDown();
            this.lbl_crossoverProb = new System.Windows.Forms.Label();
            this.num_mutChangeCount = new System.Windows.Forms.NumericUpDown();
            this.lbl_mutChangeCount = new System.Windows.Forms.Label();
            this.num_mutationMagnitude = new System.Windows.Forms.NumericUpDown();
            this.lbl_mutationMagnitude = new System.Windows.Forms.Label();
            this.cbx_inputLinesMode = new System.Windows.Forms.CheckBox();
            this.num_inputLineCount = new System.Windows.Forms.NumericUpDown();
            this.lbl_inputLineCount = new System.Windows.Forms.Label();
            this.lbl_customSpinners = new System.Windows.Forms.Label();
            this.txt_customSpinners = new System.Windows.Forms.RichTextBox();
            this.cbx_avoidWalls = new System.Windows.Forms.CheckBox();
            this.cbx_enableSteepTurns = new System.Windows.Forms.CheckBox();
            this.lbl_manualHitboxes = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grd_manualHitboxes = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.num_population)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_generations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_survivorCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_framecount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_checkpoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_simplificationProbability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutationProbability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_crossoverProbability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutChangeCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutationMagnitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_inputLineCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_manualHitboxes)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_beginAlg
            // 
            this.btn_beginAlg.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btn_beginAlg.Location = new System.Drawing.Point(12, 12);
            this.btn_beginAlg.Name = "btn_beginAlg";
            this.btn_beginAlg.Size = new System.Drawing.Size(123, 63);
            this.btn_beginAlg.TabIndex = 0;
            this.btn_beginAlg.Text = "Run Genetic Algorithm";
            this.btn_beginAlg.UseVisualStyleBackColor = false;
            this.btn_beginAlg.Click += new System.EventHandler(this.btn_beginAlg_Click);
            // 
            // btn_selectInfoFile
            // 
            this.btn_selectInfoFile.Location = new System.Drawing.Point(360, 12);
            this.btn_selectInfoFile.Name = "btn_selectInfoFile";
            this.btn_selectInfoFile.Size = new System.Drawing.Size(223, 39);
            this.btn_selectInfoFile.TabIndex = 2;
            this.btn_selectInfoFile.Text = "Select information Source File";
            this.btn_selectInfoFile.UseVisualStyleBackColor = true;
            this.btn_selectInfoFile.Click += new System.EventHandler(this.btn_selectInfoFile_Click);
            // 
            // txt_infoFile
            // 
            this.txt_infoFile.Location = new System.Drawing.Point(595, 19);
            this.txt_infoFile.Name = "txt_infoFile";
            this.txt_infoFile.ReadOnly = true;
            this.txt_infoFile.Size = new System.Drawing.Size(197, 23);
            this.txt_infoFile.TabIndex = 9;
            // 
            // btn_infoExtraction
            // 
            this.btn_infoExtraction.Location = new System.Drawing.Point(145, 12);
            this.btn_infoExtraction.Name = "btn_infoExtraction";
            this.btn_infoExtraction.Size = new System.Drawing.Size(204, 39);
            this.btn_infoExtraction.TabIndex = 14;
            this.btn_infoExtraction.Text = "Prepare Info Extraction";
            this.btn_infoExtraction.UseVisualStyleBackColor = true;
            this.btn_infoExtraction.Click += new System.EventHandler(this.btn_infoExtraction_Click);
            // 
            // txt_initSolution
            // 
            this.txt_initSolution.Location = new System.Drawing.Point(401, 74);
            this.txt_initSolution.Name = "txt_initSolution";
            this.txt_initSolution.Size = new System.Drawing.Size(134, 354);
            this.txt_initSolution.TabIndex = 17;
            this.txt_initSolution.Text = "";
            // 
            // lbl_initSolution
            // 
            this.lbl_initSolution.AutoSize = true;
            this.lbl_initSolution.Location = new System.Drawing.Point(404, 54);
            this.lbl_initSolution.Name = "lbl_initSolution";
            this.lbl_initSolution.Size = new System.Drawing.Size(129, 15);
            this.lbl_initSolution.TabIndex = 18;
            this.lbl_initSolution.Text = "(Optional) Initial Inputs";
            // 
            // lbl_population
            // 
            this.lbl_population.AutoSize = true;
            this.lbl_population.Location = new System.Drawing.Point(40, 88);
            this.lbl_population.Name = "lbl_population";
            this.lbl_population.Size = new System.Drawing.Size(65, 15);
            this.lbl_population.TabIndex = 19;
            this.lbl_population.Text = "Population";
            // 
            // num_population
            // 
            this.num_population.Location = new System.Drawing.Point(24, 106);
            this.num_population.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_population.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.num_population.Name = "num_population";
            this.num_population.Size = new System.Drawing.Size(98, 23);
            this.num_population.TabIndex = 20;
            this.num_population.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_population.ValueChanged += new System.EventHandler(this.num_population_ValueChanged);
            // 
            // num_generations
            // 
            this.num_generations.Location = new System.Drawing.Point(24, 193);
            this.num_generations.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_generations.Name = "num_generations";
            this.num_generations.Size = new System.Drawing.Size(98, 23);
            this.num_generations.TabIndex = 22;
            this.num_generations.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // lbl_generations
            // 
            this.lbl_generations.AutoSize = true;
            this.lbl_generations.Location = new System.Drawing.Point(37, 175);
            this.lbl_generations.Name = "lbl_generations";
            this.lbl_generations.Size = new System.Drawing.Size(70, 15);
            this.lbl_generations.TabIndex = 21;
            this.lbl_generations.Text = "Generations";
            // 
            // num_survivorCount
            // 
            this.num_survivorCount.Location = new System.Drawing.Point(25, 282);
            this.num_survivorCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_survivorCount.Name = "num_survivorCount";
            this.num_survivorCount.Size = new System.Drawing.Size(97, 23);
            this.num_survivorCount.TabIndex = 24;
            this.num_survivorCount.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // lbl_survivorCount
            // 
            this.lbl_survivorCount.AutoSize = true;
            this.lbl_survivorCount.Location = new System.Drawing.Point(34, 264);
            this.lbl_survivorCount.Name = "lbl_survivorCount";
            this.lbl_survivorCount.Size = new System.Drawing.Size(79, 15);
            this.lbl_survivorCount.TabIndex = 23;
            this.lbl_survivorCount.Text = "Gen Survivors";
            // 
            // num_framecount
            // 
            this.num_framecount.Location = new System.Drawing.Point(24, 373);
            this.num_framecount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_framecount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_framecount.Name = "num_framecount";
            this.num_framecount.Size = new System.Drawing.Size(97, 23);
            this.num_framecount.TabIndex = 26;
            this.num_framecount.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // lbl_framecount
            // 
            this.lbl_framecount.AccessibleDescription = "asdfasdfasdf";
            this.lbl_framecount.AutoSize = true;
            this.lbl_framecount.Location = new System.Drawing.Point(24, 355);
            this.lbl_framecount.Name = "lbl_framecount";
            this.lbl_framecount.Size = new System.Drawing.Size(97, 15);
            this.lbl_framecount.TabIndex = 25;
            this.lbl_framecount.Text = "Max Framecount";
            // 
            // grd_checkpoints
            // 
            this.grd_checkpoints.AllowUserToResizeColumns = false;
            this.grd_checkpoints.AllowUserToResizeRows = false;
            this.grd_checkpoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grd_checkpoints.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cpL,
            this.cpU,
            this.cpR,
            this.cpD});
            this.grd_checkpoints.Location = new System.Drawing.Point(145, 75);
            this.grd_checkpoints.MultiSelect = false;
            this.grd_checkpoints.Name = "grd_checkpoints";
            this.grd_checkpoints.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.grd_checkpoints.RowTemplate.Height = 25;
            this.grd_checkpoints.Size = new System.Drawing.Size(243, 354);
            this.grd_checkpoints.TabIndex = 27;
            // 
            // cpL
            // 
            this.cpL.Frozen = true;
            this.cpL.HeaderText = "L Edge";
            this.cpL.Name = "cpL";
            this.cpL.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cpL.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cpL.Width = 50;
            // 
            // cpU
            // 
            this.cpU.Frozen = true;
            this.cpU.HeaderText = "U Edge";
            this.cpU.Name = "cpU";
            this.cpU.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cpU.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cpU.Width = 50;
            // 
            // cpR
            // 
            this.cpR.Frozen = true;
            this.cpR.HeaderText = "R Edge";
            this.cpR.Name = "cpR";
            this.cpR.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cpR.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cpR.Width = 50;
            // 
            // cpD
            // 
            this.cpD.Frozen = true;
            this.cpD.HeaderText = "D Edge";
            this.cpD.Name = "cpD";
            this.cpD.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cpD.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cpD.Width = 50;
            // 
            // lbl_checkpoints
            // 
            this.lbl_checkpoints.AutoSize = true;
            this.lbl_checkpoints.Location = new System.Drawing.Point(145, 56);
            this.lbl_checkpoints.Name = "lbl_checkpoints";
            this.lbl_checkpoints.Size = new System.Drawing.Size(115, 15);
            this.lbl_checkpoints.TabIndex = 29;
            this.lbl_checkpoints.Text = "Feather Checkpoints";
            // 
            // num_simplificationProbability
            // 
            this.num_simplificationProbability.Location = new System.Drawing.Point(351, 460);
            this.num_simplificationProbability.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_simplificationProbability.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_simplificationProbability.Name = "num_simplificationProbability";
            this.num_simplificationProbability.Size = new System.Drawing.Size(97, 23);
            this.num_simplificationProbability.TabIndex = 35;
            this.num_simplificationProbability.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbl_simplificationProbability
            // 
            this.lbl_simplificationProbability.AutoSize = true;
            this.lbl_simplificationProbability.Location = new System.Drawing.Point(329, 442);
            this.lbl_simplificationProbability.Name = "lbl_simplificationProbability";
            this.lbl_simplificationProbability.Size = new System.Drawing.Size(140, 15);
            this.lbl_simplificationProbability.TabIndex = 34;
            this.lbl_simplificationProbability.Text = "Simplification Probability";
            // 
            // num_mutationProbability
            // 
            this.num_mutationProbability.Location = new System.Drawing.Point(24, 460);
            this.num_mutationProbability.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_mutationProbability.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_mutationProbability.Name = "num_mutationProbability";
            this.num_mutationProbability.Size = new System.Drawing.Size(98, 23);
            this.num_mutationProbability.TabIndex = 33;
            this.num_mutationProbability.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lbl_mutationProbability
            // 
            this.lbl_mutationProbability.AutoSize = true;
            this.lbl_mutationProbability.Location = new System.Drawing.Point(15, 442);
            this.lbl_mutationProbability.Name = "lbl_mutationProbability";
            this.lbl_mutationProbability.Size = new System.Drawing.Size(116, 15);
            this.lbl_mutationProbability.TabIndex = 32;
            this.lbl_mutationProbability.Text = "Mutation Probability";
            // 
            // num_crossoverProbability
            // 
            this.num_crossoverProbability.Location = new System.Drawing.Point(184, 460);
            this.num_crossoverProbability.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_crossoverProbability.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_crossoverProbability.Name = "num_crossoverProbability";
            this.num_crossoverProbability.Size = new System.Drawing.Size(98, 23);
            this.num_crossoverProbability.TabIndex = 31;
            this.num_crossoverProbability.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbl_crossoverProb
            // 
            this.lbl_crossoverProb.AutoSize = true;
            this.lbl_crossoverProb.Location = new System.Drawing.Point(173, 442);
            this.lbl_crossoverProb.Name = "lbl_crossoverProb";
            this.lbl_crossoverProb.Size = new System.Drawing.Size(119, 15);
            this.lbl_crossoverProb.TabIndex = 30;
            this.lbl_crossoverProb.Text = "Crossover Probability";
            // 
            // num_mutChangeCount
            // 
            this.num_mutChangeCount.Location = new System.Drawing.Point(676, 460);
            this.num_mutChangeCount.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.num_mutChangeCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_mutChangeCount.Name = "num_mutChangeCount";
            this.num_mutChangeCount.Size = new System.Drawing.Size(97, 23);
            this.num_mutChangeCount.TabIndex = 39;
            this.num_mutChangeCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lbl_mutChangeCount
            // 
            this.lbl_mutChangeCount.AutoSize = true;
            this.lbl_mutChangeCount.Location = new System.Drawing.Point(655, 442);
            this.lbl_mutChangeCount.Name = "lbl_mutChangeCount";
            this.lbl_mutChangeCount.Size = new System.Drawing.Size(138, 15);
            this.lbl_mutChangeCount.TabIndex = 38;
            this.lbl_mutChangeCount.Text = "Max Mut. Change Count";
            // 
            // num_mutationMagnitude
            // 
            this.num_mutationMagnitude.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.num_mutationMagnitude.Location = new System.Drawing.Point(515, 460);
            this.num_mutationMagnitude.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.num_mutationMagnitude.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_mutationMagnitude.Name = "num_mutationMagnitude";
            this.num_mutationMagnitude.Size = new System.Drawing.Size(98, 23);
            this.num_mutationMagnitude.TabIndex = 37;
            this.num_mutationMagnitude.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lbl_mutationMagnitude
            // 
            this.lbl_mutationMagnitude.AutoSize = true;
            this.lbl_mutationMagnitude.Location = new System.Drawing.Point(506, 442);
            this.lbl_mutationMagnitude.Name = "lbl_mutationMagnitude";
            this.lbl_mutationMagnitude.Size = new System.Drawing.Size(117, 15);
            this.lbl_mutationMagnitude.TabIndex = 36;
            this.lbl_mutationMagnitude.Text = "Mutation Magnitude";
            // 
            // cbx_inputLinesMode
            // 
            this.cbx_inputLinesMode.AutoSize = true;
            this.cbx_inputLinesMode.Location = new System.Drawing.Point(544, 510);
            this.cbx_inputLinesMode.Name = "cbx_inputLinesMode";
            this.cbx_inputLinesMode.Size = new System.Drawing.Size(193, 19);
            this.cbx_inputLinesMode.TabIndex = 41;
            this.cbx_inputLinesMode.Text = "Limited Input Lines mode (WIP)";
            this.cbx_inputLinesMode.UseVisualStyleBackColor = true;
            this.cbx_inputLinesMode.CheckedChanged += new System.EventHandler(this.cbx_inputLinesMode_CheckedChanged);
            // 
            // num_inputLineCount
            // 
            this.num_inputLineCount.Location = new System.Drawing.Point(544, 554);
            this.num_inputLineCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_inputLineCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_inputLineCount.Name = "num_inputLineCount";
            this.num_inputLineCount.Size = new System.Drawing.Size(97, 23);
            this.num_inputLineCount.TabIndex = 42;
            this.num_inputLineCount.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // lbl_inputLineCount
            // 
            this.lbl_inputLineCount.AutoSize = true;
            this.lbl_inputLineCount.Location = new System.Drawing.Point(544, 536);
            this.lbl_inputLineCount.Name = "lbl_inputLineCount";
            this.lbl_inputLineCount.Size = new System.Drawing.Size(96, 15);
            this.lbl_inputLineCount.TabIndex = 43;
            this.lbl_inputLineCount.Text = "Input Line Count";
            // 
            // lbl_customSpinners
            // 
            this.lbl_customSpinners.AutoSize = true;
            this.lbl_customSpinners.Location = new System.Drawing.Point(24, 497);
            this.lbl_customSpinners.Name = "lbl_customSpinners";
            this.lbl_customSpinners.Size = new System.Drawing.Size(293, 15);
            this.lbl_customSpinners.TabIndex = 44;
            this.lbl_customSpinners.Text = "Precise Custom Spinner Name(s) (likely not necessary)";
            // 
            // txt_customSpinners
            // 
            this.txt_customSpinners.Location = new System.Drawing.Point(25, 515);
            this.txt_customSpinners.Name = "txt_customSpinners";
            this.txt_customSpinners.Size = new System.Drawing.Size(275, 67);
            this.txt_customSpinners.TabIndex = 45;
            this.txt_customSpinners.Text = "";
            // 
            // cbx_avoidWalls
            // 
            this.cbx_avoidWalls.AutoSize = true;
            this.cbx_avoidWalls.Location = new System.Drawing.Point(326, 522);
            this.cbx_avoidWalls.Name = "cbx_avoidWalls";
            this.cbx_avoidWalls.Size = new System.Drawing.Size(132, 19);
            this.cbx_avoidWalls.TabIndex = 46;
            this.cbx_avoidWalls.Text = "Avoid Wall Collision";
            this.cbx_avoidWalls.UseVisualStyleBackColor = true;
            // 
            // cbx_enableSteepTurns
            // 
            this.cbx_enableSteepTurns.AutoSize = true;
            this.cbx_enableSteepTurns.Location = new System.Drawing.Point(326, 558);
            this.cbx_enableSteepTurns.Name = "cbx_enableSteepTurns";
            this.cbx_enableSteepTurns.Size = new System.Drawing.Size(153, 19);
            this.cbx_enableSteepTurns.TabIndex = 47;
            this.cbx_enableSteepTurns.Text = "Enable Slow Steep Turns";
            this.cbx_enableSteepTurns.UseVisualStyleBackColor = true;
            // 
            // lbl_manualHitboxes
            // 
            this.lbl_manualHitboxes.AutoSize = true;
            this.lbl_manualHitboxes.Location = new System.Drawing.Point(549, 56);
            this.lbl_manualHitboxes.Name = "lbl_manualHitboxes";
            this.lbl_manualHitboxes.Size = new System.Drawing.Size(171, 15);
            this.lbl_manualHitboxes.TabIndex = 49;
            this.lbl_manualHitboxes.Text = "Custom Killboxes and Colliders";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Frozen = true;
            this.dataGridViewTextBoxColumn1.HeaderText = "L Edge";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 50;
            // 
            // grd_manualHitboxes
            // 
            this.grd_manualHitboxes.AllowUserToResizeColumns = false;
            this.grd_manualHitboxes.AllowUserToResizeRows = false;
            this.grd_manualHitboxes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grd_manualHitboxes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.Column1});
            this.grd_manualHitboxes.Location = new System.Drawing.Point(549, 74);
            this.grd_manualHitboxes.MultiSelect = false;
            this.grd_manualHitboxes.Name = "grd_manualHitboxes";
            this.grd_manualHitboxes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.grd_manualHitboxes.RowTemplate.Height = 25;
            this.grd_manualHitboxes.Size = new System.Drawing.Size(307, 354);
            this.grd_manualHitboxes.TabIndex = 50;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.Frozen = true;
            this.dataGridViewTextBoxColumn2.HeaderText = "L Edge";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 50;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.Frozen = true;
            this.dataGridViewTextBoxColumn3.HeaderText = "U Edge";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn3.Width = 50;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.Frozen = true;
            this.dataGridViewTextBoxColumn4.HeaderText = "R Edge";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn4.Width = 50;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.Frozen = true;
            this.dataGridViewTextBoxColumn5.HeaderText = "D Edge";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn5.Width = 50;
            // 
            // Column1
            // 
            this.Column1.Frozen = true;
            this.Column1.HeaderText = "Is Collider";
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Column1.Width = 65;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(869, 596);
            this.Controls.Add(this.grd_manualHitboxes);
            this.Controls.Add(this.lbl_manualHitboxes);
            this.Controls.Add(this.cbx_enableSteepTurns);
            this.Controls.Add(this.cbx_avoidWalls);
            this.Controls.Add(this.txt_customSpinners);
            this.Controls.Add(this.lbl_customSpinners);
            this.Controls.Add(this.lbl_inputLineCount);
            this.Controls.Add(this.num_inputLineCount);
            this.Controls.Add(this.cbx_inputLinesMode);
            this.Controls.Add(this.num_mutChangeCount);
            this.Controls.Add(this.lbl_mutChangeCount);
            this.Controls.Add(this.num_mutationMagnitude);
            this.Controls.Add(this.lbl_mutationMagnitude);
            this.Controls.Add(this.num_simplificationProbability);
            this.Controls.Add(this.lbl_simplificationProbability);
            this.Controls.Add(this.num_mutationProbability);
            this.Controls.Add(this.lbl_mutationProbability);
            this.Controls.Add(this.num_crossoverProbability);
            this.Controls.Add(this.lbl_crossoverProb);
            this.Controls.Add(this.lbl_checkpoints);
            this.Controls.Add(this.grd_checkpoints);
            this.Controls.Add(this.num_framecount);
            this.Controls.Add(this.lbl_framecount);
            this.Controls.Add(this.num_survivorCount);
            this.Controls.Add(this.lbl_survivorCount);
            this.Controls.Add(this.num_generations);
            this.Controls.Add(this.lbl_generations);
            this.Controls.Add(this.num_population);
            this.Controls.Add(this.lbl_population);
            this.Controls.Add(this.lbl_initSolution);
            this.Controls.Add(this.txt_initSolution);
            this.Controls.Add(this.btn_infoExtraction);
            this.Controls.Add(this.txt_infoFile);
            this.Controls.Add(this.btn_selectInfoFile);
            this.Controls.Add(this.btn_beginAlg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Featherline";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.num_population)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_generations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_survivorCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_framecount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_checkpoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_simplificationProbability)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutationProbability)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_crossoverProbability)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutChangeCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutationMagnitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_inputLineCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_manualHitboxes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_beginAlg;
        private System.Windows.Forms.Button btn_selectInfoFile;
        private System.Windows.Forms.TextBox txt_infoFile;
        private System.Windows.Forms.Button btn_infoExtraction;
        private System.Windows.Forms.RichTextBox txt_initSolution;
        private System.Windows.Forms.Label lbl_initSolution;
        private System.Windows.Forms.Label lbl_population;
        private System.Windows.Forms.NumericUpDown num_population;
        private System.Windows.Forms.NumericUpDown num_generations;
        private System.Windows.Forms.Label lbl_generations;
        private System.Windows.Forms.NumericUpDown num_survivorCount;
        private System.Windows.Forms.Label lbl_survivorCount;
        private System.Windows.Forms.NumericUpDown num_framecount;
        private System.Windows.Forms.Label lbl_framecount;
        private System.Windows.Forms.DataGridView grd_checkpoints;
        private System.Windows.Forms.Label lbl_checkpoints;
        private System.Windows.Forms.NumericUpDown num_simplificationProbability;
        private System.Windows.Forms.Label lbl_simplificationProbability;
        private System.Windows.Forms.NumericUpDown num_mutationProbability;
        private System.Windows.Forms.Label lbl_mutationProbability;
        private System.Windows.Forms.NumericUpDown num_crossoverProbability;
        private System.Windows.Forms.Label lbl_crossoverProb;
        private System.Windows.Forms.NumericUpDown num_mutChangeCount;
        private System.Windows.Forms.Label lbl_mutChangeCount;
        private System.Windows.Forms.NumericUpDown num_mutationMagnitude;
        private System.Windows.Forms.Label lbl_mutationMagnitude;
        private System.Windows.Forms.CheckBox cbx_inputLinesMode;
        private System.Windows.Forms.NumericUpDown num_inputLineCount;
        private System.Windows.Forms.Label lbl_inputLineCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn cpL;
        private System.Windows.Forms.DataGridViewTextBoxColumn cpU;
        private System.Windows.Forms.DataGridViewTextBoxColumn cpR;
        private System.Windows.Forms.DataGridViewTextBoxColumn cpD;
        private System.Windows.Forms.Label lbl_customSpinners;
        private System.Windows.Forms.RichTextBox txt_customSpinners;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox cbx_avoidWalls;
        private System.Windows.Forms.CheckBox cbx_enableSteepTurns;
        private System.Windows.Forms.Label lbl_manualHitboxes;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridView grd_manualHitboxes;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}

