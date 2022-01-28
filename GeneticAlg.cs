using System.Linq;
using System;
using static System.Math;
using System.Threading.Tasks;
using static Featherline.FrameGenesGA;

namespace Featherline
{
    public class FrameGenesGA
    {
        private ParallelOptions paralOpt;
        public static readonly Random rand = new Random();
        public Settings sett;

        private float crossoverProb;
        private float mutationProb;
        private float mutationMagnitude;

        public Individual[] inds;
        public FeatherSim sim;

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

            //for (int i = sett.SurvivorCount; i < inds.Length; i++)
                //DoSim(i);
            Parallel.For(sett.SurvivorCount, inds.Length, paralOpt, DoSim);

            inds = inds.OrderByDescending(ind => ind.fitness).ToArray();

            if (shortestFrameCount < sett.Framecount) {
                sett.Framecount = shortestFrameCount;
                foreach (var ind in inds)
                    Array.Resize(ref ind.genes, shortestFrameCount);
            }

            void DoSim(int i)
            {
                var sim = new FeatherSim(sett);
                sim.SimulateIndivitual(inds[i].genes, true, upToFrame, inds[i].SkippingState);
                sim.Evaluate(out var fitness, out int frames);
                inds[i].fitness = fitness;
                shortestFrameCount = Min(shortestFrameCount, frames);
            }
        }

        private void GetAllStatesOfBest()
        {
            for (int i = 0; i < sett.SurvivorCount; i++)
                if (inds[i].fStates is null)
                    inds[i].fStates = new FeatherSim(sett).GetAllFrameData(inds[i].genes);
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
                    inds[parent].genes.CopyTo(inds[i].genes, 0);

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
                    inds[parent].genes.CopyTo(inds[i].genes, 0);

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

        public void Mutate(Individual ind)
        {
            int mutationCount = rand.Next(sett.MaxMutChangeCount);
            int earliestMutatedFrame = 99999999;
            for (int m = 0; m <= mutationCount; m++) {
                int end = rand.Next(1, UpToFrame - 1);
                int start = Max(0, end - rand.Next(1, Max(2, UpToFrame / 8)));
                float increment = (float)((rand.NextDouble() - 0.5d) * mutationMagnitude);

                earliestMutatedFrame = start < earliestMutatedFrame ? start : earliestMutatedFrame;

                for (int i = start; i < end; i++) {
                    ind.genes[i] = (float)Round(ind.genes[i] + increment, 3);
                    ind.genes[i] = ind.genes[i] >= 360
                        ? ind.genes[i] - 360
                        : ind.genes[i] < 0 ? ind.genes[i] + 360 : ind.genes[i];
                }
            }

            if (UseSavestates) {
                earliestMutatedFrame--;
                if (earliestMutatedFrame < 0) {
                    ind.SkippingState = null;
                    return;
                }
                ind.SkippingState = ind.parent.fStates[Min(earliestMutatedFrame, ind.parent.fStates.Length - 1)];
            }
        }

        public (Individual child1, Individual child2) Crossover(Individual parent1, Individual parent2)
        {
            int index = rand.Next(1, UpToFrame - 1);

            var res = (new Individual(parent1.genes[..index].Concat(parent2.genes[index..]).ToArray()),
                       new Individual(parent2.genes[..index].Concat(parent1.genes[index..]).ToArray()));

            if (UseSavestates) {
                res.Item1.parent = parent1;
                res.Item2.parent = parent2;
                res.Item1.SkippingState = parent1.fStates[Min(index - 1, parent1.fStates.Length - 1)];
                res.Item2.SkippingState = parent2.fStates[Min(index - 1, parent2.fStates.Length - 1)];
            }

            return res;
        }

        public void Simplify(Individual ind)
        {
            int len = rand.Next(UpToFrame / 3);
            int start = rand.Next(1, UpToFrame - len - 1);
            int end = start + len;

            float newVal = rand.Next(3) switch {
                0 => ind.genes[start - 1], //set to value on the range's left
                1 => ind.genes[end], //set to value on the range's right
                2 => (float)Round(ind.genes[start..end].Sum() / (end - start), 3), //set to the average of the range
                _ => 0
            };

            for (int i = start; i < end; i++)
                ind.genes[i] = newVal;

            if (UseSavestates)
                ind.SkippingState = ind.parent.fStates[Min(start, ind.parent.fStates.Length - 1)];
        }

        #endregion

        public float[] favorite;

        Individual CopyFavorite()
        {
            float[] newArr = new float[sett.Framecount];
            Array.Copy(favorite, newArr, sett.Framecount);
            return new Individual(newArr);
        }

        public FrameGenesGA(Settings s)
        {
            sett = s;
            paralOpt = new ParallelOptions() { MaxDegreeOfParallelism = sett.MaxThreadCount };

            crossoverProb = sett.SurvivorCount <= 1 ? 0 : sett.CrossoverProbability / 2;
            mutationProb = sett.MutationProbability;
            float probSum = crossoverProb + mutationProb + sett.SimplificationProbability;
            crossoverProb /= probSum;
            mutationProb = mutationProb / probSum + crossoverProb;

            mutationMagnitude = sett.MutationMagnitude;

            favorite = GAManager.ParseFavorite(s.Favorite, s.Framecount);
            inds = favorite == null
                ? new Individual[sett.Population].Select(i => new Individual(sett.Framecount)).ToArray()
                : new int[sett.Population].Select(i => CopyFavorite()).ToArray();
            sim = new FeatherSim(sett);

            if (favorite == null)
                foreach (var ind in inds) {
                    sim.SimulateIndivitual(ind.genes);
                    sim.Evaluate(out var fitness, out _);
                    ind.fitness = fitness;
                }
        }

        public double GetBestFitness() => inds[0].fitness;
        public void PrintBestIndividual() => PrintFromFrameGenes(inds[0].genes);

        public void PrintFromFrameGenes(float[] genes)
        {
            if (!sett.EnableSteepTurns)
                new FeatherSim(GAManager.settings).SimulateIndivitual(genes, true);

            Console.WriteLine(GAManager.FrameGenesToString(genes));
        }

        public float[] GetBestIndividual() => inds[0].genes;

        public class Individual
        {
            public float[] genes;
            public double fitness = -999999;

            private Savestate skippingState = null;
            public Savestate SkippingState {
                get { return skippingState?.Copy(); }
                set { skippingState = value; }
            }

            public Savestate[] fStates = null;
            public Individual parent = null;

            public Individual(int fCount) => genes = new float[fCount].Select(n => (float)(rand.NextDouble() * 360d)).ToArray();
            public Individual(float[] genes) => this.genes = genes;
        }
    }

