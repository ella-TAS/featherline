# tntfalle, 16.11.2021

import math
import random
import time

import requests
import yaml


def main():
    check_connection()
    s = load_settings()
    s = format_settings(s)
    check_file(s["tas_file"])
    p = create_population(s)
    # schleife
    p = simulate_population(s, p)

    for i in p:
        print(i)
        print("\n\n\n")

    input(">>>>>>>>>>>>>>>>>>>>>")


# change this to get better results (cateline)
def fitness(s: dict[str, any], data: (int, int, bool)) -> float:
    posx, posy, speedx, speedy, dead = data

    if s["axis"] == "x":
        return (posx if s["prim_dir"] == "+" else -posx) - (0 if (s["sec_min"] <= posy <= s["sec_max"]) or (s["sec_max"] <= posy <= s["sec_min"]) else
                                                            s["sec_factor"] * min(abs(posy - s["sec_min"]), abs(posy - s["sec_max"]))) - (100000 if dead else 0)
    elif s["axis"] == "y":
        return (posy if s["prim_dir"] == "+" else -posy) - (0 if (s["sec_min"] <= posx <= s["sec_max"]) or (s["sec_max"] <= posx <= s["sec_min"]) else
                                                            s["sec_factor"] * min(abs(posx - s["sec_min"]), abs(posx - s["sec_max"]))) - (100000 if dead else 0)
    else:
        return 100000 - math.hypot(s["goal_x"] - posx, s["goal_y"] - posy) - (100000 if dead else 0)


def simulate_population(s: dict[str, any], p: list[list[list[float]]]) -> list[list[list[float], int]]:
    for i in p:
        i.append(fitness(s, play_tas(s, i[0])))

    p.sort(key=take_second, reverse=True)
    return p


def mutate(strength: float, inputs: list[float]):
    pass


def create_population(s: dict[str, any]) -> list[list[float]]:
    p = []

    for i in range(s["population"]):
        p.append([random_perm(s["dna_length"])])

    if len(s["favorite"]) != 0:
        p[-1] = [s["favorite"]]

    return p


def random_perm(length: int) -> list[float]:
    perm = []

    for i in range(length):
        perm.append(random.randint(0, 35999) / 100)

    return perm


def play_tas(s: dict[str, any], t: list[float]) -> (float, float):
    inputs = [s["header"]]

    for i in t:
        inputs.append(f"1,F,{str(i)}")

    inputs.append("***\n1")

    with open(s["tas_file"], "w") as tas:
        tas.write("\n".join(inputs))

    requests.post('http://localhost:32270/tas/sendhotkey?id=Restart', timeout=5)

    while True:
        time.sleep(s["tas_wait"])
        session_data = requests.get('http://localhost:32270/tas/info', timeout=2).text

        if session_data.split("State")[1].split('<')[0] == ': Enable, FrameStep':
            break
        else:
            print("tas_wait too short")

    pos = session_data.split("Pos:   ")[1].split("\r\nSpeed: ")[0].split(",")
    return float(pos[0]), float(pos[1]), "Dead" in session_data


def format_settings(s: dict[str, any]) -> dict[str, any]:
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

    s["header"] = s["console_load"].strip() + s["header"].replace(" ", "\n") + "***\n"

    if s["axis"] != "x" and s["axis"] != "y" and s["axis"] != "radial":
        raise SystemError("Invalid axis: must be x, y or radial")

    if s["prim_dir"] != "+" and s["prim_dir"] != "-":
        print("Invalid prim_dir: must be + or -")

    print("Config OK")
    return s


def load_settings() -> dict[str, any]:
    try:
        with open("config.yaml", "r") as config_file:
            settings = yaml.safe_load(config_file)

        str(settings["tas_file"])
        str(settings["console_load"])
        str(settings["header"])
        str(settings["favorite"])
        int(settings["dna_length"])
        int(settings["population"])
        str(settings["axis"])
        str(settings["prim_dir"])
        float(settings["sec_min"])
        float(settings["sec_max"])
        float(settings["sec_factor"])
        float(settings["goal_x"])
        float(settings["goal_y"])
        float(settings["tas_wait"])

        return settings
    except yaml.YAMLError as error:
        print(f"Couldn't parse settings:\n{str(error)}")
    except (ValueError, TypeError, KeyError, FileNotFoundError) as error:
        print(f"Invalid config file: {repr(error)}")

    raise SystemExit


def check_file(path: str):
    try:
        with open(path, "w") as file:
            file.write("test")

        print("TAS file OK")
        return
    except FileNotFoundError:
        print("TAS file error: file not found")
    except PermissionError:
        print("TAS file error: no writing permission")
    raise SystemError


def check_connection():
    while True:
        try:
            requests.get('http://localhost:32270/tas/info', timeout=2)
        except requests.ConnectionError:
            print("No connection to DebugRC server, please restart your game")
            input("press enter to try again >>>")
        else:
            print("DebugRC OK")


def take_second(x: list[list]) -> any:
    return x[1]


if __name__ == "__main__":
    main()
