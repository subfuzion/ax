// ========================================================================
// Actor.cs
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

namespace Ax
{
	using System;

	public abstract class Actor
	{
		private Guid _id = Guid.NewGuid();
		private string _name;

		public Guid ID
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name ?? ID.ToString(); }
			internal set { _name = value; }
		}

		/// <summary>
		/// Gets the current context, which is updated for each message that is dequeued.
		/// </summary>
		protected internal ActorContext Context { get; internal set; }

		protected ActorRef Sender
		{
			get { return Context.Sender; }
		}

		protected bool SenderAsked
		{
			get { return Context.SenderAsked; }
		}

		protected void Reply(object message)
		{
			Context.Reply(message);
		}

		public virtual void Receive(object message)
		{
			// todo: place message in dlq

			if (SenderAsked) Reply(null);
		}

		protected internal virtual void PreStart()
		{
		}

		protected internal virtual void PostRestart(Exception e)
		{
		}

		protected internal virtual void PostStop()
		{
		}
	}
}