using System;
using System.Linq;
using System.Diagnostics;

namespace Featherline
{
	public partial class FeatherSim
	{
		const float DeltaTime = 0.0166667f;

		public Settings sett;

		public FeatherSim(Settings s)
		{
			sett = s;
			IsDead = Level.HasHazards ? (Func<bool>)DeathCheck : (() => false);
			// debugging
			/*if (true)
			{
				while (true) {
					int yOffset = 0;
					int xOffset = 0;
					si = new FState() { position = new Vector2(100 + xOffset, 66 + yOffset) };

					si.UpdateIntPos();
					Console.WriteLine(Level.windTriggers.Any(wt => wt.TouchingAsFeather(si.posAsInts)));
					
                    /*float xMin = -1543;
					float xMax = -1400;

					float yMin = -1548;
					float yMax = -1480;

					for (float y = yMin; y < yMax; y += 1) {
						for (float x = xMin; x < xMax; x += 1) {
							si.position = new Vector2(x, y);
							si.posAsInts = new IntVec2((int)Math.Round(si.position.X), (int)Math.Round(si.position.Y));
						}
						Console.WriteLine();
					}*

                    Console.ReadLine();
				}
			}*/
		}

		private FState si;
		private WindState wind;

		private (
			IntVec2[]         spinners,
			RectangleHitbox[] killboxes,
			Spike[]           spikes,
			Collider[]        colls
		) distFiltered;

		private int framesSinceDistFilter = 999;

		private (float closestDist, int atFrame) fitEval = (9999999, 0);

		private float fitnessOffset = 0;
		// return value is fitness
		public (float fitness, int frames) SimulateIndivitual(float[] ind, bool cleaningInputs = false, int frameCount = 99999999)
		{
			frameCount = Math.Min(ind.Length, frameCount);

			si = new FState();
			si.position = new Vector2(sett.StartX, sett.StartY);
			si.UpdateIntPos();

			//previousDist = GetDistToNextCp();

			wind = new WindState();

			// input cleanup stuff
			int turningStart = 0;
			float angleBeforeTurn = 0;

			//bool dead = false;

			int i = 0;
			if (sett.DefineStartBoost) {
				si.aim = new Vector2(sett.BoostX, sett.BoostY);
				si.speed = si.aim;
			}
			else {
				si.aim = Vector2.FromTasAngle(ind[i], 1);
				si.speed = si.aim * 250;
				
				UpdatePosition();
				i++;
			}
			si.intPos.X = (int)Math.Round(si.position.X);
			si.intPos.Y = (int)Math.Round(si.position.Y);

			WindFrame();


			for (; i < frameCount; i++) {
				InputCleaning();

				si.aim = Vector2.FromTasAngle(ind[i], 1);
				FeatherUpdate();

				UpdatePosition();


				//flight debugging
				//Console.WriteLine($"f: {si.frameNum}   Spd: {si.speed}   Pos: {si.position}   Angle: {si.speed.TASAngle}");

				if (AnalyzeProgress() || IsDead()) {
					InputCleaning(true);
					break;
				}

				WindFrame();
			}

			float cpExtras = si.CheckpointsGotten * 10000;
			if (si.CheckpointsGotten >= sett.Checkpoints.Length) {
				si.CheckpointsGotten--;
				return (cpExtras - i * 8 - GetDistToNextCp() + fitnessOffset, i + 1);
			}
			else
				return (cpExtras - fitEval.closestDist * 2 - fitEval.atFrame + fitnessOffset - GetDistToNextCp() - si.frameNum, sett.Framecount);

			void InputCleaning(bool forceClean = false)
            {
				if (cleaningInputs) {
					ind[i] += ind[i] < 0f ? 360f : 0f;
					ind[i] -= ind[i] >= 360f ? 360f : 0f;

					// hard disable slow steep turns
					float actualAngle = si.speed.TASAngle;
					float turnForce = DegreesDiff(actualAngle, ind[i]);

					TurnState current = turnForce > 5.333 ? TurnState.Clockwise : turnForce < -5.333 ? TurnState.AntiClockwise : TurnState.None;
					if (current != turning || forceClean) {
						if (turning == TurnState.None) {
							// begin a turn
							turningStart = i;
							angleBeforeTurn = actualAngle;
						}
						else {
							// end the turn
							int j = turningStart;
							float newAngle = (float)Math.Round(angleBeforeTurn);
							while (true) {
								if (i - j <= 11) {
									newAngle = turning == TurnState.Clockwise ? (float)Math.Ceiling(actualAngle + 1) : (float)Math.Floor(actualAngle - 1);
									for (; j < i; j++)
										ind[j] = newAngle;
									break;
								}

								newAngle += 60 * (int)turning;
								newAngle = newAngle < 0f ? newAngle + 360f : newAngle;
								newAngle = newAngle >= 360f ? newAngle - 360f : newAngle;
								
								int lineTarget = j + 11;
								for (; j < lineTarget; j++)
									ind[j] = newAngle;
							}

							if (current != TurnState.None) {
								// begin a turn
								angleBeforeTurn = actualAngle;
								turningStart = i;
							}
						}
						turning = current;
					}
				}
			}
		}

