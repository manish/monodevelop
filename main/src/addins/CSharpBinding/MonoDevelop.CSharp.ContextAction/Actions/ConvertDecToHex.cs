// 
// ConvertDecToHexQuickFix.cs
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
using MonoDevelop.Projects.Dom;
using ICSharpCode.NRefactory.CSharp;
using MonoDevelop.Core;

namespace MonoDevelop.CSharp.ContextAction
{
	public class ConvertDecToHex : CSharpContextAction
	{
		protected override string GetMenuText (CSharpContext context)
		{
			return GettextCatalog.GetString ("Convert dec to hex.");
		}
		
		protected override void Run (CSharpContext context)
		{
			var pExpr = context.GetNode<PrimitiveExpression> ();
			context.Do (pExpr.Replace (context.Document, string.Format ("0x{0:x}", pExpr.Value)));
		}
		
		protected override bool IsValid (CSharpContext context)
		{
			var pExpr = context.GetNode<PrimitiveExpression> ();
			if (pExpr == null || pExpr.LiteralValue.ToUpper ().StartsWith ("0X"))
				return false;
			return (pExpr.Value is int) || (pExpr.Value is long) || (pExpr.Value is short) || (pExpr.Value is sbyte) ||
				(pExpr.Value is uint) || (pExpr.Value is ulong) || (pExpr.Value is ushort) || (pExpr.Value is byte);
		}
	}
	
}

