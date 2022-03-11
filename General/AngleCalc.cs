using static Featherline.GAManager;

namespace Featherline;

static class AngleCalc
{
    public static float ToBounds(this float val) => (val + Revolution) % Revolution;
    public static double ToBounds(this double val) => (val + Revolution) % Revolution;
    public static AngleSet ToAngleSet(this IEnumerable<float> values) => new AngleSet(values.ToArray());

    private static Aim[] aimValues;

    public static void Initialize()
    {
        PreCalcAngles();
    }

    public static void Reset()
    {
        aimValues = null;
    }

    public static Aim GetAim(float degAngle)
    {
        var mult = degAngle * 1000f;
        int index = (int)Math.Round(mult);
        return aimValues[index];
    }

    private static void PreCalcAngles()
    {
        aimValues = new Aim[360_001];
        float angle = 0f;
        for (int i = 0; i < 360_000; i++) {
            aimValues[i] = new Aim(angle);
            angle = (float)Math.Round(angle + 0.001f, 3);
        }
        aimValues[360_000] = aimValues[0];
    }
}

public readonly struct Aim
{
    public readonly Vector2 Value;
    public readonly float Angle;

    public Aim(float angle)
    {
        double toRad = angle * (Math.PI / Revolution * 2);
        Value = new Vector2((float)Math.Sin(toRad), -(float)Math.Cos(toRad)).Normalize();
        Angle = Value.Angle();
    }

    public override string ToString() => $"{Angle}, {Value}";
}
