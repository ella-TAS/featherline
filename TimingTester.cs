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

        private double SourceFrameIndFitness;

        private int mostRecentImprovement = -1;

        public void Run()
        {
            WriteColor("\nStarting up timing tester.\n\n", Yellow);

            if (best.timings.Length == 0) {
                if (settings.TimingTestFavDirectly)
                {
                    best.ind.fitness = 69420.12345;
                    Console.WriteLine("why are you using timing tester with zero timings lol");
                    return;
                }

                EndDueToZeroTimings();
                return;
            }

            if (!settings.TimingTestFavDirectly) {
                GAManager.lastPhase = AlgPhase.TimingTesterLight;
                var firstTiming = UseAlgOnTiming(best.timings, best.ind.angles, settings.GensPerTiming);
                best = new SavedTiming(firstTiming, best.timings);
                Console.WriteLine("\n");
                AddCandidate(AlgPhase.TimingTesterLight);
                if (GAManager.abortAlgorithm) return;

                CleanupAndReset(); // light cleanup
                if (best.timings.Length == 0) { EndDueToZeroTimings(); return; }

                TestUntilNoImprovement(settings.GensPerTiming / 4, false, 1); // phase 1 general testing
                AddCandidate(AlgPhase.TimingTesterLight);

                WriteColor("Increasing generation count per timing.\n\n", Yellow);

                TestTiming(best.timings, best.ind.angles, settings.GensPerTiming, -1);
                AddCandidate(AlgPhase.TimingTesterHeavy);
                if (GAManager.abortAlgorithm) return;

                //CleanupAndReset();
                RemoveUnnecessaryLinesUsingSim();
                WriteColor($"Light input cleanup. Down to {best.ind.angles.Length} lines of input.\n\n", Yellow); // light cleanup
                if (best.timings.Length == 0) { EndDueToZeroTimings(); return; }

                GAManager.lastPhase = AlgPhase.TimingTesterHeavy;
                TestUntilNoImprovement(settings.GensPerTiming, true, 2); // phase 2 intermediate testing
                AddCandidate(AlgPhase.TimingTesterHeavy);
                if (GAManager.abortAlgorithm) return;
                Level.PermanentDistFilter(best.inputs);

                TestTiming(best.timings, best.ind.angles, settings.GensPerTiming, -1);

                RemoveUnnecessaryLinesUsingSim();
                WriteColor($"Strict logical input cleanup. Down to {best.ind.angles.Length} lines of input.\n\n", Yellow); // strict cleanup
                if (best.timings.Length == 0) { EndDueToZeroTimings(); return; }

                WriteColor("Optimizing cleaned inputs.\n\n", Yellow);
                TestUntilNoImprovement(settings.GensPerTiming, false, 0); // phase 3 more intermediate testing
                AddCandidate(AlgPhase.TimingTesterHeavy);
                if (GAManager.abortAlgorithm) return;

                TestTiming(best.timings, best.ind.angles, settings.GensPerTiming, -1);

                RemoveUnnecessaryLinesUsingSim();
                WriteColor($"Final input cleanup. Down to {best.ind.angles.Length} lines of input.\n\n", Yellow); // last strict cleanup
                if (best.timings.Length == 0) { EndDueToZeroTimings(); return; }
                best.ind = FixInd(best.ind, best.timings, settings.GensPerTiming * 3);
            }

            perfecting = true;
            best.ind.fitness = -99999d;
            current = best.Clone();
            GAManager.lastPhase = AlgPhase.AnglePerfector;

            TestPerfectTiming(best.timings, precision, 0);
            TestUntilNoImprovement(0, true, 0);
            AddCandidate(AlgPhase.AnglePerfector);

            Level.PermanentDistFilter(best.inputs);

            if (settings.ShuffleCount > 0)
                ShufflingProcess();
        }

        #region Feedback

        private void WriteColor(string msg, ConsoleColor col)
        {
            Console.ForegroundColor = col;
            Console.Write(msg);
            Console.ForegroundColor = White;
        }

        private void EndDueToZeroTimings() => Console.WriteLine("oh god 0 timings this wont do");

        private void PrintTimings(int[] timings, int currentIndex)
        {
            for (int i = 0; i < timings.Length; i++) {
                var color = i == currentIndex ? Blue : i == mostRecentImprovement ? Green : Gray;
                WriteColor(" " + timings[i].ToString(), color);
            }
        }

        private void AddCandidate(AlgPhase src) => GAManager.finalResultCandidates.Add((best.ind.ToFrameGenes(settings.Framecount, best.timings), best.ind.fitness, src));

        #endregion

        #region TimingTestingLogic

        private void TestUntilNoImprovement(int gensPerTest, bool backtrackOnImprovement, int maxImprCountWhereBreak)
        {
            mostRecentImprovement = -1;
            testedSinceImprovement = new List<SingleTiming>();
            while (TestEveryTimingChange(gensPerTest, backtrackOnImprovement) > maxImprCountWhereBreak) { }

            WriteColor($"\nBest fitness of this phase was {best.ind.fitness.FitnessFormat()} with these inputs:\n\n", Yellow);
            var inputs = best.ind.ToFrameGenes(settings.Framecount, best.timings);
            new FeatherSim(settings).AddInputCleaner(false).SimulateIndivitual(inputs).Evaluate(out _, out int fCount);
            Console.WriteLine(inputs.ToString(fCount) + "\n");
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

                // predicted improvements
                if (perfecting) {
                    while (true) {
                        bool noImprovement = true;
                        for (int j = 0; j < Math.Min(best.timings.Length, Stop()); j++) {
                            if (GAManager.abortAlgorithm) return 0;
                            if (current.ind.borders[j] <= 0) {
                                change = new TimingChange(j, 1);
                                if (change.ApplyTo(current.timings, testedSinceImprovement)) {
                                    testedSinceImprovement.Add(new SingleTiming(j, current.timings[j]));
                                    Console.WriteLine("predicted improvement " + change);
                                    current.ind.borders[j] = AnglePerfector.MaxFrameSteer;
                                    if (TestPerfectTiming(current.timings, precision, j)) {
                                        mostRecentImprovement = j;
                                        testedSinceImprovement.Clear();
                                        noImprovement = false;
                                        current = best.Clone();
                                        break;
                                    }
                                    else {
                                        change.TakeBack();
                                        current.ind.borders[j] = 0f;
                                    }
                                }
                            }
                            else if (current.ind.borders[j] >= AnglePerfector.MaxFrameSteer) {
                                change = new TimingChange(j, -1);
                                if (change.ApplyTo(current.timings, testedSinceImprovement)) {
                                    testedSinceImprovement.Add(new SingleTiming(j, current.timings[j]));
                                    Console.WriteLine("predicted improvement " + change);
                                    current.ind.borders[j] = 0f;
                                    if (TestPerfectTiming(current.timings, precision, j)) {
                                        mostRecentImprovement = j;
                                        testedSinceImprovement.Clear();
                                        noImprovement = false;
                                        current = best.Clone();
                                        break;
                                    }
                                    else {
                                        change.TakeBack();
                                        current.ind.borders[j] = AnglePerfector.MaxFrameSteer;
                                    }
                                }
                            }
                        }
                        if (noImprovement) break;
                    }
                }
                if (GAManager.abortAlgorithm) return 0;

                // regular timing testing
                change = new TimingChange(i, -1);
                if (change.ApplyTo(current.timings, testedSinceImprovement)) {
                    testedSinceImprovement.Add(new SingleTiming(i, current.timings[i]));
                    if (ThisTimingChange(i)) {
                        mostRecentImprovement = i;
                        improvementCount++;

                        if (backtrackOnImprovement)
                            i = Math.Max(-1, i - 2);

                        testedSinceImprovement.Clear();

                        continue;
                    }
                    else change.TakeBack();
                }
                if (GAManager.abortAlgorithm) return 0;

                if (change.ApplyInverse(testedSinceImprovement)) {
                    testedSinceImprovement.Add(new SingleTiming(i, current.timings[i]));
                    if (ThisTimingChange(i)) {
                        mostRecentImprovement = i;
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

            bool ThisTimingChangeRandom(int i) => TestTiming(current.timings, current.ind.angles, gensPerTest, i);
            bool ThisTimingChangePerfect(int i) => TestPerfectTiming(current.timings, precision, i);
        }

        // returns whether the tested timing was faster
        private bool TestTiming(int[] timings, AngleSet angles, int generations, int changedAt)
        {
            Console.Write(testingTimingPrefix + "Testing Timings:");
            PrintTimings(timings, changedAt);
            Console.WriteLine();

            var algResult = UseAlgOnTiming(timings, angles, generations);

            if (algResult.fitness > best.ind.fitness) {
                WriteColor("\nTiming was faster. Setting it as new best.\n\n", Green);

                best = new SavedTiming(algResult, current.timings);

                return true;
            }

            WriteColor("\nTiming was slower.\n\n", Red);

            return false;
        }

        private SavedTiming previousBaseInfoSrc;
        private bool TestPerfectTiming(int[] turnPoints, float precision, int changedAt)
        {
            Console.Write(testingTimingPrefix + "Testing Timings:");
            PrintTimings(turnPoints, changedAt);
            Console.WriteLine();

            var baseInfoSrc = backup != null && backup.ind.fitness > best.ind.fitness ? backup : best;
            if (baseInfoSrc != previousBaseInfoSrc) {
                AnglePerfector.baseInfo = new FeatherSim(settings).GetAllFrameData(
                    baseInfoSrc.ind.ToFrameGenes(settings.Framecount, baseInfoSrc.timings)
                    , out var finishes, out var wallboops);

                AnglePerfector.baseInfoFinishes = finishes;
                AnglePerfector.baseInfoWallboops = wallboops;
                AnglePerfector.baseInfoFitness = baseInfoSrc.ind.fitness;
                previousBaseInfoSrc = baseInfoSrc;
            }

            var algResult = new AnglePerfector(current, precision, changedAt, settings).Run();

            if (algResult.fitness > best.ind.fitness && !current.timings.SequenceEqual(best.timings)) {
                WriteColor("\nTiming was faster. Setting it as new best.\n\n", Green);

                //previousBestTimings = best.timings;
                best = new SavedTiming(algResult, current.timings);

                return true;
            }

            WriteColor("\nTiming was slower.\n\n", Red);

            return false;
        }

        private LineInd UseAlgOnTiming(int[] timings, AngleSet angles, int generations)
        {
            var ga = new LineGenesGA(settings, timings, angles);

            int gen = 1;

            int firstHalf = generations / 3;
            int secondHalf = generations - firstHalf;

            for (int i = 0; i < firstHalf; i++) {
                if (GAManager.abortAlgorithm) return ga.inds[0];
                ga.DoGeneration();
                GAManager.GenerationFeedback(gen, generations, ga.inds[0].fitness);
                gen++;
            }

            if (ga.inds[0].fitness + 200d < Math.Max(
                    SourceFrameIndFitness, Math.Max(
                    best.ind.fitness,
                    backup?.ind.fitness ?? -99999d))) {
                Console.WriteLine("\nBad timing result. Attempting to fix them.");
                ga = new LineGenesGA(settings, timings,
                    FixInd(ga.inds[0], timings, secondHalf).angles);
            }

            for (int i = 0; i < secondHalf; i++) {
                if (GAManager.abortAlgorithm) return ga.inds[0];
                ga.DoGeneration();
                GAManager.GenerationFeedback(gen, generations, ga.inds[0].fitness);
                gen++;
            }

            return ga.inds[0];
        }

        public static LineInd FixInd(LineInd toFix, int[] timings, int gens)
        {
            int lines = timings.Length + 1;
            var avgGens = (float)gens / lines;
            var lowerBound = avgGens * 0.5f;
            var upperBound = avgGens * 1.5f;

            var lineGens = Enumerable.Range(0, lines).Select(i => (int)Math.Round(
                lowerBound + (upperBound - lowerBound) * i / (lines - 1)
                )).ToArray();

            int sum = lineGens.Sum();
            int increment = sum < gens ? 1 : -1;
            while (true)
                for (int i = 0; i < lineGens.Length; i++) {
                    if (sum == gens)
                        goto Stop;
                    lineGens[i] += increment;
                    sum += increment;
                }
            Stop:

            var ga = new LineGenesGA(GAManager.settings, timings, toFix.angles);
            for (int t = 0; t < lines; t++)
                for (int gen = 0; gen < lineGens[t]; gen++) {
                    if (GAManager.abortAlgorithm) return ga.inds[0];
                    ga.DoGeneration(0, t);
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
        private void RemoveUnnecessaryLinesUsingSim(bool keepExact = false)
        {
            var anglesBackup = best.ind.angles.Clone();

            var unnecessaryLines = new List<int>();
            var timingStates = GetTurnpointStates(out var wallboops);

            best.ind.SkippingState = null;

            int firstInChain = 0;
            for (int i = 1; i < best.ind.angles.Length; i++) {
                for (int j = firstInChain; j < i; j++)
                    best.ind.angles[j] = best.ind.angles[i];

                var sim = new FeatherSim(settings);
                var (dead, state, boops) = sim.LineIndInfoAtFrame(best.ind,
                    i == best.timings.Length ? best.inputs.Length : best.timings[i], best.timings,
                    (stop, fs, ws) =>
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
                    && ValidWallboops(wallboops, boops, new IntRange(0, i == best.timings.Length ? settings.Framecount : best.timings[i]))
                    && (!keepExact || state.fState.ExactPosition == timingStates[i].fState.ExactPosition);
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
        public static bool ValidWallboops(int[] goodBoops, IEnumerable<int> newBoops, IntRange range)
        {
            if (!newBoops.All(nb => !goodBoops.All(gb => Math.Abs(nb - gb) > 2)))
                return false;

            /*if (newBoopRange is null)
                return true;*/

            var filtGBs = goodBoops.Where(gb => Math.Max(range.start - gb, gb - range.end) <= 2);
            return filtGBs.All(gb => !newBoops.All(nb => Math.Abs(gb - nb) > 2));
        }

        #endregion

        #region Shuffling

        private void ShufflingProcess()
        {
            bool shuffleDir = true;
            WriteColor("\nBeginning shuffling process.\n\n", Blue);

            backup = best.Clone();
            RemoveUnnecessaryLinesUsingSim();

            for (int i = 0; i < settings.ShuffleCount; i++) {
                double fitnessToBeat = Math.Max(best.ind.fitness, backup?.ind.fitness ?? -99999d);
                testingTimingPrefix = $"(shuffle {i + 1} vs fitness {fitnessToBeat.FitnessFormat()}) ";

                int strength = 3 - (int)Math.Floor(i / (float)Math.Max(6, settings.ShuffleCount) * 3);

                ShuffleTimings(shuffleDir, strength, fitnessToBeat);
                if (GAManager.abortAlgorithm) break;

                shuffleDir = !shuffleDir;
            }

            if (backup.ind.fitness >= best.ind.fitness)
                best = backup;
        }

        bool[] shuffleImprovements;
        bool previousShuffleFaster = false;
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
            TestUntilNoImprovement(settings.GensPerTiming, true, 0);
            //TestTiming(best.timings, best.ind.angles, 0);

            GetShuffleImprovements(positiveOnEvenIndex);

            if (best.ind.fitness >= toBeat) {
                WriteColor("\nShuffle result was faster.\n\n\n", Green);
                AddCandidate(AlgPhase.AnglePerfector);
                previousShuffleFaster = true;
                return;
            }

            WriteColor("\nShuffle result was slower.\n\n\n", Red);
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

        public TimingTester(Settings settings, FrameInd ind)
        {
            this.settings = settings;

            WriteColor($"\nBest inputs of the first phase were:\n\n", Yellow);
            var inputs = ind.genes;
            new FeatherSim(settings).SimulateIndivitual(inputs).AddInputCleaner(true).Evaluate(out _, out int fCount);
            Console.WriteLine(inputs.ToString(fCount));

            SavedTiming.sett = settings;

            //Console.WriteLine("\n" + GAManager.FrameGenesToString(src.genes));
            var simplified = SimplifyFrameGenes(ind.genes);
            //Console.WriteLine("\n" + GAManager.FrameGenesToString(simplified));

            best = GenesToRawLineInd(simplified);

            SourceFrameIndFitness = ind.fitness;
        }

        public TimingTester(Settings settings, string favorite)
        {
            this.settings = settings;

            var inputs = GAManager.RawFavorite(favorite);
            settings.Framecount = Math.Max(inputs.Length, settings.Framecount);

            var sim = new FeatherSim(settings);
            AnglePerfector.baseInfo = sim.GetAllFrameData(inputs, out AnglePerfector.baseInfoFinishes, out AnglePerfector.baseInfoWallboops);
            sim.Evaluate(out AnglePerfector.baseInfoFitness, out _);

            SavedTiming.sett = settings;

            best = GenesToRawLineInd(inputs);
            RemoveUnnecessaryLinesUsingSim(true);

            sim = new FeatherSim(settings);
            AnglePerfector.baseInfo = sim.GetAllFrameData(best.inputs, out AnglePerfector.baseInfoFinishes, out AnglePerfector.baseInfoWallboops);
            sim.Evaluate(out AnglePerfector.baseInfoFitness, out _);
            var test = 1;
        }

        private SavedTiming GenesToRawLineInd(AngleSet inputs)
        {
            var anglesLs = new List<float>();
            var timingLs = new List<int>();

            int lastTiming = 0;
            foreach (var line in inputs.GroupConsecutive()) {
                anglesLs.Add(line.value);
                lastTiming += line.count;
                timingLs.Add(lastTiming);
            }
            timingLs.RemoveAt(timingLs.Count - 1);

            var borderLs = new List<float>();
            borderLs.Add(0f);
            for (int i = 1; i < timingLs.Count; i++) {
                if (timingLs[i] - timingLs[i - 1] == 1) {
                    var diff = anglesLs[i] - anglesLs[i - 1];
                    var diff2 = anglesLs[i + 1] - anglesLs[i - 1];
                    if (Math.Abs(diff) < 5.333f && Math.Abs(diff2) > 5.334f && (diff < 0) == (diff2 < 0)) {
                        anglesLs.RemoveAt(i);
                        borderLs[--i] = Math.Abs(diff);
                        timingLs.RemoveAt(i);
                        continue;
                    }
                }
                borderLs.Add(0f);
            }

            return new SavedTiming(new AngleSet(anglesLs), new AngleSet(borderLs), timingLs.ToArray());
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
                    var ContinueTurning = angDif[i] < 5 ? (Func<bool>)(() => angDif[++i] < -5) : (() => angDif[++i] > 5);
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