//
// AddNuGetDialog.cs
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
using NUnit.Framework;
using MonoDevelop.Components.Commands;

namespace UserInterfaceTests
{
	[TestFixture]
	[Category ("Dialog")]
	[Category ("NuGetDialog")]
	public class AddNuGetDialogTests : GitRepositoryConfigurationBase
	{
		[Test]
		public void AddNuGetDialogTest ()
		{
			var templateOptions = new TemplateSelectionOptions {
				CategoryRoot = OtherCategoryRoot,
				Category = ".NET",
				TemplateKindRoot = GeneralKindRoot,
				TemplateKind = "Console Project"
			};
			CreateBuildProject (templateOptions, EmptyAction);
			TakeScreenShot ("Before-Fail");
			Assert.Fail ();
		}

//		[Test]
//		[Category ("DeleteBranch")]
//		public void CheckConfirmDialog ()
//		{
//			try {
//				TestClone ("git@github.com:mono/jurassic.git");
//				Ide.WaitForSolutionCheckedOut ();
//
//				OpenRepositoryConfiguration ();
//				CreateNewBranch ("new-branch");
//				SelectBranch ("new-branch");
//				SwitchToBranch ("new-branch");
//				Session.ClickElement (c => c.Window ().Marked ("MonoDevelop.VersionControl.Git.GitConfigurationDialog").Children ().Button ().Marked ("buttonRemoveBranch"), false);
//				Session.WaitForElement (c => c.Window ().Children (), 7000);
//				CloseRepositoryConfiguration ();
//			} catch (TimeoutException) {
//				TakeScreenShot ("Timed-Out");
//				throw;
//			}
//		}

		protected override void OnBuildTemplate (int buildTimeoutInSecs = 180)
		{
			Session.Query (c => c.Window ().Children ().Marked ("__gtksharp_50_MonoDevelop_Ide_Gui_Components_ExtensibleTreeView"));
			Session.ExecuteCommand ("MonoDevelop.PackageManagement.Commands.AddNuGetPackages", source: CommandSource.MainMenu);
			WaitForAddButton ();
			TakeScreenShot ("NuGet-Screen");

			Session.EnterText (c => c.Window ().Marked ("Add Packages").Children ().Textfield ().Marked ("search-entry"), "CommandLineParser");
			TakeScreenShot ("CommandLineParser-Found");
			WaitForAddButton (true);

			Session.ToggleElement (c => c.Window ().Marked ("Add Packages").Children ().CheckButton ().Marked ("Show pre-release packages"), true);
			WaitForAddButton (true);
			TakeScreenShot ("Pre-Release-Packages-Shown");
			Session.SelectElement (c => c.Window ().Marked ("Add Packages").Children ().TreeView ().Model ().Children ().Index (2));
			//Session.Query (c => c.Window ().Marked ("Add Packages").Children ().CheckType (typeof (Gtk.Label)));
			//Session.WaitForElement (c => c.Window ().Marked ("Add Packages").Children ().CheckType (typeof (Gtk.Label)).Property ("Text", "1.9.71"));
			TakeScreenShot ("Select-Third element");

			Session.ClickElement (c => c.Window ().Marked ("Add Packages").Children ().Button ().Marked ("Add Package"));
		}

		void WaitForAddButton (bool? enabled = null)
		{
			if (enabled == null)
				Session.WaitForElement (c => c.Window ().Marked ("Add Packages").Children ().Button ().Marked ("Add Package"));
			else
				Session.WaitForElement (c => c.Window ().Marked ("Add Packages").Children ().Button ().Marked ("Add Package").Sensitivity (enabled.Value), 10000);
		}
	}
}

