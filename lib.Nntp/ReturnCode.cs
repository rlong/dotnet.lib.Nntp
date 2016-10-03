using System;
namespace lib.Nntp
{
	public class ReturnCode
	{

		public static readonly String NO_SUCH_GROUP_411 = "nntp_client.ReturnCode.NO_SUCH_GROUP_411";
		public static readonly String NO_SUCH_ARTICLE_430 = "nntp_client.ReturnCode.NO_SUCH_ARTICLE_430";
		public static readonly String NO_SUCH_ARTICLE_IN_GROUP_423 = "nntp_client.ReturnCode.NO_SUCH_ARTICLE_IN_GROUP_423";

		public ReturnCode()
		{
		}

		public static String Lookup(int code, String defaultValue)
		{

			if (411 == code)
			{
				return NO_SUCH_GROUP_411;
			}

			if (423 == code)
			{
				return NO_SUCH_ARTICLE_IN_GROUP_423;
			}

			if (430 == code)
			{
				return NO_SUCH_ARTICLE_430;
			}

			return defaultValue;

		}

	}
}
