using System;
using System.Windows.Forms;
using static Featherline.GAManager;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Featherline
{
    public partial class Form1 : Form
    {
        public const string version = "0.3.0";

        public static Settings settings;

        public Form1()
        {
            InitializeComponent();
            LoadSettings(ref settings);
            Text = "Featherline " + version;
        }

        private void btn_beginAlg_Click(object sender, EventArgs e)
        {
            BeginAlgorithm(this, false);
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

        private void num_population_ValueChanged(object sender, EventArgs e) => num_survivorCount.Maximum = num_population.Value / 2;

        const string helpFile = "help.txt";
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(helpFile)) {
                var text = @"
To get started with Featherline, follow these steps:

1: Click on Copy -> Info Template to copy the custom info template to your clipboard.
Then Right click on the info panel of celeste studio and click 'Set Custom Info Template From Clipboard'.
Also, entity watch would likely break it if you have it on.

2: Click on Copy -> Setup TAS Snippet to copy the info file creation script to your clipboard,
then paste it to your TAS. Then make sure you make the first frame of feather movement is in between the commands.
As an example:

  13,R,U,X
  26
StartExportGameInfo infodump.txt
   1,R,U
FinishExportGameInfo

Then run the TAS over those commands to create/update infodump.txt in your Celeste folder.
This same method works even if the start frame is in an already going feather.

3: Click on the Select Information Source File button and select the infodump.txt file in your celeste folder.

4: Either define a checkpoint at every major turn or branching path of the part you want to TAS,
or give the program initial inputs to work with then define one checkpoint as the ending.
Checkpoints are further explained later in this file.

5: Click the Run Algorithm button.


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

- Defined the same way as checkpoints.
- A defined checkpoint is a killbox by default, based on the green hurtbox.
- To define a collider (based on the red collision box) instead of a killbox, place 'c' after the definition.
  Example: '218, -104, 234, -72 c'


--- Supported Gameplay Mechanics ---

- anything with a static spinner hitbox, spikes and lightning
- Wind and wind triggers
- Dodging or bouncing off walls. Block entities work but have to be defined manually.
- Jumpthroughs
- Correct physics with room bounds
";

                var file = File.Create(helpFile);
                file.Write(Encoding.UTF8.GetBytes(text));
                file.Close();
            }
            Process.Start("notepad.exe", helpFile);
        }

        private void cbx_frameBasedOnly_CheckedChanged(object sender, EventArgs e)
        {
            cbx_timingTestFavDirectly.Enabled = !cbx_frameBasedOnly.Checked;
            num_gensPerTiming.Enabled = !cbx_frameBasedOnly.Checked;
            num_shuffleCount.Enabled = !cbx_frameBasedOnly.Checked;
        }

        private const string customInfoTemplate =
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

        private void infoTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var custom = GetCustomSpinnerNames();
            Clipboard.SetText(custom + customInfoTemplate);
        }

        private const string infoOutputTAS = "StartExportGameInfo infodump.txt\n   1\nFinishExportGameInfo";
        private void setupTASSnippetToolStripMenuItem_Click(object sender, EventArgs e) => Clipboard.SetText(infoOutputTAS);

        private void num_maxThreadCount_ValueChanged(object sender, EventArgs e)
        {
            if (num_maxThreadCount.Value < 1.5m)
                num_maxThreadCount.Value = num_maxThreadCount.Value < 0 ? -1 : 1;
            num_maxThreadCount.Value = Math.Round(num_maxThreadCount.Value);
        }

        private void logFlightOfInitialInputsToolStripMenuItem_Click(object sender, EventArgs e) => BeginAlgorithm(this, true);
    }
}
