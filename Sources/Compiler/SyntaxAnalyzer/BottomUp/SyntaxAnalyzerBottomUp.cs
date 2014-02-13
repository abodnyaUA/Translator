using System;
using System.Collections.Generic;
using System.IO;

namespace Translators
{
	public class SyntaxAnalyzerBottomUp
	{
		private Grammar grammar = new Grammar();
		private BottomUpTable table = new BottomUpTable();
		List<string> stack = new List<string>();
		string htmlTable = "<html>\n<head></head>\n" +
			"<body>\n<table border=\"1\">\n<tr><td>Stack</td><td>Connotial</td><td>Source Code</td></tr>";
		private SyntaxAnalyzerBottomUp ()
		{
			table.GenerateTableWithGrammar(this.grammar);
			;
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
		private List<string> lexems;
		public void AnalyzeLexems()
		{
			List<Lexem> lexemsFull = LexemAnalyzer.sharedAnalyzer.Lexems;
			lexems = new List<string>();
			lexems.Add("#");
			foreach (Lexem lexem in lexemsFull)
			{
				if (lexem.key == LexemAnalyzer.sharedAnalyzer.dict.Count-2)
				{
					lexems.Add("ID");
				}
				else if (lexem.key == LexemAnalyzer.sharedAnalyzer.dict.Count-1)
				{
					lexems.Add("CONST");
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

		private void Analyze()
		{
			/* So-so code */
			int failedProcessCount = 0;
			int LastLexemsCount = lexems.Count;
			stack.Clear();
			lexems.RemoveAt(0);
			stack.Add("#");
			LogToTable(stack,Translators.BottomUpTable.Connotial.NoConnotial,lexems);
			do
			{
				Translators.BottomUpTable.Connotial connotial = 
					table.ConnotialBetweenTerminals(stack[stack.Count-1],lexems[0]);
				if (connotial == BottomUpTable.Connotial.EqualConnotial 
				    || connotial == BottomUpTable.Connotial.LessConnotial
				    || connotial == BottomUpTable.Connotial.NoConnotial)
				{
					stack.Add(lexems[0]);
					lexems.RemoveAt(0);
				}
				if (connotial == BottomUpTable.Connotial.GreaterConnotial)
				{
					int openScobeIdx = int.MaxValue;
					for (int i=stack.Count-2;i>=0 && openScobeIdx == int.MaxValue;i--)
					{
						if (table.ConnotialBetweenTerminals(stack[i],stack[i+1]) == 
						    BottomUpTable.Connotial.LessConnotial)
						{
							openScobeIdx = i+1;
						}
					}
					GrammarPair pair = grammarPairWithLexemList(openScobeIdx,stack.Count-1);
					if (pair != null)
					{
						if (lowPriorityItems.Contains(pair.RootLexem) && false == allowLowPriorityForItemAtIndex(stack.Count-1))
						{
							stack.Add(lexems[0]);
							lexems.RemoveAt(0);
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
								stack.Add(lexems[0]);
								lexems.RemoveAt(0);
							}
						}
					}
				}

				LogToTable(stack,connotial,lexems);
				Out.Log(Out.State.LogInfo,"=========Dumb LOG:============");
				Out.Log(Out.State.LogInfo,"Stack: ");
				for (int i = 0; i< stack.Count; i++)
				{
					Out.Log(Out.State.LogDebug, stack[i] + " ");
				}
				Out.Log(Out.State.LogInfo,"Connotial: "+table.ConnotialToString(connotial));
				for (int i = 0; i< lexems.Count -1; i++)
				{
					BottomUpTable.Connotial connotial2 = table.ConnotialBetweenTerminals(lexems[i],lexems[i+1]);
					string ConnotialString = table.ConnotialToString(connotial2);
					Out.Log(Out.State.LogDebug, ConnotialString + lexems[i] + " ");
				}
				Out.Log(Out.State.LogDebug, "[ ]" +lexems[lexems.Count-1]);

				if (LastLexemsCount == lexems.Count)
				{
					failedProcessCount++;
				}
				else
				{
					failedProcessCount = 0;
				}
				LastLexemsCount = lexems.Count;
			} while (stack[1] != "<app>" && failedProcessCount != 20);
			htmlTable += "</table></body></html>";
			File.WriteAllLines("/home/abodnya/TranslatorOutput.html",new string[] { htmlTable } );
		}

		void replace(int idx, GrammarPair grammarpair)
		{
			stack.RemoveRange(idx,grammarpair.PartLexems.Count);
			stack.Insert(idx,grammarpair.RootLexem);
			string original = "";
			foreach (string str in grammarpair.PartLexems)
			{
				original += str + " ";
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
						if (stack[i+startIdx] != pair.PartLexems[i])
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
			}catch(Exception e) 
			{
				Console.WriteLine("SHIT!");
			};
			return result;
		}

		void LogToTable(List<string> stack, BottomUpTable.Connotial connotial, List<string> source)
		{
			string content = "<tr><td>";
			for (int i=0;i<stack.Count;i++) 
			{
				content += stack[i].Replace("<","&lt;").Replace(">","&gt;")+" ";
			}
			content += "</td><td>" + table.ConnotialToString(connotial) + "</td><td>";
			for (int i=0;i<lexems.Count;i++) 
			{
				content += lexems[i].Replace("<","&lt;").Replace(">","&gt;")+" ";
			}
			content += "</td></tr>";
			htmlTable += content;
		}

	}
}

