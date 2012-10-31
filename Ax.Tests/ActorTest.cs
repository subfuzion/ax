// ========================================================================
// ActorTest.cs
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
	}
}