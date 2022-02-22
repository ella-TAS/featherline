using System;
using System.Windows.Forms;
using static Featherline.GAManager;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using static System.Drawing.Color;
using System.Drawing;
using System.Net.Http;

namespace Featherline
{
    public partial class Form1 : Form
    {
        public const string version = "0.3.1";

        public static Settings settings;

        NumMenuItem population, genSurvivors, mutMagnitude, maxMutations, threadCount;

        public Icon appIcon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public Form1()
        {
            InitializeComponent();

            Text = "Featherline " + version;
            Icon = appIcon;

            population = new NumMenuItem(populationToolStripMenuItem, 2m, 999999m, 0) {
                onValueUpdate = vl => {
                    genSurvivors.Value = Math.Min(genSurvivors.Value, vl - 1);
                    return vl;
                }
            };
            genSurvivors = new NumMenuItem(generationSurvivorsToolStripMenuItem, 1m, 999999m, 0, () => (1m, population.Value - 1));
            mutMagnitude = new NumMenuItem(mutationMagnitudeToolStripMenuItem, 0m, 180m, 2);
            maxMutations = new NumMenuItem(maxMutationCountToolStripMenuItem, 1m, 999999m, 0);
            threadCount =  new NumMenuItem(simulationThreadCountToolStripMenuItem, -1m, 100m, 0) { onValueUpdate = vl => vl <= 0m ? -1m : vl };

            LoadSettings(ref settings);
        }

        private bool algRunning;
        private bool algClosing;
        private void btn_beginAlg_Click(object sender, EventArgs e)
        {
            if (!algRunning) {
                algRunning = true;

                SaveSettings();
                new Thread(() => {
                    bool runEnd = RunAlgorithm(this, btn_beginAlg, false);
                    algRunning = false;
                    algClosing = false;
                    if (runEnd)
                        EndAlgorithm();
                    btn_beginAlg.Invoke((Action)(() => {
                        btn_beginAlg.Text = "Run Algorithm";
                        btn_beginAlg.BackColor = LightSteelBlue;
                    }));
                }).Start();

                btn_beginAlg.Text = "Abort Algorithm";
                btn_beginAlg.BackColor = FromArgb(255, 255, 150, 150);
            }
            else if (!algClosing) {
                abortAlgorithm = true;
                algClosing = true;
                btn_beginAlg.Text = "Run Algorithm";
                btn_beginAlg.BackColor = LightSteelBlue;
            }
        }

        public static void ResetRunAlgButton(Control button)
        {
            button.Text = "Run Algorithm";
            button.BackColor = LightSteelBlue;
        }

        private void btn_selectInfoFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Info Dump|infodump.txt";
            dialog.Title = "Select infodump.txt";
            var res = dialog.ShowDialog();
            if (res == DialogResult.OK) {
                txt_infoFile.Text = dialog.FileName;
                settings.InfoFile = dialog.FileName;
            }
        }

        private void num_population_ValueChanged(object sender, EventArgs e) => new string('a', 3);//num_survivorCount.Maximum = num_population.Value / 2;

