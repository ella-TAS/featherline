namespace Featherline;

public class AnglePerfector
{
    private LineInd best;
    private LineInd current;

    private int[] timings;
    private int indLen;

    private Settings settings;

    public int count = 0;

    private float precision;

    public static Savestate[] baseInfo;
    public static bool baseInfoFinishes;
    public static int[] baseInfoWallboops;
    public static double baseInfoFitness;

    private int optimizingAt;

    public LineInd Run()
    {
        for (int simLen = 2; simLen < best.angles.Length; simLen++) {
            if (current.fitness + 200d < baseInfoFitness)
                current = TimingTester.FixInd(current, timings, settings.GensPerTiming / 2);
            for (int border = 0; border + simLen < best.angles.Length; border++) {
                LineInd res = current;
                for (int i = border; i < border + simLen; i++)
                    res = PerfectBorderAt(res, range(border, border + simLen), out int survival);
                if (res.fitness >= current.fitness)
                    current = res;
            }
        }
        //return current;




        int startingAt = 0;
        var improvedBorders = Enumerable.Repeat(true, timings.Length).ToArray();

        int farthestSurvival = startingAt - 1;

        for (int i = 0; i < 10; i++) {

            new FeatherSim(settings)
                    .SimulateIndivitual(current.ToFrameGenes(indLen, timings), indLen, current.SkippingState)
                    .Evaluate(out current.fitness, out _);
            if (current.fitness + 200d < baseInfoFitness)
                current = TimingTester.FixInd(current, timings, settings.GensPerTiming / 2);

            var oldBorderExtras = current.borders.Clone(); 

            farthestSurvival = startingAt - 1;
            for (; optimizingAt < timings.Length; optimizingAt++) {
                current = PerfectBorderAt(current, range(optimizingAt, best.angles.Length), out int survival);
                farthestSurvival = Math.Max(farthestSurvival, survival);

                bool improvement = current.fitness > best.fitness;
                if (improvement) {
                    best = current;
                    current = new LineInd(best.angles, best.borders);
                }
                improvedBorders[optimizingAt] = improvement;

                if (improvedBorders.All(b => !b))
                    goto End;
            }

            optimizingAt = 0;

            if (current.borders.SequenceEqual(oldBorderExtras))
                goto End;
        }

        End:

        Console.WriteLine("fitness " + best.fitness.FitnessFormat());
        return best;
    }

    #region Borders

    public const float MaxFrameSteer = 5.334f;

    private LineInd PerfectBorderAt(LineInd ind, IntRange borderIndex, out int survival)
    {
        var possibilities = Enumerable.Range(0, 9).Select(n => n * MaxFrameSteer / 8).ToArray();
        var inds = new LineInd[9];

        int farthestSurvival = 0;

        MyParallel.Run(0, 9, GetInd);

        int best = IndexBest(inds);

        if (inds.Count(ind => ind.fitness == inds[best].fitness) > 1) {
            survival = farthestSurvival;
            return inds[best];
        }

        float lowerBound = best == 0 ? -5 : possibilities[best - 1];
        float upperBound = best == 8 ? 10 : possibilities[best + 1];

        survival = farthestSurvival;
        return BorderTestNode(borderIndex, lowerBound, upperBound, inds[best], possibilities[best]);
        void GetInd(int i)
        {
            var candidate = new LineInd(ind.angles, ind.borders);
            candidate.borders[borderIndex.start] = possibilities[i];
            AdjustAngles(candidate, borderIndex, out int survive);
            farthestSurvival = Math.Max(survive, farthestSurvival);
            var sim = new FeatherSim(settings);
            sim.SimulateIndivitual(candidate.ToFrameGenes(indLen, timings), IndexTimings(borderIndex.end));
            sim.Evaluate(out var fitness, out _);
            candidate.fitness = fitness;
            inds[i] = candidate;
        }
    }

