using System;

namespace Translators.Lab01
{
	public class Transition
	{
		enum StackUsing {UsePop, UsePush, NoUse};

		public static string LexemID 	= "__ID__";
		public static string LexemCONST = "__CONST__";
		public static string NoLexem 	= null;

		private Lexem lexem;
		private int newState;
		private int stackState;
		private StackUsing stackUsing;

		static public Transition DefaultTransition(string lexemString, int newState)
		{
			Transition transition = new Transition();
			transition.lexem = lexemString;
			transition.newState = newState;
			transition.stackState = int.MaxValue;
			transition.stackUsing = StackUsing.NoUse;
			return transition;
		}

		static public Transition ExitTransition(string lexemString)
		{
			Transition transition = new Transition();
			transition.lexem = lexemString;
			transition.newState = int.MaxValue;
			transition.stackState = int.MaxValue;
			transition.stackUsing = StackUsing.UsePop;
			return transition;
		}

		static public Transition CallTransition(string lexemString, int newState, int stackState)
		{
			Transition transition = new Transition();
			transition.lexem = lexemString;
			transition.newState = newState;
			transition.stackState = stackState;
			transition.stackUsing = StackUsing.UsePush;
			return transition;
		}

		public bool RespondLexem(Lexem lexem)
		{
			if (lexem == null && this.lexem == Transition.NoLexem)
			{
				return true;
			}
			if (lexem.key == LexemAnalyzer.sharedAnalyzer.dict.Count - 2 && this.lexem == Transition.LexemID)
			{
				return true;
			}
			if (lexem.key == LexemAnalyzer.sharedAnalyzer.dict.Count - 1 && this.lexem == Transition.LexemCONST)
			{
				return true;
			}
			if (lexem.command == this.lexem)
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

			if (lexem != Transition.NoLexem)
			{
				lexemsIterator++;
			}
		}
	}
}

