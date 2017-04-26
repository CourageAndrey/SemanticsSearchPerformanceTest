using System;

namespace SemanticsSearchPerformanceTest
{
	public static class MemoryHelper
	{
		public static void Free()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}
}
