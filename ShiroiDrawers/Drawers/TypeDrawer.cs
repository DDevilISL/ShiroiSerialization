﻿using System;
using System.Reflection;
using UnityEngine;

namespace Shiroi.Drawing.Drawers {
    public delegate void Setter(object value);

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenericDrawerAttribute : Attribute {
        private readonly Type supportedType;

        public Type SupportedType {
            get {
                return supportedType;
            }
        }

        public GenericDrawerAttribute(Type supportedType) {
            this.supportedType = supportedType;
        }
    }

    public abstract class TypeDrawer : IComparable<TypeDrawer> {
        public abstract bool Supports(Type type);

        public abstract void Draw(Rect rect, GUIContent label, object value, Type valueType, FieldInfo fieldInfo, Setter setter);

        public virtual int GetPriority() {
            return 0;
        }

        public virtual uint GetTotalLines() {
            return 1;
        }

        public int CompareTo(TypeDrawer other) {
            return GetPriority().CompareTo(other.GetPriority());
        }
    }

    public abstract class TypeDrawer<T> : TypeDrawer {
        private readonly Type supportedType;

        public override void Draw(Rect rect, GUIContent label, object value, Type valueType, FieldInfo fieldInfo, Setter setter) {
            T finalV;
            if (value == null || value is T) {
                finalV = (T) value;
            } else {
                var msg = string.Format(
                    "[{2}] Expected an object of type {0} but got {1}! Using default value...",
                    supportedType,
                    value,
                    GetType().Name);
                Debug.LogWarning(msg);
                finalV = default(T);
            }
            Draw(rect, label, finalV, valueType, fieldInfo, setter);
        }

        protected TypeDrawer() {
            supportedType = typeof(T);
        }

        public override bool Supports(Type type) {
            return supportedType.IsAssignableFrom(type);
        }

        public abstract void Draw(Rect rect, GUIContent name, T value, Type valueType, FieldInfo fieldInfo, Setter setter);
    }
}