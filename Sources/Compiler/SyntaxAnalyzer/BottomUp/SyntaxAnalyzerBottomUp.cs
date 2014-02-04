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
		HashSet<string> globalItems = new HashSet<string>() {"<definition2>","<definition>","<operator2>","<operator>"};
		bool allowLowPriority()
		{
			int globalItemsCount = 0;
			for (int i=0;i<lexems.Count-1;i++)
			{
				if (globalItems.Contains(lexems[i]))
				{
					globalItemsCount++;
				}
			}
			return globalItemsCount == 0;
		}

		private void Analyze()
		{
			/* So-so code */
			bool replacable = false;
			int openscobeIdx = int.MaxValue;
			do
			{
				replacable = false;
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
						GrammarPair pair = grammarPairWithLexemList(
							openscobeIdx == int.MaxValue ? i : openscobeIdx,i);
						if (pair != null)
						{
							if (lowPriorityItems.Contains(pair.RootLexem) && false == allowLowPriority())
							{
								Out.Log(Out.State.LogInfo,"It's not time for low-level pair");
							}
							else
							{
								replace(openscobeIdx,pair);
							}
						}
						replacable = true;
						//break;
					}
				}

			} while (lexems.Count > 3);
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

