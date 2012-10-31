// ========================================================================
// Props.cs
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