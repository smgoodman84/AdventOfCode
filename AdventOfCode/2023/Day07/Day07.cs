using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day07
{
    public class Day07 : Day
    {
        public Day07() : base(2023, 7, "Day07/input_2023_07.txt", "255048101", "", false)
        {

        }

        private List<Hand> _hands;
        public override void Initialise()
        {
            _hands = InputLines
                .Select(l => new Hand(l))
                .ToList();
        }

        public override string Part1()
        {
            var ordered = _hands
                .OrderBy(h => h.HandType)
                .ThenBy(h => h.CardRankings[0])
                .ThenBy(h => h.CardRankings[1])
                .ThenBy(h => h.CardRankings[2])
                .ThenBy(h => h.CardRankings[3])
                .ThenBy(h => h.CardRankings[4])
                .ToList();

            var result = 0;
            var rank = ordered.Count();
            foreach(var hand in ordered)
            {
                TraceLine($"{rank}: {hand}");
                result += rank * hand.Bid;
                rank -= 1;
            }

            return result.ToString();
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private class Hand
        {
            public int Bid { get; set; }
            public List<char> Cards { get; set; }
            public List<int> CardRankings { get; set; }
            public HandType HandType { get; set; }
            public Hand(string description)
            {
                var split = description.Split(" ");

                Cards = split[0].ToList();
                CardRankings = Cards
                    .Select(c => CardRankingLookup.IndexOf(c))
                    .ToList();

                Bid = int.Parse(split[1]);

                HandType = GetHandType();
            }

            private List<char> CardRankingLookup = new List<char>
            {
                'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'
            };

            private HandType GetHandType()
            {
                var groups = Cards.GroupBy(c => c);

                if (groups.Count() == 1)
                {
                    return HandType.FiveOfAKind;
                }

                if (groups.Any(g => g.Count() == 4))
                {
                    return HandType.FourOfAKind;
                }

                if (groups.Any(g => g.Count() == 3))
                {
                    if (groups.Any(g => g.Count() == 2))
                    {
                        return HandType.FullHouse;
                    }

                    return HandType.ThreeOfAKind;
                }

                if (groups.Count(g => g.Count() == 2) == 2)
                {
                    return HandType.TwoPair;
                }

                if (groups.Count(g => g.Count() == 2) == 1)
                {
                    return HandType.OnePair;
                }

                return HandType.HighCard;
            }

            public override string ToString()
            {
                return $"{Bid:10} - {HandType} {string.Join("", Cards)},  {string.Join(",", CardRankings)}";
            }
        }

        private enum HandType
        {
            FiveOfAKind,
            FourOfAKind,
            FullHouse,
            ThreeOfAKind,
            TwoPair,
            OnePair,
            HighCard
        }
    }
}

