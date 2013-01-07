// ========================================================================
// ActorPath.cs
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