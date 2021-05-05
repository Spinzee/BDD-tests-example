using System.ServiceModel;

namespace Products.ServiceWrapper.BankDetailsService
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/serviceFault/")]
    public partial class invalidRequestFaultType : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
                this.RaisePropertyChanged("description");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
    public partial class serviceDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        private serviceDetailsTypeServiceIdentifier serviceIdentifierField;

        private string serviceUserNameField;

        private string serviceUserNumberField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public serviceDetailsTypeServiceIdentifier serviceIdentifier
        {
            get
            {
                return this.serviceIdentifierField;
            }
            set
            {
                this.serviceIdentifierField = value;
                this.RaisePropertyChanged("serviceIdentifier");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string serviceUserName
        {
            get
            {
                return this.serviceUserNameField;
            }
            set
            {
                this.serviceUserNameField = value;
                this.RaisePropertyChanged("serviceUserName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string serviceUserNumber
        {
            get
            {
                return this.serviceUserNumberField;
            }
            set
            {
                this.serviceUserNumberField = value;
                this.RaisePropertyChanged("serviceUserNumber");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
    public enum serviceDetailsTypeServiceIdentifier
    {

        /// <remarks/>
        E,

        /// <remarks/>
        G,

        /// <remarks/>
        T,

        /// <remarks/>
        H,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
    public partial class bankFormattedAddressType : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string bankAddressLine1Field;

        private string bankAddressLine2Field;

        private string bankAddressLine3Field;

        private string bankAddressLine4Field;

        private string bankPostcodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string bankAddressLine1
        {
            get
            {
                return this.bankAddressLine1Field;
            }
            set
            {
                this.bankAddressLine1Field = value;
                this.RaisePropertyChanged("bankAddressLine1");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string bankAddressLine2
        {
            get
            {
                return this.bankAddressLine2Field;
            }
            set
            {
                this.bankAddressLine2Field = value;
                this.RaisePropertyChanged("bankAddressLine2");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string bankAddressLine3
        {
            get
            {
                return this.bankAddressLine3Field;
            }
            set
            {
                this.bankAddressLine3Field = value;
                this.RaisePropertyChanged("bankAddressLine3");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string bankAddressLine4
        {
            get
            {
                return this.bankAddressLine4Field;
            }
            set
            {
                this.bankAddressLine4Field = value;
                this.RaisePropertyChanged("bankAddressLine4");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public string bankPostcode
        {
            get
            {
                return this.bankPostcodeField;
            }
            set
            {
                this.bankPostcodeField = value;
                this.RaisePropertyChanged("bankPostcode");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
    public partial class brandDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string brandCodeField;

        private System.Nullable<brandDetailsTypeBrandCodeLineOfBusiness> brandCodeLineOfBusinessField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true, Order = 0)]
        public string brandCode
        {
            get
            {
                return this.brandCodeField;
            }
            set
            {
                this.brandCodeField = value;
                this.RaisePropertyChanged("brandCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true, Order = 1)]
        public System.Nullable<brandDetailsTypeBrandCodeLineOfBusiness> brandCodeLineOfBusiness
        {
            get
            {
                return this.brandCodeLineOfBusinessField;
            }
            set
            {
                this.brandCodeLineOfBusinessField = value;
                this.RaisePropertyChanged("brandCodeLineOfBusiness");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
    public enum brandDetailsTypeBrandCodeLineOfBusiness
    {

        /// <remarks/>
        E,

        /// <remarks/>
        G,

        /// <remarks/>
        T,

        /// <remarks/>
        H,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/serviceFault/")]
    public partial class requestFailedFaultType : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
                this.RaisePropertyChanged("description");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(Namespace = "http://www.sse.com/SSE/BankDetailsService", ConfigurationName = "IBankDetailsService")]

    public interface BankDetailsServiceInterface
    {

        // CODEGEN: Generating message contract since the wrapper namespace (http://www.sse.com/SSE/BankDetailsService/V1/schema/messages/) of message getBankDetailsRequest does not match the default value (http://www.sse.com/SSE/BankDetailsService)
        [OperationContract(Action = "http://www.sse.com/SSE/BankDetailsService:getBankDetailsIn", ReplyAction = "*")]
        [FaultContract(typeof(invalidRequestFaultType), Action = "http://www.sse.com/SSE/BankDetailsService:getBankDetailsIn", Name = "invalidRequestFault", Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/messages/")]
        [FaultContract(typeof(requestFailedFaultType), Action = "http://www.sse.com/SSE/BankDetailsService:getBankDetailsIn", Name = "requestFailedFault", Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/messages/")]
        [XmlSerializerFormat(SupportFaults = true)]
        getBankDetailsResponse getBankDetails(getBankDetailsRequest request);
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
    public partial class messageHeader : object, System.ComponentModel.INotifyPropertyChanged
    {

        private messageHeaderChannelID channelIDField;

        private string clientIDField;

        private string messageIDField;

        private System.DateTime creationTimeStampField;

        private string correlationIDField;

        private string environmentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public messageHeaderChannelID channelID
        {
            get
            {
                return this.channelIDField;
            }
            set
            {
                this.channelIDField = value;
                this.RaisePropertyChanged("channelID");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string clientID
        {
            get
            {
                return this.clientIDField;
            }
            set
            {
                this.clientIDField = value;
                this.RaisePropertyChanged("clientID");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string messageID
        {
            get
            {
                return this.messageIDField;
            }
            set
            {
                this.messageIDField = value;
                this.RaisePropertyChanged("messageID");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public System.DateTime creationTimeStamp
        {
            get
            {
                return this.creationTimeStampField;
            }
            set
            {
                this.creationTimeStampField = value;
                this.RaisePropertyChanged("creationTimeStamp");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public string correlationID
        {
            get
            {
                return this.correlationIDField;
            }
            set
            {
                this.correlationIDField = value;
                this.RaisePropertyChanged("correlationID");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        public string environment
        {
            get
            {
                return this.environmentField;
            }
            set
            {
                this.environmentField = value;
                this.RaisePropertyChanged("environment");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
    public enum messageHeaderChannelID
    {

        /// <remarks/>
        Web,

        /// <remarks/>
        Mobile,

        /// <remarks/>
        Telephony,

        /// <remarks/>
        ClientUI,
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "getBankDetailsRequest", WrapperNamespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/messages/", IsWrapped = true)]
    public partial class getBankDetailsRequest
    {

        [MessageHeader(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", DataType = "integer")]
        public string sortCode;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 1)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
        public string bankAccountNumber;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 2)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("brandDetails", IsNullable = false)]
        public brandDetailsType[] brandLineOfBusinessCollection;

        public getBankDetailsRequest()
        {
        }

        public getBankDetailsRequest(messageHeader messageHeader, string sortCode, string bankAccountNumber, brandDetailsType[] brandLineOfBusinessCollection)
        {
            this.messageHeader = messageHeader;
            this.sortCode = sortCode;
            this.bankAccountNumber = bankAccountNumber;
            this.brandLineOfBusinessCollection = brandLineOfBusinessCollection;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "getBankDetailsResponse", WrapperNamespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/messages/", IsWrapped = true)]
    public partial class getBankDetailsResponse
    {

        [MessageHeader(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", DataType = "integer")]
        public string suppliedSortCode;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 1)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
        public string suppliedBankAccountNumber;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 2)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
        public string bankName;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 3)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
        public bankFormattedAddressType bankFormattedAddress;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 4)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
        public bool sortCodeValid;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 5)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
        public bool sortCodeAccountNumberValid;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 6)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
        public bool corporateAccountValid;

        [MessageBodyMember(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/", Order = 7)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/BankDetailsService/V1/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("serviceDetails", IsNullable = false)]
        public serviceDetailsType[] serviceDetailsCollection;

        public getBankDetailsResponse()
        {
        }

        public getBankDetailsResponse(messageHeader messageHeader, string suppliedSortCode, string suppliedBankAccountNumber, string bankName, bankFormattedAddressType bankFormattedAddress, bool sortCodeValid, bool sortCodeAccountNumberValid, bool corporateAccountValid, serviceDetailsType[] serviceDetailsCollection)
        {
            this.messageHeader = messageHeader;
            this.suppliedSortCode = suppliedSortCode;
            this.suppliedBankAccountNumber = suppliedBankAccountNumber;
            this.bankName = bankName;
            this.bankFormattedAddress = bankFormattedAddress;
            this.sortCodeValid = sortCodeValid;
            this.sortCodeAccountNumberValid = sortCodeAccountNumberValid;
            this.corporateAccountValid = corporateAccountValid;
            this.serviceDetailsCollection = serviceDetailsCollection;
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface BankDetailsServiceInterfaceChannel : BankDetailsServiceInterface, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BankDetailsServiceInterfaceClient : System.ServiceModel.ClientBase<BankDetailsServiceInterface>, BankDetailsServiceInterface
    {

        public BankDetailsServiceInterfaceClient()
        {
        }

        public BankDetailsServiceInterfaceClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public BankDetailsServiceInterfaceClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public BankDetailsServiceInterfaceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public BankDetailsServiceInterfaceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getBankDetailsResponse BankDetailsServiceInterface.getBankDetails(getBankDetailsRequest request)
        {
            return base.Channel.getBankDetails(request);
        }

        public string getBankDetails(ref messageHeader messageHeader, string sortCode, string bankAccountNumber, brandDetailsType[] brandLineOfBusinessCollection, out string suppliedBankAccountNumber, out string bankName, out bankFormattedAddressType bankFormattedAddress, out bool sortCodeValid, out bool sortCodeAccountNumberValid, out bool corporateAccountValid, out serviceDetailsType[] serviceDetailsCollection)
        {
            getBankDetailsRequest inValue = new getBankDetailsRequest();
            inValue.messageHeader = messageHeader;
            inValue.sortCode = sortCode;
            inValue.bankAccountNumber = bankAccountNumber;
            inValue.brandLineOfBusinessCollection = brandLineOfBusinessCollection;
            getBankDetailsResponse retVal = ((BankDetailsServiceInterface)(this)).getBankDetails(inValue);
            messageHeader = retVal.messageHeader;
            suppliedBankAccountNumber = retVal.suppliedBankAccountNumber;
            bankName = retVal.bankName;
            bankFormattedAddress = retVal.bankFormattedAddress;
            sortCodeValid = retVal.sortCodeValid;
            sortCodeAccountNumberValid = retVal.sortCodeAccountNumberValid;
            corporateAccountValid = retVal.corporateAccountValid;
            serviceDetailsCollection = retVal.serviceDetailsCollection;
            return retVal.suppliedSortCode;
        }
    }




}
