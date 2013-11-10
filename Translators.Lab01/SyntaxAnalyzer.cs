﻿using System;
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
            if (lexems[lexemsIterator].key == 0)
            {
                Console.WriteLine("'Interface' accept");
                lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". Application should start with '@interface'");
                throw error;
            }
            
            /* ID */
            if (lexems[lexemsIterator].key == lexemsDict.Count-2)
            {
                Console.WriteLine("Name "+lexems[lexemsIterator].command+" accept");
                lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". Invalid @interface name: "+lexems[lexemsIterator].command + " ("+lexems[lexemsIterator].key+")");
                throw error;
            }

            /* ENTER */
            if (lexems[lexemsIterator].key == 6)
            {
                Console.WriteLine("ENTER accepted");
                lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". Not found ENTER after @interface name");
                throw error;
            }
                
            /* PARSE Interface */
            if (AnalyzeInterface(ref lexemsIterator))
            {
                Console.WriteLine("Definitions [Interface block] accept");
                //lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". Invalid declarations");
                throw error;
            }

            /* @implementation */
            if (lexems[lexemsIterator].key == 1)
            {
                Console.WriteLine("'Implementation' accept");
                lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". Not found '@implementation' word");
                throw error;
            }

            /* ENTER */
            if (lexems[lexemsIterator].key == 6)
            {
                Console.WriteLine("ENTER accepted");
                lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". Not found ENTER after @implementation");
                throw error;
            }

            /* Parse Implementation */
            if (AnalyzeImplementation(ref lexemsIterator))
            {
                Console.WriteLine("Operators [Implementation block] accept");
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". Invalide by parsing implementation");
                throw error;
            }
            
            /* @end */
            if (lexems[lexemsIterator].key == 2)
            {
                Console.WriteLine("End accept. Success syntax analyze.");
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber + 
                    ". Invalide declaration @end");
                throw error;
            }       
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
                    Console.WriteLine("Confirm type int");
                    lexemsIterator++;
                }
                else
                {
                    Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                        ". Invalid declaration construction. Need to set type");
                    throw error;
                }

                /* ID */
				if (lexems[lexemsIterator].key == lexemsDict.Count-2)
                {
                    Console.WriteLine("Confirm ID "+lexems[lexemsIterator].command);
                    lexemsIterator++;
                }
                else
                {
                    Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                        ". Empty declaration");
                    throw error;
                }

                /* See next IDs */
                while (lexems[lexemsIterator].key != 6) // ENTER
                {
					if (lexems[lexemsIterator].key == 31 && lexems[lexemsIterator + 1].key == lexemsDict.Count-2) //,id
                    {
                        Console.WriteLine("Confirm ID " + lexems[lexemsIterator+1].command);
                        lexemsIterator += 2;
                    }
                    else
                    {
                        Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                            ". Invalid declaration");
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
                    Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                        ". Missed ENTER");
                    throw error;
                }
            } while (lexems[lexemsIterator].key != 1); /* @implementation */
            //lexemsIterator++;
            return true;
        }
		public bool AnalyzeImplementation(ref int lexemsIterator)
        {
            if (lexems[lexemsIterator].key == 2) //empty implementation
            {
                return true;
            }
            do
            {
				
				Console.WriteLine("Previous lexem: "+lexems[lexemsIterator-1].command);
				Console.WriteLine("Current lexem: "+lexems[lexemsIterator].command);
				AnalyzeOperator(ref lexemsIterator);
                //Finishing
                if (lexems[lexemsIterator].key == 6) // ENTER
                {
                    lexemsIterator++;
                }
                else
                {
                    Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                        ". Missed ENTER. Find operator: "+lexems[lexemsIterator].command);
                    throw error;
                }
            }
            while (lexems[lexemsIterator].key != 2);
            //lexemsIterator++;
            return true;
        }

		public void AnalyzeOperator(ref int lexemsIterator)
		{
			Console.WriteLine("Current lexem: "+lexems[lexemsIterator].command);

			if (lexems[lexemsIterator].key == 4) //input
			{
				AnalyzeIO(ref lexemsIterator);
			}
			else if (lexems[lexemsIterator].key == 5) //output
			{
				AnalyzeIO(ref lexemsIterator);
			}
			else if (lexems[lexemsIterator].key == 7) //for
			{
			}
			else if (lexems[lexemsIterator].key == 11) //if
			{
			}
			else if (lexems[lexemsIterator].key == lexemsDict.Count-2) //setter
			{
				AnalyzeSetter(ref lexemsIterator);
			}
			else
			{
				Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
				                                ". Invalid operator");
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
                Console.WriteLine("Confirm '('");
                lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". IO functions must start with '('");
                throw error;
            }
            
            /* ID */
			if (lexems[lexemsIterator].key == lexemsDict.Count-2) //id
            {
                Console.WriteLine("Confirm ID "+lexems[lexemsIterator].command);
                lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
				                                ". IO functions can't be used wiyhout arguments");
                throw error;
            }

            /* See next IDs */
            while (lexems[lexemsIterator].key != 17) // )
            {
				if (lexems[lexemsIterator].key == 31 && lexems[lexemsIterator + 1].key == lexemsDict.Count-2) //,id
                {
                    Console.WriteLine("Confirm ID "+lexems[lexemsIterator+1].command);
                    lexemsIterator += 2;
                }
                else
                {
                    Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                        ". Invalid using the variables in function 'input'");
                    throw error;
                }
            }
            Console.WriteLine("Confirm ')'");
                
            
            lexemsIterator++;
            return true;
        }

		public void AnalyzeSetter(ref int lexemsIterator)
		{
			lexemsIterator++;

			if (lexems[lexemsIterator].command == "=") //id
			{
				Console.WriteLine("'=' accepted");
				lexemsIterator++;
			}
			else
			{
				Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
				                                ". Invalid setter. Missed '='");
				throw error;
			}
			AnalyzeExpression(ref lexemsIterator);
		}
        
		public void AnalyzeExpression(ref int lexemsIterator)
		{
			Console.WriteLine("Start analyze expression");
			HashSet<int> expressionSet = new HashSet<int>()
			{ 
				lexemsDict.Count-2, //const 
				lexemsDict.Count-1, //id
				16, //(
				17, //)
				26, //+
				27, //-
				28, // /
				29, // *
				30 // ^ 
			};

			Console.WriteLine("!Current lexem: "+lexems[lexemsIterator].command);
			if (lexems[lexemsIterator].command == "-") 
			{
				lexemsIterator++;
				if (isTerm(ref lexemsIterator))
				{
					Console.WriteLine("Expression starts with '-': "+lexems[lexemsIterator].command);
					lexemsIterator ++;
				}
				else
				{
					Console.WriteLine("Expression doesn't start with '-': "+lexems[lexemsIterator].command);
					lexemsIterator--;
				}
			} 
			else if (isTerm(ref lexemsIterator)) 
			{
				Console.WriteLine("Expression starts normally: "+lexems[lexemsIterator].command);
				lexemsIterator ++;
			} 
			else
			{
				Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
				                                ". Invalid expression");
				throw error;
			}
			Console.WriteLine("!!Current lexem: "+lexems[lexemsIterator].command);


			while (expressionSet.Contains(lexems[lexemsIterator].key))
			{
				Console.WriteLine("Continue analyze expression");
				if (lexems[lexemsIterator].command == "-" || lexems[lexemsIterator].command == "+")
				{
					Console.WriteLine("Match operator "+lexems[lexemsIterator].command);
					Console.WriteLine("After operator lexem is "+lexems[lexemsIterator+1].command);
					lexemsIterator++;
					if (isTerm(ref lexemsIterator))
					{
						Console.WriteLine("Next term accepted: "+lexems[lexemsIterator].command+"\n");
						lexemsIterator++;
					}
					else
					{
						Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
						                                ". Symbol "+lexems[lexemsIterator].command+" is invalid");
						throw error;
					}
				}
				else
				{
					Console.WriteLine("+-Current lexem: "+lexems[lexemsIterator].command);
					break;
				}
			}
		}
		public bool isTerm(ref int lexemsIterator)
		{
			int oldIterator = lexemsIterator;
			if (isMultiplier(ref lexemsIterator))
			{
				Console.WriteLine("First term is multiplier. All OK");
			}
			else
			{
				Console.WriteLine("First term isn't multiplier. All BAD");
				lexemsIterator = oldIterator;
				return false;
			}

			HashSet<int> termSet = new HashSet<int>()
			{ 
				lexemsDict.Count-2, //const 
				lexemsDict.Count-1, //id
				16, //(
				17, //)
				28, // /
				29, // *
				30 // ^ 
			};
			Console.WriteLine("Yeah. Let see what's next: "+lexems[lexemsIterator].command+
			                  ". And after that I see "+lexems[lexemsIterator+1].command);
			if (lexems[lexemsIterator+1].command == "*" || lexems[lexemsIterator+1].command == "/")
			{
				Console.WriteLine("I see you Use */ after that. So I will inc iterator for you");
				lexemsIterator++;
				while (termSet.Contains(lexems[lexemsIterator].key))
				{
					if (lexems[lexemsIterator].command == "*" || lexems[lexemsIterator].command == "/")
					{
						Console.WriteLine("Match operator "+lexems[lexemsIterator].command);
						lexemsIterator++;
						if (isMultiplier(ref lexemsIterator))
						{
							Console.WriteLine("Next multiplier accepted: "+lexems[lexemsIterator].command);
							lexemsIterator++;
						}
						else
						{
							Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
							                                ". Symbol "+lexems[lexemsIterator].command+" is invalid");
							throw error;
						}
					}
					else
					{
						Console.WriteLine("*/Current lexem: "+lexems[lexemsIterator].command);
						break;
					}
				}
				lexemsIterator--;
			}
			else
			{
				Console.WriteLine("You don't use /* ? :'(. I will cry all night");
			}
			return true;
		}
		public bool isMultiplier(ref int lexemsIterator)
		{
			int oldIterator = lexemsIterator;
			if (isExprResponce(ref lexemsIterator))
			{
				Console.WriteLine("First multiplier is exprResponce. All OK");
			}
			else
			{
				Console.WriteLine("First multiplier isn't exprResponce. All BAD");
				lexemsIterator = oldIterator;
				return false;
			}

			
			HashSet<int> multSet = new HashSet<int>()
			{ 
				lexemsDict.Count-2, //const 
				lexemsDict.Count-1, //id
				16, //(
				17, //)
				30 // ^ 
			};
			Console.WriteLine("Yeah. Let see what's next: "+lexems[lexemsIterator].command+
			                  ". And after that I see "+lexems[lexemsIterator+1].command);
			if (lexems[lexemsIterator+1].command == "^")
			{
				Console.WriteLine("I see you Use ^ after that. So I will inc iterator for you");
				lexemsIterator++;
				while (multSet.Contains(lexems[lexemsIterator].key))
				{
					if (lexems[lexemsIterator].command == "^")
					{
						Console.WriteLine("Match operator "+lexems[lexemsIterator].command);
						lexemsIterator++;
						if (isExprResponce(ref lexemsIterator))
						{
							Console.WriteLine("Next expressionResponse accepted: "+lexems[lexemsIterator].command);
							lexemsIterator++;
						}
						else
						{
							Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
							                                ". Symbol "+lexems[lexemsIterator].command+" is invalid");
							throw error;
						}
					}
					else
					{
						if (lexems[lexemsIterator].command == ")")
						{
							#warning BADCODE
							Console.WriteLine("I now it's bad hack and I promise, I will write good code. But now promise me");
							break;
						}

						Console.WriteLine("^Current lexem: "+lexems[lexemsIterator].command);
					}
				}
				lexemsIterator--;
			}
			else
			{
				Console.WriteLine("You don't use ^ ? >.<. I will never speak with you");
			}

			return true;
		}

		int openScobesCount;
		public bool isExprResponce(ref int lexemsIterator)
		{
			int oldIterator = lexemsIterator;
			while (lexems[lexemsIterator].command == " " || lexems[lexemsIterator].command == "") lexemsIterator++;
			if (lexems[lexemsIterator].key == lexemsDict.Count-2)
			{
				Console.WriteLine("Founded exprResponce is ID: "+lexems[lexemsIterator].command);
			}
			else if (lexems[lexemsIterator].key == lexemsDict.Count-1)
			{
				Console.WriteLine("Founded exprResponce is CONST: "+lexems[lexemsIterator].command);
			}
			else if (lexems[lexemsIterator].command == "(")
			{
				Console.WriteLine("!!!!!!!!!!!I see you want play game. Open scobes!!!!!!");
				openScobesCount++;
				lexemsIterator++;
				AnalyzeExpression(ref lexemsIterator);
				Console.WriteLine("===== I here again. And lexem is "+lexems[lexemsIterator].command);
//				if (lexems[lexemsIterator].command == ")")
//				{
//					Console.WriteLine("Close it manually");
//					lexemsIterator++;
//				}
				Console.WriteLine("===========I see you finish playing game. Close scobes!!!!!!");
				//lexemsIterator++;
				Console.WriteLine("===========Next lexem is "+lexems[lexemsIterator].command);
				openScobesCount--;
			}
			else if (lexems[lexemsIterator].command == ")")
			{
				//lexemsIterator++;
				Console.WriteLine("!!!!!!!!!!!I see you finish playing game. Close scobes!!!!!!");
				openScobesCount--;
			}
			else
			{
				Console.WriteLine("HEY! What is wrong with you?! I see here: "+lexems[lexemsIterator].command);
				lexemsIterator = oldIterator;
				return false;
			}
			return true;
		}
    }
}
