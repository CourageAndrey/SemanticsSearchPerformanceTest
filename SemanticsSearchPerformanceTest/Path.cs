using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace SemanticsSearchPerformanceTest
{
	public class Path : ReadOnlyCollection<Arc>, IEquatable<Path>
	{
		#region Properties

		public readonly int ComputingLongevity;
		public readonly string Text;
		public readonly Node From, To;

		#endregion

		#region Constructors

		public Path(Arc arc)
			: this(arc.From, arc.To, new[] { arc })
		{ }

		public Path(Path path, Arc addItem)
			: this(path.From, addItem.To, join(path, addItem))
		{ }

		private Path(Node from, Node to, IList<Arc> collection)
			: base(collection)
		{
			From = from;
			To = to;
			ComputingLongevity = this.Sum(arc => arc.ComputingLongevity);
			Text = getText();
		}

		private static List<Arc> join(Path path, Arc addItem)
		{
			if (path.Last().To != addItem.From) throw new Exception("Impossible to join arc to path because of path's last node.");
			if (path.Any(arc => arc.From == addItem.To)) throw new Exception("Loops are forbidden!");
			return new List<Arc>(path) { addItem };
		}

		#endregion

		public override string ToString()
		{
			return Text;
		}

		private string getText()
		{
			string arcs = Count > 0
				? string.Join("", this.Select(arc => string.Format(CultureInfo.InvariantCulture, "({0} > {1})", arc.From.Id, arc.To.Id)))
				: "[ ]";
			return string.Format(CultureInfo.InvariantCulture, "path from {0} to {1} longevity = {2}: {3}", From.Id, To.Id, ComputingLongevity, arcs);
		}

		public bool Equals(Path other)
		{
			return this.SequenceEqual(other);
		}
	}

	public static class PathsHelper
	{
		public static List<Path> TryGet(this IEnumerable<Path> allPaths, Node from, Node to)
		{
			return allPaths.Where(p => p.From == from && p.To == to).ToList();
		}

		public static void Add(this Dictionary<Node, Dictionary<Node, List<Path>>> allPaths, Path path)
		{
			Dictionary<Node, List<Path>> pathsDictionary;
			if (!allPaths.TryGetValue(path.From, out pathsDictionary))
			{
				allPaths[path.From] = pathsDictionary = new Dictionary<Node, List<Path>>();
			}
			List<Path> paths;
			if (!pathsDictionary.TryGetValue(path.To, out paths))
			{
				pathsDictionary[path.To] = paths = new List<Path>();
			}
			paths.Add(path);
		}
	}
}
