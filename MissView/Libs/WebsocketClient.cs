using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MissView.Libs
{
	public class WebSocketClient
	{
		private readonly ClientWebSocket _webSocket = new();

		public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
		{
			await _webSocket.ConnectAsync(uri, cancellationToken);
			await StartListening();
		}

		private async Task StartListening()
		{
			var buffer = new byte[1024];
			while (_webSocket.State == WebSocketState.Open)
			{
				var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				Debug.WriteLine("websocket message received.");
				if (result.MessageType == WebSocketMessageType.Text)
				{
					var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
					// メッセージを処理するロジックをここに追加
					Debug.WriteLine(message);
				}
			}
		}

		public async Task SendAsync(string message, CancellationToken cancellationToken)
		{
			var bytes = System.Text.Encoding.UTF8.GetBytes(message);
			await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
		}

		public async Task DisconnectAsync()
		{
			if (_webSocket.State == WebSocketState.Open)
				await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
		}
	}
}
