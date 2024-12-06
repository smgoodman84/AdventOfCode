using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day05;

public class Day05 : Day
{
    public Day05() : base(2024, 5, "Day05/input_2024_05.txt", "4957", "6938")
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
        var incorrectlyOrdered = _pageOrders
            .Where(x => !x.FollowsRules(_orderingRules))
            .ToList();

        var reordered = incorrectlyOrdered
            .Select(x => x.ApplyOrder(_orderingRules))
            .ToList();

        var result = reordered.Sum(x => x.GetMiddlePage());

        return result.ToString();
    }

    private class OrderingRule
    {
        public int Left { get; }
        public int Right { get; }

        private string _orderingRule;

        public OrderingRule(string orderingRule)
        {
            _orderingRule = orderingRule;
            var pageNumbers = orderingRule.Split('|');
            Left = int.Parse(pageNumbers[0]);
            Right = int.Parse(pageNumbers[1]);
        }

        public override string ToString()
        {
            return _orderingRule;
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

        public PageOrder(List<int> order)
        {
            _pageOrder = string.Join(",", order);
            Order = order.ToList();
        }

        public override string ToString()
        {
            return _pageOrder;
        }

        public PageOrder ApplyOrder(List<OrderingRule> orderingRules)
        {
            var applicableRules = orderingRules
                .Where(r => Order.Contains(r.Left))
                .Where(r => Order.Contains(r.Right))
                .ToList();

            var distinctPages = Order.ToList();

            var remainingRules = applicableRules.ToList();
            var orderedPages = new List<int>();
            while (distinctPages.Any())
            {
                var leftMostPage = distinctPages
                    .Where(p => !remainingRules.Any(r => r.Right == p))
                    .First();

                orderedPages.Add(leftMostPage);

                distinctPages.Remove(leftMostPage);
                remainingRules.RemoveAll(r => r.Left == leftMostPage);
            }

            return new PageOrder(orderedPages);
        }

        public PageOrder ApplyOrder(List<int> pageOrder)
        {
            var filtered = pageOrder.Where(p => Order.Contains(p)).ToList();

            return new PageOrder(filtered);
        }

        public bool FollowsRules(List<OrderingRule> orderingRules)
        {
            // Console.WriteLine($"Checking {this}");
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
                    // Console.WriteLine($"{rule.Left} should be before {rule.Right}");
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