# cython: language_level=3

import functools
import math
from typing import List, Optional, Tuple


def sim(posx: float, posy: float, inputs: List[float], spinners: List[Tuple[int, int]], killbox: List[Tuple[int, int, int, int]], boostx: float, boosty: float) \
        -> (float, float, float, float, bool):
    s = Sim()
    s.position = Vector2(posx, posy)

    if boostx != 0 or boosty != 0:
        s.aim = Vector2(boostx, boosty)
        feather_update(s)

        if collision(s, spinners, killbox):
            return s.position.x, s.position.y, s.speed.x, s.speed.y, True

        # print(s)

    for i in inputs:
        s.aim = vector_from_tas_angle(i, 1)
        feather_update(s)

        if collision(s, spinners, killbox):
            return s.position.x, s.position.y, s.speed.x, s.speed.y, True

        # print(s)

    return s.position.x, s.position.y, s.speed.x, s.speed.y, False


def main():
    print(sim(7165.795268267390, 4475.500048935410, [109.864, 180.769, 196.475, 185.295, 319.761, 281.535, 194.868, 225.642, 203.646, 247.309, 251.15, 160.534, 20.195, 95.19, 126.693, 87.671, 198.911, 268.617, 348.728, 285.396, 67.764, 329.259, 230.667, 163.095, 166.592, 313.803, 255.698, 79.247, 20.586, 270.308, 88.257, 200.551, 153.003, 289.382, 85.426, 40.363, 183.414, 173.476, 45.497, 347.559, 137.441, 220.284, 328.099, 130.849, 109.36, 229.979, 335.439, 160.798], [[7304, 4448]], [], 1, -1))


class Vector2:
    def __init__(self, x: float, y: float):
        self.x = x
        self.y = y

    def __str__(self) -> str:
        return "({:.3f}, {:.3f})".format(self.x, self.y)

    def __eq__(self, other) -> bool:
        return self.x == other.x and self.y == other.y

    def __add__(self, other):
        return Vector2(self.x + other.x, self.y + other.y)

    def __sub__(self, other):
        return Vector2(self.x - other.x, self.y - other.y)

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
        self.position: Optional[Vector2] = None
        self.aim: Optional[Vector2] = None
        self.speed: Vector2 = Vector2.zero()
        self.frame_num: int = 0
        self.star_fly_speed_lerp: float = 0

    def __str__(self) -> str:
        return "{:<25}{:<25}{:.4f}".format(str(self.speed), str(self.position), self.aim.tas_angle())


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
    return value1.x * value2.x + value1.y * value2.y


def feather_update(si: Sim):
    if si.frame_num == 0:
        # then 1 frame of featherboost from StarFlyCoroutine() method
        si.speed = si.aim * 250
    else:
        # then normal feather movement from StarFlyUpdate()
        feather_movement(si)

    si.frame_num += 1
    si.position.translate(si.speed * DELTA_TIME)


# refactored StarFlyUpdate()
def feather_movement(si: Sim):
    if si.aim == Vector2.zero():  # don't go neutral (lazy to implement the neutral input code)
        raise SystemError
    if si.speed == Vector2.zero():  # why would this ever happen
        raise SystemError

    current_dir = si.speed.normalize_and_copy()
    current_dir = rotate_towards(current_dir, si.aim.angle(), 5.5850534 * DELTA_TIME)  # 5.33334 degrees

    # curving and speed acceleration
    if current_dir != Vector2.zero() and dot(current_dir, si.aim) >= 0.45:  # angle after rotating < acos(.45)
        si.star_fly_speed_lerp = approach(si.star_fly_speed_lerp, 1, DELTA_TIME)
        max_speed = lerp(140, 190, si.star_fly_speed_lerp)
    else:  # don't go here
        si.star_fly_speed_lerp = 0
        max_speed = 140  # approach 140 speed

    # update speed
    num = si.speed.length()
    num = approach(num, max_speed, 1000 * DELTA_TIME)
    si.speed = current_dir * num


def collision(si: Sim, spinners: List[Tuple[int, int]], killbox: List[Tuple[int, int, int, int]]) -> bool:
    for s in spinners:
        if (si.position.x - s[0]) ** 2 + (si.position.y + 6 - s[1]) ** 2 < 141:
            if (s[0] - 10.5 < si.position.x < s[0] + 10.5 and s[1] + 0.5 < si.position.y < s[1] + 9.5) or (
                    s[0] - 8.5 < si.position.x < s[0] + 8.5 and s[1] - 0.5 < si.position.y < s[1] + 12.5) or (
                    s[0] - 7.5 < si.position.x < s[0] + 7.5 and s[1] - 1.5 < si.position.y < s[1] + 13.5) or (
                    s[0] - 6.5 < si.position.x < s[0] + 6.5 and s[1] - 2.5 < si.position.y < s[1] + 14.5):
                return True
    for k in killbox:
        if k[0] - 2.5 < si.position.x < k[2] + 2.5 and k[1] + 3.5 < si.position.y < k[3] + 8.5:
            return True
    return False


DELTA_TIME = 0.0166667

if __name__ == '__main__':
    main()
