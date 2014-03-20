using System;

namespace Translators
{
	public class PolizOperation
	{
		private PolizOperation() { }

		private string operation;
		private int priority;
		public string Operation { get { return operation; } }
		public int Priority { get { return priority; } }

		static public PolizOperation NewOperation(string opearation, int priority)
		{
			PolizOperation polizOperation = new PolizOperation();
			polizOperation.operation = opearation;
			polizOperation.priority = priority;
			return polizOperation;
		}
	}
}

