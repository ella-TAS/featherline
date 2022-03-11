namespace Featherline;

public partial class FeatherSim
{
	public const float DeltaTime = 0.0166667f;

	public Settings sett;
	public FeatherSim(Settings s)
	{
		sett = s;
		CheckDeath = Level.HasHazards ? (Action)DeathCheck : () => { };
	}
	public FeatherSim AddInputCleaner(bool extremeTurns)
	{
		inputCleaner = new InputCleaner(this, extremeTurns);
		return this;
	}

	public void Debug(AngleSet ind)
	{
		AddInputCleaner(false);
		this.ind = ind;

		LoadSavestate(Level.StartState);
		int prevCPs = 0;
		int prevBoops = 0;
		bool cpNotify = true;

		bool evaluated = false;
		double fitness = 0d;

		while (fs.f < ind.Length) {
			RunFrame(ind[fs.f]);
			fs.Print();

			if (wallboops.Count() > prevBoops)
				Console.WriteLine("Collided with a wall.");

			if (fs.checkpointsGotten > prevCPs) {
				if (fs.checkpointsGotten >= Level.Checkpoints.Length) {
					if (!evaluated) {
						Evaluate(out fitness, out _);
						evaluated = true;
					}
					stop = false;
					if (cpNotify)
						Console.WriteLine("Finish checkpoint collected.");
					fs.checkpointsGotten = 0;
					cpNotify = false;
				}
				else if (cpNotify)
					Console.WriteLine($"Collected checkpoint {fs.checkpointsGotten}.");
			}

			if (stop) {
				if (!evaluated) {
					Evaluate(out fitness, out _);
					evaluated = true;
				}
				Console.WriteLine("Died.");
				stop = false;
			}

			prevCPs = fs.checkpointsGotten;
			prevBoops = wallboops.Count();
		}

		if (!evaluated)
			Evaluate(out fitness, out _);

		Console.WriteLine("\nInputs after cleanup algorithm:\n");
		Console.WriteLine(ind.ToString(ind.Length));
		Console.WriteLine("\nFitness: " + fitness.FitnessFormat());
	}

	public FeatherState fs;
	private WindState wind;
	public AngleSet ind;

	private InputCleaner inputCleaner;

	private (double closestDist, int atFrame) fitEval = (9999999, 0);

	private bool stop = false;

	public FeatherSim SimulateIndivitual(AngleSet ind, int frameCount = 99999999, Savestate ss = null)
	{
		this.ind = ind;

		frameCount = Math.Min(ind.Length, frameCount);

		LoadSavestate(ss ?? Level.StartState);

		if (fs.checkpointsGotten < Level.Checkpoints.Length) {
			while (fs.f < frameCount) {
				RunFrame(ind[fs.f]);
				if (stop) break;
			}
		}

		return this;
	}

	private void RunFrame(float aim)
	{
		fs.previousPos = fs.ExactPosition;

		fs.aim = AngleCalc.GetAim(aim);//Vector2.FromTasAngle(aim, 1);
		UpdateAngle();
		inputCleaner?.Update();
		fs.f++;

		UpdatePosition();

		AnalyzeProgress();
		CheckDeath();
		if (stop) {
			inputCleaner?.Update(true);
			return;
		}

		WindFrame();
	}

	public void Evaluate(out double fitness, out int fCount)
	{
		int cpExtras = fs.checkpointsGotten * 10000;
		if (fs.checkpointsGotten >= Level.Checkpoints.Length) {
			fs.checkpointsGotten--;
			double nextCpDist = Level.Checkpoints[Level.Checkpoints.Length - 1].GetFinalCPDistance(fs.ExactPosition, fs.previousPos, out bool touched);

			// prevents a small bug where the sim got the checkpoint according to intPos collision but not with final cp dist check,
			// making the result 0 instead of 3 + 1/6 pixels that it should be
			if (!touched)
				nextCpDist = 3.16666d;

			fitness = cpExtras - fs.f * 8 - nextCpDist;
			fCount = fs.f + 1;
		}
		else {
			double nextCpDist = fs.GetDistToNextCp();
			fitness = cpExtras - fitEval.closestDist * 2d - fitEval.atFrame - nextCpDist - fs.f;
			fCount = sett.Framecount;
		}
	}

	private void AnalyzeProgress() // return value -> whether simulation is finished
	{
		var dist = fs.GetDistToNextCp();
		fitEval = dist < fitEval.closestDist ? (dist, fs.f) : fitEval;

		if (Level.Checkpoints[fs.checkpointsGotten].TouchingAsFeather(fs.pos)) {
			fs.checkpointsGotten++;
			if (fs.checkpointsGotten >= Level.Checkpoints.Length)
				stop = true;
			fitEval.closestDist = 999999;
		}
	}

