﻿// ========================================================================
// ReflectionHelper.cs
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
	using System.Linq;
	using System.Reflection;

	internal static class ReflectionHelper
	{
		public static Type[] GetTypes(this object[] args)
		{
			return args.Select(arg => arg != null ? arg.GetType() : typeof (object)).ToArray();
		}

		public static MethodInfo GetMethodWithSignature(this Type type, BindingFlags bindingFlags, Type returnType,
			params Type[] parameterTypes)
		{
			if (returnType == null) returnType = typeof (void);

			return type.GetMethods(bindingFlags).FirstOrDefault(m =>
				m.ReturnType == returnType &&
					m.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes));
		}

		public static MethodInfo GetInstanceMethodWithSignature(this Type type, Type returnType, params Type[] parameterTypes)
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
			return GetMethodWithSignature(type, bindingFlags,
				returnType, parameterTypes);
		}

		public static MethodInfo GetInstanceMethodWithSignature(this object o, Type returnType, params Type[] parameterTypes)
		{
			return GetInstanceMethodWithSignature(o.GetType(), returnType, parameterTypes);
		}

		public static MethodInfo GetMethodForParameters(this object o, Type returnType, params object[] args)
		{
			return GetInstanceMethodWithSignature(o, returnType, args.GetTypes());
		}

		public static MethodInfo GetMethodForParameters(this object o, params object[] args)
		{
			return GetMethodForParameters(o, typeof (void), args);
		}
	}
}