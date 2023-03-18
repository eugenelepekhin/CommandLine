using CommandLineParser;

namespace CommandLineUnitTest {
	[TestClass]
	public class CommandLineTest {
		private void ShouldNotBeCalled() {
			Assert.Fail("This should no be called");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UndefinedParametersTest() {
			string[] args = { };
			CommandLine commandLine = new CommandLine();
			commandLine.Parse(args, l => this.ShouldNotBeCalled());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void UniqueParameters1Test() {
			string[] args = { };
			CommandLine commandLine = new CommandLine()
				.AddFlag("a", null, "note", true, a => this.ShouldNotBeCalled())
				.AddFlag("a", null, "note", true, a => this.ShouldNotBeCalled())
			;
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void UniqueParameters2Test() {
			string[] args = { };
			CommandLine commandLine = new CommandLine()
				.AddFlag("c", null, "note", true, a => this.ShouldNotBeCalled())
				.AddFlag("a", null, "note", true, a => this.ShouldNotBeCalled())
				.AddFlag("b", "a", "note", true, a => this.ShouldNotBeCalled())
			;
		}

		[TestMethod]
		public void EmptyOptionalTest() {
			string[] args = { };

			bool flag = true;
			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "optional", false, f => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && flag);

			string help = commandLine.Help();
			StringAssert.Contains(help, "optional");
		}

		[TestMethod]
		public void NullOptionalTest() {
			string[]? args = null;

			bool flag = true;
			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "optional", false, f => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args!, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && flag);
		}

		[TestMethod]
		public void Flag1Test() {
			string[] args = { "/f" };

			bool flag = false;
			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "note", true, f => flag = f)
			;
			string? errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && flag);

			string help = commandLine.Help();
			StringAssert.Contains(help, "note");
		}

		[TestMethod]
		public void MissingFlagTest() {
			string[] args = { };
			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "note", true, f => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			//"Required parameter f is missing"
			StringAssert.Contains(errors, "\"f\"");
		}

		[TestMethod]
		public void MultipleMissingFlagTest() {
			string[] args = { };
			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "note", true, f => this.ShouldNotBeCalled())
				.AddFlag("g", null, "note", true, f => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			//"Required parameter f is missing"
			//"Required parameter g is missing"
			StringAssert.Contains(errors, "\"f\"");
			StringAssert.Contains(errors, "\"g\"");
		}

