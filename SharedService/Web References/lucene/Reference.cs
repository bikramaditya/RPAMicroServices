﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace SharedService.lucene {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="SearchWebServiceSoap", Namespace="http://tempuri.org/")]
    public partial class SearchWebService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback HelloWorldOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetSearchResultsOperationCompleted;
        
        private System.Threading.SendOrPostCallback UpdateLuceneOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public SearchWebService() {
            this.Url = global::SharedService.Properties.Settings.Default.SharedService_lucene_SearchWebService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event HelloWorldCompletedEventHandler HelloWorldCompleted;
        
        /// <remarks/>
        public event GetSearchResultsCompletedEventHandler GetSearchResultsCompleted;
        
        /// <remarks/>
        public event UpdateLuceneCompletedEventHandler UpdateLuceneCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/HelloWorld", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string HelloWorld() {
            object[] results = this.Invoke("HelloWorld", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void HelloWorldAsync() {
            this.HelloWorldAsync(null);
        }
        
        /// <remarks/>
        public void HelloWorldAsync(object userState) {
            if ((this.HelloWorldOperationCompleted == null)) {
                this.HelloWorldOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHelloWorldOperationCompleted);
            }
            this.InvokeAsync("HelloWorld", new object[0], this.HelloWorldOperationCompleted, userState);
        }
        
        private void OnHelloWorldOperationCompleted(object arg) {
            if ((this.HelloWorldCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HelloWorldCompleted(this, new HelloWorldCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetSearchResults", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public RPA.RPATicket GetSearchResults(RPA.RPATicket ticket) {
            object[] results = this.Invoke("GetSearchResults", new object[] {
                        ticket});
            return ((RPA.RPATicket)(results[0]));
        }
        
        /// <remarks/>
        public void GetSearchResultsAsync(RPATicket ticket) {
            this.GetSearchResultsAsync(ticket, null);
        }
        
        /// <remarks/>
        public void GetSearchResultsAsync(RPATicket ticket, object userState) {
            if ((this.GetSearchResultsOperationCompleted == null)) {
                this.GetSearchResultsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetSearchResultsOperationCompleted);
            }
            this.InvokeAsync("GetSearchResults", new object[] {
                        ticket}, this.GetSearchResultsOperationCompleted, userState);
        }
        
        private void OnGetSearchResultsOperationCompleted(object arg) {
            if ((this.GetSearchResultsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetSearchResultsCompleted(this, new GetSearchResultsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/UpdateLucene", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void UpdateLucene(RPA.RPAResult sampleDataFileRow) {
            this.Invoke("UpdateLucene", new object[] {
                        sampleDataFileRow});
        }
        
        /// <remarks/>
        public void UpdateLuceneAsync(RPA.RPAResult sampleDataFileRow) {
            this.UpdateLuceneAsync(sampleDataFileRow, null);
        }
        
        /// <remarks/>
        public void UpdateLuceneAsync(RPA.RPAResult sampleDataFileRow, object userState) {
            if ((this.UpdateLuceneOperationCompleted == null)) {
                this.UpdateLuceneOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUpdateLuceneOperationCompleted);
            }
            this.InvokeAsync("UpdateLucene", new object[] {
                        sampleDataFileRow}, this.UpdateLuceneOperationCompleted, userState);
        }
        
        private void OnUpdateLuceneOperationCompleted(object arg) {
            if ((this.UpdateLuceneCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UpdateLuceneCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1586.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class RPATicket {
        
        private string ticketIdField;
        
        private string ticketNumberField;
        
        private string[] categoriesField;
        
        private RPAResult[] matchesField;
        
        private string ticketTitleField;
        
        private string ticketDescriptionField;
        
        private string userIDField;
        
        private string errorField;
        
        /// <remarks/>
        public string TicketId {
            get {
                return this.ticketIdField;
            }
            set {
                this.ticketIdField = value;
            }
        }
        
        /// <remarks/>
        public string TicketNumber {
            get {
                return this.ticketNumberField;
            }
            set {
                this.ticketNumberField = value;
            }
        }
        
        /// <remarks/>
        public string[] Categories {
            get {
                return this.categoriesField;
            }
            set {
                this.categoriesField = value;
            }
        }
        
        /// <remarks/>
        public RPAResult[] Matches {
            get {
                return this.matchesField;
            }
            set {
                this.matchesField = value;
            }
        }
        
        /// <remarks/>
        public string TicketTitle {
            get {
                return this.ticketTitleField;
            }
            set {
                this.ticketTitleField = value;
            }
        }
        
        /// <remarks/>
        public string TicketDescription {
            get {
                return this.ticketDescriptionField;
            }
            set {
                this.ticketDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string userID {
            get {
                return this.userIDField;
            }
            set {
                this.userIDField = value;
            }
        }
        
        /// <remarks/>
        public string Error {
            get {
                return this.errorField;
            }
            set {
                this.errorField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1586.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class RPAResult {
        
        private int scriptIDField;
        
        private string scriptTextField;
        
        private float scoreField;
        
        /// <remarks/>
        public int ScriptID {
            get {
                return this.scriptIDField;
            }
            set {
                this.scriptIDField = value;
            }
        }
        
        /// <remarks/>
        public string ScriptText {
            get {
                return this.scriptTextField;
            }
            set {
                this.scriptTextField = value;
            }
        }
        
        /// <remarks/>
        public float Score {
            get {
                return this.scoreField;
            }
            set {
                this.scoreField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void HelloWorldCompletedEventHandler(object sender, HelloWorldCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HelloWorldCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal HelloWorldCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void GetSearchResultsCompletedEventHandler(object sender, GetSearchResultsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetSearchResultsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetSearchResultsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public RPATicket Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((RPATicket)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void UpdateLuceneCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591