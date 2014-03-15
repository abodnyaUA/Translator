using System;
using System.Collections.Generic;
using System.IO;

namespace Translators
{
	public class SyntaxAnalyzerBottomUp : ISyntaxAnalyzer
	{
		private Grammar grammar = new Grammar();
		private BottomUpTable table = new BottomUpTable();
		List<string> stack = new List<string>();
		List<string> lexems;
		List<string> poliz = new List<string>();
		HTMLTable htmlTable;

		private CompileMode AnalyzeMode { 
			get { return Compiler.sharedCompiler.AnalyzeMode; } set {} }

		private SyntaxAnalyzerBottomUp ()
		{
			table.GenerateTableWithGrammar(this.grammar);
		}
		private static SyntaxAnalyzerBottomUp _sharedAnalyzer = null;
		public static SyntaxAnalyzerBottomUp sharedAnalyzer
		{
			get
			{
				if (_sharedAnalyzer == null) _sharedAnalyzer = new SyntaxAnalyzerBottomUp();
				return _sharedAnalyzer;
			}
		}

		public void PrintTable()
		{
			this.table.PrintTable();
		}

		public void AnalyzeLexems()
		{
			List<Lexem> lexemsFull = LexemList.Instance.Lexems;
			lexems = new List<string>();
			lexems.Add("#");
			foreach (Lexem lexem in lexemsFull)
			{
				if (lexem.isID())
				{
					lexems.Add("ID_"+lexem.command);
				}
				else if (lexem.isCONST())
				{
					lexems.Add("CONST_"+lexem.command);
				}
				else if (lexem.command == "\n")
				{
					lexems.Add("ENTER");
				}
				else
				{
					lexems.Add(lexem.command);	
				}
			}
			lexems.Add("#");
			Analyze();
		}


		/* BAD. Very bad code. Absolute shity */
		HashSet<string> lowPriorityItems = new HashSet<string>() {"<list of definitions2>","<list of operators2>"};
		HashSet<string> globalItems = new HashSet<string>() {"<definition2>","<definition>","<operator2>","<operator>",
			"<setter>","<input>","<output>","<cycle>","<condition>","if","for"};
		Dictionary<string,string> globalSections = new Dictionary<string, string>()
		{ {"@interface","@implementation"},{"@implementation","@end"},{"if","endif"},{"for","next"} };
		bool allowLowPriorityForItemAtIndex(int idx)
		{
			if (AnalyzeMode == CompileMode.PolizConverter) return true;
			HashSet<string> globalSectionsBegins = new HashSet<string>(globalSections.Keys);
			int beginBlock = int.MaxValue;

			List<string> allElements = new List<string>(stack);
			for (int i=0;i<lexems.Count;i++) allElements.Add(lexems[i]);

			for (int i=idx;beginBlock == int.MaxValue;i--)
			{
				if (globalSectionsBegins.Contains(allElements[i]))
				{
					beginBlock = i;
				}
			}

			string endValue = globalSections[allElements[beginBlock]];
			int endBlock = int.MaxValue;
			for (int i=idx;endBlock == int.MaxValue;i++)
			{
				if (allElements[i] == endValue)
				{
					endBlock = i;
				}
			}

			int globalItemsCount = 0;
			for (int i=beginBlock+1;i<endBlock;i++)
			{
				if (globalItems.Contains(allElements[i]))
				{
					globalItemsCount++;
				}
			}
			
			return globalItemsCount == 0;
		}

		private bool readyToReplace(string lexemOrigin, int begin)
		{
			if (lexemOrigin == "<expression2>")
			{
				HashSet<string> connotials = new HashSet<string>()
				{ ">","<",">=","<=","equ","!=" };
				bool containsPrevious = begin > 0 ? connotials.Contains(stack[begin-1]) : false;
				bool containsNext = begin < stack.Count-1 ? connotials.Contains(stack[begin+1]) : false;
				return !(containsNext || containsPrevious || connotials.Contains(lexems[0]));
			}
			return true;
		}

		string processValue(string element)
		{
			string processed = element;
			if (processed.StartsWith("ID")) processed = "ID";
			if (processed.StartsWith("CONST")) processed = "CONST";
			return processed;
		}

		private void PutToStack()
		{
			stack.Add(lexems[0]);
			lexems.RemoveAt(0);
			string element = stack[stack.Count-1];
			if (element.StartsWith("ID") || element.StartsWith("CONST"))
			{
				poliz.Add(element);
			}
		}

