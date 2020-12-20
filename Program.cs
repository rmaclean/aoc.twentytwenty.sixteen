using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

var data = await File.ReadAllLinesAsync("data.txt");
var input = new Input(data);
var result = new List<int>();
foreach (var row in input.NearbyTickets)
{
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
            result.Add(value);
        }
    }
}

var total = result.Sum();
Console.WriteLine($"Result {total}");
