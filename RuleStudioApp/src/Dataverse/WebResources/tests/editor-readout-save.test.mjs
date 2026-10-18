// Unit tests of the plain-words readout (readoutView.ts) and the save changeset (ruleSave.ts).
import assert from 'node:assert/strict';
import { before, beforeEach, test } from 'node:test';

import { installDom } from './support/dom.mjs';
import { load } from './support/load.mjs';

let model;
let readout;
let save;
let element;

const CATALOG = { functions: [{ id: 'f1', name: 'Rate' }], lookups: [{ id: 'l1', name: 'Discount cap' }] };

before(async () => {
    element = installDom();
    model = await load('html/rule-editor/model');
    readout = await load('html/rule-editor/readoutView');
    save = await load('html/rule-editor/ruleSave');
    globalThis.__tables = {
        quote: {
            label: 'Quote',
            columns: [
                { logicalName: 'name', label: 'Name', kind: 'text', options: [], targets: [] },
                { logicalName: 'ca_total', label: 'Total', kind: 'number', options: [], targets: [] },
                { logicalName: 'ca_approved', label: 'Approved', kind: 'boolean', options: [], targets: [] },
                { logicalName: 'ca_stage', label: 'Stage', kind: 'choice', options: [{ value: 1, label: 'Draft' }, { value: 2, label: 'Won' }], targets: [] },
                { logicalName: 'ca_customer', label: 'Customer', kind: 'lookup', options: [], targets: ['account'] },
            ],
        },
        account: { label: 'Account', columns: [{ logicalName: 'ca_segment', label: 'Segment', kind: 'text', options: [], targets: [] }] },
        ca_quoteline: { label: 'Quote line', columns: [{ logicalName: 'ca_product', label: 'Product', kind: 'text', options: [], targets: [] }, { logicalName: 'ca_amount', label: 'Amount', kind: 'number', options: [], targets: [] }] },
    };
});

beforeEach(() => {
    globalThis.__changesets = [];
});

const path = (column, ...hops) => ({ table: 'quote', hops, column });
const parent = { kind: 'parent', attribute: 'ca_customer', table: 'account' };
const child = { kind: 'child', attribute: 'ca_quote', table: 'ca_quoteline' };

function fixed(json) {
    return { ...model.emptyValue(), value: JSON.stringify(json) };
}

function condition(fieldPath, operator, value = model.emptyValue(), aggregate = model.Aggregate.None) {
    return { ...model.newCondition(), fieldPath, operator, value, aggregate };
}

function action(type, settings = {}, branch = model.Branch.Then) {
    return { ...model.newAction(branch), actionType: type, ...settings };
}

function state(conditions = [], actions = [], versionSettings = {}, rootOperator = model.Logical.AND) {
    const version = { ...model.newVersion(1), targetTable: 'quote', ...versionSettings };
    version.root = { ...model.newGroup(rootOperator), children: conditions };
    version.actions = actions;
    return { rule: { ...model.newRule(), ruleKey: 'QUOTE.TEST', name: 'Test' }, version, removed: model.noneRemoved(), replacesDraftId: null, ruleDirty: true, versionDirty: true };
}

async function sentence(s) {
    const container = element();
    readout.renderReadout(container, s, CATALOG);
    for (let i = 0; i < 20 && container.childNodes.length === 0; i++) await new Promise((r) => setTimeout(r, 0));
    return container.textContent;
}

test('without a table the readout asks for one', async () => {
    assert.equal(await sentence(state([], [], { targetTable: '' })), 'Choose a target table in section 1 to see the rule in plain words.');
});

test('trigger, no conditions and no actions read as one sentence', async () => {
    assert.equal(await sentence(state()), "WHEN a Quote is updated, before it's saved, IF always (there are no conditions), THEN do nothing ELSE do nothing");
    assert.equal(
        await sentence(state([], [], { event: model.RuleEvent.Create, stage: model.Stage.Aftersave })),
        "WHEN a Quote is created, after it's saved, IF always (there are no conditions), THEN do nothing ELSE do nothing");
});

test('filtered columns name the columns that must be part of the save', async () => {
    assert.match(await sentence(state([], [], { filteredColumns: 'name, ca_total' })), /and Name or Total are part of the save,/);
    assert.doesNotMatch(await sentence(state([], [], { event: model.RuleEvent.Create, filteredColumns: 'name' })), /part of the save/);
});

test('conditions use column labels, choice labels and the joining word, with nested groups in brackets', async () => {
    const nested = { ...model.newGroup(model.Logical.OR), children: [condition(path('ca_total'), model.Operator.Greaterthan, fixed(100)), condition(path('ca_approved'), model.Operator.Equals, fixed(true))] };
    const text = await sentence(state([condition(path('ca_stage'), model.Operator.Equals, fixed(2)), nested]));
    assert.match(text, /IF the Stage is Won and \(the Total is more than 100 or the Approved is Yes\), THEN/);
});

