using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Helpers
{
    public static class BracketsHelper
    {
        public static List<Bracket> GenerateBrackets(List<Team> teams)
        {
            var perfectBrackets = new int[] { 2, 4, 5, 8, 16, 32, 64 };

            var baseNum = teams.Count;
            var closestPerfectBracket = perfectBrackets.OrderBy(x => x).First(x => x >= teams.Count);
            var byes = closestPerfectBracket - baseNum;

            if (byes > 0)
            {
                baseNum = closestPerfectBracket;
            }

            var brackets = new List<Bracket>();
            var round = 1;
            decimal baseT = baseNum / 2;
            decimal baseC = baseNum / 2;
            var teamMark = 0;
            decimal nextInc = baseNum / 2;

            for (var i = 1; i <= (baseNum - 1); i++)
            {
                decimal baseR = i / baseT;
                var isBye = false;

                if (byes > 0 && (i % 2 != 0 || byes >= (baseT - i)))
                {
                    isBye = true;
                    byes--;
                }

                var last = brackets.Where(x => x.NextGame == i).Select(x => new Matchup() { Game = x.BracketNo });

                var newBracket = new Bracket()
                {
                    LastGames = round == 1 ? null : new Tuple<int, int>(last.ElementAt(0).Game, last.ElementAt(1).Game),
                    NextGame = nextInc + 1 > Convert.ToDecimal(baseNum) - 1 ? (decimal?)null : nextInc + i,
                    BracketNo = i,
                    RoundNo = round,
                    Bye = isBye
                };

                brackets.Add(newBracket);

                teamMark += 2;
                if (i % 2 != 0)
                {
                    nextInc--;
                }

                while (baseR >= 1)
                {
                    round++;
                    baseC /= 2;
                    baseT = baseT + baseC;
                    baseR = i / baseT;
                }
            }

            return brackets;
        }

        public class Matchup
        {
            public int Game { get; set; }
        }

        public class Bracket
        {
            public int BracketNo { get; set; }

            public bool Bye { get; set; }

            public Tuple<int, int> LastGames { get; set; }

            public decimal? NextGame { get; set; }

            public int RoundNo { get; set; }
        }
    }
}