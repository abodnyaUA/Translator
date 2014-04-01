using System;
using System.Collections.Generic;

namespace Translators
{
	public class Lexem
	{
		public static int kIDKey { get { return LexemList.Instance.Grammar.Count-2; } }
		public static int kConstKey { get { return LexemList.Instance.Grammar.Count-1; } }

		public Lexem(int line, string command, int key)
		{
			this.key = key;
			this.lineNumber = line;
			this.command = command;
		}

		public int LineNumber { get { return lineNumber; } }
		public string Command { get { return command; } }
		public int Key { get { return key; } }
		
		private int lineNumber;
		private string command;
		private int key;
		
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
		public bool isSeparator()
		{
			return this.command == "\n";
		}
		 
		public int Value 
		{
			set 
			{
				if (this.isID())
				{
					Variable variable = LexemList.Instance.VariableWithName(this.command);
					variable.Value = value;
				}
				else if (this.isCONST())
				{
					this.command = value.ToString();
				}
				else
				{
					throw new Exception("Try to set value to non-ID");
				}
			}

			get
			{
				if (this.isID())
				{
					Variable variable = LexemList.Instance.VariableWithName(this.command);
					return variable.Value;
				}
				else if (this.isCONST())
				{
					if (this.command[0] != '"')
					{
						return Convert.ToInt32(this.command);
					}
					else
					{
						throw new LexemException(this.LineNumber,"Can't set string as value of variable");
					}
				}
				else
				{
					return int.MaxValue;
				}
			}
		}
	}

	public class Variable
	{
		private static int iterator = 0;
		private int adress = 0;
		private string name = "";

		public int Value = 0;

		public int Adress { get { return adress; } }
		public string Name { get { return name; } }
		public Variable(string name)
		{
			this.name = name;
			this.adress = Variable.iterator;
			Variable.iterator++;
		}
	}

	public class LexemList
	{
		public LexemList()
		{
			CreateLexemsGrammar();
		}

		public static LexemList Instance 
		{ get { return Compiler.sharedCompiler.LexemList; } }

		public List<string> Grammar { get { return grammar; } }
		public HashSet<Variable> IDs { get { return ids; } }
		public List<string> Consts { get { return consts; } }
		public List<Lexem>  Lexems { get { return lexems; } }

		public void UpdateLexems(List<Lexem> lexems) 
		{ 
			for (int i = 0; i < lexems.Count - 1; i++)
			{
				if (lexems[i].isSeparator() && lexems[i+1].isSeparator())
				{
					lexems.RemoveAt(i);
					i--;
				}
			}
			this.lexems = lexems; 
		}
		public void UpdateIDs(List<string> IDs) 
		{
			this.ids = new HashSet<Variable>();
			IDs.RemoveAt(0); // Remove AppName
			foreach (string id in IDs)
			{
				this.ids.Add(new Variable(id));
			}

			// For cycles //
			this.ids.Add(new Variable("$r1"));
			this.ids.Add(new Variable("$r2"));
		}

		public void UpdateConsts(List<string> consts) 
		{ 
			this.consts = consts; 
		}

		public Variable VariableWithName(string name)
		{
			foreach (Variable id in this.IDs)
			{
				if (id.Name == name)
				{
					return id;
				}
			}
			return null;
		}

		private List<string> grammar;
		private HashSet<Variable> ids = new HashSet<Variable>();
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
			grammar.Add("do");
			grammar.Add("next");
			grammar.Add("if");
			grammar.Add("then");
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
			grammar.Add("%");
			grammar.Add("*");
			grammar.Add("^");
			grammar.Add("root");
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

		public int KeyForOperator(string oper)
		{
			int index = this.grammar.FindIndex((string op) => {
				return op == oper;
			});
			return index;
		}

		public void PrintIDs()
		{
			Out.Log(Translators.Out.State.LogInfo,"Variable trace:");
			foreach (Variable variable in this.ids)
			{
				if (variable.Name != "r1" && variable.Name != "r2")
				{
					Out.Log(Translators.Out.State.LogInfo,"ID: "+variable.Name+
					        "; Adress: "+variable.Adress+"; Value: "+variable.Value);
				}
			}
		}
	}
}

