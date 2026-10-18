// Unit tests of the rule editor's model and field paths (src/html/rule-editor/model.ts, paths.ts).
import assert from 'node:assert/strict';
import { before, test } from 'node:test';

import { load } from './support/load.mjs';

let model;
let paths;

before(async () => {
    model = await load('html/rule-editor/model');
    paths = await load('html/rule-editor/paths');
    globalThis.__tables = {
        quote: { label: 'Quote', columns: [{ logicalName: 'ca_customer', label: 'Customer' }, { logicalName: 'name', label: 'Name' }] },
        account: { label: 'Account', columns: [{ logicalName: 'ca_segment', label: 'Segment' }] },
        ca_quoteline: { label: 'Quote line', columns: [{ logicalName: 'ca_product', label: 'Product' }] },
    };
});

function tree(isNew) {
    const condition = { ...model.newCondition(), isNew, mappings: [{ ...model.newMapping('p'), isNew }] };
    const inner = { ...model.newGroup(model.Logical.OR), isNew, children: [condition, { ...model.newCondition(), isNew }] };
    return { ...model.newGroup(), isNew, children: [inner, { ...model.newCondition(), isNew }] };
}

test('new rows get unique ids and the editor defaults', () => {
    const version = model.newVersion(3);
    assert.equal(version.versionNumber, 3);
    assert.equal(version.status, model.VersionStatus.Draft);
    assert.equal(version.priority, 100);
    assert.equal(version.logLevel, null);
    assert.equal(version.event, model.RuleEvent.Update);
    assert.equal(version.stage, model.Stage.Beforesave);
    assert.notEqual(model.newId(), model.newId());
    assert.equal(model.newRule().enabled, true);
});

test('a value is empty when its source has nothing chosen or typed', () => {
    const value = model.emptyValue();
    assert.equal(model.isEmptyValue(value), true);
    assert.equal(model.isEmptyValue({ ...value, value: '   ' }), true);
    assert.equal(model.isEmptyValue({ ...value, value: '0' }), false);
    assert.equal(model.isEmptyValue({ ...value, sourceType: model.SourceType.Fieldpath }), true);
    assert.equal(model.isEmptyValue({ ...value, sourceType: model.SourceType.Fieldpath, valuePath: { table: 'quote', hops: [], column: 'name' } }), false);
    assert.equal(model.isEmptyValue({ ...value, sourceType: model.SourceType.Lookup, lookupDefinitionId: 'x' }), false);
    assert.equal(model.isEmptyValue({ ...value, sourceType: model.SourceType.Function }), true);
    assert.equal(model.isEmptyValue({ ...value, sourceType: model.SourceType.Expression, value: '[a] + 1' }), false);
});

test('conditions are counted through nested groups', () => {
    assert.equal(model.countConditions(tree(true)), 3);
    assert.equal(model.countConditions(model.newGroup()), 0);
});

test('removing a group collects its saved rows, children before parents, and skips unsaved ones', () => {
    const saved = tree(false);
    const removed = model.noneRemoved();
    model.collectRemoved(saved, removed);
    assert.equal(removed.conditions.length, 3);
    assert.equal(removed.mappings.length, 1);
    assert.deepEqual(removed.groups, [saved.children[0].id, saved.id]);

    const unsaved = model.noneRemoved();
    model.collectRemoved(tree(true), unsaved);
    assert.deepEqual(unsaved, model.noneRemoved());
});

test('copying a version as a draft gives every row a new id and keeps the content', () => {
    const source = { ...model.newVersion(1), isNew: false, status: model.VersionStatus.Published, changeNote: 'live', root: tree(false) };
    source.actions = [{ ...model.newAction(model.Branch.Then), isNew: false, targetField: 'name', mappings: [{ ...model.newMapping('a'), isNew: false }] }];

    const copy = model.copyAsDraft(source, 5);

    assert.equal(copy.versionNumber, 5);
    assert.equal(copy.status, model.VersionStatus.Draft);
    assert.equal(copy.derivedFromId, source.id);
    assert.equal(copy.changeNote, '');
    assert.equal(copy.isNew, true);
    assert.notEqual(copy.id, source.id);
    assert.notEqual(copy.root.id, source.root.id);
    assert.equal(copy.root.children[0].children[0].mappings[0].isNew, true);
    assert.notEqual(copy.root.children[0].children[0].id, source.root.children[0].children[0].id);
    assert.equal(copy.actions[0].targetField, 'name');
    assert.notEqual(copy.actions[0].id, source.actions[0].id);
    assert.notEqual(copy.actions[0].mappings[0].id, source.actions[0].mappings[0].id);
    copy.actions[0].value.value = 'changed';
    assert.equal(source.actions[0].value.value, '', 'the copy does not share value objects with the source');
});

test('field paths are read and stored as JSON, and broken text reads as no path', () => {
    const path = { table: 'quote', hops: [{ kind: 'parent', attribute: 'ca_customer', table: 'account' }], column: 'ca_segment', extra: 1 };
    assert.equal(paths.storePath(path), '{"table":"quote","hops":[{"kind":"parent","attribute":"ca_customer","table":"account"}],"column":"ca_segment"}');
    assert.deepEqual(paths.parsePath(paths.storePath(path)), { table: 'quote', hops: path.hops, column: 'ca_segment' });
    assert.equal(paths.parsePath('{not json'), null);
    assert.equal(paths.parsePath('{"table":"quote"}'), null);
    assert.equal(paths.parsePath(''), null);
    assert.equal(paths.storePath(null), null);
});

test('a path ends on the table of its last hop and knows when it reads children', () => {
    const child = { table: 'quote', hops: [{ kind: 'child', attribute: 'ca_quote', table: 'ca_quoteline' }], column: 'ca_product' };
    assert.equal(paths.endTable(child), 'ca_quoteline');
    assert.equal(paths.endTable({ table: 'quote', hops: [], column: 'name' }), 'quote');
    assert.equal(paths.hasChildHop(child), true);
    assert.equal(paths.hasChildHop(null), false);
});

test('paths are described with table and column labels', async () => {
    assert.equal(
        await paths.describePath({ table: 'quote', hops: [{ kind: 'parent', attribute: 'ca_customer', table: 'account' }], column: 'ca_segment' }),
        'Quote → Customer (Account) → Segment');
    assert.equal(
        await paths.describePath({ table: 'quote', hops: [{ kind: 'child', attribute: 'ca_quote', table: 'ca_quoteline' }], column: 'ca_product' }),
        'Quote ⇉ Quote line → Product');
    assert.equal(await paths.describePath({ table: 'quote', hops: [], column: 'gone' }), 'Quote → gone');
});
