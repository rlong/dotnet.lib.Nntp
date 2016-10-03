using System;
using NUnit.Framework;
namespace lib.Nntp
{

	[TestFixture]
	public class MessageUnitTest
	{
		public MessageUnitTest()
		{
		}

		[Test]
		public void TestBuildReferences1()
		{

			String[] references = Message.BuildReferences("a b c");
			Assert.Equals(3, references.Length);

		}


		[Test]
		public void TestBuildReferences2()
		{

			String[] references = Message.BuildReferences("a  b  c");
			Assert.Equals(3, references.Length);
		}
	}
}
