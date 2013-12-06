using System;
using System.Collections;
using System.Collections.Generic;

namespace Translators.Lab01
{
	public class AutomatTable
	{
		private Dictionary<int,State> states = new Dictionary<int, State>();
		public void AddState(int number, List<Transition> transitions, string errorMessage)
		{
			states.Add(number, new State(number,transitions,errorMessage));
		}
		public State StateWithNumber(int number)
		{
			State state = null;
			try 
			{
				state = states[number];
			} 
			catch 
			{
				throw new Exception("[Bug] Invalid state number");
			};
			return state;
		}
	}
}

