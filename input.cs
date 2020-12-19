using System.Threading;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public record Input
{
    static Regex RuleParser = new Regex("(?<rule>(?<ruleName>\\w+):\\s(?<range1Start>\\d+)-(?<range1End>\\d+)\\sor\\s(?<range2Start>\\d+)-(?<range2End>\\d+))");

    public List<Rule> Rules { get; } = new List<Rule>();
    public int[] MyTicketData { get; }
    public List<int[]> NearbyTickets { get; } = new List<int[]>();

    public Input(IEnumerable<string> data)
    {
        foreach (var line in data)
        {
            if (line == "")
            {
                break;
            }

            var match = RuleParser.Match(line);
            var range1 = new Range(Convert.ToInt32(match.Groups["range1Start"].Value), Convert.ToInt32(match.Groups["range1End"].Value));
            var range2 = new Range(Convert.ToInt32(match.Groups["range2Start"].Value), Convert.ToInt32(match.Groups["range2End"].Value));
            Rules.Add(new Rule(match.Groups["ruleName"].Value, range1, range2));
        }

        var myTicketHeader = data.IndexOf("your ticket:");
        MyTicketData = data.ElementAt(myTicketHeader + 1).ToTicketData();

        var nearByTicketHeader = data.IndexOf("nearby tickets:");
        for (var index = nearByTicketHeader + 1; index < data.Count(); index++)
        {
            var line = data.ElementAt(index);
            NearbyTickets.Add(line.ToTicketData());
        }
    }
}

public static class Extensions
{
    public static int[] ToTicketData(this string data) => data.Split(',').Select(c => Convert.ToInt32(c)).ToArray();

    public static int IndexOf<T>(this IEnumerable<T> list, T query)
    {
        var index = 0;
        foreach (var item in list)
        {
            if (item.Equals(query))
            {
                return index;
            }

            index++;
        }

        throw new Exception($"{query} not found");
    }
}

public record Rule(string name, Range range1, Range range2)
{
    public bool ValidForRule(int i)
    {
        return range1.Between(i) || range2.Between(i);
    }
}

public record Range(int start, int end)
{
    public bool? Unique { get; set; } = null;

    public bool Between(int i) => i >= this.start && i <= this.end;

    public bool Overlaps(Range other) => Between(other.start) || Between(other.end);
} 