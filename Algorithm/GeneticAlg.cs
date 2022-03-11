using static System.Math;
using static Featherline.GAManager;

namespace Featherline
{
    public class FrameGenesGA
    {
        public Settings sett;

        private const float crossoverProb = 0.2f;
        private const float mutationProb = 0.8f;
        private float mutationMagnitude;

        public FrameInd[] inds;

        private int UpToFrame;
        private bool UseSavestates;
        public void DoGeneration(int upToFrame, bool useSavestates)
        {
            UpToFrame = Min(upToFrame, inds[0].genes.Length);
            UseSavestates = useSavestates;

            if (useSavestates)
                GetAllStatesOfBest();

            GenerateNewChildren();

            int shortestFrameCount = sett.Framecount;

            MyParallel.Run(sett.SurvivorCount, inds.Length, DoSim);

            inds = inds.OrderByDescending(ind => ind.fitness).ToArray();

            if (shortestFrameCount < sett.Framecount) {
                sett.Framecount = shortestFrameCount;
                foreach (var ind in inds)
                    ind.genes.Resize(shortestFrameCount);
            }

            void DoSim(int i)
            {
                new FeatherSim(sett).AddInputCleaner(true).SimulateIndivitual(inds[i].genes, upToFrame, inds[i].SkippingState)
                    .Evaluate(out var fitness, out int frames);
                inds[i].fitness = fitness;
                shortestFrameCount = Min(shortestFrameCount, frames);
            }
        }

        private void GetAllStatesOfBest()
        {
            for (int i = 0; i < sett.SurvivorCount; i++)
                if (inds[i].fStates is null)
                    inds[i].fStates = new FeatherSim(sett).GetAllFrameData(inds[i].genes, out _, out _);
        }

        #region GeneratingChildren

        public void GenerateNewChildren()
        {
            for (int i = sett.SurvivorCount; i < inds.Length;) {
                float actionChooser = (float)rand.NextDouble();

                // crossover
                if (crossoverProb > actionChooser) {
                    int i1 = rand.Next(sett.SurvivorCount);
                    int i2 = rand.Next(sett.SurvivorCount - 1);
                    i2 += i2 >= i1 ? 1 : 0;

                    var res = Crossover(inds[i1], inds[i2]);
                    inds[i] = res.child1;
                    i++;
                    if (i >= inds.Length) break;
                    inds[i] = res.child2;
                }

                // mutation
                else if (mutationProb > actionChooser) {
                    int parent = rand.Next(sett.SurvivorCount);
                    inds[parent].genes.CopyTo(inds[i].genes);

                    if (UseSavestates) {
                        inds[i].parent = inds[parent];
                        inds[i].fStates = null;
                        inds[i].SkippingState = null;
                    }

                    Mutate(inds[i]);
                }

                // simplification
                else {
                    int parent = rand.Next(sett.SurvivorCount);
                    inds[parent].genes.CopyTo(inds[i].genes);

                    if (UseSavestates) {
                        inds[i].parent = inds[parent];
                        inds[i].fStates = null;
                        inds[i].SkippingState = null;
                    }

                    Simplify(inds[i]);
                }
                i++;
            }
        }

        public void Mutate(FrameInd ind)
        {
            int mutationCount = rand.Next(sett.MaxMutChangeCount);
            int earliestMutatedFrame = 99999999;
            for (int m = 0; m <= mutationCount; m++) {
                int end = rand.Next(1, UpToFrame - 1);
                int start = Max(0, end - rand.Next(1, Max(2, UpToFrame / 8)));
                float increment = (float)((rand.NextDouble() - 0.5d) * mutationMagnitude);

                earliestMutatedFrame = start < earliestMutatedFrame ? start : earliestMutatedFrame;

                for (int i = start; i < end; i++)
                    ind.genes[i] += increment;
            }

            if (UseSavestates && ind.parent.fStates.Length > 0) {
                earliestMutatedFrame--;
                if (earliestMutatedFrame < 0) {
                    ind.SkippingState = null;
                    return;
                }
                ind.SkippingState = ind.parent.fStates[Min(earliestMutatedFrame, ind.parent.fStates.Length - 1)];
            }
        }

