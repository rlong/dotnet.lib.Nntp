using System;
using lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.json;
using dotnet.lib.CoreAnnex.auxiliary;

namespace lib.Nntp
{
	public class Group
	{

		private static Log log = Log.getLog(typeof(Group));

		public String Name { get; private set; }
		public long High { get; private set; }
		public long Low { get; private set; }
		public long? Count { get; private set; }

		public Group()
		{
			Count = null;
		}

		public void Debug()
		{
			log.debugFormat("{0}[{1}-{2}]", Name, Low, High);
		}

		public JsonObject ToJsonObject()
		{
			JsonObject answer = new JsonObject();

			answer.put("name", Name);
			answer.put("low", Low);
			answer.put("high", High);

			if (null != Count)
			{
				answer.put("count", Count);
			}

			return answer;
		}

		public static Group BuildFromListResponse(String listResponse)
		{
			var tokens = listResponse.Split(' ');
			Group answer = new Group();

			answer.Name = tokens[0];
			answer.High = NumericUtilities.ParseLong(tokens[1]);
			answer.Low = NumericUtilities.ParseLong(tokens[2]);
			answer.Count = null;

			return answer;
		}

		public static Group BuildFromGroupResponse(String groupResponse)
		{
			//211 1639 1 1639 gmane.comp.python.committers
			var tokens = groupResponse.Split(' ');
			Group answer = new Group();

			// tokens[0]: response code
			answer.Count = NumericUtilities.ParseLong(tokens[1]);
			answer.Low = NumericUtilities.ParseLong(tokens[2]);
			answer.High = NumericUtilities.ParseLong(tokens[3]);
			answer.Name = tokens[4];

			return answer;
		}
	}
}
