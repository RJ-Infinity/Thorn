from dataclasses import dataclass

import bin

def typeAssert(opType:bin.OpDef,args:list[int|bin.RegisterPtr]):
	assert type(args) == list or len(opType.value.args) == len(args)
	for i, argDef in enumerate(opType.value.args):
		assert (
			(bin.datatype.Number in argDef and type(args[i]) == int) or
			(bin.datatype.Register in argDef and type(args[i]) == bin.RegisterPtr)
		)
def getValue(prog:bin.Program,val:int|bin.RegisterPtr): return val if type(val) == int else prog.Registers[val].value


def interpretProgram(prog: bin.Program):
	while True:
		nextPC:int = prog.PC+1
		assert len(bin.OpDef) == 6, "Non exahaustive match"
		args = None # this is so the args created in the match dosent leak to another itteration
		match prog.Ops[prog.PC]:
			case bin.Op(op=bin.OpDef.halt): break
			case bin.Op(op=bin.OpDef.nop):pass
			case bin.Op(op=bin.OpDef.dump):
				print("[\n\t"+",\n\t".join([repr(r) for r in prog.Registers])+"\n]")
			case bin.Op(op=bin.OpDef.mov,args=args):
				typeAssert(bin.OpDef.mov,args)
				prog.Registers[args[1]].value = getValue(prog,args[0])
			case bin.Op(op=bin.OpDef.add,args=args):
				typeAssert(bin.OpDef.add,args)
				prog.Registers[args[0]].value += getValue(prog,args[1])
			case bin.Op(op=bin.OpDef.sub,args=args):
				typeAssert(bin.OpDef.sub,args)
				prog.Registers[args[0]].value -= getValue(prog,args[1])
			case op: raise ValueError(f"Unexpected Operation `{op.op.name}`")
		prog.PC = nextPC

def main():interpretProgram(bin.Program([
	bin.Op(bin.OpDef.mov,[10,bin.RegisterPtr(0)]),
	bin.Op(bin.OpDef.mov,[11,bin.RegisterPtr(1)]),
	bin.Op(bin.OpDef.mov,[5,bin.RegisterPtr(2)]),
	bin.Op(bin.OpDef.add,[bin.RegisterPtr(0),bin.RegisterPtr(1)]),
	bin.Op(bin.OpDef.sub,[bin.RegisterPtr(0),bin.RegisterPtr(2)]),

	bin.Op(bin.OpDef.dump,[]),
	bin.Op(bin.OpDef.halt,[]),
],[bin.Register(0,"A"),bin.Register(0,"B"),bin.Register(0,"C")]))

if __name__ == "__main__": main()