        public (FrameInd child1, FrameInd child2) Crossover(FrameInd parent1, FrameInd parent2)
        {
            int index = rand.Next(1, UpToFrame - 1);

            var res = (new FrameInd(parent1.genes[..index].Concat(parent2.genes[index..])),
                       new FrameInd(parent2.genes[..index].Concat(parent1.genes[index..])));

            if (UseSavestates && parent1.fStates.Length > 0 && parent2.fStates.Length > 0) {
                res.Item1.parent = parent1;
                res.Item2.parent = parent2;
                res.Item1.SkippingState = parent1.fStates[Min(index - 1, parent1.fStates.Length - 1)];
                res.Item2.SkippingState = parent2.fStates[Min(index - 1, parent2.fStates.Length - 1)];
            }

            return res;
        }

        public void Simplify(FrameInd ind)
        {
            int len = rand.Next(UpToFrame / 3);
            int start = rand.Next(1, UpToFrame - len - 1);
            int end = start + len;

            float newVal = rand.Next(3) switch {
                0 => ind.genes[start - 1], //set to value on the range's left
                1 => ind.genes[end], //set to value on the range's right
                2 => ind.genes[start..end].Sum() / (end - start), //set to the average of the range
                _ => 0
            };

            ind.genes.SetRange(newVal, start, end);

            if (UseSavestates && ind.parent.fStates.Length != 0)
                ind.SkippingState = ind.parent.fStates[Min(start, ind.parent.fStates.Length - 1)];
        }

        #endregion

        public AngleSet favorite;

        public FrameGenesGA(Settings s, int upToFrame)
        {
            sett = s;
            mutationMagnitude = sett.MutationMagnitude;

            favorite = ParseFavorite(s.Favorite, s.Framecount);
            inds = favorite == null
                ? new FrameInd[sett.Population].Select(i => new FrameInd(new AngleSet(sett.Framecount).Randomize())).ToArray()
                : new int[sett.Population].Select(i => new FrameInd(favorite.Clone())).ToArray();

            if (favorite == null)
                foreach (var ind in inds) {
                    var sim = new FeatherSim(sett);
                    sim.SimulateIndivitual(ind.genes, upToFrame);
                    sim.Evaluate(out var fitness, out _);
                    ind.fitness = fitness;
                }
        }

        public double GetBestFitness() => inds[0].fitness;

        public AngleSet GetBestIndividual() => inds[0].genes;
    }

    public class FrameInd
    {
        public AngleSet genes;
        public double fitness = -999999d;

        private Savestate skippingState = null;
        public Savestate SkippingState
        {
            get { return skippingState?.Copy(); }
            set { skippingState = value; }
        }

        public Savestate[] fStates = null;
        public FrameInd parent = null;

        public FrameInd(int fCount) => genes = new AngleSet(new float[fCount].Select(n => (float)(rand.NextDouble() * Revolution)).ToArray());
        public FrameInd(AngleSet genes) => this.genes = genes;
    }

    public class LineGenesGA
    {
        public LineInd[] inds;
        private Settings sett;

        public int indLength;
        public int[] timings;

        private const float crossoverProb = 0.2f;

        private int earliestMutateableAngle;
        private int lastMutateableAngle;

