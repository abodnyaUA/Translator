using System;
using System.Collections.Generic;

namespace Translators.Lab01
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
		private List<string> table;

		private void SetupDictionary()
		{
			this.terminals = new Dictionary<string, int>();
			List<string> terms = new List<string>()
			{
				"<app>","<appName>","<list defs>","<definition>","<definition2>","<list of var>","<list ops>","<action>",
				"<operator>","<operator2>","<setter>","<input>","<output>","<cycle>","<condition>","<log.expr.>","<log.exp.l1>",
				"<log.exp.l2>","<relation>","<express.2>","<express.>","<term>","<multiplier>","<expr.resp.>","ID","CONST",
				"@implement.","@interface","@end","int","input","output","ENTER","for","to","step","next","if","else","endif",
				"{","}","(",")","[","]","=","equ","!=",">","<",">=","<=","!","+","-","/","*","^",",","and","or"
			};
			for (int i=0;i<terms.Count;i++)
			{
				this.terminals.Add(terms[i],i);
			}
		}

		private void SetupTable()
		{
			this.table = new List<string>()
			{
				"                                                              >",
				"                                =                             >",
				"                                =                             >",
				"                                >                             >",
				"                                >                             >",
				"                                >                             >",
				"                                =                             >",
				"                                =                             >",
				"                                >                             >",
				"                                >                             >",
				"                                >                             >",
				"                                >                             >",
				"                                >                             >",
				"                                >                             >",
				"                                >                             >",
				"                                =            =               =>",
				"                                >                           = >",
				"                                >                             >",
				"                                >                             >",
				"                                =                             >",
				"                                > ==       =   ====== ==      >",
				"                                >                       ==    >",
				"                                >                         =   >",
				"                                >                             >",
				"                                >             =            =  >",
				"                                >                             >",
				"                                =                             >",
				" =                                                            >",
				"                                                              >",
				"     =                  <                                     >",
				"                                          =                   >",
				"                                          =                   >",
				"  ===<==<=<<<<<         < = =<<< <  =<== =                    >",
				"                        =                                     >",
				"                    =<<<<<                <            <      >",
				"                   =<<<<<<                <            <      >",
				"                                >                             >",
				"               =<<< <<<<<<                <            <      >",
				"                                =                             >",
				"                                >                             >",
				"                                =                             >",
				"                                >                             >",
				"     =              =<<<<<                <            <      >",
				"                                > >>           >>>>>> >>>>>   >",
				"               =<<< <<<<<<                <            <      >",
				"                                >                           >>>",
				"                    =<<<<<                <            <      >",
				"                    =<<<<<                <            <      >",
				"                    =<<<<<                <            <      >",
				"                    =<<<<<                <            <      >",
				"                    =<<<<<                <            <      >",
				"                    =<<<<<                <            <      >",
				"                    =<<<<<                <            <      >",
				"                 =< <<<<<<                < <          <      >",
				"                     =<<<<                <                   >",
				"                     =<<<<                <                   >",
				"                      =<<<                <                   >",
				"                      =<<<                <                   >",
				"                       =<<                <                   >",
				"     =                  <                                     >",
				"                 =< <<<<<<                < <        < <      >",
				"                =<< <<<<<<                < <        < <      >",
				"<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< "
			};
		}

		private Connotial Cell(int row, int column)
		{
			switch (this.table[row][column])
			{
			case ' ': return Connotial.NoConnotial;
			case '<': return Connotial.LessConnotial;
			case '>': return Connotial.GreaterConnotial;
			case '=': return Connotial.EqualConnotial;
			}
		}

		public Connotial CompareTerminals(string leftTerminal,string rightTerminal)
		{
			int row = this.terminals[leftTerminal];
			int column = this.terminals[rightTerminal];
			return Cell(row,column);
		}

		public BottomUpTable ()
		{
			SetupDictionary();
			SetupTable();
		}
	}
}

