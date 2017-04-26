using System;
using System.Globalization;

namespace SemanticsSearchPerformanceTest
{
	public class Arc
	{
		public Node From
		{ get { return from; } }

		public Node To
		{ get { return to; } }

		public int ComputingLongevity
		{ get { return computingLongevity; } }

		private readonly Node from, to;
		private readonly int computingLongevity;

		public Arc(Node from, Node to, int computingLongevity)
		{
			if (from == to)
			{
				throw new ArgumentException("Self-loops are forbidden!");
			}
			this.from = from;
			this.to = to;
			this.computingLongevity = computingLongevity;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "arc from {0} to {1} (lasts {2} ms)", from, to, computingLongevity);
		}
	}
}
