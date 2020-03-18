using System.IO;
using System.Net;
using System.Text;

namespace ChatHost
{
	internal class Program
	{
		private const string proxyUri = "http://localhost/proxy/";
		private const string targetUri = "http://mail.ru/";

		static void Main(string[] args)
		{
			var listener = new HttpListener();
			listener.Prefixes.Add(proxyUri);
			listener.Start();
			while (true)
			{
				var context = listener.GetContext();
				using (var inputStream = context.Request.InputStream)
				{
					var requestBody = GetBody(inputStream);
					var responseBody = SendReceive(targetUri, requestBody);
					using (var outputStream = context.Response.OutputStream)
					{
						SetBody(outputStream, responseBody);
					}
				}
			}
		}

		private static byte[] SendReceive(string uri, byte[] body)
		{
			var webRequest = WebRequest.Create(uri);
			webRequest.Method = "POST";

			using (var requestStream = webRequest.GetRequestStream())
			{
				SetBody(requestStream, body);
			}

			var response = webRequest.GetResponse();

			using (var responseStream = response.GetResponseStream())
			{
				return GetBody(responseStream);
			}
		}

		private static byte[] GetBody(Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				return Encoding.UTF8.GetBytes(reader.ReadToEnd());
			}
		}

		private static void SetBody(Stream stream, byte[] body) =>
			stream.Write(body, 0, body.Length);
	}
}