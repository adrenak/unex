using System;
using UnityEngine;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace UniPrep.Extensions {
    public static class GenericExtensions {
        public static T[] SubArray<T>(this T[] _array, int _index, int _length) {
            T[] result = new T[_length];
            Array.Copy(_array, _index, result, 0, _length);
            return result;
        }

        public static NameValueCollection ToNVC(this Dictionary<object, object> _this) {
            NameValueCollection nvc = new NameValueCollection();
            foreach (KeyValuePair<object, object> pair in _this) {
                if (pair.Value.IsNotNull())
                    nvc.Add(pair.Key.ToString(), pair.Value.ToString());
            }
            return nvc;
        }

        public static T[] Add<T>(this T[] _array, T _newElement) {
            List<T> list = _array.ToList();
            list.Add(_newElement);
            return list.ToArray();
        }

        public static T[] RemoveAt<T>(this T[] _array, int _position) {
            List<T> list = _array.ToList();
            list.RemoveAt(_position);
            return list.ToArray();
        }

        public static List<T> ToList<T>(this T[] _array) {
            List<T> list = new List<T>();
            foreach (T e in _array)
                list.Add(e);
            return list;
        }

        public static string GetElementsSeparatedBy<T>(this T[] _array, string _char) {
            if (_array.IsNull())
                return "";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _array.Length; i++) {
                sb.Append(_array[i].ToString());
                if (i != _array.Length - 1)
                    sb.Append(_char);
            }
            return sb.ToString();
        }

        // https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos) {
                if (pinfo.CanWrite) {
                    try {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
                finfo.SetValue(comp, finfo.GetValue(other));
            return comp as T;
        }

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }


    }
}
