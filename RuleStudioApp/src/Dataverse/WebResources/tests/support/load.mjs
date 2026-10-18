// Loads a TypeScript page module for unit tests: esbuild bundles it to a temporary ES module,
// with chosen imports replaced by stubs (e.g. modules that call the Web API).
import { build } from 'esbuild';
import { mkdtempSync, rmSync } from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { fileURLToPath, pathToFileURL } from 'node:url';

const root = fileURLToPath(new URL('../../', import.meta.url));
const stubs = fileURLToPath(new URL('./stubs/', import.meta.url));

// Imports whose path ends with a key are served from tests/support/stubs/<value>.
const STUBBED = {
    '/shared/tableMetadata': 'tableMetadata.mjs',
    '/shared/batch': 'batch.mjs',
    '/shared/xrm': 'xrm.mjs',
};

const stubPlugin = {
    name: 'stubs',
    setup(builder) {
        builder.onResolve({ filter: /.*/ }, (args) => {
            const key = Object.keys(STUBBED).find((suffix) => args.path.endsWith(suffix));
            return key ? { path: path.join(stubs, STUBBED[key]) } : undefined;
        });
    },
};

/** Imports src/<relative>.ts as a module. */
export async function load(relative) {
    const dir = mkdtempSync(path.join(os.tmpdir(), 'rs-unit-'));
    const outfile = path.join(dir, 'module.mjs');
    try {
        await build({
            entryPoints: [path.join(root, 'src', `${relative}.ts`)],
            bundle: true,
            format: 'esm',
            platform: 'node',
            outfile,
            logLevel: 'silent',
            plugins: [stubPlugin],
        });
        return await import(pathToFileURL(outfile).href);
    } finally {
        rmSync(dir, { recursive: true, force: true });
    }
}
