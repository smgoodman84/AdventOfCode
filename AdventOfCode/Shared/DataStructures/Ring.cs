using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.DataStructures
{
	public class Ring<T> : Queue<T>
	{
		public int Index { get; private set; }

		public Ring(IEnumerable<T> collection)
			: base(collection)
		{

		}

		public T CurrentThenMoveNext()
		{
			var element = Dequeue();
            Enqueue(element);

			Index += 1;
			Index %= Count;

            return element;
		}
    }
}

