using System.Collections;
using static System.Math;

namespace Featherline;

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

    public float Dist(IntVec2 other)
    {
        var xDiff = X - other.X;
        var yDiff = Y - other.Y;
        return (float)Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

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
    public Vector2 Normalized()
    {
        float lengthInverted = 1f / (float)Sqrt(X * X + Y * Y);
        return new Vector2(X * lengthInverted, Y * lengthInverted);
    }

    public override string ToString() => $"{{ X: {X}, Y: {Y} }}";

    public static Vector2 operator +(Vector2 v1, Vector2 v2) => new Vector2(v1.X + v2.X, v1.Y + v2.Y);
    public static Vector2 operator +(IntVec2 v1, Vector2 v2) => new Vector2(v1.X + v2.X, v1.Y + v2.Y);
    public static Vector2 operator -(Vector2 v1, Vector2 v2) => new Vector2(v1.X - v2.X, v1.Y - v2.Y);
    public static Vector2 operator *(Vector2 v1, float m) => new Vector2(v1.X * m, v1.Y * m);
    public static bool operator ==(Vector2 v1, Vector2 v2) => v1.X == v2.X && v1.Y == v2.Y;
    public static bool operator !=(Vector2 v1, Vector2 v2) => v1.X != v2.X || v1.Y != v2.Y;

    public static float Dot(Vector2 v1, Vector2 v2) => v1.X * v2.X + v1.Y * v2.Y;

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

public enum AlgPhase
{
    None = 0,
    FrameGenes = 1,
    TimingTesterLight = 2,
    TimingTesterHeavy = 3,
    AnglePerfector = 4
}
