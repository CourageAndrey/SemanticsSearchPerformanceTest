using System.Collections.Generic;
using System.Linq;

namespace SemanticsSearchPerformanceTest
{
	public class AstarCache : SearchProcedure
	{
		public AstarCache()
			: base("A* with path cache")
		{ }

		public override IEnumerable<Path> Search(KnowledgeBase knowledgeBase, Node from, Node to)
		{
			if (pathsCache == null)
			{
				pathsCache = findAllAstar(knowledgeBase);
			}
			Dictionary<Node, List<Path>> pathsDictionary;
			List<Path> paths;
			return pathsCache.TryGetValue(from, out pathsDictionary) && pathsDictionary.TryGetValue(to, out paths)
				? paths
				: new List<Path>();
		}

		private Dictionary<Node, Dictionary<Node, List<Path>>> pathsCache;

		private static Dictionary<Node, Dictionary<Node, List<Path>>> findAllAstar(KnowledgeBase knowledgeBase)
		{
			var allPaths = new Dictionary<Node, Dictionary<Node, List<Path>>>();

			// get all single-arc paths
			var fromArcs = new Dictionary<Node, List<Arc>>();
			foreach (var node in knowledgeBase.Nodes)
			{
				fromArcs[node] = new List<Arc>();
			}
			var toCheck = new List<Path>();
			foreach (var arc in knowledgeBase.Arcs)
			{
				toCheck.Add(new Path(arc));
				// cache out arcs
				fromArcs[arc.From].Add(arc);
			}

			while (toCheck.Count > 0) // while there is something to add
			{
				// save all new paths
				foreach (var path in toCheck)
				{
					allPaths.Add(path);
				}
				// create list for next step
				var newWave = new List<Path>();
				foreach (var path in toCheck) // for each new path
				{ // for each arc which starts at path's end
					foreach (var arc in fromArcs[path.To]/*knowledgeBase.Arcs.Where(a => a.From == path.To)*/)
					{
						if (path.Any(a => a.From == arc.To)) continue; // no loops
						var newPath = new Path(path, arc);
						Dictionary<Node, List<Path>> pathsDictionary;
						List<Path> paths;
						if (!allPaths.TryGetValue(path.From, out pathsDictionary) ||
							!pathsDictionary.TryGetValue(arc.To, out paths) ||
							paths.All(p => !p.Equals(newPath)))
						{ // if this path is absolutely new
							newWave.Add(newPath);
						}
					}
				}
				toCheck = newWave; // go to next step
			}
			return allPaths;
		}
	}
}