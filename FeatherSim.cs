using System;
using System.Linq;
using System.Diagnostics;

namespace Featherline
{
	public partial class FeatherSim
	{
		public const float DeltaTime = 0.0166667f;

		public Settings sett;
		public FeatherSim(Settings s)
		{
			sett = s;
			CheckDeath = Level.HasHazards ? (Action)DeathCheck : () => { };
		}

		public void Debug(AngleSet ind)
        {
			this.ind = ind;
			cleaningInputs = false;

			LoadSavestate(Level.StartState);
			int prevCPs = 0;
			int prevBoops = 0;
			bool cpNotify = true;
			while (si.f < ind.Length) {
				RunFrame(ind[si.f]);
				si.Print();

				if (wallboops.Count() > prevBoops)
					Console.WriteLine("Collided with a wall.");

				if (si.checkpointsGotten > prevCPs) {
					if (si.checkpointsGotten >= Level.Checkpoints.Length) {
						stop = false;
						if (cpNotify)
							Console.WriteLine("Finish checkpoint collected.");
						si.checkpointsGotten = 0;
						cpNotify = false;
					}
					else if (cpNotify)
						Console.WriteLine($"Collected checkpoint {si.checkpointsGotten}.");
				}

				if (stop) {
					Console.WriteLine("Died.");
					stop = false;
				}

				prevCPs = si.checkpointsGotten;
				prevBoops = wallboops.Count();
			}
		}

		private FeatherState si;
		private WindState wind;
		private AngleSet ind;

		private (double closestDist, int atFrame) fitEval = (9999999, 0);

		private bool cleaningInputs;
		private bool stop = false;

		public FeatherSim SimulateIndivitual(AngleSet ind, bool cleaningInputs = false, int frameCount = 99999999, Savestate ss = null)
		{
			this.ind = ind;
			this.cleaningInputs = cleaningInputs;

			frameCount = Math.Min(ind.Length, frameCount);

			LoadSavestate(ss ?? Level.StartState);

			if (si.checkpointsGotten < Level.Checkpoints.Length) {
				while (si.f < frameCount) {
					RunFrame(ind[si.f]);
					if (stop) break;
				}
			}

			return this;
		}

		public void Evaluate(out double fitness, out int fCount)
        {
			int cpExtras = si.checkpointsGotten * 10000;
			if (si.checkpointsGotten >= Level.Checkpoints.Length) {
				si.checkpointsGotten--;
				double nextCpDist = Level.Checkpoints[Level.Checkpoints.Length - 1].GetFinalCPDistance(si.ExactPosition, si.previousPos, out bool touched);

				// prevents a small bug where the sim got the checkpoint according to intPos collision but not with final cp dist check,
				// making the result 0 instead of 3 + 1/6 pixels that it should be
				if (!touched)
					nextCpDist = 3.16666d;

				fitness = cpExtras - si.f * 8 - nextCpDist;
				fCount = si.f + 1;
			}
			else {
				double nextCpDist = si.GetDistToNextCp();
				fitness = cpExtras - fitEval.closestDist * 2d - fitEval.atFrame - nextCpDist - si.f;
				fCount = sett.Framecount;
			}
		}

		private void RunFrame(float aim)
		{
			si.previousPos = si.ExactPosition;

			InputCleaning();

			si.aim = PreCalc.GetAim(aim);//Vector2.FromTasAngle(aim, 1);
			UpdateAngle();
			si.f++;

			UpdatePosition();

			AnalyzeProgress();
			CheckDeath();
			if (stop) {
				InputCleaning(true);
				return;
			}

			WindFrame();
		}

		private void AnalyzeProgress() // return value -> whether simulation is finished
		{
			var dist = si.GetDistToNextCp();
			fitEval = dist < fitEval.closestDist ? (dist, si.f) : fitEval;

			if (Level.Checkpoints[si.checkpointsGotten].TouchingAsFeather(si.pos)) {
				si.checkpointsGotten++;
				if (si.checkpointsGotten >= Level.Checkpoints.Length)
					stop = true;
				fitEval.closestDist = 999999;
			}
		}

