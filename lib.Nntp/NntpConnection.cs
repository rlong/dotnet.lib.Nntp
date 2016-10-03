using lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
namespace lib.Nntp
{
	public class NntpConnection
	{

		private static Log log = Log.getLog(typeof(NntpConnection));
		private static readonly String XFEATURE_COMPRESS = "XFEATURE-COMPRESS";

		LineReader _lineReader;
		NetworkStream _networkStream;
		Socket _socket;
		StreamWriter _streamWriter;
		Dictionary<String, String> _capabilities = new Dictionary<String, String>();

		public NntpConnection(String host, String username, String password)
		{

			IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddress, 119);

			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_socket.Connect(endPoint);
			_networkStream = new NetworkStream(_socket);
			_streamWriter = new StreamWriter(_networkStream);

			_lineReader = new LineReader(_networkStream);

			if (username != null && password != null)
			{
				Authenticate(username, password);
			}

			// supports compression ?
			{
				String compressHeader = this.GetCapability(XFEATURE_COMPRESS);
				log.Debug(compressHeader, "compressHeader");
				if (null != compressHeader)
				{
					if (-1 != compressHeader.IndexOf("GZIP", StringComparison.CurrentCulture))
					{
						log.info("server supports `gzip` compression");
						EnableCompression();
					}
					else {
						log.info("server supports compression but *NOT* `gzip` compression");
					}
				} else {
					log.info("server does *NOT* support compression");
				}
			}
		}

		public NntpConnection(String host) : this(host, null, null)
		{			
		}


		private void Authenticate(String username, String password)
		{

			_streamWriter.Write("AUTHINFO USER " + username);
			_streamWriter.Write('\n');
			_streamWriter.Flush();
			String response = _lineReader.ReadLine();
			log.debug(response);
		
			String authinfoPassResponse = Command("AUTHINFO PASS " + password);
			log.Debug(authinfoPassResponse, "authinfoPassResponse");

		}


		public LineIterator Body(long articleNumber)
		{

			LineIterator answer = Query("BODY " + articleNumber);
			return answer;

		}

		public LineIterator Body(String messageId)
		{

			LineIterator answer = Query("BODY " + messageId);
			return answer;

		}


		private void CheckReturnCode(String command, String response)
		{

			log.debugFormat("`{0}` >>>--->>> `{1}`", command, response);

			var tokens = response.Split(' ');
			int returnCode = int.Parse(tokens[0]);
			// http://tools.ietf.org/html/rfc3977#page-117
			if (returnCode > 399)
			{
				BaseException be = new BaseException(this, "returnCode > 299; returnCode = %d; response = '%s'", returnCode, response);
				be.ErrorDomain = ReturnCode.Lookup(returnCode, response);
				throw be;
			}
		}

		public String Command(String cmd)
		{
			_streamWriter.Write(cmd);
			_streamWriter.Write('\n');
			_streamWriter.Flush();
			String answer = ReadResponse();
			CheckReturnCode(cmd, answer);
			return answer;
		}

		private void EnableCompression()
		{
			// vvv http://lists.eyrie.org/pipermail/ietf-nntp/2009-December/006075.html

			Command( "XFEATURES" ); // should get something like '390 send features'

			_streamWriter.Write("COMPRESS GZIP");
			_streamWriter.Write('\n');
			_streamWriter.Flush();

			Command("."); // should get something like '290 features updated'

			// ^^^ http://lists.eyrie.org/pipermail/ietf-nntp/2009-December/006075.html
		}


		// can return null
		public String GetCapability(String capability)
		{
			if (null == _capabilities)
			{
				_capabilities = new Dictionary<string, string>();
				LineIterator lineIterator = Query("CAPABILITIES");
				int i = 0;
				while (lineIterator.HasNext())
				{
					String capabilityLine = lineIterator.ReadLine();
					log.debugFormat("CAPABILITIES[{0}] = '{1}'", capabilityLine, i++);

					var tokens = Regex.Split(capabilityLine, "\\s+");
					if (0 == tokens.Length)
					{
						log.warnFormat("0 == tokens.Length; capabilityLine = '{0}'", capabilityLine);
					}
					else {
						String capabilityLabel = tokens[0];
						_capabilities[capabilityLabel] = capabilityLine;
					}
				}
			}
			return _capabilities[capability];
			
		}


		public Group Group(String groupName)
		{
			// 211 87 3651 3785 alt.fan.spinal-tap
			String groupResponse = Command("GROUP " + groupName);
			return lib.Nntp.Group.BuildFromGroupResponse(groupResponse);
		}



		public GroupIterator List()
		{
			LineIterator lineIterator = Query("LIST");
			return new GroupIterator(lineIterator);
		}


		private String ReadResponse()
		{

			String answer = _lineReader.ReadLine();
			if (".".Equals(answer))
			{
				log.warn("discarding spurious '.'; can happen (sometimes) with compressed responses");
				answer = _lineReader.ReadLine();
			}
			return answer;
		}

		public LineIterator Query(String query)
		{
			_streamWriter.Write(query);
			_streamWriter.Write('\n');
			_streamWriter.Flush();
			// return code ...
			String ret = ReadResponse();


			CheckReturnCode(query, ret);

			if (-1 != ret.IndexOf("[COMPRESS=GZIP]", StringComparison.CurrentCulture))
			{
				GZipStream compressedStream = new GZipStream(_networkStream, CompressionMode.Decompress);
				LineReader lineReader = new LineReader(compressedStream);
				return new LineIterator(lineReader);
			}
			else {
				return new LineIterator(_lineReader);
			}
		}

		public MessageIterator Xover(long low, long high)
		{

			LineIterator basicHeaders = Query("XOVER " + low + "-" + high);
			return new MessageIterator(basicHeaders);
		}
	}
}
