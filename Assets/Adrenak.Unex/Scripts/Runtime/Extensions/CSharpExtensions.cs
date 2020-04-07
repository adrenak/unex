using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Adrenak.Unex {
	public static class CSharpExtensions {
		// ENUM
		public static bool Has<T>(this Enum type, T value) {
			try {
				return (((int)(object)type & (int)(object)value) == (int)(object)value);
			}
			catch {
				return false;
			}
		}

		public static bool Is<T>(this Enum type, T value) {
			try {
				return (int)(object)type == (int)(object)value;
			}
			catch {
				return false;
			}
		}

		public static T Add<T>(this Enum type, T value) {
			try {
				return (T)(object)(((int)(object)type | (int)(object)value));
			}
			catch (Exception ex) {
				throw new ArgumentException(
					string.Format(
						"Could not append value from enumerated type '{0}'.",
						typeof(T).Name
						), ex);
			}
		}


		public static T Remove<T>(this Enum type, T value) {
			try {
				return (T)(object)(((int)(object)type & ~(int)(object)value));
			}
			catch (Exception ex) {
				throw new ArgumentException(
					string.Format(
						"Could not remove value from enumerated type '{0}'.",
						typeof(T).Name
						), ex);
			}
		}

		// MATH
		public static bool Approximately(this float a, float b) {
			return Mathf.Approximately(a, b);
		}

        // BYTE[]
		public static string ToUTF8String(this byte[] bytes) {
			return Encoding.UTF8.GetString(bytes);
		}


		// STRINGS
		public static bool IsNullOrEmpty(this string _string) {
			return string.IsNullOrEmpty(_string);
		}

		public static byte[] ToUTF8Bytes(this string str) {
			return Encoding.UTF8.GetBytes(str);
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

		public static byte[] ToBytes(this float[] floatArray) {
			var floatLen = floatArray.Length;
			Int16[] intData = new Int16[floatLen];
			Byte[] bytesData = new Byte[floatLen * 2];
			Byte[] byteArr = new Byte[2];

			int rescaleFactor = 32767; //to convert float to Int16

			for (int i = 0; i < floatArray.Length; i++) {
				intData[i] = (short)(floatArray[i] * rescaleFactor);
				byteArr = BitConverter.GetBytes(intData[i]);
				byteArr.CopyTo(bytesData, i * 2);
			}
			return bytesData;
		}

		public static bool IsNullOrEmpty<T>(this List<T> list) {
			return list == null || list.Count == 0;
		}

		public static bool IsNullOrEmpty<T>(this T[] array) {
			return array == null || array.Length == 0;
		}

        public static T FromLast<T>(this List<T> list, int index) {
            if (index >= list.Count)
                throw new IndexOutOfRangeException(index + " is out of range");
            return list[list.Count - 1 - index];
        }

        public static T Last<T>(this List<T> list) {
            return list.FromLast(0);
        }

        public static void RemoveLast<T>(this List<T> list) {
            var last = list.Last();
            list.Remove(last);
        }
    }
}
