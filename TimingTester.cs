using System.Linq;
using System.Collections.Generic;
using System;
using static System.ConsoleColor;

namespace Featherline
{
    public class TimingTester
    {
        public Settings settings;

        private SavedTiming best;
        private SavedTiming current;

        private SavedTiming backup;

        private string testingTimingPrefix = "";

        public void Run()
        {
            WriteColor("\nStarting up timing tester.", Yellow);

            if (best.timings.Length == 0) {
                if (settings.TimingTestFavDirectly)
                {
                    best.ind.fitness = 69420.12345;
                    PrintResult();
                    Console.WriteLine("why are you using timing tester with zero timings lol");
                    return;
                }

                EndDueToZeroTimings();
                return;
            }

            if (!settings.TimingTestFavDirectly) {
                var firstTiming = UseAlgOnTiming(best.timings, best.ind.angles, settings.GensPerTiming);
                best = new SavedTiming(firstTiming, best.timings);
                Console.WriteLine("\n");

                CleanupAndReset(); // light cleanup
                if (best.timings.Length == 0) { EndDueToZeroTimings(); return; }

                TestUntilNoImprovement(settings.GensPerTiming / 4, false, 1); // phase 1 general testing

                WriteColor("Increasing generation count per timing.\n", Yellow);

                TestTiming(best.timings, best.ind.angles, settings.GensPerTiming);

                CleanupAndReset();
                WriteColor($"Light input cleanup. Down to {best.ind.angles.Length} lines of input.\n", Yellow); // light cleanup
                if (best.timings.Length == 0) { EndDueToZeroTimings(); return; }

                TestUntilNoImprovement(settings.GensPerTiming, true, 2); // phase 2 intermediate testing

                TestTiming(best.timings, best.ind.angles, settings.GensPerTiming);

                RemoveUnnecessaryLinesUsingSim();
                WriteColor($"Strict logical input cleanup. Down to {best.ind.angles.Length} lines of input.\n", Yellow); // strict cleanup
                if (best.timings.Length == 0) { EndDueToZeroTimings(); return; }

                WriteColor("Approximately optimizing cleaned inputs.\n", Yellow);
                TestUntilNoImprovement(settings.GensPerTiming, false, 0); // phase 3 more intermediate testing

                TestTiming(best.timings, best.ind.angles, settings.GensPerTiming);

                RemoveUnnecessaryLinesUsingSim();
                WriteColor($"Final input cleanup. Down to {best.ind.angles.Length} lines of input.\n", Yellow); // last strict cleanup
                if (best.timings.Length == 0) { EndDueToZeroTimings(); return; }
            }

            perfecting = true;
            best.ind.fitness = -99999;
            current = best.Clone();

            TestPerfectTiming(best.timings, precision, 0);
            TestUntilNoImprovement(0, false, 0);

            if (settings.ShuffleCount > 0)
                ShufflingProcess();

            PrintResult();
        }

        #region Feedback

        private void PrintResult()
        {
            WriteColor($"\nFinished with fitness {best.ind.fitness}\n", Yellow);
            new FeatherSim(settings).SimulateIndivitual(best.inputs, true);
            Console.WriteLine(GAManager.FrameGenesToString(best.inputs));
        }

        private void WriteColor(string msg, ConsoleColor col)
        {
            Console.ForegroundColor = col;
            Console.WriteLine(msg);
            Console.ForegroundColor = White;
        }

        private void EndDueToZeroTimings()
        {
            Console.WriteLine("oh god 0 timings this wont do");
            PrintResult();
        }

        #endregion

        #region TimingTestingLogic

        private void TestUntilNoImprovement(int gensPerTest, bool backtrackOnImprovement, int maxImprCountWhereBreak)
        {
            testedSinceImprovement = new List<SingleTiming>();
            while (TestEveryTimingChange(gensPerTest, backtrackOnImprovement) > maxImprCountWhereBreak) { }
        }

