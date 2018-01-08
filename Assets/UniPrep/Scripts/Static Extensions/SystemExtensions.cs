using System;
using UnityEngine;

namespace UniPrep.Extensions {
    public static class SystemExceptions {
        // MATH
        public static bool Approximately(this float a, float b) {
            return Mathf.Approximately(a, b);
        }

        public static void BubbleSort(this float[] arr, bool ascending) {
            for(int i = 0; i < arr.Length; i++) {
                for(int j = 0; j < arr.Length - 1; j++) {
                    if (ascending) {
                        if(arr[j] > arr[i]) {
                            var temp = arr[j];
                            arr[j] = arr[i];
                            arr[i] = temp;
                        }
                    }
                    else {
                        if (arr[j] < arr[i]) {
                            var temp = arr[j];
                            arr[j] = arr[i];
                            arr[i] = temp;
                        }
                    }
                }
            }
        }

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
    }
}
