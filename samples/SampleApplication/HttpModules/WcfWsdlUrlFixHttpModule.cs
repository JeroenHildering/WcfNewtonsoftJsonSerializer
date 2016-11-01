using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SampleApplication.HttpModules
{
    public class WcfWsdlUrlFixHttpModule : IHttpModule
    {
        public void Dispose() { }

        void IHttpModule.Init( HttpApplication context )
        {
            context.BeginRequest += ContextBeginRequest;
        }

        static void ContextBeginRequest( object sender, EventArgs e )
        {
            var app = sender as HttpApplication;
            if ( app != null && app.Request.Url.LocalPath.EndsWith( ".svc" ) )
            {
                app.Response.Filter = new WcfWsdlUrlFixFilter( app.Response.Filter );
            }
        }
    }

    internal class WcfWsdlUrlFixFilter : Stream
    {
        private static readonly Regex Regex = new Regex( @"(http)[^\s^\>^\'^\""]*?(\.svc)" );
        private readonly Stream _responseStream;

        public WcfWsdlUrlFixFilter( Stream responseStream )
        {
            _responseStream = responseStream;
        }

        public override bool CanRead
        {
            get { return _responseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _responseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _responseStream.CanWrite; }
        }

        public override void Flush()
        {
            _responseStream.Flush();
        }

        public override long Length
        {
            get { return _responseStream.Length; }
        }

        public override long Position
        {
            get { return _responseStream.Position; }
            set { _responseStream.Position = value; }
        }

        public override int Read( byte[] buffer, int offset, int count )
        {
            return _responseStream.Read( buffer, offset, count );
        }

        public override long Seek( long offset, SeekOrigin origin )
        {
            return _responseStream.Seek( offset, origin );
        }

        public override void SetLength( long value )
        {
            _responseStream.SetLength( value );
        }

        public override void Write( byte[] buffer, int offset, int count )
        {
            var content = System.Text.Encoding.UTF8.GetString( buffer );

            // Create URL to .svc "file" based on the current request's RawUrl
            var request = HttpContext.Current.Request;
            var url = string.Format(
              "{0}://{1}{2}{3}",
              request.Url.Scheme,
              request.Url.Host,
              ( request.Url.Port != 80 && request.Url.Port != 443 ? string.Format( ":{0}", request.Url.Port.ToString( CultureInfo.InvariantCulture ) ) : string.Empty ),
              request.RawUrl.Split( '?' ).First()
            );

            content = Regex.Replace( content, url );
            buffer = System.Text.Encoding.UTF8.GetBytes( content );
            _responseStream.Write( buffer, 0, buffer.Length );
        }
    }
}