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
            return _rounds.Select(r => r.TotalScore()).Sum().ToString();
        }

        public override string Part2()
        {
            return _rounds.Select(r => r.TotalScore2()).Sum().ToString();
        }

        private enum RPS
        {
            Rock,
            Paper,
            Scissors
        }


        private enum Outcome
        {
            Win,
            Lose,
            Draw
        }

        private class Round
        {
            private RPS _opponentMove { get; set; }
            private RPS _myMove { get; set; }
            private RPS _myMove2 { get; set; }
            private Outcome _outcome2 { get; set; }
            public Round(string round)
            {
                var moves = round.Split(" ");
                _opponentMove = ParseMove(moves[0]);
                _myMove = ParseMove(moves[1]);
                _outcome2 = ParseOutcome(moves[1]);


                switch(_outcome2)
                {
                    case Outcome.Win:
                        switch (_opponentMove)
                        {
                            case RPS.Rock:
                                _myMove2 = RPS.Paper;
                                break;
                            case RPS.Paper:
                                _myMove2 = RPS.Scissors;
                                break;
                            case RPS.Scissors:
                                _myMove2 = RPS.Rock;
                                break;
                        }
                        break;
                    case Outcome.Draw:
                        _myMove2 = _opponentMove;
                        break;
                    case Outcome.Lose:
                        switch (_opponentMove)
                        {
                            case RPS.Rock:
                                _myMove2 = RPS.Scissors;
                                break;
                            case RPS.Paper:
                                _myMove2 = RPS.Rock;
                                break;
                            case RPS.Scissors:
                                _myMove2 = RPS.Paper;
                                break;
                        }
                        break;
                }
            }

            private RPS ParseMove(string input)
            {
                switch (input)
                {
                    case "A": return RPS.Rock;
                    case "B": return RPS.Paper;
                    case "C": return RPS.Scissors;
                    case "X": return RPS.Rock;
                    case "Y": return RPS.Paper;
                    case "Z": return RPS.Scissors;
                }

                throw new Exception("Unknown shape");
            }

            private Outcome ParseOutcome(string input)
            {
                switch (input)
                {
                    case "X": return Outcome.Lose;
                    case "Y": return Outcome.Draw;
                    case "Z": return Outcome.Win;
                }

                throw new Exception("Unknown outcome");
            }


            public int ShapeScore()
            {
                switch (_myMove)
                {
                    case RPS.Rock: return 1;
                    case RPS.Paper: return 2;
                    case RPS.Scissors: return 3;
                }

                throw new Exception("Unknown shape");
            }


            public int OutcomeScore()
            {

                switch (_myMove)
                {
                    case RPS.Rock: return _opponentMove == RPS.Scissors ? 6 : _opponentMove == RPS.Rock ? 3 : 0;
                    case RPS.Paper: return _opponentMove == RPS.Rock ? 6 : _opponentMove == RPS.Paper ? 3 : 0;
                    case RPS.Scissors: return _opponentMove == RPS.Paper ? 6 : _opponentMove == RPS.Scissors ? 3 : 0;
                }

                throw new Exception("Unknown shape");
            }



            public int ShapeScore2()
            {
                switch (_myMove2)
                {
                    case RPS.Rock: return 1;
                    case RPS.Paper: return 2;
                    case RPS.Scissors: return 3;
                }

                throw new Exception("Unknown shape");
            }



            public int OutcomeScore2()
            {
                switch (_outcome2)
                {
                    case Outcome.Lose: return 0;
                    case Outcome.Draw: return 3;
                    case Outcome.Win: return 6;
                }

                throw new Exception("Unknown outcome");
            }

            public int TotalScore() => ShapeScore() + OutcomeScore();
            public int TotalScore2() => ShapeScore2() + OutcomeScore2();
        }
    }
}

