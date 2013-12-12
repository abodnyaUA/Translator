using System;
using System.IO;

namespace Translators
{
	public partial class MainWindow : Gtk.Window
	{
		private Gtk.TextBuffer CodeTextBuffer { get {return CodeTextView.Buffer; } }
		private string filepath = "";

		public MainWindow () : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}

		public Gtk.TextView Console { get { return ConsoleTextView; } }
		public Gtk.ProgressBar ProgressBar { get { return CompileProgressBar; } }

		protected void OpenFileEventHandler (object sender, EventArgs e)
		{
			Gtk.FileChooserDialog dialog = new Gtk.FileChooserDialog("Choose text file",
				this,Gtk.FileChooserAction.Open,
				"Cancel",Gtk.ResponseType.Cancel,
				"Open",Gtk.ResponseType.Accept);
			if (dialog.Run() == (int)Gtk.ResponseType.Accept) 
			{
				String list = "";
				filepath = dialog.Filename;
				StreamReader sr = new StreamReader(filepath);
				list = sr.ReadToEnd();
				sr.Close();
				CodeTextView.Buffer.Text = list;
			}
			//Don't forget to call Destroy() or the FileChooserDialog window won't get closed.
			dialog.Destroy();
		}

		private void SaveFile()
		{
			StreamWriter sw = new StreamWriter(filepath);
			sw.Write(CodeTextView.Buffer.Text);
			sw.Close();
		}

		protected void CompileFileEventHandler (object sender, EventArgs e)
		{
			SaveFile();
			Compiler.sharedCompiler.CompileFile(filepath);
		}

		protected void SaveButtonEventHandler (object sender, EventArgs e)
		{
			SaveFile();
		}
		
		protected void TableButtonHandler (object sender, EventArgs e)
		{
			SyntaxAnalyzerBottomUp analyzer = SyntaxAnalyzerBottomUp.sharedAnalyzer;
		}
	}
}

