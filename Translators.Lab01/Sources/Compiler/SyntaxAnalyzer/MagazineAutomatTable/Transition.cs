using System;

namespace Translators.Lab01
{
	public class Transition
	{
		enum StackUsing {UsePop, UsePush, NoUse};
		private Lexem lexem;
		private int newState;
		private int stackState;
		private StackUsing stackUsing;

		static public Transition DefaultTransition(Lexem lexem, int newState)
		{
			Transition transition = new Transition();
			transition.lexem = lexem;
			transition.newState = newState;
			transition.stackState = int.MaxValue;
			transition.stackUsing = StackUsing.NoUse;
			return transition;
		}

		static public Transition ExitTransition(Lexem lexem)
		{
			Transition transition = new Transition();
			transition.lexem = lexem;
			transition.newState = int.MaxValue;
			transition.stackState = int.MaxValue;
			transition.stackUsing = StackUsing.UsePop;
			return transition;
		}

		static public Transition CallTransition(Lexem lexem, int newState, int stackState)
		{
			Transition transition = new Transition();
			transition.lexem = lexem;
			transition.newState = newState;
			transition.stackState = stackState;
			transition.stackUsing = StackUsing.UsePush;
			return transition;
		}

		public bool RespondLexem(Lexem lexem)
		{
			if (lexem == null && this.lexem == null)
			{
				return true;
			}
			if (lexem.key == this.lexem.key)
			{
				return true;
			}
			return false;
		}

		public void PerformLexem(Lexem lexem, ref int stateIterator, ref int lexemsIterator)
		{
			if (false == RespondLexem(lexem)) return;
			switch (this.stackUsing)
			{
				case StackUsing.NoUse:
				{
					stateIterator = this.newState;
					break;
				}
				case StackUsing.UsePop:
				{
					int newState = SyntaxAnalyzerWithTable.sharedAnalyzer.stack.Pop();
					stateIterator = newState;
					break;
				}
				case StackUsing.UsePush:
				{
					SyntaxAnalyzerWithTable.sharedAnalyzer.stack.Push(this.stackState);
					stateIterator = this.newState;
					break;
				}
			}

			if (lexem != null)
			{
				lexemsIterator++;
			}
		}
	}
}

