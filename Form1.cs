using System;
using System.Windows.Forms;
using static Featherline.GAManager;

namespace Featherline
{
    public partial class Form1 : Form
    {
        public static Settings settings;

        public Form1()
        {
            InitializeComponent();
            LoadSettings(ref settings);
            Settings.manualHBSrc = grd_manualHitboxes;
        }

        private void btn_beginAlg_Click(object sender, EventArgs e)
        {
            BeginAlgorithm(this);
        }

        private const string customInfoTemplate = "{CrystalStaticSpinner.Position}{DustStaticSpinner.Position}{FrostHelper.CustomSpinner@FrostTempleHelper.Position}{VivHelper.Entities.CustomSpinner@VivHelper.Position}{Celeste.Mod.XaphanHelper.Entities.CustomSpinner@XaphanHelper.Position} LightningUL: {Lightning.TopLeft} LightningDR: {Lightning.BottomRight} SpikeUL: {Spikes.TopLeft} SpikeDR: {Spikes.BottomRight} SpikeDir: {Spikes.Direction} Feathers: {FlyFeather.TopLeft}{ColorfulFlyFeather.TopLeft} TouchSwitches: {TouchSwitch.TopLeft}{FlagTouchSwitch.TopLeft} Wind: {Level.Wind} WTPos: {WindTrigger.Position} WTPattern: {WindTrigger.Pattern} WTWidth: {WindTrigger.Width} WTHeight: {WindTrigger.Height} Bounds: {Level.Bounds} Solids: {Level.Session.LevelData.Solids}";
        private const string explainSettingCustomInfo = "A custom info template has been copied to your clipboard.\nBefore pressing OK, right click on the info panel of celeste studio and click \"Set Custom Info Template From Clipboard\".\nDisable entity watch if you have it on.";
        private const string infoOutputTAS = "StartExportGameInfo infodump.txt\n   1\nFinishExportGameInfo";
        private const string explainInfoOutput = "A TAS script was copied to your clipboard.\nPaste it to your TAS and make sure that the single frame in between the two TAS commands is the last frame before\nsimulation begins, then play back that part of the TAS.\nIt will create infodump.txt in your Celeste folder.";

        private void btn_infoExtraction_Click(object sender, EventArgs e)
        {
            var custom = GetCustomSpinnerNames();
            Clipboard.SetText(custom + customInfoTemplate);
            MessageBox.Show(explainSettingCustomInfo);
            Clipboard.SetText(infoOutputTAS);
            MessageBox.Show(explainInfoOutput);
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

        private void cbx_inputLinesMode_CheckedChanged(object sender, EventArgs e) => num_inputLineCount.Enabled = cbx_inputLinesMode.Checked;

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
