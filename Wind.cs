namespace Featherline;

partial class FeatherSim
{
    public static bool doWind;

    public void WindFrame()
    {
        if (doWind) {
            // check wind triggers and update target wind speed
            foreach (var WT in Level.WindTriggers) {
                if (fs.pos.X >= WT.bounds.L &&
                    fs.pos.X < WT.bounds.R &&
                    fs.pos.Y >= WT.bounds.U &&
                    fs.pos.Y < WT.bounds.D) {

                    if (WT.vertWind) {
                        wind.target.Y = WT.strength;
                        wind.changeStates.Y = WT.strength > wind.current.Y ? ChangeState.Rising : ChangeState.Descending;
                    }
                    else {
                        wind.target.X = WT.strength;
                        wind.changeStates.X = WT.strength > wind.current.X ? ChangeState.Rising : ChangeState.Descending;
                    }

                    break;
                }
            }

            wind.UpdateWindSpeed();

            fs.moveCounter += wind.current * DeltaTime;
            fs.MoveH();
            fs.MoveV();
        }
    }
}

public class WindTrigger : RectangleHitbox
{
    public bool vertWind;
    public float strength;

    public WindTrigger(IntVec2 pos, IntVec2 size, bool vertical, float stren)
        : base(new Bounds(pos.X, pos.Y, pos.X + size.X, pos.Y + size.Y).Expand(true))
    {
        vertWind = vertical;
        strength = stren;
    }
}

public class WindState
{
    public Vector2 current = Level.InitWind;
    public Vector2 target;

    public (ChangeState X, ChangeState Y) changeStates = (ChangeState.None, ChangeState.None);

    const float changeRate = 1.66666666f;

    public void UpdateWindSpeed()
    {
        switch (changeStates.X) {
            case ChangeState.Rising:
                current.X += changeRate;
                if (current.X >= target.X) {
                    current.X = target.X;
                    changeStates.X = ChangeState.None;
                }
                break;
            case ChangeState.Descending:
                current.X -= changeRate;
                if (current.X <= target.X) {
                    current.X = target.X;
                    changeStates.X = ChangeState.None;
                }
                break;
        }
        switch (changeStates.Y) {
            case ChangeState.Rising:
                current.Y += changeRate;
                if (current.Y >= target.Y) {
                    current.Y = target.Y;
                    changeStates.Y = ChangeState.None;
                }
                break;
            case ChangeState.Descending:
                current.Y -= changeRate;
                if (current.Y <= target.Y) {
                    current.Y = target.Y;
                    changeStates.Y = ChangeState.None;
                }
                break;
        }
    }

    public WindState Copy()
    {
        var copied = new WindState();
        copied.current = current;
        copied.target = target;
        copied.changeStates = (changeStates.X, changeStates.Y);
        return copied;
    }
}

public enum ChangeState
{
    None = 0,
    Rising = 1,
    Descending = 2
}
