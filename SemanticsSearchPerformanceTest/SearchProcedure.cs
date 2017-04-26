using System;
using System.Collections.Generic;

namespace SemanticsSearchPerformanceTest
{
	public abstract class SearchProcedure
	{
		public readonly string Name;

		protected SearchProcedure(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			Name = name;
		}

		public abstract IEnumerable<Path> Search(KnowledgeBase knowledgeBase, Node from, Node to);
	}
}
