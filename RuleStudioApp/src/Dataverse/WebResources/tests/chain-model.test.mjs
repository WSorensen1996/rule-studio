// Unit tests of the execution timeline's chain model (src/html/execution-timeline/chainModel.ts).
import assert from 'node:assert/strict';
import { before, test } from 'node:test';

import { load } from './support/load.mjs';

const UPDATE = 126250001;
const BEFORE = 126250000;
const TRIGGER = { table: 'quote', event: UPDATE, stage: BEFORE };

let chain;

before(async () => {
    chain = await load('html/execution-timeline/chainModel');
});

function version(overrides = {}) {
    return { id: crypto.randomUUID(), versionNumber: 1, status: null, targetTable: 'quote', event: UPDATE, stage: BEFORE, priority: 100, filteredColumns: '', stopProcessing: false, ...overrides };
}

function rule(key, live, draft = null, enabled = true) {
    return { id: crypto.randomUUID(), name: key, ruleKey: key, enabled, live, draft };
}

test('distinct triggers come from live and draft versions, once each', () => {
    const rules = [
        rule('a', version(), version({ table: 'quote' })),
        rule('b', version({ targetTable: 'account' })),
        rule('c', null, version({ stage: 126250001 })),
        rule('d', version({ targetTable: null })),
    ];
    assert.deepEqual(chain.distinctTriggers(rules).map(chain.triggerKey).sort(), [
        `account|${UPDATE}|${BEFORE}`, `quote|${UPDATE}|${BEFORE}`, `quote|${UPDATE}|126250001`,
    ]);
});

test('rules run by priority, then by rule key as the engine orders them (ordinal, ignoring case)', () => {
    const rules = [rule('b', version({ priority: 10 })), rule('A_B', version({ priority: 10 })), rule('ab', version({ priority: 10 })), rule('z', version({ priority: 1 })), rule('none', version({ priority: null }))];
    const { steps } = chain.buildChain(rules, TRIGGER, 'live');
    assert.deepEqual(steps.map((s) => s.rule.ruleKey), ['z', 'ab', 'A_B', 'b', 'none']);
});

test('the draft view uses the draft where one exists and shows what it adds, changes or removes', () => {
    const rules = [
        rule('same', version(), version()),
        rule('changed', version(), version({ priority: 5 })),
        rule('added', null, version()),
        rule('moved', version(), version({ targetTable: 'account' })),
        rule('liveonly', version()),
    ];
    const draft = chain.buildChain(rules, TRIGGER, 'draft');
    assert.deepEqual(Object.fromEntries(draft.steps.map((s) => [s.rule.ruleKey, s.diff])), { changed: 'changed', same: 'none', added: 'added', liveonly: 'none' });
    assert.deepEqual(draft.others.map((s) => [s.rule.ruleKey, s.diff]), [['moved', 'removed']]);

    const live = chain.buildChain(rules, TRIGGER, 'live');
    assert.deepEqual(live.steps.find((s) => s.rule.ruleKey === 'moved').diff, 'added');
    assert.deepEqual(live.others.map((s) => s.rule.ruleKey), ['added']);
});

test('rules after an enabled stop-processing rule are marked as possibly not reached', () => {
    const rules = [rule('a', version({ priority: 1, stopProcessing: true }), null, false), rule('b', version({ priority: 2, stopProcessing: true })), rule('c', version({ priority: 3 }))];
    const { steps } = chain.buildChain(rules, TRIGGER, 'live');
    assert.deepEqual(steps.map((s) => [s.rule.ruleKey, s.active, s.cutOff]), [['a', false, false], ['b', true, false], ['c', true, true]]);
});

test('a rule with neither version on the trigger is left out', () => {
    const { steps, others } = chain.buildChain([rule('elsewhere', version({ targetTable: 'account' }))], TRIGGER, 'live');
    assert.equal(steps.length + others.length, 0);
});
