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

namespace MessagesHandling
{
	using System;
	using Ax;

	public class MessageDemoActor : Actor
	{
		public void Greet(string message)
		{
			Console.WriteLine("Hello, " + message);
		}

		public void Square(int x)
		{
			Console.WriteLine("The square of {0} is {1}", x, x*x);
		}

		public int Sum(int a, int b)
		{
			return a + b;
		}

		// You can always override the default Receive method.
		// You can use it in place of specifically typed methods
		// (such as Greet and Square shown above), or you can provide
		// it as a catch-all method to handle any other unexpected
		// messages, if you wish. If not provided, unhandled messages
		// will be forwarded to the Dead Letter Queue (DLQ) and logged.
		public override void Receive(object message)
		{
			Console.WriteLine("Don't know what to do with this: {0}", message);

			// base Actor will forward to the dead letter queue
			base.Receive(message);
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			var system = new ActorSystem("DemoSystem");

			var actorRef = system.ActorOf<MessageDemoActor>();

			actorRef.Tell("world");

			actorRef.Tell(5);

			actorRef.Tell(5.0);

			var future = actorRef.Ask<int>(2, 3)
			                     .ContinueWith(task =>
				                     Console.WriteLine("The sum of 2 + 3 is " + task.Result));

			future.Wait();

			Console.WriteLine("press any key to exit");
			Console.ReadKey();
		}
	}
}