﻿//
// NuGetDialogTests.cs
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
using System.IO;
using System.Xml;
using System.Linq;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Core;

namespace UserInterfaceTests
{
	[TestFixture]
	[Category ("Dialog")]
	[Category ("PackagesDialog")]
	public class NuGetDialogTests : CreateBuildTemplatesTestBase
	{
		[Test]
		public void AddPackagesTest ()
		{
			CreateProject ();
			NuGetController.AddPackage (new NuGetPackageOptions {
				PackageName = "CommandLineParser",
				Version = "2.0.99-alpha",
				IsPreRelease = true
			});
		}

		[Test]
		public void TestReadmeTxtOpens ()
		{
			CreateProject ();
			NuGetController.AddPackage (new NuGetPackageOptions {
				PackageName = "RestSharp",
				Version = "105.0.1",
				IsPreRelease = true
			});
			Session.WaitForElement (c => c.Window ().Marked ("MonoDevelop.Ide.Gui.DefaultWorkbench").Property ("TabControl.CurrentTab.Text", "readme.txt"));
		}

		[Test]
		public void TestReadmeTxtUpgradeOpens ()
		{
			CreateProject ();
			NuGetController.AddPackage (new NuGetPackageOptions {
				PackageName = "RestSharp",
				Version = "105.0.1",
				IsPreRelease = true
			}, TakeScreenShot);
			Session.WaitForElement (c => c.Window ().Marked ("MonoDevelop.Ide.Gui.DefaultWorkbench").Property ("TabControl.CurrentTab.Text", "readme.txt"));
			Session.ExecuteCommand (FileCommands.CloseFile);
			Session.WaitForElement (IdeQuery.TextArea);
			NuGetController.UpdatePackage (new NuGetPackageOptions {
				PackageName = "RestSharp",
				Version = "105.1.0",
				IsPreRelease = true
			}, TakeScreenShot);
			Session.WaitForElement (c => c.Window ().Marked ("MonoDevelop.Ide.Gui.DefaultWorkbench").Property ("TabControl.CurrentTab.Text", "readme.txt"));
		}

		[Test, Category ("LocalPreserve")]
		public void TestLocalCopyPreservedUpdate ()
		{
			var templateOptions = new TemplateSelectionOptions {
				CategoryRoot = OtherCategoryRoot,
				Category = ".NET",
				TemplateKindRoot = GeneralKindRoot,
				TemplateKind = "Console Project"
			};
			var projectDetails = new ProjectDetails (templateOptions);
			CreateProject (templateOptions, projectDetails);
			NuGetController.AddPackage (new NuGetPackageOptions {
				PackageName = "CommandLineParser",
				Version = "1.9.7",
				IsPreRelease = false
			}, TakeScreenShot);

			string solutionFolder = GetSolutionDirectory ();
			string solutionPath = Path.Combine (solutionFolder, projectDetails.SolutionName+".sln");
			var projectPath = Path.Combine (solutionFolder, projectDetails.ProjectName, projectDetails.ProjectName + ".csproj");
			Assert.IsTrue (File.Exists (projectPath));

			TakeScreenShot ("About-To-Close-Solution");
			Session.ExecuteCommand (FileCommands.CloseWorkspace);
			TakeScreenShot ("Closed-Solution");

			AddOrCheckLocalCopy (projectPath, true);

			Session.GlobalInvoke ("MonoDevelop.Ide.IdeApp.Workspace.OpenWorkspaceItem", new FilePath (solutionPath), true);
			TakeScreenShot ("Solution-Opened");
			Ide.WaitForPackageUpdate ();
			TakeScreenShot ("Solution-Ready");
			Session.WaitForElement (IdeQuery.TextArea);

			NuGetController.UpdateAllNuGetPackages (TakeScreenShot);

			AddOrCheckLocalCopy (projectPath, false);
		}

		void AddOrCheckLocalCopy (string projectPath, bool addLocalCopy)
		{
			using (var stream = new FileStream (projectPath, FileMode.Open, FileAccess.ReadWrite)) {
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(stream);
				var ns = "http://schemas.microsoft.com/developer/msbuild/2003";
				XmlNamespaceManager xnManager = new XmlNamespaceManager(xmlDoc.NameTable);
				xnManager.AddNamespace("ui", ns);
				XmlNode root = xmlDoc.DocumentElement; 
				var uitest = root.SelectSingleNode ("//ui:Reference[@Include=\"CommandLine\"]", xnManager);
				Assert.IsNotNull (uitest, "Cannot find CommandLine package reference in file: "+projectPath);
				var privateUITestNode = uitest.SelectSingleNode ("./ui:Private", xnManager);

				if (addLocalCopy) {
					Assert.IsNull (privateUITestNode, uitest.InnerXml, "CommandLine package is already set to 'No Local Copy'");
					var privateNode = xmlDoc.CreateElement ("Private", ns);
					privateNode.InnerText = "False";
					uitest.AppendChild (privateNode);
					stream.SetLength (0);
					xmlDoc.Save (stream);
					stream.Flush ();
				} else {
					Assert.IsNotNull (privateUITestNode, "Cannot find CommandLine package with 'No Local Copy' set");
					Assert.AreEqual (privateUITestNode.InnerText, "False");
				}
				stream.Close ();
			}
		}

		ProjectDetails CreateProject (TemplateSelectionOptions templateOptions = null, ProjectDetails projectDetails = null)
		{
			templateOptions = templateOptions ?? new TemplateSelectionOptions {
				CategoryRoot = OtherCategoryRoot,
				Category = ".NET",
				TemplateKindRoot = GeneralKindRoot,
				TemplateKind = "Console Project"
			};
			projectDetails = projectDetails ?? new ProjectDetails (templateOptions);
			CreateProject (templateOptions,
					projectDetails,
				new GitOptions { UseGit = true, UseGitIgnore = true});
			Session.WaitForElement (IdeQuery.TextArea);
			FoldersToClean.Add (projectDetails.SolutionLocation);
			return projectDetails;
		}
	}
}

