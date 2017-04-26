using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SemanticsSearchPerformanceTest
{
	public class KnowledgeBase
	{
		#region Properties

		public IReadOnlyList<Node> Nodes
		{ get { return nodes; } }

		public IReadOnlyList<Arc> Arcs
		{ get { return arcs; } }

		private readonly IReadOnlyList<Node> nodes;
		private readonly IReadOnlyList<Arc> arcs;

		#endregion

		#region Constructors

		public KnowledgeBase(List<Node> allNodes, List<Arc> allArcs)
			: this(new Tuple<List<Node>, List<Arc>>(allNodes, allArcs))
		{ }

		public KnowledgeBase(int nodesCount, int arcsCount, int minLongevityMs, int maxLongevityMs)
			: this(generate(nodesCount, arcsCount, minLongevityMs, maxLongevityMs))
		{ }

		private KnowledgeBase(Tuple<List<Node>, List<Arc>> content)
		{
			nodes = new ReadOnlyCollection<Node>(content.Item1);
			arcs = new ReadOnlyCollection<Arc>(content.Item2);
		}

		private static Tuple<List<Node>, List<Arc>> generate(int nodesCount, int arcsCount, int minLongevityMs, int maxLongevityMs)
		{
			if (nodesCount < 2)
			{
				throw new ArgumentOutOfRangeException("nodesCount", "nodesCount must be greater than 1!");
			}
			if (arcsCount < 1)
			{
				throw new ArgumentOutOfRangeException("nodesCount", "nodesCount must be greater than 0!");
			}
			if (minLongevityMs <= 0)
			{
				throw new ArgumentOutOfRangeException("minLongevityMs", "minLongevityMs must be greater than 0!");
			}
			if (maxLongevityMs <= minLongevityMs)
			{
				throw new ArgumentException("maxLongevityMs must be greater than minLongevityMs!");
			}

			var nodes = new List<Node>();
			for (int n = 0; n < nodesCount; n++)
			{
				nodes.Add(new Node(n));
			}

			var arcs = new List<Arc>();
			var random = new Random(DateTime.Now.Millisecond);
			for (int a = 0; a < arcsCount; a++)
			{
				var from = nodes[a%nodes.Count];
				Node to;
				do
				{
					to = nodes[random.Next(0, nodes.Count)];
				} while (to == from);
				arcs.Add(new Arc(from, to, minLongevityMs + random.Next(maxLongevityMs - minLongevityMs)));
			}

			return new Tuple<List<Node>, List<Arc>>(nodes, arcs);
		}

		#endregion

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "knowledge base {0} nodes {1} arcs", Nodes.Count, Arcs.Count);
		}
	}
}
