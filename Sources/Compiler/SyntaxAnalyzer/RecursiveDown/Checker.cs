using System;
using System.Linq.Expressions;

namespace Translators
{
	public class Checker
	{
		public enum IncrementMode
		{
			NoIncrement = 0,
			IncrementOnce = 1,
			DoubleIncrement = 2
		}
		
		///  Check with Key ///
		public static void Check(ref int lexemsIterator, int key, string success, string failure)
		{
			Check(ref lexemsIterator,key,success,failure,IncrementMode.IncrementOnce,Out.State.LogVerbose);
		}
		public static void Check(ref int lexemsIterator, int key, string success, string failure, Out.State logState)
		{
			Check(ref lexemsIterator,key,success,failure,IncrementMode.IncrementOnce,logState);
		}
		public static void Check(ref int lexemsIterator, int key, string success, string failure, IncrementMode incrementValue)
		{
			Check(ref lexemsIterator,key,success,failure,incrementValue,Out.State.LogVerbose);
		}
		public static void Check(ref int lexemsIterator, int key, string success, string failure, IncrementMode incrementValue, Out.State logState)
		{
			if (SyntaxAnalyzerRecursiveDown.sharedAnalyzer.lexems[lexemsIterator].Key == key)
			{
				Out.Log(logState,success);
				lexemsIterator+=(int)incrementValue;
			}
			else
			{
				throw new LexemException(
					SyntaxAnalyzerRecursiveDown.sharedAnalyzer.lexems[lexemsIterator].LineNumber,failure);
				
			}
		}

		///  Check with Command ///
		public static void Check(ref int lexemsIterator, string command, string success, string failure)
		{
			Check(ref lexemsIterator,command,success,failure,IncrementMode.IncrementOnce,Out.State.LogVerbose);
		}
		public static void Check(ref int lexemsIterator, string command, string success, string failure, IncrementMode incrementValue)
		{
			Check(ref lexemsIterator,command,success,failure,incrementValue,Out.State.LogVerbose);
		}
		public static void Check(ref int lexemsIterator, string command, string success, string failure, IncrementMode incrementValue, Out.State logState)
		{
			if (SyntaxAnalyzerRecursiveDown.sharedAnalyzer.lexems[lexemsIterator].Command == command)
			{
				Out.Log(logState,success);
				lexemsIterator+=(int)incrementValue;
			}
			else
			{
				throw new LexemException(
					SyntaxAnalyzerRecursiveDown.sharedAnalyzer.lexems[lexemsIterator].LineNumber,failure);
				
			}
		}

		/// Check with function ///
		public delegate bool CheckerFunc <T>(ref T obj);
		
		public static void Check(ref int lexemsIterator, CheckerFunc <int> Func, string success, string failure)
		{
			Check(ref lexemsIterator,Func,success,failure,Checker.IncrementMode.NoIncrement,Out.State.LogVerbose);
		}

		public static void Check(ref int lexemsIterator, CheckerFunc <int> Func, string success, string failure, Checker.IncrementMode incrementValue)
		{
			Check(ref lexemsIterator,Func,success,failure,incrementValue,Out.State.LogVerbose);
		}
		
		public static void Check(ref int lexemsIterator, CheckerFunc <int> Func, string success, string failure, Out.State logState)
		{
			Check(ref lexemsIterator,Func,success,failure,Checker.IncrementMode.NoIncrement,logState);
		}

		public static void Check(ref int lexemsIterator, CheckerFunc <int> Func, string success, string failure, Checker.IncrementMode incrementValue, Out.State logState)
		{
			if (Func(ref lexemsIterator))
			{
				Out.Log(logState,success);
				lexemsIterator += (int)incrementValue;
			}
			else
			{
				throw new LexemException(SyntaxAnalyzerRecursiveDown.sharedAnalyzer.lexems[lexemsIterator].LineNumber,
				                                          failure);
				
			}
		}

		/// Check with Predicate ///
//
//		public delegate bool CheckerPredicate();
//		
//		public static void CheckWithPredicate(ref int lexemsIterator, Expression<Func<bool>> predicate, string success, string failure)
//		{
//			CheckWithPredicate(ref lexemsIterator,predicate,success,failure,IncrementMode.NoIncrement,Out.State.LogVerbose);
//		}
//		public static void CheckWithPredicate(ref int lexemsIterator, Expression<Func<bool>> predicate, string success, string failure, IncrementMode incrementValue)
//		{
//			CheckWithPredicate(ref lexemsIterator,predicate,success,failure,incrementValue,Out.State.LogVerbose);
//		}
//		public static void CheckWithPredicate(ref int lexemsIterator, Expression<Func<bool>> predicate, string success, string failure, IncrementMode incrementValue, Out.State logState)
//		{
//			if (predicate())
//			{
//				Out.Log(logState,success);
//				lexemsIterator+=(int)incrementValue;
//			}
//			else
//			{
//				throw new LexemException(SyntaxAnalyzer.sharedAnalyzer.lexems[lexemsIterator].lineNumber,
//				                                          failure);
//				
//			}
//		}
	}
}
