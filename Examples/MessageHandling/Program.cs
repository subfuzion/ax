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