using System;
using System.IO;

namespace Translators
{
	public partial class MainWindow : Gtk.Window
	{
		private string filePath = "";

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
				StreamReader sr = new StreamReader(dialog.Filename);
				list = sr.ReadToEnd();
				sr.Close();
				filePath = dialog.Filename;
				CodeTextView.Buffer.Text = list;
			}
			//Don't forget to call Destroy() or the FileChooserDialog window won't get closed.
			dialog.Destroy();
		}

		protected void CompileFileEventHandler (object sender, EventArgs e)
		{
			Compiler.sharedCompiler.CompileFile(filePath);
		}
	}
}

