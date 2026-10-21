// Guards the field help standard (docs/ui-standard.md, "Field help"): every form control in a
// page is either tagged with data-help or listed as self-explanatory, and every help key used
// in a page or in code exists in shared/fieldHelp.ts. Also guards "Page help": no static info boxes.
import assert from 'node:assert/strict';
import { readdirSync, readFileSync, statSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
import { test } from 'node:test';

const src = fileURLToPath(new URL('../src', import.meta.url));
const pages = path.join(src, 'html');

// Controls whose label says it all (names, descriptions, search boxes, plain dates).
const TRIVIAL = new Set([
    'environments/bundleFile', 'execution-logs/fRule', 'execution-logs/fTable', 'execution-logs/fFrom', 'execution-logs/fTo',
    'custom-functions/functionPick', 'lookup-definitions/lookupPick', 'new-lookup-definition/lookupName', 'new-lookup-definition/lookupDescription',
    'publish-release/releaseDescription',
    'register-function/funcName', 'register-function/funcDescription', 'register-function/apiPickerSearch',
    'rule-editor/ruleName', 'rule-editor/ruleDesc', 'rule-library/librarySearch',
    'rule-library/libraryTable', 'rule-library/libraryStatus', 'rule-library/selectAll', 'test-trace/ruleFilter',
]);

function files(folder) {
    return readdirSync(folder).flatMap((name) => {
        const full = path.join(folder, name);
        if (statSync(full).isDirectory()) return files(full);
        return /\.(html|ts)$/.test(name) ? [full] : [];
    });
}

const keys = new Set([...readFileSync(path.join(src, 'shared', 'fieldHelp.ts'), 'utf8').matchAll(/^\s+(\w+): "/gm)].map((m) => m[1]));

test('every non-trivial form control has field help', () => {
    const missing = [];
    for (const page of readdirSync(pages)) {
        const file = path.join(pages, page, 'index.html');
        if (!statSync(file, { throwIfNoEntry: false })) continue;
        const html = readFileSync(file, 'utf8');
        for (const [tag, id] of html.matchAll(/<(?:input|select|textarea)\b[^>]*\bid="([^"]+)"[^>]*>/g)) {
            if (TRIVIAL.has(`${page}/${id}`)) continue;
            const own = /data-help="/.test(tag);
            const before = html.slice(0, html.indexOf(tag));
            const wrapped = /<[^<>]*data-help="[^"]*"[^<>]*>\s*$/.test(before);
            if (!own && !wrapped) missing.push(`${page}/${id}`);
        }
    }
    assert.deepEqual(missing, []);
});

test('every help key exists in shared/fieldHelp.ts', () => {
    const unknown = [];
    for (const file of files(src)) {
        const text = readFileSync(file, 'utf8');
        const used = [...text.matchAll(/data-help(?:"|)[=:] ?"(\w+)"/g), ...text.matchAll(/withHelp\([\s\S]*?, "(\w+)"\)/g)].map((m) => m[1]);
        for (const key of used) if (!keys.has(key)) unknown.push(`${path.relative(src, file)}: ${key}`);
    }
    assert.deepEqual(unknown, []);
});

test('no static info boxes: explanations go behind the page help icon', () => {
    const boxes = [];
    for (const page of readdirSync(pages)) {
        const file = path.join(pages, page, 'index.html');
        if (!statSync(file, { throwIfNoEntry: false })) continue;
        for (const [tag] of readFileSync(file, 'utf8').matchAll(/<div class="notice info[^"]*"[^>]*>/g)) {
            if (!/\bid="/.test(tag)) boxes.push(page);
        }
    }
    assert.deepEqual(boxes, []);
});
