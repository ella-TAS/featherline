using System.IO;
using System.Xml.Serialization;
using System;

namespace Featherline
{
    partial class Form1
    {
        public static FileStream configFile = new FileStream("settings.xml", FileMode.OpenOrCreate);

        public void LoadSettings(ref Settings s)
        {
            try {
                s = (Settings)new XmlSerializer(typeof(Settings)).Deserialize(configFile);
            }
            catch {
                s = new Settings();
            }

            num_framecount.Value = settings.Framecount;
            population.Value = settings.Population;
            genSurvivors.Value = settings.SurvivorCount;
            num_generations.Value = settings.Generations;
            num_population_ValueChanged(null, null);

            mutMagnitude.Value = (decimal)settings.MutationMagnitude;
            maxMutations.Value = settings.MaxMutChangeCount;

            txt_infoFile.Text = settings.InfoFile;
            txt_initSolution.Text = settings.Favorite;

            //txt_customSpinners.Text = settings.CustomSpinnerNames;

            disallowWallCollisionToolStripMenuItem.Checked = settings.AvoidWalls;

            txt_customHitboxes.Lines = settings.ManualHitboxes;
            txt_checkpoints.Lines = settings.Checkpoints;

            cbx_timingTestFavDirectly.Checked = settings.TimingTestFavDirectly;
            frameGenesOnlyToolStripMenuItem.Checked = settings.FrameBasedOnly;
            num_gensPerTiming.Value = settings.GensPerTiming;

            num_shuffleCount.Value = settings.ShuffleCount;

            threadCount.Value = settings.MaxThreadCount;

            logAlgorithmResultsToolStripMenuItem.Checked = settings.LogResults;
        }

        public void SaveSettings()
        {
            // put information into the settings object
            settings.InfoFile = txt_infoFile.Text;
            settings.Favorite = txt_initSolution?.Text ?? null;

            settings.Generations = (int)num_generations.Value;
            settings.Population = (int)population.Value;
            settings.SurvivorCount = (int)genSurvivors.Value;
            settings.Framecount = (int)num_framecount.Value;

            settings.MutationMagnitude = (float)mutMagnitude.Value;
            settings.MaxMutChangeCount = (int)maxMutations.Value;

            settings.AvoidWalls = disallowWallCollisionToolStripMenuItem.Checked;

            settings.ManualHitboxes = txt_customHitboxes.Lines;
            settings.Checkpoints = txt_checkpoints.Lines;

            settings.TimingTestFavDirectly = cbx_timingTestFavDirectly.Checked;
            settings.FrameBasedOnly = frameGenesOnlyToolStripMenuItem.Checked;
            settings.GensPerTiming = (int)num_gensPerTiming.Value;

            settings.ShuffleCount = (int)num_shuffleCount.Value;

            settings.MaxThreadCount = (int)threadCount.Value;

            settings.LogResults = logAlgorithmResultsToolStripMenuItem.Checked;

            // reset the config file and serialize
            configFile.SetLength(0);
            new XmlSerializer(typeof(Settings)).Serialize(configFile, settings);
        }

        /*private string GetCustomSpinnerNames() => Regex.Matches(txt_customSpinners.Text, @"\S+")
            .Aggregate("", (res, m) => res + $"{{{m.Value}.Position}}");*/
    }

    public class Settings
    {
        public string InfoFile;

        public string Favorite;
        public int Framecount = 120;

        public int Population = 50;
        public int Generations = 3000;
        public int SurvivorCount = 20;

        public float MutationMagnitude = 8;
        public int MaxMutChangeCount = 5;

        public string[] Checkpoints;

        public string CustomSpinnerNames;

        public bool AvoidWalls = true;
        //public bool EnableSteepTurns = false;

        public string[] ManualHitboxes;

        public bool FrameBasedOnly = false;
        public bool TimingTestFavDirectly = false;
        public int GensPerTiming = 150;

        public int ShuffleCount = 2;

        public int MaxThreadCount;

        public bool LogResults = true;

        public bool ComputeHazards;
        public bool ComputeCollision;

        public Settings Copy() => (Settings)MemberwiseClone();

        public Settings()
        {
            MaxThreadCount = Environment.ProcessorCount;
        }
    }
}