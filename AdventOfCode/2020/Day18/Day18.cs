using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day18;

public class Day18 : Day
{
    public Day18() : base(2020, 18, "Day18/input_2020_18.txt", "12956356593940", string.Empty)
    {

    }

    private List<string> _expressions;
    public override void Initialise()
    {
        _expressions = InputLines;
    }

    public override string Part1()
    {
        return _expressions
            .Select(EvaluateExpression)
            .Sum()
            .ToString();
    }

    private long EvaluateExpression(string expression)
    {
        var tokens = expression
            .Replace("(", " ( ")
            .Replace(")", " ) ")
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        return EvaluateExpression(tokens);
    }

    private long EvaluateExpression(List<string> tokens)
    {
        long value = 0;
        var @operator = "+";
        var readingSubExpression = false;
        var subExpression = new List<string>();
        var depth = 0;
        foreach (var token in tokens)
        {
            if (readingSubExpression)
            {
                switch (token)
                {
                    case ")":
                        if (depth == 0)
                        {
                            switch (@operator)
                            {
                                case "+":
                                    value += EvaluateExpression(subExpression);
                                    break;
                                case "*":
                                    value *= EvaluateExpression(subExpression);
                                    break;
                            }

                            readingSubExpression = false;
                            subExpression = new List<string>();
                            depth = 0;
                        }
                        else
                        {
                            depth -= 1;
                            subExpression.Add(token);
                        }
                        break;
                    case "(":
                        depth += 1;
                        subExpression.Add(token);
                        break;
                    default:
                        subExpression.Add(token);
                        break;
                }
            }
            else
            {
                switch (token)
                {
                    case "+":
                    case "*":
                        @operator = token;
                        break;
                    case "(":
                        readingSubExpression = true;
                        break;
                    default:
                        var parsedValue = long.Parse(token);
                        switch (@operator)
                        {
                            case "+":
                                value += parsedValue;
                                break;
                            case "*":
                                value *= parsedValue;
                                break;
                        }
                        break;
                }
            }
        }

        return value;
    }

    public override string Part2()
    {
        return string.Empty;
    }
}