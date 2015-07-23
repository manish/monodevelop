﻿//
// GitStashManagerTests.cs
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

using NUnit.Framework;
using System;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Components.AutoTest;

namespace UserInterfaceTests
{
	[TestFixture]
	[Category ("Git")]
	[Category ("StashManager")]
	public class GitStashManagerTests : StashManagerBase
	{
		[Test, Ignore]
		public void GitRemoveStashTest ()
		{
			CreateProjectAndCommitAndStash ();

			OpenStashManager ();
			RemoveStash (0);
			Assert.IsEmpty (Session.Query (StashEntries));
			CloseStashManager ();
		}

		[Test, Ignore]
		public void GitApplyAndRemoveStashTest ()
		{
			CreateProjectAndCommitAndStash ();

			OpenStashManager ();
			ApplyAndRemoveStash (0);
			OpenStashManager ();

			TakeScreenShot ("Asserting-if-Not-Stash-Present");
			Assert.IsEmpty (Session.Query (StashEntries));
			CloseStashManager ();
		}

		[Test, Ignore]
		public void GitApplyStashTest ()
		{
			CreateProjectAndCommitAndStash ();

			OpenStashManager ();
			ApplyStash (0);
			OpenStashManager ();

			TakeScreenShot ("Asserting-if-Stash-Still-Present");
			Assert.IsNotEmpty (Session.Query (StashEntries));
			CloseStashManager ();
		}

		[Test]
		public void GitStashConvertToBranchTest ()
		{
			CreateProjectAndCommitAndStash ();

			OpenStashManager ();

			CloseStashManager ();
		}

		void CreateProjectAndCommitAndStash ()
		{
			var templateOptions = new TemplateSelectionOptions {
				CategoryRoot = OtherCategoryRoot,
				Category = ".NET",
				TemplateKindRoot = GeneralKindRoot,
				TemplateKind = "Console Project"
			};
			GitCreateAndCommit (templateOptions, "First commit");
			var changeDescription = MakeSomeChangesAndSaveAll ("Program.cs");
			TestGitStash (changeDescription);
			Session.WaitForElement (IdeQuery.TextArea);
			TakeScreenShot ("After-Stash");
		}
	}

	public abstract class StashManagerBase : VCSBase
	{
		protected Func<AppQuery, AppQuery> StashEntries = c => c.Window ().Marked (
			"Stash Manager").Children ().TreeView ().Marked ("list").Model ().Children ();
		
		protected void OpenStashManager ()
		{
			Session.ExecuteCommand ("MonoDevelop.VersionControl.Git.Commands.ManageStashes");
			Session.WaitForElement (c => c.Window ().Marked ("Stash Manager"));
			TakeScreenShot ("StashManager-Opened");
		}

		protected void CloseStashManager ()
		{
			Session.ClickElement (c => c.Window ().Marked ("Stash Manager").Children ().Text ("Close"));
			Session.WaitForElement (IdeQuery.TextArea);
			TakeScreenShot ("StashManager-Closed");
		}

		protected void SelectStashEntry (int index = 0)
		{
			Session.WaitForElement (c => StashEntries (c).Index (index));
			Session.SelectElement (c => StashEntries (c).Index (index));
		}

		protected void RemoveStash (int index)
		{
			SelectStashEntry (index);
			TakeScreenShot ("About-To-Click-Remove");
			Session.ClickElement (c => c.Window ().Marked ("Stash Manager").Children ().Button ().Text ("Remove"));
			Session.WaitForElement (c => c.Window ().Marked ("Stash Manager"));
		}

		protected void ApplyAndRemoveStash (int index)
		{
			SelectStashEntry (index);
			TakeScreenShot ("About-To-Click-Apply-and-Remove");
			Session.ClickElement (c => c.Window ().Marked ("Stash Manager").Children ().Button ().Text ("Apply and Remove"));
		}

		protected void ApplyStash (int index)
		{
			SelectStashEntry (index);
			TakeScreenShot ("About-To-Click-Apply");
			Session.ClickElement (c => c.Window ().Marked ("Stash Manager").Children ().Button ().Text ("Apply"));
		}
	}
}

