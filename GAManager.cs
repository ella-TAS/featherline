using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace Featherline
{
    static class GAManager
    {
        public static Settings settings;
        public static Random rand = new Random();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static int generation;

        static FrameGenesGA ga;
        public static void BeginAlgorithm(Form1 sender, bool debugFavorite)
        {
            if (!InitializeAlgorithm(sender)) {
                ClearAlgorithmData();
                return;
            }

            var timer = Stopwatch.StartNew();

            if (debugFavorite) {
                var initGenes = ParseFavorite(settings.Favorite, 120);

                if (initGenes is null)
                    Console.WriteLine("No initial inputs to debug");
                else
                    new FeatherSim(settings).Debug(ParseFavorite(settings.Favorite, 120));

                return;
            }

            if (settings.FrameBasedOnly || !settings.TimingTestFavDirectly) {
                DoFrameGeneBasedAlgorithm();
                Console.WriteLine("\nBasic Algorithm Finished!\n");
            }

            if (!settings.FrameBasedOnly) {
                TimingTester TT;
                if (settings.TimingTestFavDirectly) {
                    if (ParseFavorite(settings.Favorite, settings.Framecount) is null)
                        Console.WriteLine("No initial inputs to test timings on.");
                    else {
                        TT = new TimingTester(settings, settings.Favorite);
                        TT.Run();
                    }
                }
                else {
                    TT = new TimingTester(settings, ga.inds[0].genes);
                    TT.Run();
                }
            }
            else {
                Console.WriteLine();
                ga.PrintBestIndividual();
            }

            timer.Stop();
            Console.WriteLine($"\nAlgorithm took {timer.Elapsed} to run.");

            ClearAlgorithmData();
        }

        private static bool InitializeAlgorithm(Form1 sender)
        {
            AllocConsole();
            Console.Title = "Featherline Output Console";
            Console.Clear();

            // data extraction and input exception handling
            try {
                sender.SaveSettings();
                Level.Prepare(Form1.settings);
            }
            catch (ArgumentException e) {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                Console.WriteLine("\nThe extracted information is either invalid or an exception occured during the setup process otherwise.\nPing TheRoboMan on the Celeste discord if you think this shouldnt have happened.");
                return false;
            }
            finally { }
            if (Level.Checkpoints.Length == 0) {
                Console.WriteLine("No valid checkpoints were provided; the algorithm has nothing to aim for.");
                return false;
            }

            settings = Form1.settings.Copy();
            MyParallel.Initialize(settings);
            return true;
        }

        #region FrameBasedAlgorithm

        private static void DoFrameGeneBasedAlgorithm()
        {
            var fav = RawFavorite(settings.Favorite);
            int startAt = Math.Max(5, fav is null ? 0 : fav.Length);

            ga = new FrameGenesGA(settings, startAt);
            generation = 1;

            DoGensWhileSimulationsGetLonger(startAt);

            NormalGenerations();
        }

        private static void DoGensWhileSimulationsGetLonger(int startAt)
        {
            int gensForIncreasingFrameCount = Math.Max(1, settings.Generations / 2);

            int divisor = settings.Framecount - startAt;
            if (divisor == 0) return;

            int gensPerFrame = gensForIncreasingFrameCount / divisor;
            for (int i = startAt; i < settings.Framecount; i++) {
                for (int j = 0; j < gensPerFrame; j++) {
                    ga.DoGeneration(i, true);
                    Console.Write($"\r{generation}/{settings.Generations} generations done. Best fitness: {ga.GetBestFitness()}     ");
                    generation++;
                }
            }
        }

        private static void NormalGenerations()
        {
            for (; generation <= settings.Generations; generation++) {
                Console.Write($"\r{generation}/{settings.Generations} generations done. Best fitness: {ga.GetBestFitness()}     ");
                ga.DoGeneration(settings.Framecount, true);
            }
        }

        #endregion

        #region InputConverting

        public static float[] ParseFavorite(string src, int targetLen)
        {
            float[] res = RawFavorite(src);

            if (res is null) return null;

            if (res.Length > targetLen)
                res = res.Take(targetLen).ToArray();
            else if (res.Length < targetLen)
                res = res.Concat(new float[targetLen - res.Length].Select(n => (float)(rand.NextDouble() * 360))).ToArray();

            return res;
        }

        public static float[] RawFavorite(string src)
        {
            float[] res = { };

            var matches = Regex.Matches(src, @"(\d+),F,(\d+\.?\d*)");
            if (matches.Count == 0) return null;

            foreach (Match m in matches) {
                int fCount = int.Parse(m.Groups[1].Value);
                var angle = float.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
                res = res.Concat(Enumerable.Repeat(angle, fCount)).ToArray();
            }
            return res;
        }

        public static string FrameGenesToString(float[] inputs)
        {
            var sb = new StringBuilder();

            float lastAngle = inputs[0];
            int consecutive = 1;
            foreach (var f in inputs.Skip(1)) {
                if (f != lastAngle) {
                    sb.AppendLine($"{consecutive,4},F,{lastAngle.ToString().Replace(',', '.')}");
                    lastAngle = f;
                    consecutive = 1;
                }
                else
                    consecutive++;
            }
            sb.AppendLine($"{consecutive,4},F,{lastAngle.ToString().Replace(',', '.')}");

            return sb.ToString();
        }

        #endregion

        private static void ClearAlgorithmData()
        {
            Level.Spinners = null;
            Level.DeathMap = null;
            Level.Tiles = null;
            Level.WindTriggers = null;
            Level.Checkpoints = null;
            Level.Colliders = null;
            Level.Killboxes = null;
            Level.Spikes = null;
            settings = null;
            ga = null;
            AnglePerfector.baseInfo = null;
            GC.Collect();
        }
    }
}
