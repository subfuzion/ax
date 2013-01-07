// ========================================================================
// MethodCache.cs
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
		private readonly Dictionary<Signature, MethodInfo> _methods =
			new Dictionary<Signature, MethodInfo>(new SignatureComparer());

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

			try
			{
				result = methodInfo.Invoke(o, BindingFlags.Instance, null, parameters, null);
			}
			catch (TargetInvocationException e)
			{
				// if the actor raises an exception, we want to surface it to the caller (if expecting a result)
				throw e.InnerException;
			}

			return true;
		}
	}
}