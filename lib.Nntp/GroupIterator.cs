using System;
using System.Collections;

namespace lib.Nntp
{
	public class GroupIterator : IEnumerable, IEnumerator
	{
		LineIterator _lineIterator;

		public GroupIterator(LineIterator lineIterator)
		{
			_lineIterator = lineIterator;
		}

		public Group Current { get; private set; }


		#region IEnumerable

		public IEnumerator GetEnumerator()
		{
			return this;
		}

		#endregion

		#region IEnumerator

		Object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public bool MoveNext()
		{
			if (_lineIterator.HasNext())
			{

				Current = Group.BuildFromListResponse(_lineIterator.ReadLine());

				return true;
			}
			Current = null;
			return false;
		}

		public void Reset()
		{
			// vvv http://stackoverflow.com/questions/1468170/why-the-reset-method-on-enumerator-class-must-throw-a-notsupportedexception
			throw new NotSupportedException();
			// ^^^ http://stackoverflow.com/questions/1468170/why-the-reset-method-on-enumerator-class-must-throw-a-notsupportedexception
		}
		#endregion


	}
}
