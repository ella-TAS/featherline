using System.Linq;
using System;
using static System.Math;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Featherline
{
    public abstract class GA
    {
        public static readonly ParallelOptions opt = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
        public static readonly Random rand = new Random();
        public Settings sett;

        public float crossoverProb;
        public float mutationProb;
        public float mutationMagnitude;

        public abstract void DoGeneration(int upToFrame);

        public abstract float GetBestFitness();
        public abstract void PrintBestIndividual();

        public GA(Settings s)
        {
            sett = s;

            crossoverProb = sett.SurvivorCount <= 1 ? 0 : sett.CrossoverProbability / 2;
            mutationProb = sett.MutationProbability;
            float probSum = crossoverProb + mutationProb + sett.SimplificationProbability;
            crossoverProb /= probSum;
            mutationProb = mutationProb / probSum + crossoverProb;

            mutationMagnitude = sett.MutationMagnitude;
        }

        public void PrintFromFrameGenes(float[] genes)
        {
            if (!sett.EnableSteepTurns)
                new FeatherSim(GAManager.settings).SimulateIndivitual(genes, true);

            float lastAngle = genes[0];
            int consecutive = 1;
            foreach (var f in genes.Skip(1)) {
                if (f != lastAngle) {
                    Console.WriteLine($"{consecutive,4},F,{lastAngle.ToString().Replace(',', '.')}");
                    lastAngle = f;
                    consecutive = 1;
                }
                else
                    consecutive++;
            }
            Console.WriteLine($"{consecutive,4},F,{lastAngle.ToString().Replace(',', '.')}");
        }
    }

    public class FrameGenesGA : GA
    {
        public Individual[] inds;
        public FeatherSim sim;

        private int UpToFrame;
        public override void DoGeneration(int upToFrame)
        {
            UpToFrame = Math.Min(upToFrame, inds[0].genes.Length);
            GenerateNewChildren();

            int shortestFrameCount = sett.Framecount;

            //for (int i = sett.SurvivorCount; i < inds.Length; i++)
                //inds[i].fitness = new FeatherSim(sett).SimulateIndivitual(inds[i].genes, true);
            Parallel.For(sett.SurvivorCount, inds.Length, opt, DoSim);

            inds = inds.Reverse().OrderByDescending(ind => ind.fitness).ToArray();

            if (shortestFrameCount < sett.Framecount) {
                sett.Framecount = shortestFrameCount;
                foreach (var ind in inds)
                    Array.Resize(ref ind.genes, shortestFrameCount);
            }

            void DoSim(int i)
            {
                var res = new FeatherSim(sett).SimulateIndivitual(inds[i].genes, true, upToFrame);
                inds[i].fitness = res.fitness;
                shortestFrameCount = Min(shortestFrameCount, res.frames);
            }
        }

        public void GenerateNewChildren()
        {
            for (int i = sett.SurvivorCount; i < inds.Length;) {
                float actionChooser = (float)rand.NextDouble();
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
                else if (mutationProb > actionChooser) {
                    inds[rand.Next(sett.SurvivorCount)].genes.CopyTo(inds[i].genes, 0);
                    Mutate(inds[i]);
                }
                else { // simplification
                    inds[rand.Next(sett.SurvivorCount)].genes.CopyTo(inds[i].genes, 0);
                    Simplify(inds[i]);
                }
                i++;
            }
        }

        public void Mutate(Individual ind)
        {
            int mutationCount = rand.Next(sett.MaxMutChangeCount) + 1;
            for (int m = 0; m < mutationCount; m++) {
                int end = rand.Next(1, UpToFrame - 1);
                int start = Max(0, end - rand.Next(1, Max(2, UpToFrame / 8)));
                float increment = (float)((rand.NextDouble() - 0.5d) * mutationMagnitude);

                for (int i = start; i < end; i++) {
                    ind.genes[i] = (float)Round(ind.genes[i] + increment, 3);
                    ind.genes[i] = ind.genes[i] >= 360
                        ? ind.genes[i] - 360
                        : ind.genes[i] < 0 ? ind.genes[i] + 360 : ind.genes[i];
                }
            }
        }

        public (Individual child1, Individual child2) Crossover(Individual parent1, Individual parent2)
        {
            int index = rand.Next(1, UpToFrame - 1);
            return (new Individual(parent1.genes[..index].Concat(parent2.genes[index..]).ToArray()),
                    new Individual(parent2.genes[..index].Concat(parent1.genes[index..]).ToArray()));
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
        }

        public float[] favorite;

        Individual CopyFavorite()
        {
            float[] newArr = new float[sett.Framecount];
            Array.Copy(favorite, newArr, sett.Framecount);
            return new Individual(newArr);
        }

        public FrameGenesGA(Settings s) : base(s)
        {
            favorite = GAManager.ParseFavorite(s.Favorite, s.Framecount);
            inds = favorite == null
                ? new Individual[sett.Population].Select(i => new Individual(sett.Framecount)).ToArray()
                : new int[sett.Population].Select(i => CopyFavorite()).ToArray();
            sim = new FeatherSim(sett);

            if (favorite == null)
                foreach (var ind in inds)
                    ind.fitness = sim.SimulateIndivitual(ind.genes).fitness;
        }

        public override float GetBestFitness() => inds[0].fitness;
        public override void PrintBestIndividual() => PrintFromFrameGenes(inds[0].genes);

        public class Individual
        {
            public float[] genes;
            public float fitness = -100000000000;

            public Individual(int fCount) => genes = new float[fCount].Select(n => (float)(rand.NextDouble() * 360d)).ToArray();
            public Individual(float[] genes) => this.genes = genes;
        }
    }

    public class LineGenesGA : GA
    {
        public Individual[] inds;

        public override void DoGeneration(int upToFrame)
        {
            GenerateNewChildren();

            //for (int i = sett.SurvivorCount; i < inds.Length; i++)
                //inds[i].fitness = new FeatherSim(sett).SimulateIndivitual(inds[i].ToFrameGenes());
            Parallel.For(sett.SurvivorCount, inds.Length, opt,
                i => inds[i].fitness = new FeatherSim(sett).SimulateIndivitual(inds[i].ToFrameGenes(), false, upToFrame).fitness);

            inds = inds.Reverse().OrderByDescending(ind => ind.fitness).ToArray();
        }

        public void GenerateNewChildren()
        {
            for (int i = sett.SurvivorCount; i < inds.Length; i++) {
                float actionChooser = (float)rand.NextDouble();

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
                else if (mutationProb > actionChooser) {
                    inds[i].CopyFrom(inds[rand.Next(sett.SurvivorCount)]);
                    Mutate(inds[i]);
                }
                else {
                    inds[i].CopyFrom(inds[rand.Next(sett.SurvivorCount)]);
                    ChangeTiming(inds[i]);
                }
            }
        }

        public void Mutate(Individual ind)
        {
            int changeCount = rand.Next(sett.MaxMutChangeCount) + 1;
            for (int i = 0; i < changeCount; i++) {
                float decider = (float)rand.NextDouble();
                if (decider < 0.45f) { // adjust angle of an input line
                    int target = rand.Next(ind.angles.Length);
                    ind.angles[target] += (float)(rand.NextDouble() * 2 - 1) * mutationMagnitude;
                    if (ind.angles[target] < 0f) ind.angles[target] += 360f;
                    else if (ind.angles[target] >= 360f) ind.angles[target] -= 360f;
                }
                else if (decider < 0.9f) { // change a turn transition frame
                    int target = rand.Next(ind.borders.Length);
                    ind.borders[target].turnTransitionExtra = (float)(rand.NextDouble() - 0.5d) * 10.6666667f;
                }
                else {
                    ChangeTiming(ind);
                }
            }
        }

        public void ChangeTiming(Individual ind)
        {
            for (int j = 0; j < 10; j++) { // attempt 10 times at most
                int bor = rand.Next(ind.borders.Length);
                int change = (rand.Next(2) << 1) - 1;
                int resIndex = ind.borders[bor].index + change;
                if (resIndex <= 0 || resIndex >= sett.Framecount || ind.borders.Any(b => b.index == resIndex))
                    continue;
                ind.borders[bor].index = resIndex;
                break;
            }
        }

        public (Individual child1, Individual child2) Crossover(Individual parent1, Individual parent2)
        {
            int index = rand.Next(1, parent1.borders.Length);
            return (new Individual(parent1.borders[..index].Concat(parent2.borders[index..]).ToArray(), parent1.angles[..index].Concat(parent2.angles[index..]).ToArray()),
                    new Individual(parent2.borders[..index].Concat(parent1.borders[index..]).ToArray(), parent2.angles[..index].Concat(parent1.angles[index..]).ToArray()));
        }

        public override float GetBestFitness() => inds[0].fitness;
        public override void PrintBestIndividual()
        {
            var bestGenes = inds[0].ToFrameGenes();
            //new FeatherSim(sett).SimulateIndivitual(bestGenes, true);
            PrintFromFrameGenes(bestGenes);
        }

        public LineGenesGA(Settings s) : base(s)
        {
            Individual.totalFrames = s.Framecount;
            inds = new Individual[s.Population];
            LineBorder[] evenlySpacedBorders = Enumerable.Range(1, s.InputLineCount - 1)
                .Select(i => new LineBorder() {
                    index = s.Framecount * i / s.InputLineCount,
                    turnTransitionExtra = 0
                }).ToArray();

            for (int i = 0; i < inds.Length; i++)
                inds[i] = new Individual(evenlySpacedBorders);
        }

        public class Individual
        {
            public static int totalFrames;

            public LineBorder[] borders;
            public float[] angles;

            public float fitness = -9999999999;

            public float[] ToFrameGenes()
            {
                var res = new float[totalFrames];

                int lastBorderPos = 0;
                for (int border = 0; border < borders.Length; border++) {
                    int borderPos = borders[border].index;
                    float rangeAngle = (float)Round(angles[border], 3);
                    for (int i = lastBorderPos; i < borderPos; i++) // assign the input line angle to the range before the border we're looking at
                        res[i] = rangeAngle;
                    res[borderPos - 1] = (float)Round(res[borderPos - 1] + borders[border].turnTransitionExtra, 3); // add the offset to the single unique frame in between input lines
                    lastBorderPos = borderPos;
                }
                float lastAngle = angles[angles.Length - 1];
                for (int i = lastBorderPos; i < res.Length; i++) // do the last line of input
                    res[i] = lastAngle;

                return res;
            }

            public Individual(LineBorder[] borders)
            {
                // copy borders by value
                this.borders = new LineBorder[borders.Length];
                Array.Copy(borders, this.borders, borders.Length);

                angles = new float[borders.Length + 1];
                for (int i = 0; i < angles.Length; i++)
                    angles[i] = (float)Round(rand.NextDouble() * 360, 3);
            }
            public Individual(LineBorder[] borders, float[] angles)
            {
                this.borders = new LineBorder[borders.Length];
                Array.Copy(borders, this.borders, borders.Length);

                this.angles = new float[angles.Length];
                Array.Copy(angles, this.angles, angles.Length);
            }
            public void CopyFrom(Individual src)
            {
                Array.Copy(src.borders, borders, borders.Length);
                Array.Copy(src.angles, angles, angles.Length);
            }
        }

        public struct LineBorder
        {
            public int index;
            public float turnTransitionExtra;
        }
    }
}