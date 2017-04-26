using System.Globalization;

namespace SemanticsSearchPerformanceTest
{
	public class Node
	{
		public int Id
		{ get { return id; } }

		private readonly int id;

		public Node(int id)
		{
			this.id = id;
		}

		public override string ToString()
		{
			return id.ToString("X16", CultureInfo.InvariantCulture);
		}
	}
}
