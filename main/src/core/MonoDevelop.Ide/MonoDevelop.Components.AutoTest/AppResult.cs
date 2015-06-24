﻿//
// AppResult.cs
//
// Author:
//       iain holmes <iain@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Linq;
using System.Collections.ObjectModel;
using MonoDevelop.Components.AutoTest.Results;

namespace MonoDevelop.Components.AutoTest
{
	public abstract class AppResult : MarshalByRefObject
	{
		//public Gtk.Widget ResultWidget { get; private set; }

		public AppResult ParentNode { get; set; }
		public AppResult FirstChild { get; set; }
		public AppResult PreviousSibling { get; set; }
		public AppResult NextSibling { get; set; }

		// Operations
		public abstract AppResult Marked (string mark);
		public abstract AppResult CheckType (Type desiredType);
		public abstract AppResult Text (string text, bool exact);
		public abstract AppResult Model (string column);
		public abstract AppResult Property (string propertyName, object value);
		public abstract List<AppResult> NextSiblings ();

		// Actions
		public abstract bool Select ();
		public abstract bool Click ();
		public abstract bool TypeKey (char key, string state = "");
		public abstract bool TypeKey (string keyString, string state = "");
		public abstract bool EnterText (string text);
		public abstract bool Toggle (bool active);

		// Inspection Operations
		public abstract ObjectProperties Properties ();

		public string SourceQuery { get; set; }

		void AddChildrenToList (List<AppResult> children, AppResult child)
		{
			AppResult node = child.FirstChild;
			children.Add (child);

			while (node != null) {
				AddChildrenToList (children, node);
				node = node.NextSibling;
			}
		}

		public virtual List<AppResult> FlattenChildren ()
		{
			List<AppResult> children = new List<AppResult> ();
			AddChildrenToList (children, FirstChild);

			return children;
		}

		protected object GetPropertyValue (string propertyName, object requestedObject)
		{
			return AutoTestService.CurrentSession.UnsafeSync (delegate {
				PropertyInfo propertyInfo = requestedObject.GetType().GetProperty(propertyName,
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
				if (propertyInfo != null && propertyInfo.CanRead && !propertyInfo.GetIndexParameters ().Any ()) {
					var propertyValue = propertyInfo.GetValue (requestedObject);
					if (propertyValue != null) {
						return propertyValue;
					}
				}

				return null;
			});
		}

		protected ObjectProperties GetProperties (object resultObject)
		{
			var propertiesObject = new ObjectProperties ();
			if (resultObject != null) {
				var properties = resultObject.GetType ().GetProperties (
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				foreach (var property in properties) {
					var value = GetPropertyValue (property.Name, resultObject);
					AppResult result = new ObjectResult (value);
					var gtkWidgetValue = value as Gtk.Widget;
					if (gtkWidgetValue != null) {
						var gtkNotebookValue = value as Gtk.Notebook;
						var gtkTreeviewValue = value as Gtk.TreeView;
						if (gtkNotebookValue != null) {
							result = new GtkNotebookResult (gtkNotebookValue);
						} else if (gtkTreeviewValue != null) {
							result = new GtkTreeModelResult (gtkTreeviewValue, gtkTreeviewValue.Model, 0);
						} else {
							result = new GtkWidgetResult (gtkWidgetValue);
						}
					}
					#if MAC
					var nsObjectValue = value as Foundation.NSObject;
					if (nsObjectValue != null)
							result = new NSObjectResult (nsObjectValue);
					#endif
					
					propertiesObject.Add (property.Name, result, property);
				}
			}

			return propertiesObject;
		}

		protected AppResult MatchProperty (string propertyName, object objectToCompare, object value)
		{
			foreach (var singleProperty in propertyName.Split (new [] { '.' })) {
				objectToCompare = GetPropertyValue (singleProperty, objectToCompare);
			}
			if (objectToCompare != null && value != null &&
				CheckForText (objectToCompare.ToString (), value.ToString (), false)) {
				return this;
			}
			return null;
		}

		protected bool CheckForText (string haystack, string needle, bool exact)
		{
			if (exact) {
				return haystack == needle;
			} else {
				return (haystack.IndexOf (needle, StringComparison.Ordinal) > -1);
			}
		}
	}

	public class ObjectProperties : MarshalByRefObject
	{
		readonly Dictionary<string,AppResult> propertyMap = new Dictionary<string,AppResult> ();

		readonly Dictionary<string,PropertyMetaData> propertyMetaData = new Dictionary<string,PropertyMetaData> ();

		internal ObjectProperties () { }

		internal void Add (string propertyName, AppResult propertyValue, PropertyInfo propertyInfo)
		{
			propertyMap.Add (propertyName, propertyValue);
			propertyMetaData.Add (propertyName, new PropertyMetaData (propertyInfo));
		}

		public ReadOnlyCollection<string> GetPropertyNames ()
		{
			return propertyMap.Keys.ToList ().AsReadOnly ();
		}

		public AppResult this [string propertyName]
		{
			get {
				return propertyMap [propertyName];
			}
		}

		public PropertyMetaData GetMetaData (string propertyName)
		{
			return propertyMetaData [propertyName];
		}
	}

	public class PropertyMetaData : MarshalByRefObject
	{
		readonly PropertyInfo propertyInfo;

		internal PropertyMetaData (PropertyInfo propertyInfo)
		{
			this.propertyInfo = propertyInfo;
		}

		public string Name
		{
			get {
				return propertyInfo.Name;
			}
		}

		public bool CanRead
		{
			get {
				return propertyInfo.CanRead;
			}
		}

		public bool CanWrite
		{
			get {
				return propertyInfo.CanWrite;
			}
		}

		public string PropertyType
		{
			get {
				return propertyInfo.PropertyType.FullName;
			}
		}
	}
}

