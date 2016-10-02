using System;
using System.Text.RegularExpressions;
using dotnet.lib.CoreAnnex.json;
using lib.CoreAnnex.log;

namespace lib.Nntp
{
	public class CrossReference
	{

		private static Log log = Log.getLog(typeof(CrossReference));

		public class Location
		{
			public String NewsgroupName { get; private set; }
			public String ArticleLocator { get; private set; }


			private Location(String newsgroupName, String articleLocator)
			{
				NewsgroupName = newsgroupName;
				ArticleLocator = articleLocator;
			}


			// can return null;
			public static Location Build(String value)
			{
				String[] elements = value.Split(':');
				if (elements.Length != 2)
				{
					log.warnFormat("elements.Length != 2; value = '{0}'", value);
					return null;
				}
				return new Location(elements[0], elements[1]);
			}

			public JsonArray ToJsonArray()
			{
				JsonArray answer = new JsonArray();
				answer.Add(NewsgroupName);
				answer.Add(ArticleLocator);
				return answer;
			}
		}

		public Location[] Locations { get; private set; }

		public String ServerName { get; private set; }

		public CrossReference()
		{
		}

		// can return null;
		public static CrossReference Build(String value)
		{
			value = value.Trim();

			// empty string
			if (0 == value.Length)
			{
				return null;
			}

			// vvv http://tools.ietf.org/search/rfc5536#section-3.2.14

			String xref = "Xref: ";
			if (0 != value.IndexOf(xref, StringComparison.CurrentCulture))
			{
				log.warnFormat("0 != value.IndexOf(xref, StringComparison.CurrentCulture); value = '{0}'", value);
				return null;
			}

			String xrefValue = value.Substring(xref.Length);

			// vvv https://www.dotnetperls.com/split
			String[] xrefComponents = Regex.Split(xrefValue, " ");
			// ^^^ https://www.dotnetperls.com/split


			if (2 > xrefComponents.Length)
			{
				log.warnFormat("2 > xrefComponents.Length; value = '{0}'", value);
				return null;
			}

			CrossReference answer = new CrossReference();
			answer.ServerName = xrefComponents[0];
			answer.Locations = new Location[xrefComponents.Length - 1];
			for (int i = 1, count = xrefComponents.Length; i < count; i++)
			{
				Location location = Location.Build(xrefComponents[i]);
				if (null == location)
				{
					return null;
				}
				answer.Locations[i - 1] = location;

			}

			// ^^^ http://tools.ietf.org/search/rfc5536#section-3.2.14

			return answer;
		}

		public JsonArray ToJsonArray()
		{
			JsonArray answer = new JsonArray();
			answer.Add(ServerName);
			for (int i = 0, count = Locations.Length; i < count; i++)
			{
				answer.Add(Locations[i].ToJsonArray());
			}
			return answer;
		}
	}
}
