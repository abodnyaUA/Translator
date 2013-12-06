using System;
using System.Collections.Generic;

namespace Translators.Lab01
{
	public class SyntaxAnalyzerWithTable
	{
		private static SyntaxAnalyzerWithTable _sharedAnalyzer = null;
		public static SyntaxAnalyzerWithTable sharedAnalyzer
		{
			get
			{
				if (_sharedAnalyzer == null) _sharedAnalyzer = new SyntaxAnalyzerWithTable();
				return _sharedAnalyzer;
			}
		}


		public Stack<int> stack = new Stack<int>();
		private AutomatTable table = new AutomatTable();

		private SyntaxAnalyzerWithTable() 
		{
			FIllTable();
		}

		private void FIllTable()
		{

		}		

		public void ProcessLexemOnState(Lexem lexem, ref int lexemIterator, ref int stateNumber)
		{
			State state = table.StateWithNumber(stateNumber);
			state.Run(lexem,ref stateNumber, ref lexemIterator);
		}

		public void Run(List<Lexem> lexems)
		{
			int lexemsIterator = 0;
			int currentState = 1;
			while (lexemsIterator < lexems.Count)
			{
				ProcessLexemOnState(lexems[lexemsIterator],ref lexemsIterator,ref currentState);
				Out.Log(Out.State.LogVerbose,"On state "+currentState+". Processed lexem: "+lexems[lexemsIterator].command);
				Out.Log(Out.State.LogInfo,"Processed "+lexemsIterator+" of "+lexems.Count+" lexems");
			}
		}

	}
}