test('every operator reads naturally', async () => {
    const cases = [
        [model.Operator.Notequals, fixed('x'), 'is not x'],
        [model.Operator.Greaterorequal, fixed(1), 'is at least 1'],
        [model.Operator.Lessthan, fixed(1), 'is less than 1'],
        [model.Operator.Lessorequal, fixed(1), 'is at most 1'],
        [model.Operator.Contains, fixed('Co'), 'contains Co'],
        [model.Operator.Startswith, fixed('Co'), 'starts with Co'],
        [model.Operator.Isempty, model.emptyValue(), 'is empty'],
        [model.Operator.Isnotempty, model.emptyValue(), 'is filled in'],
        [model.Operator.Changed, model.emptyValue(), 'has changed'],
        [model.Operator.Inlist, fixed(['a', 'b']), 'is one of a, b'],
        [model.Operator.Between, fixed([1, 9]), 'is between 1 and 9'],
    ];
    for (const [operator, value, words] of cases) {
        assert.match(await sentence(state([condition(path('name'), operator, value)])), new RegExp(`IF the Name ${words}, THEN`), `operator ${operator}`);
    }
});

test('parent paths read as possessives and child aggregates as quantities', async () => {
    assert.match(await sentence(state([condition(path('ca_segment', parent), model.Operator.Equals, fixed('SMB'))])), /IF the Customer's Segment is SMB,/);
    const aggregate = async (agg, op, value) => sentence(state([condition(path('ca_product', child), op, value, agg)]));
    assert.match(await aggregate(model.Aggregate.Any, model.Operator.Equals, fixed('X')), /at least one Quote line has Product that is X/);
    assert.match(await aggregate(model.Aggregate.All, model.Operator.Equals, fixed('X')), /every Quote line has Product that is X \(also true when there are none\)/);
    assert.match(await aggregate(model.Aggregate.Noneof, model.Operator.Equals, fixed('X')), /no Quote line has Product that is X/);
    assert.match(await aggregate(model.Aggregate.Count, model.Operator.Greaterthan, fixed(2)), /the number of Quote line records is more than 2/);
    assert.match(await sentence(state([condition(path('ca_amount', child), model.Operator.Greaterthan, fixed(10), model.Aggregate.Sum)])), /the sum of Amount on Quote line records is more than 10/);
    assert.match(await aggregate(model.Aggregate.None, model.Operator.Equals, fixed('X')), /\(choose Any, All, None-of or a count\)/);
});

test('value sources other than fixed are named', async () => {
    const cases = [
        [{ ...model.emptyValue(), sourceType: model.SourceType.Fieldpath, valuePath: path('name') }, 'the Name'],
        [{ ...model.emptyValue(), sourceType: model.SourceType.Expression, value: '[ca_total] * 2' }, 'the formula \\[ca_total\\] \\* 2'],
        [{ ...model.emptyValue(), sourceType: model.SourceType.Lookup, lookupDefinitionId: 'l1' }, 'the lookup Discount cap'],
        [{ ...model.emptyValue(), sourceType: model.SourceType.Function, customFunctionId: 'f1' }, 'the result of Rate\\(\\)'],
        [{ ...model.emptyValue(), sourceType: model.SourceType.Lookup, lookupDefinitionId: 'gone' }, 'the lookup \\(not chosen\\)'],
    ];
    for (const [value, words] of cases) {
        assert.match(await sentence(state([condition(path('ca_total'), model.Operator.Equals, value)])), new RegExp(`is ${words}`));
    }
});

test('actions are listed in order on both branches', async () => {
    const actions = [
        action(model.ActionType.Setfield, { targetField: 'ca_total', value: fixed(5) }),
        action(model.ActionType.Logmessage, { message: 'Checked.', messageLevel: 126250001 }),
        action(model.ActionType.Blocksave, { message: 'No discount.' }, model.Branch.Else),
        action(model.ActionType.Callfunction, { value: { ...model.emptyValue(), sourceType: model.SourceType.Function, customFunctionId: 'f1' } }, model.Branch.Else),
    ];
    const text = await sentence(state([], actions));
    assert.match(text, /THEN set Total to 5, then write a warning to the execution log: “Checked\.” ELSE stop the save and show “No discount\.”, then call Rate$/);
});

test('a new rule saves the rule, the version, the tree and the draft pointer in one changeset', async () => {
    const s = state(
        [condition(path('name'), model.Operator.Equals, fixed('A')), { ...model.newGroup(model.Logical.OR), children: [condition(path('ca_total'), model.Operator.Greaterthan, fixed(1))] }],
        [action(model.ActionType.Setfield, { targetField: 'ca_total', value: fixed(5) }), action(model.ActionType.Blocksave, { message: 'No.' }, model.Branch.Else)]);
    await save.saveState(s);

    const requests = globalThis.__changesets.at(-1);
    const summary = requests.map((r) => `${r.method} ${r.path.replace(/\(.*\)/, '(id)')}`);
    assert.deepEqual(summary, [
        'POST rs_rules', 'POST rs_ruleversions', 'POST rs_conditiongroups', 'POST rs_conditions', 'POST rs_conditiongroups', 'POST rs_conditions', 'POST rs_actions', 'POST rs_actions', 'PATCH rs_rules(id)',
    ]);
    const [rule, version, root, first, inner, second] = requests;
    assert.equal(rule.body.rs_ruleid, s.rule.id);
    assert.equal(version.body['rs_RuleId@odata.bind'], `/rs_rules(${s.rule.id})`);
    assert.equal(version.body.rs_name, 'QUOTE.TEST v1');
    assert.equal(version.body.rs_status, model.VersionStatus.Draft);
    assert.equal(version.body.rs_loglevel, null);
    assert.equal(root.body.rs_name, 'Root');
    assert.equal('rs_ParentGroupId@odata.bind' in root.body, false, 'a new root has no parent bind');
    assert.equal(inner.body['rs_ParentGroupId@odata.bind'], `/rs_conditiongroups(${root.body.rs_conditiongroupid})`);
    assert.equal(first.body.rs_fieldpath, '{"table":"quote","hops":[],"column":"name"}');
    assert.equal(first.body.rs_value, '"A"');
    assert.equal(second.body['rs_ConditionGroupId@odata.bind'], `/rs_conditiongroups(${inner.body.rs_conditiongroupid})`);
    assert.equal(requests[7].body.rs_name, 'Else 1');
    assert.equal(requests[8].body['rs_DraftVersionId@odata.bind'], `/rs_ruleversions(${s.version.id})`);
});

test('removed rows are deleted first, children before parents', async () => {
    const s = state();
    s.rule.isNew = false;
    s.version.isNew = false;
    s.removed = { mappings: ['m1'], conditions: ['c1', 'c1'], groups: ['g1'], actions: ['a1'] };
    await save.saveState(s);
    const deletes = globalThis.__changesets.at(-1).filter((r) => r.method === 'DELETE').map((r) => r.path);
    assert.deepEqual(deletes, ['rs_actioninputmappings(m1)', 'rs_conditions(c1)', 'rs_conditiongroups(g1)', 'rs_actions(a1)']);
});

test('a rule-only change saves only the rule', async () => {
    const s = { ...state(), versionDirty: false };
    s.rule.isNew = false;
    await save.saveState(s);
    assert.deepEqual(globalThis.__changesets.at(-1).map((r) => r.method + ' ' + r.path), [`PATCH rs_rules(${s.rule.id})`]);
});

test('a new draft that replaces one marks the old draft discarded', async () => {
    const s = state();
    s.rule.isNew = false;
    s.replacesDraftId = 'old';
    await save.saveState(s);
    const discard = globalThis.__changesets.at(-1).find((r) => r.path === 'rs_ruleversions(old)');
    assert.deepEqual(discard.body, { rs_status: model.VersionStatus.Discarded });
});

test('function inputs are saved, and emptied saved inputs are deleted', async () => {
    const fn = { ...model.emptyValue(), sourceType: model.SourceType.Function, customFunctionId: 'f1' };
    const kept = { ...model.newMapping('Text'), value: fixed('hi') };
    const emptied = { ...model.newMapping('Other'), isNew: false };
    const s = state([], [action(model.ActionType.Callfunction, { value: fn, mappings: [kept, emptied] })]);
    await save.saveState(s);
    const requests = globalThis.__changesets.at(-1);
    const mapping = requests.find((r) => r.path === 'rs_actioninputmappings');
    assert.equal(mapping.body.rs_parametername, 'Text');
    assert.equal(mapping.body['rs_ActionId@odata.bind'], `/rs_actions(${s.version.actions[0].id})`);
    assert.equal(requests.find((r) => r.path === 'rs_actions').body['rs_CustomFunctionId@odata.bind'], '/rs_customfunctions(f1)');
    assert.deepEqual(s.removed.mappings, [emptied.id]);
});

test('discarding a draft and deleting a rule', async () => {
    await save.discardDraft('r1', 'd1');
    assert.deepEqual(globalThis.__changesets.at(-1), [
        { method: 'PATCH', path: 'rs_ruleversions(d1)', body: { rs_status: model.VersionStatus.Discarded } },
        { method: 'PATCH', path: 'rs_rules(r1)', body: { 'rs_DraftVersionId@odata.bind': null } },
    ]);
    await save.deleteRule('r1');
    assert.deepEqual(globalThis.__changesets.at(-1), [{ method: 'DELETE', path: 'rs_rules(r1)' }]);
});
