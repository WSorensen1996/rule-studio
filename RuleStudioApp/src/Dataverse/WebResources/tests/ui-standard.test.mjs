// Guards the page standard (docs/ui-standard.md): no page may fall back to the old heading
// patterns, and every page applies the standard at start.
import assert from 'node:assert/strict';
import { readdirSync, readFileSync, statSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
import { test } from 'node:test';

const pages = fileURLToPath(new URL('../src/html', import.meta.url));
const shared = fileURLToPath(new URL('../src/shared', import.meta.url));
const OLD = [
    { pattern: /\beyebrow\b/, why: 'breadcrumb labels were removed; use rs-summary on records' },
    { pattern: /form-heading/, why: 'use rs-pagehead (list page) or rs-summary + rs-title (record)' },
    { pattern: /section-desc/, why: 'use rs-desc inside the section-title' },
    { pattern: /class="sub"|className: "sub"/, why: 'use rs-desc for one-line descriptions' },
];

function files(folder) {
    return readdirSync(folder).flatMap((name) => {
        const full = path.join(folder, name);
        if (statSync(full).isDirectory()) return files(full);
        return /\.(html|ts|css)$/.test(name) ? [full] : [];
    });
}

test('no page uses the old heading patterns', () => {
    const problems = [];
    for (const file of [...files(pages), ...files(shared)]) {
        const text = readFileSync(file, 'utf8');
        for (const { pattern, why } of OLD) {
            if (pattern.test(text)) problems.push(`${path.relative(pages, file)}: ${pattern} (${why})`);
        }
    }
    assert.deepEqual(problems, []);
});

test('every page applies the standard', () => {
    const missing = readdirSync(pages)
        .filter((name) => statSync(path.join(pages, name, 'index.html'), { throwIfNoEntry: false }))
        .filter((name) => !/applyStandard\(\)/.test(readFileSync(path.join(pages, name, 'main.ts'), 'utf8')));
    assert.deepEqual(missing, []);
});

test('activate / deactivate is a command bar button, never a switch in the form', () => {
    const problems = [];
    for (const file of files(pages).filter((f) => f.endsWith('.html'))) {
        const text = readFileSync(file, 'utf8');
        const name = path.relative(pages, file);
        const switches = text.match(/<input[^>]*class="switch"[^>]*>/g) ?? [];
        for (const tag of switches.filter((s) => /enabled|active/i.test(s))) problems.push(`${name}: on/off switch ${tag}`);
        const bar = /<div class="rs-commandbar"[\s\S]*?\n<\/div>/.exec(text)?.[0] ?? '';
        if (/id="cmdActivate"/.test(text) && !/id="cmdActivate"/.test(bar)) problems.push(`${name}: cmdActivate outside the command bar`);
    }
    assert.deepEqual(problems, []);
});

// Records open full-page (docs/ui-standard.md, "Records open full-page"); only these stay pop-ups.
const DIALOGS = ['publish-release', 'rollback-release', 'restore-version'];
const FORMS = ['rule-editor', 'register-function', 'new-lookup-definition'];
const shells = () => readdirSync(pages)
    .map((name) => [name, path.join(pages, name, 'index.html')])
    .filter(([, file]) => statSync(file, { throwIfNoEntry: false }))
    .map(([name, file]) => [name, readFileSync(file, 'utf8')]);

test('every page but a dialog has a command bar whose first command is Back', () => {
    const problems = [];
    for (const [name, text] of shells()) {
        const bar = /<div class="rs-commandbar"[^>]*>\s*(<[^>]*>)/.exec(text);
        if (DIALOGS.includes(name)) {
            if (bar) problems.push(`${name}: a dialog has no command bar`);
            continue;
        }
        if (!bar) problems.push(`${name}: no command bar`);
        else if (!/id="cmdBack"/.test(bar[1])) problems.push(`${name}: the first command is not Back (#cmdBack)`);
        if (/class="rs-dialog"/.test(text)) problems.push(`${name}: opens full-page, so no rs-dialog frame`);
    }
    assert.deepEqual(problems, []);
});

test('only confirm-and-act steps open as dialogs', () => {
    const problems = [];
    for (const file of files(pages).filter((f) => f.endsWith('.ts'))) {
        for (const match of readFileSync(file, 'utf8').matchAll(/openDialog\(\s*"([\w-]+)"/g)) {
            if (!DIALOGS.includes(match[1])) problems.push(`${path.relative(pages, file)}: ${match[1]} is a record; use openRecord`);
        }
    }
    assert.deepEqual(problems, []);
});

test('every form has Save & close in its command bar', () => {
    const missing = shells()
        .filter(([name]) => FORMS.includes(name))
        .filter(([, text]) => !/id="cmdSaveClose"/.test(/<div class="rs-commandbar"[\s\S]*?\n<\/div>/.exec(text)?.[0] ?? ''))
        .map(([name]) => name);
    assert.deepEqual(missing, []);
});

test('every page title sits in a list page head or a record title', () => {
    const withH1 = readdirSync(pages)
        .map((name) => [name, path.join(pages, name, 'index.html')])
        .filter(([, file]) => statSync(file, { throwIfNoEntry: false }))
        .filter(([, file]) => /<h1>/.test(readFileSync(file, 'utf8')))
        .filter(([, file]) => !/class="rs-(pagehead|title)[ "]/.test(readFileSync(file, 'utf8')))
        .map(([name]) => name);
    assert.deepEqual(withH1, []);
});
