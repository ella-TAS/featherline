import functools
import math
from typing import Union


def sim(pos: (float, float), inputs: list[float], precise: bool, squareboost: bool) -> (
        float, float, float, float, bool):
    s = Sim()
    x, y = pos
    s.position = Vector2(x, y)
    for i in inputs:
        s.input_ = vector_from_tas_angle(i, 1)


def main():
    # enter feather here
    s = Sim()
    s.position = Vector2(91, 276)

    for frame in range(27 + 6 + 8):
        if frame < 27:
            s.input_ = Vector2(-1, -1)  # featherboost upleft
        elif frame < 27 + 6:
            s.input_ = vector_from_tas_angle(340, 1)  # hold 340 for 6f
        else:
            s.input_ = vector_from_tas_angle(210, 1)  # then hold 210 for 8f

        feather_update(s)


class Vector2:
    def __init__(self, x: float, y: float):
        self.x = x
        self.y = y

    def __str__(self):
        return f"({round(self.x, 3)}, {round(self.y, 3)})"

    def __add__(self, other):
        return Vector2(self.x + other.x, self.y + other.y)

    def __sub__(self, other):
        return Vector2(self.x - other.x, self.y - other.y)

    def __eq__(self, other):
        return self.x == other.x and self.y == other.y

    def __mul__(self, other: float):
        return Vector2(self.x * other, self.y * other)

    @staticmethod
    @functools.cache
    def zero():
        return Vector2(0, 0)

    # normal angle (1, 0) = 0, counterclockwise, radians
    def angle(self) -> float:
        return math.atan2(self.y, self.x)

    # CelesteTas angle (0, -1) = 0, clockwise, degrees
    def tas_angle(self) -> float:
        angle = self.angle() + math.pi / 2
        angle += math.tau if angle < 0 else 0

        return math.degrees(angle)

    def length(self) -> float:
        return math.sqrt(self.x * self.x + self.y * self.y)

    # turns this Vector2 to a unit vector with the same direction
    def normalize(self):
        num = 1 / self.length()
        self.x *= num
        self.y *= num

    # same as above but don't alter this vector
    def normalize_and_copy(self):
        num = 1 / self.length()
        return Vector2(self.x * num, self.y * num)

    def translate(self, v2):
        self.x += v2.x
        self.y += v2.y


class Sim:
    def __init__(self):
        self.position: Union[Vector2, None] = None
        self.input_: Union[Vector2, None] = None
        self.speed: Vector2 = Vector2.zero()
        self.frame_num: int = 0
        self.star_fly_speed_lerp: float = 0


# linearly interpolates between two values
def lerp(value1: float, value2: float, amount: float) -> float:
    return value1 + (value2 - value1) * amount


def approach(val: float, target: float, max_move: float) -> float:
    if val <= target:
        return min(val + max_move, target)
    else:
        return max(val - max_move, target)


def approach_vector(val: Vector2, target: Vector2, max_move: float) -> Vector2:
    if max_move == 0 or val == target:
        return val

    value: Vector2 = target - val

    if value.length() < max_move:
        return target

    value.normalize()
    return val + value * max_move


def clamp(value: float, minimum: float, maximum: float) -> float:
    value = maximum if value > maximum else value
    value = minimum if value < minimum else value
    return value


def angle_approach(val: float, target: float, max_move: float) -> float:
    value = angle_diff(val, target)

    if abs(value) < max_move:
        return target

    return val + clamp(value, -max_move, max_move)


def angle_diff(radians_a: float, radians_b: float) -> float:
    num = radians_b - radians_a

    while num > math.pi:
        num -= math.tau

    while num <= -math.pi:
        num += math.tau

    return num


def rotate_towards(vec: Vector2, target_angle_radians: float, max_move_radians: float) -> Vector2:
    return angle_to_vector(angle_approach(vec.angle(), target_angle_radians, max_move_radians), vec.length())


def angle_to_vector(angle_radians: float, length: float) -> Vector2:
    return Vector2(math.cos(angle_radians) * length, math.sin(angle_radians) * length)


def vector_from_tas_angle(angle_degrees: float, length: float) -> Vector2:
    angle_radians = math.radians(angle_degrees)
    return Vector2(math.sin(angle_radians) * length, -1 * math.cos(angle_radians) * length)


def dot(value1: Vector2, value2: Vector2) -> float:
    return value1.x * value2.x + value1.x * value2.y


def feather_update(sim: Sim):
    if sim.frame_num == 0:
        # then 1 frame of featherboost from StarFlyCoroutine() method
        sim.speed = sim.input_ * 250
    else:
        # then normal feather movement from StarFlyUpdate()
        feather_movement(sim)

    sim.frame_num += 1
    sim.position.translate(sim.speed * DELTA_TIME)
    print(f"{sim.speed}\t{sim.position}\t{round(sim.input_.tas_angle(), 4)}\n")


# refactored StarFlyUpdate()
def feather_movement(sim: Sim):
    value = sim.input_

    if value == Vector2.zero():  # don't go neutral (lazy to implement the neutral input code)
        raise SystemError
    if sim.speed == Vector2.zero():  # why would this ever happen
        raise SystemError

    vector = sim.speed.normalize_and_copy()
    vector = rotate_towards(vector, value.angle(), 5.5850534 * DELTA_TIME)  # 5.33334 degrees

    # curving and speed acceleration
    if vector != Vector2.zero() and dot(vector, value) >= 0.45:  # angle after rotating < acos(.45)
        sim.star_fly_speed_lerp = approach(sim.star_fly_speed_lerp, 1, DELTA_TIME)
        target = lerp(140, 190, sim.star_fly_speed_lerp)
    else:  # don't go here
        sim.star_fly_speed_lerp = 0
        target = 140  # approach 140 speed

    # update speed
    num = sim.speed.length()
    num = approach(num, target, 1000 * DELTA_TIME)
    sim.speed = vector * num


DELTA_TIME = 0.0166667

if __name__ == '__main__':
    main()
