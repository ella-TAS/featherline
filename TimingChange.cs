using System.Collections.Generic;

namespace Featherline
{
    class TimingChange
    {
        private int i;
        private int increment;

        private int beforeChange;
        private int[] target;

        public bool ApplyTo(int[] timings, List<SingleTiming> notSame = null)
        {
            beforeChange = timings[i];
            target = timings;

            timings[i] += increment;

            if (notSame != null)
                for (int j = 0; j < notSame.Count; j++)
                    if (notSame[j].index == i && notSame[j].value == timings[i]) {
                        timings[i] = beforeChange;
                        return false;
                    }

            return Validate();
        }

        private bool Validate()
        {
            if (i == 0) {
                if (target[i] < 0) target[i] = 0;
                else RightFix();
            }
            else if (i == target.Length - 1) {
                int cap = GAManager.settings.Framecount - 1;
                if (target[i] > cap) target[i] = cap;
                else LeftFix();
            }
            else {
                LeftFix();
                RightFix();
            }

            // if applying then validating the change changed nothing, the timing change was invalid.
            return beforeChange != target[i];

            void LeftFix() { if (target.Length > 1 && target[i] <= target[i - 1]) target[i] = target[i - 1] + 1; }
            void RightFix() { if (target.Length > 1 && target[i] >= target[i + 1]) target[i] = target[i + 1] - 1; }
        }

        public void TakeBack() => target[i] = beforeChange;

        public bool ApplyInverse(List<SingleTiming> notSame = null) {
            target[i] = beforeChange - increment;

            if (notSame != null)
                for (int j = 0; j < notSame.Count; j++)
                    if (notSame[j].index == i && notSame[j].value == target[i]) {
                        target[i] = beforeChange;
                        return false;
                    }

            return Validate();
        }

        public TimingChange(int index, int increment)
        {
            i = index;
            this.increment = increment;
        }

        public override string ToString() => target is null ? $"i{i}: {increment}" : $"i{i}: {beforeChange} -> {beforeChange + increment}";
    }

    public struct SingleTiming
    {
        public int index;
        public int value;

        public SingleTiming(int index, int value)
        {
            this.index = index;
            this.value = value;
        }
    }
}