    public class LineGenesGA
    {
        public LineInd[] inds;
        private Settings sett;
        private ParallelOptions paralOpt;

        public int indLength;
        public int[] timings;

        private float crossoverProb;

        private int earliestMutateableAngle;
        private int lastMutateableAngle;

        public void DoGeneration(int earliestMutTurn = 0, int runToTurnPoint = 9999999)
        {
            earliestMutateableAngle = earliestMutTurn;
            lastMutateableAngle = runToTurnPoint;
            int runToFrame = runToTurnPoint >= timings.Length ? indLength : timings[runToTurnPoint];

            GenerateNewChildren();

            //for (int i = sett.SurvivorCount; i < inds.Length; i++)
                //DoSim(i);
            Parallel.For(sett.SurvivorCount, inds.Length, paralOpt, DoSim);

            inds = inds.OrderByDescending(ind => ind.fitness).ToArray();

            GetAllStatesOfBest();

            void DoSim(int i)
            {
                var sim = new FeatherSim(sett);
                sim.SimulateIndivitual(inds[i].ToFrameGenes(indLength, timings), false, runToFrame, inds[i].SkippingState);
                sim.Evaluate(out double fitness, out _);
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
                var states = new FeatherSim(sett).GetAllFrameData(ind.ToFrameGenes(indLength, timings));

                return states;
            }
            return null;
        }

