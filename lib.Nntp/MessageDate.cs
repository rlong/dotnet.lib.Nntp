using System;
using System.Globalization;
using dotnet.lib.CoreAnnex.exception;
namespace lib.Nntp
{
	public class MessageDate
	{

		public static readonly String DATE_FORMAT_1 = "R"; // RFC1123 format: "EEE',' dd MMM yyyy hh:mm:ss Z"
		public static readonly String DATE_FORMAT_2 = "dd MMM yyyy hh:mm:ss Z";
		static readonly String[] FORMATS = new String[] { DATE_FORMAT_1, DATE_FORMAT_2 };

		public long UtcTime { get; private set; }
		public String TimeZone { get; private set; }


		public MessageDate(String value)
		{
			DateTime result;


			if (!DateTime.TryParseExact(value, FORMATS, CultureInfo.CurrentCulture,
										System.Globalization.DateTimeStyles.None, out result))
			{
				throw new BaseException(typeof(MessageDate), "invalid date {0}", value);
			}

			UtcTime = result.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond;

			var indexOfLastSpace = value.LastIndexOf(' ');
			TimeZone = value.Substring(indexOfLastSpace + 1); // '1 +' to skip over the ' '
		}

		public static int GetLocalTimezoneOffset()
		{
			// vvv https://www.dotnetperls.com/timezone
			var currentTimeZone = System.TimeZone.CurrentTimeZone;
			var timeSpan = currentTimeZone.GetUtcOffset(DateTime.Now);
			return timeSpan.Milliseconds;
			// ^^^ https://www.dotnetperls.com/timezone
		}
	}
}

