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
		public ISyntaxAnalyzer SyntaxAnalyzer;
		public LexemList LexemList;

        private Compiler() 
		{
			SyntaxAnalyzer = SyntaxAnalyzerWithTable.sharedAnalyzer;
			LexemList = new LexemList();
		}

        public void CompileFile()
		{
			string path = Program.window.ChoosedFileName;
			
			Gtk.Application.Invoke(delegate {
				Program.window.ProgressBar.Adjustment.Value = 0;
				Program.window.Console.Buffer.Text = ""; });
			try
			{
				Out.Log(Out.State.LogInfo,"======== Parse code ========");
				List<List<string>> parsed = Parser.sharedParser.ParseFile(path);
				Gtk.Application.Invoke(delegate {
					Program.window.ProgressBar.Adjustment.Value += 25;
				});

				Out.Log(Out.State.LogInfo,"======== Lexem Analyzer ========");
				LexemAnalyzer.sharedAnalyzer.AnalyzeWithDoubleList(parsed);
				Gtk.Application.Invoke(delegate {
					Program.window.ProgressBar.Adjustment.Value += 25; });
				
				Out.Log(Out.State.LogInfo,"======== Syntax Analyzer ========");
				SyntaxAnalyzer.AnalyzeLexems();
				Gtk.Application.Invoke(delegate {
					Program.window.ProgressBar.Adjustment.Value += 25;	});		
				
				Out.Log(Out.State.LogInfo,"======== Poliz Analyzer ========");
				PolizAnalyzer.sharedAnalyzer.AnalyzeLexems();
				Gtk.Application.Invoke(delegate {
					Program.window.ProgressBar.Adjustment.Value += 25; });
			}
			catch (LexemException error)
			{
				Out.Log(Out.State.LogInfo,"\n"+error.UserInfo);
			}
		}
    }
}
