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

		public PolizOperarionsList ()
		{
			//operations.Add(PolizOperation.Operation("(",0));
			//operations.Add(PolizOperation.Operation(")",1));
			operations.Add(PolizOperation.Operation("+",1));
			operations.Add(PolizOperation.Operation("-",1));
			operations.Add(PolizOperation.Operation("*",2));
			operations.Add(PolizOperation.Operation("/",2));
			operations.Add(PolizOperation.Operation("^",3));
		}
	}
}

