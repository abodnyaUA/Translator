
// This file has been generated by the GUI designer. Do not modify.
namespace Translators
{
	public partial class MainWindow
	{
		private global::Gtk.HBox hbox2;
		private global::Gtk.VBox vbox3;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Button OpenButton;
		private global::Gtk.Button SaveButton;
		private global::Gtk.Button SaveAsButton;
		private global::Gtk.VSeparator vseparator1;
		private global::Gtk.Button CompileButton;
		private global::Gtk.ProgressBar CompileProgressBar;
		private global::Gtk.VSeparator vseparator2;
		private global::Gtk.Button ShowLexemTableButton;
		private global::Gtk.Button ShowGrammarButton;
		private global::Gtk.VPaned vpaned1;
		private global::Gtk.VPaned vpaned2;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TextView CodeTextView;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TextView ConsoleTextView;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Translators.MainWindow
			this.Name = "Translators.MainWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.BorderWidth = ((uint)(6));
			// Container child Translators.MainWindow.Gtk.Container+ContainerChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.OpenButton = new global::Gtk.Button ();
			this.OpenButton.CanFocus = true;
			this.OpenButton.Name = "OpenButton";
			this.OpenButton.UseUnderline = true;
			this.OpenButton.Label = global::Mono.Unix.Catalog.GetString ("Open");
			this.hbox3.Add (this.OpenButton);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.OpenButton]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.SaveButton = new global::Gtk.Button ();
			this.SaveButton.CanFocus = true;
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.UseUnderline = true;
			this.SaveButton.Label = global::Mono.Unix.Catalog.GetString ("Save");
			this.hbox3.Add (this.SaveButton);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.SaveButton]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.SaveAsButton = new global::Gtk.Button ();
			this.SaveAsButton.CanFocus = true;
			this.SaveAsButton.Name = "SaveAsButton";
			this.SaveAsButton.UseUnderline = true;
			this.SaveAsButton.Label = global::Mono.Unix.Catalog.GetString ("Save As");
			this.hbox3.Add (this.SaveAsButton);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.SaveAsButton]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.vseparator1 = new global::Gtk.VSeparator ();
			this.vseparator1.Name = "vseparator1";
			this.hbox3.Add (this.vseparator1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.vseparator1]));
			w4.Position = 3;
			w4.Expand = false;
			w4.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.CompileButton = new global::Gtk.Button ();
			this.CompileButton.CanFocus = true;
			this.CompileButton.Name = "CompileButton";
			this.CompileButton.UseUnderline = true;
			this.CompileButton.Label = global::Mono.Unix.Catalog.GetString ("Compile");
			this.hbox3.Add (this.CompileButton);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.CompileButton]));
			w5.Position = 4;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.CompileProgressBar = new global::Gtk.ProgressBar ();
			this.CompileProgressBar.Name = "CompileProgressBar";
			this.hbox3.Add (this.CompileProgressBar);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.CompileProgressBar]));
			w6.Position = 5;
			// Container child hbox3.Gtk.Box+BoxChild
			this.vseparator2 = new global::Gtk.VSeparator ();
			this.vseparator2.Name = "vseparator2";
			this.hbox3.Add (this.vseparator2);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.vseparator2]));
			w7.Position = 6;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.ShowLexemTableButton = new global::Gtk.Button ();
			this.ShowLexemTableButton.CanFocus = true;
			this.ShowLexemTableButton.Name = "ShowLexemTableButton";
			this.ShowLexemTableButton.UseUnderline = true;
			this.ShowLexemTableButton.Label = global::Mono.Unix.Catalog.GetString ("Lexems");
			this.hbox3.Add (this.ShowLexemTableButton);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.ShowLexemTableButton]));
			w8.Position = 7;
			w8.Expand = false;
			w8.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.ShowGrammarButton = new global::Gtk.Button ();
			this.ShowGrammarButton.CanFocus = true;
			this.ShowGrammarButton.Name = "ShowGrammarButton";
			this.ShowGrammarButton.UseUnderline = true;
			this.ShowGrammarButton.Label = global::Mono.Unix.Catalog.GetString ("Grammar");
			this.hbox3.Add (this.ShowGrammarButton);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.ShowGrammarButton]));
			w9.Position = 8;
			w9.Expand = false;
			w9.Fill = false;
			this.vbox3.Add (this.hbox3);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox3]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.vpaned1 = new global::Gtk.VPaned ();
			this.vpaned1.CanFocus = true;
			this.vpaned1.Name = "vpaned1";
			this.vpaned1.Position = 10;
			// Container child vpaned1.Gtk.Paned+PanedChild
			this.vpaned2 = new global::Gtk.VPaned ();
			this.vpaned2.CanFocus = true;
			this.vpaned2.Name = "vpaned2";
			this.vpaned2.Position = 289;
			// Container child vpaned2.Gtk.Paned+PanedChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.HscrollbarPolicy = ((global::Gtk.PolicyType)(2));
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.CodeTextView = new global::Gtk.TextView ();
			this.CodeTextView.CanFocus = true;
			this.CodeTextView.Name = "CodeTextView";
			this.CodeTextView.WrapMode = ((global::Gtk.WrapMode)(2));
			this.CodeTextView.LeftMargin = 5;
			this.GtkScrolledWindow.Add (this.CodeTextView);
			this.vpaned2.Add (this.GtkScrolledWindow);
			global::Gtk.Paned.PanedChild w12 = ((global::Gtk.Paned.PanedChild)(this.vpaned2 [this.GtkScrolledWindow]));
			w12.Resize = false;
			// Container child vpaned2.Gtk.Paned+PanedChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.HscrollbarPolicy = ((global::Gtk.PolicyType)(2));
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.ConsoleTextView = new global::Gtk.TextView ();
			this.ConsoleTextView.CanFocus = true;
			this.ConsoleTextView.Name = "ConsoleTextView";
			this.ConsoleTextView.Editable = false;
			this.ConsoleTextView.WrapMode = ((global::Gtk.WrapMode)(2));
			this.ConsoleTextView.LeftMargin = 5;
			this.GtkScrolledWindow1.Add (this.ConsoleTextView);
			this.vpaned2.Add (this.GtkScrolledWindow1);
			global::Gtk.Paned.PanedChild w14 = ((global::Gtk.Paned.PanedChild)(this.vpaned2 [this.GtkScrolledWindow1]));
			w14.Resize = false;
			this.vpaned1.Add (this.vpaned2);
			this.vbox3.Add (this.vpaned1);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.vpaned1]));
			w16.Position = 1;
			this.hbox2.Add (this.vbox3);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vbox3]));
			w17.Position = 0;
			this.Add (this.hbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 662;
			this.DefaultHeight = 398;
			this.Show ();
			this.OpenButton.Clicked += new global::System.EventHandler (this.OpenFileEventHandler);
		}
	}
}
