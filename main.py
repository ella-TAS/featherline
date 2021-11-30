# tntfalle, 16.11.2021
# Kataiser, cake, TheRoboMan
# cython: language_level=3

import os
import random
import re
import sys
from typing import Dict, List, Tuple

import tqdm
import yaml
from pyeasyga import pyeasyga

try:
    import feather_sim_c as feather_sim
except ModuleNotFoundError:
    import feather_sim


def main():
    sys.stdout = Logger()
    s = load_settings()

    if s["spinner_file"] != "":
        check_file(s["spinner_file"])
        s["spinners"] += import_spinners(s["spinner_file"])

    ga = pyeasyga.GeneticAlgorithm(s, s["population"], s["generations"], s["mutation_probability"], s["crossover_probability"], s["elitism"])
    ga.fitness_function = fitness
    ga.create_individual = create_individual
    ga.crossover_function = crossover
    ga.mutate_function = mutate
    print("Starting Genetic Algorithm\n\n")
    ga.create_first_generation()

    for _ in tqdm.trange(1, ga.generations, initial=1, total=ga.generations, ncols=100):
        ga.create_next_generation()

    print("Last generation:\n")
    for individual in ga.last_generation():
        print(individual)

    print("\n\nBest individual:\n")
    print(ga.best_individual())

    print(to_inputs(ga.best_individual()[1]))


# change this to get better results (cateline)
def fitness(individual: List[float], s: Dict[str, any]) -> float:
    posx, posy, speedx, speedy, dead = feather_sim.sim(s["pos_x"], s["pos_y"], individual, s["spinners"], s["killboxes"], s["boost_x"], s["boost_y"])
    dead_offset = 100000 if dead else 0

    if s["goal"] in ("x", "y"):
        pos = posx if s["goal"] == "x" else posy

        return (pos if s["prim_dir"] else -pos) - dead_offset - (0 if (s["sec_min"] <= pos <= s["sec_max"]) or (s["sec_max"] <= pos <= s["sec_min"]) else
                                                                 s["sec_factor"] * min(abs(pos - s["sec_min"]), abs(pos - s["sec_max"])))
    else:
        return 100000 - (s["goal_x"] - posx) ** 2 + (s["goal_y"] - posy) ** 2 - dead_offset


def crossover(parent_1: List[float], parent_2: List[float]) -> Tuple[any, any]:
    if random.random() < 0.5:
        # exchange parts
        index = random.randrange(1, len(parent_1))
        child_1 = parent_1[:index] + parent_2[index:]
        child_2 = parent_2[:index] + parent_1[index:]
    else:
        # exchange every other value
        child_1, child_2 = parent_1, parent_2

        for i in range(0, len(parent_1), 2):
            child_1[i], child_2[i] = child_2[i], child_1[i]

    return child_1, child_2


def mutate(individual: List[float]):
    # change multiple values
    length = random.randrange(int(len(individual) / 2))
    x = random.randrange(len(individual) - length)
    increment = random.gauss(0, 2)

    for i in range(x, x + length):
        individual[i] = round(individual[i] + increment, 3)

        if individual[i] >= 360:
            individual[i] -= 360
        elif individual[i] < 0:
            individual[i] += 360

    # change a single value (for more precise optimisation) (rounding missing)
    # individual[random.randrange(len(individual))] += random.randint(-4000, 4000) / 1000

    # simplify (bias towards shorter inputs)
    # length = random.randrange(int(len(individual) / 4))
    # start = random.randint(1, len(individual) - length)
    #
    # for i in range(start, start + length):
    #     individual[i] = round(individual[start - 1], 3)


def create_individual(s: Dict[str, any]) -> List[float]:
    return s["favorite"] if s["favorite"] else [random.randrange(0, 360000) / 1000 for _ in range(s["dna_length"])]


