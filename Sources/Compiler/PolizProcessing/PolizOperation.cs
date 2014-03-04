using System;

namespace Translators
{
	public class PolizOperation
	{
		public string operation;
		public int priority;
		public PolizOperation()
		{
		}

		static public PolizOperation Operation(string opearation, int priority)
		{
			PolizOperation polizOperation = new PolizOperation();
			polizOperation.operation = opearation;
			polizOperation.priority = priority;
			return polizOperation;
		}
	}
}

