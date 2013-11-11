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

        public List<string> Split(string source, List<char> separators)
        {

            source = source.Replace("\t", "");

            List<string> result = new List<string>();
            result.Add(source);
            foreach (char separator in separators)
            {
                bool existSeparator = false;
				List <string> parsedForSeparator = new List<string>();
                do
                {
                    Console.WriteLine("=========== Separator: " + separator);
                    existSeparator = false;
                    string sepString = "";
                    sepString += separator;
					if (separator == '|' || separator == '&') sepString += separator;

                    for (int i = 0; i < result.Count; i++)
                    {
                        Console.WriteLine("!!Parse string "+result[i]);
                        string[] parts = result[i].Split(separator);
                        if (parts[0] == result[i]) continue;
                        if (sepString == result[i]) continue;
                        else
                        {
							int number = 0;
							if (result[i][0] == ' ') Console.WriteLine("@@:" + result[i]);
							result.RemoveAt(i);
							int size = 0;
							foreach (string part in parts)
								size++;

							foreach (string part in parts)
							{
								if (part == "") continue;
								existSeparator = true;
								Console.WriteLine("Insert part: " + part);
								result.Insert(i + number, part);
								Console.WriteLine("Insert part: " + sepString);
								result.Insert(i + number + 1, sepString);
								number += 2;
							}

							// -a
							if (size > 1)
							{
								if (parts[0] == "" && parts[1] != "")
								{
									Console.WriteLine("Remove part: " + result[i + number - 1]);
									result.RemoveAt(i + number - 1);
									Console.WriteLine("Insert part: " + sepString);
									result.Insert(i + number - 2,sepString);
								}
							}
							// ???
							if (number > 2) result.RemoveAt(i + number - 1);
							i += number;
                        }
                    }
                    Console.WriteLine("/////////// Separator: " + separator);
                    //Console.ReadKey();
                } while (existSeparator);

            }

            // Merge >= <= == !=
            for (int i = 0; i < result.Count; i++)
            {
                if ((result[i][0] == '>' || result[i][0] == '<' || result[i][0] == '!' || result[i][0] == '=') && i < result.Count - 1)
                {
					Console.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                    // Check back
                    if (i > 0)
                    {
                        if (result[i - 1][0] == '>' || result[i - 1][0] == '<' || result[i - 1][0] == '!' || result[i - 1][0] == '=')
                            continue;
                    }
                    if (result[i + 1][0] == '=')
					{
						Console.WriteLine("::::::::::::::::::::::::::::::::::");
                        result[i] += '=';
                        result.RemoveAt(i + 1);
                        continue;
                    }
                }
            }


            // Remove empty cases
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] == " ")
                {
                    result.RemoveAt(i);
                }
                else
                {
                    Console.WriteLine(result[i]);
                }
            }

            return result;
        }

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
            List<string> parsedWords = Split(sourceCode,separators);
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
