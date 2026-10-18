// Guards html.build.mjs: every built page holds its whole script and stylesheet. A string
// replacement once expanded `$&` / `$'` from the bundle, which cut a page's script short.
import assert from 'node:assert/strict';
import { existsSync, readdirSync, readFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
import { test } from 'node:test';
import vm from 'node:vm';

const dist = fileURLToPath(new URL('../dist/rs_RuleStudio/html', import.meta.url));

test('built pages keep no build marker and their scripts parse', { skip: !existsSync(dist) && 'run the build first' }, () => {
    const problems = [];
    for (const name of readdirSync(dist).filter((n) => n.endsWith('.html'))) {
        const html = readFileSync(path.join(dist, name), 'utf8');
        if (/<!--(SCRIPT|STYLE)-->/.test(html)) problems.push(`${name}: build marker left in the page`);
        for (const [, js] of html.matchAll(/<script>\n([\s\S]*?)\n<\/script>/g)) {
            try {
                new vm.Script(js);
            } catch (error) {
                problems.push(`${name}: ${error.message}`);
            }
        }
    }
    assert.deepEqual(problems, []);
});
