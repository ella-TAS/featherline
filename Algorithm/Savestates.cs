namespace Featherline;

public class Savestate
{
    public WindState wind;
    public FeatherState fState;

    public Savestate(FeatherState fState, WindState wind)
    {
        this.wind = wind.Copy();
        this.fState = fState.Copy();
    }

    public Savestate()
    {
        wind = new WindState();
        fState = new FeatherState();
    }

    public Savestate Copy() => new Savestate(fState, wind);
}

public partial class FeatherSim
{
    public Savestate[] GetAllFrameData(AngleSet ind, out bool finishes, out int[] wallboops)
    {
        this.ind = ind;

        LoadSavestate(Level.StartState);
        var states = new List<Savestate>();

        while (fs.f < ind.Length) {
            RunFrame(ind[fs.f]);
            if (stop)
                break;
            states.Add(new Savestate(fs, wind));
        }

        finishes = fs.checkpointsGotten == Level.Checkpoints.Length;
        wallboops = this.wallboops.ToArray();

        return states.ToArray();
    }

    public T TryGetInfoAtFrame<T>(AngleSet ind, int f, Func<bool, FeatherState, WindState, T> GetInfo)
    {
        this.ind = ind;

        LoadSavestate(Level.StartState);

        while (fs.f < f) {
            RunFrame(ind[fs.f]);
            if (stop) break;
        }

        return GetInfo(stop, fs, wind);
    }

    public bool TryGetStateAtFrame(AngleSet ind, int f, out Savestate state)
    {
        if (f < 0) {
            state = null;
            return true;
        }
        bool stop;
        (stop, state) = TryGetInfoAtFrame(ind, f,
            (stop, fs, ws) => (stop, new Savestate(fs, ws)));
        return stop;
    }

    public T LineIndInfoAtFrame<T>(LineInd ind, int f, int[] timings, Func<bool, FeatherState, WindState, T> GetInfo)
    {
        var skipState = ind.SkippingState;
        LoadSavestate(skipState is null || skipState.fState.f >= f ? Level.StartState : skipState);

        int nextTimingIndex = 0;
        int nextTiming = 0;

        bool foundTiming = false;
        for (int i = 0; i < timings.Length; i++) {
            if (fs.f < timings[i]) {
                nextTimingIndex = i;
                nextTiming = timings[i] - 1;
                foundTiming = true;
                break;
            }
        }
        if (!foundTiming) {
            nextTiming = 99999999;
            nextTimingIndex = -1;
        }

        if (fs.checkpointsGotten < Level.Checkpoints.Length) {
            while (fs.f < f) {
                var angle = GetCurrentAngle();
                RunFrame(angle);
                if (stop) break;
            }
        }

        return GetInfo(stop, fs, wind);

        float GetCurrentAngle()
        {
            if (fs.f == nextTiming) {
                float res = ind.angles[nextTimingIndex];
                float angDiff = DegreesDiff(ind.angles[nextTimingIndex], ind.angles[nextTimingIndex + 1]);
                if (Math.Abs(angDiff) > 5.334) {
                    res += angDiff > 0 ? ind.borders[nextTimingIndex] : -ind.borders[nextTimingIndex];
                    res = res < 0f ? res + 360f : res >= 360f ? res - 360f : res;
                }

                nextTimingIndex++;
                nextTiming = nextTimingIndex == timings.Length ? 99999999 : timings[nextTimingIndex] - 1;
                return res;
            }
            else
                return ind.angles[nextTimingIndex];
        }
    }

    public double FitnessGetter(bool a, FeatherState b, WindState c)
    {
        Evaluate(out var res, out _);
        return res;
    }
}
