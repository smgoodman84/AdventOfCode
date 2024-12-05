using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day04;

public class Day05 : Day
{
    public Day05() : base(2024, 5, "Day05/input_2024_05.txt", "4957", "")
    {

    }

    private List<OrderingRule> _orderingRules = new List<OrderingRule>();
    private List<PageOrder> _pageOrders = new List<PageOrder>();
    public override void Initialise()
    {
        var orderingRules = true;
        foreach (var line in InputLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                orderingRules = false;
                continue;
            }

            if (orderingRules)
            {
                _orderingRules.Add(new OrderingRule(line));
                continue;
            }

            _pageOrders.Add(new PageOrder(line));
        }
    }

    public override string Part1()
    {
        var result = 0;
        foreach (var pageOrder in _pageOrders)
        {
            if (pageOrder.FollowsRules(_orderingRules))
            {
                result += pageOrder.GetMiddlePage();
            }
        }

        return result.ToString();
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private class OrderingRule
    {
        public int Left { get; }
        public int Right { get; }

        public OrderingRule(string orderingRule)
        {
            var pageNumbers = orderingRule.Split('|');
            Left = int.Parse(pageNumbers[0]);
            Right = int.Parse(pageNumbers[1]);
        }
    }

    private class PageOrder
    {
        public List<int> Order { get; }
        private string _pageOrder;
        public PageOrder(string pageOrder)
        {
            _pageOrder = pageOrder;
            Order = pageOrder.Split(',').Select(int.Parse).ToList();
        }

        public override string ToString()
        {
            return _pageOrder;
        }

        public bool FollowsRules(List<OrderingRule> orderingRules)
        {
            Console.WriteLine($"Checking {this}");
            foreach (var rule in orderingRules)
            {
                var leftIndex = Order.IndexOf(rule.Left);
                var rightIndex = Order.IndexOf(rule.Right);

                if (leftIndex == -1 || rightIndex == -1)
                {
                    continue;
                }

                if (leftIndex > rightIndex)
                {
                    Console.WriteLine($"{rule.Left} should be before {rule.Right}");
                    return false;
                }
            }

            return true;
        }

        public int GetMiddlePage()
        {
            var middle = (Order.Count - 1) / 2;
            return Order[middle];
        }
    }
}