        private void UnExtremeifyAnglesOf(LineInd ind)
        {
            if (timings[0] <= ind.states.Length) {
                float firstAngDiff = timings[0] == 1
                    ? ind.states[0].fState.spd.TASAngle - sett.StartAngle
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
                    float increment = ((float)rand.NextDouble() * 2 - 1) * sett.MutationMagnitude;

                    float angle = ind.angles[target] + increment;
                    angle = angle < 0 ? angle + 360 : angle;
                    angle = angle >= 360 ? angle - 360 : angle;
                    angle = (float)Round(angle, 3);

                    ind.angles[target] = angle;

                    earliestChange = target == 0 ? -1 : Min(timings[target - 1], earliestChange);
                }

                // adjust end of line extra
                else {
                    int target = rand.Next(Max(0, earliestMutateableAngle - 1), Min(ind.borderExtras.Length, lastMutateableAngle));
                    ind.borderExtras[target] = (float)rand.NextDouble() * 5.3334f;

                    earliestChange = Min(timings[target] - 1, earliestChange);
                }
            }

            earliestChange--;
            if (earliestChange >= 0)
                ind.SkippingState = earliestChange >= ind.parent.states.Length ? null : ind.parent.states[earliestChange];
        }
        
        private void Crossover(LineInd p1, LineInd p2, LineInd c1, LineInd c2)
        {
            int index = rand.Next(earliestMutateableAngle + 1, Min(lastMutateableAngle, timings.Length + 1));

            int skipStateIndex = timings[index - 1] - 2;
            bool ableToAddSkipState = skipStateIndex >= 0;

            c1.CloneFrom(new LineInd(p1.angles[..index].Concat(p2.angles[index..]).ToArray(), p1.borderExtras[..index].Concat(p2.borderExtras[index..]).ToArray()));
            if (ableToAddSkipState) {
                c1.parent = p1;
                c1.states = null;
                c1.SkippingState = skipStateIndex >= p1.states.Length ? null : p1.states[skipStateIndex];
            }

            if (!(c2 is null)) {
                c2.CloneFrom(new LineInd(p2.angles[..index].Concat(p1.angles[index..]).ToArray(), p2.borderExtras[..index].Concat(p1.borderExtras[index..]).ToArray()));
                if (ableToAddSkipState) {
                    c2.parent = p2;
                    c2.states = null;
                    c2.SkippingState = skipStateIndex >= p2.states.Length ? null : p2.states[skipStateIndex];
                }
            }
        }

        #endregion

        public LineGenesGA(Settings s, int[] turnPoints, float[] lineAngles)
        {
            sett = s;
            paralOpt = new ParallelOptions() { MaxDegreeOfParallelism = sett.MaxThreadCount };

            crossoverProb = sett.SurvivorCount <= 1 ? 0 : sett.CrossoverProbability;
            crossoverProb /= (crossoverProb + sett.MutationProbability) * 2;

            timings = turnPoints;
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
        public float[] borderExtras;
        public float[] angles;

        public double fitness = -9999999999;

        private Savestate skippingState = null;
        public Savestate SkippingState {
            get { return skippingState?.Copy(); }
            set { skippingState = value; }
        }

        public Savestate[] states = null;
        public LineInd parent = null;

        public float[] ToFrameGenes(int duration, int[] timings)
        {
            var res = new float[duration];
            int line = 0;
            for (int i = 0; i < res.Length; i++) {
                line += line < timings.Length && i >= timings[line] ? 1 : 0;
                res[i] = angles[line];
            }
            for (line = 0; line < borderExtras.Length; line++)
                if (Abs(angles[line + 1] - angles[line]) > 5.3334)
                    res[timings[line] - 1] += angles[line + 1] > angles[line] ? borderExtras[line] : -borderExtras[line];
            return res;
        }

        #region Constructors

        public LineInd(float[] angles)
        {
            this.angles = (float[])angles.Clone();
            borderExtras = new float[angles.Length - 1];
        }

        public LineInd(float[] angles, float[] borderExtras, bool cloneBorders = true)
        {
            this.angles = (float[])angles.Clone();
            this.borderExtras = cloneBorders ? (float[])borderExtras.Clone() : borderExtras;
        }

        public void CloneFrom(LineInd parent)
        {
            Array.Copy(parent.angles, angles, angles.Length);
            Array.Copy(parent.borderExtras, borderExtras, borderExtras.Length);
        }

        #endregion
    }
}
