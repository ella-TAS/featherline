using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace Featherline;

public static class GAManager
{
    #region WindowManipulationImports

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();

    [DllImport("user32.dll")]
    static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    #endregion

    public const float Revolution = 360f;

    public static Settings settings;
    public static Random rand = new Random();

    private static FrameGenesGA ga;
    private static int generation;

    private static Stopwatch algTimer = new Stopwatch();

    private static IntPtr consoleWindow;

    public static bool abortAlgorithm;

    public static List<(AngleSet inputs, double fitness, AlgPhase source)> finalResultCandidates;
    public static AlgPhase lastPhase;

    private static Control Button;

    public static bool RunAlgorithm(Form1 sender, Control button, bool debugFavorite)
    {
        Button = button;
        abortAlgorithm = false;
        algTimer.Restart();
        if (!InitializeAlgorithm())
            return false;
        algTimer.Stop();
        Console.WriteLine("Preparing the algorithm took " + algTimer.Elapsed);
        algTimer.Restart();

        finalResultCandidates = new List<(AngleSet, double, AlgPhase)>();

        if (debugFavorite) {
            if (!ValidFavDecPlaces()) {
                Console.WriteLine("Featherline cannot handle more than 3 decimal points.");
                return false;
            }
            var initGenes = RawFavorite(settings.Favorite);

            if (initGenes is null) {
                return false;
                Console.WriteLine("No initial inputs to debug");
            }
            else
                new FeatherSim(settings).Debug(initGenes);

            return false;
        }

        if (settings.FrameBasedOnly | !settings.TimingTestFavDirectly) {
            DoFrameGeneBasedAlgorithm();
            if (abortAlgorithm) {
                Console.Write("\n\n");
                return true;
            }
            Console.WriteLine("\nBasic Algorithm Finished!\n");
        }

        if (!settings.FrameBasedOnly) {
            TimingTester TT;
            if (settings.TimingTestFavDirectly) {
                if (!ValidFavDecPlaces()) {
                    Console.WriteLine("Featherline cannot handle more than 3 decimal points.");
                    return false;
                }
                if (ParseFavorite(settings.Favorite, settings.Framecount) is null)
                    Console.WriteLine("No initial inputs to test timings on.");
                else {
                    TT = new TimingTester(settings, settings.Favorite);
                    TT.Run();
                }
            }
            else {
                Level.PermanentDistFilter(ga.inds[0].genes);
                TT = new TimingTester(settings, ga.inds[0]);
                TT.Run();
            }
        }

        if (!abortAlgorithm)
            FlashWindow(consoleWindow, true);

        return true;
    }

    #region AlgActions

    private static bool InitializeAlgorithm()
    {
        AllocConsole();
        Console.Title = "Featherline Output Console";
        Console.Clear();

        consoleWindow = GetConsoleWindow();
        ShowWindow(consoleWindow, 1);
        SetForegroundWindow(consoleWindow);

        // data extraction and input exception handling
        try {
            Level.Prepare(Form1.settings);
        }
        catch (ArgumentException e) {
            Console.WriteLine(e.Message);
            return false;
        }
        catch (Exception e) {
            Console.WriteLine(e.ToString());
            Console.WriteLine("\nThe extracted information is either invalid or an exception occured during the setup process otherwise.\nPing TheRoboMan on the Celeste discord if you think this shouldnt have happened.");
            return false;
        }
        finally { }
        if (Level.Checkpoints.Length == 0) {
            Console.WriteLine("No valid checkpoints were provided; the algorithm has nothing to aim for.");
            return false;
        }

        settings = Form1.settings.Copy();
        MyParallel.Initialize(settings);
        AngleCalc.Initialize();
        return true;
    }

    public static void EndAlgorithm()
    {
        algTimer.Stop();

        if (finalResultCandidates.Count > 0) {
            var best = finalResultCandidates.OrderByDescending(r => r.fitness).First();
            Console.WriteLine("Finished with best fitness " + best.fitness.FitnessFormat());

            if (lastPhase != best.source)
                Console.WriteLine("Result from " + best.source switch {
                    AlgPhase.FrameGenes => "frame genes GA.",
                    AlgPhase.TimingTesterLight => "light timing tester.",
                    AlgPhase.TimingTesterHeavy => "heavy timing tester.",
                    AlgPhase.AnglePerfector => "angle perfector.",
                    _ => "[error lol]."
                });

            new FeatherSim(settings)
                .AddInputCleaner(best.source == AlgPhase.FrameGenes)
                .SimulateIndivitual(best.inputs)
                .Evaluate(out _, out int fCount);

            var output = best.inputs.ToString(fCount);
            Console.WriteLine("\n" + output);

            // log in result log.txt
            if (settings.LogResults && !(algTimer.ElapsedMilliseconds < 5000 && abortAlgorithm)) {
                try {
                    var logFile = File.Exists("result log.txt")
                        ? File.AppendText("result log.txt")
                        : new StreamWriter(File.Create("result log.txt"));
                    logFile.WriteLine($"{DateTime.Now}\nBeginning position: {Level.startState.fState.ExactPosition}\n\n{output}");
                    logFile.Close();
                }
                catch {
                    Console.WriteLine("Failed to log the result to result log.txt");
                }
            }

            if (!abortAlgorithm) {
                Button.Invoke((Action)(() => Clipboard.SetText(output)));
                Console.WriteLine("Copied to your clipboard!");
            }
        }

        Console.WriteLine($"\nAlgorithm took {algTimer.Elapsed} to run.");
    }

