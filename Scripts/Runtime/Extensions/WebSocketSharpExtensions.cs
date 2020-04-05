//#define ENABLE
#if ENABLE
using System;
using System.Threading.Tasks;

namespace WebSocketSharp {
    public static class WebSocketSharpExtensions {
        public static void ConnectAsync(this WebSocket socket, Action OnConnect, Action<Exception> OnError) {
            Action onDone = () => { };

            void onConnect(object sender, EventArgs e) {
                OnConnect?.Invoke();
                onDone();
            }

            void onClose(object sender, CloseEventArgs e) {
                OnError?.Invoke(new Exception(e.Reason));
                onDone();
            }

            onDone = () => {
                socket.OnOpen -= onConnect;
                socket.OnClose -= onClose;
            };

            socket.OnOpen += onConnect;
            socket.OnClose += onClose;

            socket.ConnectAsync();
        }

        public static Task ConnectAsync(this WebSocket socket) {
            var source = new TaskCompletionSource<bool>();
            socket.ConnectAsync(
                () => source.SetResult(true),
                exception => source.SetException(exception)
            );
            return source.Task;
        }

        public static void CloseAsync(this WebSocket socket, Action OnDisconnect) {
            if (!socket.IsAlive)
                OnDisconnect?.Invoke();

            Action onDone = () => { };

            EventHandler<CloseEventArgs> onClose = (sender, e) => {
                OnDisconnect();
                onDone();
            };

            onDone = () => socket.OnClose -= onClose;
            socket.OnClose += onClose;
            socket.CloseAsync();
        }

        public static Task CloseAsync(this WebSocket socket) {
            var source = new TaskCompletionSource<bool>();
            socket.CloseAsync(
                () => source.SetResult(true)
            );
            return source.Task;
        }
    }
}
#endif