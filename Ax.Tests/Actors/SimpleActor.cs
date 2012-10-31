// ========================================================================
// SimpleActor.cs
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

namespace Ax.Tests.Actors
{
	using System.Threading;

	public class SimpleMessage
	{
		private readonly string _message;

		// since we're testing a one way message, we'll signal
		// an EventWaitHandle to indicate that we sucessfully received it
		private readonly string _signalName;

		public SimpleMessage(string message, string signalName)
		{
			_message = message;
			_signalName = signalName;
		}

		public string Message
		{
			get { return _message; }
		}

		public string SignalName
		{
			get { return _signalName; }
		}
	}

	public class SimpleActor : Actor
	{
		// In this contrived example, the caller sent the actor a one-way
		// message (using Tell instead of Ask) to perform some work.
		// For unit testing, we'll indicate the message was received
		// by signaling an event waithandle created in the unit test.
		public void ProcessMessage(SimpleMessage message)
		{
			if (message.Message == "hello")
			{
				// signals the caller that we've processed the message
				var signal = EventWaitHandle.OpenExisting(message.SignalName);
				signal.Set();
			}
		}
	}
}