using System;
using System.Collections.Generic;

namespace Translators
{
	public class Stack
	{
		private static List<Action> _stack = new List<Action>();

		public static Action WrongLexem = null;

		public static void Push(Action value)
		{
			_stack.Add(value);
		}

		public static Action Pop()
		{
			Action returnValue = Stack.Last();
			if (returnValue != Stack.WrongLexem)
			{
				_stack.RemoveAt(_stack.Count-1);
			}
			return returnValue;
		}

		public static Action Last()
		{
			if (_stack.Count > 0)
			{
				Action returnValue = _stack[_stack.Count-1];
				return returnValue;
			}
			return Stack.WrongLexem;
		}
	}
}

