using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Adrenak.Unex {
    public class TaskGroup<T, K> {
        public event Action<K, int> SingleTaskCompleted;

        Dictionary<T, Task<K>> taskMap;

        public TaskGroup(Dictionary<T, Task<K>> map) {
            taskMap = map;
        }

        public async Task<Dictionary<T, K>> Run() {
            var resultMap = new Dictionary<T, K>();

            while (taskMap.Count != 0) {
                var done = await Task.WhenAny(taskMap.Values.ToList());
                var index = taskMap.Values.ToList().IndexOf(done);
                var key = taskMap.Keys.ToList()[index];

                resultMap.Add(key, done.Result);
                SingleTaskCompleted?.Invoke(done.Result, index);
                taskMap.Remove(key);
            }

            return resultMap;
        }
    }
}
