namespace Featherline;
public class Settings
{
    [Setting(Input = "txt_infoFile.Text")]
    public string? InfoFile;

    [Setting(Input = "txt_initSolution.Text")]
    public string? Favorite;
    [Setting(Input = "num_framecount.Value")]
    public int Framecount = 120;

    [Setting(Input = "population.Value")]
    public int Population = 50;
    [Setting(Input = "num_generations.Value")]
    public int Generations = 2000;
    [Setting(Input = "genSurvivors.Value")]
    public int SurvivorCount = 20;

    [Setting(Input = "mutMagnitude.Value")]
    public float MutationMagnitude = 8;
    [Setting(Input = "maxMutations.Value")]
    public int MaxMutChangeCount = 5;

    [Setting(Input = "txt_checkpoints.Lines")]
    public string[]? Checkpoints;

    public string? CustomSpinnerNames;

    [Setting(Input = "disallowWallCollisionToolStripMenuItem.Checked")]
    public bool AvoidWalls = false;

    [Setting(Input = "txt_customHitboxes.Lines")]
    public string[]? ManualHitboxes;

    [Setting(Input = "frameGenesOnlyToolStripMenuItem.Checked")]
    public bool FrameBasedOnly = false;
    [Setting(Input = "cbx_timingTestFavDirectly.Checked")]
    public bool TimingTestFavDirectly = false;
    [Setting(Input = "num_gensPerTiming.Value")]
    public int GensPerTiming = 150;

    [Setting(Input = "num_shuffleCount.Value")]
    public int ShuffleCount = 6;

    [Setting(Input = "threadCount.Value")]
    public int MaxThreadCount = Environment.ProcessorCount;

    [Setting(Input = "logAlgorithmResultsToolStripMenuItem.Checked")]
    public bool LogResults = true;

    //public bool IgnoreHazards = false;
    //public bool IgnoreCollision = false;

    public string? OldInfoTemplate;

    public bool KnowsHelpTxt;

    public Settings Copy() => (Settings)MemberwiseClone();
}