    public static void ClearAlgorithmData()
    {
        Level.Spinners = null;
        Level.DeathMap = null;
        Level.Tiles = null;
        Level.WindTriggers = null;
        Level.Checkpoints = null;
        Level.Colliders = null;
        Level.Killboxes = null;
        Level.Spikes = null;
        settings = null;
        ga = null;
        AnglePerfector.baseInfo = null;
        AnglePerfector.baseInfoWallboops = null;
        AngleCalc.Reset();
        Level.NormalJTs = null;
        Level.CustomJTs = null;
        Level.startState = null;
        finalResultCandidates = null;
        GC.Collect();
    }

    #endregion

    #region FrameBasedAlgorithm

    private static void DoFrameGeneBasedAlgorithm()
    {
        lastPhase = AlgPhase.FrameGenes;
        var fav = RawFavorite(settings.Favorite);
        int startAt = Math.Max(5, fav is null ? 0 : fav.Length);

        ga = new FrameGenesGA(settings, startAt);
        generation = 1;

        DoGensWhileSimulationsGetLonger(startAt);

        NormalGenerations();

        finalResultCandidates.Add((ga.inds[0].genes, ga.inds[0].fitness, AlgPhase.FrameGenes));
    }

    private static void DoGensWhileSimulationsGetLonger(int startAt)
    {
        int gensForIncreasingFrameCount = Math.Max(1, settings.Generations / 2);

        int divisor = settings.Framecount - startAt;
        if (divisor == 0) return;

        int gensPerFrame = gensForIncreasingFrameCount / divisor;
        for (int i = startAt; i < settings.Framecount; i++) {
            for (int j = 0; j < gensPerFrame; j++) {
                if (abortAlgorithm) return;
                ga.DoGeneration(i, true);
                GenerationFeedback(generation, settings.Generations, ga.GetBestFitness());
                generation++;
            }
        }
    }

    private static void NormalGenerations()
    {
        for (; generation <= settings.Generations; generation++) {
            if (abortAlgorithm) return;
            GenerationFeedback(generation, settings.Generations, ga.GetBestFitness());
            ga.DoGeneration(settings.Framecount, true);
        }
    }

    #endregion

    #region InputConverting

    public static AngleSet ParseFavorite(string src, int targetLen)
    {
        var res = RawFavorite(src);

        if (res is null) return null;

        if (res.Length > targetLen)
            return res.Take(targetLen).ToAngleSet();
        else if (res.Length < targetLen)
            return res.Concat(new float[targetLen - res.Length].Select(n => (float)(rand.NextDouble() * Revolution))).ToAngleSet();

        return res.ToAngleSet();
    }

    public static AngleSet? RawFavorite(string src)
    {
        var matches = Regex.Matches(src, @"(\d+),F,(\d*\.?\d*)");
        if (matches.Count == 0) return null;
        return matches.SelectMany(m =>
            Enumerable.Repeat(
                m.Groups[2].Length == 0 ? 0f : float.Parse(m.Groups[2].Value),
                int.Parse(m.Groups[1].Value)))
            .ToAngleSet();

        /*float[] res = { };


        foreach (Match m in matches) {
            int fCount = int.Parse(m.Groups[1].Value);
            var angle = m.Groups[2].Length == 0 ? 0f : float.Parse(m.Groups[2].Value);
            res = res.Concat(Enumerable.Repeat(angle, fCount)).ToArray();
        }
        return res.ToAngleSet();*/
    }

    public static string ToString(this AngleSet inputs, int fCount)
    {
        inputs = inputs[..Math.Min(fCount - (fCount >= inputs.Length ? 0 : 1), inputs.Length)];
        var sb = new StringBuilder();

        float lastAngle = inputs[0];
        int consecutive = 1;
        foreach (var f in inputs.Skip(1)) {
            if (f != lastAngle) {
                sb.AppendLine($"{consecutive,4},F,{lastAngle.ToString().Replace(',', '.')}");
                lastAngle = f;
                consecutive = 1;
            }
            else
                consecutive++;
        }
        sb.AppendLine($"{consecutive,4},F,{lastAngle.ToString().Replace(',', '.')}");

        return sb.ToString();
    }

    private static bool ValidFavDecPlaces()
    {
        foreach (Match m in Regex.Matches(settings.Favorite, @"\d,F,\d*\.?(\d*)"))
            if (m.Groups[1].Length > 3)
                return false;
        return true;
    }

    #endregion

    #region Feedback

    private static readonly Regex numParse = new Regex(@"\.\d+", RegexOptions.Compiled);
    public static string FitnessFormat(this double val)
    {
        var raw = val.ToString();
        if (raw.Contains('E')) return raw;
        return raw.Contains('.')
            ? numParse.Replace(raw, m => m.Length <= 6 ? m.Value.PadRight(6, '0') : m.Value[..6])
            : raw + ".00000";
    }

    public static void GenerationFeedback(int gen, int maxGens, double fitness) =>
        Console.Write($"\r{gen}/{maxGens} generations done. Best fitness: {fitness.FitnessFormat()}");

    #endregion
}