        bool perfecting = false;
        float precision = 0.001f;
        List<SingleTiming> testedSinceImprovement;
        // returns amount of improvements found
        private int TestEveryTimingChange(int gensPerTest, bool backtrackOnImprovement)
        {
            var ThisTimingChange = perfecting ? (Func<int, bool>)ThisTimingChangePerfect : ThisTimingChangeRandom;
            int improvementCount = 0;

            int Stop() => best.ind is null
                ? 99999
                : best.timings.TakeWhile(t => t < (best.ind.states?.Length ?? 99999)).Count() + 1;
            for (int i = 0; i < Math.Min(best.timings.Length, Stop()); i++) {
                current = best.Clone();


                TimingChange change;

                if (perfecting) {
                    while (true) {
                        bool noImprovement = true;
                        for (int j = 0; j < Math.Min(best.timings.Length, Stop()); j++) {
                            if (current.ind.borders[j] <= 0) {
                                change = new TimingChange(j, 1);
                                if (change.ApplyTo(current.timings, testedSinceImprovement)) {
                                    testedSinceImprovement.Add(new SingleTiming(j, current.timings[j]));
                                    Console.WriteLine("predicted improvement " + change);
                                    current.ind.borders[j] = AnglePerfector.MaxFrameSteer;
                                    if (TestPerfectTiming(current.timings, precision, j)) {
                                        testedSinceImprovement.Clear();
                                        noImprovement = false;
                                        current = best.Clone();
                                        break;
                                    }
                                    else current.ind.borders[j] = 0f;
                                }
                            }
                            else if (current.ind.borders[j] >= AnglePerfector.MaxFrameSteer) {
                                change = new TimingChange(j, -1);
                                if (change.ApplyTo(current.timings, testedSinceImprovement)) {
                                    testedSinceImprovement.Add(new SingleTiming(j, current.timings[j]));
                                    Console.WriteLine("predicted improvement " + change);
                                    current.ind.borders[j] = 0f;
                                    if (TestPerfectTiming(current.timings, precision, j)) {
                                        testedSinceImprovement.Clear();
                                        noImprovement = false;
                                        current = best.Clone();
                                        break;
                                    }
                                    else current.ind.borders[j] = AnglePerfector.MaxFrameSteer;
                                }
                            }
                        }
                        if (noImprovement) break;
                    }
                }

                change = new TimingChange(i, -1);
                if (change.ApplyTo(current.timings, testedSinceImprovement)) {
                    testedSinceImprovement.Add(new SingleTiming(i, current.timings[i]));
                    if (ThisTimingChange(i)) {
                        improvementCount++;

                        if (backtrackOnImprovement)
                            i = Math.Max(-1, i - 2);

                        testedSinceImprovement.Clear();

                        continue;
                    }
                    else change.TakeBack();
                }

                if (change.ApplyInverse(testedSinceImprovement)) {
                    testedSinceImprovement.Add(new SingleTiming(i, current.timings[i]));
                    if (ThisTimingChange(i)) {
                        improvementCount++;

                        if (backtrackOnImprovement)
                            i = Math.Max(-1, i - 2);

                        testedSinceImprovement.Clear();

                        continue;
                    }
                    else change.TakeBack();
                }
            }

            return improvementCount;

            bool ThisTimingChangeRandom(int i) => TestTiming(current.timings, current.ind.angles, gensPerTest);
            bool ThisTimingChangePerfect(int i) => TestPerfectTiming(current.timings, precision, i);
        }

        // returns whether the tested timing was faster
        private bool TestTiming(int[] turnPoints, AngleSet angles, int generations)
        {
            Console.WriteLine(testingTimingPrefix + "Testing Timing: " + string.Join(", ", turnPoints.Select(tp => tp.ToString()).ToArray()));

            var algResult = UseAlgOnTiming(turnPoints, angles, generations);

            var inputs = algResult.ToFrameGenes(settings.Framecount, turnPoints);
            new FeatherSim(settings).SimulateIndivitual(inputs, true);
            Console.WriteLine("\n" + GAManager.FrameGenesToString(inputs));

            if (algResult.fitness > best.ind.fitness) {
                WriteColor("\nTiming was faster. Setting it as new best.\n", Green);

                best = new SavedTiming(algResult, current.timings);

                return true;
            }

            WriteColor("\nTiming was slower.\n", Red);

            return false;
        }

