import os
from distutils.core import setup

from Cython.Build import cythonize


def main():
    os.rename('feather_sim.py', 'feather_sim_c.py')

    try:
        setup(name='feather_sim_c', ext_modules=cythonize('feather_sim_c.py', nthreads=2, annotate=True, compiler_directives={'warn.unused': True, 'warn.unused_arg': True,
                                                                                                                              'warn.unused_result': True}))
    except Exception:
        raise
    finally:
        os.rename('feather_sim_c.py', 'feather_sim.py')


if __name__ == '__main__':
    main()
