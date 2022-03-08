using System.Collections;
using static Featherline.GAManager;

namespace Featherline;

public class AngleSet : IEnumerable<float>
{
    protected float[] values;

    public float this[int index]
    {
        get => values[index];
        set => values[index] = FixAngle(value);
    }

    public void SetRange(float value, int from, int to)
    {
        var val = FixAngle(value);
        for (int i = from; i < to; i++)
            values[i] = val;
    }

    public AngleSet this[Range range] => new AngleSet(values[range]);

    public int Length { get => values.Length; }

    public AngleSet(int length) => values = new float[length];

    public AngleSet(IEnumerable<float> angles) => values = angles.ToArray();

    public AngleSet Randomize()
    {
        var rand = new Random();
        for (int i = 0; i < values.Length; i++)
            values[i] = FixAngle((float)rand.NextDouble() * Revolution);
        return this;
    }

    public void CopyTo(AngleSet target)
    {
        for (int i = 0; i < values.Length; i++)
            target.values[i] = values[i];
    }

    public AngleSet Clone()
    {
        var clone = new AngleSet(Length);
        CopyTo(clone);
        return clone;
    }

    public void Resize(int newLen) => Array.Resize(ref values, newLen);

    public AngleSet Concat(AngleSet o) => new AngleSet(values.Concat(o.values).ToArray());

    private float FixAngle(float a) => (float)Math.Round(Math.Round(a, 3).ToBounds(), 3);

    IEnumerator<float> IEnumerable<float>.GetEnumerator() => values.AsEnumerable().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
}
