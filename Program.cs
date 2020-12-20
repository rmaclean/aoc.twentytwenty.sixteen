using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

var data = await File.ReadAllLinesAsync("data.txt");
var input = new Input(data);
var validTickets = GetValidTickets();


var possibles = new Dictionary<int, List<Rule>>();
for (var column = 0; column < input.Rules.Count; column++)
{
    possibles[column] = new List<Rule>();
    foreach (var rule in input.Rules)
    {
        var ruleValidForColumn = true;
        foreach (var row in validTickets)
        {
            var value = row[column];
            if (!rule.range1.Between(value) && !rule.range2.Between(value))
            {
                ruleValidForColumn = false;
                break;
            }
        }

        if (ruleValidForColumn)
        {
            possibles[column].Add(rule);
        }
    }
}

var alignment = new Dictionary<Rule, int>();
while (possibles.Any())
{
    var unique = possibles.Single(i => i.Value.Count == 1);
    var rule = unique.Value.Single();
    alignment[rule] = unique.Key;

    possibles.Remove(unique.Key);

    foreach (var possible in possibles)
    {
        possible.Value.Remove(rule);
    }
}

var result = alignment
    .Where(i => i.Key.name.StartsWith("departure"))
    .Select(i => i.Value)
    .Select(col => input.MyTicketData[col])
    .Aggregate(1L, (curr, next) => curr *= next);

Console.WriteLine(result);

List<int[]> GetValidTickets()
{
    var validTickets = new List<int[]>();
    foreach (var row in input.NearbyTickets)
    {
        var validTicket = true;
        for (var index = 0; index < row.Length; index++)
        {
            var value = row[index];
            var validForAtleastOne = false;
            foreach (var rule in input.Rules)
            {
                if (rule.range1.Between(value) || rule.range2.Between(value))
                {
                    validForAtleastOne = true;
                    break;
                }
            }

            if (!validForAtleastOne)
            {
                validTicket = false;
                break;
            }
        }

        if (validTicket)
        {
            validTickets.Add(row);
        }
    }
    return validTickets;
}

