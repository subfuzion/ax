// ========================================================================
// ActorPath.cs
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
	using System.Collections.Generic;
	using System.Linq;

	public class ActorPath
	{
		private readonly string _name;
		private readonly ActorPath _parent;

		public ActorPath(string name, ActorPath parent)
		{
			_name = name;
			_parent = parent;
		}

		public string Name
		{
			get { return _name; }
		}

		public ActorPath Parent
		{
			get { return _parent; }
		}

		public ActorPath Root
		{
			get
			{
				ActorPath element = this;
				while (element.Parent != null)
				{
					element = element.Parent;
				}
				return element;
			}
		}

		/// <summary>
		/// Gets sequence of names for this path from the root to this.
		/// </summary>
		public IEnumerable<string> Elements
		{
			get
			{
				var list = new List<string>();
				ActorPath element = this;
				while (element != null)
				{
					list.Add(element.Name);
					element = element.Parent;
				}
				list.Reverse();
				return list;
			}
		}

		public ActorPath AddChild(string name)
		{
			return new ActorPath(name, this);
		}

		/// <summary>
		/// Recursively create a descendant's paths by appending all child name elements.
		/// </summary>
		/// <param name="elements"></param>
		/// <returns></returns>
		public ActorPath AddDescendant(IEnumerable<string> elements)
		{
			ActorPath actorPath = this;

			actorPath = elements.Aggregate(actorPath, (current, element) => current.AddChild(element));

			return actorPath;
		}

		public ActorPath AddDescendant(string path)
		{
			return AddDescendant(path.Split('/'));
		}
	}
}