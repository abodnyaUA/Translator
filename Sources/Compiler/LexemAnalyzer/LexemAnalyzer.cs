using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Translators
{
    class LexemAnalyzer
    {
        private static LexemAnalyzer _sharedAnalyzer = null;
        public static LexemAnalyzer sharedAnalyzer
        {
            get
            {
                if (_sharedAnalyzer == null) _sharedAnalyzer = new LexemAnalyzer();
                return _sharedAnalyzer;
            }
        }
		private LexemAnalyzer() { }

		// Checking right section //
        private bool InterfaceWasDeclarated = false;
        private bool ImplementationWasDeclarated = false;
        private bool EndWasDeclarated = false;

		// Additional usefull methods //
		/// <summary>
		/// Checks lexem-string for global commands.
		/// </summary>
		/// <param name='value'>
		/// Lexem-string.
		/// </param>
		/// <param name='line'>
		/// Line number.
		/// </param>
        private void CheckForGlobalCommands(string value, int line)
        {
            if (value == "@interface")
            {
                if (ImplementationWasDeclarated || EndWasDeclarated || InterfaceWasDeclarated)
                {
					throw new LexemException(line,"Invalide declaration @interface");
                }
                else
				{
                    InterfaceWasDeclarated = true;
				}
            }
            if (value == "@implementation")
            {
                if (!InterfaceWasDeclarated || EndWasDeclarated || ImplementationWasDeclarated)
                {
					throw new LexemException(line,"Invalide declaration @implementation");
                }
                else
				{
                    ImplementationWasDeclarated = true;
				}
            }
            if (value == "@end")
            {
                if (!InterfaceWasDeclarated || !ImplementationWasDeclarated || EndWasDeclarated)
                {
                    throw new LexemException(line,"Invalide declaration @end");
                }
                else
				{
                    EndWasDeclarated = true;
				}
            }
            if (value == "int" && ImplementationWasDeclarated)
            {
                throw new LexemException(line,"Variables can be declarated only in @interface section");
            }
        }
		// Check is lexem-string 'const' lexem
		private bool isConst(string value)
		{
			bool con = true;
			if (value[0] == '"' && value[value.Length-1] == '"')
			{
				con = true;
			}
			else foreach (char ch in value)
			{
				if (ch < '0' || ch > '9')
				{
					con = false;
					break;
				}
			}
			return con;
		}

		// Analyzing //
		private void AnalyzeConst(string value)
		{
			// Tabel base //
			PrintBaseTableLine(value);
			// Check earlier definition //
			int wasDeclaratedIndex = -1;
			for (int j = 0; j < CONSTs.Count; j++)
			{
				if (value == CONSTs[j])
				{
					wasDeclaratedIndex = j;
					break;
				}
			}
			// It hasn't declarated.
			if (wasDeclaratedIndex == -1)
			{
				CONSTs.Add(value);
				Lexems.Add(new Lexem(line, value, dict.Count-1));
				Out.Log(Out.State.LogInfo,dict.Count + "\t\t" + CONSTs.Count);
			}
			else
			{
				Out.Log(Out.State.LogInfo,dict.Count + "\t\t" + (wasDeclaratedIndex+1));
				Lexems.Add(new Lexem(line, value, dict.Count-1));
			}
		}
		private void ValidateID(string value)
		{
			if (!((value[0] >= 'a' && value[0] <= 'z') ||
			      (value[0] >= 'A' && value[0] <= 'Z')))
			{
				Out.Log(Out.State.LogInfo,"");
				throw new LexemException((line+1),"Invalid symbol '" + value[0]+"'");
			}
			for (int c = 1; c < value.Length; c++)
			{
				if (!((value[c] >= 'a' && value[c] <= 'z') ||
				      (value[c] >= 'A' && value[c] <= 'Z') ||
				      (value[c] >= '0' && value[c] <= '9')))
				{
					Out.Log(Out.State.LogInfo,"");
					throw new LexemException((line+1),"Invalid symbol '"+value[c]+"'");
				}
			}
		}
		private void InsertNewID(string value)
		{
			int wasDeclaratedIndex = -1;
			for (int j = 0; j < IDs.Count; j++)
			{
				if (value == IDs[j])
				{
					wasDeclaratedIndex = j;
					break;
				}
			}
			// It hasn't declarated.
			if (wasDeclaratedIndex != -1)
			{
				Out.Log(Out.State.LogInfo,"");
				throw new LexemException((line+1),"Variable " + value + " has declarated earlier");
			}
			else
			{
				// Declaration zone //
				IDs.Add(value);
				Lexems.Add(new Lexem(line, value, dict.Count - 2));
				Out.Log(Out.State.LogInfo,dict.Count - 1 + "\t" + IDs.Count);
			}
		}
		private void ProcessExistID(string value)
		{
			// Find in declarations
			int wasDeclaratedIndex = -1;
			for (int j = 0; j < IDs.Count; j++)
			{
				if (value == IDs[j])
				{
					wasDeclaratedIndex = j;
					break;
				}
			}
			// It hasn't declarated.
			if (wasDeclaratedIndex == -1)
			{
				Out.Log(Out.State.LogInfo,"");
				LexemException error2 = new LexemException((line+1),"Variable " + value + " hasn't declarated");
				throw error2;
			}
			// Fuuuh. I've find it.
			else
			{
				Lexems.Add(new Lexem(line, value, dict.Count - 2));
				Out.Log(Out.State.LogInfo, dict.Count - 1 + "\t" + (wasDeclaratedIndex + 1));
			}
		}
		private void AnalyzeID(string value)
		{
			// Table base //
			PrintBaseTableLine(value);

			// Validate ID name //
			ValidateID(value);

			// It's interface zone ? Then it's mabe new id
			if (InterfaceWasDeclarated && !ImplementationWasDeclarated)
			{
				InsertNewID(value);
			}
			// No? It was declarated?
			else
			{
				ProcessExistID(value);
			}
		}
		private int IndexOfOperation(string value)
		{
			// Try find lexem in Lexem's Table
			int find = int.MaxValue;
			for (int j = 0; j < dict.Count; j++)
			{
				// Find it. Great
				if (value.Equals(dict[j]))
				{
					// Check it for global
					CheckForGlobalCommands(value,line);
					find = j;
					break;
				}
			}
			return find;
		}

		// Main containers //
		private List<string> IDs;
		private List<string> CONSTs;
		private List<Lexem> Lexems;
		private List<string> dict;
		private int line;

        // Main Function: Parse doubleArray //
        public void AnalyzeWithDoubleList(List<List<string>> parsedList)
		{
			InterfaceWasDeclarated = false;
			ImplementationWasDeclarated = false;
			EndWasDeclarated = false;
			this.IDs = new List<string>();
			this.CONSTs = new List<string>();
			this.Lexems = new List<Lexem>();
			this.dict = LexemList.Instance.Grammar;

			Out.Log(Out.State.LogInfo,"Line  Command         Key\tID\tConst");
            // line cycle
			for (this.line = 0; this.line < parsedList.Count; this.line++)
            {
				// Check empty line. Some magic.
				bool validLine = true;
				try
				{
					if (parsedList[this.line][0] == "\n" 
					    && this.line == parsedList.Count-1 
					    && parsedList[this.line].Count == 1) validLine = false;
				} catch (Exception) {}
                // Lexems in line cycle
				if (validLine)
				foreach (string lexem in parsedList[this.line])
                {
					//First lexem shouldn't be ENTER
					if (Lexems.Count == 0 && lexem == "\n") continue;
					
					string value = lexem.Replace(" ","");
					//Continue
                    
					if (value == "-" && !Lexems[Lexems.Count-1].isIDorCONST())
					{
						AnalyzeConst("0");
					}
                    //It's table's lexem
					int lexemIndex = IndexOfOperation(value);
                    if (int.MaxValue != lexemIndex)
                    {						
						PrintBaseTableLine(value);
						Out.Log(Out.State.LogInfo,""+(lexemIndex + 1));
						Lexems.Add(new Lexem(this.line,value,lexemIndex));
                    }
                    // No? Don't worry. May be it's ID or CONST
                    else
                    {
                        // It's const?
                        if (isConst(value))
                        {
							AnalyzeConst(value);
                        }
                        // No? Okay, May be ID
                        else
                        {
							AnalyzeID(value);
                        }
                    }
                } // Lexems Cycle
            } // Line cycle
			LexemList.Instance.UpdateConsts(CONSTs);
			LexemList.Instance.UpdateIDs(IDs);
			LexemList.Instance.UpdateLexems(Lexems);

			PrintTables();
        }

		// Print Table //
		private void PrintBaseTableLine(string value)
		{
			Out.LogOneLine(Out.State.LogInfo,
			               (line < 9 ? "0" : "") + (line + 1) + "  " + (value == "\n" ? "ENTER" : value));
			for (int j = 0; j < 18 - value.Length; j++) 
				Out.LogOneLine(Out.State.LogInfo," ");
		}
        public void PrintTables()
        {
			Out.Log(Out.State.LogInfo,"IDs:");
			Out.Log(Out.State.LogInfo,"Num  ID");
            for (int i=0;i<IDs.Count;i++)
            {
                // Start new line
				Out.LogOneLine(Out.State.LogInfo,(i < 9 ? "0" : "") + (i + 1) + "  " + IDs[i]);
				for (int j = 0; j < 18 - IDs[i].Length; j++) Out.LogOneLine(Out.State.LogInfo," ");
				Out.Log(Out.State.LogInfo,"");
            }

			Out.Log(Out.State.LogInfo,"");
			Out.Log(Out.State.LogInfo,"CONSTs:");
			Out.Log(Out.State.LogInfo,"Num  CONST");
            for (int i = 0; i < CONSTs.Count; i++)
            {
                // Start new line
				Out.LogOneLine(Out.State.LogInfo,(i < 9 ? "0" : "") + (i + 1) + "  " + CONSTs[i]);
				for (int j = 0; j < 18 - CONSTs[i].Length; j++) Out.LogOneLine(Out.State.LogInfo," ");
				Out.Log(Out.State.LogInfo,"");
            }
			Out.Log(Out.State.LogInfo,"");
        }
    }
}
