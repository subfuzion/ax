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

namespace HelloWorld
{
	using System;
	using Ax;

	public class GreetingActor : Actor
	{
		public void Greet()
		{
			Console.WriteLine("Hello, anonymous");
		}

		public void Greet(string message)
		{
			Console.WriteLine("Hello, " + message);
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			var system = new ActorSystem("example");

			var actorRef = system.ActorOf<GreetingActor>();

			actorRef.Do();

			actorRef.Tell("world");

			actorRef.Tell("universe");

			// Tell is non-blocking, so you will most likely see the following
			// prompt before you see the output from the GreetingActor
			Console.WriteLine("press any key to exit");
			Console.ReadKey();
		}
	}
}