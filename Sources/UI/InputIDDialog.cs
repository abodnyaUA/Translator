using System;

namespace Translators
{
	public partial class InputIDDialog : Gtk.Dialog
	{
		public InputIDDialog ()
		{
			this.Build ();
			this.numericField.Adjustment.Upper = Int16.MaxValue;
			this.numericField.Adjustment.Lower = Int16.MinValue;
		}
		public int Result()
		{
			return (int)this.numericField.Value;
		}
	}
}

