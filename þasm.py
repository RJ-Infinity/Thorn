from dataclasses import dataclass
from typing import TextIO

import interpreter
import bin

@dataclass(repr=False)
class token:
	content: str
	startLine: int
	startRow: int
	def __repr__(self):return f"<token:{self.startIndex}:{self.startRow} {repr(self.content)}>"

class assembler:
	stream: TextIO
	tokens: list[token] = []
	registers: list[bin.Register] = []
	Ops: list[bin.Op] = []
	def __init__(self, stream:TextIO):
		self.stream = stream

	def tokenise(self):
		currToken:str=""
		startPos:int = None
		startLine:int = None
		currPos:int = 0
		currLine:int = 1 #lines are 0 based
		while char := self.stream.read(1):
			currPos += 1
			if char == "\n":
				currLine += 1
				currPos = 0
			if char.isspace():
				if len(currToken) > 0:
					self.tokens.append(token(currToken,startPos,startLine))
					currToken = ""
					startPos = None
				if char == "\n" and self.tokens[-1].content != "\n":
					self.tokens.append(token("\n",currPos,currLine))
			else:
				if startPos == None:
					startPos = currPos
					startLine = currLine
				currToken += char
		if startPos != None:
			self.tokens.append(token(currToken,startPos,currLine))
		if self.tokens[-1].content != "\n":
			self.tokens.append(token("\n",currPos,currLine))
	def parseLitteral(self, tknContent: str) -> int|bin.RegisterPtr:
		if tknContent.isdigit():
			return int(tknContent)
		try:
			return bin.RegisterPtr([r.name for r in self.registers].index(tknContent))
		except:
			self.registers.append(bin.Register(0,tknContent))
			return bin.RegisterPtr(len(self.registers) - 1)
	def assemble(self):
		inst: token = None
		args:list[int|bin.RegisterPtr] = []
		for tkn in self.tokens:
			if inst == None:
				inst = tkn
				args = []
			elif tkn.content == "\n":
				if inst.content not in bin.OpDef._member_map_:
					assert False, f"ERROR INVALID INST {inst}"
				self.Ops.append(bin.Op(bin.OpDef._member_map_[inst.content],args))
				inst = None
			else:
				args.append(self.parseLitteral(tkn.content))
	def createProgram(self) -> bin.Program:return bin.Program(self.Ops,self.registers)
if __name__ == "__main__":
	# try:
	asmb = assembler(open("test1.thasm","r"))
	asmb.tokenise()
	asmb.assemble()
	prog = asmb.createProgram()
	print(prog)
	interpreter.interpretProgram(prog)
	# except Exception as e:
	# 	print(e)
	# finally:
	# 	import IPython
	# 	# IPython.embed()