        public void DoGeneration(int earliestMutTurn = 0, int runToTiming = 9999999)
        {
            earliestMutateableAngle = earliestMutTurn;
            lastMutateableAngle = runToTiming;
            int runToFrame = runToTiming >= timings.Length ? indLength : timings[runToTiming];

            GenerateNewChildren();

            MyParallel.Run(sett.SurvivorCount, inds.Length, DoSim);

            inds = inds.OrderByDescending(ind => ind.fitness).ToArray();

            GetAllStatesOfBest();

            void DoSim(int i)
            {
                var sim = new FeatherSim(sett);
                sim.SimulateIndivitual(inds[i].ToFrameGenes(indLength, timings), runToFrame, inds[i].SkippingState);
                sim.Evaluate(out var fitness, out _);
                inds[i].fitness = fitness;
            }
        }

        private void GetAllStatesOfBest()
        {
            foreach (var ind in inds.Take(sett.SurvivorCount)) {
                ind.states ??= GetStatesOf(ind);
                UnExtremeifyAnglesOf(ind);
            }
        }

        private Savestate[] GetStatesOf(LineInd ind)
        {
            if (ind.states is null) {
                var states = new FeatherSim(sett).GetAllFrameData(ind.ToFrameGenes(indLength, timings), out _, out _);

                return states;
            }
            return null;
        }

        private void UnExtremeifyAnglesOf(LineInd ind)
        {
            if (timings[0] <= ind.states.Length) {
                float firstAngDiff = timings[0] == 1
                    ? ind.states[0].fState.spd.TASAngle - Level.StartState.fState.spd.TASAngle
                    : ind.states[timings[0] - 1].fState.spd.TASAngle - ind.states[timings[0] - 2].fState.spd.TASAngle;
                AdjustLine(0, firstAngDiff);

                for (int i = 1; i < timings.Length && timings[i] <= ind.states.Length; i++) {
                    float angDiff = ind.states[timings[i] - 1].fState.spd.TASAngle - ind.states[timings[i] - 2].fState.spd.TASAngle;
                    AdjustLine(i, angDiff);
                }
            }

            void AdjustLine(int line, float angleDiff)
            {
                if (angleDiff >= 5.3333)
                    ind.angles[line] = (float)Ceiling(ind.states[timings[line] - 1].fState.spd.TASAngle);
                else if (angleDiff <= -5.3333)
                    ind.angles[line] = (float)Floor(ind.states[timings[line] - 1].fState.spd.TASAngle);
            }
        }

        #region ChildGeneration

        private void GenerateNewChildren()
        {
            for (int i = sett.SurvivorCount; i < inds.Length; i++) {
                // crossover
                if (rand.NextDouble() < crossoverProb) {
                    int p1 = rand.Next(sett.SurvivorCount);
                    int p2 = rand.Next(sett.SurvivorCount - 1);
                    p2 += p2 >= p1 ? 1 : 0;

                    Crossover(inds[p1], inds[p2], inds[i], i == inds.Length - 1 ? null : inds[++i]);
                }

                // mutation
                else {
                    var parent = inds[rand.Next(sett.SurvivorCount)];
                    inds[i].CloneFrom(parent);

                    inds[i].parent = parent;
                    inds[i].states = null;
                    inds[i].SkippingState = null;

                    Mutate(inds[i]);
                }
            }
        }

        private void Mutate(LineInd ind)
        {
            int mutCount = rand.Next(sett.MaxMutChangeCount);
            int earliestChange = 99999999;
            for (int i = 0; i <= mutCount; i++) {
                // adjust actual line angle
                if (rand.Next(3) != 0) {
                    int target = rand.Next(earliestMutateableAngle, Min(ind.angles.Length, lastMutateableAngle));
                    ind.angles[target] += ((float)rand.NextDouble() * 2 - 1) * sett.MutationMagnitude;
                    earliestChange = target == 0 ? -1 : Min(timings[target - 1], earliestChange);
                }
                // adjust end of line extra
                else {
                    int target = rand.Next(Max(0, earliestMutateableAngle - 1), Min(ind.borders.Length, lastMutateableAngle));
                    ind.borders[target] = (float)rand.NextDouble() * 5.334f;

                    earliestChange = Min(timings[target] - 1, earliestChange);
                }
            }

            earliestChange--;
            if (earliestChange >= 0)
                ind.SkippingState = earliestChange >= ind.parent.states.Length ? null : ind.parent.states[earliestChange];
        }
        
