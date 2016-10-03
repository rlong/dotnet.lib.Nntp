using lib.CoreAnnex.log;
using NUnit.Framework;
using System;
using System.Globalization;

namespace lib.Nntp
{
	[TestFixture]
	public class MessageDateUnitTest
	{
		private static Log log = Log.getLog(typeof(MessageDateUnitTest));

		public MessageDateUnitTest()
		{
		}

		[Test]
		public void TestDateFormat1()
		{

			DateTime result;

			Assert.True(DateTime.TryParseExact("Tue, 26 Apr 2011 00:20:40 +0200", MessageDate.DATE_FORMAT_1, CultureInfo.CurrentCulture,
												 System.Globalization.DateTimeStyles.None, out result));


			log.debug(result.TimeOfDay, "result.TimeOfDay");
			log.debug(result, "result");

			String formattedDate = result.ToString(MessageDate.DATE_FORMAT_1);
			log.Debug(formattedDate, "formattedDate");
		}


		[Test]
		public void TestDateFormat2()
		{

			DateTime result;

			Assert.True(DateTime.TryParseExact("28 Dec 2013 09:02:23 GMT", MessageDate.DATE_FORMAT_2, CultureInfo.CurrentCulture,
												 System.Globalization.DateTimeStyles.None, out result));


			log.debug(result.TimeOfDay, "result.TimeOfDay");
			log.debug(result, "result");

			String formattedDate = result.ToString(MessageDate.DATE_FORMAT_1);
			log.Debug(formattedDate, "formattedDate");
		}


		[Test]
		public void TestConstructor()
		{

			{
				var md = new MessageDate("Tue, 26 Apr 2011 00:20:40 +0200");
				log.debug(md.UtcTime, "md.UtcTime");
				log.Debug(md.TimeZone, "md.TimeZone");

				var dt = new DateTime(md.UtcTime * TimeSpan.TicksPerMillisecond);
				log.debug(dt, "dt");
			}

			{
				var md = new MessageDate("28 Dec 2013 09:02:23 GMT");
				log.debug(md.UtcTime, "md.UtcTime");
				log.Debug(md.TimeZone, "md.TimeZone");

				var dt = new DateTime(md.UtcTime * TimeSpan.TicksPerMillisecond);
				log.debug(dt, "dt");
			}
		}

		[Test]
		public void TestGetLocalTimezoneOffset()
		{
			log.debug(MessageDate.GetLocalTimezoneOffset(), "MessageDate.GetLocalTimezoneOffset()");
		}
	}
}
