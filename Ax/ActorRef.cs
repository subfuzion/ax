﻿// ========================================================================
// ActorRef.cs
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
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;
	using Helpers;

	public class PoisonPill : Message
	{
		public PoisonPill(ActorRef sender = null) : base(sender, null, null)
		{
		}
	}

	public class ActorRef
	{
		private static readonly Lazy<MethodCache> _methodCache = new Lazy<MethodCache>(() => new MethodCache());
		private readonly string _name;
		private readonly ActorPath _path;
		private readonly Props _props;
		private readonly ActorSystem _system;
		private Actor _actor;
		private bool _isTerminated;

		private BufferBlock<ActorContext> _messageBuffer;

		internal ActorRef(string name, ActorPath path, ActorSystem system, Props props)
		{
			_name = name;
			_path = path;
			_system = system;
			_props = props;
			CreateMessageBuffer();
		}

		public ActorPath Path
		{
			get { return _path; }
		}

		internal string Name
		{
			get { return _name; }
		}

		internal Props Props
		{
			get { return _props; }
		}

		/// <summary>
		/// True if the Actor has been stopped. Once true, it will never be false again.
		/// </summary>
		public bool IsTerminated
		{
			get { return _isTerminated; }
			internal set
			{
				if (_isTerminated && !value)
				{
					throw new InvalidOperationException("The Actor was already stopped");
				}
				_isTerminated = value;
			}
		}

		private void CreateMessageBuffer()
		{
			var messageBuffer = new BufferBlock<ActorContext>();

			// later, ensure options are specified for bounding the message queue,
			// cancellation token, even different task schedulers
			// http://msdn.microsoft.com/en-us/library/system.threading.tasks.dataflow.dataflowblockoptions.aspx

			_messageBuffer = messageBuffer;
		}

		internal void AttachActor(Actor actor, Exception e)
		{
			_actor = actor;

			_actor.PreStart();

			if (e != null)
				_actor.PostRestart(e);
			else
			{
				// todo: add support for dispatchers
				// if we want a threaded dispatcher...
				//Task.Factory.StartNew(ReceiveAsync);

				// non-threaded
				ReceiveAsync();
			}
		}

		private async void ReceiveAsync()
		{
			// todo: stop not implemented yet, so quit will never be true
			bool quit = false;
			while (!quit)
			{
				// await does not block the caller thread. The compiler registers the
				// rest of this method as a continuation of the awaited task;
				// meanwhile, control returns to the caller of this async method.
				var actorContext = await _messageBuffer.ReceiveAsync();

				var message = actorContext.Message;

				_actor.Context = actorContext;

				// TODO - actor should gracefully terminate after receiving a poison pill
				if (message is PoisonPill) break;

				try
				{
					object result;
					if (_methodCache.Value.InvokeMethod(_actor, message.Parameters, message.ResultType, out result))
					{
						if (_actor.Context.SenderAsked)
							_actor.Context.Reply(result);
					}
					else
					{
						if (message.Parameters.Length != 1)
							throw new AxException("The Actor.Receive method expects a single object for a message");
						_actor.Receive(message.Parameters[0]);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("[System] Actor crashed, creating new instance ({0})", e.Message);

					if (_actor.Context.SenderAsked)
					{
						_actor.Context.Reply(e);
					}

					_system.RestartActor(this, e);
				}
			}
		}

		public void Do()
		{
			Tell(null, typeof (void));
		}

		/// <summary>
		/// Posts a message to the Actor.
		/// </summary>
		/// <param name="arg"> </param>
		/// <param name="messageArgs"> </param>
		public void Tell(params object[] messageArgs)
		{
			Tell(null, messageArgs);
		}

		/// <summary>
		/// Posts a message to the Actor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="messageArgs"> </param>
		public void Tell(ActorRef sender, params object[] messageArgs)
		{
			var message = new Message(sender, messageArgs);
			var actorContext = new ActorContext(_system, this, message);
			_messageBuffer.Post(actorContext);
		}

		/// <summary>
		/// Posts a message to the Actor and returns a future.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public async Task<TResult> Ask<TResult>(params object[] args)
		{
			return await Ask<TResult>(null, args);
		}

		/// <summary>
		/// Posts a message to the Actor and returns a future.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="messageArgs"></param>
		/// <returns></returns>
		public async Task<TResult> Ask<TResult>(ActorRef sender, params object[] messageArgs)
		{
			var message = new Message(sender, messageArgs, typeof (TResult));
			var actorContext = new ActorContext(_system, this, message);
			_messageBuffer.Post(actorContext);
			return await actorContext.GetReplyAsync<TResult>();
		}
	}
}