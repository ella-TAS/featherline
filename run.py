try:
    import main_c as main
except ModuleNotFoundError:
    import main


def run():
    main.main()


if __name__ == '__main__':
    run()
