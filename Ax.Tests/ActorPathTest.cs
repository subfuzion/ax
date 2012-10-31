// ========================================================================
// ActorPathTest.cs
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

namespace Ax.Tests
{
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public class ActorPathTest
	{
		[Test]
		public void WhenAddChild_ThenNewActorRefPathIncludesCurrentPlusChild()
		{
			var actorPath = new ActorPath("root", null);
			ActorPath childPath = actorPath.AddChild("child");
			string[] elements = childPath.Elements.ToArray();
			Assert.AreEqual("root", elements[0]);
			Assert.AreEqual("child", elements[1]);
		}

		[Test]
		public void WhenAddDescendant_ThenNewActorRefPathIncludesCurrentPlusAllDescendants()
		{
			var actorPath = new ActorPath("root", null);
			ActorPath childPath1 = actorPath.AddChild("child1");
			ActorPath childPath2 = childPath1.AddChild("child2");
			string[] elements = childPath2.Elements.ToArray();
			Assert.AreEqual("root", elements[0]);
			Assert.AreEqual("child1", elements[1]);
			Assert.AreEqual("child2", elements[2]);
		}

		[Test]
		public void WhenAddDescendantsWithPathString_ThenNewActorRefPathIncludesCurrentPlusChild()
		{
			var actorPath = new ActorPath("root", null);
			ActorPath childPath = actorPath.AddDescendant("child1/child2/child3");
			string[] elements = childPath.Elements.ToArray();
			Assert.AreEqual("root", elements[0]);
			Assert.AreEqual("child1", elements[1]);
			Assert.AreEqual("child2", elements[2]);
			Assert.AreEqual(actorPath, childPath.Root);
		}
	}
}