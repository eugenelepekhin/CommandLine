using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using CommandLineParser;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CommandLine.UnitTest")]

namespace CommandLineSample {
	internal class Program {
		// /?
		// /n Mercury Venus Mars
		// /v /n Mercury Venus Mars "Planets of the Solar system"
		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters")]
		static void Main(string[] args) {
			bool verbose = false;
			bool printHelp = false;
			List<string> names = new List<string>();
			int count = 1;
			CommandLine commandLine = new CommandLine()
				.AddString("n", null, "<Name>", "name of someone to greet", true, l => names.Add(l))
				.AddInt("count", "c", "<Count>", "number of times to repeat greetings", false, 1, 10, i => count = i)
				.AddFlag("verbose", "v", "verbose debug output", false, v => verbose = v)
				.AddFlag("help", "?", "print command usage", false, h => printHelp = h)
			;
			string? error = commandLine.Parse(args, l => names.AddRange(l));
			if(printHelp) {
				Usage(commandLine.Help());
				return;
			}
			if(error != null) {
				string appName = Process.GetCurrentProcess().ProcessName;
				Console.WriteLine("{0}: {1}", appName, error);
				Console.WriteLine("For more information run {0} /?", appName);
				return;
			}
			Greet(verbose, names, count);
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters")]
		private static void Usage(string help) {
			string appName = Process.GetCurrentProcess().ProcessName;
			Console.WriteLine("{0} - greeting application", appName);
			Console.WriteLine("usage: {0} OPTIONS [Other names]", appName);
			Console.WriteLine("Greets people in the provided list");
			Console.WriteLine();
			Console.WriteLine("OPTIONS:");
			Console.WriteLine(help);
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters")]
		private static void Greet(bool verbose, List<string> names, int count) {
			if(verbose) {
				Console.WriteLine("Printing greetings for {0} persons in verbose mode, repeating {1} time{2}", names.Count, count, (1 < count) ? "s" : string.Empty);
			}
			foreach(string name in names) {
				for(int i = 0; i < count; i++) {
					Console.WriteLine("Hello {0}", name);
				}
			}
		}
	}
}
