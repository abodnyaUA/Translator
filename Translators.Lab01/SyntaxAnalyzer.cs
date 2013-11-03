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


        List<Lexem> lexems;
        List<string> IDs;
        List<string> CONSTs;
        List<string> lexemsDict;
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
            if (lexems[lexemsIterator].key == 34)
            {
                Console.WriteLine("Name "+lexems[lexemsIterator].command+" accept");
                lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". Invalid @interface name");
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
        private bool AnalyzeInterface(ref int lexemsIterator)
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
                if (lexems[lexemsIterator].key == 34)
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
                    if (lexems[lexemsIterator].key == 31 && lexems[lexemsIterator + 1].key == 34) //,id
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
        private bool AnalyzeImplementation(ref int lexemsIterator)
        {
            if (lexems[lexemsIterator].key == 2) //empty implementation
            {
                return true;
            }
            do
            {
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
                else if (lexems[lexemsIterator].key == 34) //setter
                {
                }
                else
                {
                    Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                        ". Invalid operator");
                    throw error;
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
            }
            while (lexems[lexemsIterator].key != 2);
            //lexemsIterator++;
            return true;
        }

        private bool AnalyzeIO(ref int lexemsIterator)
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
                    ". 'Read' functions must start with '('");
                throw error;
            }
            
            /* ID */
            if (lexems[lexemsIterator].key == 34) //id
            {
                Console.WriteLine("Confirm ID "+lexems[lexemsIterator].command);
                lexemsIterator++;
            }
            else
            {
                Exception error = new Exception("Error! Line: " + lexems[lexemsIterator].lineNumber +
                    ". 'Read' functions can't be used wiyhout arguments");
                throw error;
            }

            /* See next IDs */
            while (lexems[lexemsIterator].key != 17) // )
            {
                if (lexems[lexemsIterator].key == 31 && lexems[lexemsIterator + 1].key == 34) //,id
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
        
        private bool AnalyzeLogicalExpression(ref int lexemsIterator)
        {
            List<Lexem> logicalLexems = new List<Lexem>();
            while (lexems[lexemsIterator + 1].key != 6)
            {
                logicalLexems.Add(lexems[lexemsIterator]);
                lexemsIterator++;
            }
            
            return true;
        }

        private void AnalyzeLogicalExpressionRecursive(List<Lexem> logicalLexems)
        {
  
            /* Try find ( ) */
            /* vsdvs ( sfcs ( s sf ( s ) s sss ) s s ) */
            bool findOpen = false;
            bool findClose = false;
            for (int i = 0; i < logicalLexems.Count; i++)
            {
                if (logicalLexems[i].command == "(")
                {
                    findOpen = true;
                    for (int j = logicalLexems.Count - 1; j > i; j--)
                    {
                        if (logicalLexems[i].command == ")")
                        {
                            findClose = true;
                            break;
                        }
                    }
                    if (findClose == true)
                    {
                        Console.Write("Parse lexemlist: ");
                    }
                }
            }
            return;
        }
    }
}
