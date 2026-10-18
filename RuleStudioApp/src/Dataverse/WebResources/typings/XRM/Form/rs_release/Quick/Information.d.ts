declare namespace Form.rs_release.Quick {
  namespace Information {
    namespace Tabs {
    }


    interface Controls extends XDTForm.ControlCollectionBase {
      get(name: "rs_name"): Xrm.Controls.StringControl;
      get(name: string): null;
      get(): Xrm.Controls.Control[];
      get(index: number): Xrm.Controls.Control;
      get(chooser: (item: Xrm.Controls.Control, index: number) => boolean): Xrm.Controls.Control[];
    }


    interface Tabs extends XDTForm.TabCollectionBase {
      get(name: string): null;
      get(): Xrm.Controls.Tab[];
      get(index: number): Xrm.Controls.Tab;
      get(chooser: (item: Xrm.Controls.Tab, index: number) => boolean): Xrm.Controls.Tab[];
    }
  }

  interface Information extends XDTForm.QuickViewForm<Information.Tabs,Information.Controls> {
    getAttribute(attributeName: "rs_Name"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: string): null;
    getAttribute(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Attributes.Attribute>): Xrm.Attributes.Attribute[];
    getControl(controlName: "rs_name"): Xrm.Controls.StringControl;
    getControl(controlName: string): null;
    getControl(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Controls.Control>): Xrm.Controls.Control[];
  }
}
