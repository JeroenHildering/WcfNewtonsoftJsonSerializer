using System.ServiceModel;
using System.ServiceModel.Web;
using SampleApplication.Models;

namespace SampleApplication.Api.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISampleService" in both code and config file together.
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface ISampleService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/samplemessage/{*message}")]
        SampleResponse Get(string message);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/samplemessage")]
        SampleResponse Post(SampleRequest request);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/samplemessage/single")]
        SampleResponse PostSingleParam(string message);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/samplemessage/multiple")]
        SampleResponse PostMultipleParams(string param1, string param2);
    }
}
