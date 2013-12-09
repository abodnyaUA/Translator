using System;
using System.Collections.Generic;

namespace Translators
{
	public class BottomUpTable
	{
		private Dictionary<string,int> terminals;
		public enum Connotial
		{
			NoConnotial,
			LessConnotial,
			EqualConnotial,
			GreaterConnotial
		};
		private List<List<Connotial>> table;

		private void SetupDictionary()
		{
			this.terminals = new Dictionary<string, int>();
			List<string> terms = new List<string>()
			{
				"<app>","<appName>","<list of definitions>","<definition>","<definition2>","<list of var>","<list of operators>",
				"<operator>","<operator2>","<setter>","<input>","<output>","<cycle>","<condition>","<logical expression>","<log.exp.lev1>",
				"<log.exp.lev2>","<relation>","<expression2>","<expression>","<term>","<multiplier>","<expr.response>","ID","CONST",
				"@implementation","@interface","@end","int","input","output","ENTER","for","to","step","next","if","else","endif",
				"{","}","(",")","[","]","=","equ","!=",">","<",">=","<=","!","+","-","/","*","^",",","and","or",";","#"
			};
			for (int i=0;i<terms.Count;i++)
			{
				this.terminals.Add(terms[i],i);
			}
		}

		private Grammar grammar;
		public void GenerateTableWithGrammar(Grammar grammar)
		{
			this.grammar = grammar;
			this.table = new List<List<Connotial>>();
			for (int i=0;i<terminals.Count;i++)
			{
				this.table.Add(new List<Connotial>());
				for (int j=0;j<terminals.Count-1;j++)
				{
					this.table[i].Add(Connotial.NoConnotial);
				}
				this.table[i].Add(Connotial.GreaterConnotial);
			}
			this.table.Add(new List<Connotial>());
			for (int j=0;j<terminals.Count;j++)
			{
				this.table[this.table.Count-1].Add(Connotial.LessConnotial);
			}

			RecursiveSetup("#",grammar.GrammarPairWithRootLexem("<app>")[0],"#");
			for (int i=0;i<terminals.Count;i++)
			{
				for (int j=0;j<terminals.Count;j++)
				{
					char connotial = ' ';
					switch (this.table[i][j])
					{
					case Connotial.NoConnotial: 	connotial = ' '; break;
					case Connotial.LessConnotial: 	connotial = '<'; break;
					case Connotial.GreaterConnotial:connotial = '>'; break;
					case Connotial.EqualConnotial: 	connotial = '='; break;
					}
					string connotialString = "";
					connotialString += connotial;
					connotialString += "\t";
					Out.LogOneLine(Out.State.LogDebug,connotialString);
				}
				Out.LogOneLine(Out.State.LogDebug,"\n");
			}
		}

		public void RecursiveSetup(string prevLevelPrevTerm, GrammarPair grammarPair, string prevLevelNextTerm)
		{

			string currentTerm = grammarPair.PartLexems[0];
			string nextTerm = grammarPair.PartLexems.Count > 1 ? grammarPair.PartLexems[1] : prevLevelNextTerm;
			SetConnotialBetweenTerminals(Connotial.LessConnotial,prevLevelNextTerm,currentTerm);

			if (this.grammar.GrammarPairWithRootLexem(currentTerm) != null)
			{
				List<GrammarPair> pairs = this.grammar.GrammarPairWithRootLexem(currentTerm);
				foreach (GrammarPair pair in pairs)
				{
					if (ConnotialBetweenTerminals(currentTerm,pair.RootLexem) == Connotial.NoConnotial)
					{
						SetConnotialBetweenTerminals(Connotial.LessConnotial,currentTerm,pair.RootLexem);
						RecursiveSetup(currentTerm,pair,nextTerm);
					}
				}
			}

			for (int i=1;i<grammarPair.PartLexems.Count; i++)
			{
				currentTerm = grammarPair.PartLexems[i-1];
				nextTerm = grammarPair.PartLexems[i];

				if (ConnotialBetweenTerminals(currentTerm,nextTerm) == Connotial.NoConnotial)
				{
					if (this.grammar.GrammarPairWithRootLexem(nextTerm) == null)
					{
						SetConnotialBetweenTerminals(Connotial.EqualConnotial,currentTerm,nextTerm);
					}
					else
					{
						if (i < grammarPair.PartLexems.Count-1)
						{
							SetConnotialBetweenTerminals(Connotial.LessConnotial,currentTerm,nextTerm);
							string nextNextTerm = grammarPair.PartLexems[i+1];
							List<GrammarPair> pairs = this.grammar.GrammarPairWithRootLexem(nextTerm);
							foreach (GrammarPair pair in pairs)
							{
								RecursiveSetup(currentTerm,pair,nextNextTerm);
							}
						}
						else
						{
							SetConnotialBetweenTerminals(Connotial.LessConnotial,currentTerm,nextTerm);
							List<GrammarPair> pairs = this.grammar.GrammarPairWithRootLexem(nextTerm);
							foreach (GrammarPair pair in pairs)
							{
								RecursiveSetup(currentTerm,pair,prevLevelNextTerm);
							}
						}
					}
				}
			}
			string lastTerm = grammarPair.PartLexems[grammarPair.PartLexems.Count-1];
			SetConnotialBetweenTerminals(Connotial.GreaterConnotial,lastTerm,prevLevelNextTerm);
		}

		private Connotial Cell(int row, int column)
		{
			return this.table[row][column];
		}

		public Connotial ConnotialBetweenTerminals(string leftTerminal,string rightTerminal)
		{
			int row = this.terminals[leftTerminal];
			int column = this.terminals[rightTerminal];
			return Cell(row,column);
		}

		private void SetConnotialBetweenTerminals(Connotial connotial,string leftTerminal,string rightTerminal)
		{
			int row = this.terminals[leftTerminal];
			int column = this.terminals[rightTerminal];
			this.table[row][column] = connotial;
		}

		public BottomUpTable ()
		{
			SetupDictionary();
		}
	}
}

