// ========================================================================
// ActorContext.cs
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
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;

	public class Message
	{
		private readonly object[] _parameters;
		private readonly Type _resultType;
		private readonly ActorRef _sender;

		public Message(ActorRef sender, object[] parameters, Type resultType = null)
		{
			_sender = sender;
			_parameters = parameters;
			_resultType = resultType;
		}

		public ActorRef Sender
		{
			get { return _sender; }
		}

		public object[] Parameters
		{
			get { return _parameters; }
		}

		public Type ResultType
		{
			get { return _resultType; }
		}
	}

	public class ActorContext : ActorRefFactory
	{
		private readonly Message _message;
		private readonly BufferBlock<object> _replyBuffer;
		private readonly ActorRef _self;
		private readonly ActorSystem _system;

		/// <summary>
		/// Creates an ActorContext for an Actor (self) that is associated with
		/// an ActorSystem and the Actor that was the message sender. 
		/// </summary>
		/// <param name="system"></param>
		/// <param name="self"></param>
		/// <param name="message"> </param>
		internal ActorContext(ActorSystem system, ActorRef self, Message message)
		{
			if (system == null) throw new ArgumentNullException("system");
			if (self == null) throw new ArgumentNullException("self");
			if (message == null) throw new ArgumentNullException("message");

			_system = system;
			_self = self;
			_message = message;
			if (message.ResultType != null)
			{
				_replyBuffer = new BufferBlock<object>();
			}
		}

		public ActorSystem System

		{
			get { return _system; }
		}

		/// <summary>
		/// Gets the Actor receiver of the current message.
		/// </summary>
		public ActorRef Self
		{
			get { return _self; }
		}

		/// <summary>
		/// Gets the Actor sender of the current message.
		/// </summary>
		public ActorRef Sender
		{
			get { return _message.Sender; }
		}

		internal Message Message
		{
			get { return _message; }
		}

		public bool SenderAsked
		{
			get { return _message.ResultType != null; }
		}

		public override ActorRef ActorOf<T>()
		{
			return ActorOf(new Props(typeof (T)));
		}

		public override ActorRef ActorOf(Props props)
		{
			return ActorOf(props, null);
		}

		public override ActorRef ActorOf(Props props, string name)
		{
			var actorPath = Self.Path.AddChild(name);
			return System.ActorOf(props, name, actorPath);
		}

		/// <summary>
		/// Stops the specified Actor asynchronously.
		/// </summary>
		/// <param name="actor"></param>
		public override void Stop(ActorRef actor)
		{
			System.Stop(actor);
		}

		public void Reply(object message)
		{
			if (!SenderAsked) throw new AxException("Sender did not ask for a reply");
// ReSharper disable UseMethodIsInstanceOfType
			if (message != null && !(message is Exception) && !(Message.ResultType.IsAssignableFrom(message.GetType())))
// ReSharper restore UseMethodIsInstanceOfType
			{
				throw new AxException(
					string.Format("Result type ({0}) is not an instance of the type expected by the sender: {1}",
						message.GetType(),
						Message.ResultType));
			}

			if (_replyBuffer != null)
			{
				_replyBuffer.Post(message);
			}
			else
			{
				// todo: send to dead letter queue
			}
		}

		/// <summary>
		/// ActorRef waits on this method for a result if the sender invoked Ask instead of Tell
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		internal async Task<TResult> GetReplyAsync<TResult>()
		{
			var replyMessage = await _replyBuffer.ReceiveAsync();

			var exception = replyMessage as Exception;
			if (exception != null)
			{
				throw exception;
			}

			return (TResult)replyMessage;
		}
	}
}