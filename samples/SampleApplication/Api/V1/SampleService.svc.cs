using System.ServiceModel;
using System.ServiceModel.Activation;
using SampleApplication.Models;

namespace SampleApplication.Api.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SampleService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SampleService.svc or SampleService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class SampleService : ISampleService
    {
        public SampleResponse Get(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                message = "Hello world!";

            return new SampleResponse
            {
                Message = message
            };
        }

        public SampleResponse Post(SampleRequest request)
        {
            return new SampleResponse
            {
                Message = request.Message
            };
        }

        public SampleResponse PostSingleParam(string message)
        {
            return new SampleResponse { Message = message };
        }

        public SampleResponse PostMultipleParams(string param1, string param2)
        {
            return new SampleResponse
            {
                Message = string.Format("Param1: {0}, Param2: {1}", param1, param2)
            };
        }
    }
}
