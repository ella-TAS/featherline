using System;
using System.Collections;
using static System.Math;
using System.Collections.Generic;

namespace Featherline
{
    public class DeathMapInfo
    {
        public BitArray[] map;
        public int xMin;
        public int xMax;
        public int yMin;
        public int yMax;
        public int widthInTiles;
        public int heightInTiles;
    }

    public class IntVec2
    {
        public int X;
        public int Y;

        public IntVec2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IntVec2(Vector2 src)
        {
            X = (int)Round(src.X);
            Y = (int)Round(src.Y);
        }

        public float Dist(IntVec2 other) => (float)Sqrt(Pow(X-other.X, 2) + Pow(Y-other.Y, 2));

        public IntVec2() { }

        public override string ToString() => $"{{ X: {X}, Y: {Y} }}";
    }

    public struct Vector2
    {
        public float X, Y;

        public static readonly Vector2 Zero = new Vector2(0f, 0f);

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        // normal angle (1, 0) = 0, counterclockwise, radians
        public float Angle() => (float)Atan2(Y, X);

        // CelesteTas angle (0, -1) = 0, clockwise, degrees
        public float TASAngle {
            get {
                double angle = Angle() + PI / 2;
                if (angle < 0)
                    angle += 2 * PI;
                return (float)(angle * Rad2Deg);
            }
        }

        public float Length() => (float)Sqrt(X * X + Y * Y);

        // Turns this Vector2 to a unit vector with the same direction.
        public Vector2 Normalize()
        {
            float num = 1f / (float)Sqrt(X * X + Y * Y);
            X *= num;
            Y *= num;
            return this;
        }

        // same as above but don't alter this vector
        public Vector2 NormalizeAndCopy()
        {
            float num = 1f / (float)Sqrt(X * X + Y * Y);
            return new Vector2(X * num, Y * num);
        }

        public override string ToString() => $"{{ X: {X}, Y: {Y} }}";

        public void translate(Vector2 v2)
        {
            X += v2.X;
            Y += v2.Y;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2) => new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        public static Vector2 operator -(Vector2 v1, Vector2 v2) => new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        public static Vector2 operator *(Vector2 v1, float m) => new Vector2(v1.X * m, v1.Y * m);
        public static bool operator ==(Vector2 v1, Vector2 v2) => v1.X == v2.X && v1.Y == v2.Y;
        public static bool operator !=(Vector2 v1, Vector2 v2) => v1.X != v2.X || v1.Y != v2.Y;

        public static Vector2 AngleToVector(float angleRadians, float length) =>
            new Vector2((float)Cos(angleRadians) * length,
                (float)Sin(angleRadians) * length);

        public static float Dot(Vector2 v1, Vector2 v2) => v1.X * v2.X + v1.Y * v2.Y;

        public static Vector2 FromTasAngle(float degrees, float magnitude)
        {
            float toRad = degrees / (float)Rad2Deg;
            return new Vector2((float)Sin(toRad) * magnitude,
                        (float)(-1d * Cos(toRad) * magnitude));
        }

        const double Rad2Deg = 180d / PI;
    }

    public class RectangleHitbox
    {
        public int L;
        public int R;
        public int U;
        public int D;

        public bool TouchingAsFeather(IntVec2 pos) =>
            L <= pos.X && pos.X <= R &&
            U <= pos.Y && pos.Y <= D;

        public float GetActualDistance(IntVec2 pos)
        {
            int xDiff = (L <= pos.X && pos.X <= R) ? 0 : Min(Abs(pos.X - L), Abs(pos.X - R));
            int yDiff = (U <= pos.Y && pos.Y <= D) ? 0 : Min(Abs(pos.Y - U), Abs(pos.Y - D));
            return (float)Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        public RectangleHitbox(int L, int U, int R, int D)
        {
            this.L = L - 2;
            this.U = U + 4;
            this.R = R + 3;
            this.D = D + 9;
        }

        public RectangleHitbox() { }
    }

    public class Checkpoint : RectangleHitbox
    {
        public Vector2 center;

        public float Lf;
        public float Rf;
        public float Uf;
        public float Df;

        public Checkpoint(int L, int U, int R, int D) : base(L, U, R - 1, D - 1)
        {
            center = new Vector2((L + R) / 2f, (U + D) / 2f);
            Lf = this.L - 0.5f;
            Rf = this.R + 0.5f;
            Uf = this.U - 0.5f;
            Df = this.D + 0.5f;
        }
        public Checkpoint() : base() { }

        public double GetFinalCPDistance(Vector2 pos, Vector2 previousPos)
        {
            var rawL = Lf - pos.X;
            var rawR = pos.X - Rf;
            var rawU = Uf - pos.Y;
            var rawD = pos.Y - Df;

            if (rawL<0&rawR<0&rawU<0&rawD<0) {
                rawL = Lf - previousPos.X;
                rawR = previousPos.X - Rf;
                rawU = Uf - previousPos.Y;
                rawD = previousPos.Y - Df;
            }

            double xDiff = rawL > 0 ? rawL : rawR > 0 ? rawR : 0;
            double yDiff = rawU > 0 ? rawU : rawD > 0 ? rawD : 0;
            return Sqrt(xDiff * xDiff + yDiff * yDiff);
        }
    }

    public class Spike : RectangleHitbox
    {
        public Func<Vector2, bool> moveDirAllowsDeath;

        public Spike(int L, int U, int R, int D, string dir) : base(L, U, R, D)
        {
            moveDirAllowsDeath =
            dir switch {
                "Left" => spd => spd.X >= 0f,
                "Right" => spd => spd.X <= 0f,
                "Up" => spd => spd.Y >= 0f,
                "Down" => spd => spd.Y <= 0,
                _ => spd => false
            };
        }

        public bool Died(IntVec2 pos, Vector2 spd)
            => TouchingAsFeather(pos) && moveDirAllowsDeath(spd);
    }

    public class Collider : RectangleHitbox
    {
        public Collider(int L, int U, int R, int D)
            : base(L-1, U-1, R, D) { }
    }

    public class SolidTileInfo
    {
        public BitArray[] map;
        public int x;
        public int y;
        public int width;
        public int height;
        public int rightBound;
        public int lowestYIndex;
    }
}