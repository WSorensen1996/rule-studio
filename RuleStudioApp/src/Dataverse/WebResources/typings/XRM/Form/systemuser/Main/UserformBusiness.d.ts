declare namespace Form.systemuser.Main {
  namespace UserformBusiness {
    namespace Tabs {
      interface ADMINISTRATION_TAB extends XDTForm.SectionCollectionBase {
        get(name: "administration"): Xrm.Controls.Section;
        get(name: "e-mail configuration"): Xrm.Controls.Section;
        get(name: string): null;
        get(): Xrm.Controls.Section[];
        get(index: number): Xrm.Controls.Section;
        get(chooser: (item: Xrm.Controls.Section, index: number) => boolean): Xrm.Controls.Section[];
      }

      interface SUMMARY_TAB extends XDTForm.SectionCollectionBase {
        get(name: "DirectReports"): Xrm.Controls.Section;
        get(name: "TEAMS_TAB"): Xrm.Controls.Section;
        get(name: "mailing address"): Xrm.Controls.Section;
        get(name: "online account information"): Xrm.Controls.Section;
        get(name: "onpremise account information"): Xrm.Controls.Section;
        get(name: "organization information"): Xrm.Controls.Section;
        get(name: "user information"): Xrm.Controls.Section;
        get(name: string): null;
        get(): Xrm.Controls.Section[];
        get(index: number): Xrm.Controls.Section;
        get(chooser: (item: Xrm.Controls.Section, index: number) => boolean): Xrm.Controls.Section[];
      }
    }

    interface Attributes extends XDTForm.AttributeCollectionBase {
      get(name: "accessmode"): Xrm.Attributes.OptionSetAttribute<systemuser_accessmode>;
      get(name: "address1_city"): Xrm.Attributes.Attribute<string> | null;
      get(name: "address1_composite"): Xrm.Attributes.Attribute<string> | null;
      get(name: "address1_country"): Xrm.Attributes.Attribute<string> | null;
      get(name: "address1_line1"): Xrm.Attributes.Attribute<string> | null;
      get(name: "address1_line2"): Xrm.Attributes.Attribute<string> | null;
      get(name: "address1_line3"): Xrm.Attributes.Attribute<string> | null;
      get(name: "address1_postalcode"): Xrm.Attributes.Attribute<string> | null;
      get(name: "address1_stateorprovince"): Xrm.Attributes.Attribute<string> | null;
      get(name: "address1_telephone1"): Xrm.Attributes.Attribute<string>;
      get(name: "businessunitid"): XDTForm.LookupAttribute<"businessunit">;
      get(name: "caltype"): Xrm.Attributes.OptionSetAttribute<systemuser_caltype>;
      get(name: "defaultmailbox"): XDTForm.LookupAttribute<"mailbox">;
      get(name: "domainname"): Xrm.Attributes.Attribute<string>;
      get(name: "fullname"): Xrm.Attributes.Attribute<string> | null;
      get(name: "internalemailaddress"): Xrm.Attributes.Attribute<string>;
      get(name: "invitestatuscode"): Xrm.Attributes.OptionSetAttribute<systemuser_invitestatuscode>;
      get(name: "mobilephone"): Xrm.Attributes.Attribute<string>;
      get(name: "parentsystemuserid"): XDTForm.LookupAttribute<"systemuser">;
      get(name: "preferredaddresscode"): Xrm.Attributes.OptionSetAttribute<systemuser_preferredaddresscode>;
      get(name: "title"): Xrm.Attributes.Attribute<string>;
      get(name: "windowsliveid"): Xrm.Attributes.Attribute<string>;
      get(name: string): null;
      get(): Xrm.Attributes.Attribute[];
      get(index: number): Xrm.Attributes.Attribute;
      get(chooser: (item: Xrm.Attributes.Attribute, index: number) => boolean): Xrm.Attributes.Attribute[];
    }


    interface Controls extends XDTForm.ControlCollectionBase {
      get(name: "accessmode"): XDTForm.OptionSetControl<systemuser_accessmode>;
      get(name: "address1_composite"): Xrm.Controls.StringControl | null;
      get(name: "address1_composite_compositionLinkControl_address1_city"): Xrm.Controls.StringControl | null;
      get(name: "address1_composite_compositionLinkControl_address1_country"): Xrm.Controls.StringControl | null;
      get(name: "address1_composite_compositionLinkControl_address1_line1"): Xrm.Controls.StringControl | null;
      get(name: "address1_composite_compositionLinkControl_address1_line2"): Xrm.Controls.StringControl | null;
      get(name: "address1_composite_compositionLinkControl_address1_line3"): Xrm.Controls.StringControl | null;
      get(name: "address1_composite_compositionLinkControl_address1_postalcode"): Xrm.Controls.StringControl | null;
      get(name: "address1_composite_compositionLinkControl_address1_stateorprovince"): Xrm.Controls.StringControl | null;
      get(name: "address1_telephone1"): Xrm.Controls.StringControl;
      get(name: "businessunitid"): XDTForm.LookupControl<"businessunit">;
      get(name: "caltype"): XDTForm.OptionSetControl<systemuser_caltype>;
      get(name: "defaultmailbox"): XDTForm.LookupControl<"mailbox">;
      get(name: "DirectReports"): XDTForm.SubGridControl<"systemuser">;
      get(name: "domainname"): Xrm.Controls.StringControl;
      get(name: "fullname"): Xrm.Controls.StringControl | null;
      get(name: "internalemailaddress"): Xrm.Controls.StringControl;
      get(name: "invitestatuscode"): XDTForm.OptionSetControl<systemuser_invitestatuscode>;
      get(name: "mobilephone"): Xrm.Controls.StringControl;
      get(name: "parentsystemuserid"): XDTForm.LookupControl<"systemuser">;
      get(name: "preferredaddresscode"): XDTForm.OptionSetControl<systemuser_preferredaddresscode>;
      get(name: "TeamsSubGrid"): XDTForm.SubGridControl<"team">;
      get(name: "title"): Xrm.Controls.StringControl;
      get(name: "windowsliveid"): Xrm.Controls.StringControl;
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
      get(name: "ADMINISTRATION_TAB"): XDTForm.PageTab<Tabs.ADMINISTRATION_TAB>;
      get(name: "SUMMARY_TAB"): XDTForm.PageTab<Tabs.SUMMARY_TAB>;
      get(name: string): null;
      get(): Xrm.Controls.Tab[];
      get(index: number): Xrm.Controls.Tab;
      get(chooser: (item: Xrm.Controls.Tab, index: number) => boolean): Xrm.Controls.Tab[];
    }
  }

  interface UserformBusiness extends XDTForm.FormContextBase<UserformBusiness.Attributes,UserformBusiness.Tabs,UserformBusiness.Controls,UserformBusiness.QuickViewForms> {
    getAttribute(attributeName: "accessmode"): Xrm.Attributes.OptionSetAttribute<systemuser_accessmode>;
    getAttribute(attributeName: "address1_city"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "address1_composite"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "address1_country"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "address1_line1"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "address1_line2"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "address1_line3"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "address1_postalcode"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "address1_stateorprovince"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "address1_telephone1"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "businessunitid"): XDTForm.LookupAttribute<"businessunit">;
    getAttribute(attributeName: "caltype"): Xrm.Attributes.OptionSetAttribute<systemuser_caltype>;
    getAttribute(attributeName: "defaultmailbox"): XDTForm.LookupAttribute<"mailbox">;
    getAttribute(attributeName: "domainname"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "fullname"): Xrm.Attributes.Attribute<string> | null;
    getAttribute(attributeName: "internalemailaddress"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "invitestatuscode"): Xrm.Attributes.OptionSetAttribute<systemuser_invitestatuscode>;
    getAttribute(attributeName: "mobilephone"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "parentsystemuserid"): XDTForm.LookupAttribute<"systemuser">;
    getAttribute(attributeName: "preferredaddresscode"): Xrm.Attributes.OptionSetAttribute<systemuser_preferredaddresscode>;
    getAttribute(attributeName: "title"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: "windowsliveid"): Xrm.Attributes.Attribute<string>;
    getAttribute(attributeName: string): null;
    getAttribute(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Attributes.Attribute>): Xrm.Attributes.Attribute[];
    getControl(controlName: "accessmode"): XDTForm.OptionSetControl<systemuser_accessmode>;
    getControl(controlName: "address1_composite"): Xrm.Controls.StringControl | null;
    getControl(controlName: "address1_composite_compositionLinkControl_address1_city"): Xrm.Controls.StringControl | null;
    getControl(controlName: "address1_composite_compositionLinkControl_address1_country"): Xrm.Controls.StringControl | null;
    getControl(controlName: "address1_composite_compositionLinkControl_address1_line1"): Xrm.Controls.StringControl | null;
    getControl(controlName: "address1_composite_compositionLinkControl_address1_line2"): Xrm.Controls.StringControl | null;
    getControl(controlName: "address1_composite_compositionLinkControl_address1_line3"): Xrm.Controls.StringControl | null;
    getControl(controlName: "address1_composite_compositionLinkControl_address1_postalcode"): Xrm.Controls.StringControl | null;
    getControl(controlName: "address1_composite_compositionLinkControl_address1_stateorprovince"): Xrm.Controls.StringControl | null;
    getControl(controlName: "address1_telephone1"): Xrm.Controls.StringControl;
    getControl(controlName: "businessunitid"): XDTForm.LookupControl<"businessunit">;
    getControl(controlName: "caltype"): XDTForm.OptionSetControl<systemuser_caltype>;
    getControl(controlName: "defaultmailbox"): XDTForm.LookupControl<"mailbox">;
    getControl(controlName: "DirectReports"): XDTForm.SubGridControl<"systemuser">;
    getControl(controlName: "domainname"): Xrm.Controls.StringControl;
    getControl(controlName: "fullname"): Xrm.Controls.StringControl | null;
    getControl(controlName: "internalemailaddress"): Xrm.Controls.StringControl;
    getControl(controlName: "invitestatuscode"): XDTForm.OptionSetControl<systemuser_invitestatuscode>;
    getControl(controlName: "mobilephone"): Xrm.Controls.StringControl;
    getControl(controlName: "parentsystemuserid"): XDTForm.LookupControl<"systemuser">;
    getControl(controlName: "preferredaddresscode"): XDTForm.OptionSetControl<systemuser_preferredaddresscode>;
    getControl(controlName: "TeamsSubGrid"): XDTForm.SubGridControl<"team">;
    getControl(controlName: "title"): Xrm.Controls.StringControl;
    getControl(controlName: "windowsliveid"): Xrm.Controls.StringControl;
    getControl(controlName: string): null;
    getControl(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Controls.Control>): Xrm.Controls.Control[];
  }
}
