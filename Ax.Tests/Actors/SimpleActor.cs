﻿// ========================================================================
// SimpleActor.cs
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