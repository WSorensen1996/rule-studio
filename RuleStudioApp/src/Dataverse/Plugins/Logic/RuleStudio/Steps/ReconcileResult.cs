using System.Globalization;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Steps;

/// <summary>What a step reconciliation did, and what it could not do.</summary>
public sealed class ReconcileResult
{
    public bool EngineRegistered { get; set; }

    public int Created { get; set; }

    public int Updated { get; set; }

    public int Deleted { get; set; }

    public int Unchanged { get; set; }

    // Publish warnings (severity warning; codes ENGINE_NOT_REGISTERED, STEP_NOT_SUPPORTED).
    public IList<ValidationIssue> Warnings { get; } = new List<ValidationIssue>();

    // One line for the rs_ReconcileSteps Summary output.
    public string Summary()
    {
        var text = EngineRegistered
            ? string.Format(CultureInfo.InvariantCulture, "Engine steps: {0} created, {1} updated, {2} deleted, {3} unchanged.", Created, Updated, Deleted, Unchanged)
            : "Engine steps were not reconciled.";
        return Warnings.Count == 0 ? text : text + " " + string.Join(" ", Warnings.Select(w => w.Message));
    }
}
