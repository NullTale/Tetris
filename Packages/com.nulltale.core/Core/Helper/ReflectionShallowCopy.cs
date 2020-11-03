/*
Copyright (c) 2010 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

using System;
using System.Reflection;


namespace Utilities
{
	/// <summary>
	/// Utility class that handles various
	/// functions dealing with reflection.
	/// </summary>
	public static class Reflection
	{

		/// <summary>
		/// Makes a shallow copy of the object
		/// </summary>
		/// <param name="Object">Object to copy</param>
		/// <param name="SimpleTypesOnly">If true, it only copies simple types (no classes, only items like int, string, etc.), false copies everything.</param>
		/// <returns>A copy of the object</returns>
		public static object MakeShallowCopy(object Object, bool SimpleTypesOnly)
		{
			try
			{
				Type ObjectType = Object.GetType();
				PropertyInfo[] Properties = ObjectType.GetProperties();
				FieldInfo[] Fields = ObjectType.GetFields();
				object ClassInstance = Activator.CreateInstance(ObjectType);

				foreach (PropertyInfo Property in Properties)
				{
					try
					{
						if (SimpleTypesOnly)
						{
							SetPropertyifSimpleType(Property, ClassInstance, Object);
						}
						else
						{
							SetProperty(Property, ClassInstance, Object);
						}
					}
					catch { }
				}

				foreach (FieldInfo Field in Fields)
				{
					try
					{
						if (SimpleTypesOnly)
						{
							SetFieldifSimpleType(Field, ClassInstance, Object);
						}
						else
						{
							SetField(Field, ClassInstance, Object);
						}
					}
					catch { }
				}

				return ClassInstance;
			}
			catch { throw; }
		}

		/// <summary>
		/// Copies a field value
		/// </summary>
		/// <param name="Field">Field object</param>
		/// <param name="ClassInstance">Class to copy to</param>
		/// <param name="Object">Class to copy from</param>
		private static void SetField(FieldInfo Field, object ClassInstance, object Object)
		{
			try
			{
				SetField(Field, Field, ClassInstance, Object);
			}
			catch { }
		}

		/// <summary>
		/// Copies a field value
		/// </summary>
		/// <param name="Field">Field object</param>
		/// <param name="ClassInstance">Class to copy to</param>
		/// <param name="Object">Class to copy from</param>
		private static void SetFieldifSimpleType(FieldInfo Field, object ClassInstance, object Object)
		{
			try
			{
				SetFieldifSimpleType(Field, Field, ClassInstance, Object);
			}
			catch { }
		}

		/// <summary>
		/// Copies a field value
		/// </summary>
		/// <param name="ChildField">Child field object</param>
		/// <param name="Field">Field object</param>
		/// <param name="ClassInstance">Class to copy to</param>
		/// <param name="Object">Class to copy from</param>
		private static void SetField(FieldInfo ChildField, FieldInfo Field, object ClassInstance, object Object)
		{
			try
			{
				if (Field.IsPublic && ChildField.IsPublic)
				{
					ChildField.SetValue(ClassInstance, Field.GetValue(Object));
				}
			}
			catch { }
		}

		/// <summary>
		/// Copies a field value
		/// </summary>
		/// <param name="ChildField">Child field object</param>
		/// <param name="Field">Field object</param>
		/// <param name="ClassInstance">Class to copy to</param>
		/// <param name="Object">Class to copy from</param>
		private static void SetFieldifSimpleType(FieldInfo ChildField, FieldInfo Field, object ClassInstance, object Object)
		{
			try
			{
				Type FieldType = Field.FieldType;
				if (Field.FieldType.FullName.StartsWith("System.Collections.Generic.List", StringComparison.CurrentCultureIgnoreCase))
				{
					FieldType = Field.FieldType.GetGenericArguments()[0];
				}

				if (FieldType.FullName.StartsWith("System"))
				{
					SetField(ChildField, Field, ClassInstance, Object);
				}
			}
			catch { throw; }
		}

		/// <summary>
		/// Copies a property value
		/// </summary>
		/// <param name="Property">Property object</param>
		/// <param name="ClassInstance">Class to copy to</param>
		/// <param name="Object">Class to copy from</param>
		private static void SetPropertyifSimpleType(PropertyInfo Property, object ClassInstance, object Object)
		{
			try
			{
				SetPropertyifSimpleType(Property, Property, ClassInstance, Object);
			}
			catch { }
		}

		/// <summary>
		/// Copies a property value
		/// </summary>
		/// <param name="Property">Property object</param>
		/// <param name="ClassInstance">Class to copy to</param>
		/// <param name="Object">Class to copy from</param>
		private static void SetProperty(PropertyInfo Property, object ClassInstance, object Object)
		{
			try
			{
				SetProperty(Property, Property, ClassInstance, Object);
			}
			catch { }
		}

		/// <summary>
		/// Copies a property value
		/// </summary>
		/// <param name="ChildProperty">Child property object</param>
		/// <param name="Property">Property object</param>
		/// <param name="ClassInstance">Class to copy to</param>
		/// <param name="Object">Class to copy from</param>
		private static void SetProperty(PropertyInfo ChildProperty, PropertyInfo Property, object ClassInstance, object Object)
		{
			try
			{
				if (ChildProperty.GetSetMethod() != null && Property.GetGetMethod() != null)
				{
					ChildProperty.SetValue(ClassInstance, Property.GetValue(Object, null), null);
				}
			}
			catch { }
		}

		/// <summary>
		/// Copies a property value
		/// </summary>
		/// <param name="ChildProperty">Child property object</param>
		/// <param name="Property">Property object</param>
		/// <param name="ClassInstance">Class to copy to</param>
		/// <param name="Object">Class to copy from</param>
		private static void SetPropertyifSimpleType(PropertyInfo ChildProperty, PropertyInfo Property, object ClassInstance, object Object)
		{
			try
			{
				Type PropertyType = Property.PropertyType;
				if (Property.PropertyType.FullName.StartsWith("System.Collections.Generic.List", StringComparison.CurrentCultureIgnoreCase))
				{
					PropertyType = Property.PropertyType.GetGenericArguments()[0];
				}

				if (PropertyType.FullName.StartsWith("System"))
				{
					SetProperty(ChildProperty, Property, ClassInstance, Object);
				}
			}
			catch { throw; }
		}

	}
}