using System;
using System.IO;
using System.Text;
using dotnet.lib.CoreAnnex.auxiliary;
using dotnet.lib.CoreAnnex.io;
using lib.CoreAnnex.log;

namespace lib.Nntp
{
	public class LineReader
	{

		// private static Log log = Log.getLog(typeof(LineReader));

		// list of next ops ...
		private static readonly int OPEN = 1;
		private static readonly int CLOSED = 2;

		int _state = OPEN;

		private MutableData _buffer;
		private Stream _inputStream;

		public LineReader(Stream inputStream)
		{
			_inputStream = inputStream;
		}

		// clears the buffer after finishing
		private byte[] ExtractBytesFromBuffer()
		{
			var count = _buffer.GetCount();
			byte[] answer = new byte[count];
			_buffer.Arraycopy(0, answer, 0, count);
			_buffer.Clear();
			return answer;
		}

		// null corresponds to the end of a stream
		public byte[] ReadLineBytes()
		{
			if (_state == CLOSED)
			{
				return null;

			}

			int byteRead = _inputStream.ReadByte();

			if (-1 == byteRead)
			{
				_state = CLOSED;
				return null;
			}

			do
			{

				// filter out '\r'
				if ('\r' != byteRead)
				{

					// end of the line
					if ('\n' == byteRead)
					{
						byte[] answer = ExtractBytesFromBuffer();
						return answer;
					}
					_buffer.Append((byte)byteRead);
				}

				byteRead = _inputStream.ReadByte();

				if (-1 == byteRead)
				{
					_state = CLOSED;
					return ExtractBytesFromBuffer();
				}

			} while (true);
		}

		// never called when in the finished state
		public String ReadLine()
		{
			byte[] bytes = ReadLineBytes();
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
