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

namespace ExceptionHandling
{
	using System;
	using System.Threading.Tasks;
	using Ax;

	class ErrorProneActor : Actor
	{
		protected override void PostRestart(Exception e)
		{
			Console.WriteLine("[Actor] System restarted Actor after crash ({0})", e.Message);
		}

		public int Divide(int dividend, int divisor)
		{
			// No divide by zero check...
			return dividend/divisor;
		}
	}


	internal class Program
	{
		private static void Main(string[] args)
		{
			var system = new ActorSystem("ExceptionHandlingDemo");
			var actorRef = system.ActorOf<ErrorProneActor>();

			var samples = new[]
			{
				Tuple.Create(25, 5, 1),
				Tuple.Create(100,10, 2),
				Tuple.Create(10, 0, 3), // should trigger an exception, but won't affect message queue
				Tuple.Create(20, 4, 4),
				Tuple.Create(9, 3, 5),
				Tuple.Create(16, 4, 6),
				Tuple.Create(8, 2, 7),
				Tuple.Create(7, 0, 8), // should trigger an exception, but won't affect message queue
				Tuple.Create(30, 5, 9),
				Tuple.Create(64, 8, 10),
			};

			Parallel.ForEach(samples, sample =>
			{
				try
				{
					var future = actorRef.Ask<int>(sample.Item1, sample.Item2);
					future.Wait();
					Console.WriteLine("(Sample #{0}) {1} / {2} = {3}", sample.Item3, sample.Item1, sample.Item2, future.Result);
				}
				catch (AggregateException e)
				{
					Console.WriteLine("(Sample #{0}) {1} / {2} = <<< actor crashed >>> ({3})", sample.Item3, sample.Item1, sample.Item2, e.Flatten().InnerException.Message);
				}
			});

			Console.WriteLine("press any key to exit");
			Console.ReadKey();
		}
	}
}