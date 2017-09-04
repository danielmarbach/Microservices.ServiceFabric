using System.Runtime.Serialization;

namespace OrderChocolate
{
    [DataContract]
    public class Result
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public long InstanceId { get; set; }
    }
}