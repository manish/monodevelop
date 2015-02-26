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

namespace UserInterfaceTests
{
	public class SimpleTest: UITestBase
	{
		[Test]
		public void TestCreateBuildConsoleProject ()
		{
			CreateBuildProject ("ConsoleProject", "Console Project", ".NET");
		}
			
		public void TestCollectionsGeneric ()
		{
			var projectName = "ConsoleProject";
			var solutionParentDirectory = Util.CreateTmpDir (projectName);

			var solutionDirectory = Path.Combine (solutionParentDirectory, projectName);

			var projectDir = Path.Combine (solutionDirectory, projectName);
			var programFile = Path.Combine (projectDir, "Program.cs");
			var exe = Path.Combine (solutionDirectory, projectName, "bin", "debug", projectName+".exe");

			Ide.CreateProject (projectName, ".NET", "Console Project", solutionParentDirectory);

			//Ide.OpenFile (programFile);

			Session.SelectActiveWidget ();

			const string data = "List<string> s = new List<string> () {\"one\", \"two\", \"three\"};\nConsole.WriteLine (\"Hello Xamarin!\");";
			for (int i = 0; i < 8; i++)
				Session.ExecuteCommand (TextEditorCommands.LineDown);
			Session.ExecuteCommand (TextEditorCommands.LineStart);
			Session.ExecuteCommand (TextEditorCommands.DeleteToLineEnd);
			Session.TypeText (data);

			Ide.BuildSolution (false);

			Session.ExecuteCommand (RefactoryCommands.QuickFix);
			Thread.Sleep (1000);
			Session.PressKey (Gdk.Key.Return);

			Ide.BuildSolution ();

			AssertExeHasOutput (exe, "Hello Xamarin!");

			Ide.CloseAll ();
		}

		void AssertExeHasOutput (string exe, string expectedOutput)
		{
			var sw = new StringWriter ();
			var p = ProcessUtils.StartProcess (new ProcessStartInfo (exe), sw, sw, CancellationToken.None);
			Assert.AreEqual (0, p.Result);
			string output = sw.ToString ();

			Assert.AreEqual (expectedOutput, output.Trim ());
		}

		void CreateBuildProject (string projectName, string kind, string category)
		{
			var solutionParentDirectory = Util.CreateTmpDir (projectName);

			Ide.CreateProject (projectName, category, kind, solutionParentDirectory);

			Ide.BuildSolution ();

			Ide.CloseAll ();
		}
	}
}
