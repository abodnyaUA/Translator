using System;
using System.Collections.Generic;

namespace Translators
{
	public class SyntaxAnalyzerWithAutomat : ISyntaxAnalyzer
	{
		private static SyntaxAnalyzerWithAutomat _sharedAnalyzer = null;
		public static SyntaxAnalyzerWithAutomat sharedAnalyzer
		{
			get
			{
				if (_sharedAnalyzer == null) _sharedAnalyzer = new SyntaxAnalyzerWithAutomat();
				return _sharedAnalyzer;
			}
		}

		private List<Lexem> lexems;
		public List<string> IDs;
		public List<string> CONSTs;
		public List<string> lexemsDict;
		private int lexemsIterator;
		public void AnalyzeLexems()
		{
			lexemsIterator = 0;
			this.lexems = LexemAnalyzer.sharedAnalyzer.Lexems;
			this.IDs = LexemAnalyzer.sharedAnalyzer.IDs;
			this.CONSTs = LexemAnalyzer.sharedAnalyzer.CONSTs;
			this.lexemsDict = LexemAnalyzer.sharedAnalyzer.dict;
			StartAutomat();
		}

		public string CurrentLexemValue() { return lexems[lexemsIterator].command; }
		public int CurrentLexemKey() { return lexems[lexemsIterator].key; }
		public int CurrentLineNumber() { return lexems[lexemsIterator].lineNumber; }
		public bool isCurrentLexemID() { return lexems[lexemsIterator].key == lexemsDict.Count-2; }
		public bool isCurrentLexemCONST() { return lexems[lexemsIterator].key == lexemsDict.Count-1; }

		public uint CurrentState;

		#region MainAutomat
		public void StartAutomat()
		{
			CurrentState = 1;
			lexemsIterator = 0;
			State_1();
		}

		private void State_1()
		{
			if (CurrentLexemValue() == "@interface")
			{
				CurrentState = 2;
				lexemsIterator++;
				State_2();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed @interface");
			}
		}

		private void State_2()
		{
			if (isCurrentLexemID())
			{
				CurrentState = 3;
				lexemsIterator++;
				State_3();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ID");
			}
		}

		private void State_3()
		{
			if (CurrentLexemValue() == "\n")
			{
				CurrentState = 4;
				lexemsIterator++;
				State_4();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ENTER");
			}
		}

		private void State_4()
		{
			if (CurrentLexemValue() == "int")
			{
				CurrentState = 5;
				lexemsIterator++;
				State_5();
			}
			else if (CurrentLexemValue() == "@implementation")
			{
				CurrentState = 7;
				lexemsIterator++;
				State_7();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed int or @implementation");
			}
		}

		private void State_5()
		{
			if (isCurrentLexemID())
			{
				CurrentState = 6;
				lexemsIterator++;
				State_6();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ID");
			}
		}

		private void State_6()
		{
			if (CurrentLexemValue() == ",")
			{
				CurrentState = 5;
				lexemsIterator++;
				State_5();
			}
			else if (CurrentLexemValue() == "\n")
			{
				CurrentState = 4;
				lexemsIterator++;
				State_4();
			} 
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ,");
			}
		}

		private void State_7()
		{
			if (CurrentLexemValue() == "\n")
			{
				CurrentState = 8;
				lexemsIterator++;
				State_8();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ENTER");
			}
		}

		private void State_8()
		{
			if (CurrentLexemValue() == "@end")
			{
				Out.Log(Out.State.LogInfo,"Syntax Analyzer finish success");
				return;
			}
			else 
			{
				CurrentState = 11;
				ActionStack.Push(State_7);
				Out.Log(Out.State.LogInfo,"Call half-automat OPERATOR");
				HalfAutomatOperator();
			} 
		}
		#endregion

