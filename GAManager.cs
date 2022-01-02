using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Featherline
{
    static class GAManager
    {
        public static Settings settings = null;
        public static Random rand = new Random();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static int generation;

        public static void BeginAlgorithm(Form1 sender)
        {
            AllocConsole();
            Console.Title = "Featherline Output Console";
            Console.Clear();

            // data extraction and input exception handling
            try {
                Level.Prepare(Form1.settings);
                sender.SaveSettings();
            }
            catch (ArgumentException e) {
                Console.WriteLine(e.Message);
                return;
            }
            catch {
                Console.WriteLine("The information source directory is either invalid or undefined.");
                return;
            }
            if (Form1.settings.Checkpoints.Length == 0) {
                Console.WriteLine("No valid checkpoints were provided; the algorithm has nothing to aim for.");
                return;
            }
            settings = Form1.settings.Copy();

            //FeatherSim.sett = settings;

            //var test = new FeatherSim(settings).SimulateIndivitual(ParseFavorite(settings.Favorite, 120));
            //Console.ReadLine();

            GA ga = settings.LimitInputLinesMode ? (GA)new LineGenesGA(settings) : new FrameGenesGA(settings);
            generation = 1;

            {
                var fav = RawFavorite(settings.Favorite);
                int gensForIncreasingFrameCount = Math.Max(1, settings.Generations / 2);
                int startAt = Math.Max(5, fav is null ? 0 : fav.Length);
                int gensPerFrame = gensForIncreasingFrameCount / (settings.Framecount - startAt);
                for (int i = startAt; i < settings.Framecount; i++) {
                    for (int j = 0; j < gensPerFrame; j++) {
                        Console.Write($"\r{generation}/{settings.Generations} generations done. Best fitness: {ga.GetBestFitness()}     ");
                        ga.DoGeneration(i);
                        generation++;
                    }
                }
            }

            for (; generation <= settings.Generations; generation++) {
                Console.Write($"\r{generation}/{settings.Generations} generations done. Best fitness: {ga.GetBestFitness()}     ");
                ga.DoGeneration(settings.Framecount);
            }
            Console.WriteLine();

            ga.PrintBestIndividual();

            Level.Spinners = null;
            Level.DeathMap = null;
            Level.Tiles = null;
            Level.windTriggers = null;
        }

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
    }
}
