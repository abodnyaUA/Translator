using System;
using System.IO;
using System.Threading;
using Gtk;

namespace Translators
{
	public partial class RootWindow : Gtk.Window
	{
		public Gtk.TextView Console { get { return ConsoleTextView; } }
		public Gtk.ProgressBar ProgressBar { get { return CompileProgressBar; } }
		private string FileName = "/home/abodnya/.translatorfile";

		private string sourceName = null;
		public string ChoosedFileName { get { return sourceName; } }

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
			if (LexemList.Instance.Lexems.Count > 0)
			{
				System.Diagnostics.Process.Start("/usr/bin/google-chrome",Constants.HTMLTablePath);
			}
			else
			{
				Gtk.Dialog dialog = new Gtk.Dialog("Ошибочка",this,Gtk.DialogFlags.Modal);
				Label text = new Label();
				text.Text = "Сначала скомпилируйте файл";
				dialog.VBox.Add(text);
				dialog.AddButton ("Close", ResponseType.Close);
				dialog.ShowAll();
				dialog.Run();
				dialog.Destroy();
			}
		}

		protected void CompileFileEventHandler (object o, EventArgs args)
		{
			if (FileChooser.Filename != "")
			{
				sourceName = FileChooser.Filename;
				Thread calc = new Thread(new ThreadStart(
					Compiler.sharedCompiler.CompileFile));
				calc.Start();
			}
		}

		protected void OpenFileEventHandler (object sender, EventArgs e)
		{
			File.WriteAllLines(FileName,new string[] { FileChooser.Filename } );
		}
	}
}

