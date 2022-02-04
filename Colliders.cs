using System;
using static System.Math;

namespace Featherline
{
    public readonly struct Bounds
    {
        public readonly int L, U, R, D;

        public Bounds(int L, int U, int R, int D)
        {
            this.L = L;
            this.U = U;
            this.R = R;
            this.D = D;
        }

        public Bounds Expand(/*bool feahterHitbox, */bool collider)
        {
            int L = this.L - 2;
            int U = this.U + 4;
            int R = this.R + 3;
            int D = this.D + 9;

            /*if (!feahterHitbox) {
                if (collider) {
                    U -= 3; D++;
                }
                else {
                    L--; R++; U--; D += 2;
                }
            }
            else */if (collider) {
                L--; U--; R++; D++;
            }

            return new Bounds(L, U, R, D);
        }

        public Bounds Expand() => new Bounds(L, U, R + 1, D + 1);
    }
    public readonly struct FloatBounds
    {
        public readonly float L, U, R, D;

        public FloatBounds(Bounds b)
        {
            L = b.L - 0.5f;
            U = b.U - 0.5f;
            R = b.R - 0.5f;
            D = b.D - 0.5f;
        }
    }

    public class RectangleHitbox
    {
        public Bounds bounds;

        public bool TouchingAsFeather(IntVec2 pos) =>
            bounds.L <= pos.X && pos.X < bounds.R &&
            bounds.U <= pos.Y && pos.Y < bounds.D;

        public float GetActualDistance(IntVec2 pos)
        {
            int xDiff = (bounds.L <= pos.X && pos.X <= bounds.R) ? 0 : Min(Abs(pos.X - bounds.L), Abs(pos.X - bounds.R));
            int yDiff = (bounds.U <= pos.Y && pos.Y <= bounds.D) ? 0 : Min(Abs(pos.Y - bounds.U), Abs(pos.Y - bounds.D));
            return (float)Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        public RectangleHitbox(Bounds b) => bounds = b;
        public RectangleHitbox() { }
    }

    public class Checkpoint : RectangleHitbox
    {
        public Vector2 center;

        private FloatBounds fb;

        public Checkpoint(Bounds b) : base(b)
        {
            center = new Vector2((b.L + b.R) / 2f, (b.U + b.D) / 2f);
            fb = new FloatBounds(b);
        }
        public Checkpoint() : base() { }

        public double GetFinalCPDistance(Vector2 pos, Vector2 previousPos)
        {
            var rawL = fb.L - pos.X;
            var rawR = pos.X - fb.R;
            var rawU = fb.U - pos.Y;
            var rawD = pos.Y - fb.D;

            if (rawL < 0 & rawR < 0 & rawU < 0 & rawD < 0) {
                rawL = fb.L - previousPos.X;
                rawR = previousPos.X - fb.R;
                rawU = fb.U - previousPos.Y;
                rawD = previousPos.Y - fb.D;
            }

            double xDiff = rawL > 0 ? rawL : rawR > 0 ? rawR : 0;
            double yDiff = rawU > 0 ? rawU : rawD > 0 ? rawD : 0;
            return Sqrt(xDiff * xDiff + yDiff * yDiff);
        }
    }

    public class Spike : RectangleHitbox
    {
        public Func<Vector2, bool> moveDirAllowsDeath;

        public Spike(Bounds b, string dir) : base(b)
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

    public class JumpThru : RectangleHitbox
    {
        public Func<Vector2, Vector2, bool> Booped;
    }
}