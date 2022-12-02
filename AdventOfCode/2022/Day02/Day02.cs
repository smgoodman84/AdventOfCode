using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;

namespace AdventOfCode._2022.Day02
{
    public class Day02 : Day
    {
        public Day02() : base(2022, 2, "Day02/input_2022_02.txt", "12740", "11980")
        {

        }

        private List<Round> _rounds;
        public override void Initialise()
        {
            _rounds = InputLines
                .Select(line => new Round(line))
                .ToList();
        }

        public override string Part1()
        {
            return _rounds
                .Select(r => r.TotalScore())
                .Sum()
                .ToString();
        }

        public override string Part2()
        {
            return _rounds
                .Select(r => r.TotalScore2())
                .Sum()
                .ToString();
        }

        private class RockPaperScissors
        {
            public enum Move
            {
                Rock,
                Paper,
                Scissors
            }

            public enum Outcome
            {
                Win,
                Lose,
                Draw
            }

            public static Outcome GetOutcome(Move yourMove, Move oppononentMove)
            {
                switch (yourMove)
                {
                    case Move.Rock:
                        switch (oppononentMove)
                        {
                            case Move.Rock:
                                return Outcome.Draw;
                            case Move.Paper:
                                return Outcome.Lose;
                            case Move.Scissors:
                                return Outcome.Win;
                        }
                        break;
                    case Move.Paper:
                        switch (oppononentMove)
                        {
                            case Move.Rock:
                                return Outcome.Win;
                            case Move.Paper:
                                return Outcome.Draw;
                            case Move.Scissors:
                                return Outcome.Lose;
                        }
                        break;
                    case Move.Scissors:
                        switch (oppononentMove)
                        {
                            case Move.Rock:
                                return Outcome.Lose;
                            case Move.Paper:
                                return Outcome.Win;
                            case Move.Scissors:
                                return Outcome.Draw;
                        }
                        break;
                }

                throw new Exception("Unexpected outcome");
            }


            public static Move GetMoveForOutcome(Move oppononentMove, Outcome desiredOutcome)
            {
                switch (oppononentMove)
                {
                    case Move.Rock:
                        switch (desiredOutcome)
                        {
                            case Outcome.Win:
                                return Move.Paper;
                            case Outcome.Draw:
                                return Move.Rock;
                            case Outcome.Lose:
                                return Move.Scissors;
                        }
                        break;
                    case Move.Paper:
                        switch (desiredOutcome)
                        {
                            case Outcome.Win:
                                return Move.Scissors;
                            case Outcome.Draw:
                                return Move.Paper;
                            case Outcome.Lose:
                                return Move.Rock;
                        }
                        break;
                    case Move.Scissors:
                        switch (desiredOutcome)
                        {
                            case Outcome.Win:
                                return Move.Rock;
                            case Outcome.Draw:
                                return Move.Scissors;
                            case Outcome.Lose:
                                return Move.Paper;
                        }
                        break;
                }

                throw new Exception("Unexpected outcome");
            }
        }

        private class Round
        {
            private RockPaperScissors.Move _opponentMove { get; set; }

            private RockPaperScissors.Move _myMove { get; set; }
            private RockPaperScissors.Outcome _outcome { get; set; }

            private RockPaperScissors.Move _myMove2 { get; set; }
            private RockPaperScissors.Outcome _outcome2 { get; set; }

            public Round(string round)
            {
                var moves = round.Split(" ");

                _opponentMove = ParseMove(moves[0]);

                _myMove = ParseMove(moves[1]);
                _outcome = RockPaperScissors.GetOutcome(_myMove, _opponentMove);

                _outcome2 = ParseOutcome(moves[1]);
                _myMove2 = RockPaperScissors.GetMoveForOutcome(_opponentMove, _outcome2);
            }

            private RockPaperScissors.Move ParseMove(string input)
            {
                switch (input)
                {
                    case "A": return RockPaperScissors.Move.Rock;
                    case "B": return RockPaperScissors.Move.Paper;
                    case "C": return RockPaperScissors.Move.Scissors;
                    case "X": return RockPaperScissors.Move.Rock;
                    case "Y": return RockPaperScissors.Move.Paper;
                    case "Z": return RockPaperScissors.Move.Scissors;
                }

                throw new Exception("Unknown move");
            }

            private RockPaperScissors.Outcome ParseOutcome(string input)
            {
                switch (input)
                {
                    case "X": return RockPaperScissors.Outcome.Lose;
                    case "Y": return RockPaperScissors.Outcome.Draw;
                    case "Z": return RockPaperScissors.Outcome.Win;
                }

                throw new Exception("Unknown outcome");
            }


            public int ShapeScore(RockPaperScissors.Move move)
            {
                switch (move)
                {
                    case RockPaperScissors.Move.Rock: return 1;
                    case RockPaperScissors.Move.Paper: return 2;
                    case RockPaperScissors.Move.Scissors: return 3;
                }

                throw new Exception("Unknown move");
            }


            public int OutcomeScore(RockPaperScissors.Outcome outcome)
            {
                switch (outcome)
                {
                    case RockPaperScissors.Outcome.Win: return 6;
                    case RockPaperScissors.Outcome.Draw: return 3;
                    case RockPaperScissors.Outcome.Lose: return 0;
                }

                throw new Exception("Unknown shape");
            }

            public int TotalScore() => ShapeScore(_myMove) + OutcomeScore(_outcome);
            public int TotalScore2() => ShapeScore(_myMove2) + OutcomeScore(_outcome2);
        }
    }
}

