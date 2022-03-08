using static Featherline.GAManager;
using System.Diagnostics;
using System.Text;
using static System.Drawing.Color;
using System.Text.RegularExpressions;

namespace Featherline;

public partial class Form1 : Form
{
    public const string version = "0.3.2";

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

        LoadSettings();

        if (!settings.KnowsHelpTxt) {
            new Thread(() => {
                bool toggle = false;
                var yellow = FromArgb(240, 255, 50);
                var normal = FromArgb(245, 245, 245);
                try {
                    while (true) {
                        if (settings.KnowsHelpTxt) {
                            Invoke((Action)(() => helpToolStripMenuItem.BackColor = normal));
                            return;
                        }
                        Invoke((Action)(() => helpToolStripMenuItem.BackColor = toggle ? normal : yellow));
                        toggle = !toggle;
                        Thread.Sleep(500);
                    }
                }
                catch { }
            }).Start();
        }

        void ResetPress(object? sender, KeyEventArgs e)
        {
            if (ModifierKeys == Keys.Control && e.KeyCode == Keys.U) {
                txt_checkpoints.Text = "";
                txt_initSolution.Text = "";
                txt_customHitboxes.Text = "";
            }
        }

        txt_checkpoints.KeyDown += ResetPress;
        txt_initSolution.KeyDown += ResetPress;
        txt_customHitboxes.KeyDown += ResetPress;
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
                if (runEnd)
                    EndAlgorithm();
                ClearAlgorithmData();
                btn_beginAlg.Invoke(() => {
                    btn_beginAlg.Text = "Run Algorithm";
                    btn_beginAlg.BackColor = LightSteelBlue;
                });
                algRunning = false;
                algClosing = false;
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
        using var dialog = new OpenFileDialog();
        dialog.Filter = "Info Dump|infodump.txt";
        dialog.Title = "Select infodump.txt";
        var res = dialog.ShowDialog();
        if (res == DialogResult.OK) {
            txt_infoFile.Text = dialog.FileName;
            settings.InfoFile = dialog.FileName;
            settings.KnowsHelpTxt = true;
        }
    }

    private void num_population_ValueChanged(object sender, EventArgs e) => new string('a', 3);//num_survivorCount.Maximum = num_population.Value / 2;

    const string helpFile = "help.txt";
    private void helpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!File.Exists(helpFile)) {
            var text = @"
To get started with Featherline, follow these steps:

1:  Click on Setup -> Auto Set Info Template when you have celeste open to automatically
    apply the custom info template for extracting game information.
    If this doesn't work, click on Setup -> Info Template to copy the template to your clipboard then
    right click on the info panel of celeste studio and click 'Set Custom Info Template From Clipboard'.
    The automatic way only works if you had debug mode enabled when launching celeste.

2:  Click on Setup -> Copy Info Logging Snippet to copy the info file creation script to your clipboard,
    then paste it into your TAS. Then make sure that the last frame before the simulated inputs is in between the commands.
    Example:

      13,R,U,X
      26
    StartExportGameInfo infodump.txt
       1,R,U
    FinishExportGameInfo

    Then run the TAS over those commands to create/update infodump.txt in your Celeste folder.
    This same method works even if the start frame is in an already going feather.
    Enabling custom info in celesteTAS is not needed. It works regardless.
    The 1,R,U input is for a featherboost. If you don't know what that is, you can ask in #tas_general on the celeste discord.

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
  If the algorithm messes up at any of the points where progress is reversed, it has to be able to
  fix itself by simply attempting to fly toward the next checkpoint the entire time.

- Making checkpoints really small is not recommended. Making them big does not make the result worse
  and it only cares about whether you touched the checkpoint or not.
  When the algorithm at some moment has not reached a checkpoint, it tries to get to it by aiming for its center
  (the final checkpoint is an exception to this). You can use this to guide the algorithm better by
  making the checkpoints bigger.


--- Custom Hitboxes ---

- Defined the same way as checkpoints but in the text box on the right.
- A defined hitbox is a killbox by default, based on the green hurtbox.
- To define a collider (based on the red collision box) instead of a killbox, place 'c' after the definition.
  Example: '218, -104, 234, -72 c'
- Fully static tile entities will automatically be added as colliders behind the scenes,
  but kevins, falling blocks and others that have the potential to move in some way,
  you will have to define manually.


--- Algorithm Facts ---

- Sometimes the final results of the algorithm will die by an extremely small amount, like 0.0002 pixels.
  When this happens, the solution is to change one of the angles before that point by 0.001 manually
  or by a little bit more if needed.
- Each checkpoint collected adds 10000 to fitness. You can use that knowledge to track how the algorithm is doing.


--- Supported Gameplay Mechanics ---

- anything with a static spinner hitbox, spikes and lightning
- Wind and wind triggers
- Dodging or bouncing off walls. Tile entities explained in Custom Hitboxes section.
- Jumpthroughs
- Correct physics with room bounds


If you have questions that aren't explained anywhere in this guide, feel free to ping TheRoboMan on the celeste discord.
";

            var file = File.Create(helpFile);
            file.Write(Encoding.UTF8.GetBytes(text));
            file.Close();
        }
        Process.Start("notepad.exe", helpFile);
        settings.KnowsHelpTxt = true;
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

    private void infoTemplateToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Clipboard.SetText(/*custom + */customInfoTemplate);
        using var toolTip = new ToolTip();
        toolTip.Show("Failed", this, PointToClient(Cursor.Position), 2000);
    }

    private const string InfoLogSnippet = "StartExportGameInfo infodump.txt\n   1\nFinishExportGameInfo";
    private void setupTASSnippetToolStripMenuItem_Click(object sender, EventArgs e) => Clipboard.SetText(InfoLogSnippet);

    private void logFlightOfInitialInputsToolStripMenuItem_Click(object sender, EventArgs e) {
        if (!algRunning & !algClosing) {
            SaveSettings();
            RunAlgorithm(this, btn_beginAlg, true);
        }
    }

    private void dontComputeHazardsToolStripMenuItem_Click(object sender, EventArgs e) => dontComputeHazardsToolStripMenuItem.Checked ^= true;

    private async void AutoSetInfoTemplate_Click(object sender, EventArgs e)
    {
        returnToOldTemplateToolStripMenuItem.Checked = false;
        try {
            using var client = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(500)};
            var oldTemplateRequest = await client.GetAsync("http://localhost:32270/tas/custominfo");
            var oldTemplate = Regex.Match(await oldTemplateRequest.Content.ReadAsStringAsync(), @"<pre>([\S\s]*)</pre>").Groups[1].Value;

            await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"http://localhost:32270/tas/custominfo?template={customInfoTemplate}"));

            if (oldTemplate != customInfoTemplate)
                settings.OldInfoTemplate = oldTemplate;

            AutoSetInfoTemplate.Checked = true;
        }
        catch {
            AutoSetInfoTemplate.Checked = false;
            var msgBoxRes = MessageBox.Show(this, "Failed to set the custom info template.\nCopy it to clipboard instead?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2);
            if (msgBoxRes == DialogResult.Yes)
                infoTemplateToolStripMenuItem_Click(null, null);
        }
    }

    private async void returnToOldTemplateToolStripMenuItem_Click(object sender, EventArgs e)
    {
        AutoSetInfoTemplate.Checked = false;
        if (settings.OldInfoTemplate is null) {
            returnToOldTemplateToolStripMenuItem.Checked = false;
            MessageBox.Show(this, "No old info template to return to has been saved.");
            return;
        }
        try {
            using var client = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(500)};
            await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"http://localhost:32270/tas/custominfo?template={settings.OldInfoTemplate}"));
            returnToOldTemplateToolStripMenuItem.Checked = true;
        }
        catch {
            returnToOldTemplateToolStripMenuItem.Checked = false;
            var msgBoxRes = MessageBox.Show(this, "Failed to return to old template.\nCopy it to clipboard instead?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2);
            if (msgBoxRes == DialogResult.Yes)
                Clipboard.SetText(settings.OldInfoTemplate);
        }
    }

    private void dontComputeCollisionsToolStripMenuItem_Click(object sender, EventArgs e) => dontComputeCollisionsToolStripMenuItem.Checked ^= true;
    private void disallowWallCollisionToolStripMenuItem_Click(object sender, EventArgs e) => disallowWallCollisionToolStripMenuItem.Checked ^= true;
    private void logAlgorithmResultsToolStripMenuItem_Click(object sender, EventArgs e) => logAlgorithmResultsToolStripMenuItem.Checked ^= true;
    private void frameGenesOnlyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        frameGenesOnlyToolStripMenuItem.Checked ^= true;
        num_gensPerTiming.Enabled = num_shuffleCount.Enabled = cbx_timingTestFavDirectly.Enabled = !frameGenesOnlyToolStripMenuItem.Checked;
    }
}
