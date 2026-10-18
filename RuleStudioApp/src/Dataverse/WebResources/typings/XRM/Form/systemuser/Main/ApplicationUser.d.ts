declare namespace Form.systemuser.Main {
  namespace ApplicationUser {
    namespace Tabs {
      interface SUMMARY_TAB extends XDTForm.SectionCollectionBase {
        get(name: "onpremise account information"): Xrm.Controls.Section;
        get(name: "user information"): Xrm.Controls.Section;
        get(name: string): null;
        get(): Xrm.Controls.Section[];
        get(index: number): Xrm.Controls.Section;
        get(chooser: (item: Xrm.Controls.Section, index: number) => boolean): Xrm.Controls.Section[];
      }
    }

    interface Attributes extends XDTForm.AttributeCollectionBase {
      get(name: "applicationid"): Xrm.Attributes.Attribute<string>;
      get(name: "applicationiduri"): Xrm.Attributes.Attribute<string>;
      get(name: "azureactivedirectoryobjectid"): Xrm.Attributes.Attribute<string>;
      get(name: "domainname"): Xrm.Attributes.Attribute<string>;
      get(name: "fullname"): Xrm.Attributes.Attribute<string> | null;
      get(name: "internalemailaddress"): Xrm.Attributes.Attribute<string>;
      get(name: string): null;
      get(): Xrm.Attributes.Attribute[];
      get(index: number): Xrm.Attributes.Attribute;
      get(chooser: (item: Xrm.Attributes.Attribute, index: number) => boolean): Xrm.Attributes.Attribute[];
    }


    interface Controls extends XDTForm.ControlCollectionBase {
      get(name: "applicationid"): Xrm.Controls.StringControl;
      get(name: "applicationiduri"): Xrm.Controls.StringControl;
      get(name: "azureactivedirectoryobjectid"): Xrm.Controls.StringControl;
      get(name: "domainname"): Xrm.Controls.StringControl;
      get(name: "fullname"): Xrm.Controls.StringControl | null;
      get(name: "internalemailaddress"): Xrm.Controls.StringControl;
      get(name: string): null;
      get(): Xrm.Controls.Control[];
      get(index: number): Xrm.Controls.Control;
      get(chooser: (item: Xrm.Controls.Control, index: number) => boolean): Xrm.Controls.Control[];
    }

    interface QuickViewForms extends XDTForm.QuickViewFormCollectionBase {
      get(name: string): null;
      get(): XDTForm.QuickViewFormBase[];
      get(index: number): XDTForm.QuickViewFormBase;
      get(chooser: (item: XDTForm.QuickViewFormBase, index: number) => boolean): XDTForm.QuickViewFormBase[];
    }


    interface Tabs extends XDTForm.TabCollectionBase {
      get(name: "SUMMARY_TAB"): XDTForm.PageTab<Tabs.SUMMARY_TAB>;
      get(name: string): null;
      get(): Xrm.Controls.Tab[];
      get(index: number): Xrm.Controls.Tab;
      get(chooser: (item: Xrm.Controls.Tab, index: number) => boolean): Xrm.Controls.Tab[];
    }
  }

  interface ApplicationUser extends XDTForm.FormContextBase<ApplicationUser.Attributes,ApplicationUser.Tabs,ApplicationUser.Controls,ApplicationUser.QuickViewForms> {
    getAttribute(attributeName: "applicationid"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "applicationiduri"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "azureactivedirectoryobjectid"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "domainname"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "fullname"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "internalemailaddress"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: string): null;
    getAttribute(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Attributes.Attribute>): Xrm.Attributes.Attribute[];
    getControl(controlName: "applicationid"): Xrm.Controls.StringControl;
    getControl(controlName: "applicationiduri"): Xrm.Controls.StringControl;
    getControl(controlName: "azureactivedirectoryobjectid"): Xrm.Controls.StringControl;
    getControl(controlName: "domainname"): Xrm.Controls.StringControl;
    getControl(controlName: "fullname"): Xrm.Controls.StringControl | null;
    getControl(controlName: "internalemailaddress"): Xrm.Controls.StringControl;
    getControl(controlName: string): null;
    getControl(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Controls.Control>): Xrm.Controls.Control[];
  }
}