        private SavedTiming previousBaseInfoSrc;
        private bool TestPerfectTiming(int[] turnPoints, float precision, int changedAt)
        {
            Console.WriteLine(testingTimingPrefix + "Testing Timing: " + string.Join(", ", turnPoints.Select(tp => tp.ToString()).ToArray()));

            var baseInfoSrc = backup != null && backup.ind.fitness > best.ind.fitness ? backup : best;
            if (baseInfoSrc != previousBaseInfoSrc) {
                AnglePerfector.baseInfo = new FeatherSim(settings).GetAllFrameData(
                    baseInfoSrc.ind.ToFrameGenes(settings.Framecount, baseInfoSrc.timings)
                    , out var finishes, out var wallboops);

                AnglePerfector.baseInfoFinishes = finishes;
                AnglePerfector.baseInfoWallboops = wallboops;
                previousBaseInfoSrc = baseInfoSrc;
            }

            var algResult = new AnglePerfector(current, precision, changedAt, settings).Run();

            if (algResult.fitness > best.ind.fitness && !current.timings.SequenceEqual(best.timings)) {
                WriteColor("\nTiming was faster. Setting it as new best.\n", Green);

                //previousBestTimings = best.timings;
                best = new SavedTiming(algResult, current.timings);

                return true;
            }

            WriteColor("\nTiming was slower.\n", Red);

            return false;
        }

        private LineInd UseAlgOnTiming(int[] turnPoints, AngleSet angles, int generations)
        {
            var ga = new LineGenesGA(settings, turnPoints, angles);

            for (int i = 1; i <= generations; i++)
            {
                ga.DoGeneration();
                Console.Write($"\r{i}/{generations} generations done. Best fitness: {ga.inds[0].fitness}                ");
            }

            return ga.inds[0];
        }

        #endregion

        #region InputCleanup

        private void CleanupAndReset()
        {
            CleanUpCurrentInputLines();
            best.ind.fitness = -99999;
        }

        private void CleanUpCurrentInputLines()
        {
            var timings = best.timings.ToList();
            var angles = best.ind.angles.ToList();
            var borderExtras = best.ind.borders.ToList();

            for (int i = 1; i < timings.Count;) {
                if (Math.Abs(angles[i] - angles[i + 1]) < 1f) {
                    timings.RemoveAt(i);
                    angles.RemoveAt(i + 1);
                    borderExtras.RemoveAt(i);
                }
                else if (timings[i] - timings[i - 1] == 1) {
                    timings.RemoveAt(i);
                    angles.RemoveAt(i + 1);
                    borderExtras.RemoveAt(i);
                }
                else
                    i++;
            }

            best.timings = timings.ToArray();
            best.ind.angles = new AngleSet(angles);
            best.ind.borders = new AngleSet(borderExtras);
        }