		[TestMethod]
		public void FlagSyntaxTest() {
			string[] args = { "/f-", "-d:true", "--v:on", "k=false" };

			bool flagF = true;
			bool flagD = false;
			bool flagV = false;
			bool flagK = true;
			bool flagO = true;
			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "note", true, f => flagF = f)
				.AddFlag("d", null, "note", true, d => flagD = d)
				.AddFlag("v", null, "note", true, v => flagV = v)
				.AddFlag("k", null, "note", true, k => flagK = k)
				.AddFlag("o", null, "note", false, o => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && !flagF && flagD && flagV && !flagK && flagO);
		}

		[TestMethod]
		public void FlagUnknownTest() {
			string[] args = { "/f" };

			CommandLine commandLine = new CommandLine()
				.AddFlag("a", null, "note", false, f => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			// "Unknown parameter f"
			StringAssert.Contains(errors, args[0]);
		}

		[TestMethod]
		public void InvalidFlagTest() {
			string[] args = { "/f=bad" };

			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "note", true, f => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			// "Parameter f has invalid value bad"
			StringAssert.Contains(errors, "bad");
		}

		[TestMethod]
		public void MatchedFlagTest() {
			string[] args = { "-=?" };

			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "note", false, f => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, null);
			// "Unrecognized parameter: -=?"
			StringAssert.Contains(errors, args[0]);
		}

		[TestMethod]
		public void String1Test() {
			string[] args = { "a=b", "c:d", "e", "f", "/g", "h", "-i=j", "--k:l" };
			string? textA = null;
			string? textC = null;
			string? textE = null;
			string? textG = null;
			string? textI = null;
			string? textK = null;
			string? textM = null;
			string? textZ = null;
			CommandLine commandLine = new CommandLine()
				.AddString("a", null, null, "note", true, a => textA = a)
				.AddString("c", null, null, "note", true, c => textC = c)
				.AddString("e", null, null, "note", true, e => textE = e)
				.AddString("g", null, null, "note", true, g => textG = g)
				.AddString("i", null, null, "note", true, i => textI = i)
				.AddString("k", null, null, "note", true, k => textK = k)
				.AddString("m", null, null, "note", false, m => textM = m)
				.AddString("z", null, null, "note", false, z => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && textA == "b" && textC == "d" && textE == "f" && textG == "h" && textI == "j" && textK == "l" && textM == null && textZ == null);
		}

		[TestMethod]
		public void String2Test() {
			string[] args = { "b=c" };
			CommandLine commandLine = new CommandLine()
				.AddString("a", null, null, "note", true, a => this.ShouldNotBeCalled())
				.AddString("b", null, null, "note", false, b => { })
			;
			string? errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			//"Required parameter a is missing"
			StringAssert.Contains(errors, "\"a\"");
		}

		[TestMethod]
		public void String3Test() {
			string[] args = { "b" };
			CommandLine commandLine = new CommandLine()
				.AddString("b", null, null, "note", false, b => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			//"Parameter b is missing value"
			StringAssert.Contains(errors, "\"b\"");
		}

		[TestMethod]
		public void StringSingleNamedPath() {
			string[] args = { @"/path", @"c:\folder\file.txt" };
			string? actual = null;
			CommandLine commandLine = new CommandLine()
				.AddString("path", null, "path", "note", true, s => { Assert.IsNull(actual); actual = s; })
			;
			string? errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsNull(errors);
			Assert.AreEqual(args[1], actual);
		}

		[TestMethod]
		public void StringMultipleNamedPath() {
			string[] args = {
				@"/path", @"c:\folder\file1.txt",
				@"/path", @"c:\folder\file2.txt",
				@"/path", @"c:\folder\file3.txt",
			};
			int index = 0;
			CommandLine commandLine = new CommandLine()
				.AddString("path", null, "path", "note", true, s => { Assert.AreEqual(args[index * 2 + 1], s); index++; })
			;
			string? errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsNull(errors);
			Assert.AreEqual(args.Length / 2, index);
		}

		[TestMethod]
		public void StringSingleUnmatchedPath() {
			string[] args = { @"c:\folder\file.txt" };
			CommandLine commandLine = new CommandLine()
				.AddFlag("dummy", null, "note", false, b => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => Assert.IsTrue(1 == l.Count() && args[0] == l.First()));
			Assert.IsNull(errors);
		}

		[TestMethod]
		public void StringMultipleUnmatchedPath() {
			string[] args = {
				@"c:\folder\file1.txt",
				@"c:\folder\file2.txt",
				@"c:\folder\file3.txt",
			};
			CommandLine commandLine = new CommandLine()
				.AddFlag("dummy", null, "note", false, b => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, list => Assert.IsTrue(args.Length == list.Count() && list.All(s => args.Contains(s))));
			Assert.IsNull(errors);
		}

		[TestMethod]
		public void Int1Test() {
			string[] args = { "/a", "4", "b=6", "-c:-7", "-d", "+15", "/e=0", "-f", int.MinValue.ToString(), "/g:" + int.MaxValue.ToString() };
			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;
			int e = 100;
			int f = 0;
			int g = 0;
			CommandLine commandLine = new CommandLine()
				.AddInt("a", null, "a", "a", true, 1, 10, i => a = i)
				.AddInt("b", null, "b", "b", true, 1, 10, i => b = i)
				.AddInt("c", null, "c", "c", true, -10, 10, i => c = i)
				.AddInt("d", null, "d", "d", true, 1, 100, i => d = i)
				.AddInt("e", null, "e", "e", true, 0, 10, i => e = i)
				.AddInt("f", null, "f", "f", true, i => f = i)
				.AddInt("g", null, "g", "g", true, i => g = i)
			;
			string? errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && a == 4 && b == 6 && c == -7 && d == 15 && e == 0 & f == int.MinValue && g == int.MaxValue);
		}

		[TestMethod]
		public void Int2NotNumberTest() {
			string[] args = { "/aaa", "bbb" };
			CommandLine commandLine = new CommandLine()
				.AddInt("aaa", null, "aaa", "aaa", true, 1, 10, i => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			StringAssert.Contains(errors, "aaa");
			StringAssert.Contains(errors, "bbb");
		}

		[TestMethod]
		public void Int3NotInRangeTest() {
			string[] args = { "/aaa", "4" };
			CommandLine commandLine = new CommandLine()
				.AddInt("aaa", null, "aaa", "aaa", true, 10, 100, i => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			StringAssert.Contains(errors, "aaa");
			StringAssert.Contains(errors, "4");
		}

		[TestMethod]
		public void Int4NotInRangeTest() {
			string[] args = { "/aaa", "400" };
			CommandLine commandLine = new CommandLine()
				.AddInt("aaa", null, "aaa", "aaa", true, 10, 100, i => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			StringAssert.Contains(errors, "aaa");
			StringAssert.Contains(errors, "400");
		}

		[TestMethod]
		public void Unknow1Test() {
			string[] args = { "b" };
			CommandLine commandLine = new CommandLine()
				.AddString("a", null, null, "note", false, a => this.ShouldNotBeCalled())
			;
			string? errors = commandLine.Parse(args, null);
			//"Unrecognized parameter: b"
			StringAssert.Contains(errors, " b");
		}

		[TestMethod]
		public void List1Test() {
			string[] args = { "/f:a", "-f:b", "--f:c", "f:d", "f", "g", "/f=i" };
			string[] expected = { "a", "b", "c", "d", "g", "i" };
			List<string> list = new List<string>();
			CommandLine commandLine = new CommandLine()
				.AddString("f", null, null, "note", true, l => list.Add(l))
			;
			string? errors = commandLine.Parse(args, null);
			Assert.IsTrue(errors == null && list != null && list.Count() == expected.Length && expected.All(s => list.Contains(s)));
		}

		[TestMethod]
		public void ReentryTest() {
			string[] args1 = { "/a" };
			string[] args2 = { "/b" };
			string[] args3 = { "/a-", "/b:False" };
			bool flagA = false;
			bool flagB = false;
			CommandLine commandLine = new CommandLine()
				.AddFlag("a", null, "note", true, a => flagA = a)
				.AddFlag("b", null, "note", true, b => flagB = b)
			;
			string? errors1 = commandLine.Parse(args1, null);
			StringAssert.Contains(errors1, "\"b\"");
			
			string? errors2 = commandLine.Parse(args2, null);
			StringAssert.Contains(errors2, "\"a\"");

			Assert.IsTrue(flagA && flagB);

			string? errors3 = commandLine.Parse(args3, null);
			Assert.IsTrue(errors3 == null && !flagA && !flagB);
		}

		[TestMethod]
		public void Help1Test() {
			string[] expected = { "flag", "say", "hello", "green", "cool", "aid", "note", "red", "world", "color", "logo" };

			CommandLine commandLine = new CommandLine()
				.AddString("flag", null, "say", "hello", true, a => this.ShouldNotBeCalled())
				.AddString("green", "cool", "aid", "note", true, a => this.ShouldNotBeCalled())
				.AddString("red", "world", "color", "logo", false, a => this.ShouldNotBeCalled())
			;
			string help = commandLine.Help();
			Assert.IsTrue(expected.All(s => help.Contains(s)));
		}
	}
}
