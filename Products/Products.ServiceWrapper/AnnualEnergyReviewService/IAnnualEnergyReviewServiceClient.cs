using System;
using System.ServiceModel;

namespace Products.ServiceWrapper.AnnualEnergyReviewService
{
    [ServiceContract]
    public interface IAnnualEnergyReviewServiceClient : IDisposable
    {
        [OperationContract(Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:checkAERIn", ReplyAction = "*")]
        [FaultContract(typeof(invalidRequestFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:checkAERIn", Name = "invalidRequestFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [FaultContract(typeof(requestFailedFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:checkAERIn", Name = "requestFailedFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [XmlSerializerFormat(SupportFaults = true)]
        checkAERResponse checkAER(checkAERRequest request);

        [OperationContract(Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:actionAERIn", ReplyAction = "*")]
        [FaultContract(typeof(invalidRequestFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:checkAERIn", Name = "invalidRequestFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [FaultContract(typeof(requestFailedFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:checkAERIn", Name = "requestFailedFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [XmlSerializerFormat(SupportFaults = true)]
        actionAERResponse actionAER(actionAERRequest request);

        [OperationContract(Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:getNextReviewDateIn", ReplyAction = "*")]
        [FaultContract(typeof(invalidRequestFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:checkAERIn", Name = "invalidRequestFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [FaultContract(typeof(requestFailedFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:checkAERIn", Name = "requestFailedFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [XmlSerializerFormat(SupportFaults = true)]
        getNextReviewDateResponse getNextReviewDate(getNextReviewDateRequest request);


        [OperationContract(Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:getEnergyDataIn", ReplyAction = " *")]
        [FaultContract(typeof(invalidRequestFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:checkAERIn", Name = "invalidRequestFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [FaultContract(typeof(requestFailedFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:checkAERIn", Name = "requestFailedFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [XmlSerializerFormat(SupportFaults = true)]
        getEnergyDataResponse getEnergyData(getEnergyDataRequest request);


        [OperationContract(Action = "http://www.sse.com/SSE/AnnualEnergyReviewService/2.2:cancelAERIn", ReplyAction = " *")]
        [FaultContract(typeof(invalidRequestFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService:checkAERIn", Name = "invalidRequestFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [FaultContract(typeof(requestFailedFaultType), Action = "http://www.sse.com/SSE/AnnualEnergyReviewService:checkAERIn", Name = "requestFailedFaultType", Namespace = "http://www.sse.com/SSE/AnnualEnergyReviewService/V1/schema/serviceFault/")]
        [XmlSerializerFormat(SupportFaults = true)]
        cancelAERResponse cancelAER(cancelAERRequest request);

    }
}