		private bool AnalyzeProgress() // return value -> whether simulation is finished
        {
			float dist = GetDistToNextCp();
			fitEval = dist < fitEval.closestDist ? (dist, si.frameNum) : fitEval;

			if (sett.Checkpoints[si.CheckpointsGotten].TouchingAsFeather(si.intPos)) {
				si.CheckpointsGotten++;
				fitEval.closestDist = 999999;
			}

			return si.CheckpointsGotten >= sett.Checkpoints.Length;
        }

		private float GetDistToNextCp() => (float)Math.Sqrt(Math.Pow(sett.Checkpoints[si.CheckpointsGotten].center.X - si.position.X, 2) + Math.Pow(sett.Checkpoints[si.CheckpointsGotten].center.Y - si.position.Y, 2));

		public class FState
		{
			public int frameNum = 0;
			public float starFlySpeedLerp = 0f;

			public Vector2 position;
			public Vector2 aim;
			public Vector2 speed;

			public int CheckpointsGotten = 0;

			public IntVec2 intPos = new IntVec2();
			public void UpdateIntPos()
			{
				intPos.X = (int)Math.Round(position.X);
				intPos.Y = (int)Math.Round(position.Y);
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

		public static float DegreesDiff(float a1, float a2)
		{
			float v1 = a2 - a1;
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

		void FeatherUpdate()
		{
			FeatherMovement();
			si.frameNum++;
		}

		// refactored StarFlyUpdate()
		private void FeatherMovement()
		{
			Vector2 vector = si.speed.NormalizeAndCopy();
			vector = RotateTowards(vector, si.aim.Angle(), 5.5850534f * DeltaTime); // 5.33334 degrees

			// curving and speed acceleration
			float target;
			if (!sett.EnableSteepTurns || Vector2.Dot(vector, si.aim) >= .45f) // angle after rotating < acos(.45)
			{
				si.starFlySpeedLerp = Approach(si.starFlySpeedLerp, 1f, DeltaTime / 1f);
				 target = Lerp(140f, 190f, si.starFlySpeedLerp);
			}
			else { // don't go here
				si.starFlySpeedLerp = 0f;
				target = 140f; // approach 140 speed
			}

			// update speed
			float num = si.speed.Length();
			num = Approach(num, target, 1000f * DeltaTime);
			si.speed = vector * num;
		}



		// for input cleanup
		private enum TurnState
		{
			None = 0,
			Clockwise = 1,
			AntiClockwise = -1
		}
		//private Vector2? wallboop = null;
		TurnState turning = TurnState.None;
		private bool wallbooped = false;
	}
}