        const string helpFile = "help.txt";
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(helpFile)) {
                var text = @"
To get started with Featherline, follow these steps:

1:  Click on Copy -> Info Template to copy the custom info template to your clipboard.
	Then Right click on the info panel of celeste studio and click 'Set Custom Info Template From Clipboard'.

2:  Click on Copy -> Info Logging Snippet to copy the info file creation script to your clipboard,
	then paste it to your TAS. Then make sure that the last frame before the simulated inputs is in between the commands.
	Example:

	  13,R,U,X
	  26
	StartExportGameInfo infodump.txt
	   1,R,U
	FinishExportGameInfo

	Then run the TAS over those commands to create/update infodump.txt in your Celeste folder.
	This same method works even if the start frame is in an already going feather.

3:  Click on the Select Information Source File button and select the infodump.txt file in your celeste folder.

4:  Define a checkpoint at every turn or branching point of the path you want to TAS.
	Checkpoints are further explained later in this file.

5:  Click the Run Algorithm button.


--- Checkpoints ---

- To define a checkpoint, hold your info HUD hotkey and drag while holding right click to draw a rectangle hitbox.
  Doing that will copy the selected area to your clipboard, where you can paste it to a line on the checkpoints text box.

- Checkpoint collision is based on your hurtbox. To define a touch switch or feather as a checkpoint, select
  an area that perfectly overlaps with its hitbox. Remember to use the pink, bigger hitbox for touch switches.

- The genetic algorithm primarily flies directly towards the next checkpoint.
  If the next checkpoint is behind a wall of spinners, it will simply fly towards that
  wall of spinners and try to get as close to the checkpoint as it can that way.
  That means you should define a checkpoint at every major turn so it knows where to go.


--- Custom Hitboxes ---

- Defined the same way as checkpoints in the text box on the right.
- A defined checkpoint is a killbox by default, based on the green hurtbox.
- To define a collider (based on the red collision box) instead of a killbox, place 'c' after the definition.
  Example: '218, -104, 234, -72 c'
- Fully static tile entities will automatically be added as colliders behind the scenes,
  but kevins, falling blocks and others that have the potential to move in some way,
  you will have to define manually.


--- Supported Gameplay Mechanics ---

- anything with a static spinner hitbox, spikes and lightning
- Wind and wind triggers
- Dodging or bouncing off walls. Tile entities explained in Custom Hitboxes section.
- Jumpthroughs
- Correct physics with room bounds


If you have questions that aren't explained anywhere in this guide, feel free to ping TheRoboMan on the celeste discord.";

                var file = File.Create(helpFile);
                file.Write(Encoding.UTF8.GetBytes(text));
                file.Close();
            }
            Process.Start("notepad.exe", helpFile);
        }

        private const string customInfoTemplate =
            "PosRemainder: {Player.PositionRemainder} " +
            "Lerp: {Player.starFlySpeedLerp} " +

            "{CrystalStaticSpinner.Position}{DustStaticSpinner.Position}{FrostHelper.CustomSpinner@FrostTempleHelper.Position}{VivHelper.Entities.CustomSpinner@VivHelper.Position}{Celeste.Mod.XaphanHelper.Entities.CustomSpinner@XaphanHelper.Position} " +
            
            "LightningUL: {Lightning.TopLeft} " +
            "LightningDR: {Lightning.BottomRight} " +

            "SpikeUL: {Spikes.TopLeft} " +
            "SpikeDR: {Spikes.BottomRight} " +
            "SpikeDir: {Spikes.Direction} " +

            "Wind: {Level.Wind} " +
            "WTPos: {WindTrigger.Position} " +
            "WTPattern: {WindTrigger.Pattern} " +
            "WTWidth: {WindTrigger.Width} " +
            "WTHeight: {WindTrigger.Height} " +

            "StarJumpUL: {StarJumpBlock.TopLeft} " +
            "StarJumpDR: {StarJumpBlock.BottomRight} " +
            "StarJumpSinks: {StarJumpBlock.sinks} " +

            "JThruUL: {JumpthruPlatform.TopLeft} " +
            "JThruDR: {JumpthruPlatform.BottomRight} " +
            "SideJTUL: {SidewaysJumpThru.TopLeft} " +
            "SideJTDR: {SidewaysJumpThru.BottomRight} " +
            "SideJTIsRight: {SidewaysJumpThru.AllowLeftToRight} " +
            "SideJTPushes: {SidewaysJumpThru.pushPlayer} " +
            "UpsDJTUL: {UpsideDownJumpThru.TopLeft} " +
            "UpsDJTDR: {UpsideDownJumpThru.BottomRight} " +
            "UpsDJTPushes: {UpsideDownJumpThru.pushPlayer} " +

            "Bounds: {Level.Bounds} " +
            "Solids: {Level.Session.LevelData.Solids}";

        private async void infoTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //using var client = new HttpClient();
            //var test = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"http://localhost:32270/tas/custominfo?template={customInfoTemplate}"));
            
            //var custom = GetCustomSpinnerNames();

            Clipboard.SetText(/*custom + */customInfoTemplate);
        }

        private const string InfoLogSnippet = "StartExportGameInfo infodump.txt\n   1\nFinishExportGameInfo";
        private void setupTASSnippetToolStripMenuItem_Click(object sender, EventArgs e) => Clipboard.SetText(InfoLogSnippet);

        private void logFlightOfInitialInputsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveSettings();
            RunAlgorithm(this, btn_beginAlg, true);
        }

        private void dontComputeHazardsToolStripMenuItem_Click(object sender, EventArgs e) => dontComputeHazardsToolStripMenuItem.Checked ^= true;
        private void dontComputeCollisionsToolStripMenuItem_Click(object sender, EventArgs e) => dontComputeCollisionsToolStripMenuItem.Checked ^= true;
        private void disallowWallCollisionToolStripMenuItem_Click(object sender, EventArgs e) => disallowWallCollisionToolStripMenuItem.Checked ^= true;
        private void logAlgorithmResultsToolStripMenuItem_Click(object sender, EventArgs e) => logAlgorithmResultsToolStripMenuItem.Checked ^= true;
        private void frameGenesOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frameGenesOnlyToolStripMenuItem.Checked ^= true;
            num_gensPerTiming.Enabled = num_shuffleCount.Enabled = cbx_timingTestFavDirectly.Enabled = !frameGenesOnlyToolStripMenuItem.Checked;
        }
    }
}
