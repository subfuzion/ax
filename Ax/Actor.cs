// ========================================================================
// Actor.cs
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

namespace Ax
{
	using System;

	public abstract class Actor
	{
		public string Name { get; internal set; }

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

		internal protected virtual void PreStart()
		{
		}

		internal protected virtual void PostRestart(Exception e)
		{
		}

		internal protected virtual void PostStop()
		{
		}
	}
}