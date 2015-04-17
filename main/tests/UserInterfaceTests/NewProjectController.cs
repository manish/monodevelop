﻿//
// NewProjectController.cs
//
// Author:
//       Manish Sinha <manish.sinha@xamarin.com>
//
// Copyright (c) 2015 
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
using MonoDevelop.Components.AutoTest;
using MonoDevelop.Ide.Commands;
using NUnit.Framework;
using System.Threading;

namespace UserInterfaceTests
{
	public class NewProjectController
	{
		static AutoTestClientSession Session {
			get { return TestService.Session; }
		}

		public void Open ()
		{
			Session.ExecuteCommand (FileCommands.NewProject);
			Session.WaitForElement (c => c.Window ().Marked ("MonoDevelop.Ide.Projects.GtkNewProjectDialogBackend"));
		}

		public bool SelectTemplateType (string categoryRoot, string category)
		{
			Session.WaitForElement (c => c.TreeView ().Marked ("templateCategoriesTreeView"));
			return Session.SelectElement (c => c.TreeView ().Marked ("templateCategoriesTreeView").Model ("templateCategoriesListStore__Name").Contains (categoryRoot).NextSiblings ().Text (category));
		}

		public bool SelectTemplate (string kindRoot, string kind)
		{
			Session.WaitForElement (c => c.TreeView ().Marked ("templatesTreeView"));
			return Session.SelectElement (c => c.TreeView ().Marked ("templatesTreeView").Model ("templateListStore__Name").Contains (kindRoot).NextSiblings ().Text (kind));
		}

		public bool Next ()
		{
			Session.WaitForElement (c => c.Button ().Marked ("nextButton"));
			return Session.ClickElement (c => c.Button ().Marked ("nextButton"));
		}

		public bool Previous ()
		{
			Session.WaitForElement (c => c.Button ().Marked ("previousButton"));
			return Session.ClickElement (c => c.Button ().Marked ("previousButton"));
		}

		public bool SetProjectName (string projectName)
		{
			Session.WaitForElement (c => c.Textfield ().Marked ("projectNameTextBox"));
			return Session.EnterText (c => c.Textfield ().Marked ("projectNameTextBox"), projectName);
		}

		public bool SetSolutionName (string solutionName)
		{
			Session.WaitForElement (c => c.Textfield ().Marked ("solutionNameTextBox"));
			return Session.EnterText (c => c.Textfield ().Marked ("solutionNameTextBox"), solutionName);
		}

		public bool SetSolutionLocation (string solutionLocation)
		{
			Session.WaitForElement (c => c.Textfield ().Marked ("locationTextBox"));
			return Session.EnterText (c => c.Textfield ().Marked ("locationTextBox"), solutionLocation);
		}

		public bool CreateProjectInSolutionDirectory (bool projectInSolution)
		{
			return SetCheckBox ("createProjectWithinSolutionDirectoryCheckBox", projectInSolution);
		}

		public bool UseGit (bool useGit, bool useGitIgnore = true)
		{
			var gitSelectionSuccess = SetCheckBox ("useGitCheckBox", useGit);
			if (gitSelectionSuccess && useGit)
				return gitSelectionSuccess && SetCheckBox ("createGitIgnoreFileCheckBox", useGitIgnore);
			return gitSelectionSuccess;
		}

		public bool SetCheckBox (string checkBoxName, bool active)
		{
			Session.WaitForElement (c => c.CheckButton ().Marked (checkBoxName));
			return Session.ToggleElement (c => c.CheckButton ().Marked (checkBoxName), active);
		}
			
		public bool GetSensitivity (string widgetName)
		{
			AppResult[] results = Session.Query (c => c.Marked (widgetName).Sensitivity (true));
			return results.Length > 0;
		}
	}
}