	private void LoadSavestate(Savestate ss)
	{
		wind = ss.wind;
		fs = ss.fState;
		if (inputCleaner != null)
			inputCleaner.lastFrameAngle = ss.fState.spd.TASAngle;
	}

	const float MaxFrameTurnRadians = 5.5850534f * DeltaTime;
	public static Vector2 RotateTowards(Vector2 vec, float target)
	{
		float orig = vec.Angle();
		float diff = target - orig;
		diff = diff > 3.1415927f ? diff - 6.2831855f : diff <= -3.1415927f ? diff + 6.2831855f : diff;
		float resAngle = Math.Abs(diff) < MaxFrameTurnRadians ? target : orig + (diff > 0 ? MaxFrameTurnRadians : -MaxFrameTurnRadians);
		float len = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
		return new Vector2((float)Math.Cos(resAngle) * len, (float)Math.Sin(resAngle) * len);
	}

	public static float RadiansDiff(float radiansA, float radiansB)
	{
		float d = radiansB - radiansA;
		return d > 3.1415927f ? d - 6.2831855f : d <= -3.1415927f ? d + 6.2831855f : d;
	}

	public static float DegreesDiff(float actual, float target)
	{
		float d = target - actual;
		return d > 180f ? d - 360f : d <= -180f ? d + 360f : d;
	}

	// refactored StarFlyUpdate()
	const float featherAccel = 1000f * DeltaTime;
	private void UpdateAngle()
	{
		// get the length of and get the normalized (on unit circle) speed vector
		float spdLen = (float)Math.Sqrt(fs.spd.X * fs.spd.X + fs.spd.Y * fs.spd.Y);
		float inverse = 1f / spdLen;

		// normalize and rotate by max 5.33334 degrees
		var normalized = RotateTowards(new Vector2(fs.spd.X * inverse, fs.spd.Y * inverse), fs.aim.Angle);

		// update the lerp and find new target speed
		fs.lerp += DeltaTime;
		fs.lerp = fs.lerp < 1f ? fs.lerp : 1f;
		float target = 140f + (190f - 140f) * fs.lerp;

		// accelerate towards speed dictated by lerp
		spdLen = spdLen <= target
			? spdLen + featherAccel < target ? spdLen + featherAccel : target
			: spdLen - featherAccel > target ? spdLen - featherAccel : target;

		// update speed value
		fs.spd = normalized * spdLen;
	}
}

public class FeatherState
{
	public int f = 0;
	public float lerp = 0f;

	public Vector2 moveCounter;
	public Vector2 previousPos;
	public Aim aim;
	public Vector2 spd;

	public int checkpointsGotten = 0;

	public IntVec2 pos = new IntVec2();

	public void MoveH()
	{
		int increment = moveCounter.X > 0 ? 1 : -1;
		int count = (int)Math.Round(moveCounter.X);
		pos.X += count;
		count = count > 0 ? count : -count;
		for (int i = 0; i < count; i++)
			moveCounter.X -= increment;
	}

	public void MoveV()
	{
		int increment = moveCounter.Y > 0 ? 1 : -1;
		int count = (int)Math.Round(moveCounter.Y);
		pos.Y += count;
		count = count > 0 ? count : -count;
		for (int i = 0; i < count; i++)
			moveCounter.Y -= increment;
	}

	public Vector2 ExactPosition => pos + moveCounter;

	public FeatherState Copy()
	{
		return new FeatherState() {
			f = f,
			lerp = lerp,
			moveCounter = moveCounter,
			aim = aim,
			spd = spd,
			checkpointsGotten = checkpointsGotten,
			pos = new IntVec2(pos.X, pos.Y)
		};
	}

	public double GetDistToNextCp() =>
		checkpointsGotten == Level.Checkpoints.Length - 1
		? Level.Checkpoints[Level.Checkpoints.Length - 1].GetFinalCPDistance(pos + moveCounter, previousPos, out _)
		: Math.Sqrt((Level.Checkpoints[checkpointsGotten].center.X - pos.X).Square() + (Level.Checkpoints[checkpointsGotten].center.Y - pos.Y).Square());

	public void Print()
	{
		Console.Write($"f{f}, pos {ExactPosition}, ");
		Console.ForegroundColor = ConsoleColor.Blue;
		Console.Write($"spd {spd}, ");
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.Write($"angle {spd.TASAngle}, ");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.Write($"lerp {lerp}, ");
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($"aim {aim}");
		Console.ForegroundColor = ConsoleColor.White;
	}
}
