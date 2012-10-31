// ========================================================================
// MethodCache.cs
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

namespace Ax.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	internal class ArrayComparer : IEqualityComparer<Type[]>
	{
		public bool Equals(Type[] a, Type[] b)
		{
			// todo: might not want to use LINQ SequenceEqual for performance
			return a.SequenceEqual(b);
		}

		public int GetHashCode(Type[] types)
		{
			return types.Aggregate(0, (current, type) => current ^ (type != null ? type.GetHashCode() : 0));
		}
	}

	internal class Signature
	{
		public Type[] ParameterTypes { get; set; }
		public Type ReturnType { get; set; }
	}

	internal class SignatureComparer : IEqualityComparer<Signature>
	{
		private readonly ArrayComparer _arrayComparer = new ArrayComparer();

		public bool Equals(Signature a, Signature b)
		{
			return _arrayComparer.Equals(a.ParameterTypes, b.ParameterTypes) && a.ReturnType == b.ReturnType;
		}

		public int GetHashCode(Signature signature)
		{
			var hashCode = _arrayComparer.GetHashCode(signature.ParameterTypes);
			return signature.ReturnType != null ? hashCode ^ signature.ReturnType.GetHashCode() : hashCode;
		}
	}

	internal class MethodCache
	{
		// This cache is specific to an individual ActorRef (and thus is thread-safe
		// since queries are serialized). May want to consider a more global cache
		// associated with the type that is being queried for a matching method.
		// It doesn't really need to be thread safe anyway (it should be okay if
		// the value is cached more than once). Should probably be weak references
		// in that case.
		private readonly Dictionary<Signature, MethodInfo> _methods = new Dictionary<Signature, MethodInfo>(new SignatureComparer());

		public bool InvokeMethod(object o, object[] args, Type returnType, out object result)
		{
			result = null;

			var parameters = args.Length == 1 && args[0] as Type == typeof (void)
				? new object[] {}
				: args;

			var signature = new Signature
			{
				ParameterTypes = parameters.GetTypes(),
				ReturnType = returnType,
			};

			MethodInfo methodInfo;
			if (!_methods.TryGetValue(signature, out methodInfo))
			{
				methodInfo = o.GetInstanceMethodWithSignature(signature.ReturnType, signature.ParameterTypes);
				if (methodInfo != null)
					_methods.Add(signature, methodInfo);
			}

			if (methodInfo == null) return false;

			result = methodInfo.Invoke(o, BindingFlags.Instance, null, parameters, null);
			return true;
		}
	}
}