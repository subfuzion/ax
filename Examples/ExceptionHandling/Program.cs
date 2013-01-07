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

namespace ExceptionHandling
{
	using System;
	using System.Threading.Tasks;
	using Ax;

	internal class ErrorProneActor : Actor
	{
		protected override void PostRestart(Exception e)
		{
			Console.WriteLine("[Actor] System restarted Actor after crash ({0})", e.Message);
		}

		public int Divide(int dividend, int divisor)
		{
			// intentional no divide by zero check...
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
				Tuple.Create(100, 10, 2),
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
				var future = actorRef.Ask<int>(sample.Item1, sample.Item2);

				try
				{
					future.Wait();
					Console.WriteLine("(Sample #{0}) {1} / {2} = {3}", sample.Item3, sample.Item1, sample.Item2, future.Result);
				}
				catch (AggregateException e)
				{
					Console.WriteLine("(Sample #{0}) {1} / {2} = <<< actor crashed >>> ({3})", sample.Item3, sample.Item1, sample.Item2,
						e.Flatten().InnerException.Message);
				}
			});

			Console.WriteLine("press any key to exit");
			Console.ReadKey();
		}
	}
}