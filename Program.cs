using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmashBrosIronMan
{
    public class Program
    {
        public static string Player1 { get; set; }
        public static string Player2 { get; set; }
        static void Main(string[] args)
        {
            string player1Data = File.ReadAllText(Environment.CurrentDirectory + "/Input/Player1Data.txt");
            string player2Data = File.ReadAllText(Environment.CurrentDirectory + "/Input/Player2Data.txt");

            Console.WriteLine("Player 1 name: ");
            Player1 = Console.ReadLine();
            Console.WriteLine("Player 2 name: ");
            Player2 = Console.ReadLine();

            HashSet<string> taken = new HashSet<string>();

            //Parse player1
            List<CharData> player1 = player1Data.Split('\n').Select(s => s.Split('\t')).Select(parsed => new CharData(parsed[0], int.Parse(parsed[1]))).ToList();
            List<CharData> player1Ordered = player1.OrderByDescending(d => d.Skill).ToList();//.Dump();
            List<IGrouping<int, CharData>> player1OrderedGroups = player1.GroupBy(d => d.Skill).ToList().OrderBy(g => g.First().Skill).ToList();

            //Parse player2
            List<CharData> player2 = player2Data.Split('\n').Select(s => s.Split('\t')).Select(parsed => new CharData(parsed[0], int.Parse(parsed[1]))).ToList();
            List<CharData> player2Ordered = player2.OrderByDescending(p => p.Skill).ToList();
            List<IGrouping<int, CharData>> player2OrderedGroups = player2.GroupBy(p => p.Skill).ToList().OrderBy(g => g.First().Skill).ToList();

            List<string> finalList = new List<string>();
            bool finished = false;
            while (!finished)
            {
                foreach (CharData theChar in player1Ordered)
                {
                    if (taken.Contains(theChar.Name))
                        continue;
                    string adding = AddChars(taken, theChar.Name, player2OrderedGroups, true);//.Dump();
                    finalList.Add(adding);
                    if (adding.Contains("LAST ROUND"))
                    {
                        finished = true;
                        break;
                    }
                    break;
                }
                if (finished)
                    break;

                foreach (CharData theChar in player2Ordered)
                {
                    if (taken.Contains(theChar.Name))
                        continue;
                    string adding = AddChars(taken, theChar.Name, player1OrderedGroups, false);
                    finalList.Add(adding);
                    if (adding.Contains("LAST ROUND"))
                    {
                        finished = true;
                        break;
                    }
                    break;
                }
            }

            finalList.ForEach(f =>
            {
                Console.WriteLine(f);
            });

            Console.WriteLine("\nPress any key to close...");
            Console.ReadKey();
        }

        public static string AddChars(HashSet<string> taken, string char1, IEnumerable<IGrouping<int, CharData>> ordered, bool firstStatic)
        {
            foreach (IGrouping<int, CharData> group in ordered)
            {
                foreach (CharData char2 in group.OrderBy(g => Guid.NewGuid()))
                {
                    if (!taken.Contains(char2.Name) && char1 != char2.Name)
                    {
                        taken.Add(char2.Name);
                        taken.Add(char1);

                        return (firstStatic) ? $"{Player1}: {char1}, {Player2}: {char2.Name}" : $"{Player1}: {char2.Name}, {Player2}: {char1}";
                    }
                }
            }

            return $"LAST ROUND. Someone plays {char1}";
        }
    }

    public class CharData
    {
        public string Name { get; set; }
        public int Skill { get; set; }
        public CharData(string name, int skill)
        {
            Name = name;
            Skill = skill;
        }
    }
}
