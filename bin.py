from dataclasses import dataclass
from enum import Enum, Flag as FlagEnum, auto as iota

class datatype(FlagEnum):
	Register = iota()
	Number = iota()
	def __repr__(self):return f"|".join([d.name for d in self.__class__ if d in self])

@dataclass(eq=False) # no equal otherwise the enum thinks they are the same operation
class OpSig:
	args: list[datatype]
	def __repr__(self):return f"<{self.__class__.__name__} <{', '.join([repr(a) for a in self.args])}>>"

class OpDef(Enum):
	@classmethod
	def from_int(cls, i: int) -> OpSig:return cls._member_map_[cls._member_names_[i]]

	halt = OpSig([])
	nop = OpSig([])
	dump = OpSig([])
	mov = OpSig([datatype.Register|datatype.Number,datatype.Register])

	add = OpSig([datatype.Register,datatype.Register|datatype.Number])
	sub = OpSig([datatype.Register,datatype.Register|datatype.Number])

@dataclass(eq=False,repr=False)
class Register:
	def __repr__(self):return f"[{'(unnamed)'if self.name == None else self.name} <{self.value}>]"
	value: int
	name: None | str = None

class RegisterPtr(int):
	def __new__(cls, *args, **kwargs):
		rv = super().__new__(cls, *args, **kwargs)
		if rv < 0:raise ValueError(f"{cls.__name__} is unsigned therefore must be positive")
		return rv
	def __str__(self): return f"<{str(int(self))}>"
	def __repr__(self): return f"{self.__class__.__name__}<{str(int(self))}>"
@dataclass
class Op:
	def __post_init__(self):
		assert len(datatype) == 2, "Non exahaustive match"
		for i, arg in enumerate(self.args):
			if (
				(datatype.Register not in self.op.value.args[i] and type(arg) == RegisterPtr) or
				(datatype.Number not in self.op.value.args[i] and type(arg) == int)
			): raise ValueError(f"the {i} value in the argument list is an invalid type. it must match the type in the given OpDef.")
	op: OpDef
	args: list[RegisterPtr|int]

@dataclass
class Program:
	Ops: list[Op]
	Registers: list[Register]
	PC: int = 0
