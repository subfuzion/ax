// ========================================================================
// Program.cs
// AX - Actor Extensions Framework for .NET
// http://axframework.io
// 
// Copyright © 2012, 2013 Tony Pujals and Subfuzion, Inc.
// 
// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ========================================================================

namespace LookMomNoLocks
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading.Tasks;
	using Ax;

	public class CollectionActor : Actor
	{
		private readonly Lazy<Collection<string>> _internalCollection =
			new Lazy<Collection<string>>(() => new Collection<string>());

		protected override void PreStart()
		{
			Console.WriteLine("PreStart");
		}

		protected override void PostRestart(Exception e)
		{
			Console.WriteLine("PostRestart: " + e.Message);
		}

		public void Do()
		{
			Console.WriteLine("throwing exception");
			throw new Exception("testing");
		}

		public void Add(string item)
		{
			var id = _internalCollection.Value.Count;
			_internalCollection.Value.Add(string.Format("({0}) {1}", id, item));
		}

		public IEnumerable<string> GetCollection()
		{
			return _internalCollection.Value.AsEnumerable();
		}

		public int GetCount()
		{
			return _internalCollection.Value.Count;
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			var system = new ActorSystem("Demo");
			var collectionActor = system.ActorOf<CollectionActor>();

			const int taskCount = 5;
			const int itemsPerTask = 20;

			Parallel.For(0, taskCount, i =>
			{
				for (var j = 0; j < itemsPerTask; j++)
				{
					var itemCount = i*itemsPerTask + j;
					var item = string.Format("item{0}", itemCount);
					collectionActor.Tell(item);
				}
			});

			var countFuture = collectionActor.Ask<int>();

			var listFuture = collectionActor.Ask<IEnumerable<string>>();

			Task.WaitAll(countFuture, listFuture);

			Console.WriteLine("The collection contains {0} items", countFuture.Result);

			var list = listFuture.Result;

			foreach (var item in list)
			{
				Console.WriteLine("- {0}", item);
			}

			// will throw an exception, which causes the actor to be restarted (with an empty collection)
			collectionActor.Do();
			countFuture = collectionActor.Ask<int>();
			countFuture.Wait();
			Console.WriteLine("The collection now contains {0} items", countFuture.Result);

			Console.WriteLine("press any key to exit");
			Console.ReadKey();
		}
	}
}