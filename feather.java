public class feather {
	public static void main(String[] args) {
		new feather();
	}

	static class Vector2 {
		float X, Y;

		static final Vector2 Zero = new Vector2(0f, 0f);

		public Vector2(float x, float y) {
			this.X = x;
			this.Y = y;
		}

		// normal angle (1, 0) = 0, counterclockwise, radians
		private float Angle() {
			return (float) Math.atan2((double) this.Y, (double) this.X);
		}

		// CelesteTas angle (0, -1) = 0, clockwise, degrees
		public float TASAngle() {
			double angle = (this.Angle() + Math.PI / 2);
			if (angle < 0)
				angle += (2 * Math.PI);
			return (float) Math.toDegrees(angle);
		}

		public float Length() {
			return (float) Math.sqrt((double) (this.X * this.X + this.Y * this.Y));
		}

		// Turns this Vector2 to a unit vector with the same direction.
		public Vector2 Normalize() {
			float num = 1f / (float) Math.sqrt((double) (this.X * this.X + this.Y * this.Y));
			this.X *= num;
			this.Y *= num;
			return this;
		}

		// same as above but don't alter this vector
		public Vector2 NormalizeAndCopy() {
			float num = 1f / (float) Math.sqrt((double) (this.X * this.X + this.Y * this.Y));
			return new Vector2(this.X * num, this.Y * num);
		}

		@Override
		public String toString() {
			return String.format("(%.3f, %.3f)", X, Y);
		}

		public void translate(Vector2 v2) {
			this.X += v2.X;
			this.Y += v2.Y;
		}

		public Vector2 plus(Vector2 v2) {
			return new Vector2(this.X + v2.X, this.Y + v2.Y);
		}

		public Vector2 minus(Vector2 v2) {
			return new Vector2(this.X - v2.X, this.Y - v2.Y);
		}

		public Vector2 multiply(float m) {
			return new Vector2(this.X * m, this.Y * m);
		}

		public boolean equ(Vector2 v2) {
			return this.X == v2.X && this.Y == v2.Y;
		}

		public static Vector2 AngleToVector(float angleRadians, float length) {
			return new Vector2((float) Math.cos((double) angleRadians) * length,
					(float) Math.sin((double) angleRadians) * length);
		}

		public static Vector2 fromTASAngle(float angleDegrees, float length) {
			double angleRadians = Math.toRadians(angleDegrees);
			return new Vector2((float) Math.sin(angleRadians) * length,
					(float) (-1d * Math.cos(angleRadians) * length));
		}

		public static float Dot(Vector2 value1, Vector2 value2) {
			return value1.X * value2.X + value1.Y * value2.Y;
		}
	}

	static class Sim {
		int frameNum = 0;
		float starFlySpeedLerp = 0f;

		Vector2 Position;
		Vector2 Speed = Vector2.Zero;

		Vector2 input;
	}

	static final float DeltaTime = 0.0166667f;

	static Sim sim = new Sim();

	feather() {
		// enter feather here
		sim.Position = new Vector2(28565.1975392997f, -8006.802460700270f);
		sim.Speed = new Vector2(0f, 0f);

		for (int frame = 0; frame < 21; frame++) {
			if (frame == 0)
				sim.input = new Vector2(-1, -1); // featherboost upleft
			else
				sim.input = Vector2.fromTASAngle(220, 1); // hold 340 for 6f
			FeatherUpdate(sim);
			// debugging
			if (frame == 13)
				System.out.println("hi");
		}

		return;
	}

	public static Vector2 RotateTowards(Vector2 vec, float targetAngleRadians, float maxMoveRadians) {
		return Vector2.AngleToVector(AngleApproach(vec.Angle(), targetAngleRadians, maxMoveRadians), vec.Length());
	}

	public static float AngleDiff(float radiansA, float radiansB) {
		float num;
		for (num = radiansB - radiansA; num > 3.1415927f; num -= 6.2831855f) {
		}
		while (num <= -3.1415927f) {
			num += 6.2831855f;
		}
		return num;
	}

	public static float AngleApproach(float val, float target, float maxMove) {
		float value = AngleDiff(val, target);
		if (Math.abs(value) < maxMove) {
			return target;
		}
		return val + Clamp(value, -maxMove, maxMove);
	}

	public static float Clamp(float value, float min, float max) {
		value = ((value > max) ? max : value);
		value = ((value < min) ? min : value);
		return value;
	}

	static Vector2 Approach(Vector2 val, Vector2 target, float maxMove) {
		if (maxMove == 0f || val.equ(target)) {
			return val;
		}
		Vector2 value = target.minus(val);
		if (value.Length() < maxMove) {
			return target;
		}
		value.Normalize();
		return val.plus(value.multiply(maxMove));
	}

	public static float Approach(float val, float target, float maxMove) {
		if (val <= target) {
			return Math.min(val + maxMove, target);
		}
		return Math.max(val - maxMove, target);
	}

	/// Linearly interpolates between two values.
	public static float Lerp(float value1, float value2, float amount) {
		return value1 + (value2 - value1) * amount;
	}

	void FeatherUpdate(Sim sim) {
		if (sim.frameNum == 0)
			// then 1 frame of featherboost from StarFlyCoroutine() method
			sim.Speed = sim.input.multiply(250);
		else {
			// then normal feather movement from StarFlyUpdate()
			FeatherMovement(sim);
		}
		sim.frameNum++;
		sim.Position.translate(sim.Speed.multiply(DeltaTime));
		System.out.printf("%s\t%s\t%.4f\n", sim.Speed, sim.Position, sim.input.TASAngle());
	}

	// refactored StarFlyUpdate()
	private void FeatherMovement(Sim sim) {
		Vector2 value = sim.input;

		if (value.equ(Vector2.Zero)) // don't go neutral (lazy to implement the neutral input code)
			throw new RuntimeException();
		if (sim.Speed.equ(Vector2.Zero)) // why would this ever happen
			throw new RuntimeException();

		Vector2 vector = sim.Speed.NormalizeAndCopy();
		vector = RotateTowards(vector, value.Angle(), 5.5850534f * DeltaTime); // 5.33334 degrees

		// curving and speed acceleration
		float target;
		if (vector != Vector2.Zero && Vector2.Dot(vector, value) >= 0.45f) // angle after rotating < acos(.45)
		{
			sim.starFlySpeedLerp = Approach(sim.starFlySpeedLerp, 1f, DeltaTime / 1f);
			target = Lerp(140f, 190f, sim.starFlySpeedLerp);
		} else { // don't go here
			sim.starFlySpeedLerp = 0f;
			target = 140f; // approach 140 speed
		}

		// update speed
		float num = sim.Speed.Length();
		num = Approach(num, target, 1000f * DeltaTime);
		sim.Speed = vector.multiply(num);

		// end of feather
//		if (this.starFlyTimer <= 0f) {
//			if (Input.MoveY.Value == -1) {
//				this.Speed.Y = -100f;
//			}
//			if (Input.MoveY.Value < 1) {
//				this.varJumpSpeed = this.Speed.Y;
//				this.AutoJump = true;
//				this.AutoJumpTimer = 0f;
//				this.varJumpTimer = 0.2f;
//			}
//			if (this.Speed.Y > 0f) {
//				this.Speed.Y = 0f;
//			}
//			if (Math.Abs(this.Speed.X) > 140f) {
//				this.Speed.X = 140f * (float) Math.Sign(this.Speed.X);
//			}
//			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
//			return 0;
//		}
	}

}
