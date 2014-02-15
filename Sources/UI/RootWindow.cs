using System;
using System.IO;

namespace Translators
{
	public partial class RootWindow : Gtk.Window
	{
		public Gtk.TextView Console { get { return ConsoleTextView; } }
		public Gtk.ProgressBar ProgressBar { get { return CompileProgressBar; } }
		private string FileName = "/home/abodnya/.translatorfile";

		public RootWindow () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			BaseSetup();
		}

		private void BaseSetup()
		{
			if (File.Exists(FileName))
			{
				string [] lines = File.ReadAllLines(FileName);
				if (lines.Length > 0)
				{
					FileChooser.SetFilename(lines[0]);
				}
			}
		}

		protected void TableButtonHandler (object o, EventArgs args)
		{
			SyntaxAnalyzerBottomUp.sharedAnalyzer.PrintTable();
		}

		protected void CompileFileEventHandler (object o, EventArgs args)
		{
			if (FileChooser.Filename != "")
			{
				Compiler.sharedCompiler.CompileFile(FileChooser.Filename);
			}
		}

		protected void OpenFileEventHandler (object sender, EventArgs e)
		{
			File.WriteAllLines(FileName,new string[] { FileChooser.Filename } );
		}

		protected void PolizButtonHandler (object sender, EventArgs e)
		{
			if (FileChooser.Filename != "")
			{
				Compiler.sharedCompiler.AnalyzeForPoliz(FileChooser.Filename);
			}
		}
	}
}

