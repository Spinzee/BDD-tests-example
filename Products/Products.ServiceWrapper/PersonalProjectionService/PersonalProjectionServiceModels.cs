using System;
using System.Runtime.Serialization;

[assembly: ContractNamespace("http://www.sse.com/SSE/PersonalProjectionService/V1/schema/serviceFault/", ClrNamespace = "www.sse.com.SSE1.PersonalProjectionService.V1.schema.serviceFault")]

namespace Products.ServiceWrapper.PersonalProjectionService
{

    [Serializable]
    [System.Xml.Serialization.XmlType(AnonymousType = true,
        Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
    public class messageHeader
    {

        private messageHeaderChannelID channelIDField;

        private string clientIDField;

        private string messageIDField;

        private System.DateTime creationTimeStampField;

        private string correlationIDField;

        private string environmentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Order = 0)]
        public messageHeaderChannelID channelID
        {
            get { return this.channelIDField; }
            set { this.channelIDField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Order = 1)]
        public string clientID
        {
            get { return this.clientIDField; }
            set { this.clientIDField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Order = 2)]
        public string messageID
        {
            get { return this.messageIDField; }
            set { this.messageIDField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Order = 3)]
        public System.DateTime creationTimeStamp
        {
            get { return this.creationTimeStampField; }
            set { this.creationTimeStampField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Order = 4)]
        public string correlationID
        {
            get { return this.correlationIDField; }
            set { this.correlationIDField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Order = 5)]
        public string environment
        {
            get { return this.environmentField; }
            set { this.environmentField = value; }
        }
    }

    [Serializable]
    [System.Xml.Serialization.XmlType(AnonymousType = true,
        Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
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

    [Serializable]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/data")]
    public class SiteConsumptionType
    {

        private string electricityConsumptionField;

        private string electricitySpendField;

        private string gasConsumptionField;

        private string gasSpendField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string ElectricityConsumption
        {
            get { return this.electricityConsumptionField; }
            set { this.electricityConsumptionField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string ElectricitySpend
        {
            get { return this.electricitySpendField; }
            set { this.electricitySpendField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string GasConsumption
        {
            get { return this.gasConsumptionField; }
            set { this.gasConsumptionField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string GasSpend
        {
            get { return this.gasSpendField; }
            set { this.gasSpendField = value; }
        }
    }

    [Serializable]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/data")]
    public class SiteType
    {

        private string siteIdField;

        private SiteConsumptionType siteConsumptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string SiteId
        {
            get { return this.siteIdField; }
            set { this.siteIdField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public SiteConsumptionType SiteConsumption
        {
            get { return this.siteConsumptionField; }
            set { this.siteConsumptionField = value; }
        }
    }

    //[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContract(WrapperName = "SiteProjectionRequest",
        WrapperNamespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/messages/", IsWrapped = true)]
    public class SiteProjectionRequest
    {

        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")]
        public messageHeader messageHeader;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/data", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/data")]
        public SiteType Site;

        public SiteProjectionRequest()
        {
        }

        public SiteProjectionRequest(messageHeader messageHeader, SiteType Site)
        {
            this.messageHeader = messageHeader;
            this.Site = Site;
        }
    }

    //[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContract(WrapperName = "SiteProjectionResponse",
        WrapperNamespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/messages/", IsWrapped = true)]
    public class SiteProjectionResponse
    {

        [System.ServiceModel.MessageHeaderAttribute(Namespace =
            "http://www.sse.com/SSE/CommonSchema/V5/schema/header/")] public messageHeader messageHeader;

        [System.ServiceModel.MessageBodyMemberAttribute(
            Namespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/data", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace =
            "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/data")] public string ReferenceId;

        public SiteProjectionResponse()
        {
        }

        public SiteProjectionResponse(messageHeader messageHeader, string ReferenceId)
        {
            this.messageHeader = messageHeader;
            this.ReferenceId = ReferenceId;
        }
    }

    [DataContract(Name = "requestFailedFaultType",
        Namespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/serviceFault/")]
    public class requestFailedFaultType : object, IExtensibleDataObject
    {

        private ExtensionDataObject extensionDataField;

        private string descriptionField;

        public ExtensionDataObject ExtensionData
        {
            get { return this.extensionDataField; }
            set { this.extensionDataField = value; }
        }

        [DataMember(EmitDefaultValue = false)]
        public string description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }
    }

    [DataContract(Name = "invalidRequestFaultType",
        Namespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/serviceFault/")]
    public class invalidRequestFaultType : object, IExtensibleDataObject
    {

        private ExtensionDataObject extensionDataField;

        private string descriptionField;

        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get { return this.extensionDataField; }
            set { this.extensionDataField = value; }
        }

        [DataMember(EmitDefaultValue = false)]
        public string description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }
    }
}