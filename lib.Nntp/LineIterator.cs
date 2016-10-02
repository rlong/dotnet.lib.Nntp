using System;
using System.Text;

namespace lib.Nntp
{
	public class LineIterator
	{

		// list of next ops ...
		private static readonly int READ_FROM_SOCKET = 1;
		private static readonly int USE_READ_AHEAD = 2;
		private static readonly int FINISHED = 3;

		private byte[] ReadAhead;
		LineReader LineReader;
		int State = READ_FROM_SOCKET;


		public LineIterator(LineReader lineReader)
		{
			LineReader = lineReader;
		}

		// null marks end of the iterator
		public byte[] ReadLineBytes()
		{
			if (State == FINISHED)
			{
				return null;
			}

			if (State == USE_READ_AHEAD)
			{

				byte[] answer = ReadAhead;
				State = READ_FROM_SOCKET;
				return answer;
			}


			byte[] lineBytes = LineReader.ReadLineBytes();

			// end of stream ? 
			if (null == lineBytes)
			{

				State = FINISHED;
				return null;
			}

			// end of line iterator ?
			if (lineBytes.Length == 1 && '.' == lineBytes[0])
			{

				State = FINISHED;
				return null;
			}

			return lineBytes;

		}

		public bool HasNext()
		{
			if (State == FINISHED)
			{
				return false;
			}

			if (State == USE_READ_AHEAD)
			{
				return true;
			}


			byte[] lineBytes = LineReader.ReadLineBytes();

			// end of stream ? 
			if (null == lineBytes)
			{

				State = FINISHED;
				return false;

			}

			// end of line iterator ?
			if (lineBytes.Length == 1 && '.' == lineBytes[0])
			{

				State = FINISHED;
				return false;

			}

			ReadAhead = lineBytes;
			State = USE_READ_AHEAD;

			return true;

		}

		public String ReadLine()
		{

			byte[] lineBytes = this.ReadLineBytes();

			if (null == lineBytes)
			{
				return null;
			}

			return Encoding.UTF8.GetString(lineBytes);

		}

	}
}
