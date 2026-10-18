// Labels for the no-match and multi-match policies (rs_nomatchpolicy, rs_multimatchpolicy).
import {
  rs_lookupdefinition_rs_multimatchpolicy as MultiMatch,
  rs_lookupdefinition_rs_nomatchpolicy as NoMatch,
} from "../../../typings/XRM/OptionSets";

export { MultiMatch, NoMatch };

export const NO_MATCH_OPTIONS = [
  { value: NoMatch.Block, label: "Block" },
  { value: NoMatch.Usedefault, label: "Use default" },
  { value: NoMatch.Skip, label: "Skip" },
];

export const MULTI_MATCH_OPTIONS = [
  { value: MultiMatch.Mostspecific, label: "Most specific" },
  { value: MultiMatch.Firstbysort, label: "First by sort" },
  { value: MultiMatch.Error, label: "Error" },
];

export function policyLabel(options: { value: number; label: string }[], value: number | null): string {
  return options.find((o) => o.value === value)?.label ?? "Not set";
}
