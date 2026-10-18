declare namespace Form.systemuser.Quick {
  namespace UserHierarchyTileForm {
    namespace Tabs {
      interface hierarchy extends XDTForm.SectionCollectionBase {
        get(name: "user tile"): Xrm.Controls.Section;
        get(name: string): null;
        get(): Xrm.Controls.Section[];
        get(index: number): Xrm.Controls.Section;
        get(chooser: (item: Xrm.Controls.Section, index: number) => boolean): Xrm.Controls.Section[];
      }
    }


    interface Controls extends XDTForm.ControlCollectionBase {
      get(name: "businessunitid"): XDTForm.LookupControl<"businessunit">;
      get(name: "fullname"): Xrm.Controls.StringControl | null;
      get(name: "internalemailaddress"): Xrm.Controls.StringControl;
      get(name: "mobilephone"): Xrm.Controls.StringControl;
      get(name: "title"): Xrm.Controls.StringControl;
      get(name: string): null;
      get(): Xrm.Controls.Control[];
      get(index: number): Xrm.Controls.Control;
      get(chooser: (item: Xrm.Controls.Control, index: number) => boolean): Xrm.Controls.Control[];
    }


    interface Tabs extends XDTForm.TabCollectionBase {
      get(name: "hierarchy"): XDTForm.PageTab<Tabs.hierarchy>;
      get(name: string): null;
      get(): Xrm.Controls.Tab[];
      get(index: number): Xrm.Controls.Tab;
      get(chooser: (item: Xrm.Controls.Tab, index: number) => boolean): Xrm.Controls.Tab[];
    }
  }

  interface UserHierarchyTileForm extends XDTForm.QuickViewForm<UserHierarchyTileForm.Tabs,UserHierarchyTileForm.Controls> {
    getAttribute(attributeName: "businessunitid"): XDTForm.LookupAttribute<"businessunit">;
    getAttribute(attributeName: "fullname"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "internalemailaddress"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "mobilephone"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "title"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: string): null;
    getAttribute(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Attributes.Attribute>): Xrm.Attributes.Attribute[];
    getControl(controlName: "businessunitid"): XDTForm.LookupControl<"businessunit">;
    getControl(controlName: "fullname"): Xrm.Controls.StringControl | null;
    getControl(controlName: "internalemailaddress"): Xrm.Controls.StringControl;
    getControl(controlName: "mobilephone"): Xrm.Controls.StringControl;
    getControl(controlName: "title"): Xrm.Controls.StringControl;
    getControl(controlName: string): null;
    getControl(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Controls.Control>): Xrm.Controls.Control[];
  }
}