    const int NodeBase = 5;
    private LineInd BorderTestNode(IntRange borderIndex, float lowerBound, float upperBound, LineInd center, float centerAngle)
    {
        centerAngle = (float)Math.Round(centerAngle, 3);
        var anglesIEnum = Enumerable.Empty<float>();

        if (lowerBound < 0) {
            lowerBound = 0;

            anglesIEnum = anglesIEnum.Append(centerAngle);

            float boundDiff = upperBound - lowerBound;

            anglesIEnum = anglesIEnum.Concat(
                Enumerable.Range(1, NodeBase - 1).Select(n => lowerBound + boundDiff * n / NodeBase));
        }
        else if (upperBound > 6) {
            upperBound = MaxFrameSteer;
            float boundDiff = upperBound - lowerBound;

            anglesIEnum = anglesIEnum.Concat(
                Enumerable.Range(1, NodeBase - 1).Select(n => lowerBound + boundDiff * n / NodeBase));

            anglesIEnum = anglesIEnum.Append(centerAngle);
        }
        else {
            float boundDiff = (upperBound - lowerBound) / 2;

            anglesIEnum = anglesIEnum.Concat(
                Enumerable.Range(1, NodeBase - 1).Select(n => lowerBound + boundDiff * n / NodeBase));

            anglesIEnum = anglesIEnum.Append(centerAngle);

            anglesIEnum = anglesIEnum.Concat(
                Enumerable.Range(1, NodeBase - 1).Select(n => centerAngle + boundDiff * n / NodeBase));
        }

        var angles = anglesIEnum.Select(x => (float)Math.Round(x, 3)).Distinct().ToArray();
        var inds = new LineInd[angles.Length];
        int centerIndex = Array.IndexOf(angles, centerAngle);
        inds[centerIndex] = center;

        int farthestSurvival = 0;

        MyParallel.Run(0, inds.Length, GetInd);

        int best = IndexBest(inds);

        if (inds.Count(ind => ind.fitness == inds[best].fitness) > 1)
            return best == 0 ? inds[0] : inds.Last(ind => ind.fitness == inds[best].fitness);

        lowerBound = best == 0 ? (centerIndex == 0 ? -5 : lowerBound) : angles[best - 1];
        upperBound = best == inds.Length - 1 ? (centerIndex == inds.Length - 1 ? 10 : upperBound) : angles[best + 1];

        if (angles[1] - angles[0] <= precision)
            return inds[best];

        return BorderTestNode(borderIndex, lowerBound, upperBound, inds[best], angles[best]);

        void GetInd(int i)
        {
            if (i != centerIndex) {
                var candidate = new LineInd(this.best.angles, this.best.borders);
                candidate.borders[borderIndex.start] = (float)Math.Round(angles[i], 3);
                AdjustAngles(candidate, borderIndex, out int survival);
                farthestSurvival = Math.Max(survival, farthestSurvival);
                var sim = new FeatherSim(settings);
                sim.SimulateIndivitual(candidate.ToFrameGenes(indLen, timings));
                sim.Evaluate(out var fitness, out _);
                candidate.fitness = fitness;
                inds[i] = candidate;
            }
        }
    }

    #endregion

    private int IndexBest(LineInd[] inds)
    {
        int bestIndex = -1;
        var bestFitness = -99999999999999d;
        for (int i = 0; i < inds.Length; i++)
            if (inds[i].fitness > bestFitness)
                (bestIndex, bestFitness) = (i, inds[i].fitness);
        return bestIndex;
    }

    #region MainAngles

    private void AdjustAngles(LineInd ind, IntRange changeRegion, out int farthestSurvival, bool initialAngleFix = false)
    {
        int at = changeRegion.start;
        farthestSurvival = 0;
        if (changeRegion.start == 0) {
            bool success = AdjustAngle(ind, 0, changeRegion.end, out at, initialAngleFix);
            if (!success) {
                count++;
                return;
            }
        }

        while (at < changeRegion.end) {
            ind.SkippingState = new FeatherSim(settings).LineIndInfoAtFrame(ind, IndexTimings(at - 1) - 1, timings,
                (stop, fs, ws) => new Savestate(fs, ws));

            bool success = AdjustAngle(ind, at, changeRegion.end, out at, initialAngleFix);
            if (success) farthestSurvival = at;
            else break;
        }
        count++;
    }

    public const float angleIncrement = 1.024f;

