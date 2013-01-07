// ========================================================================
// Props.cs
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

	public class Props
	{
		private readonly Func<Actor> _creator;

		/// <summary>
		/// Creates a new Props with a default creator function that always returns a new Actor
		/// instance of the specified type.
		/// </summary>
		/// <param name="actorType"></param>
		public Props(Type actorType)
		{
			_creator = () => (Actor) Activator.CreateInstance(actorType);
		}

		/// <summary>
		/// Creates a new Props with a creator function that always returns a new Actor instance.
		/// </summary>
		/// <param name="creator"></param>
		public Props(Func<Actor> creator)
		{
			_creator = creator;
		}

		public Func<Actor> Creator
		{
			get { return _creator; }
		}

		/// <summary>
		/// Creates a new Props with a creator function that always returns a new Actor instance.
		/// </summary>
		/// <param name="creator"></param>
		/// <returns></returns>
		public static Props WithCreator(Func<Actor> creator)
		{
			return new Props(creator);
		}
	}
}