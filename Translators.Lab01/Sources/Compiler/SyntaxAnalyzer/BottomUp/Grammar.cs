using System;
using System.Collections.Generic;

namespace Translators.Lab01
{
	public class Grammar
	{
		private class GrammarPair
		{
			public GrammarPair(string RootLexem,List<string> PartLexems)
			{
				this.RootLexem = RootLexem;
				this.PartLexems = PartLexems;
			}
			public string RootLexem;
			public List<string> PartLexems;
		}

		private List<GrammarPair> grammar;
		public Grammar ()
		{
			this.grammar = new List<GrammarPair>()
			{
				new GrammarPair("<app>", new List<string>() {"@interface","<appName>","ENTER","<list of definition>","ENTER",
					"@implementation","ENTER","<list of operators>","ENTER","@end"}),
				new GrammarPair("<appName>", new List<string>() {"ID"}),
				new GrammarPair("<list of definitions>", new List<string>() {"<definition>"}),
				new GrammarPair("<list of definitions>", new List<string>() {"<list of definitions>","ENTER","<definition2>"}),
				new GrammarPair("<definition2>", new List<string>() {"<definition>"}),
				new GrammarPair("<definition>", new List<string>() {"int","<list of var>"}),
				new GrammarPair("<list of var>", new List<string>() {"ID"}),
				new GrammarPair("<list of var>", new List<string>() {"ID",",","<list of var>"}),
				new GrammarPair("<list of operators>", new List<string>() {"<operator>"}),
				new GrammarPair("<list of operators>", new List<string>() {"<list of operators>","ENTER","<operator2>"}),
				new GrammarPair("<operator2>", new List<string>() {"<operator>"}),
				new GrammarPair("<action>", new List<string>() {"<operator>"}),
				new GrammarPair("<action>", new List<string>() {"{","ENTER","<list of operators>","ENTER","}"})
			};
		}
	}
}

