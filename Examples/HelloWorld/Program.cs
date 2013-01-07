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