
// This file has been generated by the GUI designer. Do not modify.
namespace Translators
{
	public partial class InputIDDialog
	{
		private global::Gtk.SpinButton numericField;
		private global::Gtk.Button buttonOk;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Translators.InputIDDialog
			this.Name = "Translators.InputIDDialog";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child Translators.InputIDDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.numericField = new global::Gtk.SpinButton (0, 2147483647, 1);
			this.numericField.CanFocus = true;
			this.numericField.Name = "numericField";
			this.numericField.Adjustment.PageIncrement = 10;
			this.numericField.ClimbRate = 10000000;
			this.numericField.Numeric = true;
			w1.Add (this.numericField);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(w1 [this.numericField]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Internal child Translators.InputIDDialog.ActionArea
			global::Gtk.HButtonBox w3 = this.ActionArea;
			w3.Name = "dialog1_ActionArea";
			w3.Spacing = 10;
			w3.BorderWidth = ((uint)(5));
			w3.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w4 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w3 [this.buttonOk]));
			w4.Expand = false;
			w4.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 244;
			this.DefaultHeight = 70;
			this.Show ();
		}
	}
}
