using System.Runtime.Serialization;

namespace SampleApplication.Models
{
    [DataContract]
    public class SampleRequest
    {
        [DataMember]
        public string Message { get; set; }
    }
}