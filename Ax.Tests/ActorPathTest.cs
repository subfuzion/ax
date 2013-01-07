// ========================================================================
// ActorPathTest.cs
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