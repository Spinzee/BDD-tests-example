namespace Products.ServiceWrapper.AnnualEnergyReviewService
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/serviceFault/")]
    public class invalidRequestFaultType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The description field
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
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

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class customerAccountVariablesType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The name field
        /// </summary>
        private string nameField;

        /// <summary>
        /// The value field
        /// </summary>
        private string valueField;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
                this.RaisePropertyChanged("name");
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
                this.RaisePropertyChanged("value");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class checkReviewResultType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The customer account number field
        /// </summary>
        private string customerAccountNumberField;

        /// <summary>
        /// The service account number field
        /// </summary>
        private string serviceAccountNumberField;

        /// <summary>
        /// The variables collection field
        /// </summary>
        private customerAccountVariablesType[] variablesCollectionField;

        /// <summary>
        /// Gets or sets the customer account number.
        /// </summary>
        /// <value>
        /// The customer account number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string customerAccountNumber
        {
            get
            {
                return this.customerAccountNumberField;
            }
            set
            {
                this.customerAccountNumberField = value;
                this.RaisePropertyChanged("customerAccountNumber");
            }
        }

        /// <summary>
        /// Gets or sets the service account number.
        /// </summary>
        /// <value>
        /// The service account number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string serviceAccountNumber
        {
            get
            {
                return this.serviceAccountNumberField;
            }
            set
            {
                this.serviceAccountNumberField = value;
                this.RaisePropertyChanged("serviceAccountNumber");
            }
        }

        /// <summary>
        /// Gets or sets the variables collection.
        /// </summary>
        /// <value>
        /// The variables collection.
        /// </value>
        [System.Xml.Serialization.XmlArrayAttribute(Order = 2)]
        [System.Xml.Serialization.XmlArrayItemAttribute("customerAccountVariables", IsNullable = false)]
        public customerAccountVariablesType[] variablesCollection
        {
            get
            {
                return this.variablesCollectionField;
            }
            set
            {
                this.variablesCollectionField = value;
                this.RaisePropertyChanged("variablesCollection");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/serviceFault/")]
    public class requestFailedFaultType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The description field
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
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

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
    public class messageHeader : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The channel identifier field
        /// </summary>
        private messageHeaderChannelID channelIDField;

        /// <summary>
        /// The client identifier field
        /// </summary>
        private string clientIDField;

        /// <summary>
        /// The message identifier field
        /// </summary>
        private string messageIDField;

        /// <summary>
        /// The creation time stamp field
        /// </summary>
        private System.DateTime creationTimeStampField;

        /// <summary>
        /// The correlation identifier field
        /// </summary>
        private string correlationIDField;

        /// <summary>
        /// The environment field
        /// </summary>
        private string environmentField;

        /// <summary>
        /// Gets or sets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
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

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
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

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
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

        /// <summary>
        /// Gets or sets the creation time stamp.
        /// </summary>
        /// <value>
        /// The creation time stamp.
        /// </value>
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

        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
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

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
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

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
    public enum messageHeaderChannelID
    {

        /// <summary>
        /// The web
        /// </summary>
        Web,

        /// <summary>
        /// The mobile
        /// </summary>
        Mobile,

        /// <summary>
        /// The telephony
        /// </summary>
        Telephony,

        /// <summary>
        /// The client UI
        /// </summary>
        ClientUI,
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "checkAERRequest", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class checkAERRequest
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The customer account collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("customerAccountNumber", IsNullable = false)]
        public string[] customerAccountCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="checkAERRequest"/> class.
        /// </summary>
        public checkAERRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="checkAERRequest"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="customerAccountCollection">The customer account collection.</param>
        public checkAERRequest(messageHeader messageHeader, string[] customerAccountCollection)
        {
            this.messageHeader = messageHeader;
            this.customerAccountCollection = customerAccountCollection;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "checkAERResponse", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class checkAERResponse
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The review result collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("checkReviewResult", IsNullable = false)]
        public checkReviewResultType[] reviewResultCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="checkAERResponse"/> class.
        /// </summary>
        public checkAERResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="checkAERResponse"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="reviewResultCollection">The review result collection.</param>
        public checkAERResponse(messageHeader messageHeader, checkReviewResultType[] reviewResultCollection)
        {
            this.messageHeader = messageHeader;
            this.reviewResultCollection = reviewResultCollection;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class aerRequestDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The customer account number field
        /// </summary>
        private string customerAccountNumberField;

        /// <summary>
        /// The actions to take field
        /// </summary>
        private actionsToTakeType actionsToTakeField;

        /// <summary>
        /// The action details field
        /// </summary>
        private actionDetailsType actionDetailsField;

        /// <summary>
        /// Gets or sets the customer account number.
        /// </summary>
        /// <value>
        /// The customer account number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string customerAccountNumber
        {
            get
            {
                return this.customerAccountNumberField;
            }
            set
            {
                this.customerAccountNumberField = value;
                this.RaisePropertyChanged("customerAccountNumber");
            }
        }

        /// <summary>
        /// Gets or sets the actions to take.
        /// </summary>
        /// <value>
        /// The actions to take.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public actionsToTakeType actionsToTake
        {
            get
            {
                return this.actionsToTakeField;
            }
            set
            {
                this.actionsToTakeField = value;
                this.RaisePropertyChanged("actionsToTake");
            }
        }

        /// <summary>
        /// Gets or sets the action details.
        /// </summary>
        /// <value>
        /// The action details.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public actionDetailsType actionDetails
        {
            get
            {
                return this.actionDetailsField;
            }
            set
            {
                this.actionDetailsField = value;
                this.RaisePropertyChanged("actionDetails");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class actionsToTakeType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The change tariff field
        /// </summary>
        private bool changeTariffField;

        /// <summary>
        /// The change dd status field
        /// </summary>
        private bool changeDDStatusField;

        /// <summary>
        /// The change paperless status field
        /// </summary>
        private bool changePaperlessStatusField;

        /// <summary>
        /// The change account benefit field
        /// </summary>
        private bool changeAccountBenefitField;

        /// <summary>
        /// The submit meter reading field
        /// </summary>
        private bool submitMeterReadingField;

        /// <summary>
        /// The took efficiency survey field
        /// </summary>
        private bool tookEfficiencySurveyField;

        /// <summary>
        /// The no action field
        /// </summary>
        private bool noActionField;

        /// <summary>
        /// The no action field specified
        /// </summary>
        private bool noActionFieldSpecified;

        /// <summary>
        /// Gets or sets a value indicating whether [change tariff].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [change tariff]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public bool changeTariff
        {
            get
            {
                return this.changeTariffField;
            }
            set
            {
                this.changeTariffField = value;
                this.RaisePropertyChanged("changeTariff");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [change dd status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [change dd status]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public bool changeDDStatus
        {
            get
            {
                return this.changeDDStatusField;
            }
            set
            {
                this.changeDDStatusField = value;
                this.RaisePropertyChanged("changeDDStatus");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [change paperless status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [change paperless status]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public bool changePaperlessStatus
        {
            get
            {
                return this.changePaperlessStatusField;
            }
            set
            {
                this.changePaperlessStatusField = value;
                this.RaisePropertyChanged("changePaperlessStatus");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [change account benefit].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [change account benefit]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public bool changeAccountBenefit
        {
            get
            {
                return this.changeAccountBenefitField;
            }
            set
            {
                this.changeAccountBenefitField = value;
                this.RaisePropertyChanged("changeAccountBenefit");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [submit meter reading].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [submit meter reading]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public bool submitMeterReading
        {
            get
            {
                return this.submitMeterReadingField;
            }
            set
            {
                this.submitMeterReadingField = value;
                this.RaisePropertyChanged("submitMeterReading");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [took efficiency survey].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [took efficiency survey]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        public bool tookEfficiencySurvey
        {
            get
            {
                return this.tookEfficiencySurveyField;
            }
            set
            {
                this.tookEfficiencySurveyField = value;
                this.RaisePropertyChanged("tookEfficiencySurvey");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no action].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no action]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
        public bool noAction
        {
            get
            {
                return this.noActionField;
            }
            set
            {
                this.noActionField = value;
                this.RaisePropertyChanged("noAction");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no action specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no action specified]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool noActionSpecified
        {
            get
            {
                return this.noActionFieldSpecified;
            }
            set
            {
                this.noActionFieldSpecified = value;
                this.RaisePropertyChanged("noActionSpecified");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class actionDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The tariff details field
        /// </summary>
        private tariffDetailsType tariffDetailsField;

        /// <summary>
        /// The payment details field
        /// </summary>
        private paymentDetailsType paymentDetailsField;

        /// <summary>
        /// The paperless billing details field
        /// </summary>
        private paperlessBillingDetailsType paperlessBillingDetailsField;

        /// <summary>
        /// The account benefit collection field
        /// </summary>
        private accountBenefitDetailsType accountBenefitCollectionField;

        /// <summary>
        /// The meter reading details field
        /// </summary>
        private meterReadingDetailsType meterReadingDetailsField;

        /// <summary>
        /// The efficiency survey details field
        /// </summary>
        private efficiencySurveyDetailsType efficiencySurveyDetailsField;

        /// <summary>
        /// The no action details field
        /// </summary>
        private noActionDetailsType noActionDetailsField;

        /// <summary>
        /// The outcome field
        /// </summary>
        private outcomeType outcomeField;

        /// <summary>
        /// Gets or sets the tariff details.
        /// </summary>
        /// <value>
        /// The tariff details.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public tariffDetailsType tariffDetails
        {
            get
            {
                return this.tariffDetailsField;
            }
            set
            {
                this.tariffDetailsField = value;
                this.RaisePropertyChanged("tariffDetails");
            }
        }

        /// <summary>
        /// Gets or sets the payment details.
        /// </summary>
        /// <value>
        /// The payment details.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public paymentDetailsType paymentDetails
        {
            get
            {
                return this.paymentDetailsField;
            }
            set
            {
                this.paymentDetailsField = value;
                this.RaisePropertyChanged("paymentDetails");
            }
        }

        /// <summary>
        /// Gets or sets the paperless billing details.
        /// </summary>
        /// <value>
        /// The paperless billing details.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public paperlessBillingDetailsType paperlessBillingDetails
        {
            get
            {
                return this.paperlessBillingDetailsField;
            }
            set
            {
                this.paperlessBillingDetailsField = value;
                this.RaisePropertyChanged("paperlessBillingDetails");
            }
        }

        /// <summary>
        /// Gets or sets the account benefit collection.
        /// </summary>
        /// <value>
        /// The account benefit collection.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public accountBenefitDetailsType accountBenefitCollection
        {
            get
            {
                return this.accountBenefitCollectionField;
            }
            set
            {
                this.accountBenefitCollectionField = value;
                this.RaisePropertyChanged("accountBenefitCollection");
            }
        }

        /// <summary>
        /// Gets or sets the meter reading details.
        /// </summary>
        /// <value>
        /// The meter reading details.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public meterReadingDetailsType meterReadingDetails
        {
            get
            {
                return this.meterReadingDetailsField;
            }
            set
            {
                this.meterReadingDetailsField = value;
                this.RaisePropertyChanged("meterReadingDetails");
            }
        }

        /// <summary>
        /// Gets or sets the efficiency survey details.
        /// </summary>
        /// <value>
        /// The efficiency survey details.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        public efficiencySurveyDetailsType efficiencySurveyDetails
        {
            get
            {
                return this.efficiencySurveyDetailsField;
            }
            set
            {
                this.efficiencySurveyDetailsField = value;
                this.RaisePropertyChanged("efficiencySurveyDetails");
            }
        }

        /// <summary>
        /// Gets or sets the no action details.
        /// </summary>
        /// <value>
        /// The no action details.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
        public noActionDetailsType noActionDetails
        {
            get
            {
                return this.noActionDetailsField;
            }
            set
            {
                this.noActionDetailsField = value;
                this.RaisePropertyChanged("noActionDetails");
            }
        }

        /// <summary>
        /// Gets or sets the outcome.
        /// </summary>
        /// <value>
        /// The outcome.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 7)]
        public outcomeType outcome
        {
            get
            {
                return this.outcomeField;
            }
            set
            {
                this.outcomeField = value;
                this.RaisePropertyChanged("outcome");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class tariffDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// From service plan identifier field
        /// </summary>
        private string fromServicePlanIdField;

        /// <summary>
        /// The lost benefits field
        /// </summary>
        private string lostBenefitsField;

        /// <summary>
        /// The termination fee field
        /// </summary>
        private decimal terminationFeeField;

        /// <summary>
        /// The termination fee field specified
        /// </summary>
        private bool terminationFeeFieldSpecified;

        /// <summary>
        /// To service plan identifier field
        /// </summary>
        private string toServicePlanIdField;

        /// <summary>
        /// To benefit field
        /// </summary>
        private string toBenefitField;

        /// <summary>
        /// The follow on tariff flag field
        /// </summary>
        private bool followOnTariffFlagField;

        /// <summary>
        /// Gets or sets from service plan identifier.
        /// </summary>
        /// <value>
        /// From service plan identifier.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string fromServicePlanId
        {
            get
            {
                return this.fromServicePlanIdField;
            }
            set
            {
                this.fromServicePlanIdField = value;
                this.RaisePropertyChanged("fromServicePlanId");
            }
        }

        /// <summary>
        /// Gets or sets the lost benefits.
        /// </summary>
        /// <value>
        /// The lost benefits.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string lostBenefits
        {
            get
            {
                return this.lostBenefitsField;
            }
            set
            {
                this.lostBenefitsField = value;
                this.RaisePropertyChanged("lostBenefits");
            }
        }

        /// <summary>
        /// Gets or sets the termination fee.
        /// </summary>
        /// <value>
        /// The termination fee.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public decimal terminationFee
        {
            get
            {
                return this.terminationFeeField;
            }
            set
            {
                this.terminationFeeField = value;
                this.RaisePropertyChanged("terminationFee");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [termination fee specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [termination fee specified]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool terminationFeeSpecified
        {
            get
            {
                return this.terminationFeeFieldSpecified;
            }
            set
            {
                this.terminationFeeFieldSpecified = value;
                this.RaisePropertyChanged("terminationFeeSpecified");
            }
        }

        /// <summary>
        /// Gets or sets to service plan identifier.
        /// </summary>
        /// <value>
        /// To service plan identifier.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string toServicePlanId
        {
            get
            {
                return this.toServicePlanIdField;
            }
            set
            {
                this.toServicePlanIdField = value;
                this.RaisePropertyChanged("toServicePlanId");
            }
        }

        /// <summary>
        /// Gets or sets to benefit.
        /// </summary>
        /// <value>
        /// To benefit.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public string toBenefit
        {
            get
            {
                return this.toBenefitField;
            }
            set
            {
                this.toBenefitField = value;
                this.RaisePropertyChanged("toBenefit");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [follow on tariff flag].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [follow on tariff flag]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        public bool followOnTariffFlag
        {
            get
            {
                return this.followOnTariffFlagField;
            }
            set
            {
                this.followOnTariffFlagField = value;
                this.RaisePropertyChanged("followOnTariffFlag");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class paymentDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The bank account number field
        /// </summary>
        private string bankAccountNumberField;

        /// <summary>
        /// The sort code number field
        /// </summary>
        private string sortCodeNumberField;

        /// <summary>
        /// The account in name of field
        /// </summary>
        private string accountInNameOfField;

        /// <summary>
        /// The regular amount dd field
        /// </summary>
        private decimal regularAmountDDField;

        /// <summary>
        /// The collection day field
        /// </summary>
        private string collectionDayField;

        /// <summary>
        /// Gets or sets the bank account number.
        /// </summary>
        /// <value>
        /// The bank account number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string bankAccountNumber
        {
            get
            {
                return this.bankAccountNumberField;
            }
            set
            {
                this.bankAccountNumberField = value;
                this.RaisePropertyChanged("bankAccountNumber");
            }
        }

        /// <summary>
        /// Gets or sets the sort code number.
        /// </summary>
        /// <value>
        /// The sort code number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string sortCodeNumber
        {
            get
            {
                return this.sortCodeNumberField;
            }
            set
            {
                this.sortCodeNumberField = value;
                this.RaisePropertyChanged("sortCodeNumber");
            }
        }

        /// <summary>
        /// Gets or sets the account in name of.
        /// </summary>
        /// <value>
        /// The account in name of.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string accountInNameOf
        {
            get
            {
                return this.accountInNameOfField;
            }
            set
            {
                this.accountInNameOfField = value;
                this.RaisePropertyChanged("accountInNameOf");
            }
        }

        /// <summary>
        /// Gets or sets the regular amount dd.
        /// </summary>
        /// <value>
        /// The regular amount dd.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public decimal regularAmountDD
        {
            get
            {
                return this.regularAmountDDField;
            }
            set
            {
                this.regularAmountDDField = value;
                this.RaisePropertyChanged("regularAmountDD");
            }
        }

        /// <summary>
        /// Gets or sets the collection day.
        /// </summary>
        /// <value>
        /// The collection day.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 4)]
        public string collectionDay
        {
            get
            {
                return this.collectionDayField;
            }
            set
            {
                this.collectionDayField = value;
                this.RaisePropertyChanged("collectionDay");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class paperlessBillingDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The change to paperless field
        /// </summary>
        private bool changeToPaperlessField;

        /// <summary>
        /// The email address field
        /// </summary>
        private string emailAddressField;

        /// <summary>
        /// Gets or sets a value indicating whether [change to paperless].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [change to paperless]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public bool changeToPaperless
        {
            get
            {
                return this.changeToPaperlessField;
            }
            set
            {
                this.changeToPaperlessField = value;
                this.RaisePropertyChanged("changeToPaperless");
            }
        }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string emailAddress
        {
            get
            {
                return this.emailAddressField;
            }
            set
            {
                this.emailAddressField = value;
                this.RaisePropertyChanged("emailAddress");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class accountBenefitDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The benefit requested field
        /// </summary>
        private string[] benefitRequestedField;

        /// <summary>
        /// Gets or sets the benefit requested.
        /// </summary>
        /// <value>
        /// The benefit requested.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute("benefitRequested", Order = 0)]
        public string[] benefitRequested
        {
            get
            {
                return this.benefitRequestedField;
            }
            set
            {
                this.benefitRequestedField = value;
                this.RaisePropertyChanged("benefitRequested");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class meterReadingDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The electricity reading field
        /// </summary>
        private electricityReadingType electricityReadingField;

        /// <summary>
        /// The gas reading field
        /// </summary>
        private gasReadingType gasReadingField;

        /// <summary>
        /// Gets or sets the electricity reading.
        /// </summary>
        /// <value>
        /// The electricity reading.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public electricityReadingType electricityReading
        {
            get
            {
                return this.electricityReadingField;
            }
            set
            {
                this.electricityReadingField = value;
                this.RaisePropertyChanged("electricityReading");
            }
        }

        /// <summary>
        /// Gets or sets the gas reading.
        /// </summary>
        /// <value>
        /// The gas reading.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public gasReadingType gasReading
        {
            get
            {
                return this.gasReadingField;
            }
            set
            {
                this.gasReadingField = value;
                this.RaisePropertyChanged("gasReading");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class electricityReadingType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The meter serial number field
        /// </summary>
        private string meterSerialNumberField;

        /// <summary>
        /// The register collection field
        /// </summary>
        private registerType[] registerCollectionField;

        /// <summary>
        /// Gets or sets the meter serial number.
        /// </summary>
        /// <value>
        /// The meter serial number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string meterSerialNumber
        {
            get
            {
                return this.meterSerialNumberField;
            }
            set
            {
                this.meterSerialNumberField = value;
                this.RaisePropertyChanged("meterSerialNumber");
            }
        }

        /// <summary>
        /// Gets or sets the register collection.
        /// </summary>
        /// <value>
        /// The register collection.
        /// </value>
        [System.Xml.Serialization.XmlArrayAttribute(Order = 1)]
        [System.Xml.Serialization.XmlArrayItemAttribute("register", IsNullable = false)]
        public registerType[] registerCollection
        {
            get
            {
                return this.registerCollectionField;
            }
            set
            {
                this.registerCollectionField = value;
                this.RaisePropertyChanged("registerCollection");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class registerType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The smty field
        /// </summary>
        private string smtyField;

        /// <summary>
        /// The description field
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// The reading field
        /// </summary>
        private string readingField;

        /// <summary>
        /// Gets or sets the smty.
        /// </summary>
        /// <value>
        /// The smty.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string smty
        {
            get
            {
                return this.smtyField;
            }
            set
            {
                this.smtyField = value;
                this.RaisePropertyChanged("smty");
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
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

        /// <summary>
        /// Gets or sets the reading.
        /// </summary>
        /// <value>
        /// The reading.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 2)]
        public string reading
        {
            get
            {
                return this.readingField;
            }
            set
            {
                this.readingField = value;
                this.RaisePropertyChanged("reading");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class gasReadingType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The meter serial number field
        /// </summary>
        private string meterSerialNumberField;

        /// <summary>
        /// The smty field
        /// </summary>
        private string smtyField;

        /// <summary>
        /// The description field
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// The reading field
        /// </summary>
        private string readingField;

        /// <summary>
        /// Gets or sets the meter serial number.
        /// </summary>
        /// <value>
        /// The meter serial number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string meterSerialNumber
        {
            get
            {
                return this.meterSerialNumberField;
            }
            set
            {
                this.meterSerialNumberField = value;
                this.RaisePropertyChanged("meterSerialNumber");
            }
        }

        /// <summary>
        /// Gets or sets the smty.
        /// </summary>
        /// <value>
        /// The smty.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string smty
        {
            get
            {
                return this.smtyField;
            }
            set
            {
                this.smtyField = value;
                this.RaisePropertyChanged("smty");
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
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

        /// <summary>
        /// Gets or sets the reading.
        /// </summary>
        /// <value>
        /// The reading.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 3)]
        public string reading
        {
            get
            {
                return this.readingField;
            }
            set
            {
                this.readingField = value;
                this.RaisePropertyChanged("reading");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class efficiencySurveyDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The ees identifier field
        /// </summary>
        private string eesIdField;

        /// <summary>
        /// Gets or sets the ees identifier.
        /// </summary>
        /// <value>
        /// The ees identifier.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string eesId
        {
            get
            {
                return this.eesIdField;
            }
            set
            {
                this.eesIdField = value;
                this.RaisePropertyChanged("eesId");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class noActionDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The no action type field
        /// </summary>
        private noActionDetailsTypeNoActionType noActionTypeField;

        /// <summary>
        /// The date for call me field
        /// </summary>
        private System.Nullable<System.DateTime> dateForCallMeField;

        /// <summary>
        /// The date for call me field specified
        /// </summary>
        private bool dateForCallMeFieldSpecified;

        /// <summary>
        /// Gets or sets the type of the no action.
        /// </summary>
        /// <value>
        /// The type of the no action.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public noActionDetailsTypeNoActionType noActionType
        {
            get
            {
                return this.noActionTypeField;
            }
            set
            {
                this.noActionTypeField = value;
                this.RaisePropertyChanged("noActionType");
            }
        }

        /// <summary>
        /// Gets or sets the date for call me.
        /// </summary>
        /// <value>
        /// The date for call me.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true, Order = 1)]
        public System.Nullable<System.DateTime> dateForCallMe
        {
            get
            {
                return this.dateForCallMeField;
            }
            set
            {
                this.dateForCallMeField = value;
                this.RaisePropertyChanged("dateForCallMe");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [date for call me specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [date for call me specified]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool dateForCallMeSpecified
        {
            get
            {
                return this.dateForCallMeFieldSpecified;
            }
            set
            {
                this.dateForCallMeFieldSpecified = value;
                this.RaisePropertyChanged("dateForCallMeSpecified");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public enum noActionDetailsTypeNoActionType
    {

        /// <summary>
        /// The call me back
        /// </summary>
        CallMeBack,

        /// <summary>
        /// The no change
        /// </summary>
        NoChange,

        /// <summary>
        /// The no thanks
        /// </summary>
        NoThanks,
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class outcomeType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The outcome notes field
        /// </summary>
        private string outcomeNotesField;

        /// <summary>
        /// The next aer date field
        /// </summary>
        private System.Nullable<System.DateTime> nextAerDateField;

        /// <summary>
        /// The next aer date field specified
        /// </summary>
        private bool nextAerDateFieldSpecified;

        /// <summary>
        /// Gets or sets the outcome notes.
        /// </summary>
        /// <value>
        /// The outcome notes.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string outcomeNotes
        {
            get
            {
                return this.outcomeNotesField;
            }
            set
            {
                this.outcomeNotesField = value;
                this.RaisePropertyChanged("outcomeNotes");
            }
        }

        /// <summary>
        /// Gets or sets the next aer date.
        /// </summary>
        /// <value>
        /// The next aer date.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true, Order = 1)]
        public System.Nullable<System.DateTime> nextAerDate
        {
            get
            {
                return this.nextAerDateField;
            }
            set
            {
                this.nextAerDateField = value;
                this.RaisePropertyChanged("nextAerDate");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [next aer date specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [next aer date specified]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool nextAerDateSpecified
        {
            get
            {
                return this.nextAerDateFieldSpecified;
            }
            set
            {
                this.nextAerDateFieldSpecified = value;
                this.RaisePropertyChanged("nextAerDateSpecified");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "actionAERRequest", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class actionAERRequest
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The CSR user identifier
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        public string csrUserID;

        /// <summary>
        /// The aer request collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 1)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("aerRequestDetails", IsNullable = false)]
        public aerRequestDetailsType[] aerRequestCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="actionAERRequest"/> class.
        /// </summary>
        public actionAERRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="actionAERRequest"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="csrUserID">The CSR user identifier.</param>
        /// <param name="aerRequestCollection">The aer request collection.</param>
        public actionAERRequest(messageHeader messageHeader, string csrUserID, aerRequestDetailsType[] aerRequestCollection)
        {
            this.messageHeader = messageHeader;
            this.csrUserID = csrUserID;
            this.aerRequestCollection = aerRequestCollection;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "actionAERResponse", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class actionAERResponse
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The customer account collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("customerAccountNumber", IsNullable = false)]
        public string[] customerAccountCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="actionAERResponse"/> class.
        /// </summary>
        public actionAERResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="actionAERResponse"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="customerAccountCollection">The customer account collection.</param>
        public actionAERResponse(messageHeader messageHeader, string[] customerAccountCollection)
        {
            this.messageHeader = messageHeader;
            this.customerAccountCollection = customerAccountCollection;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class accountNumberType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The customer account number field
        /// </summary>
        private string customerAccountNumberField;

        /// <summary>
        /// The date review due field
        /// </summary>
        private System.DateTime dateReviewDueField;

        /// <summary>
        /// Gets or sets the customer account number.
        /// </summary>
        /// <value>
        /// The customer account number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string customerAccountNumber
        {
            get
            {
                return this.customerAccountNumberField;
            }
            set
            {
                this.customerAccountNumberField = value;
                this.RaisePropertyChanged("customerAccountNumber");
            }
        }

        /// <summary>
        /// Gets or sets the date review due.
        /// </summary>
        /// <value>
        /// The date review due.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", Order = 1)]
        public System.DateTime dateReviewDue
        {
            get
            {
                return this.dateReviewDueField;
            }
            set
            {
                this.dateReviewDueField = value;
                this.RaisePropertyChanged("dateReviewDue");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getNextReviewDateRequest", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class getNextReviewDateRequest
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The request account collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("customerAccount", IsNullable = false)]
        public string[] requestAccountCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="getNextReviewDateRequest"/> class.
        /// </summary>
        public getNextReviewDateRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="getNextReviewDateRequest"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="requestAccountCollection">The request account collection.</param>
        public getNextReviewDateRequest(messageHeader messageHeader, string[] requestAccountCollection)
        {
            this.messageHeader = messageHeader;
            this.requestAccountCollection = requestAccountCollection;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getNextReviewDateResponse", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class getNextReviewDateResponse
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The next review due date
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", DataType = "date")]
        public System.DateTime nextReviewDueDate;

        /// <summary>
        /// The account collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 1)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("accountNumber", IsNullable = false)]
        public accountNumberType[] accountCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="getNextReviewDateResponse"/> class.
        /// </summary>
        public getNextReviewDateResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="getNextReviewDateResponse"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="nextReviewDueDate">The next review due date.</param>
        /// <param name="accountCollection">The account collection.</param>
        public getNextReviewDateResponse(messageHeader messageHeader, System.DateTime nextReviewDueDate, accountNumberType[] accountCollection)
        {
            this.messageHeader = messageHeader;
            this.nextReviewDueDate = nextReviewDueDate;
            this.accountCollection = accountCollection;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class customerAccountType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The customer account number field
        /// </summary>
        private string customerAccountNumberField;

        /// <summary>
        /// The customer name field
        /// </summary>
        private string customerNameField;

        /// <summary>
        /// The internet registration field
        /// </summary>
        private internetRegistrationType internetRegistrationField;

        /// <summary>
        /// The consumption details field
        /// </summary>
        private consumptionDetailsType consumptionDetailsField;

        /// <summary>
        /// The payment method field
        /// </summary>
        private paymentMethodType paymentMethodField;

        /// <summary>
        /// The benefits collection field
        /// </summary>
        private benefitsType[] benefitsCollectionField;

        /// <summary>
        /// The discount indicators field
        /// </summary>
        private discountIndicatorsType discountIndicatorsField;

        /// <summary>
        /// The last aer field
        /// </summary>
        private lastAERType lastAERField;

        /// <summary>
        /// The service plan details field
        /// </summary>
        private servicePlanDetailsType servicePlanDetailsField;

        private string energyAssistTariffEligibleField;

        /// <summary>
        /// Gets or sets the customer account number.
        /// </summary>
        /// <value>
        /// The customer account number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string customerAccountNumber
        {
            get
            {
                return this.customerAccountNumberField;
            }
            set
            {
                this.customerAccountNumberField = value;
                this.RaisePropertyChanged("customerAccountNumber");
            }
        }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        /// <value>
        /// The name of the customer.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string customerName
        {
            get
            {
                return this.customerNameField;
            }
            set
            {
                this.customerNameField = value;
                this.RaisePropertyChanged("customerName");
            }
        }

        /// <summary>
        /// Gets or sets the internet registration.
        /// </summary>
        /// <value>
        /// The internet registration.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public internetRegistrationType internetRegistration
        {
            get
            {
                return this.internetRegistrationField;
            }
            set
            {
                this.internetRegistrationField = value;
                this.RaisePropertyChanged("internetRegistration");
            }
        }

        /// <summary>
        /// Gets or sets the consumption details.
        /// </summary>
        /// <value>
        /// The consumption details.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public consumptionDetailsType consumptionDetails
        {
            get
            {
                return this.consumptionDetailsField;
            }
            set
            {
                this.consumptionDetailsField = value;
                this.RaisePropertyChanged("consumptionDetails");
            }
        }

        /// <summary>
        /// Gets or sets the payment method.
        /// </summary>
        /// <value>
        /// The payment method.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public paymentMethodType paymentMethod
        {
            get
            {
                return this.paymentMethodField;
            }
            set
            {
                this.paymentMethodField = value;
                this.RaisePropertyChanged("paymentMethod");
            }
        }

        /// <summary>
        /// Gets or sets the benefits collection.
        /// </summary>
        /// <value>
        /// The benefits collection.
        /// </value>
        [System.Xml.Serialization.XmlArrayAttribute(Order = 5)]
        [System.Xml.Serialization.XmlArrayItemAttribute("benefits", IsNullable = false)]
        public benefitsType[] benefitsCollection
        {
            get
            {
                return this.benefitsCollectionField;
            }
            set
            {
                this.benefitsCollectionField = value;
                this.RaisePropertyChanged("benefitsCollection");
            }
        }

        /// <summary>
        /// Gets or sets the discount indicators.
        /// </summary>
        /// <value>
        /// The discount indicators.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
        public discountIndicatorsType discountIndicators
        {
            get
            {
                return this.discountIndicatorsField;
            }
            set
            {
                this.discountIndicatorsField = value;
                this.RaisePropertyChanged("discountIndicators");
            }
        }

        /// <summary>
        /// Gets or sets the last aer.
        /// </summary>
        /// <value>
        /// The last aer.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 7)]
        public lastAERType lastAER
        {
            get
            {
                return this.lastAERField;
            }
            set
            {
                this.lastAERField = value;
                this.RaisePropertyChanged("lastAER");
            }
        }

        /// <summary>
        /// Gets or sets the service plan details.
        /// </summary>
        /// <value>
        /// The service plan details.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 8)]
        public servicePlanDetailsType servicePlanDetails
        {
            get
            {
                return this.servicePlanDetailsField;
            }
            set
            {
                this.servicePlanDetailsField = value;
                this.RaisePropertyChanged("servicePlanDetails");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 9)]
        public string EnergyAssistTariffEligible
        {
            get
            {
                return this.energyAssistTariffEligibleField;
            }
            set
            {
                this.energyAssistTariffEligibleField = value;
                this.RaisePropertyChanged("EnergyAssistTariffEligible");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class internetRegistrationType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The internet registered field
        /// </summary>
        private bool internetRegisteredField;

        /// <summary>
        /// The internet address field
        /// </summary>
        private string internetAddressField;

        /// <summary>
        /// Gets or sets a value indicating whether [internet registered].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [internet registered]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public bool internetRegistered
        {
            get
            {
                return this.internetRegisteredField;
            }
            set
            {
                this.internetRegisteredField = value;
                this.RaisePropertyChanged("internetRegistered");
            }
        }

        /// <summary>
        /// Gets or sets the internet address.
        /// </summary>
        /// <value>
        /// The internet address.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string internetAddress
        {
            get
            {
                return this.internetAddressField;
            }
            set
            {
                this.internetAddressField = value;
                this.RaisePropertyChanged("internetAddress");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class consumptionDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The consumption rule description field
        /// </summary>
        private consumptionDetailsTypeConsumptionRuleDescription consumptionRuleDescriptionField;

        /// <summary>
        /// The annual usage kilo watt hours field
        /// </summary>
        private string annualUsageKiloWattHoursField;

        /// <summary>
        /// The annual usage amount field
        /// </summary>
        private decimal annualUsageAmountField;

        /// <summary>
        /// The lc31a period end date field
        /// </summary>
        private System.DateTime lc31aPeriodEndDateField;

        /// <summary>
        /// The lc31a period end date field specified
        /// </summary>
        private bool lc31aPeriodEndDateFieldSpecified;

        /// <summary>
        /// The peak percentage field
        /// </summary>
        private decimal peakPercentageField;

        /// <summary>
        /// Gets or sets the consumption rule description.
        /// </summary>
        /// <value>
        /// The consumption rule description.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public consumptionDetailsTypeConsumptionRuleDescription consumptionRuleDescription
        {
            get
            {
                return this.consumptionRuleDescriptionField;
            }
            set
            {
                this.consumptionRuleDescriptionField = value;
                this.RaisePropertyChanged("consumptionRuleDescription");
            }
        }

        /// <summary>
        /// Gets or sets the annual usage kilo watt hours.
        /// </summary>
        /// <value>
        /// The annual usage kilo watt hours.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 1)]
        public string annualUsageKiloWattHours
        {
            get
            {
                return this.annualUsageKiloWattHoursField;
            }
            set
            {
                this.annualUsageKiloWattHoursField = value;
                this.RaisePropertyChanged("annualUsageKiloWattHours");
            }
        }

        /// <summary>
        /// Gets or sets the annual usage amount.
        /// </summary>
        /// <value>
        /// The annual usage amount.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public decimal annualUsageAmount
        {
            get
            {
                return this.annualUsageAmountField;
            }
            set
            {
                this.annualUsageAmountField = value;
                this.RaisePropertyChanged("annualUsageAmount");
            }
        }

        /// <summary>
        /// Gets or sets the lc31a period end date.
        /// </summary>
        /// <value>
        /// The lc31a period end date.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", Order = 3)]
        public System.DateTime lc31aPeriodEndDate
        {
            get
            {
                return this.lc31aPeriodEndDateField;
            }
            set
            {
                this.lc31aPeriodEndDateField = value;
                this.RaisePropertyChanged("lc31aPeriodEndDate");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [lc31a period end date specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [lc31a period end date specified]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lc31aPeriodEndDateSpecified
        {
            get
            {
                return this.lc31aPeriodEndDateFieldSpecified;
            }
            set
            {
                this.lc31aPeriodEndDateFieldSpecified = value;
                this.RaisePropertyChanged("lc31aPeriodEndDateSpecified");
            }
        }

        /// <summary>
        /// Gets or sets the peak percentage.
        /// </summary>
        /// <value>
        /// The peak percentage.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public decimal peakPercentage
        {
            get
            {
                return this.peakPercentageField;
            }
            set
            {
                this.peakPercentageField = value;
                this.RaisePropertyChanged("peakPercentage");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public enum consumptionDetailsTypeConsumptionRuleDescription
    {

        /// <summary>
        /// The lc31a
        /// </summary>
        lc31a,

        /// <summary>
        /// The eac or aq
        /// </summary>
        eac_or_aq,

        /// <summary>
        /// The validationestimate
        /// </summary>
        validationestimate,

        /// <summary>
        /// The other
        /// </summary>
        other,

        /// <summary>
        /// The previoussaforecast
        /// </summary>
        previoussaforecast,
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class paymentMethodType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The payment method code field
        /// </summary>
        private paymentMethodTypePaymentMethodCode paymentMethodCodeField;

        /// <summary>
        /// The direct debit field
        /// </summary>
        private paymentMethodTypeDirectDebit directDebitField;

        /// <summary>
        /// The earliest dd start date field
        /// </summary>
        private System.DateTime earliestDDStartDateField;

        /// <summary>
        /// The earliest dd start date field specified
        /// </summary>
        private bool earliestDDStartDateFieldSpecified;

        /// <summary>
        /// Gets or sets the payment method code.
        /// </summary>
        /// <value>
        /// The payment method code.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public paymentMethodTypePaymentMethodCode paymentMethodCode
        {
            get
            {
                return this.paymentMethodCodeField;
            }
            set
            {
                this.paymentMethodCodeField = value;
                this.RaisePropertyChanged("paymentMethodCode");
            }
        }

        /// <summary>
        /// Gets or sets the direct debit.
        /// </summary>
        /// <value>
        /// The direct debit.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public paymentMethodTypeDirectDebit directDebit
        {
            get
            {
                return this.directDebitField;
            }
            set
            {
                this.directDebitField = value;
                this.RaisePropertyChanged("directDebit");
            }
        }

        /// <summary>
        /// Gets or sets the earliest dd start date.
        /// </summary>
        /// <value>
        /// The earliest dd start date.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", Order = 2)]
        public System.DateTime earliestDDStartDate
        {
            get
            {
                return this.earliestDDStartDateField;
            }
            set
            {
                this.earliestDDStartDateField = value;
                this.RaisePropertyChanged("earliestDDStartDate");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [earliest dd start date specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [earliest dd start date specified]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool earliestDDStartDateSpecified
        {
            get
            {
                return this.earliestDDStartDateFieldSpecified;
            }
            set
            {
                this.earliestDDStartDateFieldSpecified = value;
                this.RaisePropertyChanged("earliestDDStartDateSpecified");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public enum paymentMethodTypePaymentMethodCode
    {

        /// <summary>
        /// The DDB
        /// </summary>
        DDB,

        /// <summary>
        /// The DDV
        /// </summary>
        DDV,

        /// <summary>
        /// The qc
        /// </summary>
        QC,

        /// <summary>
        /// The PPT
        /// </summary>
        PPT,

        /// <summary>
        /// The sto
        /// </summary>
        STO,

        /// <summary>
        /// The exc
        /// </summary>
        EXC,

        /// <summary>
        /// The pgo
        /// </summary>
        PGO,
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class paymentMethodTypeDirectDebit : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The bank account number field
        /// </summary>
        private string bankAccountNumberField;

        /// <summary>
        /// The sort code number field
        /// </summary>
        private string sortCodeNumberField;

        /// <summary>
        /// The account in name of field
        /// </summary>
        private string accountInNameOfField;

        /// <summary>
        /// The direct debit amount field
        /// </summary>
        private decimal directDebitAmountField;

        /// <summary>
        /// The collection day field
        /// </summary>
        private string collectionDayField;

        /// <summary>
        /// Gets or sets the bank account number.
        /// </summary>
        /// <value>
        /// The bank account number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string bankAccountNumber
        {
            get
            {
                return this.bankAccountNumberField;
            }
            set
            {
                this.bankAccountNumberField = value;
                this.RaisePropertyChanged("bankAccountNumber");
            }
        }

        /// <summary>
        /// Gets or sets the sort code number.
        /// </summary>
        /// <value>
        /// The sort code number.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string sortCodeNumber
        {
            get
            {
                return this.sortCodeNumberField;
            }
            set
            {
                this.sortCodeNumberField = value;
                this.RaisePropertyChanged("sortCodeNumber");
            }
        }

        /// <summary>
        /// Gets or sets the account in name of.
        /// </summary>
        /// <value>
        /// The account in name of.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string accountInNameOf
        {
            get
            {
                return this.accountInNameOfField;
            }
            set
            {
                this.accountInNameOfField = value;
                this.RaisePropertyChanged("accountInNameOf");
            }
        }

        /// <summary>
        /// Gets or sets the direct debit amount.
        /// </summary>
        /// <value>
        /// The direct debit amount.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public decimal directDebitAmount
        {
            get
            {
                return this.directDebitAmountField;
            }
            set
            {
                this.directDebitAmountField = value;
                this.RaisePropertyChanged("directDebitAmount");
            }
        }

        /// <summary>
        /// Gets or sets the collection day.
        /// </summary>
        /// <value>
        /// The collection day.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 4)]
        public string collectionDay
        {
            get
            {
                return this.collectionDayField;
            }
            set
            {
                this.collectionDayField = value;
                this.RaisePropertyChanged("collectionDay");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class benefitsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The benefit code field
        /// </summary>
        private string benefitCodeField;

        /// <summary>
        /// The benefit description field
        /// </summary>
        private string benefitDescriptionField;

        /// <summary>
        /// Gets or sets the benefit code.
        /// </summary>
        /// <value>
        /// The benefit code.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string benefitCode
        {
            get
            {
                return this.benefitCodeField;
            }
            set
            {
                this.benefitCodeField = value;
                this.RaisePropertyChanged("benefitCode");
            }
        }

        /// <summary>
        /// Gets or sets the benefit description.
        /// </summary>
        /// <value>
        /// The benefit description.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string benefitDescription
        {
            get
            {
                return this.benefitDescriptionField;
            }
            set
            {
                this.benefitDescriptionField = value;
                this.RaisePropertyChanged("benefitDescription");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class discountIndicatorsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The direct debit indicator field
        /// </summary>
        private bool directDebitIndicatorField;

        /// <summary>
        /// The paperless billing indicator field
        /// </summary>
        private bool paperlessBillingIndicatorField;

        /// <summary>
        /// The staff discount indicator field
        /// </summary>
        private bool staffDiscountIndicatorField;

        /// <summary>
        /// Gets or sets a value indicating whether [direct debit indicator].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [direct debit indicator]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public bool directDebitIndicator
        {
            get
            {
                return this.directDebitIndicatorField;
            }
            set
            {
                this.directDebitIndicatorField = value;
                this.RaisePropertyChanged("directDebitIndicator");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [paperless billing indicator].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [paperless billing indicator]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public bool paperlessBillingIndicator
        {
            get
            {
                return this.paperlessBillingIndicatorField;
            }
            set
            {
                this.paperlessBillingIndicatorField = value;
                this.RaisePropertyChanged("paperlessBillingIndicator");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [staff discount indicator].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [staff discount indicator]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public bool staffDiscountIndicator
        {
            get
            {
                return this.staffDiscountIndicatorField;
            }
            set
            {
                this.staffDiscountIndicatorField = value;
                this.RaisePropertyChanged("staffDiscountIndicator");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class lastAERType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The date of last aer field
        /// </summary>
        private System.DateTime dateOfLastAerField;

        /// <summary>
        /// The CSR user identifier field
        /// </summary>
        private string csrUserIdField;

        /// <summary>
        /// The ees taken on aer field
        /// </summary>
        private bool eesTakenOnAerField;

        /// <summary>
        /// The ees identifier field
        /// </summary>
        private string eesIdField;

        /// <summary>
        /// Gets or sets the date of last aer.
        /// </summary>
        /// <value>
        /// The date of last aer.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", Order = 0)]
        public System.DateTime dateOfLastAer
        {
            get
            {
                return this.dateOfLastAerField;
            }
            set
            {
                this.dateOfLastAerField = value;
                this.RaisePropertyChanged("dateOfLastAer");
            }
        }

        /// <summary>
        /// Gets or sets the CSR user identifier.
        /// </summary>
        /// <value>
        /// The CSR user identifier.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string csrUserId
        {
            get
            {
                return this.csrUserIdField;
            }
            set
            {
                this.csrUserIdField = value;
                this.RaisePropertyChanged("csrUserId");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ees taken on aer].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ees taken on aer]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public bool eesTakenOnAer
        {
            get
            {
                return this.eesTakenOnAerField;
            }
            set
            {
                this.eesTakenOnAerField = value;
                this.RaisePropertyChanged("eesTakenOnAer");
            }
        }

        /// <summary>
        /// Gets or sets the ees identifier.
        /// </summary>
        /// <value>
        /// The ees identifier.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string eesId
        {
            get
            {
                return this.eesIdField;
            }
            set
            {
                this.eesIdField = value;
                this.RaisePropertyChanged("eesId");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public class servicePlanDetailsType : object, System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The plan identifier field
        /// </summary>
        private string planIdField;

        /// <summary>
        /// The plan end date field
        /// </summary>
        private System.DateTime planEndDateField;

        /// <summary>
        /// The plan end date field specified
        /// </summary>
        private bool planEndDateFieldSpecified;

        /// <summary>
        /// The termination fee field
        /// </summary>
        private decimal terminationFeeField;

        /// <summary>
        /// The termination fee field specified
        /// </summary>
        private bool terminationFeeFieldSpecified;

        private string followOnTariffServicePlanIdField;

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string planId
        {
            get
            {
                return this.planIdField;
            }
            set
            {
                this.planIdField = value;
                this.RaisePropertyChanged("planId");
            }
        }

        /// <summary>
        /// Gets or sets the plan end date.
        /// </summary>
        /// <value>
        /// The plan end date.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", Order = 1)]
        public System.DateTime planEndDate
        {
            get
            {
                return this.planEndDateField;
            }
            set
            {
                this.planEndDateField = value;
                this.RaisePropertyChanged("planEndDate");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [plan end date specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [plan end date specified]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool planEndDateSpecified
        {
            get
            {
                return this.planEndDateFieldSpecified;
            }
            set
            {
                this.planEndDateFieldSpecified = value;
                this.RaisePropertyChanged("planEndDateSpecified");
            }
        }

        /// <summary>
        /// Gets or sets the termination fee.
        /// </summary>
        /// <value>
        /// The termination fee.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public decimal terminationFee
        {
            get
            {
                return this.terminationFeeField;
            }
            set
            {
                this.terminationFeeField = value;
                this.RaisePropertyChanged("terminationFee");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [termination fee specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [termination fee specified]; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool terminationFeeSpecified
        {
            get
            {
                return this.terminationFeeFieldSpecified;
            }
            set
            {
                this.terminationFeeFieldSpecified = value;
                this.RaisePropertyChanged("terminationFeeSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string FollowOnTariffServicePlanId
        {
            get
            {
                return this.followOnTariffServicePlanIdField;
            }
            set
            {
                this.followOnTariffServicePlanIdField = value;
                this.RaisePropertyChanged("FollowOnTariffServicePlanId");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getEnergyDataRequest", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class getEnergyDataRequest
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The customer account collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("customerAccountNumber", IsNullable = false)]
        public string[] customerAccountCollection;

        /// <summary>
        /// The service account collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 1)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("serviceAccountNumber", IsNullable = false)]
        public string[] serviceAccountCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="getEnergyDataRequest"/> class.
        /// </summary>
        public getEnergyDataRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="getEnergyDataRequest"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="customerAccountCollection">The customer account collection.</param>
        /// <param name="serviceAccountCollection">The service account collection.</param>
        public getEnergyDataRequest(messageHeader messageHeader, string[] customerAccountCollection, string[] serviceAccountCollection)
        {
            this.messageHeader = messageHeader;
            this.customerAccountCollection = customerAccountCollection;
            this.serviceAccountCollection = serviceAccountCollection;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getEnergyDataResponse", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class getEnergyDataResponse
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The accounts collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("customerAccount", IsNullable = false)]
        public customerAccountType[] accountsCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="getEnergyDataResponse"/> class.
        /// </summary>
        public getEnergyDataResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="getEnergyDataResponse"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="accountsCollection">The accounts collection.</param>
        public getEnergyDataResponse(messageHeader messageHeader, customerAccountType[] accountsCollection)
        {
            this.messageHeader = messageHeader;
            this.accountsCollection = accountsCollection;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
    public enum cancelAERRequestTypeAction
    {

        /// <summary>
        /// The cancel
        /// </summary>
        cancel,
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "cancelAERRequest", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class cancelAERRequest
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The cancel aer collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("customerAccountNumber", IsNullable = false)]
        public string[] cancelAERCollection;

        /// <summary>
        /// The action
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 1)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        public cancelAERRequestTypeAction action;

        /// <summary>
        /// The CSR user identifier
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 2)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        public string csrUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="cancelAERRequest"/> class.
        /// </summary>
        public cancelAERRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cancelAERRequest"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="cancelAERCollection">The cancel aer collection.</param>
        /// <param name="action">The action.</param>
        /// <param name="csrUserId">The CSR user identifier.</param>
        public cancelAERRequest(messageHeader messageHeader, string[] cancelAERCollection, cancelAERRequestTypeAction action, string csrUserId)
        {
            this.messageHeader = messageHeader;
            this.cancelAERCollection = cancelAERCollection;
            this.action = action;
            this.csrUserId = csrUserId;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "cancelAERResponse", WrapperNamespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/messages/", IsWrapped = true)]
    public class cancelAERResponse
    {

        /// <summary>
        /// The message header
        /// </summary>
        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        /// <summary>
        /// The cancel aer collection
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        [System.Xml.Serialization.XmlArrayItemAttribute("customerAccountNumber", IsNullable = false)]
        public string[] cancelAERCollection;

        /// <summary>
        /// The date time stamp
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/", Order = 1)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V2/schema/data/")]
        public System.DateTime dateTimeStamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="cancelAERResponse"/> class.
        /// </summary>
        public cancelAERResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cancelAERResponse"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="cancelAERCollection">The cancel aer collection.</param>
        /// <param name="dateTimeStamp">The date time stamp.</param>
        public cancelAERResponse(messageHeader messageHeader, string[] cancelAERCollection, System.DateTime dateTimeStamp)
        {
            this.messageHeader = messageHeader;
            this.cancelAERCollection = cancelAERCollection;
            this.dateTimeStamp = dateTimeStamp;
        }
    }
}
