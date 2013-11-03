using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translators.Lab01
{
    class Compiler
    {
        private static Compiler _sharedCompiler = null;
        public static Compiler sharedCompiler
        {
            get
            {
                if (_sharedCompiler == null) _sharedCompiler = new Compiler();
                return _sharedCompiler;
            }
        }
        private Compiler()
        {
        }

        public void CompileFile(string path)
        {
            Console.WriteLine("======== Parse code ========");
            List<List<string>> parsed = Parser.sharedParser.ParseFile(path);
            try
            {
                Console.WriteLine("======== Lexem Analyzer ========");
                LexemAnalyzer.sharedAnalyzer.AnalyzeWithDoubleList(parsed);
                LexemAnalyzer.sharedAnalyzer.outputTables();

                Console.WriteLine("======== Syntax Analyzer ========");
                SyntaxAnalyzer.sharedAnalyzer.AnalyzeLexems();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
