using System;
using System.Collections.Generic;

namespace Translators
{
	public class GrammarPair
	{
		public GrammarPair(string RootLexem,List<string> PartLexems)
		{
			this.RootLexem = RootLexem;
			this.PartLexems = PartLexems;
		}
		public string RootLexem;
		public List<string> PartLexems;
	}

	public class Grammar
	{
		private List<GrammarPair> grammar;
		public List<GrammarPair> Gramatic
		{
			get { return grammar; }
		}
		public Grammar()
		{
			this.grammar = new List<GrammarPair>()
			{
				new GrammarPair("<app>", 
					new List<string>() {"@interface","<appName>","ENTER","<list of definitions2>","ENTER",
					"@implementation","ENTER","<list of operators2>","ENTER","@end"}),
				new GrammarPair("<appName>", 
					new List<string>() {"ID",";"}),
				new GrammarPair("<list of definitions2>", 
					new List<string>() {"<list of definitions>"}),
				new GrammarPair("<list of definitions>", 
					new List<string>() {"ENTER","<definition2>"}),
				new GrammarPair("<list of definitions>", 
					new List<string>() {"<list of definitions>","ENTER","<definition2>"}),
				new GrammarPair("<definition2>", 
					new List<string>() {"<definition>"}),
				new GrammarPair("<definition>", 
					new List<string>() {"int","<list of var>"}),
				new GrammarPair("<list of var>", 
					new List<string>() {",","ID"}),
				new GrammarPair("<list of var>", 
					new List<string>() {",","ID","<list of var>"}),
				new GrammarPair("<list of operators2>", 
				    new List<string>() {"<list of operators>"}),
				new GrammarPair("<list of operators>", 
					new List<string>() {"ENTER","<operator2>"}),
				new GrammarPair("<list of operators>", 
					new List<string>() {"<list of operators>","ENTER","<operator2>"}),
				new GrammarPair("<operator2>", 
					new List<string>() {"<operator>"}),
				new GrammarPair("<operator>", 
					new List<string>() {"<output>"}),
				new GrammarPair("<operator>", 
					new List<string>() {"<setter>"}),
				new GrammarPair("<operator>", 
					new List<string>() {"<condition>"}),
				new GrammarPair("<operator>", 
					new List<string>() {"<cycle>"}),
				new GrammarPair("<operator>", 
					new List<string>() {"<input>"}),
				new GrammarPair("<setter>", 
					new List<string>() {"ID","=","<expression>"}),
				new GrammarPair("<input>", 
					new List<string>() {"input","(","<list of var>",")"}),
				new GrammarPair("<output>", 
					new List<string>() {"output","(","<list of var>",")"}),
				new GrammarPair("<cycle>", 
					new List<string>() {"for","ID","=","<expression>","to","<expression>","step","<expression2>","ENTER","<list of operators>","ENTER","next"}),
				new GrammarPair("<expression2>", 
					new List<string>() {"<expression>"}),
				new GrammarPair("<condition>", 
					new List<string>() {"if","<logical expression>","ENTER","<list of operators>","ENTER","else","ENTER","<list of operators>","ENTER","endif"}),
				new GrammarPair("<logical expression>", 
					new List<string>() {"<log.exp.lev1>"}),
				new GrammarPair("<logical expression>", 
					new List<string>() {"<logical expression>","or","<log.exp.lev1>"}),
				new GrammarPair("<log.exp.lev1>", 
					new List<string>() {"<log.exp.lev2>"}),
				new GrammarPair("<log.exp.lev1>", 
					new List<string>() {"<log.exp.lev1>","and","<log.exp.lev2>"}),
				new GrammarPair("<log.exp.lev2>", 
					new List<string>() {"!","<log.exp.lev2>"}),
				new GrammarPair("<log.exp.lev2>", 
					new List<string>() {"<relation>"}),
				new GrammarPair("<log.exp.lev2>", 
					new List<string>() {"[","<logical expression>","]"}),
				new GrammarPair("<relation>", 
					new List<string>() {"<expression>",">","<expression>"}),
				new GrammarPair("<relation>", 
					new List<string>() {"<expression>","<","<expression>"}),
				new GrammarPair("<relation>", 
					new List<string>() {"<expression>","equ","<expression>"}),
				new GrammarPair("<relation>", 
					new List<string>() {"<expression>","!=","<expression>"}),
				new GrammarPair("<relation>", 
					new List<string>() {"<expression>",">=","<expression>"}),
				new GrammarPair("<relation>", 
					new List<string>() {"<expression>","<=","<expression>"}),
				new GrammarPair("<expression>", 
					new List<string>() {"<term>"}),
				new GrammarPair("<expression>", 
					new List<string>() {"-","<term>"}),
				new GrammarPair("<expression>", 
					new List<string>() {"<expression>","+","<term>"}),
				new GrammarPair("<expression>", 
					new List<string>() {"<expression>","-","<term>"}),
				new GrammarPair("<term>", 
					new List<string>() {"<multiplier>"}),
				new GrammarPair("<term>", 
					new List<string>() {"<term>","*","<multiplier>"}),
				new GrammarPair("<term>", 
					new List<string>() {"<term>","/","<multiplier>"}),
				new GrammarPair("<multiplier>", 
					new List<string>() {"<expr.response>"}),
				new GrammarPair("<multiplier>", 
					new List<string>() {"<expr.response>","^","<multiplier>"}),
				new GrammarPair("<expr.response>", 
					new List<string>() {"ID"}),
				new GrammarPair("<expr.response>", 
					new List<string>() {"CONST"}),
				new GrammarPair("<expr.response>", 
					new List<string>() {"(","<expression>",")"})
			};
		}

		public List<GrammarPair> GrammarPairWithRootLexem(string rootLexem)
		{
			List<GrammarPair> pairs = new List<GrammarPair>();
			foreach (GrammarPair pair in this.grammar)
			{
				if (pair.RootLexem == rootLexem)
				{
					pairs.Add(pair);
				}
			}
			return pairs.Count > 0 ? pairs : null;
		}
	}
}

