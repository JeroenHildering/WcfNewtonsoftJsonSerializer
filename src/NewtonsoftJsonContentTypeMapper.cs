using System.ServiceModel.Channels;

namespace WcfNewtonsoftJsonSerializer
{
    public class NewtonsoftJsonContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType( string contentType )
        {
            return WebContentFormat.Raw;
        }
    }
}