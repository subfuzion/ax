// ========================================================================
// ActorSystem.cs
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

	public class ActorSystem : ActorRefFactory
	{
		public readonly string _name;

		public ActorSystem(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public override ActorRef ActorOf<T>()
		{
			return ActorOf(new Props(typeof (T)));
		}

		public override ActorRef ActorOf(Props props)
		{
			Guid uid = Guid.NewGuid();
			return ActorOf(props, uid.ToString());
		}

		public override ActorRef ActorOf(Props props, string name)
		{
			// todo: create system actor for parent
			return ActorOf(props, name, new ActorPath("/", null));
		}

		internal ActorRef ActorOf(Props props, string name, ActorPath actorPath, ActorRef actorRef = null, Exception e = null)
		{
			if (props == null) throw new ArgumentNullException("props");
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");
			// todo: better validity check, including valid path
			if (actorPath == null) throw new ArgumentNullException("actorPath");

			Func<Actor> creator = props.Creator;

			Actor actor = creator();

			actor.Name = name;

			if (actorRef == null)
				actorRef = new ActorRef(name, actorPath, this, props);

			actorRef.AttachActor(actor, e);

			return actorRef;
		}

		internal void RestartActor(ActorRef actorRef, Exception e)
		{
			ActorOf(actorRef.Props, actorRef.Name, actorRef.Path, actorRef, e);
		}

		public override void Stop(ActorRef actor)
		{
			throw new NotImplementedException();
		}
	}
}