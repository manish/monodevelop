﻿//
// MonoDevelopTemplatesTest.cs
//
// Author:
//       Manish Sinha <manish.sinha@xamarin.com>
//
// Copyright (c) 2015 Xamarin Inc.
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

using System.Threading;

using NUnit.Framework;

namespace UserInterfaceTests
{
	/*
	 * Project templates to be tested - Console Project, Library, Portable Library, NUnit Library, F# Tutorial
	 * Projects which cannot be tested
	 *  - Empty Project: They do not have a build target set
	 *  - Gtk# 2.0 Project - Throws an error when created, though builds fine
	 */
	public class MonoDevelopTemplatesTest : CreateBuildTemplatesTestBase
	{
		readonly static string DotNetProjectKind = ".NET";

		[Test]
		public void TestCreateBuildConsoleProject ()
		{
			Thread.Sleep (3000);
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
			CreateBuildProject ("NUnitLibraryProject", "NUnit Library Project", DotNetProjectKind, delegate {
					Ide.WaitUntil (() => Ide.GetStatusMessage () == "Package updates are available.", pollStep: 1000);
				});
		}

		[Test]
		public void TestCreateBuildFSharpTutorial ()
		{
			CreateBuildProject ("FSharpTutorial", "F# Tutorial", DotNetProjectKind, EmptyAction);
		}
	}
}

