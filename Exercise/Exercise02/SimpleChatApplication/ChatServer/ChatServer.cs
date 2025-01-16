using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
	public class ChatServer
	{
		private TcpListener _listener;
		private List<TcpClient> _clients = new List<TcpClient>();

		#region Khởi động Server và lắng nghe kết nối từ Client
		public void StartServer(int port)
		{
			//Lắng nghe các kế nối từ mọi địa chỉ IP trên port
			_listener = new TcpListener(IPAddress.Any, port);
			_listener.Start();	//Bắt đầu lắng nghe từ Client
			Console.WriteLine($"Server started on port {port}");

			while (true)
			{
				//Chấp nhận kết nối từ Client vào _clients
				TcpClient client = _listener.AcceptTcpClient();
				_clients.Add(client);
				Console.WriteLine("New client connted");

				//Truyển client vào Thread
				Thread clientThread = new Thread(HandleClient);
				clientThread.Start(client);
			}
		}
		#endregion

		#region Xử lý nhiều máy khách đồng thời
		private void HandleClient(object obj)
		{
			TcpClient client = (TcpClient)obj;

			try
			{
				NetworkStream stream = client.GetStream();
				byte[] b = new byte[1024];

				while (true)
				{
					//Đọc dữ liệu từ Client
					//Số byte đọc được. Nếu = 0, nghĩa là Client đã ngắt kết nối
					int byteRead = stream.Read(b, 0, b.Length);
					if (byteRead == 0)
					{
						Console.WriteLine("Client disconnected");
						break;
					}

					//Chuyển data byte nhận được thành chuỗi UTF8
					string message = Encoding.UTF8.GetString(b, 0, byteRead);

					if (message.StartsWith("FILE"))
					{
						//Xử lý nhận file
						ReceiveFile(client);
					}
					else
					{
						Console.WriteLine($"Received: {message}");
						//Gửi tin nhắn tới tất cả Client khác
						BroadcastMessage(message, client);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error client: {ex.Message}");
			}
			finally
			{
				_clients.Remove(client);
				client.Close();
				Console.WriteLine("Client connection closed");
			}
		}
		#endregion

		#region Gửi tin nhắn từ 1 Client đến nhiều Client khác
		private void BroadcastMessage(string message, TcpClient sender)
		{
			//Chuyển chuỗi message thành dạng byte để gửi
			byte[] data = Encoding.UTF8.GetBytes(message);

			//Gửi data đến từng Client
			foreach(TcpClient client in _clients)
			{
				if(client != sender)
				{
					NetworkStream stream = client.GetStream();
					stream.Write(data, 0, data.Length);
				}
			}
		}
		#endregion

		private void ReceiveFile(TcpClient client)
		{
			try
			{
				NetworkStream stream = client.GetStream();

				//Đọc dữ liệu từ client
				byte[] buffer = new byte[1024];
				int bytesRead;

				//Tạo tên file theo format YYYYMMddHHmmss.txt
				string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.txt";

				//Tạo file stream để ghi dữ liệu
				using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
					{
						fs.Write(buffer, 0, bytesRead);
					}
				}

				//Hiển thị thông báo khi file đã được lưu
				Console.WriteLine($"File received and saved as {fileName}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error receiving file: {ex.Message}");
			}
		}

	}
}