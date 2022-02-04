using System.Collections.Generic;
using System;

namespace Featherline
{
    public class Savestate
    {
        public WindState wind;
        public FeatherSim.FState fState;

        public Savestate(FeatherSim.FState fState, WindState wind)
        {
            this.wind = wind.Copy();
            this.fState = fState.Copy();
        }

        public Savestate Copy() => new Savestate(fState, wind);
    }

    public partial class FeatherSim
    {
        public Savestate[] GetAllFrameData(float[] ind, out bool finishes, out int[] wallboops)
        {
            this.ind = ind;
            cleaningInputs = false;

            InitializeSim();
            var states = new List<Savestate>();

            while (si.f < ind.Length) {
                RunFrame(ind[si.f]);
                if (stop)
                    break;
                states.Add(new Savestate(si, wind));
            }

            finishes = si.checkpointsGotten == Level.Checkpoints.Length;
            wallboops = this.wallboops.ToArray();

            return states.ToArray();
        }

        public T TryGetInfoAtFrame<T>(float[] ind, int f, Func<bool, FState, WindState, T> GetInfo)
        {
            this.ind = ind;
            cleaningInputs = false;

            InitializeSim();

            while (si.f < f) {
                RunFrame(ind[si.f]);
                if (stop) break;
            }

            return GetInfo(stop, si, wind);
        }

        public bool TryGetStateAtFrame(float[] ind, int f, out Savestate state)
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

        public T LineIndInfoAtFrame<T>(LineInd ind, int f, int[] timings, Func<bool, FState, WindState, T> GetInfo)
        {
            if (ind.SkippingState is null)
                InitializeSim();
            else
                LoadSavestate(ind.SkippingState);

            int nextTimingIndex = 0;
            int nextTiming = 0;

            bool foundTiming = false;
            for (int i = 0; i < timings.Length; i++) {
                if (si.f < timings[i]) {
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

            if (si.checkpointsGotten < Level.Checkpoints.Length) {
                while (si.f < f) {
                    RunFrame(GetCurrentAngle());
                    if (stop) break;
                }
            }

            return GetInfo(stop, si, wind);

            float GetCurrentAngle()
            {
                if (si.f == nextTiming) {
                    float res = ind.angles[nextTimingIndex];
                    float angDiff = DegreesDiff(ind.angles[nextTimingIndex], ind.angles[nextTimingIndex + 1]);
                    if (Math.Abs(angDiff) > 5.334)
                        res += angDiff > 0 ? ind.borderExtras[nextTimingIndex] : -ind.borderExtras[nextTimingIndex];

                    nextTimingIndex++;
                    nextTiming = nextTimingIndex == timings.Length ? 99999999 : timings[nextTimingIndex] - 1;
                    return res;
                }
                else
                    return ind.angles[nextTimingIndex];
            }
        }

        public double FitnessGetter(bool a, FState b, WindState c)
        {
            Evaluate(out var res, out _);
            return res;
        }
    }
} 