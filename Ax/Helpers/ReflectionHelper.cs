// ========================================================================
// ReflectionHelper.cs
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
	using System.Linq;
	using System.Reflection;

	internal static class ReflectionHelper
	{
		public static Type[] GetTypes(this object[] args)
		{
			return args.Select(arg => arg != null ? arg.GetType() : typeof(object)).ToArray();
		}

		public static MethodInfo GetMethodWithSignature(this Type type, BindingFlags bindingFlags, Type returnType,
			params Type[] parameterTypes)
		{
			if (returnType == null) returnType = typeof(void);

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
			return GetMethodForParameters(o, typeof(void), args);
		}
	}
}