using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>Calls a registered custom function. The engine decides about mocking and retries.</summary>
public interface IFunctionInvoker
{
    // Calls the function's Custom API with typed inputs and reads the uniform result contract.
    FunctionResult Invoke(CompiledFunction customFunction, IDictionary<string, object?> inputs);

    // A registered function by Custom API name or display name, for NCalc formulas; null when none is registered.
    CompiledFunction? Find(string name);
}
