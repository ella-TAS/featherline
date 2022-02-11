using System;
using System.Collections;
using static System.Math;

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

    public enum Facings
    {
        Left = 0,
        Up = 1,
        Right = 2,
        Down = 3
    }
}