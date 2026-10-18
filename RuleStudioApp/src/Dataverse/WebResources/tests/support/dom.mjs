// A tiny DOM for unit tests: enough for h() and replaceChildren, with textContent read back.
class FakeElement {
    constructor(tag) {
        this.tagName = tag.toUpperCase();
        this.className = '';
        this.attributes = {};
        this.childNodes = [];
    }

    setAttribute(name, value) { this.attributes[name] = String(value); }

    append(...nodes) { this.childNodes.push(...nodes); }

    replaceChildren(...nodes) { this.childNodes = [...nodes]; }

    get textContent() { return this.childNodes.map((n) => (typeof n === 'string' ? n : n.textContent)).join(''); }

    set textContent(value) { this.childNodes = [String(value)]; }

    /** The child elements (not text) with this class. */
    byClass(name) {
        return this.childNodes.flatMap((n) => (typeof n === 'string' ? [] : [...(n.className.split(' ').includes(name) ? [n] : []), ...n.byClass(name)]));
    }
}

export function installDom() {
    globalThis.document = { createElement: (tag) => new FakeElement(tag) };
    return (tag = 'div') => new FakeElement(tag);
}
