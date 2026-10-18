namespace DataverseLogic.RuleStudio.Release;

/// <summary>The row an issue points at.</summary>
internal readonly record struct Location(string Kind, Guid Id, string Label);
