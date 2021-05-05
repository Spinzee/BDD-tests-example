using System;
using System.ServiceModel;

namespace Products.ServiceWrapper.PersonalProjectionService
{

    [ServiceContract]
    public interface IPersonalProjectionServiceClient : IDisposable
    {
        [OperationContract(Action = "http://www.sse.com/SSE/PersonalProjectionService/IPersonalProjectionService/SiteProjection", ReplyAction = "*")]
        [FaultContract(typeof(requestFailedFaultType), Action = "http://www.sse.com/SSE/PersonalProjectionService/IPersonalProjectionService/SiteProjection", Name = "requestFailedFault", Namespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/messages/")]
        [FaultContract(typeof(invalidRequestFaultType), Action = "http://www.sse.com/SSE/PersonalProjectionService/IPersonalProjectionService/SiteProjection", Name = "invalidRequestFault", Namespace = "http://www.sse.com/SSE/PersonalProjectionService/V1/schema/messages/")]
        [XmlSerializerFormat(SupportFaults = true)]
        SiteProjectionResponse SiteProjection(SiteProjectionRequest request);

    }


    public class PersonalProjectionServiceClient : ClientBase<IPersonalProjectionServiceClient>, IPersonalProjectionServiceClient
    {
        public SiteProjectionResponse SiteProjection(SiteProjectionRequest request)
        {
            var result = Channel.SiteProjection(request);
            return result;
        }
    }
}
