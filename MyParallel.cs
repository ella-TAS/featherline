namespace Featherline;

static class MyParallel
{
    public static Action<int, int, Action<int>>? Run;

    public static void Initialize(Settings sett)
    {
        var opt = new ParallelOptions() { MaxDegreeOfParallelism = sett.MaxThreadCount };

        Run = sett.MaxThreadCount == 1 ? NonParallel : (start, to, Act) => Parallel.For(start, to, opt, Act);

        void NonParallel(int start, int to, Action<int> Act)
        {
            for (int i = start; i < to; i++)
                Act(i);
        }
    }
}