		private void Analyze()
		{
			/* So-so code */
			int failedProcessCount = 0;
			int LastLexemsCount = lexems.Count;
			stack.Clear();
			poliz.Clear();
			PutToStack();
			if (AnalyzeMode == CompileMode.NormalAnalyze)
			{
				htmlTable = new HTMLTable("Stack","Connotial","Source Code");
			}
			else
			{
				htmlTable = new HTMLTable("Stack","Connotial","Source Code","Poliz");
			}
			LogToTable(stack,Translators.BottomUpTable.Connotial.NoConnotial,lexems);

			bool successFinish = false;
			do
			{
				Translators.BottomUpTable.Connotial connotial = 
					table.ConnotialBetweenTerminals(processValue(stack[stack.Count-1]),processValue(lexems[0]));
				if (connotial == BottomUpTable.Connotial.EqualConnotial 
				    || connotial == BottomUpTable.Connotial.LessConnotial
				    || connotial == BottomUpTable.Connotial.NoConnotial)
				{
					PutToStack();
				}
				if (connotial == BottomUpTable.Connotial.GreaterConnotial)
				{
					int openScobeIdx = int.MaxValue;
					for (int i=stack.Count-2;i>=0 && openScobeIdx == int.MaxValue;i--)
					{
						if (table.ConnotialBetweenTerminals(processValue(stack[i]),processValue(stack[i+1])) == 
						    BottomUpTable.Connotial.LessConnotial)
						{
							openScobeIdx = i+1;
						}
					}
					GrammarPair pair = grammarPairWithLexemList(openScobeIdx,stack.Count-1);
					if (pair != null)
					{
						if (lowPriorityItems.Contains(pair.RootLexem) && 
						    false == allowLowPriorityForItemAtIndex(stack.Count-1))
						{
							PutToStack();
							Out.Log(Out.State.LogDebug,"It's not time for low-level pair");
						}
						else
						{
							if (pair.PartLexems.Count > 1 || readyToReplace(pair.PartLexems[0],openScobeIdx))
							{
								replace(openScobeIdx,pair);
								failedProcessCount = 0;
							}
							else
							{
								PutToStack();
							}
						}
					}
					else
					{
						Out.Log(Out.State.LogInfo,"Invalid pair");
						return;
					}
				}

				LogToTable(stack,connotial,lexems);

				if (LastLexemsCount == lexems.Count)
				{
					failedProcessCount++;
				}
				else
				{
					failedProcessCount = 0;
				}
				LastLexemsCount = lexems.Count;

				if (AnalyzeMode == CompileMode.NormalAnalyze)
				{
					successFinish = stack[1] == "<app>" && stack.Count == 2;
				}
				else
				{
					successFinish = stack[1] == "<expression3>" && stack.Count == 2;
				}

			} while (!successFinish && failedProcessCount != 20);
			htmlTable.WriteToFile(Constants.HTMLTablePath);

		}

		void replace(int idx, GrammarPair grammarpair)
		{
			stack.RemoveRange(idx,grammarpair.PartLexems.Count);
			stack.Insert(idx,grammarpair.RootLexem);
			string original = "";
			foreach (string str in grammarpair.PartLexems)
			{
				original += str + " ";
				if (PolizAnalyzer.sharedAnalyzer.operationList.isOperation(str))
				{
					poliz.Add(str);
				}
			}
			Out.Log(Out.State.LogInfo,"Replace "+original + " with " +grammarpair.RootLexem);
		}

		GrammarPair grammarPairWithLexemList(int startIdx,int endIdx)
		{
			GrammarPair result = null;
			int length = endIdx-startIdx+1;
			
			try  { 
				foreach (GrammarPair pair in grammar.Gramatic)
				{
					if (length == pair.PartLexems.Count)
					{
						bool exist = true;
						for (int i=0;i<pair.PartLexems.Count;i++)
						{
							if (processValue(stack[i+startIdx]) != pair.PartLexems[i])
							{
								exist = false;
							}
						}
						if (exist)
						{
							result = pair;
							return result;
						}
					}
				}
			}catch(Exception) 
			{
				Console.WriteLine("SHIT!");
			};
			return result;
		}

		string ListToString(List<string> strings)
		{
			string str = "";
			for (int i=0;i<strings.Count;i++) 
			{
				str += strings[i].Replace("CONST_","").Replace("ID_","")+" ";
			}
			return str;
		}

		void LogToTable(List<string> stack, BottomUpTable.Connotial connotial, List<string> source)
		{
			// Log output //
			Out.Log(Out.State.LogInfo,"=========Dumb LOG:============");
			Out.Log(Out.State.LogInfo,"Stack: ");
			for (int i = 0; i< stack.Count-1; i++)
			{
				BottomUpTable.Connotial connotial2 = 
					table.ConnotialBetweenTerminals(processValue(stack[i]),processValue(stack[i+1]));
				string ConnotialString = table.ConnotialToString(connotial2);
				Out.Log(Out.State.LogDebug, ConnotialString + stack[i] + " ");
			}
			Out.Log(Out.State.LogDebug, table.ConnotialToString(connotial)+stack[stack.Count-1]);
			Out.Log(Out.State.LogInfo,"Connotial: "+table.ConnotialToString(connotial));
			Out.Log(Out.State.LogInfo,"Source Code: ");
			for (int i = 0; i< lexems.Count -1; i++)
			{
				BottomUpTable.Connotial connotial2 = 
					table.ConnotialBetweenTerminals(processValue(lexems[i]),processValue(lexems[i+1]));
				string ConnotialString = table.ConnotialToString(connotial2);
				Out.Log(Out.State.LogDebug, ConnotialString + lexems[i] + " ");
			}
			Out.Log(Out.State.LogDebug, "[ ]" +lexems[lexems.Count-1]);

			// HTML output //
			string stackLine = ListToString(stack);
			string connotialLine = table.ConnotialToString(connotial);
			string lexemsLine = ListToString(lexems);
			if (AnalyzeMode == CompileMode.PolizConverter)
			{
				string polizLine = ListToString(poliz);
				htmlTable.AddLine(stackLine,connotialLine,lexemsLine,polizLine);
			}
			else
			{
				htmlTable.AddLine(stackLine,connotialLine,lexemsLine);
			}
		}

	}
}