    private bool AdjustAngle(LineInd ind, int index, int simTo, out int newIndex, bool initialAngleFix)
    {
        int checkingIndex = index;

        Func<bool> AnglePassedCheck;

        ExtendAndRestart:

        checkingIndex++;


        if (checkingIndex == simTo) {
            var currentFitness = GetFitness();
            float incr = angleIncrement;
            if (!TestAngle(out bool cwEq)) {
                incr = -incr;
                if (!TestAngle(out bool acEq)) {
                    if (cwEq & acEq) goto End;
                    else goto SkipConstantChange;
                }
            }

            while (TestAngle(out _)) { }
            SkipConstantChange:

            while (Math.Abs(incr) > 0.0015f) {
                incr /= 2;
                if (TestAngle(out var eq1))
                    continue;
                incr = -incr;
                if (!TestAngle(out var eq2) && (eq1 | eq2))
                    break;
            }

            goto End;

            double GetFitness()
            {
                var sim = new FeatherSim(settings);
                return sim.LineIndInfoAtFrame(ind, settings.Framecount, timings, sim.FitnessGetter);
            }

            bool TestAngle(out bool equal)
            {
                float originalAngle = ind.angles[index];
                SetAngle(ind.angles[index] + incr);

                var res = GetFitness();

                bool better = res > currentFitness;
                if (better) {
                    currentFitness = res;
                    equal = false;
                    return true;
                }

                equal = res == currentFitness;
                SetAngle(originalAngle);
                return false;
            }
        }


        int stateCheckFrame = IndexTimings(checkingIndex);

        bool initFailed = CurrentFails(out _);

        float angleDiff = GetAngleDiff();
        float increment = angleDiff > 0 ? angleIncrement : -angleIncrement;
        var oldPos = new Vector2();

        if (initFailed) {
            var oldCWPos = new Vector2();
            var oldACPos = new Vector2();
            var doCW = true;
            var doAC = true;
            increment = -angleIncrement;

            int iteration = 0;
            while (doCW | doAC) {
                if (doAC) { // anti-clockwise test
                    if (!TryAngle(out var ACPos)) break;
                    doAC = ACPos != oldACPos;
                    oldACPos = ACPos;
                }
                else if (GetAngleDiff() < 0) goto End;
                increment = -increment;
                if (doCW) { // clockwise test
                    if (!TryAngle(out var CWPos)) break;
                    doCW = CWPos != oldCWPos;
                    oldCWPos = CWPos;
                }
                else if (GetAngleDiff() > 0) goto End;
                increment = -increment - angleIncrement;

                if (Math.Abs(increment) > (initialAngleFix ? 180f : 20f)) {
                    newIndex = checkingIndex;
                    return false;
                }

                if ((!doCW & !doAC) && iteration == 0) {
                    doCW = true;
                    doAC = true;
                    SetAngle(ind.SkippingState.fState.spd.TASAngle);
                }
                iteration++;
            }

            if (!doCW & !doAC) {
                newIndex = checkingIndex;
                return false;
            }

            increment = increment > 0 ? -angleIncrement : angleIncrement;

            initFailed = false;

            AnglePassedCheck = () => false;
        }
        else {
            AnglePassedCheck = angleDiff > 0 ? (Func<bool>)
                  (() => GetAngleDiff() <= 0)
                : (() => GetAngleDiff() >= 0);

            while (TryAngle(out var pos)) {
                if (AnglePassedCheck()) {
                    SetAngle(ind.angles[checkingIndex]);
                    goto ExtendAndRestart;
                }
                if (pos == oldPos) goto End;
                oldPos = pos;
            }
        }

        while (Math.Abs(increment) > 0.0015f) {
            increment /= 2;
            TryAngle(out _);
        }

        End:

        newIndex = checkingIndex;
        return true;

        bool TryAngle(out Vector2 endPos)
        {
            float originalAngle = ind.angles[index];
            SetAngle(ind.angles[index] + increment);

            bool failed = CurrentFails(out endPos);

            SetAngle(failed ? originalAngle : ind.angles[index]);

            return failed == initFailed;
        }

        bool CurrentFails(out Vector2 endPos)
        {
            bool stop; int cps; List<int> wallboops;
            var sim = new FeatherSim(settings);
            (stop, cps, endPos, wallboops) = sim.LineIndInfoAtFrame(ind, stateCheckFrame, timings,
                (stop, fs, ws) => (stop, fs.checkpointsGotten, fs.ExactPosition, sim.wallboops));

            if (cps >= Level.Checkpoints.Length) return false;
            if (stop) return true;

            if ((baseInfo.Length >= stateCheckFrame | baseInfoFinishes)
                && cps < baseInfo[Math.Min(stateCheckFrame, baseInfo.Length - 1)].fState.checkpointsGotten)
                return true;

            return !TimingTester.ValidWallboops(baseInfoWallboops, wallboops, new IntRange(IndexTimings(index), stateCheckFrame));
        }

        float GetAngleDiff() => FeatherSim.DegreesDiff(ind.angles[index], ind.angles[checkingIndex]);

        void SetAngle(float a) => ind.angles.SetRange(a, index, checkingIndex);
    }

    private int IndexTimings(int i) => i == -1 ? 0 : i >= timings.Length ? indLen : timings[i];

    #endregion

    public AnglePerfector(SavedTiming src, float precision, int changedTiming, Settings settings)
    {
        this.precision = precision * 1.01f;
        best = new LineInd(src.ind.angles, src.ind.borders);
        current = best;
        timings = (int[])src.timings.Clone();
        indLen = settings.Framecount;
        this.settings = settings;
        AdjustAngles(best, range(0, timings.Length), out _, true);
        var sim = new FeatherSim(settings);
        sim.SimulateIndivitual(best.ToFrameGenes(indLen, timings));
        sim.Evaluate(out var fitness, out _);
        best.fitness = fitness;
        optimizingAt = changedTiming;
    }

    public IntRange range(int start, int end) => new IntRange(start, end);
}

public readonly struct IntRange
{
    public readonly int start;
    public readonly int end;

    public IntRange(int start, int end)
    {
        this.start = start;
        this.end = end;
    }

    public (int start, int end) Deconstruct() => (start, end);
}
