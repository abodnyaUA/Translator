using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Translators
{
    class SyntaxAnalyzer
    {
        private static SyntaxAnalyzer _sharedAnalyzer = null;
        public static SyntaxAnalyzer sharedAnalyzer
        {
            get
            {
                if (_sharedAnalyzer == null) _sharedAnalyzer = new SyntaxAnalyzer();
                return _sharedAnalyzer;
            }
        }
        private SyntaxAnalyzer()
        {
        }

		public List<Lexem> lexems;
		public List<string> IDs;
		public List<string> CONSTs;
		public List<string> lexemsDict;
        public void AnalyzeLexems()
        {
            int lexemsIterator = 0;
            this.lexems = LexemAnalyzer.sharedAnalyzer.Lexems;
            this.IDs = LexemAnalyzer.sharedAnalyzer.IDs;
            this.CONSTs = LexemAnalyzer.sharedAnalyzer.CONSTs;
            this.lexemsDict = LexemAnalyzer.sharedAnalyzer.dict;

            /* @interface */
			Checker.Check(ref lexemsIterator,0,
			      "'Interface' accept",
			      "Application should start with '@interface'");
            
			/* ID */
			Checker.Check(ref lexemsIterator,lexemsDict.Count-2,
			      "Name "+lexems[lexemsIterator].command+" accept",
			      "Invalid @interface name: "+lexems[lexemsIterator].command + " ("+lexems[lexemsIterator].key+")");

			/* ENTER */
			Checker.Check(ref lexemsIterator,6,
			      "ENTER accepted",
			      "Not found ENTER after @interface name");
                
            /* PARSE Interface */
			Checker.Check(ref lexemsIterator,AnalyzeInterface,
			      "Definitions [Interface block] accept",
			      "Invalid declarations");

			/* @implementation */
			Checker.Check(ref lexemsIterator,1,
			      "'Implementation' accept",
			      "Not found '@implementation' word");

			/* ENTER */
			Checker.Check(ref lexemsIterator,6,
			      "ENTER accepted",
			      "Not found ENTER after @implementation name");

			/* Parse Implementation */
			Checker.Check(ref lexemsIterator,AnalyzeImplementation,
			          "Operators [Implementation block] accept",
			          "Invalid operators block");
            
			/* @end */
			Checker.Check(ref lexemsIterator,2,
			      "End accept. Success syntax analyze.",
			      "Invalide declaration @end",Checker.IncrementMode.NoIncrement,Out.State.LogInfo);
        }

		public bool AnalyzeInterface(ref int lexemsIterator)
        {
            if (lexems[lexemsIterator].key == 1) //empty interface
            {
                return true;
            }
            do
            {
                /* Type int */
				Checker.Check(ref lexemsIterator, 3,
				              "Confirm type int",
				              "Invalid declaration construction. Need to set type");

				/* ID */
				Checker.Check(ref lexemsIterator, lexemsDict.Count-2,
				              "Confirm ID "+lexems[lexemsIterator].command,
				              "Empty declaration",Out.State.LogInfo);

                /* See next IDs */
                while (lexems[lexemsIterator].key != 6) // ENTER
                {
					if (lexems[lexemsIterator].key == 31 && lexems[lexemsIterator + 1].key == lexemsDict.Count-2) //,id
                    {
						Out.Log(Out.State.LogInfo,"Confirm ID " + lexems[lexemsIterator+1].command);
                        lexemsIterator += 2;
                    }
                    else
                    {
                        LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
                            "Invalid declaration");
                        throw error;
                    }
                }

                //Finishing
				Checker.Check(ref lexemsIterator,6,"","Missed ENTER");

            } while (lexems[lexemsIterator].key != 1); /* @implementation */
            return true;
        }
		public bool AnalyzeImplementation(ref int lexemsIterator)
        {
			AnalyzeOperatorsBlock(ref lexemsIterator,2);
            return true;
        }

		public void AnalyzeOperatorsBlock(ref int lexemsIterator, int endLexemKey)
		{
			if (lexems[lexemsIterator].key == 2) //empty implementation
			{
				return;
			}
			do
			{
				Out.Log(Out.State.LogDebug,"Previous lexem: "+lexems[lexemsIterator-1].command);
				Out.Log(Out.State.LogDebug,"Current lexem: "+lexems[lexemsIterator].command);
				AnalyzeOperator(ref lexemsIterator);
				//Finishing
				Checker.Check(ref lexemsIterator,6,"","Missed ENTER. Find operator: "+lexems[lexemsIterator].command);
			}
			while (lexems[lexemsIterator].key != endLexemKey);
		}

		public bool AnalyzeOperator(ref int lexemsIterator)
		{
			Out.Log(Out.State.LogDebug,"Current lexem: "+lexems[lexemsIterator].command);

			if (lexems[lexemsIterator].command == "input") //input
			{
				AnalyzeIO(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Input accepted");
			}
			else if (lexems[lexemsIterator].command == "output") //output
			{
				AnalyzeIO(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Output accepted");
			}
			else if (lexems[lexemsIterator].command == "for") //for
			{
				AnalyzeCycle(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Cycle 'for' accepted");
			}
			else if (lexems[lexemsIterator].command == "if") //if
			{
				AnalyzeCondition(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Condition accepted");
			}
			else if (lexems[lexemsIterator].key == lexemsDict.Count-2) //setter
			{
				AnalyzeSetter(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Assignment accepted");
			}
			else
			{
				LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
				                                          "Invalid operator "+lexems[lexemsIterator].command);
				throw error;
			}
			return true;
		}

		/// <summary>
		/// Analyzes the IO operators (input, output)
		/// </summary>
		/// <returns><c>true</c>, if I was analyzed, <c>false</c> otherwise.</returns>
		/// <param name="lexemsIterator">Lexems iterator.</param>
		public bool AnalyzeIO(ref int lexemsIterator)
        {
            lexemsIterator++;

            /* ( */
			Checker.Check(ref lexemsIterator, "(",
			              "Confirm (",
			              "IO functions must start with '('");
            
            /* ID */
			Checker.Check(ref lexemsIterator, lexemsDict.Count-2,
			              "Confirm ID "+lexems[lexemsIterator].command,
			              "IO functions can't be used without arguments");

            /* See next IDs */
            while (lexems[lexemsIterator].key != 17) // )
            {
				if (lexems[lexemsIterator].key == 31 && lexems[lexemsIterator + 1].key == lexemsDict.Count-2) //,id
                {
					Out.Log(Out.State.LogVerbose,"Confirm ID "+lexems[lexemsIterator+1].command);
                    lexemsIterator += 2;
                }
                else
                {
                    LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
                        "Invalid using the variables in function 'input'");
                    throw error;
                }
            }
			Out.Log(Out.State.LogVerbose,"Confirm ')'");
                
            
            lexemsIterator++;
            return true;
        }

		public bool AnalyzeSetter(ref int lexemsIterator)
		{
			lexemsIterator++;
			Checker.Check(ref lexemsIterator,"=",
			              "'=' accepted","Invalid setter. Missed '='");
			AnalyzeExpression(ref lexemsIterator);
			return true;
		}

		public bool AnalyzeCondition(ref int lexemsIterator)
		{
			lexemsIterator++;

			Checker.Check(ref lexemsIterator,AnalyzeLogicalExpression,
			              "ACCEPT LOGICAL EXPRESSION. NEXT LEXEM IS: "+lexems[lexemsIterator].command,
			              "Invalid Condition Operator",Checker.IncrementMode.IncrementOnce);
			
			Checker.Check(ref lexemsIterator,AnalyzeAction,
			              "ACCEPT ACTION EXPRESSION. NEXT LEXEM IS: "+lexems[lexemsIterator].command,
			              "Invalid Condition Operator",Checker.IncrementMode.IncrementOnce);

			Checker.Check(ref lexemsIterator,"else",
			              "Passed ELSE","Missed 'else'",
			              Checker.IncrementMode.DoubleIncrement);
			
			Checker.Check(ref lexemsIterator,AnalyzeAction,
			              "ACCEPT ACTION EXPRESSION. NEXT LEXEM IS: "+lexems[lexemsIterator].command,
			              "Invalid Condition Operator",Checker.IncrementMode.IncrementOnce);
			
			Checker.Check(ref lexemsIterator,"endif",
			              "Passed ELSE","Missed 'endif'");
			return true;
		}

		public bool AnalyzeCycle(ref int lexemsIterator)
		{
			lexemsIterator++;

			Checker.Check(ref lexemsIterator, AnalyzeSetter,
			              "SETTER PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].command,
			              "Invalid cycle. Error while analyze SETTER");

			Checker.Check(ref lexemsIterator, "to","'to' accept","'to' missed");

			Checker.Check(ref lexemsIterator, AnalyzeExpression,
			              "EXPRESSION PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].command,
			              "Invalid cycle. Error while analyze EXPRESSION");
			
			Checker.Check(ref lexemsIterator, "step","'step' accept","'step' missed");
			
			Checker.Check(ref lexemsIterator, AnalyzeExpression,
			              "EXPRESSION PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].command,
			              "Invalid cycle. Error while analyze EXPRESSION",
			              Checker.IncrementMode.IncrementOnce);
			
			Checker.Check(ref lexemsIterator, AnalyzeAction,
			              "ACTION PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].command,
			              "Invalid cycle. Error while analyze ACTION",
			              Checker.IncrementMode.IncrementOnce);
			
			Checker.Check(ref lexemsIterator, "next","'next' accept","'next' missed");
			return true;
		}

		public bool AnalyzeAction(ref int lexemsIterator)
		{
			if (lexems[lexemsIterator].command == "{")
			{
				lexemsIterator += 2; // {\n

				AnalyzeOperatorsBlock(ref lexemsIterator,15);

				Checker.Check(ref lexemsIterator,"}","Accept '}'","Missed '}'");
			}
			else
			{
				AnalyzeOperator(ref lexemsIterator);
			}
			return true;
		}
        
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool AnalyzeExpression(ref int lexemsIterator)
		{
			Out.Log(Out.State.LogVerbose,"Start analyze expression");

			Out.Log(Out.State.LogVerbose,"!Current lexem: "+lexems[lexemsIterator].command);
			if (lexems[lexemsIterator].command == "-") 
			{
				lexemsIterator++;
				if (isTerm(ref lexemsIterator))
				{
					Out.Log(Out.State.LogVerbose,"Expression starts with '-': "+lexems[lexemsIterator].command);
					lexemsIterator ++;
				}
				else
				{
					Out.Log(Out.State.LogVerbose,"Expression doesn't start with '-': "+lexems[lexemsIterator].command);
					lexemsIterator--;
				}
			} 
			else Checker.Check(ref lexemsIterator,isTerm,
				              "Expression starts normally: "+lexems[lexemsIterator].command,
				              "Invalid expression",Checker.IncrementMode.IncrementOnce);
			Out.Log(Out.State.LogVerbose,"!!Current lexem: "+lexems[lexemsIterator].command);


			while (true)
			{
				Out.Log(Out.State.LogVerbose,"Continue analyze expression");
				if (lexems[lexemsIterator].command == "-" || lexems[lexemsIterator].command == "+")
				{
					Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].command);
					Out.Log(Out.State.LogVerbose,"After operator lexem is "+lexems[lexemsIterator+1].command);
					lexemsIterator++;
					Checker.Check(ref lexemsIterator,isTerm,
					              "Next term accepted: "+lexems[lexemsIterator].command,
					              "Symbol "+lexems[lexemsIterator].command+" is invalid",
					              Checker.IncrementMode.IncrementOnce);
				}
				else
				{
					Out.Log(Out.State.LogVerbose,"+-Current lexem: "+lexems[lexemsIterator].command);
					break;
				}
			}
			return true;
		}
		public bool isTerm(ref int lexemsIterator)
		{
			int oldIterator = lexemsIterator;
			if (isMultiplier(ref lexemsIterator))
			{
				Out.Log(Out.State.LogVerbose,"First term is multiplier. All OK");
			}
			else
			{
				Out.Log(Out.State.LogVerbose,"First term isn't multiplier. All BAD");
				lexemsIterator = oldIterator;
				return false;
			}

			Out.Log(Out.State.LogVerbose,"Yeah. Let see what's next: "+lexems[lexemsIterator].command+
			                  "And after that I see "+lexems[lexemsIterator+1].command);
			if (lexems[lexemsIterator+1].command == "*" || lexems[lexemsIterator+1].command == "/")
			{
				Out.Log(Out.State.LogVerbose,"Using */. Increment operator");
				lexemsIterator++;
				while (true)
				{
					if (lexems[lexemsIterator].command == "*" || lexems[lexemsIterator].command == "/")
					{
						Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].command);
						lexemsIterator++;
						Checker.Check(ref lexemsIterator,isMultiplier,
						              "Next multiplier accepted: "+lexems[lexemsIterator].command,
						              "Symbol "+lexems[lexemsIterator].command+" is invalid",
						              Checker.IncrementMode.IncrementOnce);
					}
					else
					{
						Out.Log(Out.State.LogVerbose,"*/Current lexem: "+lexems[lexemsIterator].command);
						break;
					}
				}
				lexemsIterator--;
			}
			else
			{
				Out.Log(Out.State.LogVerbose,"Don't use */");
			}
			return true;
		}
		public bool isMultiplier(ref int lexemsIterator)
		{
			int oldIterator = lexemsIterator;
			if (isExprResponce(ref lexemsIterator))
			{
				Out.Log(Out.State.LogVerbose,"First multiplier is exprResponce. All OK");
			}
			else
			{
				Out.Log(Out.State.LogVerbose,"First multiplier isn't exprResponce. All BAD");
				lexemsIterator = oldIterator;
				return false;
			}

			Out.Log(Out.State.LogVerbose,"Yeah. Let see what's next: "+lexems[lexemsIterator].command+
			                  "And after that I see "+lexems[lexemsIterator+1].command);
			if (lexems[lexemsIterator+1].command == "^")
			{
				Out.Log(Out.State.LogVerbose,"Using ^. Increment iterator");
				lexemsIterator++;
				while (true)
				{
					if (lexems[lexemsIterator].command == "^")
					{
						Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].command);
						lexemsIterator++;
						
						Checker.Check(ref lexemsIterator,isExprResponce,
						              "Next expressionResponse accepted: "+lexems[lexemsIterator].command,
						              "Symbol "+lexems[lexemsIterator].command+" is invalid",
						              Checker.IncrementMode.IncrementOnce);
					}
					else
					{
						Out.Log(Out.State.LogVerbose,"^Current lexem: "+lexems[lexemsIterator].command);
						break;
					}
				}
				lexemsIterator--;
			}
			else
			{
				Out.Log(Out.State.LogVerbose,"Don't use ^");
			}

			return true;
		}

		public bool isExprResponce(ref int lexemsIterator)
		{
			int oldIterator = lexemsIterator;
			while (lexems[lexemsIterator].command == " " || lexems[lexemsIterator].command == "") lexemsIterator++;
			if (lexems[lexemsIterator].key == lexemsDict.Count-2)
			{
				Out.Log(Out.State.LogVerbose,"Founded exprResponce is ID: "+lexems[lexemsIterator].command);
			}
			else if (lexems[lexemsIterator].key == lexemsDict.Count-1)
			{
				Out.Log(Out.State.LogVerbose,"Founded exprResponce is CONST: "+lexems[lexemsIterator].command);
			}
			else if (lexems[lexemsIterator].command == "(")
			{
				Out.Log(Out.State.LogVerbose,"() Open Scobes ()");
				lexemsIterator++;
				AnalyzeExpression(ref lexemsIterator);
				Out.Log(Out.State.LogVerbose,"() Return from block with lexem "+lexems[lexemsIterator].command);
				Out.Log(Out.State.LogVerbose,"() Close Scobes ()");
			}
			else
			{
				Out.Log(Out.State.LogVerbose,"Wrong lexem: "+lexems[lexemsIterator].command);
				lexemsIterator = oldIterator;
				return false;
			}
			return true;
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool AnalyzeLogicalExpression(ref int lexemsIterator)
		{
			Out.Log(Out.State.LogVerbose,"Start analyze logical expression");

			Out.Log(Out.State.LogVerbose,"!Current lexem: "+lexems[lexemsIterator].command);
			Checker.Check(ref lexemsIterator,isLogicalExpressionLevel1,
			              "Logical expression starts normally: "+lexems[lexemsIterator].command,
			              "Invalid logical expression");
			Out.Log(Out.State.LogVerbose,"!!Current lexem: "+lexems[lexemsIterator].command);

			while (true)
			{
				Out.Log(Out.State.LogVerbose,"Continue analyze expression");
				if (lexems[lexemsIterator].command == "or")
				{
					Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].command);
					Out.Log(Out.State.LogVerbose,"After operator lexem is "+lexems[lexemsIterator+1].command);
					lexemsIterator++;
					Checker.Check(ref lexemsIterator,isLogicalExpressionLevel1,
					              "Next LogicalExpressionLevel1 accepted: "+lexems[lexemsIterator].command+"\n",
					              "Symbol "+lexems[lexemsIterator].command+" is invalid");
				}
				else
				{
					Out.Log(Out.State.LogVerbose,"||Current lexem: "+lexems[lexemsIterator].command);
					break;
				}
			}
			return true;
		}

		public bool isLogicalExpressionLevel1(ref int lexemsIterator)
		{
			int oldIterator = lexemsIterator;
			if (isLogicalExpressionLevel2(ref lexemsIterator))
			{
				Out.Log(Out.State.LogVerbose,"First LogicalExpressionLevel1 is LogicalExpressionLevel2. All OK");
			}
			else
			{
				Out.Log(Out.State.LogVerbose,"First LogicalExpressionLevel1 isn't LogicalExpressionLevel2. All BAD");
				lexemsIterator = oldIterator;
				return false;
			}

			Out.Log(Out.State.LogVerbose,"Yeah. Let see what's next: "+lexems[lexemsIterator].command+
			                  "And after that I see "+lexems[lexemsIterator+1].command);
			if (lexems[lexemsIterator].command == "and")
			{
				Out.Log(Out.State.LogVerbose,"I see you Use && after that. So I will inc iterator for you");
				//lexemsIterator++;
				while (true)
				{
					if (lexems[lexemsIterator].command == "and")
					{
						Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].command);
						lexemsIterator++;
						Checker.Check(ref lexemsIterator,isLogicalExpressionLevel2,
						              "Next LogicalExpressionLevel2 accepted: "+lexems[lexemsIterator].command+"\n",
						              "Symbol "+lexems[lexemsIterator].command+" is invalid",
						              Checker.IncrementMode.IncrementOnce);
					}
					else
					{
						Out.Log(Out.State.LogVerbose,"&&Current lexem: "+lexems[lexemsIterator].command);
						break;
					}
				}
				lexemsIterator--;
			}
			else
			{
				Out.Log(Out.State.LogVerbose,"Don't use operator 'and'");
			}
			return true;
		}

		
		public bool isLogicalExpressionLevel2(ref int lexemsIterator)
		{
			while (lexems[lexemsIterator].command == "!")
			{
				Out.Log(Out.State.LogVerbose,"Using '!'");
				lexemsIterator++;
			}

			if (lexems[lexemsIterator].command == "[")
			{
				Out.Log(Out.State.LogVerbose,"[] Open scobes []");
				lexemsIterator++;
				AnalyzeLogicalExpression(ref lexemsIterator);
				Out.Log(Out.State.LogVerbose,"[] Return from block with lexem "+lexems[lexemsIterator].command);
				Out.Log(Out.State.LogVerbose,"[] Close scobes []");
				lexemsIterator++;
			}
			else
			{
				return isNormalRelation(ref lexemsIterator);
			}

			return true;
		}

		public bool isNormalRelation(ref int lexemsIterator)
		{
			HashSet<string> connotials = new HashSet<string>()
			{
				">", ">=", "<=", "<", "equ", "!="
			};

			AnalyzeExpression(ref lexemsIterator);
			Out.Log(Out.State.LogVerbose,"....... 1.After analyze expression next lexem is: "+lexems[lexemsIterator].command);
			if (connotials.Contains(lexems[lexemsIterator].command)) 
			{
				Out.Log(Out.State.LogVerbose,"Match connotial: "+lexems[lexemsIterator].command);
				lexemsIterator++;
			}
			else
			{
				LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
				                                "Invalid Connotial "+lexems[lexemsIterator].command);
				throw error;
			}
			AnalyzeExpression(ref lexemsIterator);
			Out.Log(Out.State.LogVerbose,"....... 2. After analyze expression next lexem is: "+lexems[lexemsIterator].command);
			return true;
		}
	}
}