        private void Crossover(LineInd p1, LineInd p2, LineInd c1, LineInd c2)
        {
            int index = rand.Next(earliestMutateableAngle + 1, Min(lastMutateableAngle, timings.Length) + 1);

            int skipStateIndex = timings[index - 1] - 2;
            bool ableToAddSkipState = skipStateIndex >= 0;

            c1.CloneFrom(new LineInd(p1.angles[..index].Concat(p2.angles[index..]), p1.borders[..index].Concat(p2.borders[index..])));
            if (ableToAddSkipState) {
                c1.parent = p1;
                c1.states = null;
                c1.SkippingState = skipStateIndex >= p1.states.Length ? null : p1.states[skipStateIndex];
            }

            if (!(c2 is null)) {
                c2.CloneFrom(new LineInd(p2.angles[..index].Concat(p1.angles[index..]), p2.borders[..index].Concat(p1.borders[index..])));
                if (ableToAddSkipState) {
                    c2.parent = p2;
                    c2.states = null;
                    c2.SkippingState = skipStateIndex >= p2.states.Length ? null : p2.states[skipStateIndex];
                }
            }
        }

        #endregion

        public LineGenesGA(Settings s, int[] timings, AngleSet lineAngles)
        {
            sett = s;

            this.timings = timings;
            indLength = s.Framecount;


            inds = new LineInd[s.Population];

            inds[0] = new LineInd(lineAngles);
            inds[0].states = GetStatesOf(inds[0]);

            for (int i = 1; i < inds.Length; i++) {
                inds[i] = new LineInd(lineAngles);
                inds[i].states = inds[0].states;
                inds[i].parent = inds[0];
            }
        }
    }

    public class LineInd
    {
        public AngleSet borders;
        public AngleSet angles;

        public double fitness = -999999d;

        private Savestate skippingState = null;
        public Savestate SkippingState {
            get { return skippingState?.Copy(); }
            set { skippingState = value; }
        }

        public Savestate[] states = null;
        public LineInd parent = null;

        public AngleSet ToFrameGenes(int duration, int[] timings)
        {
            var res = new AngleSet(duration);
            int line = 0;

            res.SetRange(angles[0], 0, timings.Length == 0 ? duration : Min(timings[0], duration));
            for (int i = 0; i < timings.Length; i++)
                res.SetRange(angles[line + 1], timings[i], i == timings.Length - 1 ? duration : timings[i + 1]);

            for (int i = 0; i < res.Length; i++) {
                line += line < timings.Length && i >= timings[line] ? 1 : 0;
                res[i] = angles[line];
            }
            for (line = 0; line < borders.Length; line++) {
                float angDiff = FeatherSim.DegreesDiff(angles[line], angles[line + 1]);
                if (Abs(angDiff) > 5.334) {
                    res[timings[line] - 1] += angDiff > 0 ? borders[line] : -borders[line];
                    /*float angle = (float)Round(res[timings[line] - 1] + (angDiff > 0 ? borders[line] : -borders[line]), 3);
                    res[timings[line] - 1] = angle < 0f ? angle + Revolution : angle >= Revolution ? angle - Revolution : angle;*/
                }
            }
            return new AngleSet(res);
        }

        #region Constructors

        public LineInd(AngleSet angles)
        {
            this.angles = angles.Clone();
            borders = new AngleSet(angles.Length - 1);
        }

        public LineInd(AngleSet angles, AngleSet borders, bool cloneBorders = true)
        {
            this.angles = angles.Clone();
            this.borders = cloneBorders ? borders.Clone() : borders;
        }

        public void CloneFrom(LineInd parent)
        {
            parent.angles.CopyTo(angles);
            parent.borders.CopyTo(borders);
        }

        #endregion
    }
}
