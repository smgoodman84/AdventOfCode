using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day07;

public class Day07 : Day
{
    public Day07() : base(2023, 7, "Day07/input_2023_07.txt", "255048101", "253718286", false)
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
        var ordered = _hands
            .OrderBy(h => h.JokerHandType)
            .ThenBy(h => h.JokerCardRankings[0])
            .ThenBy(h => h.JokerCardRankings[1])
            .ThenBy(h => h.JokerCardRankings[2])
            .ThenBy(h => h.JokerCardRankings[3])
            .ThenBy(h => h.JokerCardRankings[4])
            .ToList();

        var result = 0;
        var rank = ordered.Count();
        foreach (var hand in ordered)
        {
            TraceLine($"{rank}: {hand}");
            result += rank * hand.Bid;
            rank -= 1;
        }

        return result.ToString();
    }

    private class Hand
    {
        public int Bid { get; set; }
        public List<char> Cards { get; set; }
        public List<int> CardRankings { get; set; }
        public List<int> JokerCardRankings { get; set; }
        public HandType HandType { get; set; }
        public HandType JokerHandType { get; set; }
        public Hand(string description)
        {
            var split = description.Split(" ");

            Cards = split[0].ToList();
            CardRankings = Cards
                .Select(c => CardRankingLookup.IndexOf(c))
                .ToList();

            JokerCardRankings = Cards
                .Select(c => JokerCardRankingLookup.IndexOf(c))
                .ToList();

            Bid = int.Parse(split[1]);

            HandType = GetHandType();
            JokerHandType = GetJokerHandType();
        }

        private List<char> CardRankingLookup = new List<char>
        {
            'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'
        };

        private List<char> JokerCardRankingLookup = new List<char>
        {
            'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J'
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

        private HandType GetJokerHandType()
        {
            var jokerCount = Cards.Count(c => c == 'J');
            if (jokerCount == 0)
            {
                return GetHandType();
            }

            if (jokerCount == 5 || jokerCount == 4)
            {
                return HandType.FiveOfAKind;
            }

            var groups = Cards.Where(c => c != 'J').GroupBy(c => c);
            if (jokerCount == 3)
            {
                if (groups.Any(g => g.Count() == 2))
                {
                    return HandType.FiveOfAKind;
                }

                return HandType.FourOfAKind;
            }

            if (jokerCount == 2)
            {
                if (groups.Any(g => g.Count() == 3))
                {
                    return HandType.FiveOfAKind;
                }

                if (groups.Any(g => g.Count() == 2))
                {
                    return HandType.FourOfAKind;
                }

                return HandType.ThreeOfAKind;
            }

            // jokerCount == 1
            if (groups.Any(g => g.Count() == 4))
            {
                return HandType.FiveOfAKind;
            }

            if (groups.Any(g => g.Count() == 3))
            {
                return HandType.FourOfAKind;
            }

            if (groups.Count(g => g.Count() == 2) == 2)
            {
                return HandType.FullHouse;
            }

            if (groups.Any(g => g.Count() == 2))
            {
                return HandType.ThreeOfAKind;
            }

            return HandType.OnePair;
        }

        public override string ToString()
        {
            return $"{Bid:10} - {JokerHandType} {string.Join("", Cards)},  {string.Join(",", JokerCardRankings)}";
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