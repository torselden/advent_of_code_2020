﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AdventOfCode;
//using Position = AdventOfCode.GenericPosition2D<int>;

namespace day22
{
    public class Day22
    {
        readonly static string nsname = typeof(Day22).Namespace;
        readonly static string inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\" + nsname + "\\input.txt");

        // Day 22: Crab Combat - Play card game, first simple, then with recursion and more

        static List<int> ReadInput(string path)
        {
            var strs = ReadIndata.Strings(path);
            var list = new List<int>();
            foreach (var line in strs)
                if (line.Length > 0 && char.IsDigit(line[0]))
                    list.Add(int.Parse(line));
            return list;
        }

        static (bool, int) PlayGameFast(ref int[] p1, ref int[] p2, int n)
        {
            int r1 = 0;
            int w1 = n / 2;
            int r2 = 0;
            int w2 = n / 2;
            void Update(ref int[] a, ref int ra, ref int wa, ref int[] b, ref int rb)
            {
                a[wa] = a[ra];
                wa = (wa == n - 1) ? 0 : wa + 1;
                ra = (ra == n - 1) ? 0 : ra + 1;
                a[wa] = b[rb];
                wa = (wa == n - 1) ? 0 : wa + 1;
                rb = (rb == n - 1) ? 0 : rb + 1;
            }
            bool p1won = true;
            while (r1 != w1)
            {
                p1won = p1[r1] > p2[r2];
                if (p1won)
                    Update(ref p1, ref r1, ref w1, ref p2, ref r2);
                else
                    Update(ref p2, ref r2, ref w2, ref p1, ref r1);
            }
            return (p1won, p1won ? r1 : r2);
        }

        static List<int> PlayGame(List<int> p1, List<int> p2)
        {
            List<int> a = new List<int>(p1);
            List<int> b = new List<int>(p2);
            bool awon = true;
            while (a.Count > 0 && b.Count > 0)
            {
                awon = a.First() > b.First();
                var w = awon ? a : b;
                var l = awon ? b : a;
                w.Add(w.First());
                w.RemoveAt(0);
                w.Add(l.First());
                l.RemoveAt(0);
            }
            return awon ? a : b;
        }

        static int Score(ICollection<int> c) { return c.Reverse().Select((x, i) => (i + 1) * x).Sum(); }

        static Object PartA()
        {
            var input = ReadInput(inputPath);
            int cards = input.Count;
            int n = cards / 2;
            int[] p1 = new int[cards];
            int[] p2 = new int[cards];
            input.Take(n).ToArray().CopyTo(p1, 0);
            input.Skip(n).Take(n).ToArray().CopyTo(p2, 0);
            (bool p1win, int rIdx) = PlayGameFast(ref p1, ref p2, cards);
            var deck = (p1win ? p1 : p2).ToList();
            deck.AddRange(deck);
            deck = deck.Skip(rIdx).Take(cards).ToList();
            int ans = Score(deck);

            //var deck = PlayGame(input.Take(n).ToList(), input.Skip(n).Take(n).ToList());
            //deck.Reverse();
            //int ans = deck.Select((x, i) => (i + 1) * x).Sum();

            Console.WriteLine("Part A: Result is {0}", ans);
            return ans;
        }

        static void PrintStatus(List<int> p1, List<int> p2, int round, int game, bool subGame)
        {
            if (round == 1)
            {
                Console.WriteLine("=== Game {0} ===", game);
                Console.WriteLine();
            }
            Console.WriteLine("-- Round {0} (Game {1}) --", round, game);
            Console.WriteLine("Player 1's deck: {0}", string.Join(", ", p1));
            Console.WriteLine("Player 2's deck: {0}", string.Join(", ", p2));
            Console.WriteLine("Player 1 plays: {0}", p1.First());
            Console.WriteLine("Player 2 plays: {0}", p2.First());
            if (subGame)
                Console.WriteLine("Playing a sub-game to determine the winner...");
            else
                Console.WriteLine("Player {0} wins round {1} of game {2}!", p1.First() > p2.First() ? 1 : 2, round, game);
            Console.WriteLine();
        }

        static int nextGame = 1;

        static (bool, List<int>) PlayGameB(List<int> p1, List<int> p2, int game)
        {
            HashSet<(string, string)> visited = new HashSet<(string, string)>();
            List<int> a = new List<int>(p1);
            List<int> b = new List<int>(p2);
            bool awon = true;
            bool done = false;
            int round = 1;
            while (a.Count > 0 && b.Count > 0 && !done)
            {
                var id = (string.Join(",", a), string.Join(",", b));
                done = !visited.Add(id);
                if (!done)
                {
                    int ca = a.First();
                    int cb = b.First();
                    bool subGame = (ca < a.Count && cb < b.Count);
                    //PrintStatus(a, b, round, game, subGame);
                    awon = ca > cb;
                    if (subGame)
                        (awon, _) = PlayGameB(a.Skip(1).Take(ca).ToList(), b.Skip(1).Take(cb).ToList(), ++nextGame);
                    var w = awon ? a : b;
                    var l = awon ? b : a;
                    w.Add(w.First());
                    w.RemoveAt(0);
                    w.Add(l.First());
                    l.RemoveAt(0);
                    round++;
                }
            }
            awon |= done;
            return (awon, awon ? a : b);
        }

        static Object PartB()
        {
            var input = ReadInput(inputPath);
            int n = input.Count / 2;
            var (_, deck) = PlayGameB(input.Take(n).ToList(), input.Skip(n).Take(n).ToList(), 1);
            int ans = Score(deck);
            Console.WriteLine("Part B: Result is {0}", ans);
            return ans;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("AoC 2020 - " + nsname + ":");
            PartA();
            PartB();
        }

        public static bool MainTest()
        {
            int a = 33772;
            int b = 35070;
            return (PartA().Equals(a)) && (PartB().Equals(b));
        }
    }
}