        // a line is useless if you can set its angle to the angle of the line after it,
        // and you dont die or miss a checkpoint during either of those two lines
        private void RemoveUnnecessaryLinesUsingSim()
        {
            var anglesBackup = best.ind.angles.Clone();

            var unnecessaryLines = new List<int>();
            var timingStates = GetTurnpointStates(out var wallboops);

            int firstInChain = 0;
            for (int i = 1; i < best.ind.angles.Length; i++) {
                for (int j = firstInChain; j < i; j++)
                    best.ind.angles[j] = best.ind.angles[i];

                var sim = new FeatherSim(settings);
                var (dead, state, boops) = sim.LineIndInfoAtFrame(best.ind,
                    i == best.timings.Length ? best.inputs.Length : best.timings[i], best.timings,
                    (bool stop, FeatherSim.FState fs, WindState ws) =>
                    (stop && fs.checkpointsGotten < Level.Checkpoints.Length, new Savestate(fs, ws), sim.wallboops));

                if (NoDifference())
                    unnecessaryLines.Add(i);
                else {
                    firstInChain = i;
                    anglesBackup.CopyTo(best.ind.angles);
                }

                bool NoDifference() =>
                    !dead
                    && timingStates[i] != null
                    && state.fState.checkpointsGotten == timingStates[i].fState.checkpointsGotten
                    && ValidWallboops(wallboops, boops);
            }

            var timings = best.timings.ToList();
            var borderExtras = best.ind.borders.ToList();
            var angles = best.ind.angles.ToList();

            foreach (int t in unnecessaryLines.AsEnumerable().Reverse()) {
                timings.RemoveAt(t - 1);
                borderExtras.RemoveAt(t - 1);
                angles.RemoveAt(t);
            }

            best.timings = timings.ToArray();
            best.ind.borders = new AngleSet(borderExtras);
            best.ind.angles = new AngleSet(angles);

            best.ind.fitness = -99999;

            Savestate[] GetTurnpointStates(out int[] wallboops)
            {
                var all = new FeatherSim(settings).GetAllFrameData(best.inputs, out _, out wallboops);
                return best.timings.Select(t => t <= all.Length ? all[t - 1] : null).Append(all.Last()).ToArray();
            }
        }

        // returns whether all of the new boops are so at least one of the approved boops is close to it
        public static bool ValidWallboops(int[] goodBoops, IEnumerable<int> newBoops)
        {
            return newBoops.All(nb => !goodBoops.All(gb => Math.Abs(nb - gb) > 2));
        }

        #endregion

        #region Shuffling

        private void ShufflingProcess()
        {
            bool shuffleDir = true;
            WriteColor("\nBeginning shuffling process.\n", Blue);

            for (int i = 0; i < settings.ShuffleCount; i++) {
                double fitnessToBeat = Math.Max(best.ind.fitness, backup?.ind.fitness ?? 0);
                testingTimingPrefix = $"(shuffle {i + 1} vs fitness {fitnessToBeat}) ";

                int strength = 3 - (int)Math.Floor(i / (float)Math.Max(6, settings.ShuffleCount) * 3);

                ShuffleTimings(shuffleDir, strength, fitnessToBeat);

                shuffleDir = !shuffleDir;
            }

            if (backup.ind.fitness >= best.ind.fitness)
                best = backup;
        }

        bool[] shuffleImprovements;
        bool previousShuffleFaster = true;
        private void ShuffleTimings(bool positiveOnEvenIndex, int strength, double toBeat)
        {
            if (previousShuffleFaster) backup = best.Clone();
            else strength = Math.Max(2, strength);
            current = best;

            for (int i = 0; i < best.timings.Length; i++) {
                if (shuffleImprovements is null || shuffleImprovements[i] ^ previousShuffleFaster) {
                    new TimingChange(i, (i & 1) == (positiveOnEvenIndex ? 0 : 1) ? strength : -strength)
                        .ApplyTo(best.timings);
                }
            }
            best.ind.fitness = -9999999;
            best.ind.borders = new AngleSet(best.ind.borders.Length);

            //TestTiming(best.timings, best.ind.angles, settings.GensPerTiming);
            TestUntilNoImprovement(settings.GensPerTiming, false, 0);
            //TestTiming(best.timings, best.ind.angles, 0);

            new FeatherSim(settings).SimulateIndivitual(best.inputs, true);
            Console.WriteLine(GAManager.FrameGenesToString(best.inputs));

            GetShuffleImprovements(positiveOnEvenIndex);

            if (best.ind.fitness >= toBeat) {
                WriteColor("\nShuffle result was faster.\n\n", Green);
                previousShuffleFaster = true;
                return;
            }

            WriteColor("\nShuffle result was slower.\n\n", Red);
            previousShuffleFaster = false;
        }

