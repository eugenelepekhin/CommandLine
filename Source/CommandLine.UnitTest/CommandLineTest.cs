﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandLineParser;
using System.Collections.Generic;

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
			string errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && flag);

			string help = commandLine.Help();
			StringAssert.Contains(help, "optional");
		}

		[TestMethod]
		public void NullOptionalTest() {
			string[] args = null;

			bool flag = true;
			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "optional", false, f => this.ShouldNotBeCalled())
			;
			string errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && flag);
		}

		[TestMethod]
		public void Flag1Test() {
			string[] args = { "/f" };

			bool flag = false;
			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "note", true, f => flag = f)
			;
			string errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
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
			string errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
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
			string errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
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
			string errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && !flagF && flagD && flagV && !flagK && flagO);
		}

		[TestMethod]
		public void FlagUnknownTest() {
			string[] args = { "/f" };

			CommandLine commandLine = new CommandLine()
				.AddFlag("a", null, "note", false, f => this.ShouldNotBeCalled())
			;
			string errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			// "Unknown parameter f"
			StringAssert.Contains(errors, args[0]);
		}

		[TestMethod]
		public void InvalidFlagTest() {
			string[] args = { "/f=bad" };

			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "note", true, f => this.ShouldNotBeCalled())
			;
			string errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			// "Parameter f has invalid value bad"
			StringAssert.Contains(errors, "bad");
		}

		[TestMethod]
		public void MatchedFlagTest() {
			string[] args = { "-=?" };

			CommandLine commandLine = new CommandLine()
				.AddFlag("f", null, "note", false, f => this.ShouldNotBeCalled())
			;
			string errors = commandLine.Parse(args, null);
			// "Unrecognized parameter: -=?"
			StringAssert.Contains(errors, args[0]);
		}

		[TestMethod]
		public void String1Test() {
			string[] args = { "a=b", "c:d", "e", "f", "/g", "h", "-i=j", "--k:l" };
			string textA = null;
			string textC = null;
			string textE = null;
			string textG = null;
			string textI = null;
			string textK = null;
			string textM = null;
			string textZ = null;
			CommandLine commandLine = new CommandLine()
				.AddString("a", null, "note", true, a => textA = a)
				.AddString("c", null, "note", true, c => textC = c)
				.AddString("e", null, "note", true, e => textE = e)
				.AddString("g", null, "note", true, g => textG = g)
				.AddString("i", null, "note", true, i => textI = i)
				.AddString("k", null, "note", true, k => textK = k)
				.AddString("m", null, "note", false, m => textM = m)
				.AddString("z", null, "note", false, z => this.ShouldNotBeCalled())
			;
			string errors = commandLine.Parse(args, l => Assert.AreEqual(0, l.Count()));
			Assert.IsTrue(errors == null && textA == "b" && textC == "d" && textE == "f" && textG == "h" && textI == "j" && textK == "l" && textM == null && textZ == null);
		}

		[TestMethod]
		public void String2Test() {
			string[] args = { "b=c" };
			CommandLine commandLine = new CommandLine()
				.AddString("a", null, "note", true, a => this.ShouldNotBeCalled())
				.AddString("b", null, "note", false, b => { })
			;
			string errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			//"Required parameter a is missing"
			StringAssert.Contains(errors, "\"a\"");
		}

		[TestMethod]
		public void String3Test() {
			string[] args = { "b" };
			CommandLine commandLine = new CommandLine()
				.AddString("b", null, "note", false, b => this.ShouldNotBeCalled())
			;
			string errors = commandLine.Parse(args, l => this.ShouldNotBeCalled());
			//"Parameter b is missing value"
			StringAssert.Contains(errors, "\"b\"");
		}

		[TestMethod]
		public void Unknow1Test() {
			string[] args = { "b" };
			CommandLine commandLine = new CommandLine()
				.AddString("a", null, "note", false, a => this.ShouldNotBeCalled())
			;
			string errors = commandLine.Parse(args, null);
			//"Unrecognized parameter: b"
			StringAssert.Contains(errors, " b");
		}

		[TestMethod]
		public void List1Test() {
			string[] args = { "/f:a", "-f:b", "--f:c", "f:d", "f", "g", "/f=i" };
			string[] expected = { "a", "b", "c", "d", "g", "i" };
			List<string> list = new List<string>();
			CommandLine commandLine = new CommandLine()
				.AddString("f", null, "note", true, l => list.Add(l))
			;
			string errors = commandLine.Parse(args, null);
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
			string errors1 = commandLine.Parse(args1, null);
			StringAssert.Contains(errors1, "\"b\"");
			
			string errors2 = commandLine.Parse(args2, null);
			StringAssert.Contains(errors2, "\"a\"");

			Assert.IsTrue(flagA && flagB);

			string errors3 = commandLine.Parse(args3, null);
			Assert.IsTrue(errors3 == null && !flagA && !flagB);
		}

		[TestMethod]
		public void Help1Test() {
			string[] expected = { "flag", "hello", "green", "cool", "note", "red", "world", "logo" };

			CommandLine commandLine = new CommandLine()
				.AddString("flag", null, "hello", true, a => this.ShouldNotBeCalled())
				.AddString("green", "cool", "note", true, a => this.ShouldNotBeCalled())
				.AddString("red", "world", "logo", false, a => this.ShouldNotBeCalled())
			;
			string help = commandLine.Help();
			Assert.IsTrue(expected.All(s => help.Contains(s)));
		}
	}
}
