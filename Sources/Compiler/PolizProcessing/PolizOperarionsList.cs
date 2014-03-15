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
				if (polizOperation.operation == lexem.command)
				{
					return polizOperation.priority;
				}
			}
			return int.MaxValue;
		}

		public bool OpenScobe(Lexem lexem)
		{
			return lexem.command == "(" || lexem.command == "[";
		}
		
		public bool CloseScobe(Lexem lexem)
		{
			return lexem.command == ")" || lexem.command == "]";
		}

		public PolizOperarionsList()
		{
			operations.Add(PolizOperation.Operation("(",0));
			operations.Add(PolizOperation.Operation(")",1));
			operations.Add(PolizOperation.Operation("+",1));
			operations.Add(PolizOperation.Operation("-",1));
			operations.Add(PolizOperation.Operation("*",2));
			operations.Add(PolizOperation.Operation("/",2));
			operations.Add(PolizOperation.Operation("^",3));
		}
	}
}

