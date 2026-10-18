using DataverseLogic.RuleStudio.Steps;
using RuleStudio.SharedContext;
using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Enabling or disabling a rule adds or removes its engine steps at once.</summary>
public class ReconcileOnRuleEnabled : Plugin
{
    public ReconcileOnRuleEnabled()
    {
        RegisterStep<rs_Rule, StepReconciler>(EventOperation.Update, ExecutionStage.PostOperation, service => service.Reconcile())
            .AddFilteredAttributes(x => x.rs_Enabled);
    }
}