        private void GetShuffleImprovements(bool positOnEven)
        {
            shuffleImprovements = new bool[best.timings.Length];
            for (int i = 0; i < shuffleImprovements.Length; i++) {
                bool indexIsEven = (i & 1) == 0;
                bool shuffledToLess = indexIsEven ^ positOnEven;

                shuffleImprovements[i] = shuffledToLess
                    ? best.timings[i] < backup.timings[i]
                    : best.timings[i] > backup.timings[i];
            }
        }

        #endregion

        #region Constructors

        public TimingTester(Settings settings, AngleSet src)
        {
            this.settings = settings;

            SavedTiming.sett = settings;

            Console.WriteLine("\n" + GAManager.FrameGenesToString(src));
            var simplified = SimplifyFrameGenes(src);
            Console.WriteLine("\n" + GAManager.FrameGenesToString(simplified));

            SetUpFirstIndividual(simplified);
        }

        public TimingTester(Settings settings, string favorite)
        {
            this.settings = settings;

            var frames = GAManager.RawFavorite(favorite);

            SavedTiming.sett = settings;

            SetUpFirstIndividual(frames);
        }

        private void SetUpFirstIndividual(AngleSet frames)
        {
            var anglesLs = new List<float>();
            var timingsLs = new List<int>();

            float currentAngle = frames[0];
            for (int i = 1; i < frames.Length; i++) {
                if (frames[i] != currentAngle) {
                    anglesLs.Add(currentAngle);
                    timingsLs.Add(i);
                    currentAngle = frames[i];
                }
            }
            anglesLs.Add(currentAngle);

            best = new SavedTiming(new AngleSet(anglesLs.ToArray()), new AngleSet(timingsLs.Count), timingsLs.ToArray());
        }

        #endregion

        #region ConvertingFromFrameGenes

        private AngleSet SimplifyFrameGenes(AngleSet src)
        {
            float[] angles = GetFlightAngles(src).Select(a => (float)Math.Round(a, 3)).ToArray();
            float[] angDif = ToAngleDifferences(angles).Select(a => (float)Math.Round(a, 3)).ToArray();

            for (int i = 0; i < angDif.Length; i++) {
                if (Math.Abs(angDif[i]) < 2) {
                    int start = i;
                    var startAngle = angles[i];
                    while (i < angDif.Length - 1 && Math.Abs(FeatherSim.DegreesDiff(angles[++i], startAngle)) < 2) { }
                    for (int j = start; j < i; j++)
                        src[j] = startAngle;
                }

                if (Math.Abs(angDif[i]) > 5) {
                    int start = i;
                    var ContinueTurning = angDif[i] < 5 ? (Func<bool>)(() => angDif[++i] < 5) : (() => angDif[++i] > 5);
                    while (i < angDif.Length - 1 && ContinueTurning()) { }
                    for (int j = start; j < i; j++)
                        src[j] = angles[i - 1];
                }
            }

            return src;
        }

        private float[] GetFlightAngles(AngleSet src)
        {
            var flightStates = new FeatherSim(settings).GetAllFrameData(src, out _, out _);
            return flightStates
                .Select(fs => fs.fState.spd.TASAngle)
                .ToArray();
        }
        
        private float[] ToAngleDifferences(float[] angles)
        {
            var res = new float[angles.Length - 1];
            for (int i = 0; i < res.Length; i++)
                res[i] = FeatherSim.DegreesDiff(angles[i], angles[i + 1]);
            return res;
        }

        #endregion
    }

    public class SavedTiming
    {
        public static Settings sett;

        public LineInd ind;

        public int[] timings;

        public AngleSet inputs;

        // is deep copy
        public SavedTiming(LineInd src, int[] timings)
        {
            ind = new LineInd(src.angles, src.borders);
            ind.fitness = src.fitness;
            this.timings = (int[])timings.Clone();
            inputs = ind.ToFrameGenes(sett.Framecount, timings);
        }

        public SavedTiming(AngleSet angles, AngleSet borderExtras, int[] timings)
            : this(new LineInd(angles, borderExtras), timings) { }

        public SavedTiming Clone() => new SavedTiming(ind, timings);
    }
}