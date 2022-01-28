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
			IsDead = Level.HasHazards ? (Func<bool>)DeathCheck : (() => false);
		}

		public void Debug(float[] ind)
        {
			this.ind = ind;
			cleaningInputs = false;

			InitializeSim();

			while (si.f < ind.Length) {
				RunFrame(ind[si.f]);
				si.Print();
			}

			Console.ReadLine();
		}

		public void DebugCollision(int xMin, int yMin, int xMax, int yMax)
        {
			//int yOffset = 0;
			//int xOffset = 0;
			//si = new FState() { pos = new Vector2(100 + xOffset, 66 + yOffset) };
			//si.UpdateIntPos();
			//Console.WriteLine(Level.WindTriggers.Any(wt => wt.TouchingAsFeather(si.intPos)));

			for (float y = yMin; y < yMax; y += 1) {
				for (float x = xMin; x < xMax; x += 1) {
					si.pos = new Vector2(x, y);
					si.UpdateIntPos();
					Console.Write(CollisionChar());
				}
				Console.WriteLine();
			}

			Console.ReadLine();

			char CollisionChar()
            {

				return 'F';
            }
		}

		private FState si;
		private WindState wind;
		private float[] ind;

		private (float closestDist, int atFrame) fitEval = (9999999, 0);

		private bool cleaningInputs;
		private bool stop = false;

		public void SimulateIndivitual(float[] ind, bool cleaningInputs = false, int frameCount = 99999999, Savestate ss = null)
		{
			this.ind = ind;
			this.cleaningInputs = cleaningInputs;

			frameCount = Math.Min(ind.Length, frameCount);

			if (ss == null)
				InitializeSim();
			else
				LoadSavestate(ss);

			while (si.f < frameCount) {
				RunFrame(ind[si.f]);
				if (stop) break;
			}
		}

		public void Evaluate(out double fitness, out int fCount)
        {
			int cpExtras = si.checkpointsGotten * 10000;
			if (si.checkpointsGotten >= sett.Checkpoints.Length) {
				si.checkpointsGotten--;
				double nextCpDist = si.GetDistToNextCp();
				fitness = cpExtras - si.f * 8 - nextCpDist;
				fCount = si.f + 1;
			}
			else {
				double nextCpDist = si.GetDistToNextCp();
				fitness = cpExtras - fitEval.closestDist * 2d - fitEval.atFrame - nextCpDist - si.f;
				fCount = sett.Framecount;
			}
		}

		private void InitializeSim()
		{
			si = new FState();
			si.pos = new Vector2(sett.StartX, sett.StartY);
			si.UpdateIntPos();

			wind = new WindState();

			if (sett.DefineStartBoost) {
				si.aim = new Vector2(sett.BoostX, sett.BoostY);
				si.spd = si.aim;
			}
			else {
				si.aim = Vector2.FromTasAngle(ind[si.f], 1);
				si.spd = si.aim * 250;

				UpdatePosition();
				si.f++;
			}

			si.intPos.X = (int)Math.Round(si.pos.X);
			si.intPos.Y = (int)Math.Round(si.pos.Y);

			WindFrame();
		}

		private void RunFrame(float aim)
		{
			si.previousPos = si.pos;

			InputCleaning();

			si.aim = Vector2.FromTasAngle(aim, 1);
			UpdateAngle();
			si.f++;

			UpdatePosition();

			if (AnalyzeProgress() || IsDead()) {
				InputCleaning(true);
				stop = true;
				return;
			}

			WindFrame();
		}

		private bool AnalyzeProgress() // return value -> whether simulation is finished
		{
			float dist = si.GetDistToNextCp();
			fitEval = dist < fitEval.closestDist ? (dist, si.f) : fitEval;

			if (Level.Checkpoints[si.checkpointsGotten].TouchingAsFeather(si.intPos)) {
				si.checkpointsGotten++;
				fitEval.closestDist = 999999;
			}

			return si.checkpointsGotten >= sett.Checkpoints.Length;
		}

		int turningStart = 0;
		float angleBeforeTurn = 0;
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

		public class FState
		{
			public int f = 0;
			public float speedLerp = 0f;

			public Vector2 pos;
			public Vector2 previousPos;
			public Vector2 aim;
			public Vector2 spd;

			public int checkpointsGotten = 0;

			public IntVec2 intPos = new IntVec2();
			public void UpdateIntPos()
			{
				intPos.X = (int)Math.Round(pos.X);
				intPos.Y = (int)Math.Round(pos.Y);
			}

			public FState Copy()
			{
				return new FState() {
					f = f,
					speedLerp = speedLerp,
					pos = pos,
					aim = aim,
					spd = spd,
					checkpointsGotten = checkpointsGotten,
					intPos = new IntVec2(intPos.X, intPos.Y)
				};
			}

			public float GetDistToNextCp() =>
				checkpointsGotten == Level.Checkpoints.Length - 1
				? (float)Level.Checkpoints[Level.Checkpoints.Length - 1].GetFinalCPDistance(pos, previousPos)
				: (float)Math.Sqrt(
					Math.Pow(Level.Checkpoints[checkpointsGotten].center.X - pos.X, 2)
				  + Math.Pow(Level.Checkpoints[checkpointsGotten].center.Y - pos.Y, 2));

            public void Print()
            {
				Console.Write($"f{f}, pos {pos}, ");
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.Write($"spd {spd}, ");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write($"angle {spd.TASAngle}, ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write($"lerp {speedLerp}, ");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"aim {aim}");
				Console.ForegroundColor = ConsoleColor.White;
            }
        }

		public static Vector2 RotateTowards(Vector2 vec, float targetAngleRadians, float maxMoveRadians) =>
			Vector2.AngleToVector(AngleApproach(vec.Angle(), targetAngleRadians, maxMoveRadians), vec.Length());

		public static float AngleDiff(float radiansA, float radiansB)
		{
			float num;
			for (num = radiansB - radiansA; num > 3.1415927f; num -= 6.2831855f) { }
			while (num <= -3.1415927f)
				num += 6.2831855f;
			return num;
		}

		public static float DegreesDiff(float actual, float target)
		{
			float v1 = target - actual;
			float v2 = v1 > 0 ? v1 - 360 : 360f + v1;
			return Math.Abs(v1) < Math.Abs(v2) ? v1 : v2;
		}

		public static float AngleApproach(float val, float target, float maxMove)
		{
			float value = AngleDiff(val, target);
			if (Math.Abs(value) < maxMove)
				return target;
			return val + Clamp(value, -maxMove, maxMove);
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
			Vector2 vector = si.spd.NormalizeAndCopy();
			vector = RotateTowards(vector, si.aim.Angle(), 5.5850534f * DeltaTime); // 5.33334 degrees

			object a = "abc";
			object b = 10;

			// curving and speed acceleration
			float target;
			if (!sett.EnableSteepTurns || Vector2.Dot(vector, si.aim) >= .45f) // angle after rotating < acos(.45)
			{
				si.speedLerp = Approach(si.speedLerp, 1f, DeltaTime / 1f);
				target = Lerp(140f, 190f, si.speedLerp);
			}
			else { // don't go here
				si.speedLerp = 0f;
				target = 140f; // approach 140 speed
			}

			// update speed
			float num = si.spd.Length();
			num = Approach(num, target, 1000f * DeltaTime);
			si.spd = vector * num;
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
}