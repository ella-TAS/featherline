
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
            this.lbl_checkpoints = new System.Windows.Forms.Label();
            this.num_mutChangeCount = new System.Windows.Forms.NumericUpDown();
            this.lbl_mutChangeCount = new System.Windows.Forms.Label();
            this.num_mutationMagnitude = new System.Windows.Forms.NumericUpDown();
            this.lbl_mutationMagnitude = new System.Windows.Forms.Label();
            this.lbl_customSpinners = new System.Windows.Forms.Label();
            this.txt_customSpinners = new System.Windows.Forms.RichTextBox();
            this.cbx_avoidWalls = new System.Windows.Forms.CheckBox();
            this.cbx_enableSteepTurns = new System.Windows.Forms.CheckBox();
            this.lbl_manualHitboxes = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupTASSnippetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFlightOfInitialInputsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txt_checkpoints = new System.Windows.Forms.RichTextBox();
            this.txt_customHitboxes = new System.Windows.Forms.RichTextBox();
            this.cbx_timingTestFavDirectly = new System.Windows.Forms.CheckBox();
            this.cbx_frameBasedOnly = new System.Windows.Forms.CheckBox();
            this.num_gensPerTiming = new System.Windows.Forms.NumericUpDown();
            this.lbl_gensPerTiming = new System.Windows.Forms.Label();
            this.num_shuffleCount = new System.Windows.Forms.NumericUpDown();
            this.lbl_shuffleCount = new System.Windows.Forms.Label();
            this.num_maxThreadCount = new System.Windows.Forms.NumericUpDown();
            this.lbl_maxThreadCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.num_population)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_generations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_survivorCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_framecount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutChangeCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutationMagnitude)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_gensPerTiming)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_shuffleCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxThreadCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_beginAlg
            // 
            this.btn_beginAlg.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btn_beginAlg.Location = new System.Drawing.Point(14, 38);
            this.btn_beginAlg.Name = "btn_beginAlg";
            this.btn_beginAlg.Size = new System.Drawing.Size(123, 42);
            this.btn_beginAlg.TabIndex = 0;
            this.btn_beginAlg.Text = "Run Algorithm";
            this.btn_beginAlg.UseVisualStyleBackColor = false;
            this.btn_beginAlg.Click += new System.EventHandler(this.btn_beginAlg_Click);
            // 
            // btn_selectInfoFile
            // 
            this.btn_selectInfoFile.Location = new System.Drawing.Point(147, 38);
            this.btn_selectInfoFile.Name = "btn_selectInfoFile";
            this.btn_selectInfoFile.Size = new System.Drawing.Size(223, 42);
            this.btn_selectInfoFile.TabIndex = 2;
            this.btn_selectInfoFile.Text = "Select information Source File";
            this.btn_selectInfoFile.UseVisualStyleBackColor = true;
            this.btn_selectInfoFile.Click += new System.EventHandler(this.btn_selectInfoFile_Click);
            // 
            // txt_infoFile
            // 
            this.txt_infoFile.Location = new System.Drawing.Point(382, 47);
            this.txt_infoFile.Name = "txt_infoFile";
            this.txt_infoFile.ReadOnly = true;
            this.txt_infoFile.Size = new System.Drawing.Size(203, 23);
            this.txt_infoFile.TabIndex = 9;
            // 
            // txt_initSolution
            // 
            this.txt_initSolution.DetectUrls = false;
            this.txt_initSolution.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txt_initSolution.Location = new System.Drawing.Point(225, 103);
            this.txt_initSolution.Name = "txt_initSolution";
            this.txt_initSolution.Size = new System.Drawing.Size(136, 354);
            this.txt_initSolution.TabIndex = 17;
            this.txt_initSolution.Text = "";
            // 
            // lbl_initSolution
            // 
            this.lbl_initSolution.AutoSize = true;
            this.lbl_initSolution.Location = new System.Drawing.Point(229, 83);
            this.lbl_initSolution.Name = "lbl_initSolution";
            this.lbl_initSolution.Size = new System.Drawing.Size(129, 15);
            this.lbl_initSolution.TabIndex = 18;
            this.lbl_initSolution.Text = "(Optional) Initial Inputs";
            // 
            // lbl_population
            // 
            this.lbl_population.AutoSize = true;
            this.lbl_population.Location = new System.Drawing.Point(624, 37);
            this.lbl_population.Name = "lbl_population";
            this.lbl_population.Size = new System.Drawing.Size(65, 15);
            this.lbl_population.TabIndex = 19;
            this.lbl_population.Text = "Population";
            // 
            // num_population
            // 
            this.num_population.Location = new System.Drawing.Point(607, 55);
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
            this.num_population.Size = new System.Drawing.Size(99, 23);
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
            this.num_generations.Location = new System.Drawing.Point(607, 131);
            this.num_generations.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_generations.Name = "num_generations";
            this.num_generations.Size = new System.Drawing.Size(99, 23);
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
            this.lbl_generations.Location = new System.Drawing.Point(622, 113);
            this.lbl_generations.Name = "lbl_generations";
            this.lbl_generations.Size = new System.Drawing.Size(70, 15);
            this.lbl_generations.TabIndex = 21;
            this.lbl_generations.Text = "Generations";
            // 
            // num_survivorCount
            // 
            this.num_survivorCount.Location = new System.Drawing.Point(607, 207);
            this.num_survivorCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_survivorCount.Name = "num_survivorCount";
            this.num_survivorCount.Size = new System.Drawing.Size(99, 23);
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
            this.lbl_survivorCount.Location = new System.Drawing.Point(617, 189);
            this.lbl_survivorCount.Name = "lbl_survivorCount";
            this.lbl_survivorCount.Size = new System.Drawing.Size(79, 15);
            this.lbl_survivorCount.TabIndex = 23;
            this.lbl_survivorCount.Text = "Gen Survivors";
            // 
            // num_framecount
            // 
            this.num_framecount.Location = new System.Drawing.Point(607, 283);
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
            this.num_framecount.Size = new System.Drawing.Size(98, 23);
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
            this.lbl_framecount.Location = new System.Drawing.Point(608, 265);
            this.lbl_framecount.Name = "lbl_framecount";
            this.lbl_framecount.Size = new System.Drawing.Size(97, 15);
            this.lbl_framecount.TabIndex = 25;
            this.lbl_framecount.Text = "Max Framecount";
            // 
            // lbl_checkpoints
            // 
            this.lbl_checkpoints.AutoSize = true;
            this.lbl_checkpoints.Location = new System.Drawing.Point(16, 85);
            this.lbl_checkpoints.Name = "lbl_checkpoints";
            this.lbl_checkpoints.Size = new System.Drawing.Size(115, 15);
            this.lbl_checkpoints.TabIndex = 29;
            this.lbl_checkpoints.Text = "Feather Checkpoints";
            // 
            // num_mutChangeCount
            // 
            this.num_mutChangeCount.Location = new System.Drawing.Point(607, 435);
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
            this.num_mutChangeCount.Size = new System.Drawing.Size(99, 23);
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
            this.lbl_mutChangeCount.Location = new System.Drawing.Point(587, 417);
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
            this.num_mutationMagnitude.Location = new System.Drawing.Point(607, 359);
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
            this.num_mutationMagnitude.Size = new System.Drawing.Size(99, 23);
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
            this.lbl_mutationMagnitude.Location = new System.Drawing.Point(598, 341);
            this.lbl_mutationMagnitude.Name = "lbl_mutationMagnitude";
            this.lbl_mutationMagnitude.Size = new System.Drawing.Size(117, 15);
            this.lbl_mutationMagnitude.TabIndex = 36;
            this.lbl_mutationMagnitude.Text = "Mutation Magnitude";
            // 
            // lbl_customSpinners
            // 
            this.lbl_customSpinners.AutoSize = true;
            this.lbl_customSpinners.Location = new System.Drawing.Point(11, 467);
            this.lbl_customSpinners.Name = "lbl_customSpinners";
            this.lbl_customSpinners.Size = new System.Drawing.Size(180, 15);
            this.lbl_customSpinners.TabIndex = 44;
            this.lbl_customSpinners.Text = "Precise Custom Spinner Name(s)";
            // 
            // txt_customSpinners
            // 
            this.txt_customSpinners.Location = new System.Drawing.Point(12, 485);
            this.txt_customSpinners.Name = "txt_customSpinners";
            this.txt_customSpinners.Size = new System.Drawing.Size(184, 55);
            this.txt_customSpinners.TabIndex = 45;
            this.txt_customSpinners.Text = "";
            // 
            // cbx_avoidWalls
            // 
            this.cbx_avoidWalls.AutoSize = true;
            this.cbx_avoidWalls.Location = new System.Drawing.Point(207, 467);
            this.cbx_avoidWalls.Name = "cbx_avoidWalls";
            this.cbx_avoidWalls.Size = new System.Drawing.Size(132, 19);
            this.cbx_avoidWalls.TabIndex = 46;
            this.cbx_avoidWalls.Text = "Avoid Wall Collision";
            this.cbx_avoidWalls.UseVisualStyleBackColor = true;
            // 
            // cbx_enableSteepTurns
            // 
            this.cbx_enableSteepTurns.AutoSize = true;
            this.cbx_enableSteepTurns.Location = new System.Drawing.Point(207, 496);
            this.cbx_enableSteepTurns.Name = "cbx_enableSteepTurns";
            this.cbx_enableSteepTurns.Size = new System.Drawing.Size(153, 19);
            this.cbx_enableSteepTurns.TabIndex = 47;
            this.cbx_enableSteepTurns.Text = "Enable Slow Steep Turns";
            this.cbx_enableSteepTurns.UseVisualStyleBackColor = true;
            // 
            // lbl_manualHitboxes
            // 
            this.lbl_manualHitboxes.AutoSize = true;
            this.lbl_manualHitboxes.Location = new System.Drawing.Point(381, 85);
            this.lbl_manualHitboxes.Name = "lbl_manualHitboxes";
            this.lbl_manualHitboxes.Size = new System.Drawing.Size(171, 15);
            this.lbl_manualHitboxes.TabIndex = 49;
            this.lbl_manualHitboxes.Text = "Custom Killboxes and Colliders";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(729, 24);
            this.menuStrip1.TabIndex = 51;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoTemplateToolStripMenuItem,
            this.setupTASSnippetToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // infoTemplateToolStripMenuItem
            // 
            this.infoTemplateToolStripMenuItem.Name = "infoTemplateToolStripMenuItem";
            this.infoTemplateToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.infoTemplateToolStripMenuItem.Text = "Info Template";
            this.infoTemplateToolStripMenuItem.Click += new System.EventHandler(this.infoTemplateToolStripMenuItem_Click);
            // 
            // setupTASSnippetToolStripMenuItem
            // 
            this.setupTASSnippetToolStripMenuItem.Name = "setupTASSnippetToolStripMenuItem";
            this.setupTASSnippetToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.setupTASSnippetToolStripMenuItem.Text = "Setup TAS Snippet";
            this.setupTASSnippetToolStripMenuItem.Click += new System.EventHandler(this.setupTASSnippetToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logFlightOfInitialInputsToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // logFlightOfInitialInputsToolStripMenuItem
            // 
            this.logFlightOfInitialInputsToolStripMenuItem.Name = "logFlightOfInitialInputsToolStripMenuItem";
            this.logFlightOfInitialInputsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.logFlightOfInitialInputsToolStripMenuItem.Text = "Log flight of initial inputs";
            this.logFlightOfInitialInputsToolStripMenuItem.Click += new System.EventHandler(this.logFlightOfInitialInputsToolStripMenuItem_Click);
            // 
            // txt_checkpoints
            // 
            this.txt_checkpoints.Location = new System.Drawing.Point(12, 103);
            this.txt_checkpoints.Name = "txt_checkpoints";
            this.txt_checkpoints.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txt_checkpoints.Size = new System.Drawing.Size(199, 354);
            this.txt_checkpoints.TabIndex = 52;
            this.txt_checkpoints.Text = "";
            // 
            // txt_customHitboxes
            // 
            this.txt_customHitboxes.Location = new System.Drawing.Point(376, 103);
            this.txt_customHitboxes.Name = "txt_customHitboxes";
            this.txt_customHitboxes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txt_customHitboxes.Size = new System.Drawing.Size(208, 354);
            this.txt_customHitboxes.TabIndex = 53;
            this.txt_customHitboxes.Text = "";
            // 
            // cbx_timingTestFavDirectly
            // 
            this.cbx_timingTestFavDirectly.AutoSize = true;
            this.cbx_timingTestFavDirectly.Location = new System.Drawing.Point(365, 467);
            this.cbx_timingTestFavDirectly.Name = "cbx_timingTestFavDirectly";
            this.cbx_timingTestFavDirectly.Size = new System.Drawing.Size(221, 19);
            this.cbx_timingTestFavDirectly.TabIndex = 54;
            this.cbx_timingTestFavDirectly.Text = "Test Timings On Initial Inputs Directly";
            this.cbx_timingTestFavDirectly.UseVisualStyleBackColor = true;
            // 
            // cbx_frameBasedOnly
            // 
            this.cbx_frameBasedOnly.AutoSize = true;
            this.cbx_frameBasedOnly.Location = new System.Drawing.Point(207, 525);
            this.cbx_frameBasedOnly.Name = "cbx_frameBasedOnly";
            this.cbx_frameBasedOnly.Size = new System.Drawing.Size(200, 19);
            this.cbx_frameBasedOnly.TabIndex = 55;
            this.cbx_frameBasedOnly.Text = "Use Frame Based Algorithm Only";
            this.cbx_frameBasedOnly.UseVisualStyleBackColor = true;
            this.cbx_frameBasedOnly.CheckedChanged += new System.EventHandler(this.cbx_frameBasedOnly_CheckedChanged);
            // 
            // num_gensPerTiming
            // 
            this.num_gensPerTiming.Location = new System.Drawing.Point(607, 505);
            this.num_gensPerTiming.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_gensPerTiming.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_gensPerTiming.Name = "num_gensPerTiming";
            this.num_gensPerTiming.Size = new System.Drawing.Size(99, 23);
            this.num_gensPerTiming.TabIndex = 56;
            this.num_gensPerTiming.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lbl_gensPerTiming
            // 
            this.lbl_gensPerTiming.AutoSize = true;
            this.lbl_gensPerTiming.Location = new System.Drawing.Point(592, 487);
            this.lbl_gensPerTiming.Name = "lbl_gensPerTiming";
            this.lbl_gensPerTiming.Size = new System.Drawing.Size(129, 15);
            this.lbl_gensPerTiming.TabIndex = 57;
            this.lbl_gensPerTiming.Text = "Gens Per Tested Timing";
            // 
            // num_shuffleCount
            // 
            this.num_shuffleCount.Location = new System.Drawing.Point(432, 517);
            this.num_shuffleCount.Name = "num_shuffleCount";
            this.num_shuffleCount.Size = new System.Drawing.Size(120, 23);
            this.num_shuffleCount.TabIndex = 58;
            this.num_shuffleCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lbl_shuffleCount
            // 
            this.lbl_shuffleCount.AutoSize = true;
            this.lbl_shuffleCount.Location = new System.Drawing.Point(432, 499);
            this.lbl_shuffleCount.Name = "lbl_shuffleCount";
            this.lbl_shuffleCount.Size = new System.Drawing.Size(120, 15);
            this.lbl_shuffleCount.TabIndex = 59;
            this.lbl_shuffleCount.Text = "Timing Shuffle Count";
            // 
            // num_maxThreadCount
            // 
            this.num_maxThreadCount.DecimalPlaces = 1;
            this.num_maxThreadCount.Increment = new decimal(new int[] {
            11,
            0,
            0,
            65536});
            this.num_maxThreadCount.Location = new System.Drawing.Point(509, 590);
            this.num_maxThreadCount.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_maxThreadCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.num_maxThreadCount.Name = "num_maxThreadCount";
            this.num_maxThreadCount.Size = new System.Drawing.Size(120, 23);
            this.num_maxThreadCount.TabIndex = 60;
            this.num_maxThreadCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_maxThreadCount.ValueChanged += new System.EventHandler(this.num_maxThreadCount_ValueChanged);
            // 
            // lbl_maxThreadCount
            // 
            this.lbl_maxThreadCount.AutoSize = true;
            this.lbl_maxThreadCount.Location = new System.Drawing.Point(517, 569);
            this.lbl_maxThreadCount.Name = "lbl_maxThreadCount";
            this.lbl_maxThreadCount.Size = new System.Drawing.Size(105, 15);
            this.lbl_maxThreadCount.TabIndex = 61;
            this.lbl_maxThreadCount.Text = "Max Thread Count";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(729, 625);
            this.Controls.Add(this.lbl_maxThreadCount);
            this.Controls.Add(this.num_maxThreadCount);
            this.Controls.Add(this.lbl_shuffleCount);
            this.Controls.Add(this.num_shuffleCount);
            this.Controls.Add(this.lbl_gensPerTiming);
            this.Controls.Add(this.num_gensPerTiming);
            this.Controls.Add(this.cbx_frameBasedOnly);
            this.Controls.Add(this.cbx_timingTestFavDirectly);
            this.Controls.Add(this.txt_customHitboxes);
            this.Controls.Add(this.txt_checkpoints);
            this.Controls.Add(this.lbl_manualHitboxes);
            this.Controls.Add(this.cbx_enableSteepTurns);
            this.Controls.Add(this.cbx_avoidWalls);
            this.Controls.Add(this.txt_customSpinners);
            this.Controls.Add(this.lbl_customSpinners);
            this.Controls.Add(this.num_mutChangeCount);
            this.Controls.Add(this.lbl_mutChangeCount);
            this.Controls.Add(this.num_mutationMagnitude);
            this.Controls.Add(this.lbl_mutationMagnitude);
            this.Controls.Add(this.lbl_checkpoints);
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
            this.Controls.Add(this.txt_infoFile);
            this.Controls.Add(this.btn_selectInfoFile);
            this.Controls.Add(this.btn_beginAlg);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Featherline";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.num_population)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_generations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_survivorCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_framecount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutChangeCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_mutationMagnitude)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_gensPerTiming)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_shuffleCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxThreadCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_beginAlg;
        private System.Windows.Forms.Button btn_selectInfoFile;
        private System.Windows.Forms.TextBox txt_infoFile;
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
        private System.Windows.Forms.Label lbl_checkpoints;
        private System.Windows.Forms.NumericUpDown num_mutChangeCount;
        private System.Windows.Forms.Label lbl_mutChangeCount;
        private System.Windows.Forms.NumericUpDown num_mutationMagnitude;
        private System.Windows.Forms.Label lbl_mutationMagnitude;
        private System.Windows.Forms.Label lbl_customSpinners;
        private System.Windows.Forms.RichTextBox txt_customSpinners;
        private System.Windows.Forms.CheckBox cbx_avoidWalls;
        private System.Windows.Forms.CheckBox cbx_enableSteepTurns;
        private System.Windows.Forms.Label lbl_manualHitboxes;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txt_checkpoints;
        private System.Windows.Forms.RichTextBox txt_customHitboxes;
        private System.Windows.Forms.CheckBox cbx_timingTestFavDirectly;
        private System.Windows.Forms.CheckBox cbx_frameBasedOnly;
        private System.Windows.Forms.NumericUpDown num_gensPerTiming;
        private System.Windows.Forms.Label lbl_gensPerTiming;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupTASSnippetToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown num_shuffleCount;
        private System.Windows.Forms.Label lbl_shuffleCount;
        private System.Windows.Forms.NumericUpDown num_maxThreadCount;
        private System.Windows.Forms.Label lbl_maxThreadCount;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logFlightOfInitialInputsToolStripMenuItem;
    }
}

