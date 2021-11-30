@echo off
python cython_compile.py build_ext --inplace
python run.py
pause