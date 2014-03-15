using System;
using System.Collections.Generic;

namespace Translators
{
	
	public class Lexem
	{
		public int lineNumber;
		public string command;
		public int key;
		public Lexem(int line, string command, int key)
		{
			this.key = key;
			this.lineNumber = line;
			this.command = command;
		}
		
		public bool isIDorCONST()
		{
			return this.key >= LexemList.Instance.Grammar.Count-2;
		}
		
		public bool isID()
		{
			return this.key == LexemList.Instance.Grammar.Count-2;
		}
		
		public bool isCONST()
		{
			return this.key == LexemList.Instance.Grammar.Count-1;
		}
	}

	public class LexemList
	{
		public LexemList()
		{
			CreateLexemsGrammar();
		}

		public static LexemList Instance 
		{ get { return Compiler.sharedCompiler.LexemList; } set { } }


		public List<string> Grammar { get { return grammar; } }
		public List<string> IDs { get { return ids; } }
		public List<string> Consts { get { return consts; } }
		public List<Lexem>  Lexems { get { return lexems; } }

		public void UpdateLexems(List<Lexem> lexems) 
		{ this.lexems = lexems; }
		public void UpdateIDs(List<string> IDs) 
		{ this.ids = IDs; }
		public void UpdateConsts(List<string> consts) 
		{ this.consts = consts; }

		private List<string> grammar;
		private List<string> ids = new List<string>();
		private List<string> consts = new List<string>();
		private List<Lexem>  lexems = new List<Lexem>();
		private void CreateLexemsGrammar()
		{
			this.grammar = new List<string>();
			grammar.Add("@interface");
			grammar.Add("@implementation");
			grammar.Add("@end");
			grammar.Add("int");
			grammar.Add("input");
			grammar.Add("output");
			grammar.Add("\n");
			grammar.Add("for");
			grammar.Add("to");
			grammar.Add("step");
			grammar.Add("next");
			grammar.Add("if");
			grammar.Add("else");
			grammar.Add("endif");
			grammar.Add("{");
			grammar.Add("}");
			grammar.Add("(");
			grammar.Add(")");
			grammar.Add("=");
			grammar.Add("equ");
			grammar.Add("!=");
			grammar.Add(">");
			grammar.Add("<");
			grammar.Add(">=");
			grammar.Add("<=");
			grammar.Add("!");
			grammar.Add("+");
			grammar.Add("-");
			grammar.Add("/");
			grammar.Add("*");
			grammar.Add("^");
			grammar.Add(",");
			grammar.Add("and");
			grammar.Add("or");
			grammar.Add("[");
			grammar.Add("]");
			grammar.Add(";");
			grammar.Add("endset");
			grammar.Add("from");
			grammar.Add("var");
			grammar.Add("const");
		}
	}
}

