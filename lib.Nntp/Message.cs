
using System;
using System.Text;
using System.Text.RegularExpressions;
using dotnet.lib.CoreAnnex.auxiliary;
using lib.CoreAnnex.log;

namespace lib.Nntp
{
	public class Message
	{

		private static Log log = Log.getLog(typeof(Message));

		public String Raw { get; private set; }
		public long ArticleNumber { get; private set; }
		public String Subject { get; private set; }
		public String From { get; private set; }
		public MessageDate Date { get; private set; }
		public String MessageId { get; private set; }
		public String[] References { get; private set; }
		public int Bytes { get; private set; }
		public int Lines { get; private set; }
		public CrossReference XRef { get; private set; }

		public Message( String basicHeader )
		{

			Raw = basicHeader;


			var tokens = basicHeader.Split('\t');

			// from 8.3.2 of rfc3977 ...
			ArticleNumber = NumericUtilities.ParseInteger(tokens[0]);
			Subject = tokens[1];
			From = tokens[2];
			Date = new MessageDate(tokens[3]); 
			MessageId = tokens[4];
			var references = tokens[5];
			References = BuildReferences(references );
			Bytes = NumericUtilities.ParseInteger(tokens[6]);
			Lines = NumericUtilities.ParseInteger(tokens[7]);

			if (8 < tokens.Length)
			{
				StringBuilder xrefValue = new StringBuilder();

				for (int i = 8, count = tokens.Length; i < count; i++)
				{
					xrefValue.Append( tokens[i] );
				}
				XRef = CrossReference.Build(xrefValue.ToString());
			}
		}

		public static String[] BuildReferences(String references)
		{
			// no references ? 
			if ("".Equals(references) ) {
				return new String[] { };				
			}

			// vvv https://www.dotnetperls.com/split
			String[] answer = Regex.Split(references, " ");
			// ^^^ https://www.dotnetperls.com/split

			return answer;
		}


			
		public void Debug()
		{

			log.Debug(Raw, "Raw");
			log.debug(ArticleNumber, "ArticleNumber");
			log.Debug(Subject, "Subject");
			log.Debug(From, "From");
			log.debug(Date.UtcTime, "Date.UtcTime");
			log.Debug(MessageId, "MessageId");
			for (int i = 0, count = References.Length; i < count; i++)
			{
				log.Debug(References[i], "References[" + i + "]");
			}

			log.debug(Bytes, "Bytes");
			log.debug(Lines, "Lines");
			//		log.debug( _trailing, "_trailing" );		

		}

	}
}
