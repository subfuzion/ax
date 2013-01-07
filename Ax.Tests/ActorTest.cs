﻿// ========================================================================
// ActorTest.cs
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

namespace Ax.Tests
{
	using System;
	using System.Threading;
	using Actors;
	using NUnit.Framework;

	[TestFixture]
	public class ActorTest
	{
		[Test]
		public void GreetingActorTestAsk()
		{
			var system = new ActorSystem("test");

			var actorRef = system.ActorOf<GreetingActor>();

			var future1 = actorRef.Ask<string>("world");
			var future2 = actorRef.Ask<string>("cruel world");

			future1.Wait();
			Assert.AreEqual("Hello, world", future1.Result);

			future2.Wait();
			Assert.AreEqual("Hello, cruel world", future2.Result);
		}

		[Test]
		public void SimpleActorTestTestTell()
		{
			const string signalName = "SimpleActor.Test";
			var signal = new EventWaitHandle(false, EventResetMode.AutoReset, signalName);

			var system = new ActorSystem("test");

			var actorRef = system.ActorOf<SimpleActor>();

			var message = new SimpleMessage("hello", signalName);

			actorRef.Tell(message);

			if (!signal.WaitOne(TimeSpan.FromSeconds(5)))
			{
				Assert.Fail("timed out waiting for actor to process message");
			}
		}

		[Test]
		public void WhenActorCrashes_ThenSystemCreatesNewActor()
		{
			var system = new ActorSystem("test");
			var actorRef = system.ActorOf<MathActor>();

			// send division problems to actor to compute
			var future1 = actorRef.Ask<MathResult>(9, 3);
			var future2 = actorRef.Ask<MathResult>(9, 0);
			var future3 = actorRef.Ask<MathResult>(10, 2);

			// first division - should pass
			future1.Wait();
			var mathResult = future1.Result;

			var actorID1 = mathResult.ActorID;
			Assert.AreEqual(9/3, mathResult.Result);

			Guid actorID2 = Guid.Empty;
			try
			{
				// second division - should fail
				future2.Wait();
				Assert.Fail("shouldn't get here, expected an exception");
			}
			catch (AggregateException e)
			{
				var ex = e.Flatten().InnerException;
				actorID2 = Guid.Parse(ex.Message);
				Assert.IsTrue(ex.InnerException is DivideByZeroException);
				//Assert.IsNotNull(e.Flatten().InnerExceptions.FirstOrDefault(ex => ex.InnerException.GetType() == typeof(DivideByZeroException)));
			}

			// third division - should pass
			future3.Wait();
			mathResult = future3.Result;

			var actorID3 = mathResult.ActorID;
			Assert.AreEqual(10/2, mathResult.Result);

			// verify that the actorRef dispatched messages in the queue to a new instance of
			// an actor after the first one crashed
			Assert.AreEqual(actorID1, actorID2);
			Assert.AreNotEqual(actorID2, actorID3);
		}
	}
}