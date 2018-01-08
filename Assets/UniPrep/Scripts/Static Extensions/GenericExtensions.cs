using System;
using UnityEngine;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace UniPrep.Extensions {
    public static class GenericExtensions {
        // ARRAY
        public static T[] SubArray<T>(this T[] _array, int _index, int _length) {
            T[] result = new T[_length];
            Array.Copy(_array, _index, result, 0, _length);
            return result;
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

        // LIST
        public static bool TryAdd<T>(this List<T> list, T element) {
            if (list == null) list = new List<T>();

            if (!list.Contains(element)) {
                list.Add(element);
                return true;
            }
            return false;
        }

        public static List<T> ShiftLeft<T>(this List<T> list, int shiftBy) {
            if (list.Count <= shiftBy) {
                return list;
            }

            var result = list.GetRange(shiftBy, list.Count - shiftBy);
            result.AddRange(list.GetRange(0, shiftBy));
            return result;
        }

        public static List<T> ShiftRight<T>(this List<T> list, int shiftBy) {
            if (list.Count <= shiftBy) {
                return list;
            }

            var result = list.GetRange(list.Count - shiftBy, shiftBy);
            result.AddRange(list.GetRange(0, list.Count - shiftBy));
            return result;
        }

        public static void Add<T>(this List<T> list, T[] tail) {
            foreach (var t in tail)
                list.Add(t);
        }

        public static void Add<T>(this List<T> list, List<T> tail) {
            foreach (var t in tail)
                list.Add(t);
        }

        // MAP
        public static NameValueCollection ToNVC(this Dictionary<object, object> _this) {
            NameValueCollection nvc = new NameValueCollection();
            foreach (KeyValuePair<object, object> pair in _this) {
                if (pair.Value.IsNotNull())
                    nvc.Add(pair.Key.ToString(), pair.Value.ToString());
            }
            return nvc;
        }
        
        // UNITY
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

        // MATRIX
        public static T[] MatrixToArray<T>(this T[,] matrix, bool rowMajor) {
            var arr = new T[matrix.GetLength(0) * matrix.GetLength(1)];

            if (rowMajor) {
                for (int i = 0; i < matrix.GetLength(0); i++) {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                        arr[i * matrix.GetLength(1) + j] = matrix[i, j];
                }
            }
            else {
                for (int i = 0; i < matrix.GetLength(1); i++) {
                    for (int j = 0; j < matrix.GetLength(0); j++)
                        arr[i * matrix.GetLength(0) + j] = matrix[j, i];
                }
            }
            return arr;
        }

        public static T[,] ArrayToMatrix<T>(this T[] array, int rows) {
            return array.ArrayToMatrix(rows, -1);
        }

        public static T[,] ArrayToMatrix<T>(this T[] array, int rows, int cols) {
            if (cols == -1) {
                if (array.Length % rows != 0)
                    throw new Exception("Row count mismatch");
                else
                    cols = array.Length / rows;
            }
            else {
                if (rows * cols != array.Length)
                    throw new Exception("Row, Col count mismatch");
            }
            T[,] matrix = new T[rows, cols];
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = array[i * cols + j];
            }
            return matrix;
        }
    }
}
