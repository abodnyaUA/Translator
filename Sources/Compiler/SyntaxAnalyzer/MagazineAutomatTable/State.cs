using System;
using System.Collections.Generic;

namespace Translators
{
	public class State
	{
		private List<Transition> transitions;
		private string errorMessage;
		private int number;
		public State (int number, List<Transition> transitions, string errorMessage)
		{
			this.transitions = transitions;
			this.number = number;
			this.errorMessage = errorMessage;
		}
		public void Run(Lexem inputLexem, ref int StateIterator, ref int lexemsIterator)
		{
			foreach (Transition transition in transitions)
			{
				if (transition.RespondLexem(inputLexem))
				{
					transition.PerformLexem(inputLexem,ref StateIterator, ref lexemsIterator);
					return;
				}
			}
			throw new LexemException(inputLexem.LineNumber,errorMessage);
		}
	}
}

