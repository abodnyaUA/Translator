using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Translators
{
    class SyntaxAnalyzerRecursiveDown : ISyntaxAnalyzer
    {
		private static SyntaxAnalyzerRecursiveDown _sharedAnalyzer = null;
		public static SyntaxAnalyzerRecursiveDown sharedAnalyzer
        {
            get
            {
				if (_sharedAnalyzer == null) _sharedAnalyzer = new SyntaxAnalyzerRecursiveDown();
                return _sharedAnalyzer;
            }
        }
		private SyntaxAnalyzerRecursiveDown() { }

		public List<Lexem> lexems;
		public List<string> lexemsDict;
        public void AnalyzeLexems()
        {
            int lexemsIterator = 0;
			this.lexems = LexemList.Instance.Lexems;
			this.lexemsDict = LexemList.Instance.Grammar;

            /* @interface */
			Checker.Check(ref lexemsIterator,0,
			      "'Interface' accept",
			      "Application should start with '@interface'");
            
			/* ID */
			Checker.Check(ref lexemsIterator,lexemsDict.Count-2,
			              "Name "+lexems[lexemsIterator].Command+" accept",
			              "Invalid @interface name: "+lexems[lexemsIterator].Command
			              + " ("+lexems[lexemsIterator].Key+")");

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
            if (lexems[lexemsIterator].Key == 1) //empty interface
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
				              "Confirm ID "+lexems[lexemsIterator].Command,
				              "Empty declaration",Out.State.LogInfo);

                /* See next IDs */
				while (lexems[lexemsIterator].isSeparator()) // ENTER
                {
					if (lexems[lexemsIterator].Key == 31 && lexems[lexemsIterator + 1].isID()) //,id
                    {
						Out.Log(Out.State.LogInfo,"Confirm ID " + lexems[lexemsIterator+1].Command);
                        lexemsIterator += 2;
                    }
                    else
                    {
                        throw new LexemException(lexems[lexemsIterator].LineNumber,
                            "Invalid declaration");
                    }
                }

                //Finishing
				Checker.Check(ref lexemsIterator,6,"","Missed ENTER");

            } while (lexems[lexemsIterator].Key != 1); /* @implementation */
            return true;
        }

		public bool AnalyzeImplementation(ref int lexemsIterator)
        {
			AnalyzeOperatorsBlock(ref lexemsIterator,2);
            return true;
        }

		public void AnalyzeOperatorsBlock(ref int lexemsIterator, int endLexemKey)
		{
			if (lexems[lexemsIterator].Key == endLexemKey) //empty implementation
			{
				return;
			}
			do
			{
				Out.Log(Out.State.LogDebug,"Previous lexem: "+lexems[lexemsIterator-1].Command);
				Out.Log(Out.State.LogDebug,"Current lexem: "+lexems[lexemsIterator].Command);
				AnalyzeOperator(ref lexemsIterator);
				//Finishing
				Checker.Check(ref lexemsIterator,6,"","Missed ENTER. Find operator: "+lexems[lexemsIterator].Command);
			}
			while (lexems[lexemsIterator].Key != endLexemKey);
		}

		public bool AnalyzeOperator(ref int lexemsIterator)
		{
			Out.Log(Out.State.LogDebug,"Current lexem: "+lexems[lexemsIterator].Command);

			if (lexems[lexemsIterator].Command == "input") //input
			{
				AnalyzeIO(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Input accepted");
			}
			else if (lexems[lexemsIterator].Command == "output") //output
			{
				AnalyzeIO(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Output accepted");
			}
			else if (lexems[lexemsIterator].Command == "for") //for
			{
				AnalyzeCycle(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Cycle 'for' accepted");
			}
			else if (lexems[lexemsIterator].Command == "if") //if
			{
				AnalyzeCondition(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Condition accepted");
			}
			else if (lexems[lexemsIterator].isID()) //setter
			{
				AnalyzeSetter(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Assignment accepted");
			}
			else
			{
				throw new LexemException(lexems[lexemsIterator].LineNumber,
				                                          "Invalid operator "+lexems[lexemsIterator].Command);
				
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
			              "Confirm ID "+lexems[lexemsIterator].Command,
			              "IO functions can't be used without arguments");

            /* See next IDs */
            while (lexems[lexemsIterator].Key != 17) // )
            {
				if (lexems[lexemsIterator].Key == 31 && lexems[lexemsIterator + 1].isID()) //,id
                {
					Out.Log(Out.State.LogVerbose,"Confirm ID "+lexems[lexemsIterator+1].Command);
                    lexemsIterator += 2;
                }
                else
                {
                    throw new LexemException(lexems[lexemsIterator].LineNumber,
                        "Invalid using the variables in function 'input'");

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
			              "ACCEPT LOGICAL EXPRESSION. NEXT LEXEM IS: "+lexems[lexemsIterator].Command,
			              "Invalid Condition Operator",Checker.IncrementMode.IncrementOnce);
			
			Checker.Check(ref lexemsIterator,AnalyzeAction,
			              "ACCEPT ACTION EXPRESSION. NEXT LEXEM IS: "+lexems[lexemsIterator].Command,
			              "Invalid Condition Operator",Checker.IncrementMode.IncrementOnce);

			Checker.Check(ref lexemsIterator,"else",
			              "Passed ELSE","Missed 'else'",
			              Checker.IncrementMode.DoubleIncrement);
			
			Checker.Check(ref lexemsIterator,AnalyzeAction,
			              "ACCEPT ACTION EXPRESSION. NEXT LEXEM IS: "+lexems[lexemsIterator].Command,
			              "Invalid Condition Operator",Checker.IncrementMode.IncrementOnce);
			
			Checker.Check(ref lexemsIterator,"endif",
			              "Passed ELSE","Missed 'endif'");
			return true;
		}

		public bool AnalyzeCycle(ref int lexemsIterator)
		{
			lexemsIterator++;

			Checker.Check(ref lexemsIterator, AnalyzeSetter,
			              "SETTER PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].Command,
			              "Invalid cycle. Error while analyze SETTER");

			Checker.Check(ref lexemsIterator, "to","'to' accept","'to' missed");

			Checker.Check(ref lexemsIterator, AnalyzeExpression,
			              "EXPRESSION PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].Command,
			              "Invalid cycle. Error while analyze EXPRESSION");
			
			Checker.Check(ref lexemsIterator, "step","'step' accept","'step' missed");
			
			Checker.Check(ref lexemsIterator, AnalyzeExpression,
			              "EXPRESSION PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].Command,
			              "Invalid cycle. Error while analyze EXPRESSION",
			              Checker.IncrementMode.IncrementOnce);
			
			Checker.Check(ref lexemsIterator, AnalyzeAction,
			              "ACTION PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].Command,
			              "Invalid cycle. Error while analyze ACTION",
			              Checker.IncrementMode.IncrementOnce);
			
			Checker.Check(ref lexemsIterator, "next","'next' accept","'next' missed");
			return true;
		}

		public bool AnalyzeAction(ref int lexemsIterator)
		{
			if (lexems[lexemsIterator].Command == "{")
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

			Out.Log(Out.State.LogVerbose,"!Current lexem: "+lexems[lexemsIterator].Command);
			if (lexems[lexemsIterator].Command == "-") 
			{
				lexemsIterator++;
				if (isTerm(ref lexemsIterator))
				{
					Out.Log(Out.State.LogVerbose,"Expression starts with '-': "+lexems[lexemsIterator].Command);
					lexemsIterator ++;
				}
				else
				{
					Out.Log(Out.State.LogVerbose,"Expression doesn't start with '-': "+lexems[lexemsIterator].Command);
					lexemsIterator--;
				}
			} 
			else Checker.Check(ref lexemsIterator,isTerm,
				              "Expression starts normally: "+lexems[lexemsIterator].Command,
				              "Invalid expression",Checker.IncrementMode.IncrementOnce);
			Out.Log(Out.State.LogVerbose,"!!Current lexem: "+lexems[lexemsIterator].Command);


			while (true)
			{
				Out.Log(Out.State.LogVerbose,"Continue analyze expression");
				if (lexems[lexemsIterator].Command == "-" || lexems[lexemsIterator].Command == "+")
				{
					Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].Command);
					Out.Log(Out.State.LogVerbose,"After operator lexem is "+lexems[lexemsIterator+1].Command);
					lexemsIterator++;
					Checker.Check(ref lexemsIterator,isTerm,
					              "Next term accepted: "+lexems[lexemsIterator].Command,
					              "Symbol "+lexems[lexemsIterator].Command+" is invalid",
					              Checker.IncrementMode.IncrementOnce);
				}
				else
				{
					Out.Log(Out.State.LogVerbose,"+-Current lexem: "+lexems[lexemsIterator].Command);
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

			Out.Log(Out.State.LogVerbose,"Yeah. Let see what's next: "+lexems[lexemsIterator].Command+
			                  "And after that I see "+lexems[lexemsIterator+1].Command);
			if (lexems[lexemsIterator+1].Command == "*" || lexems[lexemsIterator+1].Command == "/")
			{
				Out.Log(Out.State.LogVerbose,"Using */. Increment operator");
				lexemsIterator++;
				while (true)
				{
					if (lexems[lexemsIterator].Command == "*" || lexems[lexemsIterator].Command == "/")
					{
						Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].Command);
						lexemsIterator++;
						Checker.Check(ref lexemsIterator,isMultiplier,
						              "Next multiplier accepted: "+lexems[lexemsIterator].Command,
						              "Symbol "+lexems[lexemsIterator].Command+" is invalid",
						              Checker.IncrementMode.IncrementOnce);
					}
					else
					{
						Out.Log(Out.State.LogVerbose,"*/Current lexem: "+lexems[lexemsIterator].Command);
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

			Out.Log(Out.State.LogVerbose,"Yeah. Let see what's next: "+lexems[lexemsIterator].Command+
			                  "And after that I see "+lexems[lexemsIterator+1].Command);
			if (lexems[lexemsIterator+1].Command == "^")
			{
				Out.Log(Out.State.LogVerbose,"Using ^. Increment iterator");
				lexemsIterator++;
				while (true)
				{
					if (lexems[lexemsIterator].Command == "^")
					{
						Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].Command);
						lexemsIterator++;
						
						Checker.Check(ref lexemsIterator,isExprResponce,
						              "Next expressionResponse accepted: "+lexems[lexemsIterator].Command,
						              "Symbol "+lexems[lexemsIterator].Command+" is invalid",
						              Checker.IncrementMode.IncrementOnce);
					}
					else
					{
						Out.Log(Out.State.LogVerbose,"^Current lexem: "+lexems[lexemsIterator].Command);
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
			while (lexems[lexemsIterator].Command == " " || lexems[lexemsIterator].Command == "") lexemsIterator++;
			if (lexems[lexemsIterator].isID())
			{
				Out.Log(Out.State.LogVerbose,"Founded exprResponce is ID: "+lexems[lexemsIterator].Command);
			}
			else if (lexems[lexemsIterator].isCONST())
			{
				Out.Log(Out.State.LogVerbose,"Founded exprResponce is CONST: "+lexems[lexemsIterator].Command);
			}
			else if (lexems[lexemsIterator].Command == "(")
			{
				Out.Log(Out.State.LogVerbose,"() Open Scobes ()");
				lexemsIterator++;
				AnalyzeExpression(ref lexemsIterator);
				Out.Log(Out.State.LogVerbose,"() Return from block with lexem "+lexems[lexemsIterator].Command);
				Out.Log(Out.State.LogVerbose,"() Close Scobes ()");
			}
			else
			{
				Out.Log(Out.State.LogVerbose,"Wrong lexem: "+lexems[lexemsIterator].Command);
				lexemsIterator = oldIterator;
				return false;
			}
			return true;
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool AnalyzeLogicalExpression(ref int lexemsIterator)
		{
			Out.Log(Out.State.LogVerbose,"Start analyze logical expression");

			Out.Log(Out.State.LogVerbose,"!Current lexem: "+lexems[lexemsIterator].Command);
			Checker.Check(ref lexemsIterator,isLogicalExpressionLevel1,
			              "Logical expression starts normally: "+lexems[lexemsIterator].Command,
			              "Invalid logical expression");
			Out.Log(Out.State.LogVerbose,"!!Current lexem: "+lexems[lexemsIterator].Command);

			while (true)
			{
				Out.Log(Out.State.LogVerbose,"Continue analyze expression");
				if (lexems[lexemsIterator].Command == "or")
				{
					Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].Command);
					Out.Log(Out.State.LogVerbose,"After operator lexem is "+lexems[lexemsIterator+1].Command);
					lexemsIterator++;
					Checker.Check(ref lexemsIterator,isLogicalExpressionLevel1,
					              "Next LogicalExpressionLevel1 accepted: "+lexems[lexemsIterator].Command+"\n",
					              "Symbol "+lexems[lexemsIterator].Command+" is invalid");
				}
				else
				{
					Out.Log(Out.State.LogVerbose,"||Current lexem: "+lexems[lexemsIterator].Command);
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

			Out.Log(Out.State.LogVerbose,"Yeah. Let see what's next: "+lexems[lexemsIterator].Command+
			                  "And after that I see "+lexems[lexemsIterator+1].Command);
			if (lexems[lexemsIterator].Command == "and")
			{
				Out.Log(Out.State.LogVerbose,"I see you Use && after that. So I will inc iterator for you");
				//lexemsIterator++;
				while (true)
				{
					if (lexems[lexemsIterator].Command == "and")
					{
						Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].Command);
						lexemsIterator++;
						Checker.Check(ref lexemsIterator,isLogicalExpressionLevel2,
						              "Next LogicalExpressionLevel2 accepted: "+lexems[lexemsIterator].Command+"\n",
						              "Symbol "+lexems[lexemsIterator].Command+" is invalid",
						              Checker.IncrementMode.IncrementOnce);
					}
					else
					{
						Out.Log(Out.State.LogVerbose,"&&Current lexem: "+lexems[lexemsIterator].Command);
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
			while (lexems[lexemsIterator].Command == "!")
			{
				Out.Log(Out.State.LogVerbose,"Using '!'");
				lexemsIterator++;
			}

			if (lexems[lexemsIterator].Command == "[")
			{
				Out.Log(Out.State.LogVerbose,"[] Open scobes []");
				lexemsIterator++;
				AnalyzeLogicalExpression(ref lexemsIterator);
				Out.Log(Out.State.LogVerbose,"[] Return from block with lexem "+lexems[lexemsIterator].Command);
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
			Out.Log(Out.State.LogVerbose,"....... 1.After analyze expression next lexem is: "+lexems[lexemsIterator].Command);
			if (connotials.Contains(lexems[lexemsIterator].Command)) 
			{
				Out.Log(Out.State.LogVerbose,"Match connotial: "+lexems[lexemsIterator].Command);
				lexemsIterator++;
			}
			else
			{
				throw new LexemException(lexems[lexemsIterator].LineNumber,
				                                "Invalid Connotial "+lexems[lexemsIterator].Command);
				
			}
			AnalyzeExpression(ref lexemsIterator);
			Out.Log(Out.State.LogVerbose,"....... 2. After analyze expression next lexem is: "+lexems[lexemsIterator].Command);
			return true;
		}
	}
}