		#region H/A Operator
		private void HalfAutomatOperator()
		{
			State_11();
		}
		private void State_11()
		{
			// 11 //
			if (isCurrentLexemID())
			{
				CurrentState = 12;
				lexemsIterator++;
				State_12();
			}
			else if (CurrentLexemValue() == "input" || CurrentLexemValue() == "output")
			{
				CurrentState = 14;
				lexemsIterator++;
				State_14();
			}
			else if (CurrentLexemValue() == "if")
			{
				CurrentState = 51;
				lexemsIterator++;
				Out.Log(Out.State.LogInfo,"Call half-automat LOGICAL EXPRESSION");
				ActionStack.Push(State_17);
				HalfAutomatLogicalExpression();
			}
			else if (CurrentLexemValue() == "for")
			{
				CurrentState = 23;
				lexemsIterator++;
				State_23();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Invalid operator");
			}
		}
		#endregion
		#region H/A Operator [=]
		private void State_12()
		{
			if (CurrentLexemValue() == "=")
			{
				ActionStack.Push(State_13);
				lexemsIterator++;
				HalfAutomatExpression();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed =");
			}

		}

		private void State_13()
		{
			if (ActionStack.Last() != ActionStack.WrongLexem)
			{
				Action returnState = ActionStack.Pop();
				returnState();
			}
		}
		#endregion
		#region H/A Operator [IO]
		private void State_14()
		{
			if (CurrentLexemValue() == "(")
			{
				CurrentState = 15;
				lexemsIterator++;
				State_15();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed '('");
			}
		} 

		private void State_15()
		{
			if (isCurrentLexemID())
			{
				CurrentState = 16;
				lexemsIterator++;
				State_16();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ID");
			}
		} 

