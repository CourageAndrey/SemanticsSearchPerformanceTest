using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SemanticsSearchPerformanceTest
{
	public class Sample : IDisposable
	{
		public readonly int NodesCount;
		public readonly int ArcsCount;
		public readonly int SearchRepeatCount;
		public readonly int MinLongevityMs;
		public readonly int MaxLongevityMs;
		private readonly List<SearchProcedure> SearchProcedures;

		public Sample(int nodesCount, int arcsCount, int searchRepeatCount, int minLongevityMs, int maxLongevityMs)
		{
			NodesCount = nodesCount;
			ArcsCount = arcsCount;
			SearchRepeatCount = searchRepeatCount;
			MinLongevityMs = minLongevityMs;
			MaxLongevityMs = maxLongevityMs;
			SearchProcedures = new List<SearchProcedure> { new AstarRegular(), new AstarCache() };
		}

		public void Perform()
		{
			Console.WriteLine("Knowledge base with {0} nodes and {1} arcs.", NodesCount, ArcsCount);
			Console.WriteLine("Get all paths between all two nodes {0} times.", SearchRepeatCount);
			Console.WriteLine("Longevity of arcs from {0} to {1} ms.", MinLongevityMs, MaxLongevityMs);
			Console.WriteLine();

			var knowledgeBase = new KnowledgeBase(NodesCount, ArcsCount, MinLongevityMs, MaxLongevityMs);
			foreach (var searchProcedure in SearchProcedures)
			{
				try
				{
					var stopWatch = new Stopwatch();
					Console.WriteLine("{0} calculation is in progress...", searchProcedure.Name);
					stopWatch.Start();

					var paths = new List<Path>();
					foreach (var from in knowledgeBase.Nodes)
					{
						foreach (var to in knowledgeBase.Nodes)
						{
							for (int i = 0; i < SearchRepeatCount; i++)
							{
								paths.AddRange(searchProcedure.Search(knowledgeBase, from, to));
							}
						}
					}
					stopWatch.Stop();
					Console.WriteLine("Calculation has just been finished.");
					var elapsedSearchProcedures = stopWatch.Elapsed;
					var elapsedRawCalculations = TimeSpan.FromMilliseconds(paths.Sum(path => path != null ? path.ComputingLongevity / SearchRepeatCount : 0));
					var elapsedTotal = elapsedSearchProcedures + elapsedRawCalculations;
					Console.WriteLine();
					Console.WriteLine("Time elapsed:");
					Console.WriteLine(" ... raw search: " + elapsedSearchProcedures);
					Console.WriteLine(" ... calculations: " + elapsedRawCalculations);
					Console.WriteLine(" ... total: " + elapsedTotal);
					Console.WriteLine("----------------------------------------------");
					Console.WriteLine();
				}
				catch (Exception error)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("An error of {0} class occurred: {1}", error.GetType().FullName, error.Message);
					Console.ForegroundColor = ConsoleColor.White;
				}
			}

			Console.WriteLine("==============================================");
			Console.WriteLine();
			Console.WriteLine();
		}

		public void Dispose()
		{
			if (!disposed)
			{
				SearchProcedures.Clear();
				MemoryHelper.Free();
				GC.SuppressFinalize(this);
				disposed = true;
			}
		}

		private bool disposed;
	}
}
