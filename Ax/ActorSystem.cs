// ========================================================================
// ActorSystem.cs
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