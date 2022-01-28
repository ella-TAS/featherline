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
        public static Settings settings;

        public Form1()
        {
            InitializeComponent();
            LoadSettings(ref settings);
        }

        private void btn_beginAlg_Click(object sender, EventArgs e)
        {
            BeginAlgorithm(this);
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        const string helpFile = "help.txt";
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(helpFile)) {
                var text = "\nTo get started with Featherline, follow these steps:\n\n1: Click on Copy -> Info Template to copy the custom info template to your clipboard.\nThen Right click on the info panel of celeste studio and click \"Set Custom Info Template From Clipboard\".\nAlso, entity watch would likely break it if you have it on.\n\n2: Click on Copy -> Setup TAS Snippet to copy the info file creation script to your clipboard,\nthen paste it to your TAS. Then make sure you make the first frame of feather movement is in between the commands.\nAs an example:\n\n  13,R,U,X\n  26\nStartExportGameInfo infodump.txt\n   1,R,U\nFinishExportGameInfo\n\nThen run the TAS over those commands to create/update infodump.txt in your Celeste folder.\n\n3: Click on the Select Information Source File button and select the infodump.txt file in your celeste folder.\n\n4: Either define a checkpoint at every major turn of the part you want to TAS,\nor give the program initial inputs to work with then define one checkpoint as the ending.\nCheckpoints are further explained later in this file.\n\n5: Click the Run Algorithm button.\n\n\n\n--- Checkpoints ---\n\n- To define a checkpoint, hold your info HUD hotkey and drag while holding right click to draw a rectangle hitbox.\n  Doing that will copy the selected area to your clipboard, where you can paste it to a line on the checkpoints text box.\n\n- Checkpoint collision is based on your hurtbox. To define a touch switch or feather as a checkpoint, select\n  an area that perfectly overlaps with its hitbox. Remember to use the pink, bigger hitbox for touch switches.\n\n- The genetic algorithm primarily flies directly towards the next checkpoint.\n  If the next checkpoint is behind a wall of spinners, it will simply fly towards that\n  wall of spinners and try to get as close to the checkpoint as it can that way.\n  That means you should define a checkpoint at every major turn so it knows where to go.\n";
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
        }

        private const string customInfoTemplate = "{CrystalStaticSpinner.Position}{DustStaticSpinner.Position}{FrostHelper.CustomSpinner@FrostTempleHelper.Position}{VivHelper.Entities.CustomSpinner@VivHelper.Position}{Celeste.Mod.XaphanHelper.Entities.CustomSpinner@XaphanHelper.Position} LightningUL: {Lightning.TopLeft} LightningDR: {Lightning.BottomRight} SpikeUL: {Spikes.TopLeft} SpikeDR: {Spikes.BottomRight} SpikeDir: {Spikes.Direction} Feathers: {FlyFeather.TopLeft}{ColorfulFlyFeather.TopLeft} TouchSwitches: {TouchSwitch.TopLeft}{FlagTouchSwitch.TopLeft} Wind: {Level.Wind} WTPos: {WindTrigger.Position} WTPattern: {WindTrigger.Pattern} WTWidth: {WindTrigger.Width} WTHeight: {WindTrigger.Height} Bounds: {Level.Bounds} Solids: {Level.Session.LevelData.Solids}";
        private void infoTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var custom = GetCustomSpinnerNames();
            Clipboard.SetText(custom + customInfoTemplate);
        }

        private const string infoOutputTAS = "StartExportGameInfo infodump.txt\n   1\nFinishExportGameInfo";
        private void setupTASSnippetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(infoOutputTAS);
        }

        private void num_maxThreadCount_ValueChanged(object sender, EventArgs e)
        {
            if (num_maxThreadCount.Value < 1.5m) {
                num_maxThreadCount.Value = num_maxThreadCount.Value < 0 ? -1 : 1;
            }
            num_maxThreadCount.Value = Math.Round(num_maxThreadCount.Value);
        }
    }
}
