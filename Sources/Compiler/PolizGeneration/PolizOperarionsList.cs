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
				if (polizOperation.Operation == operation)
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
				if (polizOperation.Operation == lexem.Command)
				{
					return polizOperation.Priority;
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
				operations.Add(PolizOperation.NewOperation(oper,priority));
			}
		}

		public PolizOperarionsList()
		{
			AddOperations(-1,"for");
			AddOperations(0,"(","[","{","if","output","input");
			AddOperations(1,")","]","}","then","else","step","to","do","next");
			AddOperations(2,"=");
			AddOperations(3,"or");
			AddOperations(4,"and");
			AddOperations(5,"!");
			AddOperations(6,">","<",">=","<=","equ","!=");
			AddOperations(7,"+","-");
			AddOperations(8,"*","/","%");
			AddOperations(9,"^","root");
		}

		public static int kLexemKeyLabelStart { get { return LexemList.Instance.Grammar.Count; } }
		public static int kLexemKeyLabelEnd { get { return LexemList.Instance.Grammar.Count + 1; } }
		public static int kLexemKeyUPL { get { return LexemList.Instance.Grammar.Count + 2; } }
		public static int kLexemKeyBP { get { return LexemList.Instance.Grammar.Count + 3; } }
	}
}

