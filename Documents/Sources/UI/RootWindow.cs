using System;
using System.IO;

namespace Translators
{
	public partial class RootWindow : Gtk.Window
	{
		public Gtk.TextView Console { get { return ConsoleTextView; } }
		public Gtk.ProgressBar ProgressBar { get { return CompileProgressBar; } }
		private Gtk.TextBuffer CodeTextBuffer { get {return CodeTextView.Buffer; } }
		private string filepath = "";

		public RootWindow () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}

		protected void OpenFileEventHandler (object o, EventArgs args)
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

		protected void SaveButtonEventHandler (object o, EventArgs args)
		{
			SaveFile();
		}

		protected void TableButtonHandler (object o, EventArgs args)
		{
			SyntaxAnalyzerBottomUp analyzer = SyntaxAnalyzerBottomUp.sharedAnalyzer;
		}

		protected void CompileFileEventHandler (object o, EventArgs args)
		{
			SaveFile();
			Compiler.sharedCompiler.CompileFile(filepath);
		}
	}
}

