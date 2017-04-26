using System.Collections.Generic;
using System.Linq;

namespace SemanticsSearchPerformanceTest
{
	public class AstarRegular : SearchProcedure
	{
		public AstarRegular()
			: base("A*")
		{ }

		public override IEnumerable<Path> Search(KnowledgeBase knowledgeBase, Node from, Node to)
		{
			// get all single-arc paths
			var fromArcs = new Dictionary<Node, List<Arc>>();
			foreach (var node in knowledgeBase.Nodes)
			{
				fromArcs[node] = new List<Arc>();
			}
			foreach (var arc in knowledgeBase.Arcs)
			{
				// cache out arcs
				fromArcs[arc.From].Add(arc);
			}
			// restrict start nodes
			var toCheck = fromArcs[from].Select(arc => new Path(arc)).ToList();

			while (toCheck.Count > 0) // while there is something to add
			{
				// save all new paths
				foreach (var path in toCheck.Where(p => p.To == to).ToList())
				{
					yield return path;
					toCheck.Remove(path);
				}
				// create list for next step
				var newWave = new List<Path>();
				foreach (var path in toCheck) // for each new path
				{ // for each arc which starts at path's end
					foreach (var arc in fromArcs[path.To]/*knowledgeBase.Arcs.Where(a => a.From == path.To)*/)
					{
						if (path.Any(a => a.From == arc.To)) continue; // no loops
						newWave.Add(new Path(path, arc));
					}
				}
				toCheck = newWave; // go to next step
			}
		}
	}
}