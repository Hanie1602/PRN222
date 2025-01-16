using System.Net.Sockets;
using System.Text;

namespace TCPChatServerandClient
{
	public class ChatClient
	{
		private TcpClient _client;

		#region Kết nối Client đến Server
		public void ConnectToServer(string ipServer, int port)
		{
			//Kết nối đến địa chỉ IP và port Server
			_client = new TcpClient(ipServer, port);
			Console.WriteLine("Connected to Server");

			//Tạo luồng riêng để liên tục nhận tin nhắn từ Server
			Thread readThread = new Thread(ReadMessage);
			readThread.Start();

			while (true)
			{
				Console.Write("Enter message: ");
				string message = Console.ReadLine();

				if (message == "sendfile")
				{
					Console.Write("Enter file path: ");
					string filePath = Console.ReadLine();
					SendFile(filePath);
				}
				else
				{
					SendMessage(message);
				}
			}
		}
		#endregion

		#region Gửi tin nhắn từ Client đến Server
		private void SendMessage(string message)
		{
			try
			{
				//Chuyển chuỗi message thành mảng byte (UTF8) để gửi
				byte[] data = Encoding.UTF8.GetBytes(message);

				//Lấy luồng data từ Server
				NetworkStream stream = _client.GetStream();
				stream.Write(data, 0, data.Length);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending message: {ex.Message}");
			}
		}
		#endregion

		#region Nhận và hiển thị tin nhắn từ Server
		private void ReadMessage()
		{
			try
			{
				//Lấy luồng dữ liệu (stream) để đọc data từ Server
				NetworkStream stream = _client.GetStream();
				byte[] buffer = new byte[1024];

				while (true)
				{
					//Đọc data từ Server vào mảng byte buffer
					int byteRead = stream.Read(buffer, 0, buffer.Length);

					//Nếu = 0, Server ngắt kết nối, thoát vòng lặp
					if (byteRead == 0)
						break;

					//Chuyển mảng byte thành chuỗi UTF8
					string message = Encoding.UTF8.GetString(buffer, 0, byteRead);
					Console.WriteLine($"Server: {message}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading message: {ex.Message}");
			}
		}
		#endregion

		#region
		public void SendFile(string filePath)
		{
			try
			{
				//Check File ko tồn tại
				if (!File.Exists(filePath))
				{
					Console.WriteLine("File does not exist.");
					return;
				}

				//Đọc nội dung file
				byte[] fileData = File.ReadAllBytes(filePath);

				//Gửi dữ liệu file
				NetworkStream stream = _client.GetStream();
				stream.Write(fileData, 0, fileData.Length);
				Console.WriteLine("File sent successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending file: {ex.Message}");
			}
		}
		#endregion
	}
}
