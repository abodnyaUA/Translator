using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translators
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
//			
        }

        public void CompileFile(string path)
        {
			Out.Log(Out.State.LogInfo,"======== Parse code ========");
            List<List<string>> parsed = Parser.sharedParser.ParseFile(path);
            try
            {
				Out.Log(Out.State.LogInfo,"======== Lexem Analyzer ========");
                LexemAnalyzer.sharedAnalyzer.AnalyzeWithDoubleList(parsed);
                LexemAnalyzer.sharedAnalyzer.outputTables();

				Out.Log(Out.State.LogInfo,"======== Syntax Analyzer ========");
                SyntaxAnalyzer.sharedAnalyzer.AnalyzeLexems();
            }
            catch (LexemException error)
            {
				Out.Log(Out.State.LogInfo,"\n"+error.UserInfo);
            }
        }
    }
}
