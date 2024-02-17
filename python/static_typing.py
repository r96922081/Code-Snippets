from typing import List


def f(a: int, b: List[str]) -> int:
    return 0


f('a', 2)

ret:str = f(2, ['a', 'b'])

'''
pip install mypy
mypy.exe static_typing.py

static_typing.py:8: error: Argument 1 to "f" has incompatible type "str"; expected "int"  [arg-type]
static_typing.py:8: error: Argument 2 to "f" has incompatible type "int"; expected "list[str]"  [arg-type]
static_typing.py:10: error: Incompatible types in assignment (expression has type "int", variable has type "str")  [assignment]
Found 3 errors in 1 file (checked 1 source file)

'''