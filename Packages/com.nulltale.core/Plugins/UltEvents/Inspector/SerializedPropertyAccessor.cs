// Serialized Property Accessor // Copyright 2019 Kybernetik //
// This script is shared by Inspector Gadgets and UltEvents.

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if INSPECTOR_GADGETS
namespace InspectorGadgets
#else
namespace UltEvents.Editor
#endif
{
    /// <summary>[Editor-Only]
    /// A wrapper for <see cref="SerializedProperty"/> that allows you to access the underlying values and fields.
    /// </summary>
    public class SerializedPropertyAccessor
    {
        /************************************************************************************************************************/
        #region Public Static API
        /************************************************************************************************************************/
        #region Get Value
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Gets the value of the specified <see cref="SerializedProperty"/>.</summary>
        public static object GetValue(SerializedProperty property, object targetObject)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean: return property.boolValue;
                case SerializedPropertyType.Float: return property.floatValue;
                case SerializedPropertyType.Integer: return property.intValue;
                case SerializedPropertyType.String: return property.stringValue;

                case SerializedPropertyType.Vector2: return property.vector2Value;
                case SerializedPropertyType.Vector3: return property.vector3Value;
                case SerializedPropertyType.Vector4: return property.vector4Value;

                case SerializedPropertyType.Quaternion: return property.quaternionValue;
                case SerializedPropertyType.Color: return property.colorValue;
                case SerializedPropertyType.AnimationCurve: return property.animationCurveValue;

                case SerializedPropertyType.Rect: return property.rectValue;
                case SerializedPropertyType.Bounds: return property.boundsValue;

#if UNITY_2017_3_OR_NEWER
                case SerializedPropertyType.Vector2Int: return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int: return property.vector3IntValue;
                case SerializedPropertyType.RectInt: return property.rectIntValue;
                case SerializedPropertyType.BoundsInt: return property.boundsIntValue;
#endif

                case SerializedPropertyType.ObjectReference: return property.objectReferenceValue;
                case SerializedPropertyType.ExposedReference: return property.exposedReferenceValue;

                case SerializedPropertyType.ArraySize: return property.intValue;
                case SerializedPropertyType.FixedBufferSize: return property.fixedBufferSize;

                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Character:
                default:
                    var accessor = GetAccessor(property);
                    //if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint) Debug.Log(accessor);
                    if (accessor != null)
                        return accessor.GetValue(targetObject);
                    else
                        return null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Gets the value of the <see cref="SerializedProperty"/>.</summary>
        public static object GetValue(SerializedProperty property)
        {
            return GetValue(property, property.serializedObject.targetObject);
        }

        /// <summary>[Editor-Only] Gets the value of the <see cref="SerializedProperty"/>.</summary>
        public static T GetValue<T>(SerializedProperty property)
        {
            return (T)GetValue(property);
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Gets the value of the <see cref="SerializedProperty"/> for each of its target objects.</summary>
        public static T[] GetValues<T>(SerializedProperty property)
        {
            try
            {
                var targetObjects = property.serializedObject.targetObjects;
                var values = new T[targetObjects.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = (T)GetValue(property, targetObjects[i]);
                }

                return values;
            }
            catch
            {
                return null;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Set Value
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Sets the value of the specified <see cref="SerializedProperty"/>.</summary>
        public static void SetValue(SerializedProperty property, object targetObject, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean: property.boolValue = (bool)value; break;
                case SerializedPropertyType.Float: property.floatValue = (float)value; break;
                case SerializedPropertyType.Integer: property.intValue = (int)value; break;
                case SerializedPropertyType.String: property.stringValue = (string)value; break;

                case SerializedPropertyType.Vector2: property.vector2Value = (Vector2)value; break;
                case SerializedPropertyType.Vector3: property.vector3Value = (Vector3)value; break;
                case SerializedPropertyType.Vector4: property.vector4Value = (Vector4)value; break;

                case SerializedPropertyType.Quaternion: property.quaternionValue = (Quaternion)value; break;
                case SerializedPropertyType.Color: property.colorValue = (Color)value; break;
                case SerializedPropertyType.AnimationCurve: property.animationCurveValue = (AnimationCurve)value; break;

                case SerializedPropertyType.Rect: property.rectValue = (Rect)value; break;
                case SerializedPropertyType.Bounds: property.boundsValue = (Bounds)value; break;

#if UNITY_2017_3_OR_NEWER
                case SerializedPropertyType.Vector2Int: property.vector2IntValue = (Vector2Int)value; break;
                case SerializedPropertyType.Vector3Int: property.vector3IntValue = (Vector3Int)value; break;
                case SerializedPropertyType.RectInt: property.rectIntValue = (RectInt)value; break;
                case SerializedPropertyType.BoundsInt: property.boundsIntValue = (BoundsInt)value; break;
#endif

                case SerializedPropertyType.ObjectReference: property.objectReferenceValue = (Object)value; break;
                case SerializedPropertyType.ExposedReference: property.exposedReferenceValue = (Object)value; break;

                case SerializedPropertyType.ArraySize: property.intValue = (int)value; break;

                case SerializedPropertyType.FixedBufferSize:
                    throw new InvalidOperationException("SetValue failed: " + Names.SerializedPropertyFixedBufferSize + " is read-only.");

                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Character:
                default:
                    var accessor = GetAccessor(property);
                    if (accessor != null)
                        accessor.SetValue(targetObject, value);
                    break;
            }
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Sets the value of the <see cref="SerializedProperty"/>.</summary>
        public static void SetValue(SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean: property.boolValue = (bool)value; break;
                case SerializedPropertyType.Float: property.floatValue = (float)value; break;
                case SerializedPropertyType.Integer: property.intValue = (int)value; break;
                case SerializedPropertyType.String: property.stringValue = (string)value; break;

                case SerializedPropertyType.Vector2: property.vector2Value = (Vector2)value; break;
                case SerializedPropertyType.Vector3: property.vector3Value = (Vector3)value; break;
                case SerializedPropertyType.Vector4: property.vector4Value = (Vector4)value; break;

                case SerializedPropertyType.Quaternion: property.quaternionValue = (Quaternion)value; break;
                case SerializedPropertyType.Color: property.colorValue = (Color)value; break;
                case SerializedPropertyType.AnimationCurve: property.animationCurveValue = (AnimationCurve)value; break;

                case SerializedPropertyType.Rect: property.rectValue = (Rect)value; break;
                case SerializedPropertyType.Bounds: property.boundsValue = (Bounds)value; break;

#if UNITY_2017_3_OR_NEWER
                case SerializedPropertyType.Vector2Int: property.vector2IntValue = (Vector2Int)value; break;
                case SerializedPropertyType.Vector3Int: property.vector3IntValue = (Vector3Int)value; break;
                case SerializedPropertyType.RectInt: property.rectIntValue = (RectInt)value; break;
                case SerializedPropertyType.BoundsInt: property.boundsIntValue = (BoundsInt)value; break;
#endif

                case SerializedPropertyType.ObjectReference: property.objectReferenceValue = (Object)value; break;
                case SerializedPropertyType.ExposedReference: property.exposedReferenceValue = (Object)value; break;

                case SerializedPropertyType.ArraySize: property.intValue = (int)value; break;

                case SerializedPropertyType.FixedBufferSize:
                    throw new InvalidOperationException("SetValue failed: " + Names.SerializedPropertyFixedBufferSize + " is read-only.");

                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Character:
                default:
                    var accessor = GetAccessor(property);
                    if (accessor != null)
                    {
                        var targets = property.serializedObject.targetObjects;
                        for (int i = 0; i < targets.Length; i++)
                        {
                            accessor.SetValue(targets[i], value);
                        }
                    }
                    break;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary>
        /// Calls the specified 'method' for each of the underlying values of the 'property' (in case it represents
        /// multiple selected objects) and records an undo step for any modifications made.
        /// </summary>
        public static void ModifyValues<T>(SerializedProperty property, Action<T> method, string undoName)
        {
            RecordUndo(property, undoName);

            var values = GetValues<T>(property);
            for (int i = 0; i < values.Length; i++)
                method(values[i]);

            OnPropertyChanged(property);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Records the state of the specified 'property' so it can be undone.
        /// </summary>
        public static void RecordUndo(SerializedProperty property, string undoName)
        {
            Undo.RecordObjects(property.serializedObject.targetObjects, undoName);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Updates the specified 'property' and marks its target objects as dirty so any changes to a prefab will be saved.
        /// </summary>
        public static void OnPropertyChanged(SerializedProperty property)
        {
            var targets = property.serializedObject.targetObjects;

            // If this change is made to a prefab, this makes sure that any instances in the scene will be updated.
            for (int i = 0; i < targets.Length; i++)
            {
                EditorUtility.SetDirty(targets[i]);
            }

            property.serializedObject.Update();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns the <see cref="SerializedPropertyType"/> that represents fields of the specified 'type'.
        /// </summary>
        public static SerializedPropertyType GetPropertyType(Type type)
        {
            // Primitives.

            if (type == typeof(bool))
                return SerializedPropertyType.Boolean;

            if (type == typeof(int))
                return SerializedPropertyType.Integer;

            if (type == typeof(float))
                return SerializedPropertyType.Float;

            if (type == typeof(string))
                return SerializedPropertyType.String;

            if (type == typeof(LayerMask))
                return SerializedPropertyType.LayerMask;

            // Vectors.

            if (type == typeof(Vector2))
                return SerializedPropertyType.Vector2;
            if (type == typeof(Vector3))
                return SerializedPropertyType.Vector3;
            if (type == typeof(Vector4))
                return SerializedPropertyType.Vector4;

            if (type == typeof(Quaternion))
                return SerializedPropertyType.Quaternion;

            // Other.

            if (type == typeof(Color) || type == typeof(Color32))
                return SerializedPropertyType.Color;
            if (type == typeof(Gradient))
                return SerializedPropertyType.Gradient;

            if (type == typeof(Rect))
                return SerializedPropertyType.Rect;
            if (type == typeof(Bounds))
                return SerializedPropertyType.Bounds;

            if (type == typeof(AnimationCurve))
                return SerializedPropertyType.AnimationCurve;

            // Int Variants.

#if UNITY_2017_3_OR_NEWER
            if (type == typeof(Vector2Int))
                return SerializedPropertyType.Vector2Int;
            if (type == typeof(Vector3Int))
                return SerializedPropertyType.Vector3Int;
            if (type == typeof(RectInt))
                return SerializedPropertyType.RectInt;
            if (type == typeof(BoundsInt))
                return SerializedPropertyType.BoundsInt;
#endif

            // Special.

            if (typeof(Object).IsAssignableFrom(type))
                return SerializedPropertyType.ObjectReference;

            if (type.IsEnum)
                return SerializedPropertyType.Enum;

            return SerializedPropertyType.Generic;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Accessor Pool
        /************************************************************************************************************************/

        private static readonly Dictionary<Type, Dictionary<string, SerializedPropertyAccessor>>
            TypeToPathToAccessor = new Dictionary<Type, Dictionary<string, SerializedPropertyAccessor>>();

        /************************************************************************************************************************/

        /// <summary>
        /// Gets a <see cref="SerializedPropertyAccessor"/> that can be used to access the details of the specified 'property'.
        /// </summary>
        public static SerializedPropertyAccessor GetAccessor(SerializedProperty property)
        {
            var propertyPath = property.propertyPath;
            object targetObject = property.serializedObject.targetObject;

            var type = targetObject.GetType();
            return GetAccessor(propertyPath, ref type);
        }

        /************************************************************************************************************************/

        private static SerializedPropertyAccessor GetAccessor(string propertyPath, ref Type propertyType)
        {
            Dictionary<string, SerializedPropertyAccessor> pathToAccessor;
            if (!TypeToPathToAccessor.TryGetValue(propertyType, out pathToAccessor))
            {
                pathToAccessor = new Dictionary<string, SerializedPropertyAccessor>();
                TypeToPathToAccessor.Add(propertyType, pathToAccessor);
            }

            SerializedPropertyAccessor accessor;
            if (!pathToAccessor.TryGetValue(propertyPath, out accessor))
            {
                var nameStartIndex = propertyPath.LastIndexOf('.');
                string elementName;
                SerializedPropertyAccessor parent;

                // Array.
                if (nameStartIndex > 6 && nameStartIndex < propertyPath.Length - 7 && string.Compare(propertyPath, nameStartIndex - 6, ".Array.data[", 0, 12) == 0)
                {
                    var index = int.Parse(propertyPath.Substring(nameStartIndex + 6, propertyPath.Length - nameStartIndex - 7));

                    var nameEndIndex = nameStartIndex - 6;
                    nameStartIndex = propertyPath.LastIndexOf('.', nameEndIndex - 1);

                    elementName = propertyPath.Substring(nameStartIndex + 1, nameEndIndex - nameStartIndex - 1);

                    FieldInfo field;
                    if (nameStartIndex >= 0)
                    {
                        parent = GetAccessor(propertyPath.Substring(0, nameStartIndex), ref propertyType);
                        field = GetField(parent != null ? parent.FieldType : propertyType, elementName);
                    }
                    else
                    {
                        parent = null;
                        field = GetField(propertyType, elementName);
                    }

                    if (field != null)
                        accessor = new SerializedPropertyArrayAccessor(parent, field, index);
                    else
                        accessor = null;
                }
                else// Single.
                {
                    if (nameStartIndex >= 0)
                    {
                        elementName = propertyPath.Substring(nameStartIndex + 1);
                        parent = GetAccessor(propertyPath.Substring(0, nameStartIndex), ref propertyType);
                    }
                    else
                    {
                        elementName = propertyPath;
                        parent = null;
                    }

                    var field = GetField(parent != null ? parent.FieldType : propertyType, elementName);

                    if (field != null)
                        accessor = new SerializedPropertyAccessor(parent, field);
                    else
                        accessor = null;
                }

                pathToAccessor.Add(propertyPath, accessor);
            }

            if (accessor != null)
                propertyType = accessor.Field.FieldType;

            return accessor;
        }

        /************************************************************************************************************************/

        private static FieldInfo GetField(Type declaringType, string name)
        {
            while (true)
            {
                var field = declaringType.GetField(name, UltEventUtils.InstanceBindings);
                if (field != null)
                    return field;

                declaringType = declaringType.BaseType;
                if (declaringType == null)
                    return null;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Instances
        /************************************************************************************************************************/

        /// <summary>The accessor for the field which this accessor is nested inside.</summary>
        public readonly SerializedPropertyAccessor Parent;

        /// <summary>The field wrapped by this accessor.</summary>
        public readonly FieldInfo Field;

        /// <summary>The type of the wrapped <see cref="Field"/>.</summary>
        public readonly Type FieldType;

        /************************************************************************************************************************/

        private SerializedPropertyAccessor(SerializedPropertyAccessor parent, FieldInfo field)
            : this(parent, field, field.FieldType)
        { }

        /// <summary>Constructs a new <see cref="SerializedPropertyAccessor"/>.</summary>
        protected SerializedPropertyAccessor(SerializedPropertyAccessor parent, FieldInfo field, Type fieldType)
        {
            Parent = parent;
            Field = field;
            FieldType = fieldType;
        }

        /************************************************************************************************************************/

        /// <summary>Gets the value of the from the <see cref="Parent"/> (if there is one), then uses it to get the value from the <see cref="Field"/>.</summary>
        public virtual object GetValue(object obj)
        {
            if (ReferenceEquals(obj, null))
                return null;

            if (Parent != null)
                obj = Parent.GetValue(obj);

            return Field.GetValue(obj);
        }

        /************************************************************************************************************************/

        /// <summary>Gets the value of the from the <see cref="Parent"/> (if there is one), then uses it to set the value from the <see cref="Field"/>.</summary>
        public virtual void SetValue(object obj, object value)
        {
            if (ReferenceEquals(obj, null))
                return;

            if (Parent != null)
                obj = Parent.GetValue(obj);

            Field.SetValue(obj, value);
        }

        /************************************************************************************************************************/

        /// <summary>Returns a description of this accessor's path.</summary>
        public override string ToString()
        {
            if (Parent != null)
                return Parent + "." + Field.Name;
            else
                return Field.Name;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }

    internal class SerializedPropertyArrayAccessor : SerializedPropertyAccessor
    {
        /************************************************************************************************************************/

        public readonly int ElementIndex;

        /************************************************************************************************************************/

        public SerializedPropertyArrayAccessor(SerializedPropertyAccessor parent, FieldInfo field, int elementIndex)
            : base(parent, field, GetElementType(field.FieldType))
        {
            ElementIndex = elementIndex;
        }

        /************************************************************************************************************************/

        private static Type GetElementType(Type fieldType)
        {
            if (fieldType.IsArray)
            {
                return fieldType.GetElementType();
            }
            else if (fieldType.IsGenericType)
            {
                return fieldType.GetGenericArguments()[0];
            }
            else
            {
                Debug.LogWarning(Names.SerializedPropertyArrayAccessor + ": unable to determine element type for " + fieldType);
                return fieldType;
            }
        }

        /************************************************************************************************************************/

        public override object GetValue(object obj)
        {
            var collection = base.GetValue(obj);
            if (collection == null)
                return null;

            var list = collection as IList;
            if (list != null)
            {
                if (ElementIndex < list.Count)
                    return list[ElementIndex];
                else
                    return null;
            }

            var enumerator = ((IEnumerable)collection).GetEnumerator();

            for (int i = 0; i < ElementIndex; i++)
            {
                if (!enumerator.MoveNext())
                    return null;
            }

            return enumerator.Current;
        }

        /************************************************************************************************************************/

        public override void SetValue(object obj, object value)
        {
            var collection = base.GetValue(obj);
            if (collection == null)
                return;

            var list = collection as IList;
            if (list != null)
            {
                if (ElementIndex < list.Count)
                    list[ElementIndex] = value;

                return;
            }

            throw new InvalidOperationException("SetValue failed: " + Field + " doesn't implement IList.");
        }

        /************************************************************************************************************************/

        public override string ToString()
        {
            return base.ToString() + "[" + ElementIndex + "]";
        }

        /************************************************************************************************************************/
    }
}

#endif