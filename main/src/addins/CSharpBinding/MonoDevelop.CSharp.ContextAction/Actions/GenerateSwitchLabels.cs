// 
// GenerateSwitchBody.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
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
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Core;
using System.Collections.Generic;
using Mono.TextEditor;
using System.Linq;
using MonoDevelop.Refactoring;

namespace MonoDevelop.CSharp.ContextAction
{
	public class GenerateSwitchLabels : CSharpContextAction
	{
		protected override string GetMenuText (CSharpContext context)
		{
			return GettextCatalog.GetString ("Generate switch labels");
		}

		SwitchStatement GetSwitchStatement (CSharpContext context)
		{
			var switchStatment = context.GetNode<SwitchStatement> ();
			if (switchStatment != null && switchStatment.SwitchSections.Count == 0)
				return switchStatment;
			return null;
		}
		
		protected override bool IsValid (CSharpContext context)
		{
			var switchStatement = GetSwitchStatement (context);
			if (switchStatement == null)
				return false;
			var resolver = context.Resolver;
			var result = resolver.Resolve (switchStatement.Expression.ToString (), new DomLocation (switchStatement.StartLocation.Line, switchStatement.StartLocation.Column));
			if (result == null || result.ResolvedType == null)
				return false;
			var type = context.Document.Dom.GetType (result.ResolvedType);
			
			return type != null && type.ClassType == ClassType.Enum;
		}
		
		protected override void Run (CSharpContext context)
		{
			var switchStatement = GetSwitchStatement (context);
			var resolver = context.Resolver;
			
			var result = resolver.Resolve (switchStatement.Expression.ToString (), new DomLocation (switchStatement.StartLocation.Line, switchStatement.StartLocation.Column));
			var type = context.Document.Dom.GetType (result.ResolvedType);
			
			var target = new TypeReferenceExpression (ShortenTypeName (context.Document, result.ResolvedType));
			foreach (var field in type.Fields) {
				if (!(field.IsLiteral || field.IsConst))
					continue;
				switchStatement.SwitchSections.Add (new SwitchSection () {
					CaseLabels = {
						new CaseLabel (new MemberReferenceExpression ( target.Clone (), field.Name))
					},
					Statements = {
						new BreakStatement ()
					}
				});
			}
			
			switchStatement.SwitchSections.Add (new SwitchSection () {
				CaseLabels = {
					new CaseLabel ()
				},
				Statements = {
					new ThrowStatement (new ObjectCreateExpression (ShortenTypeName (context.Document, "System.ArgumentOutOfRangeException")))
				}
			});
			
			context.Do (switchStatement.Replace (context.Document, switchStatement));
		}
	}
}

