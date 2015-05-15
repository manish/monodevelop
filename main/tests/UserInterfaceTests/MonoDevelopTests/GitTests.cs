//
// GitTests.cs
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

using System;
using NUnit.Core;
using NUnit.Framework;
using System.IO;
using System.Threading;

namespace UserInterfaceTests
{
	[TestFixture]
	[Category("Git")]
	public class GitTests : UITestBase
	{
		[Test]
		public void CloneTestPass ()
		{
			var folder = Util.CreateTmpDir ("git-clone");

			Session.ExecuteCommand (MonoDevelop.VersionControl.Commands.Checkout);
			Session.WaitForElement (c => c.Window ().Marked ("MonoDevelop.VersionControl.Dialogs.SelectRepositoryDialog"), 10000);

			Session.SelectElement (c => c.Marked ("repCombo").Model ().Text ("Git"));
			Assert.IsTrue (Session.EnterText (c => c.Marked ("repositoryUrlEntry"), "git@github.com:mono/monodevelop.git"));
			Assert.IsTrue (Session.EnterText (c => c.Marked ("entryFolder"), folder));
			Session.ClickElement (c => c.Button ().Marked ("buttonOk"));


			Session.WaitForElement (c => c.Window ().Marked ("MonoDevelop.Ide.Gui.DefaultWorkbench"), 10000);

			Thread.Sleep (3000);

			if (Directory.Exists (folder))
				Directory.Delete (folder, true);
		}
	}
}

