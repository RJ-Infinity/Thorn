from typing import TextIO
import sys
from dataclasses import dataclass
from enum import Enum, auto as iota

class ProgramType(Enum):
	Þ = iota()
	Thorn = Þ
	Þasm = iota()
	Thasm = Þasm

class ExecutableType(Enum):
	pass

@dataclass
class ProgramOp:
	def __post_init__(self):
		if self.__class__ == ProgramOp:
			raise RuntimeError("this class should never be created")
	InputFile: str
	Type: any

@dataclass
class ProgramOpToBin(ProgramOp):
	Type: ProgramType
@dataclass
class ProgramOpFromBin(ProgramOp):
	Type: ExecutableType
@dataclass
class ProgramOpInterpretBin(ProgramOp):
	Type: None = None

def printHelp(args:list[str],file:TextIO=sys.stdout):print(
	f"`{args[0]}`:\n"\
	f"\t`--help`, `-h`, `--h`, `-help`, `/?`: Prints this help message and exits\n"\
	f"\t`-i`, `--interpret` <file>: interperates a binary file\n"\
	f"\t`-c`, `--compile` <input file> <type> <output file>: takes a file of either Þ(Thorn) or Þasm(Thasm) and compiles it to binary also takes a type being one of `{'`, `'.join(ProgramType.__members__.keys())}` to tell the program what you want to comple to binary and an output file\n"\
	f"\t`-t`, `--translate` <input file> <type> <output file>: takes a binary file and compiles it to the specified type, supported types are `{'`, `'.join(ExecutableType.__members__.keys())}` also takes an output file\n"\
	f"these arguments can be chained to make the compiler do multiple things for instance you could run `{args[0]} -c ./main.þ Þ ./out.bin -i ./out.bin` to get the compiler to compile the `main.þ` file to binary and then interpret the binary",
	file=file
)

def hasEnoughArgs(curr, needed, length):return length-1 < curr+needed
def checkArgLength(args, curr, needed):
	if hasEnoughArgs(curr, needed, len(args)):
		print(f"Error: Not enough arguments for `{args[curr]}`\nBelow is the help message\n",file=sys.stderr)
		printHelp(args,sys.stderr)
		sys.exit(1)
def parse_args(args:list[str]) -> list[ProgramOp]:
	programOps: list[ProgramOp] = []
	LArgs:list[str] = [arg.lower() for arg in args]
	i:int = 1 # skip the program name
	while i < len(args):
		if LArgs[i] in ["--help", "-h", "--h", "-help", "/?"]:
			printHelp(args)
			sys.exit()
		if LArgs[i] in ["-i", "--interpret"]:
			checkArgLength(args, i, 1)
			programOps.append(ProgramOpInterpretBin(args[i+1]))
			i+=2
			continue
		if LArgs[i] in ["--c", "--compile"]:
			checkArgLength(args, i, 2)
			programOps.append(ProgramOpInterpretBin(args[i+1], args[i+2]))
			i+=3
			continue
		if LArgs[i] in ["--t", "--translate"]:
			checkArgLength(args, i, 2)
			programOps.append(ProgramOpInterpretBin(args[i+1], args[i+2]))
			i+=3
			continue
		print(f"Error: Either too many arguments for the last operation or an unknown flag `{args[i]}`\nBelow is the help message\n",file=sys.stderr)
		printHelp(args)
		sys.exit()
	return programOps

def main(state:list[ProgramOp]):
	print(state)

if __name__ == "__main__":
	main(parse_args(sys.argv))