		int turningStart;
		float angleBeforeTurn;
		private void InputCleaning(bool forceClean = false)
		{
			if (cleaningInputs) {
				if (forceClean && si.f >= ind.Length) return;

				ind[si.f] += ind[si.f] < 0f ? 360f : 0f;
				ind[si.f] -= ind[si.f] >= 360f ? 360f : 0f;

				// hard disable slow steep turns
				float actualAngle = si.spd.TASAngle;
				float turnForce = DegreesDiff(actualAngle, ind[si.f]);

				TurnState current = turnForce > 5.333 ? TurnState.Clockwise : turnForce < -5.333 ? TurnState.AntiClockwise : TurnState.None;
				if (current != turning || forceClean) {
					if (turning == TurnState.None) {
						// begin a turn
						turningStart = si.f;
						angleBeforeTurn = actualAngle;
					}
					else {
						// end the turn
						int i = turningStart;
						float newAngle = (float)Math.Round(angleBeforeTurn);
						while (true) {
							if (si.f - i < 11) {
								if (sett.FrameBasedOnly) {
									newAngle = turning == TurnState.Clockwise ? (float)Math.Ceiling(actualAngle + 1) : (float)Math.Floor(actualAngle - 1);
									for (; i < si.f; i++)
										ind[i] = newAngle;
								}
								break;
							}

							newAngle += 60 * (int)turning;
							newAngle = newAngle < 0f ? newAngle + 360f : newAngle;
							newAngle = newAngle >= 360f ? newAngle - 360f : newAngle;

							int lineTarget = i + 11;
							for (; i < lineTarget; i++)
								ind[i] = newAngle;
						}

						if (current != TurnState.None) {
							// begin a turn
							angleBeforeTurn = actualAngle;
							turningStart = si.f;
						}
					}
					turning = current;
				}
			}
		}

		private void LoadSavestate(Savestate ss)
		{
			wind = ss.wind;
			si = ss.fState;
		}

		const float MaxMove = 5.5850534f * DeltaTime;
		public static Vector2 RotateTowards(Vector2 vec, float target)
		{
			float orig = vec.Angle();
			float diff = target - orig;
			diff = diff > 3.1415927f ? diff - 6.2831855f : diff <= -3.1415927f ? diff + 6.2831855f : diff;
			float resAngle = Math.Abs(diff) < MaxMove ? target : orig + (diff > 0 ? MaxMove : -MaxMove);
			float len = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
			return new Vector2((float)Math.Cos(resAngle) * len, (float)Math.Sin(resAngle) * len);
		}

		public static float AngleDiff(float radiansA, float radiansB)
		{
			float d = radiansB - radiansA;
			return d > 3.1415927f ? d - 6.2831855f : d <= -3.1415927f ? d + 6.2831855f : d;
		}

		public static float DegreesDiff(float actual, float target)
		{
			float v1 = target - actual;
			float v2 = v1 > 0 ? v1 - 360 : 360f + v1;
			return Math.Abs(v1) < Math.Abs(v2) ? v1 : v2;
		}

		public static float Clamp(float value, float min, float max)
		{
			value = (value > max) ? max : value;
			value = (value < min) ? min : value;
			return value;
		}

		public static float Approach(float val, float target, float maxMove) =>
			val <= target
				? Math.Min(val + maxMove, target)
				: Math.Max(val - maxMove, target);

		/// Linearly interpolates between two values.
		public static float Lerp(float value1, float value2, float amount) => value1 + (value2 - value1) * amount;

		// refactored StarFlyUpdate()
		private void UpdateAngle()
		{
			float spdLen = (float)Math.Sqrt(si.spd.X * si.spd.X + si.spd.Y * si.spd.Y);
			float inverse = 1f / spdLen;
			var normalized = new Vector2(si.spd.X * inverse, si.spd.Y * inverse);
			normalized = RotateTowards(normalized, si.aim.Angle); // 5.33334 degrees

			// curving and speed acceleration
			float target;
			//if (!sett.EnableSteepTurns || Vector2.Dot(vector, si.aim.Value) >= .45f) // angle after rotating < acos(.45)
			//{
				si.lerp += DeltaTime;
				si.lerp = si.lerp < 1f ? si.lerp : 1f;
				target = 140f + (190f - 140f) * si.lerp;
			//}
			//else { // don't go here
			//si.speedLerp = 0f;
			//target = 140f; // approach 140 speed
			//}

			// update speed
			spdLen = Approach(spdLen, target, 1000f * DeltaTime);
			si.spd = normalized * spdLen;
		}

		// for input cleanup
		private enum TurnState
		{
			None = 0,
			Clockwise = 1,
			AntiClockwise = -1
		}

		TurnState turning = TurnState.None;
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
			count = count > 0 ? count : -count;
			for (int i = 0; i < count; i++) {
				moveCounter.X -= increment;
				pos.X += increment;
            }
        }

		public void MoveV()
		{
			int increment = moveCounter.Y > 0 ? 1 : -1;
			int count = (int)Math.Round(moveCounter.Y);
			count = count > 0 ? count : -count;
			for (int i = 0; i < count; i++) {
				moveCounter.Y -= increment;
				pos.Y += increment;
			}
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
			: Math.Sqrt(
				Math.Pow(Level.Checkpoints[checkpointsGotten].center.X - pos.X, 2)
			  + Math.Pow(Level.Checkpoints[checkpointsGotten].center.Y - pos.Y, 2));

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
}