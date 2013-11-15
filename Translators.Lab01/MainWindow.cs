using System;
using System.IO;

namespace Translators
{
	public partial class MainWindow : Gtk.Window
	{
		public MainWindow () : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}

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
				CodeTextView.Buffer.Text = list;
			}
			//Don't forget to call Destroy() or the FileChooserDialog window won't get closed.
			dialog.Destroy();
		}
	}
}

