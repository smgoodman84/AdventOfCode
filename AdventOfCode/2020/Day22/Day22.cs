using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day22
{
    public class Day22 : Day
    {
        public Day22() : base(2020, 22, "Day22/input_2020_22.txt", "33680", string.Empty)
        {

        }

        private Deck _playerOne;
        private Deck _playerTwo;

        public override void Initialise()
        {
            var lines = InputLines.ToList();

            lines.Add(string.Empty);

            var decks = new List<Deck>();
            var player = 1;
            Deck currentDeck = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("Player"))
                {
                    currentDeck = new Deck(player);
                    player += 1;
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    decks.Add(currentDeck);
                }
                else
                {
                    currentDeck.AddToBottom(int.Parse(line));
                }
            }

            _playerOne = decks[0];
            _playerTwo = decks[1];
        }

        public override string Part1()
        {
            while (!_playerOne.IsEmpty() && !_playerTwo.IsEmpty())
            {
                var winner = _playerOne;
                var loser = _playerTwo;

                if (_playerTwo.NextCard() > _playerOne.NextCard())
                {
                    winner = _playerTwo;
                    loser = _playerOne;
                }

                winner.AddToBottom(winner.TakeCard());
                winner.AddToBottom(loser.TakeCard());
            }

            var gameWinner = _playerOne.IsEmpty() ? _playerTwo : _playerOne;

            return gameWinner.GetScore().ToString();
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private class Deck
        {
            private Queue<int> _deck = new Queue<int>();
            private int _player;
            public Deck(int player)
            {
                _player = player;
            }

            public void AddToBottom(int card) => _deck.Enqueue(card);

            public int TakeCard() => _deck.Dequeue();
            public int NextCard() => _deck.Peek();

            public bool IsEmpty() => !_deck.Any();

            public int GetScore()
            {
                var result = 0;
                var multiplier = _deck.Count;
                foreach (var card in _deck)
                {
                    result += card * multiplier;
                    multiplier -= 1;
                }

                return result;
            }
        }
    }
}