def format_settings(s: Dict[str, any]) -> Dict[str, any]:
    s["spinner_file"] = s["spinner_file"].strip("\n").strip()
    s["spinners"] = [[float(j) for j in i.split(",")] for i in s["spinners"].split()]
    s["killboxes"] = [[float(j) for j in i.split(",")] for i in s["killboxes"].split()]

    if s["favorite"].strip("\n").strip() == "":
        s["favorite"] = []
    else:
        s["favorite"] = s["favorite"].split()
        n_fav = []

        for i in s["favorite"]:
            cache = i.split(",")

            try:
                for j in range(int(cache[0])):
                    n_fav.append(float(cache[2]))
            except ValueError:
                raise SystemError(f"Invalid favorite:\n{i}")

        if len(n_fav) != s["dna_length"]:
            raise SystemError(f"Invalid favorite:\nLength {len(n_fav)} isn't equal to dna_length")

        s["favorite"] = n_fav

    if s["goal"] != "x" and s["goal"] != "y" and s["goal"] != "radial":
        raise SystemError("Invalid goal: must be x, y or radial")

    print("Config OK")
    return s


def load_settings() -> Dict[str, any]:
    try:
        with open("config.yaml", "r") as config_file:
            settings = yaml.safe_load(config_file)

        # check for correct keys/values
        settings["killboxes"] = str(settings["killboxes"])
        settings["spinners"] = str(settings["spinners"])
        settings["favorite"] = str(settings["favorite"])
        settings["goal"] = str(settings["goal"])
        settings["spinner_file"] = str(settings["spinner_file"])
        settings["dna_length"] = int(settings["dna_length"])
        settings["population"] = int(settings["population"])
        settings["generations"] = int(settings["generations"])
        settings["prim_dir"] = bool(int(settings["prim_dir"]))
        settings["elitism"] = bool(int(settings["elitism"]))
        settings["pos_x"] = float(settings["pos_x"])
        settings["pos_y"] = float(settings["pos_y"])
        settings["boost_x"] = float(settings["boost_x"])
        settings["boost_y"] = float(settings["boost_y"])
        settings["crossover_probability"] = float(settings["crossover_probability"])
        settings["mutation_probability"] = float(settings["mutation_probability"])
        settings["sec_min"] = float(settings["sec_min"])
        settings["sec_max"] = float(settings["sec_max"])
        settings["sec_factor"] = float(settings["sec_factor"])
        settings["goal_x"] = float(settings["goal_x"])
        settings["goal_y"] = float(settings["goal_y"])
    except yaml.YAMLError as error:
        print(f"Couldn't parse settings:\n{str(error)}")
    except (ValueError, TypeError, KeyError, FileNotFoundError) as error:
        print(f"Invalid config file: {repr(error)}")
    else:
        return format_settings(settings)

    raise SystemExit


def check_file(path: str):
    try:
        with open(path, "r") as file:
            file.read()

        print("Spinner file OK", path)
    except FileNotFoundError:
        raise SystemError("Spinner file error: file not found")
    except PermissionError:
        raise SystemError("Spinner file error: no reading permission")


def import_spinners(path: str) -> List[Tuple[int, int]]:
    with open(path, "r") as file:
        gameinfo = file.readlines()

    matches = re.findall(r"CrystalStaticSpinner: (-?\d+\.\d+), (-?\d+\.\d+)", gameinfo[1])
    return [(round(float(m[0])), round(float(m[1]))) for m in matches]


def to_inputs(s: Tuple[float, List[float]]) -> str:
    r = []

    for i in s:
        r.append(f"\n1,F,{str(i)}")

    return ''.join(r)


# log all prints to a file
class Logger(object):
    def __init__(self):
        self.filename: str = 'out.log'

        if os.path.isfile(self.filename):
            os.remove(self.filename)

        self.terminal = sys.stdout
        self.log = open(self.filename, 'a')
        self.print_enabled: bool = True

    def write(self, message):
        self.log.write(message)
        self.log.flush()

        if self.print_enabled:
            self.terminal.write(message)

    def flush(self):
        pass


if __name__ == "__main__":
    main()
