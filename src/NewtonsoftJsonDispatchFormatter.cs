using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using Newtonsoft.Json.Converters;

namespace WcfNewtonsoftJsonSerializer
{
    public class NewtonsoftJsonDispatchFormatter : IDispatchMessageFormatter
    {
        private readonly OperationDescription _operation;
        private readonly Dictionary<string, int> _parameterNames;

        public NewtonsoftJsonDispatchFormatter( OperationDescription operation, bool isRequest )
        {
            _operation = operation;
            if ( isRequest )
            {
                var operationParameterCount = operation.Messages[0].Body.Parts.Count;
                if ( operationParameterCount > 1 )
                {
                    _parameterNames = new Dictionary<string, int>();
                    for ( var i = 0; i < operationParameterCount; i++ )
                    {
                        _parameterNames.Add( operation.Messages[0].Body.Parts[i].Name, i );
                    }
                }
            }
        }

        public void DeserializeRequest( Message message, object[] parameters )
        {
            object bodyFormatProperty;
            if ( !message.Properties.TryGetValue( WebBodyFormatMessageProperty.Name, out bodyFormatProperty ) ||
                 ( bodyFormatProperty as WebBodyFormatMessageProperty ).Format != WebContentFormat.Raw )
            {
                throw new InvalidOperationException( "Incoming messages must have a body format of Raw. Is a ContentTypeMapper set on the WebHttpBinding?" );
            }

            var bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement( "Binary" );
            var rawBody = bodyReader.ReadContentAsBase64();
            var ms = new MemoryStream( rawBody );

            var sr = new StreamReader( ms );
            var serializer = new Newtonsoft.Json.JsonSerializer();
            if ( parameters.Length == 1 )
            {
                // single parameter, assuming bare
                parameters[0] = serializer.Deserialize( sr, _operation.Messages[0].Body.Parts[0].Type );
            }
            else
            {
                // multiple parameter, needs to be wrapped
                Newtonsoft.Json.JsonReader reader = new Newtonsoft.Json.JsonTextReader( sr );
                reader.Read();
                if ( reader.TokenType != Newtonsoft.Json.JsonToken.StartObject )
                {
                    throw new InvalidOperationException( "Input needs to be wrapped in an object" );
                }

                reader.Read();
                while ( reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName )
                {
                    var parameterName = reader.Value as string;
                    reader.Read();
                    if ( _parameterNames.ContainsKey( parameterName ) )
                    {
                        var parameterIndex = _parameterNames[parameterName];
                        parameters[parameterIndex] = serializer.Deserialize( reader, _operation.Messages[0].Body.Parts[parameterIndex].Type );
                    }
                    else
                    {
                        reader.Skip();
                    }

                    reader.Read();
                }

                reader.Close();
            }

            sr.Close();
            ms.Close();
        }

        public Message SerializeReply( MessageVersion messageVersion, object[] parameters, object result )
        {
            byte[] body;
            var serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Add( new IsoDateTimeConverter() {DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ", DateTimeStyles = DateTimeStyles.AdjustToUniversal} );

            using ( var ms = new MemoryStream() )
            {
                using ( var sw = new StreamWriter( ms, Encoding.UTF8 ) )
                {
                    using ( Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter( sw ) )
                    {
                        //writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                        serializer.Serialize( writer, result );
                        sw.Flush();
                        body = ms.ToArray();
                    }
                }
            }

            var replyMessage = Message.CreateMessage( messageVersion, _operation.Messages[1].Action, new RawBodyWriter( body ) );
            replyMessage.Properties.Add( WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty( WebContentFormat.Raw ) );
            var respProp = new HttpResponseMessageProperty();
            respProp.Headers[HttpResponseHeader.ContentType] = "application/json";
            replyMessage.Properties.Add( HttpResponseMessageProperty.Name, respProp );
            return replyMessage;
        }
    }
}