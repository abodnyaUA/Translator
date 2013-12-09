using System;

namespace Translators
{
	public class SyntaxAnalyzerBottomUp
	{
		private Grammar grammar = new Grammar();
		private BottomUpTable table = new BottomUpTable();
		private SyntaxAnalyzerBottomUp ()
		{
			table.GenerateTableWithGrammar(this.grammar);
		}
		private static SyntaxAnalyzerBottomUp _sharedAnalyzer = null;
		public static SyntaxAnalyzerBottomUp sharedAnalyzer
		{
			get
			{
				if (_sharedAnalyzer == null) _sharedAnalyzer = new SyntaxAnalyzerBottomUp();
				return _sharedAnalyzer;
			}
		}
	}
}

