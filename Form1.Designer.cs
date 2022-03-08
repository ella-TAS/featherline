
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
            this.num_generations = new System.Windows.Forms.NumericUpDown();
            this.lbl_generations = new System.Windows.Forms.Label();
            this.num_framecount = new System.Windows.Forms.NumericUpDown();
            this.lbl_framecount = new System.Windows.Forms.Label();
            this.lbl_checkpoints = new System.Windows.Forms.Label();
            this.lbl_manualHitboxes = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoSetInfoTemplate = new System.Windows.Forms.ToolStripMenuItem();
            this.infoTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupTASSnippetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.returnToOldTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFlightOfInitialInputsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geneticAlgorithmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.populationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generationSurvivorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mutationMagnitudeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maxMutationCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.computationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dontComputeHazardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dontComputeCollisionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.algorithmModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frameGenesOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disallowWallCollisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulationThreadCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logAlgorithmResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txt_checkpoints = new System.Windows.Forms.RichTextBox();
            this.txt_customHitboxes = new System.Windows.Forms.RichTextBox();
            this.cbx_timingTestFavDirectly = new System.Windows.Forms.CheckBox();
            this.num_gensPerTiming = new System.Windows.Forms.NumericUpDown();
            this.lbl_gensPerTiming = new System.Windows.Forms.Label();
            this.num_shuffleCount = new System.Windows.Forms.NumericUpDown();
            this.lbl_shuffleCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.num_generations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_framecount)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_gensPerTiming)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_shuffleCount)).BeginInit();
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
            // num_generations
            // 
            this.num_generations.Location = new System.Drawing.Point(608, 59);
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
            this.lbl_generations.Location = new System.Drawing.Point(621, 41);
            this.lbl_generations.Name = "lbl_generations";
            this.lbl_generations.Size = new System.Drawing.Size(70, 15);
            this.lbl_generations.TabIndex = 21;
            this.lbl_generations.Text = "Generations";
            // 
            // num_framecount
            // 
            this.num_framecount.Location = new System.Drawing.Point(608, 153);
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
            this.num_framecount.Size = new System.Drawing.Size(99, 23);
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
            this.lbl_framecount.Location = new System.Drawing.Point(609, 135);
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
            this.debugToolStripMenuItem,
            this.extraToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(731, 24);
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
            this.AutoSetInfoTemplate,
            this.infoTemplateToolStripMenuItem,
            this.setupTASSnippetToolStripMenuItem,
            this.returnToOldTemplateToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.copyToolStripMenuItem.Text = "Setup";
            // 
            // AutoSetInfoTemplate
            // 
            this.AutoSetInfoTemplate.Name = "AutoSetInfoTemplate";
            this.AutoSetInfoTemplate.Size = new System.Drawing.Size(216, 22);
            this.AutoSetInfoTemplate.Text = "Auto Set Info Template";
            this.AutoSetInfoTemplate.Click += new System.EventHandler(this.AutoSetInfoTemplate_Click);
            // 
            // infoTemplateToolStripMenuItem
            // 
            this.infoTemplateToolStripMenuItem.Name = "infoTemplateToolStripMenuItem";
            this.infoTemplateToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.infoTemplateToolStripMenuItem.Text = "Copy Info Template";
            this.infoTemplateToolStripMenuItem.Click += new System.EventHandler(this.infoTemplateToolStripMenuItem_Click);
            // 
            // setupTASSnippetToolStripMenuItem
            // 
            this.setupTASSnippetToolStripMenuItem.Name = "setupTASSnippetToolStripMenuItem";
            this.setupTASSnippetToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.setupTASSnippetToolStripMenuItem.Text = "Copy Info Logging Snippet";
            this.setupTASSnippetToolStripMenuItem.Click += new System.EventHandler(this.setupTASSnippetToolStripMenuItem_Click);
            // 
            // returnToOldTemplateToolStripMenuItem
            // 
            this.returnToOldTemplateToolStripMenuItem.Name = "returnToOldTemplateToolStripMenuItem";
            this.returnToOldTemplateToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.returnToOldTemplateToolStripMenuItem.Text = "Return To Old Template";
            this.returnToOldTemplateToolStripMenuItem.Click += new System.EventHandler(this.returnToOldTemplateToolStripMenuItem_Click);
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
            // extraToolStripMenuItem
            // 
            this.extraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.geneticAlgorithmToolStripMenuItem,
            this.computationToolStripMenuItem,
            this.algorithmModeToolStripMenuItem,
            this.simulationThreadCountToolStripMenuItem,
            this.logAlgorithmResultsToolStripMenuItem});
            this.extraToolStripMenuItem.Name = "extraToolStripMenuItem";
            this.extraToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.extraToolStripMenuItem.Text = "Extra";
            // 
            // geneticAlgorithmToolStripMenuItem
            // 
            this.geneticAlgorithmToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.populationToolStripMenuItem,
            this.generationSurvivorsToolStripMenuItem,
            this.mutationMagnitudeToolStripMenuItem,
            this.maxMutationCountToolStripMenuItem});
            this.geneticAlgorithmToolStripMenuItem.Name = "geneticAlgorithmToolStripMenuItem";
            this.geneticAlgorithmToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.geneticAlgorithmToolStripMenuItem.Text = "Genetic Algorithm";
            // 
            // populationToolStripMenuItem
            // 
            this.populationToolStripMenuItem.Name = "populationToolStripMenuItem";
            this.populationToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.populationToolStripMenuItem.Text = "Population";
            // 
            // generationSurvivorsToolStripMenuItem
            // 
            this.generationSurvivorsToolStripMenuItem.Name = "generationSurvivorsToolStripMenuItem";
            this.generationSurvivorsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.generationSurvivorsToolStripMenuItem.Text = "Generation Survivors";
            // 
            // mutationMagnitudeToolStripMenuItem
            // 
            this.mutationMagnitudeToolStripMenuItem.Name = "mutationMagnitudeToolStripMenuItem";
            this.mutationMagnitudeToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.mutationMagnitudeToolStripMenuItem.Text = "Mutation Magnitude";
            // 
            // maxMutationCountToolStripMenuItem
            // 
            this.maxMutationCountToolStripMenuItem.Name = "maxMutationCountToolStripMenuItem";
            this.maxMutationCountToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.maxMutationCountToolStripMenuItem.Text = "Max Mutation Count";
            // 
            // computationToolStripMenuItem
            // 
            this.computationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dontComputeHazardsToolStripMenuItem,
            this.dontComputeCollisionsToolStripMenuItem});
            this.computationToolStripMenuItem.Name = "computationToolStripMenuItem";
            this.computationToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.computationToolStripMenuItem.Text = "Computation";
            // 
            // dontComputeHazardsToolStripMenuItem
            // 
            this.dontComputeHazardsToolStripMenuItem.Name = "dontComputeHazardsToolStripMenuItem";
            this.dontComputeHazardsToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.dontComputeHazardsToolStripMenuItem.Text = "Don\'t Compute Hazards";
            this.dontComputeHazardsToolStripMenuItem.Click += new System.EventHandler(this.dontComputeHazardsToolStripMenuItem_Click);
            // 
            // dontComputeCollisionsToolStripMenuItem
            // 
            this.dontComputeCollisionsToolStripMenuItem.Name = "dontComputeCollisionsToolStripMenuItem";
            this.dontComputeCollisionsToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.dontComputeCollisionsToolStripMenuItem.Text = "Don\'t Compute Walls or Colliders";
            this.dontComputeCollisionsToolStripMenuItem.Click += new System.EventHandler(this.dontComputeCollisionsToolStripMenuItem_Click);
            // 
            // algorithmModeToolStripMenuItem
            // 
            this.algorithmModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.frameGenesOnlyToolStripMenuItem,
            this.disallowWallCollisionToolStripMenuItem});
            this.algorithmModeToolStripMenuItem.Name = "algorithmModeToolStripMenuItem";
            this.algorithmModeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.algorithmModeToolStripMenuItem.Text = "Algorithm Mode";
            // 
            // frameGenesOnlyToolStripMenuItem
            // 
            this.frameGenesOnlyToolStripMenuItem.Name = "frameGenesOnlyToolStripMenuItem";
            this.frameGenesOnlyToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.frameGenesOnlyToolStripMenuItem.Text = "Frame Genes Only";
            this.frameGenesOnlyToolStripMenuItem.Click += new System.EventHandler(this.frameGenesOnlyToolStripMenuItem_Click);
            // 
            // disallowWallCollisionToolStripMenuItem
            // 
            this.disallowWallCollisionToolStripMenuItem.Name = "disallowWallCollisionToolStripMenuItem";
            this.disallowWallCollisionToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.disallowWallCollisionToolStripMenuItem.Text = "Disallow Wall Collision";
            this.disallowWallCollisionToolStripMenuItem.Click += new System.EventHandler(this.disallowWallCollisionToolStripMenuItem_Click);
            // 
            // simulationThreadCountToolStripMenuItem
            // 
            this.simulationThreadCountToolStripMenuItem.Name = "simulationThreadCountToolStripMenuItem";
            this.simulationThreadCountToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.simulationThreadCountToolStripMenuItem.Text = "Simulation Thread Count";
            // 
            // logAlgorithmResultsToolStripMenuItem
            // 
            this.logAlgorithmResultsToolStripMenuItem.Name = "logAlgorithmResultsToolStripMenuItem";
            this.logAlgorithmResultsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.logAlgorithmResultsToolStripMenuItem.Text = "Log Algorithm Results";
            this.logAlgorithmResultsToolStripMenuItem.Click += new System.EventHandler(this.logAlgorithmResultsToolStripMenuItem_Click);
            // 
            // txt_checkpoints
            // 
            this.txt_checkpoints.DetectUrls = false;
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
            this.cbx_timingTestFavDirectly.Location = new System.Drawing.Point(604, 408);
            this.cbx_timingTestFavDirectly.Name = "cbx_timingTestFavDirectly";
            this.cbx_timingTestFavDirectly.Size = new System.Drawing.Size(110, 19);
            this.cbx_timingTestFavDirectly.TabIndex = 54;
            this.cbx_timingTestFavDirectly.Text = "Test Timings On";
            this.cbx_timingTestFavDirectly.UseVisualStyleBackColor = true;
            // 
            // num_gensPerTiming
            // 
            this.num_gensPerTiming.Location = new System.Drawing.Point(608, 247);
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
            this.lbl_gensPerTiming.Location = new System.Drawing.Point(593, 229);
            this.lbl_gensPerTiming.Name = "lbl_gensPerTiming";
            this.lbl_gensPerTiming.Size = new System.Drawing.Size(129, 15);
            this.lbl_gensPerTiming.TabIndex = 57;
            this.lbl_gensPerTiming.Text = "Gens Per Tested Timing";
            // 
            // num_shuffleCount
            // 
            this.num_shuffleCount.Location = new System.Drawing.Point(608, 341);
            this.num_shuffleCount.Name = "num_shuffleCount";
            this.num_shuffleCount.Size = new System.Drawing.Size(99, 23);
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
            this.lbl_shuffleCount.Location = new System.Drawing.Point(597, 323);
            this.lbl_shuffleCount.Name = "lbl_shuffleCount";
            this.lbl_shuffleCount.Size = new System.Drawing.Size(120, 15);
            this.lbl_shuffleCount.TabIndex = 59;
            this.lbl_shuffleCount.Text = "Timing Shuffle Count";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(600, 430);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 15);
            this.label1.TabIndex = 62;
            this.label1.Text = "Initial Inputs Directly";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(731, 469);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_shuffleCount);
            this.Controls.Add(this.num_shuffleCount);
            this.Controls.Add(this.lbl_gensPerTiming);
            this.Controls.Add(this.num_gensPerTiming);
            this.Controls.Add(this.cbx_timingTestFavDirectly);
            this.Controls.Add(this.txt_customHitboxes);
            this.Controls.Add(this.txt_checkpoints);
            this.Controls.Add(this.lbl_manualHitboxes);
            this.Controls.Add(this.lbl_checkpoints);
            this.Controls.Add(this.num_framecount);
            this.Controls.Add(this.lbl_framecount);
            this.Controls.Add(this.num_generations);
            this.Controls.Add(this.lbl_generations);
            this.Controls.Add(this.lbl_initSolution);
            this.Controls.Add(this.txt_initSolution);
            this.Controls.Add(this.txt_infoFile);
            this.Controls.Add(this.btn_selectInfoFile);
            this.Controls.Add(this.btn_beginAlg);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.num_generations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_framecount)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_gensPerTiming)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_shuffleCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_beginAlg;
        private System.Windows.Forms.Button btn_selectInfoFile;
        private System.Windows.Forms.TextBox txt_infoFile;
        private System.Windows.Forms.RichTextBox txt_initSolution;
        private System.Windows.Forms.Label lbl_initSolution;
        private System.Windows.Forms.NumericUpDown num_generations;
        private System.Windows.Forms.Label lbl_generations;
        private System.Windows.Forms.NumericUpDown num_framecount;
        private System.Windows.Forms.Label lbl_framecount;
        private System.Windows.Forms.Label lbl_checkpoints;
        private System.Windows.Forms.Label lbl_manualHitboxes;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txt_checkpoints;
        private System.Windows.Forms.RichTextBox txt_customHitboxes;
        private System.Windows.Forms.CheckBox cbx_timingTestFavDirectly;
        private System.Windows.Forms.NumericUpDown num_gensPerTiming;
        private System.Windows.Forms.Label lbl_gensPerTiming;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupTASSnippetToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown num_shuffleCount;
        private System.Windows.Forms.Label lbl_shuffleCount;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logFlightOfInitialInputsToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem extraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geneticAlgorithmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem populationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generationSurvivorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mutationMagnitudeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maxMutationCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem computationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dontComputeHazardsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dontComputeCollisionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem algorithmModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frameGenesOnlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disallowWallCollisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulationThreadCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logAlgorithmResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AutoSetInfoTemplate;
        private System.Windows.Forms.ToolStripMenuItem returnToOldTemplateToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
    }
}

