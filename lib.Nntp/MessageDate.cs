using System;
using System.Globalization;
using dotnet.lib.CoreAnnex.exception;
namespace lib.Nntp
{
	public class MessageDate
	{

		static readonly String[] FORMATS = new String[]{"R", // RFC1123 format: "EEE',' dd MMM yyyy hh:mm:ss Z"
			"dd MMM yyyy hh:mm:ss Z"};


		public long UtcTime { get; private set;}
		//public String TimeZone { get; private set; }


		public MessageDate(String value)
		{
			DateTime result;


			if (!DateTime.TryParseExact(value, FORMATS, CultureInfo.CurrentCulture,
										System.Globalization.DateTimeStyles.None, out result))
			{
				throw new BaseException(typeof(MessageDate), "invalid date {0}", value);
			}

			UtcTime = result.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond;	
		}

	}
}

