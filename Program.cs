using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

var data = await File.ReadAllLinesAsync("data.txt");
var input = new Input(data);
DetermineUniqueRules();
var alignment = new Dictionary<int, Rule>();
var positionsToDetermine = Enumerable.Range(0, input.MyTicketData.Length).ToList();
var rulesToFindAHome = input.Rules.ToList();
DetermineUniqueAlignment();
DetermineNonUniqueAlignment();

var result = DetermineInvalidValues();
var issues = result.Aggregate("", (curr, next) => curr += next.ToString() + ", ");
Console.WriteLine(issues);
Console.WriteLine($"Answer is {result.Sum()}");

List<int> DetermineInvalidValues()
{
    var invalidValues = new List<int>();

    foreach (var otherTicket in input.NearbyTickets)
    {
        for (var index = 0; index < alignment.Count; index++)
        {
            var rule = alignment[index];
            var fieldToTest = otherTicket[index];
            if (!rule.ValidForRule(fieldToTest))
            {
                invalidValues.Add(fieldToTest);
            }
        }
    }

    return invalidValues;
}

void DetermineNonUniqueAlignment()
{
    var unknownPositions = positionsToDetermine.ToList();
    foreach (var index in unknownPositions)
    {
        Rule matchedRule = null;
        foreach (var rule in rulesToFindAHome)
        {
            if (rule.range1.Between(input.MyTicketData[index]) || rule.range2.Between(input.MyTicketData[index]))
            {
                if (matchedRule == null)
                {
                    matchedRule = rule;
                }
                else
                {
                    matchedRule = null;
                    break;
                }
            }
        }

        if (matchedRule != null)
        {
            UpdateFoundAlignment(matchedRule, index);
        }
    }
}

void UpdateFoundAlignment(Rule rule, int index)
{
    rulesToFindAHome.Remove(rule);
    positionsToDetermine.Remove(index);
    alignment.Add(index, rule);
}

bool TestUniqueRange(Range range, List<int> positionsToDetermine, Rule rule)
{
    if (!range.Unique.Value)
    {
        return false;
    }

    var found = false;
    var index = 0;
    for (index = 0; index < positionsToDetermine.Count; index++)
    {
        if (range.Between(positionsToDetermine[index]))
        {
            found = true;
            break;
        }
    }

    if (found) UpdateFoundAlignment(rule, index);

    return found;
}

void DetermineUniqueAlignment()
{
    foreach (var rule in input.Rules)
    {
        var range1Test = TestUniqueRange(rule.range1, positionsToDetermine, rule);
        if (!range1Test)
        {
            TestUniqueRange(rule.range2, positionsToDetermine, rule);
        }
    }
}

bool CheckRange(Range rangeToCheck, Rule ruleToCompare)
{
    if (rangeToCheck.Unique.HasValue && !rangeToCheck.Unique.Value)
    {
        return false;
    }

    var range1 = rangeToCheck.Overlaps(ruleToCompare.range1);
    var range2 = rangeToCheck.Overlaps(ruleToCompare.range2);

    if (!range1 && !range2) return true;

    rangeToCheck.Unique = false;
    if (range1) ruleToCompare.range1.Unique = false;

    if (range2) ruleToCompare.range2.Unique = false;
    return false;
}

void DetermineUniqueRules()
{
    for (var index = 0; index < input.Rules.Count - 1; index++)
    {
        var ruleToCheck = input.Rules[index];
        if ((ruleToCheck.range1.Unique.HasValue && !ruleToCheck.range1.Unique.Value) &&
            (ruleToCheck.range2.Unique.HasValue && !ruleToCheck.range2.Unique.Value))
        {
            continue;
        }

        var range1Unique = true;
        var range2Unique = true;
        var range1ToCheck = ruleToCheck.range1;
        var range2ToCheck = ruleToCheck.range2;

        for (var secondIndex = index + 1; secondIndex < input.Rules.Count; secondIndex++)
        {
            var ruleToCompare = input.Rules[secondIndex];
            range1Unique = CheckRange(range1ToCheck, ruleToCompare);
            range2Unique = CheckRange(range2ToCheck, ruleToCompare);

            if (!range1Unique && !range2Unique)
            {
                break;
            }
        }

        range1ToCheck.Unique = range1Unique;
        range2ToCheck.Unique = range2Unique;
    }

    var last = input.Rules.Last();
    if (!last.range1.Unique.HasValue) last.range1.Unique = true;
    if (!last.range2.Unique.HasValue) last.range2.Unique = true;
}



