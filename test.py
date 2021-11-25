import math


class Vector2:
    self.zero = Vector2(0.0, 0.0)

    def __init__(self, x, y):
        self.X = x
        self.Y = y

    #  normal angle (1, 0) = 0, counterclockwise, radians
    def Angle(self):
        return Single(math.atan2(float(self.Y), float(self.X)))

    #  CelesteTas angle (0, -1) = 0, clockwise, degrees
    def TASAngle(self):
        angle = (self.Angle() + math.pi / 2)
        if (angle < 0): angle += (2 * math.pi)
        return Single(math.toDegrees(angle))

    def Length(self):
        return Single(math.sqrt(float((self.X * self.X + self.Y * self.Y))))

    #  Turns this Vector2 to a unit vector with the same direction.
    def Normalize(self):
        num = 1.0 / Single(math.sqrt(float((self.X * self.X + self.Y * self.Y))))
        self.X *= num
        self.Y *= num
        return self.this

    #  same as above but don't alter this vector
    def NormalizeAndCopy(self):
        num = 1.0 / Single(math.sqrt(float((self.X * self.X + self.Y * self.Y))))
        return Vector2(self.X * num, self.Y * num)

    def toString(self):
        return String.format("(%.3f, %.3f)", self.X, self.Y)

    def translate(self, v2):
        self.X += v2.X
        self.Y += v2.Y

    def plus(self, v2):
        return Vector2(self.X + v2.X, self.Y + v2.Y)

    def minus(self, v2):
        return Vector2(self.X - v2.X, self.Y - v2.Y)

    def multiply(self, m):
        return Vector2(self.X * m, self.Y * m)

    def equ(self, v2):
        return self.X == v2.X and self.Y == v2.Y

    def AngleToVector(self, angleRadians, length):
        return Vector2(Single(math.cos(float(angleRadians))) * length, Single(math.sin(float(angleRadians))) * length)

    def fromTASAngle(self, angleDegrees, length):
        angleRadians = math.toRadians(angleDegrees)
        return Vector2(Single(math.sin(angleRadians)) * length, Single((-1.0 * math.cos(angleRadians) * length)))

    def Dot(self, value1, value2):
        return value1.X * value2.X + value1.Y * value2.Y


class Sim:
    frameNum = 0
    starFlySpeedLerp = 0.0
    Speed = Vector2.Zero
    DeltaTime = 0.0166667
    sim = Sim()


def __init__(self):
    #  enter feather here
    self.sim.Position = Vector2(91, 276)
    self.sim.Speed = Vector2(0.0, -240.0)
    frame = 0
    while (frame < 27 + 6 + 8):
        if (frame < 27):
            self.sim.input = Vector2(-1, -1) elif (frame < 27 + 6):
            self.sim.input = Vector2.fromTASAngle(340, 1) else:
            self.sim.input = Vector2.fromTASAngle(210, 1)
        #  then hold 210 for 8f
        self.FeatherUpdate(self.sim)
        frame += 1
    return


def RotateTowards(self, vec, targetAngleRadians, maxMoveRadians):
    return Vector2.AngleToVector(self.AngleApproach(vec.Angle(), targetAngleRadians, maxMoveRadians), vec.Length())


def AngleDiff(self, radiansA, radiansB):
    num = radiansB - radiansA
    while (num > 3.1415927):
        num -= 6.2831855
    while (num <= -3.1415927):
        num += 6.2831855
    return num


def AngleApproach(self, val, target, maxMove):
    value = self.AngleDiff(val, target)
    if (abs(value) < maxMove):
        return target
    return val + self.Clamp(value, -maxMove, maxMove)


def Clamp(self, value, min, max):
    value = (max if (value > max) else value)
    value = (min if (value < min) else value)
    return value


def Approach(self, val, target, maxMove):
    if (maxMove == 0.0 or val.equ(target)):
        return val
    value = target.minus(val)
    if (value.Length() < maxMove):
        return target
    value.Normalize()
    return val.plus(value.multiply(maxMove))


def Approach(self, val, target, maxMove):
    if (val <= target):
        return min(val + maxMove, target)
    return max(val - maxMove, target)


# / Linearly interpolates between two values.
def Lerp(self, value1, value2, amount):
    return value1 + (value2 - value1) * amount


def FeatherUpdate(self, sim):
    if (sim.frameNum < 26):  # 26 frames of transforming (StarFlyUpdate)
        sim.Speed = self.Approach(sim.Speed, Vector2.Zero, 1000.0 * self.DeltaTime) elif (
                sim.frameNum == 26):  # then 1 frame of featherboost from StarFlyCoroutine() method
    sim.Speed = sim.input.multiply(250) else:
    # then normal feather movement from StarFlyUpdate()
    self.FeatherMovement(sim)


sim.frameNum += 1
sim.Position.translate(sim.Speed.multiply(self.DeltaTime))
print("%s\t%s\t%.4f\n" % (sim.Speed, sim.Position, sim.input.TASAngle()), end="", sep="")


#  refactored StarFlyUpdate()
def FeatherMovement(self, sim):
    value = sim.input
    if (value.equ(Vector2.Zero)):  # don't go neutral (lazy to implement the neutral input code)
        raise SystemExit
    if (sim.Speed.equ(Vector2.Zero)):  # why would this ever happen
        raise SystemExit
    vector = sim.Speed.NormalizeAndCopy()
    vector = self.RotateTowards(vector, value.Angle(), 5.5850534 * self.DeltaTime)
    #  5.33334 degrees
    #  curving and speed acceleration
    if (vector != Vector2.Zero and Vector2.Dot(vector, value) >= 0.45):  # angle after rotating < acos(.45)
        sim.starFlySpeedLerp = self.Approach(sim.starFlySpeedLerp, 1.0, self.DeltaTime / 1.0)
        target = self.Lerp(140.0, 190.0, sim.starFlySpeedLerp)
    else:
        #  don't go here
        sim.starFlySpeedLerp = 0.0
        target = 140.0
    #  update speed
    num = sim.Speed.Length()
    num = self.Approach(num, target, 1000.0 * self.DeltaTime)
    sim.Speed = vector.multiply(num)


if __name__ == "__main__":
    feather().main([])
