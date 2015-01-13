using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommandLineParser;

namespace CommandLineTest {
	class Program {
		// /?
		// /n Mercury Venus Mars
		// /v /n Mercury Venus Mars "Planets of the Solar system"
		static void Main(string[] args) {
			bool verbose = false;
			bool printHelp = false;
			List<string> names = new List<string>();
			CommandLine commandLine = new CommandLine()
				.AddString("name", "n", "name of someone to greet", true, l => names.Add(l))
				.AddFlag("verbose", "v", "verbose debug output", false, v => verbose = v)
				.AddFlag("help", "?", "print command usage", false, h => printHelp = h)
			;
			string error = commandLine.Parse(args, l => names.AddRange(l));
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
			Greet(verbose, names);
		}

		private static void Usage(string help) {
			string appName = Process.GetCurrentProcess().ProcessName;
			Console.WriteLine("{0} - greeting application", appName);
			Console.WriteLine("usage: {0} OPTIONS [Other names]", appName);
			Console.WriteLine("Greets people in the provided list");
			Console.WriteLine();
			Console.WriteLine("OPTIONS:");
			Console.WriteLine(help);
		}

		private static void Greet(bool verbose, IEnumerable<string> names) {
			if(verbose) {
				Console.WriteLine("Printing greetings for {0} persons in verbose mode", names.Count());
			}
			foreach(string name in names) {
				Console.WriteLine("Hello {0}", name);
			}
		}
	}
}
