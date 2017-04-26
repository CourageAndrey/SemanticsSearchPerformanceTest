using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticsSearchPerformanceTest
{
	static class Program
	{
		static void Main()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.SetBufferSize(Console.BufferWidth, 10000);

			const int minLongevityMs = 10;
			const int maxLongevityMs = 1000;
			int nodes, arcs, repeats;

			// 1. t(nodes)
			repeats = 5;
			for (nodes = 5; nodes <= 40; nodes += 5)
			{
				arcs = nodes*2;
				using (var sample = new Sample(nodes, arcs, repeats, minLongevityMs, maxLongevityMs))
				{
					sample.Perform();
				}
			}

			// 2. t(arcs)
			nodes = 10;
			repeats = 5;
			for (arcs = 5; arcs <= 50; arcs += 5)
			{
				using (var sample = new Sample(nodes, arcs, repeats, minLongevityMs, maxLongevityMs))
				{
					sample.Perform();
				}
			}

			// 3. t(repeats)
			nodes = 5;
			arcs = 50;
			for (repeats = 1; repeats < 10; repeats += 2)
			{
				using (var sample = new Sample(nodes, arcs, repeats, minLongevityMs, maxLongevityMs))
				{
					sample.Perform();
				}
			}

			// 4. max(nodes)
			nodes = 5;
			var methodErrors = new Dictionary<Func<SearchProcedure>, bool>
			{
				{ () => new AstarRegular(), false },
				{ () => new AstarCache(), false },
			};
			do
			{
				arcs = nodes*nodes/2;
				Console.WriteLine("{0} nodes, {1} arcs", nodes, arcs);
				var knowledgeBase = new KnowledgeBase(nodes, arcs, minLongevityMs, maxLongevityMs);
				foreach (var methodCreator in methodErrors.Where(pair => !pair.Value).Select(pair => pair.Key).ToList())
				{
					var method = methodCreator();
					try
					{
						method.Search(knowledgeBase, knowledgeBase.Nodes.First(), knowledgeBase.Nodes.Last());
					}
					catch
					{
						methodErrors[methodCreator] = true;
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Method {0} failed", method.Name);
						Console.ForegroundColor = ConsoleColor.White;
					}
					finally
					{
						MemoryHelper.Free();
					}
				}
				nodes = (int) (nodes * (methodErrors.Values.Any(value => value) ? 1.25 : 2));
			} while (!methodErrors.Values.All(error => error));

			Console.WriteLine();
			Console.WriteLine("Press any key to close the program...");
			Console.ReadKey();
		}
	}
}
