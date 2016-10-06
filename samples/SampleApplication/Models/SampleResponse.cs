using System.Runtime.Serialization;

namespace SampleApplication.Models
{
    [DataContract]
    public class SampleResponse
    {
        [DataMember]
        public string Message { get; set; }
    }
}