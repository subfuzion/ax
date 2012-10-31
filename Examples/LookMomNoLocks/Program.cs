// ========================================================================
// Program.cs
// AX - Actor Extensions Framework for .NET
// http://axframework.io
// 
// Copyright (C) 2012, Subfuzion, Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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