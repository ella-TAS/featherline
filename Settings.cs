using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using System.Text.RegularExpressions;
using System.Linq;

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
            num_population.Value = settings.Population;
            num_generations.Value = settings.Generations;
            num_survivorCount.Value = settings.SurvivorCount;
            num_population_ValueChanged(null, null);

            num_mutationMagnitude.Value = (decimal)settings.MutationMagnitude;
            num_mutChangeCount.Value = settings.MaxMutChangeCount;

            txt_infoFile.Text = settings.InfoFile;
            txt_initSolution.Text = settings.Favorite;

            txt_customSpinners.Text = settings.CustomSpinnerNames;

            cbx_avoidWalls.Checked = settings.AvoidWalls;
            cbx_enableSteepTurns.Checked = settings.EnableSteepTurns;

            txt_customHitboxes.Lines = settings.ManualHitboxes;
            txt_checkpoints.Lines = settings.Checkpoints;

            cbx_timingTestFavDirectly.Checked = settings.TimingTestFavDirectly;
            cbx_frameBasedOnly.Checked = settings.FrameBasedOnly;
            num_gensPerTiming.Value = settings.GensPerTiming;

            num_shuffleCount.Value = settings.ShuffleCount;

            num_maxThreadCount.Value = settings.MaxThreadCount;
        }

        public void SaveSettings()
        {
            // put information into the settings object
            settings.InfoFile = txt_infoFile.Text;
            settings.Favorite = txt_initSolution?.Text ?? null;

            settings.Generations = (int)num_generations.Value;
            settings.Population = (int)num_population.Value;
            settings.SurvivorCount = (int)num_survivorCount.Value;
            settings.Framecount = (int)num_framecount.Value;

            settings.MutationMagnitude = (float)num_mutationMagnitude.Value;
            settings.MaxMutChangeCount = (int)num_mutChangeCount.Value;

            settings.CustomSpinnerNames = txt_customSpinners.Text;

            settings.AvoidWalls = cbx_avoidWalls.Checked;
            settings.EnableSteepTurns = cbx_enableSteepTurns.Checked;

            settings.ManualHitboxes = txt_customHitboxes.Lines;
            settings.Checkpoints = txt_checkpoints.Lines;

            settings.TimingTestFavDirectly = cbx_timingTestFavDirectly.Checked;
            settings.FrameBasedOnly = cbx_frameBasedOnly.Checked;
            settings.GensPerTiming = (int)num_gensPerTiming.Value;

            settings.ShuffleCount = (int)num_shuffleCount.Value;

            settings.MaxThreadCount = (int)num_maxThreadCount.Value;

            // reset the config file and serialize
            configFile.SetLength(0);
            new XmlSerializer(typeof(Settings)).Serialize(configFile, settings);
        }

        private string GetCustomSpinnerNames() => Regex.Matches(txt_customSpinners.Text, @"\S+")
            .Aggregate("", (res, m) => res + $"{{{m.Value}.Position}}");
    }

    public class Settings
    {
        public string InfoFile;

        public string Favorite;
        public int Framecount = 120;

        public int Population = 50;
        public int Generations = 5000;
        public int SurvivorCount = 20;

        public float CrossoverProbability = 1;
        public float MutationProbability = 2;
        public float SimplificationProbability = 1;

        public float MutationMagnitude = 8;
        public int MaxMutChangeCount = 5;

        public string[] Checkpoints;

        public string CustomSpinnerNames;

        public bool AvoidWalls = true;
        public bool EnableSteepTurns = false;

        public string[] ManualHitboxes;

        public bool FrameBasedOnly = false;
        public bool TimingTestFavDirectly = false;
        public int GensPerTiming = 150;

        public int ShuffleCount = 2;

        public int MaxThreadCount;

        public Settings Copy() => (Settings)MemberwiseClone();

        public Settings()
        {
            MaxThreadCount = Environment.ProcessorCount;
        }
    }
}