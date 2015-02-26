//
// SimpleTest.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoDevelop.Core;
using MonoDevelop.Ide.Commands;
using NUnit.Framework;
using MonoDevelop.Refactoring;
using System;

namespace UserInterfaceTests
{
	/*
	 * Project templates to be tested - Console Project, Library, Portable Library, NUnit Library, F# Tutorial
	 * Projects which cannot be tested
	 *  - Empty Project: They do not have a build target set
	 *  - Gtk# 2.0 Project - Throws an error when created, though builds fine
	 */
	public class CreateBuildTemplatesTest: UITestBase
	{
		readonly static string DotNetProjectKind = ".NET";

		public readonly static Action EmptyAction = () => { };

		[Test]
		public void TestCreateBuildConsoleProject ()
		{
			CreateBuildProject ("ConsoleProject", "Console Project", DotNetProjectKind, EmptyAction);
		}

		[Test]
		public void TestCreateBuildGtkSharp20Project ()
		{
			CreateBuildProject ("Gtk20Project", "Gtk# 2.0 Project", DotNetProjectKind, EmptyAction);
		}

		[Test]
		public void TestCreateBuildLibrary ()
		{
			CreateBuildProject ("Library", "Library", DotNetProjectKind, EmptyAction);
		}

		[Test]
		public void TestCreateBuildPortableLibrary ()
		{
			CreateBuildProject ("PortableLibrary", "Portable Library", DotNetProjectKind, EmptyAction);
		}

		[Test]
		public void TestCreateBuildNUnitLibraryProject ()
		{
			/* NUnit project needs to fetch the references using NuGet, so we need
			 * NuGet to finish before we compile. A better method would be to block
			 * using ManualResetEvent and monitor the status. When the reference
			 * fetching is over, signal ManualResetEvent
			 */
			CreateBuildProject ("NUnitLibraryProject", "NUnit Library Project", DotNetProjectKind, () => Thread.Sleep (10000));
		}

		[Test]
		public void TestCreateBuildFSharpTutorial ()
		{
			CreateBuildProject ("FSharpTutorial", "F# Tutorial", DotNetProjectKind, EmptyAction);
		}

		public void AssertExeHasOutput (string exe, string expectedOutput)
		{
			var sw = new StringWriter ();
			var p = ProcessUtils.StartProcess (new ProcessStartInfo (exe), sw, sw, CancellationToken.None);
			Assert.AreEqual (0, p.Result);
			string output = sw.ToString ();

			Assert.AreEqual (expectedOutput, output.Trim ());
		}

		public void CreateBuildProject (string projectName, string kind, string category, Action beforeBuild)
		{
			var solutionParentDirectory = Util.CreateTmpDir (projectName);

			Ide.CreateProject (projectName, category, kind, solutionParentDirectory);

			beforeBuild ();

			Ide.BuildSolution ();

			Ide.CloseAll ();
		}
	}
}
