using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Translators.Lab01
{
    class Parser
    {
        private static Parser _sharedParser = null;
        public static Parser sharedParser
        {
            get
            {
                if (_sharedParser == null) _sharedParser = new Parser();
                return _sharedParser;
            }
        }
        private Parser()
        { }

        private string ReadFile(string path)
        {
            String list = "";
            StreamReader sr = new StreamReader(path);
            while (!sr.EndOfStream)
            {
                string packet = sr.ReadLine();
                for (int i = 0; i < packet.Length-1; i++)
                {
                    if (packet[i] == '/' && packet[i + 1] == '/')
                    {
                        packet = packet.Substring(0, i);
                    }
				}
				realLines.Add(packet);
                list = list + packet + "\n";
            }
            sr.Close();
            return list;
        }

        public List<char> Separators()
        {
            List<char> result = new List<char>();
            result.Add('\n');
            result.Add('+');
            result.Add('-');
            result.Add('/');
            result.Add('*');
            result.Add('^');
            result.Add('=');
            result.Add('>');
            result.Add(',');
            result.Add('<');
            result.Add('!');
            result.Add('{');
            result.Add('}');
            result.Add('(');
			result.Add(')');
			result.Add('[');
			result.Add(']');
			result.Add(' ');
			result.Add('&');
			result.Add('|');
			result.Add('!');
            return result;
        }

		public List<string> SplitBySpace(string source, List<char>separators)
		{
			source = source.Replace('\t',' ');
			HashSet<char> separtatorsSet = new HashSet<char>();
			foreach (char ch in separators) 
			{
				separtatorsSet.Add(ch);
			}
			
			Console.WriteLine("Before: "+source);
			for (int i=0; i<source.Length; i++)
			{
				char ch = source[i];
				char nextCh = '\0';
				char prevCh = '\0';
				if (i < source.Length-1) nextCh = source[i+1]; 
				if (i > 0) prevCh = source[i-1]; 
				//Console.WriteLine("char = "+ch);
				if (separtatorsSet.Contains(ch))
				{
					//Console.WriteLine("is separator");
					if (i > 0)
					{
						if (source[i-1] != ' ' && 
						    !( /**/ (prevCh == '!' || prevCh == '>' || prevCh == '<') && ch == '=') /**/) 
						{
							source = source.Insert(i," ");
							i++;
						}
					}
					if (i < source.Length-1)
					{
						if (source[i+1] != ' ' && 
						    !( /**/ (ch == '!' || ch == '>' || ch == '<') && nextCh == '=') /**/)
						{
							source = source.Insert(i+1," ");
							i++;
						}
					}
				}
			}
			Console.WriteLine("After: "+source);

			List<string> lexems = new List<string>() { new string ('\0',1) };
			string[] lexemsArray = source.Split(' ');

			for (int i=0;i<lexemsArray.Length;i++)
			{
				string lexem = lexemsArray[i];
				if (lexem != "") 
				{
					lexems.Add(lexem);
				}
			}
			lexems.RemoveAt(0);
			if (lexems[0] == "\n") lexems.RemoveAt(0);
			if (lexems.Last() == "\n") lexems.RemoveAt(lexems.Count-1);

			return lexems;
		}

		private List<string> realLines = new List<string>();

        public List<List<string>> LexemsListWithLines(List<string> source)
        {
            List<List<string>> result = new List<List<string>>();
            result.Insert(0, new List<string>());

            int number = 0;
            foreach (string word in source)
            {
                if (word == "\n")
                {
                    number++;
                    result.Insert(number, new List<string>());
                    result[number].Add(word);
                }
                else
                {
                    result[number].Add(word);
                }
            }

            return result;
        }

        public List<List<string>> ParseFile(string path)
        {
            string sourceCode = ReadFile(path);
            List<char> separators = Separators();
			List<string> parsedWords = SplitBySpace(sourceCode,separators);
            List< List<string> > parsedWordsByLines = LexemsListWithLines(parsedWords);
            for (int i = 0; i < parsedWordsByLines.Count; i++)
            {
                Console.WriteLine("Line "+i+":");
                foreach (string word in parsedWordsByLines[i])
                    Console.WriteLine("\t" + word);
            }
            return parsedWordsByLines;
        }
    }
}
