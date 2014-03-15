using System;

namespace Translators
{
	public class Transition
	{
		enum StackUsing {UsePop, UsePush, NoUse};

		public static string LexemID 	= "__ID__";
		public static string LexemCONST = "__CONST__";
		public static string NoLexem 	= null;

		private string lexem;
		private int nextState;
		private int exitState;
		private StackUsing stackUsing;

		static public Transition DefaultTransition(string lexemString, int nextState)
		{
			Transition transition = new Transition();
			transition.lexem = lexemString;
			transition.nextState = nextState;
			transition.exitState = int.MaxValue;
			transition.stackUsing = StackUsing.NoUse;
			return transition;
		}

		static public Transition ExitTransition(string lexemString)
		{
			Transition transition = new Transition();
			transition.lexem = lexemString;
			transition.nextState = int.MaxValue;
			transition.exitState = int.MaxValue;
			transition.stackUsing = StackUsing.UsePop;
			return transition;
		}

		static public Transition CallTransition(string lexemString, int nextState, int exitState)
		{
			Transition transition = new Transition();
			transition.lexem = lexemString;
			transition.nextState = nextState;
			transition.exitState = exitState;
			transition.stackUsing = StackUsing.UsePush;
			return transition;
		}

		public bool NeedEmptyLexem()
		{
			return (this.lexem == Transition.NoLexem);
		}

		public bool RespondLexem(Lexem lexem)
		{
			if (this.lexem == Transition.NoLexem)
			{
				return true;
			}
			if (lexem.isID() && this.lexem == Transition.LexemID)
			{
				return true;
			}
			if (lexem.isCONST() && this.lexem == Transition.LexemCONST)
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
					stateIterator = this.nextState;
					break;
				}
				case StackUsing.UsePop:
				{
					int newState = SyntaxAnalyzerWithTable.sharedAnalyzer.stack.Pop();
					if (newState == int.MaxValue)
					{
						Out.Log(Out.State.LogInfo,"End of File");
					}
					stateIterator = newState;
					break;
				}
				case StackUsing.UsePush:
				{
					SyntaxAnalyzerWithTable.sharedAnalyzer.stack.Push(this.exitState);
					stateIterator = this.nextState;
					break;
				}
			}

			if (this.lexem != NoLexem)
			{
				lexemsIterator++;
			}
		}
	}
}

