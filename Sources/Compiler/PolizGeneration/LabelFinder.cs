using System;
using System.Collections.Generic;

namespace Translators
{
	public class LabelFinder
	{
		public static int PositionOpenLabel(List<Lexem> poliz, string label)
		{
			int index = poliz.FindIndex((Lexem obj) => {
				return obj.Key == PolizOperarionsList.kLexemKeyLabelStart && obj.Command == label;
			});
			return index;
		}

		public static int PositionCloseLabel(List<Lexem> poliz, string label)
		{
			int index = poliz.FindIndex((Lexem obj) => {
				return obj.Key == PolizOperarionsList.kLexemKeyLabelEnd && obj.Command == label;
			});
			return index;
		}
	}
}

