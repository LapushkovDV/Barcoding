using System;
using Galaktika.ERP.Interop;

namespace Galaktika.ESB.ERP
{
	/// <summary>
	/// Atlantis Adapter start point class
	/// </summary>
	public class Program
	{
		static Program()
		{
			Utils.AssemblyFinder.Init();
		}
        /// <summary>
        /// Atlantis Adapter start point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
		{
			"Galaktika.ESB.ERP.Program.Main({0}).".ErpTrace(string.Join(",", args));
			GalaktikaErpStarter.ShutdownAppOnGalThreadWorkerExit = true;
			ServiceAdapterApi.Program.Main(args);
            Environment.Exit(1);
		}
	}
}