		private void State_16()
		{
			if (CurrentLexemValue() == ",")
			{
				CurrentState = 15;
				lexemsIterator++;
				State_15();
			}
			else if (CurrentLexemValue() == ")")
			{
				Action returnState = ActionStack.Pop();
				lexemsIterator++;
				returnState();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ) or ,");
			}
		} 
		#endregion
		#region H/A Operator [IF]
		private void State_17()
		{
			if (CurrentLexemValue() == "\n")
			{
				ActionStack.Push(State_18);
				lexemsIterator++;
				HalfAutomatOperatorsBlock();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ENTER");
			}
		} 

		private void State_18()
		{
			if (CurrentLexemValue() == "\n")
			{
				CurrentState = 19;
				lexemsIterator++;
				State_19();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ENTER");
			}
		} 

		private void State_19()
		{
			if (CurrentLexemValue() == "else")
			{
				CurrentState = 20;
				lexemsIterator++;
				State_20();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed else");
			}
		} 

		private void State_20()
		{
			if (CurrentLexemValue() == "\n")
			{
				ActionStack.Push(State_21);
				lexemsIterator++;
				HalfAutomatOperatorsBlock();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ENTER");
			}
		} 

		private void State_21()
		{
			if (CurrentLexemValue() == "\n")
			{
				CurrentState = 22;
				lexemsIterator++;
				State_22();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ENTER");
			}
		} 

		private void State_22()
		{
			if (CurrentLexemValue() == "endif")
			{
				Action returnState = ActionStack.Pop();
				lexemsIterator++;
				returnState();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed endif");
			}
		} 
		#endregion
		#region H/A Operator [FOR]
		private void State_23()
		{
			if (isCurrentLexemID())
			{
				CurrentState = 24;
				lexemsIterator++;
				State_24();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ID");
			}
		} 

		private void State_24()
		{
			if (CurrentLexemValue() == "=")
			{
				ActionStack.Push(State_25);
				lexemsIterator++;
				HalfAutomatExpression();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed =");
			}
		} 

		private void State_25()
		{
			if (CurrentLexemValue() == "to")
			{
				ActionStack.Push(State_26);
				lexemsIterator++;
				HalfAutomatExpression();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed to");
			}
		} 

		private void State_26()
		{
			if (CurrentLexemValue() == "step")
			{
				ActionStack.Push(State_27);
				lexemsIterator++;
				HalfAutomatExpression();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed step");
			}
		} 

		private void State_27()
		{
			if (CurrentLexemValue() == "\n")
			{
				ActionStack.Push(State_28);
				lexemsIterator++;
				HalfAutomatOperatorsBlock();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ENTER");
			}
		} 

		private void State_28()
		{
			if (CurrentLexemValue() == "\n")
			{
				CurrentState = 29;
				lexemsIterator++;
				State_29();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ENTER");
			}
		} 

		private void State_29()
		{
			if (CurrentLexemValue() == "next")
			{
				Action returnState = ActionStack.Pop();
				lexemsIterator++;
				returnState();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed next");
			}
		} 

		#endregion

		#region H/A Operators Block
		private void HalfAutomatOperatorsBlock()
		{
			State_31();
		}

		private void State_31()
		{
			if (CurrentLexemValue() == "{")
			{
				CurrentState = 32;
				lexemsIterator++;
				State_32();
			}
			else
			{
				ActionStack.Push(State_34);
				HalfAutomatOperator();
			}
		}

		private void State_32()
		{
			if (CurrentLexemValue() == "\n")
			{
				CurrentState = 33;
				lexemsIterator++;
				State_33();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ENTER");
			}
		}

		private void State_33()
		{
			if (CurrentLexemValue() == "}")
			{
				Action returnState = ActionStack.Pop();
				lexemsIterator++;
				returnState();
			}
			else
			{
				ActionStack.Push(State_32);
				HalfAutomatOperator();
			}
		}

		private void State_34()
		{
			if (ActionStack.Last() != ActionStack.WrongLexem)
			{
				Action returnState = ActionStack.Pop();
				returnState();
			}
		}
		#endregion

		#region H/A Expression
		private void HalfAutomatExpression()
		{
			State_41();
		}

		private void State_41()
		{
			if (isCurrentLexemID() || isCurrentLexemCONST())
			{
				CurrentState = 42;
				lexemsIterator++;
				State_42();
			}
			else if (CurrentLexemValue() == "(")
			{
				ActionStack.Push(State_43);
				lexemsIterator++;
				HalfAutomatExpression();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ( or ID or CONST");
			}
		}

		private void State_42()
		{
			HashSet<string> operators = new HashSet<string>()
			{
				"+", "-", "*", "/", "^"
			};

			if (operators.Contains(CurrentLexemValue()))
			{
				CurrentState = 41;
				lexemsIterator++;
				State_41();
			}
			else
			{
				Action returnState = ActionStack.Pop();
				returnState();
			}
		}

		private void State_43()
		{
			if (CurrentLexemValue() == ")")
			{
				CurrentState = 42;
				lexemsIterator++;
				State_42();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed )");
			}
		}
		#endregion

		#region H/A Logical Expression
		private void HalfAutomatLogicalExpression()
		{
			State_51();
		}

		private void State_51()
		{
			if (CurrentLexemValue() == "!")
			{
				CurrentState = 51;
				lexemsIterator++;
				State_51();
			}
			else if (CurrentLexemValue() == "[")
			{
				ActionStack.Push(State_52);
				lexemsIterator++;
				HalfAutomatLogicalExpression();
			}
			else 
			{
				ActionStack.Push(State_54);
				HalfAutomatExpression();
			}
		}

		private void State_52()
		{
			if (CurrentLexemValue() == "]")
			{
				CurrentState = 53;
				lexemsIterator++;
				State_53();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Missed ]");
			}
		}

		private void State_53()
		{
			if (CurrentLexemValue() == "or" || CurrentLexemValue() == "and")
			{
				CurrentState = 51;
				lexemsIterator++;
				State_51();
			}
			else
			{
				Action returnState = ActionStack.Pop();
				returnState();
			}
		}

		private void State_54()
		{
			HashSet<string> operators = new HashSet<string>()
			{
				">", ">=", "<", "<=", "equ", "!="
			};

			if (operators.Contains(CurrentLexemValue()))
			{
				ActionStack.Push(State_53);
				lexemsIterator++;
				HalfAutomatExpression();
			}
			else
			{
				throw new LexemException(CurrentLineNumber(),"Unknow operator "+CurrentLexemValue());
			}
		}
		#endregion
	}
}

