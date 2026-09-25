using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using reservation_winforms.DTO.table;

namespace reservation_winforms.Services
{
    public class WebSocketService
    {
        private static readonly WebSocketService _instance = new WebSocketService();
        public static WebSocketService Instance => _instance;

        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly string _wsUri = "ws://localhost:8081/ws-reservation-native";

        public event Action<TableUpdate> OnTableStatusChanged;
        public event Action<TableAlertMessage> OnTableAlertReceived;

        private WebSocketService() { }

        public async Task ConnectAsync()
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
                return;

            _webSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await _webSocket.ConnectAsync(new Uri(_wsUri), _cancellationTokenSource.Token);
                Console.WriteLine("WebSocket Connected!");

                string connectFrame = "CONNECT\naccept-version:1.1,1.2\nheart-beat:10000,10000\n\n\0";
                await SendFrameAsync(connectFrame);

                _ = ReceiveMessagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket Connection Error: {ex.Message}");
            }
        }

        private async Task SendFrameAsync(string frame)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(frame);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[65536];

            try
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    var sb = new StringBuilder();
                    while (true)
                    {
                        var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            Console.WriteLine("WebSocket server sent Close frame.");
                            return;
                        }

                        sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        if (result.EndOfMessage) break;
                    }
                    string message = sb.ToString();
                    HandleStompFrame(message);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("WebSocket receive cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket Receive Error: {ex.Message}");
            }
        }

        private void HandleStompFrame(string payload)
        {
            if (string.IsNullOrEmpty(payload)) return;
            string[] frames = payload.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string frame in frames)
            {
                if (frame.StartsWith("CONNECTED"))
                {
                    Console.WriteLine("STOMP Connected successfully. Subscribing to topics...");
                    SubscribeToTopics();
                }
                else if (frame.Contains("MESSAGE"))
                {
                    int bodyIndex = frame.IndexOf("\n\n");
                    if (bodyIndex == -1) bodyIndex = frame.IndexOf("\r\n\r\n");

                    if (bodyIndex != -1)
                    {
                        string body = frame.Substring(bodyIndex).Trim();
                        try
                        {
                            if (frame.Contains("destination:/topic/tables"))
                            {
                                var update = JsonConvert.DeserializeObject<TableUpdate>(body);
                                if (update != null) OnTableStatusChanged?.Invoke(update);
                            }
                            else if (frame.Contains("destination:/topic/table-alerts"))
                            {
                                var alert = JsonConvert.DeserializeObject<TableAlertMessage>(body);
                                if (alert != null) OnTableAlertReceived?.Invoke(alert);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("JSON Parse Error: " + ex.Message);
                        }
                    }
                }
            }
        }

        private async void SubscribeToTopics()
        {
            string subTables = "SUBSCRIBE\nid:sub-0\ndestination:/topic/tables\n\n\0";
            await SendFrameAsync(subTables);

            string subAlerts = "SUBSCRIBE\nid:sub-1\ndestination:/topic/table-alerts\n\n\0";
            await SendFrameAsync(subAlerts);
        }

        public async Task DisconnectAsync()
        {
            if (_webSocket == null) return;

            try
            {
                _cancellationTokenSource?.Cancel();

                if (_webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
            catch (ObjectDisposedException)
            {
                // Đã bị dispose từ lần gọi trước, bỏ qua
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket Disconnect Error: {ex.Message}");
            }
            finally
            {
                _webSocket?.Dispose();
                _webSocket = null;

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }
}