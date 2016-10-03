
using NUnit.Framework;
using System;
using lib.CoreAnnex.log;

namespace lib.Nntp
{

	[TestFixture]
	public class CrossReferenceUnitTest
	{
		private static Log log = Log.getLog(typeof(CrossReferenceUnitTest));
		public CrossReferenceUnitTest()
		{
		}


		[Test]
		public void test1()
		{
			log.debug("test1");
		}


		[Test]
		public void TestBuild()
		{
			CrossReference xref = CrossReference.Build("Xref: news.gmane.org gmane.comp.python.committers:2800");
			Assert.NotNull(xref);
			log.Debug(xref.ToJsonArray().ToString(), "xref.ToJsonArray().ToString()");
		}

	}
}
