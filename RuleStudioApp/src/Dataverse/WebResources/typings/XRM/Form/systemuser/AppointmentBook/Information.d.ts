declare namespace Form.systemuser.AppointmentBook {
  namespace Information {
    namespace Tabs {
      interface general extends XDTForm.SectionCollectionBase {
        get(name: "organization info"): Xrm.Controls.Section;
        get(name: "section 1"): Xrm.Controls.Section;
        get(name: string): null;
        get(): Xrm.Controls.Section[];
        get(index: number): Xrm.Controls.Section;
        get(chooser: (item: Xrm.Controls.Section, index: number) => boolean): Xrm.Controls.Section[];
      }
    }

    interface Attributes extends XDTForm.AttributeCollectionBase {
      get(name: "address1_telephone1"): Xrm.Attributes.Attribute<string>;
      get(name: "address1_telephone2"): Xrm.Attributes.Attribute<string>;
      get(name: "address1_telephone3"): Xrm.Attributes.Attribute<string>;
      get(name: "businessunitid"): XDTForm.LookupAttribute<"businessunit">;
      get(name: "domainname"): Xrm.Attributes.Attribute<string>;
      get(name: "firstname"): Xrm.Attributes.Attribute<string>;
      get(name: "homephone"): Xrm.Attributes.Attribute<string>;
      get(name: "internalemailaddress"): Xrm.Attributes.Attribute<string>;
      get(name: "lastname"): Xrm.Attributes.Attribute<string>;
      get(name: "mobilealertemail"): Xrm.Attributes.Attribute<string>;
      get(name: "mobilephone"): Xrm.Attributes.Attribute<string>;
      get(name: "parentsystemuserid"): XDTForm.LookupAttribute<"systemuser">;
      get(name: "personalemailaddress"): Xrm.Attributes.Attribute<string>;
      get(name: "positionid"): XDTForm.LookupAttribute<"position">;
      get(name: "preferredphonecode"): Xrm.Attributes.OptionSetAttribute<systemuser_preferredphonecode>;
      get(name: "title"): Xrm.Attributes.Attribute<string>;
      get(name: string): null;
      get(): Xrm.Attributes.Attribute[];
      get(index: number): Xrm.Attributes.Attribute;
      get(chooser: (item: Xrm.Attributes.Attribute, index: number) => boolean): Xrm.Attributes.Attribute[];
    }


    interface Controls extends XDTForm.ControlCollectionBase {
      get(name: "address1_telephone1"): Xrm.Controls.StringControl;
      get(name: "address1_telephone2"): Xrm.Controls.StringControl;
      get(name: "address1_telephone3"): Xrm.Controls.StringControl;
      get(name: "businessunitid"): XDTForm.LookupControl<"businessunit">;
      get(name: "domainname"): Xrm.Controls.StringControl;
      get(name: "firstname"): Xrm.Controls.StringControl;
      get(name: "homephone"): Xrm.Controls.StringControl;
      get(name: "internalemailaddress"): Xrm.Controls.StringControl;
      get(name: "lastname"): Xrm.Controls.StringControl;
      get(name: "mobilealertemail"): Xrm.Controls.StringControl;
      get(name: "mobilephone"): Xrm.Controls.StringControl;
      get(name: "parentsystemuserid"): XDTForm.LookupControl<"systemuser">;
      get(name: "personalemailaddress"): Xrm.Controls.StringControl;
      get(name: "positionid"): XDTForm.LookupControl<"position">;
      get(name: "preferredphonecode"): XDTForm.OptionSetControl<systemuser_preferredphonecode>;
      get(name: "title"): Xrm.Controls.StringControl;
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
      get(name: "general"): XDTForm.PageTab<Tabs.general>;
      get(name: string): null;
      get(): Xrm.Controls.Tab[];
      get(index: number): Xrm.Controls.Tab;
      get(chooser: (item: Xrm.Controls.Tab, index: number) => boolean): Xrm.Controls.Tab[];
    }
  }

  interface Information extends XDTForm.FormContextBase<Information.Attributes,Information.Tabs,Information.Controls,Information.QuickViewForms> {
    getAttribute(attributeName: "address1_telephone1"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "address1_telephone2"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "address1_telephone3"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "businessunitid"): XDTForm.LookupAttribute<"businessunit">;
    getAttribute(attributeName: "domainname"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "firstname"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "homephone"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "internalemailaddress"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "lastname"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "mobilealertemail"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "mobilephone"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "parentsystemuserid"): XDTForm.LookupAttribute<"systemuser">;
    getAttribute(attributeName: "personalemailaddress"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "positionid"): XDTForm.LookupAttribute<"position">;
    getAttribute(attributeName: "preferredphonecode"): Xrm.Attributes.OptionSetAttribute<systemuser_preferredphonecode>;
    getAttribute(attributeName: "title"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: string): null;
    getAttribute(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Attributes.Attribute>): Xrm.Attributes.Attribute[];
    getControl(controlName: "address1_telephone1"): Xrm.Controls.StringControl;
    getControl(controlName: "address1_telephone2"): Xrm.Controls.StringControl;
    getControl(controlName: "address1_telephone3"): Xrm.Controls.StringControl;
    getControl(controlName: "businessunitid"): XDTForm.LookupControl<"businessunit">;
    getControl(controlName: "domainname"): Xrm.Controls.StringControl;
    getControl(controlName: "firstname"): Xrm.Controls.StringControl;
    getControl(controlName: "homephone"): Xrm.Controls.StringControl;
    getControl(controlName: "internalemailaddress"): Xrm.Controls.StringControl;
    getControl(controlName: "lastname"): Xrm.Controls.StringControl;
    getControl(controlName: "mobilealertemail"): Xrm.Controls.StringControl;
    getControl(controlName: "mobilephone"): Xrm.Controls.StringControl;
    getControl(controlName: "parentsystemuserid"): XDTForm.LookupControl<"systemuser">;
    getControl(controlName: "personalemailaddress"): Xrm.Controls.StringControl;
    getControl(controlName: "positionid"): XDTForm.LookupControl<"position">;
    getControl(controlName: "preferredphonecode"): XDTForm.OptionSetControl<systemuser_preferredphonecode>;
    getControl(controlName: "title"): Xrm.Controls.StringControl;
    getControl(controlName: string): null;
    getControl(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Controls.Control>): Xrm.Controls.Control[];
  }
}
