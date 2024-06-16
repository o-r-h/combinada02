﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Veritrade2017.PaymeTestReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://pe.com.alignet/WalletCommerce/", ConfigurationName="PaymeTestReference.WalletCommerce")]
    public interface WalletCommerce {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://pe.com.alignet/WalletCommerce/RegisterCardHolder", ReplyAction="*")]
        Veritrade2017.PaymeTestReference.RegisterCardHolderResponse RegisterCardHolder(Veritrade2017.PaymeTestReference.RegisterCardHolderRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://pe.com.alignet/WalletCommerce/RegisterCardHolder", ReplyAction="*")]
        System.Threading.Tasks.Task<Veritrade2017.PaymeTestReference.RegisterCardHolderResponse> RegisterCardHolderAsync(Veritrade2017.PaymeTestReference.RegisterCardHolderRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RegisterCardHolderRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="RegisterCardHolderRequest", Namespace="http://pe.com.alignet/WalletCommerce/", Order=0)]
        public Veritrade2017.PaymeTestReference.RegisterCardHolderRequestBody Body;
        
        public RegisterCardHolderRequest() {
        }
        
        public RegisterCardHolderRequest(Veritrade2017.PaymeTestReference.RegisterCardHolderRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class RegisterCardHolderRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string idEntCommerce;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string codCardHolderCommerce;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string mail;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string names;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string lastNames;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string reserved1;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string reserved2;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string reserved3;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=8)]
        public string registerVerification;
        
        public RegisterCardHolderRequestBody() {
        }
        
        public RegisterCardHolderRequestBody(string idEntCommerce, string codCardHolderCommerce, string mail, string names, string lastNames, string reserved1, string reserved2, string reserved3, string registerVerification) {
            this.idEntCommerce = idEntCommerce;
            this.codCardHolderCommerce = codCardHolderCommerce;
            this.mail = mail;
            this.names = names;
            this.lastNames = lastNames;
            this.reserved1 = reserved1;
            this.reserved2 = reserved2;
            this.reserved3 = reserved3;
            this.registerVerification = registerVerification;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RegisterCardHolderResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="RegisterCardHolderResponse", Namespace="http://pe.com.alignet/WalletCommerce/", Order=0)]
        public Veritrade2017.PaymeTestReference.RegisterCardHolderResponseBody Body;
        
        public RegisterCardHolderResponse() {
        }
        
        public RegisterCardHolderResponse(Veritrade2017.PaymeTestReference.RegisterCardHolderResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class RegisterCardHolderResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string ansCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string ansDescription;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string codAsoCardHolderWallet;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string date;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string hour;
        
        public RegisterCardHolderResponseBody() {
        }
        
        public RegisterCardHolderResponseBody(string ansCode, string ansDescription, string codAsoCardHolderWallet, string date, string hour) {
            this.ansCode = ansCode;
            this.ansDescription = ansDescription;
            this.codAsoCardHolderWallet = codAsoCardHolderWallet;
            this.date = date;
            this.hour = hour;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WalletCommerceChannel : Veritrade2017.PaymeTestReference.WalletCommerce, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WalletCommerceClient : System.ServiceModel.ClientBase<Veritrade2017.PaymeTestReference.WalletCommerce>, Veritrade2017.PaymeTestReference.WalletCommerce {
        
        public WalletCommerceClient() {
        }
        
        public WalletCommerceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WalletCommerceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WalletCommerceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WalletCommerceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Veritrade2017.PaymeTestReference.RegisterCardHolderResponse Veritrade2017.PaymeTestReference.WalletCommerce.RegisterCardHolder(Veritrade2017.PaymeTestReference.RegisterCardHolderRequest request) {
            return base.Channel.RegisterCardHolder(request);
        }
        
        public string RegisterCardHolder(string idEntCommerce, string codCardHolderCommerce, string mail, string names, string lastNames, string reserved1, string reserved2, string reserved3, string registerVerification, out string ansDescription, out string codAsoCardHolderWallet, out string date, out string hour) {
            Veritrade2017.PaymeTestReference.RegisterCardHolderRequest inValue = new Veritrade2017.PaymeTestReference.RegisterCardHolderRequest();
            inValue.Body = new Veritrade2017.PaymeTestReference.RegisterCardHolderRequestBody();
            inValue.Body.idEntCommerce = idEntCommerce;
            inValue.Body.codCardHolderCommerce = codCardHolderCommerce;
            inValue.Body.mail = mail;
            inValue.Body.names = names;
            inValue.Body.lastNames = lastNames;
            inValue.Body.reserved1 = reserved1;
            inValue.Body.reserved2 = reserved2;
            inValue.Body.reserved3 = reserved3;
            inValue.Body.registerVerification = registerVerification;
            Veritrade2017.PaymeTestReference.RegisterCardHolderResponse retVal = ((Veritrade2017.PaymeTestReference.WalletCommerce)(this)).RegisterCardHolder(inValue);
            ansDescription = retVal.Body.ansDescription;
            codAsoCardHolderWallet = retVal.Body.codAsoCardHolderWallet;
            date = retVal.Body.date;
            hour = retVal.Body.hour;
            return retVal.Body.ansCode;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Veritrade2017.PaymeTestReference.RegisterCardHolderResponse> Veritrade2017.PaymeTestReference.WalletCommerce.RegisterCardHolderAsync(Veritrade2017.PaymeTestReference.RegisterCardHolderRequest request) {
            return base.Channel.RegisterCardHolderAsync(request);
        }
        
        public System.Threading.Tasks.Task<Veritrade2017.PaymeTestReference.RegisterCardHolderResponse> RegisterCardHolderAsync(string idEntCommerce, string codCardHolderCommerce, string mail, string names, string lastNames, string reserved1, string reserved2, string reserved3, string registerVerification) {
            Veritrade2017.PaymeTestReference.RegisterCardHolderRequest inValue = new Veritrade2017.PaymeTestReference.RegisterCardHolderRequest();
            inValue.Body = new Veritrade2017.PaymeTestReference.RegisterCardHolderRequestBody();
            inValue.Body.idEntCommerce = idEntCommerce;
            inValue.Body.codCardHolderCommerce = codCardHolderCommerce;
            inValue.Body.mail = mail;
            inValue.Body.names = names;
            inValue.Body.lastNames = lastNames;
            inValue.Body.reserved1 = reserved1;
            inValue.Body.reserved2 = reserved2;
            inValue.Body.reserved3 = reserved3;
            inValue.Body.registerVerification = registerVerification;
            return ((Veritrade2017.PaymeTestReference.WalletCommerce)(this)).RegisterCardHolderAsync(inValue);
        }
    }
}
