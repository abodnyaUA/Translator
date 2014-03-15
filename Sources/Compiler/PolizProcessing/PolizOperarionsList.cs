using System;
using System.Collections.Generic;

namespace Translators
{
	public class PolizOperarionsList
	{
		private List<PolizOperation> operations = new List<PolizOperation>();
		public bool isOperation(string operation)
		{
			bool exist = false;
			foreach (PolizOperation polizOperation in this.operations)
			{
				if (polizOperation.operation == operation)
				{
					exist = true;
				}
			}
			return exist;
		}

		public int LexemPriority(Lexem lexem)
		{
			foreach (PolizOperation polizOperation in this.operations)
			{
				if (polizOperation.operation == lexem.Command)
				{
					return polizOperation.priority;
				}
			}
			return int.MaxValue;
		}

		public bool OpenScobe(Lexem lexem)
		{
			return lexem.Command == "(" || lexem.Command == "[";
		}
		
		public bool CloseScobe(Lexem lexem)
		{
			return lexem.Command == ")" || lexem.Command == "]";
		}

		private void AddOperations(int priority, params string[] operators)
		{
			foreach (string oper in operators)
			{
				operations.Add(PolizOperation.Operation(oper,priority));
			}
		}

		public PolizOperarionsList()
		{
			AddOperations(0,"(","[");
			AddOperations(1,")","]");
			AddOperations(2,"=");
			AddOperations(3,"or");
			AddOperations(4,"and");
			AddOperations(5,"!");
			AddOperations(6,">","<",">=","<=","equ","!=");
			AddOperations(7,"+","-");
			AddOperations(8,"*","/");
			AddOperations(9,"^");
		}
	}
}

