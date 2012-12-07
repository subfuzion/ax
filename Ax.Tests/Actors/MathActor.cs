﻿// ========================================================================
// MathActor.cs
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

namespace Ax.Tests.Actors
{
	using System;

	internal class MathResult
	{
		private readonly Guid _actorId;
		private readonly int _result;

		public MathResult(Guid actorId, int result)
		{
			_actorId = actorId;
			_result = result;
		}

		public Guid ActorID { get { return _actorId; } }
		public int Result { get { return _result; } }
	}

	internal class MathActor : Actor
	{
		public MathResult Divide(int dividend, int divisor)
		{
			try
			{
				var result = dividend / divisor;
				return new MathResult(ID, result);
			}
			catch (Exception e)
			{
				throw new Exception(ID.ToString(), e);
			}
		}
	}
}