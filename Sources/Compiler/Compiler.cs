using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translators
{
	public enum CompileMode 
	{
		NormalAnalyze,
		PolizConverter
	}

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
		public CompileMode AnalyzeMode;		
		public ISyntaxAnalyzer SyntaxAnalyzer;

        private Compiler() 
		{
			SyntaxAnalyzer = SyntaxAnalyzerWithTable.sharedAnalyzer;
		}

        public void CompileFile(string path)
		{
			AnalyzeMode = CompileMode.NormalAnalyze;
			Analyze(path);
        }
		public void AnalyzeForPoliz(string path)
		{
			AnalyzeMode = CompileMode.PolizConverter;
			Analyze(path);
		}

		private void Analyze(string path)
		{
			Program.window.ProgressBar.Adjustment.Value = 0;
			Program.window.Console.Buffer.Text = "";
			Out.Log(Out.State.LogInfo,"======== Parse code ========");
			List<List<string>> parsed = Parser.sharedParser.ParseFile(path);
			Program.window.ProgressBar.Adjustment.Value += 25;
			try
			{
				Out.Log(Out.State.LogInfo,"======== Lexem Analyzer ========");
				LexemAnalyzer.sharedAnalyzer.AnalyzeWithDoubleList(parsed);
				LexemAnalyzer.sharedAnalyzer.outputTables();
				Program.window.ProgressBar.Adjustment.Value += 25;
				
				Out.Log(Out.State.LogInfo,"======== Syntax Analyzer ========");
				//SyntaxAnalyzer.AnalyzeLexems();
				PolizAnalyzer.sharedAnalyzer.AnalyzeLexems();
				Program.window.ProgressBar.Adjustment.Value += 50;
			}
			catch (LexemException error)
			{
				Out.Log(Out.State.LogInfo,"\n"+error.UserInfo);
			}
		}
    }
}
