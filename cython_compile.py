import os
from distutils.core import setup

from Cython.Build import cythonize


def main():
    files = ('main.py', 'feather_sim.py')

    for file in files:
        file_c = file.replace('.py', '_c.py')
        os.rename(file, file_c)

        try:
            setup(name=file_c, ext_modules=cythonize(file_c, nthreads=2, annotate=True, compiler_directives={'warn.unused': True, 'warn.unused_arg': True, 'warn.unused_result': True}))
        except Exception:
            raise
        finally:
            os.rename(file_c, file)


if __name__ == '__main__':
    main()
