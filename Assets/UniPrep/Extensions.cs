/**
*	MIT License
*
*	Copyright (c) 2017 Vatsal Ambastha
*
*	Permission is hereby granted, free of charge, to any person obtaining a copy
*	of this software and associated documentation files (the "Software"), to deal
*	in the Software without restriction, including without limitation the rights
*	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*	copies of the Software, and to permit persons to whom the Software is
*	furnished to do so, subject to the following conditions:
*
*	The above copyright notice and this permission notice shall be included in all
*	copies or substantial portions of the Software.
*
*	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*	SOFTWARE.
**/

using System;
using UnityEngine;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace UniPrep {
    public static class Extensions {
        // STRINGS
        public static bool IsNullOrEmpty(this string _string) {
            return string.IsNullOrEmpty(_string);
        }

        // OBJECT
        public static bool IsNull(this object _object) {
            return ReferenceEquals(_object, null);
        }

        public static bool IsNotNull(this object _object) {
            return !_object.IsNull();
        }

        // BOOL
        public static bool IsTrue(this bool _bool) {
            return _bool == true;
        }

        public static bool IsFalse(this bool _bool) {
            return _bool == false;
        }

        public static bool Invert(this bool _bool) {
            return !_bool;
        }
        
        // RECT
        public static Rect SetX(this Rect rect, float val) {
            return new Rect(val, rect.y, rect.width, rect.height);
        }

        public static Rect SetY(this Rect rect, float val) {
            return new Rect(rect.x, val, rect.width, rect.height);
        }

        public static Rect SetW(this Rect rect, float val) {
            return new Rect(rect.x, rect.y, val, rect.height);
        }

        public static Rect SetH(this Rect rect, float val) {
            return new Rect(rect.x, rect.y, rect.width, val);
        }

        // VECTOR3
        public static Vector3 SetX(this Vector3 v, float val) {
            return new Vector3(val, v.y, v.z);
        }

        public static Vector3 SetY(this Vector3 v, float val) {
            return new Vector3(v.x, val, v.z);
        }

        public static Vector3 SetZ(this Vector3 v, float val) {
            return new Vector3(v.x, v.y, val);
        }

        // VECTOR2
        public static Vector2 SetX(this Vector2 v, float val) {
            return new Vector2(val, v.y);
        }

        public static Vector2 SetY(this Vector2 v, float val) {
            return new Vector2(v.x, val);
        }

        // GENERICS
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

        // EXCEPTION
        public static string PrintRecursively(this Exception _exception) {
            if (_exception.InnerException.IsNotNull())
                return _exception.Message + " --> \n" + _exception.StackTrace + "\n\n" + _exception.InnerException.PrintRecursively();
            else
                return _exception.Message + " --> \n" + _exception.StackTrace;
        }

        // DATE TIME
        public static bool IsOnSameDayAs(this DateTime dateTime, DateTime other) {
            if (dateTime.Day == other.Day && dateTime.Month == other.Month && dateTime.Year == other.Year)
                return true;
            return false;
        }

        public static bool IsOnSameDayAs(this DateTime @this, int day, int month, int year) {
            return @this.Day == day && @this.Month == month && @this.Year == year;
        }

        public static bool IsDateInRange(this DateTime toCheck, DateTime lowerBound, DateTime upperBound) {
            if (lowerBound < toCheck && toCheck < upperBound)
                return true;
            return false;
        }

        public static DateTime ToHumanTime(this long unixTimeStamp) {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(Convert.ToDouble(unixTimeStamp));
        }

        public static DateTime? ToHumanTime(this string unixTimeStamp) {
            try {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddMilliseconds(Convert.ToInt64(unixTimeStamp));
            }
            catch {
                return null;
            }
        }

        public static long ToUnixTime(this DateTime date) {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)((date - epoch).TotalMilliseconds);
        }

        // THREADS
        public static void Terminate(this System.Threading.Thread t) {
            if (t == null)
                return;
            t.Abort();
            t.Join();
        }

        // GAME OBJECTS
        public static void Destroy(this GameObject gameObject) {
            MonoBehaviour.Destroy(gameObject);
        }

        public static void DestroyImmediate(this GameObject gameObject) {
            MonoBehaviour.DestroyImmediate(gameObject);
        }

        // VIEW GROUP
        public static void Set(this CanvasGroup group, bool status) {
            group.interactable = group.blocksRaycasts = status;
            group.alpha = status ? 1 : 0;
        }
    }
}
