using System;
using System.Collections.Generic;

namespace Translators
{
	public class SyntaxAnalyzerBottomUp
	{
		private Grammar grammar = new Grammar();
		private BottomUpTable table = new BottomUpTable();
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

		public void  PrintTable()
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
			"<setter>","<input>","<output>","<cycle>","<condition>","if"};
		Dictionary<string,string> globalSections = new Dictionary<string, string>()
		{ {"@interface","@implementation"},{"@implementation","@end"},{"if","endif"},{"for","next"} };
		bool allowLowPriorityForItemAtIndex(int idx)
		{
			HashSet<string> globalSectionsBegins = new HashSet<string>(globalSections.Keys);
			int beginBlock = int.MaxValue;
			for (int i=idx;beginBlock == int.MaxValue;i--)
			{
				if (globalSectionsBegins.Contains(lexems[i]))
				{
					beginBlock = i;
				}
			}

			string endValue = globalSections[lexems[beginBlock]];
			int endBlock = int.MaxValue;
			for (int i=idx;endBlock == int.MaxValue;i++)
			{
				if (lexems[i] == endValue)
				{
					endBlock = i;
				}
			}

			int globalItemsCount = 0;
			for (int i=beginBlock+1;i<endBlock;i++)
			{
				if (globalItems.Contains(lexems[i]))
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
				string previous = lexems[begin-1];
				string next = lexems[begin+1];
				return !(connotials.Contains(previous) || connotials.Contains(next));
			}
			return true;
		}

		private void Analyze()
		{
			/* So-so code */
			int failedProcessCount = 0;
			int LastLexemsCount = lexems.Count;
			int openscobeIdx = int.MaxValue;
			do
			{
				openscobeIdx = int.MaxValue;

				for (int i=0;i<lexems.Count-1;i++)
				{
					string current = lexems[i];
					string next = lexems[i+1];
					Translators.BottomUpTable.Connotial connotial = table.ConnotialBetweenTerminals(current,next);
					if (connotial == BottomUpTable.Connotial.LessConnotial)
					{
						openscobeIdx = i+1;
					}

					if (connotial == BottomUpTable.Connotial.GreaterConnotial)
					{
						int begin = openscobeIdx == int.MaxValue ? i : openscobeIdx;
						GrammarPair pair = grammarPairWithLexemList(begin,i);
						if (pair != null)
						{
							if (lowPriorityItems.Contains(pair.RootLexem) && false == allowLowPriorityForItemAtIndex(i))
							{
								Out.Log(Out.State.LogDebug,"It's not time for low-level pair");
							}
							else
							{
								if (pair.PartLexems.Count > 1 || readyToReplace(pair.PartLexems[0],begin))
								{
									replace(openscobeIdx,pair);
								}
							}
						}
						//break;
					}
				}

				Out.Log(Out.State.LogInfo,"=========Dumb LOG:============");
				for (int i = 0; i< lexems.Count -1; i++)
				{
					BottomUpTable.Connotial connotial = table.ConnotialBetweenTerminals(lexems[i],lexems[i+1]);
					string ConnotialString = table.ConnotialToString(connotial);
					Out.Log(Out.State.LogDebug, ConnotialString + lexems[i] + " ");
				}

				if (LastLexemsCount == lexems.Count)
				{
					failedProcessCount++;
				}
				else
				{
					failedProcessCount = 0;
				}
				LastLexemsCount = lexems.Count;
			} while (lexems.Count > 3 && failedProcessCount != 20);
		}

		void replace(int idx, GrammarPair grammarpair)
		{
			lexems.RemoveRange(idx,grammarpair.PartLexems.Count);
			lexems.Insert(idx,grammarpair.RootLexem);
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
			foreach (GrammarPair pair in grammar.Gramatic)
			{
				if (length == pair.PartLexems.Count)
				{
					bool exist = true;
					for (int i=0;i<pair.PartLexems.Count;i++)
					{
						if (lexems[i+startIdx] != pair.PartLexems[i])
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
			return result;
		}

	}
}

