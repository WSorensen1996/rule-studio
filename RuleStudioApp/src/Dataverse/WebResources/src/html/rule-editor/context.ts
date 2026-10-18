// What every editor section needs: the state it edits, reference data and a change callback.
import type { Catalog } from "./catalog";
import type { EditorState } from "./model";

export interface EditorContext {
  state: EditorState;
  catalog: Catalog;
  /** Viewer, or a version that cannot be edited. */
  disabled: boolean;
  /** Marks the version changed; structural = the section must be drawn again. */
  changed: (structural: boolean) => void;
}
