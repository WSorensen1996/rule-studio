namespace RuleStudio.Engine;

/// <summary>A published version found by <see cref="RuleCatalog"/>, before its snapshot is parsed.</summary>
internal sealed class CatalogRow(Guid versionId, string hash)
{
    internal Guid VersionId { get; } = versionId;

    internal string Hash { get; } = hash;
}
