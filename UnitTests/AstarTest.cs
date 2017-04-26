using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SemanticsSearchPerformanceTest;

namespace UnitTests
{
	[TestClass]
	public class AstarTest
	{
		[TestMethod]
		public void TestRegularPlain()
		{
			validatePlain(new AstarRegular(), createKnowledgeBase(false));
		}

		[TestMethod]
		public void TestCachedPlain()
		{
			validatePlain(new AstarCache(), createKnowledgeBase(false));
		}
		[TestMethod]
		public void TestRegularLoop()
		{
			validateLoop(new AstarRegular(), createKnowledgeBase(true));
		}

		[TestMethod]
		public void TestCachedLoop()
		{
			validateLoop(new AstarCache(), createKnowledgeBase(true));
		}

		private static readonly Node a = new Node(0),
									 b = new Node(1),
									 c = new Node(2),
									 d = new Node(3);
		private static readonly Arc ab = new Arc(a, b, 4),
									ac = new Arc(a, c, 6),
									ad = new Arc(a, d, 10),
									bd = new Arc(b, d, 6),
									cd = new Arc(c, d, 4),
									bc = new Arc(b, c, 1),
									db = new Arc(d, b, 1);

		private static KnowledgeBase createKnowledgeBase(bool withLoop)
		{
			return new KnowledgeBase(
				new List<Node> { a, b, c, d },
				withLoop ? new List<Arc> { ab, ac, ad, bd, cd, bc, db } : new List<Arc> { ab, ac, ad, bd, cd, bc });
		}

		private static void validatePlain(SearchProcedure method, KnowledgeBase knowledgeBase)
		{
			Assert.AreEqual(0, method.Search(knowledgeBase, a, a).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, b, b).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, c, c).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, d, d).Count());

			Assert.AreEqual(0, method.Search(knowledgeBase, b, a).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, c, a).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, c, b).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, d, a).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, d, b).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, d, c).Count());

			Assert.AreEqual(4, method.Search(knowledgeBase, a, b).Single().ComputingLongevity);
			Assert.AreEqual(1, method.Search(knowledgeBase, b, c).Single().ComputingLongevity);
			Assert.AreEqual(4, method.Search(knowledgeBase, c, d).Single().ComputingLongevity);
			var paths = method.Search(knowledgeBase, a, c).ToList();
			Assert.AreEqual(2, paths.Count);
			Assert.AreEqual(5, paths.Min(path => path.ComputingLongevity));
			Assert.AreEqual(6, paths.Max(path => path.ComputingLongevity));
			paths = method.Search(knowledgeBase, a, d).ToList();
			Assert.AreEqual(4, paths.Count);
			Assert.AreEqual(9, paths.Min(path => path.ComputingLongevity));
			Assert.AreEqual(10, paths.Max(path => path.ComputingLongevity));
			Assert.AreEqual(39, paths.Sum(path => path.ComputingLongevity));
			paths = method.Search(knowledgeBase, b, d).ToList();
			Assert.AreEqual(2, paths.Count);
			Assert.AreEqual(5, paths.Min(path => path.ComputingLongevity));
			Assert.AreEqual(6, paths.Max(path => path.ComputingLongevity));
		}

		private static void validateLoop(SearchProcedure method, KnowledgeBase knowledgeBase)
		{
			Assert.AreEqual(0, method.Search(knowledgeBase, a, a).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, b, b).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, c, c).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, d, d).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, b, a).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, c, a).Count());
			Assert.AreNotEqual(0, method.Search(knowledgeBase, c, b).Count());
			Assert.AreEqual(0, method.Search(knowledgeBase, d, a).Count());
			Assert.AreEqual(1, method.Search(knowledgeBase, d, b).Single().ComputingLongevity);
			Assert.AreEqual(2, method.Search(knowledgeBase, d, c).Single().ComputingLongevity);
		}
	}
}
