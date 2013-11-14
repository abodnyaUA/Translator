using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Translators.Lab01
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
		
		private void Check(ref int lexemsIterator, int key, string success, string failure)
		{
			Check(ref lexemsIterator,key,success,failure,1,Out.State.LogVerbose);
		}
		private void Check(ref int lexemsIterator, int key, string success, string failure, Out.State logState)
		{
			Check(ref lexemsIterator,key,success,failure,1,logState);
		}
		private void Check(ref int lexemsIterator, int key, string success, string failure, int incrementValue)
		{
			Check(ref lexemsIterator,key,success,failure,incrementValue,Out.State.LogVerbose);
		}
		private void Check(ref int lexemsIterator, int key, string success, string failure, int incrementValue, Out.State logState)
		{
			if (lexems[lexemsIterator].key == key)
			{
				Out.Log(logState,success);
				lexemsIterator+=incrementValue;
			}
			else
			{
				LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,failure);
				throw error;
			}
		}

		public delegate bool CheckerFunc <T>(ref T obj);
		
		private void Check(ref int lexemsIterator, CheckerFunc <int> Func, string success, string failure)
		{
			Check(ref lexemsIterator,Func,success,failure,Out.State.LogVerbose);
		}
		private void Check(ref int lexemsIterator, CheckerFunc <int> Func, string success, string failure, Out.State logState)
		{
			if (Func(ref lexemsIterator))
			{
				Out.Log(Out.State.LogInfo,success);
			}
			else
			{
				LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,failure);
				throw error;
			}
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
			Check(ref lexemsIterator,0,
			      "'Interface' accept",
			      "Application should start with '@interface'");
            
			/* ID */
			Check(ref lexemsIterator,lexemsDict.Count-2,
			      "Name "+lexems[lexemsIterator].command+" accept",
			      "Invalid @interface name: "+lexems[lexemsIterator].command + " ("+lexems[lexemsIterator].key+")");

			/* ENTER */
			Check(ref lexemsIterator,6,
			      "ENTER accepted",
			      "Not found ENTER after @interface name");
                
            /* PARSE Interface */
			Check(ref lexemsIterator,AnalyzeInterface,
			      "Definitions [Interface block] accept",
			      "Invalid declarations");

			/* @implementation */
			Check(ref lexemsIterator,1,
			      "'Implementation' accept",
			      "Not found '@implementation' word");

			/* ENTER */
			Check(ref lexemsIterator,6,
			      "ENTER accepted",
			      "Not found ENTER after @implementation name");

			/* Parse Implementation */
			Check(ref lexemsIterator,AnalyzeImplementation,
			          "Operators [Implementation block] accept",
			          "Invalid operators block");
            
			/* @end */
			Check(ref lexemsIterator,2,
			      "End accept. Success syntax analyze.",
			      "Invalide declaration @end",0);
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
                if (lexems[lexemsIterator].key == 3)
                {
					Out.Log(Out.State.LogVerbose,"Confirm type int");
                    lexemsIterator++;
                }
                else
                {
                    LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
                        "Invalid declaration construction. Need to set type");
                    throw error;
                }

                /* ID */
				if (lexems[lexemsIterator].key == lexemsDict.Count-2)
                {
					Out.Log(Out.State.LogInfo,"Confirm ID "+lexems[lexemsIterator].command);
                    lexemsIterator++;
                }
                else
                {
                    LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
                        "Empty declaration");
                    throw error;
                }

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
                if (lexems[lexemsIterator].key == 6) // ENTER
                {
                    lexemsIterator++;
                }
                else
                {
                    LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
                        "Missed ENTER");
                    throw error;
                }
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
				if (lexems[lexemsIterator].key == 6) // ENTER
				{
					lexemsIterator++;
				}
				else
				{
					LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
					                                "Missed ENTER. Find operator: "+lexems[lexemsIterator].command);
					throw error;
				}
			}
			while (lexems[lexemsIterator].key != endLexemKey);
		}

		public void AnalyzeOperator(ref int lexemsIterator)
		{
			Out.Log(Out.State.LogDebug,"Current lexem: "+lexems[lexemsIterator].command);

			if (lexems[lexemsIterator].key == 4) //input
			{
				AnalyzeIO(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Input accepted");
			}
			else if (lexems[lexemsIterator].key == 5) //output
			{
				AnalyzeIO(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Output accepted");
			}
			else if (lexems[lexemsIterator].key == 7) //for
			{
				AnalyzeCycle(ref lexemsIterator);
				Out.Log(Out.State.LogInfo,"Cycle 'for' accepted");
			}
			else if (lexems[lexemsIterator].key == 11) //if
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
            if (lexems[lexemsIterator].key == 16) // (
            {
				Out.Log(Out.State.LogVerbose,"Confirm '('");
                lexemsIterator++;
            }
            else
            {
                LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
                    "IO functions must start with '('");
                throw error;
            }
            
            /* ID */
			if (lexems[lexemsIterator].key == lexemsDict.Count-2) //id
            {
				Out.Log(Out.State.LogVerbose,"Confirm ID "+lexems[lexemsIterator].command);
                lexemsIterator++;
            }
            else
            {
                LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
				                                "IO functions can't be used wiyhout arguments");
                throw error;
            }

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

		public void AnalyzeSetter(ref int lexemsIterator)
		{
			lexemsIterator++;

			if (lexems[lexemsIterator].command == "=") //id
			{
				Out.Log(Out.State.LogVerbose,"'=' accepted");
				lexemsIterator++;
			}
			else
			{
				LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
				                                "Invalid setter. Missed '='");
				throw error;
			}
			AnalyzeExpression(ref lexemsIterator);
		}

		public void AnalyzeCondition(ref int lexemsIterator)
		{
			lexemsIterator++;

			AnalyzeLogicalExpression(ref lexemsIterator,6);
			Out.Log(Out.State.LogVerbose,"ACCEPT LOGICAL EXPRESSION. NEXT LEXEM IS: "+lexems[lexemsIterator].command);
			lexemsIterator++;
			AnalyzeAction(ref lexemsIterator);
			Out.Log(Out.State.LogVerbose,"ACCEPT ACTION. NEXT LEXEM IS: "+lexems[lexemsIterator].command);
			lexemsIterator++;

			if (lexems[lexemsIterator].command == "else")
			{
				Out.Log(Out.State.LogVerbose,"Passed ELSE");
				lexemsIterator += 2;
			}
			else
			{
				LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
				                                "Missed 'else'");
				throw error;
			}
			AnalyzeAction(ref lexemsIterator);
			Out.Log(Out.State.LogVerbose,"ACCEPT ACTION. NEXT LEXEM IS: "+lexems[lexemsIterator].command);
			lexemsIterator++;
			
			if (lexems[lexemsIterator].command == "endif")
			{
				Out.Log(Out.State.LogVerbose,"Passed ENDIF");
				lexemsIterator++;
			}
			else
			{
				LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
				                                "Missed 'endif'");
				throw error;
			}
		}

		public void AnalyzeCycle(ref int lexemsIterator)
		{
			lexemsIterator++;
			AnalyzeSetter(ref lexemsIterator);
			Out.Log(Out.State.LogVerbose,"SETTER PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].command);
			if (lexems[lexemsIterator].command == "to")
			{
				Out.Log(Out.State.LogVerbose,"'to' PASSED");
				lexemsIterator++;
			}
			AnalyzeExpression(ref lexemsIterator);
			
			Out.Log(Out.State.LogVerbose,"EXPRESSION PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].command);
			if (lexems[lexemsIterator].command == "step")
			{
				Out.Log(Out.State.LogVerbose,"'step' PASSED");
				lexemsIterator++;
			}
			AnalyzeExpression(ref lexemsIterator);
			Out.Log(Out.State.LogVerbose,"EXPRESSION PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].command);
			lexemsIterator++;
			AnalyzeAction(ref lexemsIterator);
			Out.Log(Out.State.LogVerbose,"ACTION PASSED. CURRENT LEXEM IS "+lexems[lexemsIterator].command);
			lexemsIterator++;
			if (lexems[lexemsIterator].command == "next")
			{
				Out.Log(Out.State.LogVerbose,"'next' PASSED");
				lexemsIterator++;
			}
			//
		}

		public void AnalyzeAction(ref int lexemsIterator)
		{
			if (lexems[lexemsIterator].command == "{")
			{
				lexemsIterator += 2; // {\n

				AnalyzeOperatorsBlock(ref lexemsIterator,15);

				if (lexems[lexemsIterator].command == "}")
				{
					lexemsIterator++; // }\n
				}
				else
				{
					LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
					                                "Missed '}'");
					throw error;
				}
			}
			else
			{
				AnalyzeOperator(ref lexemsIterator);
			}
		}
        
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public void AnalyzeExpression(ref int lexemsIterator)
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
			else if (isTerm(ref lexemsIterator)) 
			{
				Out.Log(Out.State.LogVerbose,"Expression starts normally: "+lexems[lexemsIterator].command);
				lexemsIterator ++;
			} 
			else
			{
				LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
				                                "Invalid expression");
				throw error;
			}
			Out.Log(Out.State.LogVerbose,"!!Current lexem: "+lexems[lexemsIterator].command);


			while (true)
			{
				Out.Log(Out.State.LogVerbose,"Continue analyze expression");
				if (lexems[lexemsIterator].command == "-" || lexems[lexemsIterator].command == "+")
				{
					Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].command);
					Out.Log(Out.State.LogVerbose,"After operator lexem is "+lexems[lexemsIterator+1].command);
					lexemsIterator++;
					if (isTerm(ref lexemsIterator))
					{
						Out.Log(Out.State.LogVerbose,"Next term accepted: "+lexems[lexemsIterator].command+"\n");
						lexemsIterator++;
					}
					else
					{
						LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
						                                "Symbol "+lexems[lexemsIterator].command+" is invalid");
						throw error;
					}
				}
				else
				{
					Out.Log(Out.State.LogVerbose,"+-Current lexem: "+lexems[lexemsIterator].command);
					break;
				}
			}
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
						if (isMultiplier(ref lexemsIterator))
						{
							Out.Log(Out.State.LogVerbose,"Next multiplier accepted: "+lexems[lexemsIterator].command);
							lexemsIterator++;
						}
						else
						{
							LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
							                                "Symbol "+lexems[lexemsIterator].command+" is invalid");
							throw error;
						}
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
						if (isExprResponce(ref lexemsIterator))
						{
							Out.Log(Out.State.LogVerbose,"Next expressionResponse accepted: "+lexems[lexemsIterator].command);
							lexemsIterator++;
						}
						else
						{
							LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
							                                "Symbol "+lexems[lexemsIterator].command+" is invalid");
							throw error;
						}
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
		public void AnalyzeLogicalExpression(ref int lexemsIterator, int endLexemKey)
		{
			Out.Log(Out.State.LogVerbose,"Start analyze logical expression");

			Out.Log(Out.State.LogVerbose,"!Current lexem: "+lexems[lexemsIterator].command);
			if (isLogicalExpressionLevel1(ref lexemsIterator, endLexemKey)) 
			{
				Out.Log(Out.State.LogVerbose,"Logical expression starts normally: "+lexems[lexemsIterator].command);
//				if (lexems[lexemsIterator].key != endLexemKey)
//					lexemsIterator ++;
			} 
			else
			{
				LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
				                                "Invalid logical expression");
				throw error;
			}
			Out.Log(Out.State.LogVerbose,"!!Current lexem: "+lexems[lexemsIterator].command);

            //if (lexems[lexemsIterator].command == "or")
			while (true)
			{
				Out.Log(Out.State.LogVerbose,"Continue analyze expression");
				if (lexems[lexemsIterator].command == "or")
				{
					Out.Log(Out.State.LogVerbose,"Match operator "+lexems[lexemsIterator].command);
					Out.Log(Out.State.LogVerbose,"After operator lexem is "+lexems[lexemsIterator+1].command);
					lexemsIterator++;
					if (isLogicalExpressionLevel1(ref lexemsIterator, endLexemKey))
					{
						Out.Log(Out.State.LogVerbose,"Next LogicalExpressionLevel1 accepted: "+lexems[lexemsIterator].command+"\n");

					}
					else
					{
						LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
						                                "Symbol "+lexems[lexemsIterator].command+" is invalid");
						throw error;
					}
				}
				else
				{
					Out.Log(Out.State.LogVerbose,"||Current lexem: "+lexems[lexemsIterator].command);
					break;
				}
			}
		}

		public bool isLogicalExpressionLevel1(ref int lexemsIterator, int endLexemKey)
		{
			int oldIterator = lexemsIterator;
			if (isLogicalExpressionLevel2(ref lexemsIterator, endLexemKey))
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
						if (isLogicalExpressionLevel2(ref lexemsIterator, endLexemKey))
						{
							Out.Log(Out.State.LogVerbose,"Next LogicalExpressionLevel2 accepted: "+lexems[lexemsIterator].command);
							lexemsIterator++;
						}
						else
						{
							LexemException error = new LexemException(lexems[lexemsIterator].lineNumber,
							                                "Symbol "+lexems[lexemsIterator].command+" is invalid");
							throw error;
						}
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

		
		public bool isLogicalExpressionLevel2(ref int lexemsIterator, int endLexemKey)
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
				AnalyzeLogicalExpression(ref lexemsIterator, endLexemKey);
				Out.Log(Out.State.LogVerbose,"[] Return from block with lexem "+lexems[lexemsIterator].command);
				Out.Log(Out.State.LogVerbose,"[] Close scobes []");
				lexemsIterator++;
			}
			else
			{
				return isNormalRelation(ref lexemsIterator, endLexemKey);
			}

			return true;
		}

		public bool isNormalRelation(ref int lexemsIterator, int endLexemKey)
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
