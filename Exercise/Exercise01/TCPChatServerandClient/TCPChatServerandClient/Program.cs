namespace TCPChatServerandClient
{
	public class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Choose: [1] for Server, [2] for Client");
			string mode = Console.ReadLine();

			if (mode == "1")
			{
				Console.Write("Enter port to listen on: ");
				int port = int.Parse(Console.ReadLine());
				ChatServer server = new ChatServer();
				server.StartServer(port);       //Lắng nghe trên port
			}
			else if (mode == "2")
			{
				ChatClient client = new ChatClient();

				//Nếu người dùng ko nhập hoặc nhập khoảng trắng, mặc định IP = 127.0.0.1
				Console.Write("Enter Server IP (default: 127.0.0.1): ");
				string serverIp = Console.ReadLine();
				if (string.IsNullOrEmpty(serverIp))
					serverIp = "127.0.0.1";

				Console.Write("Enter Server port: ");
				int port = int.Parse(Console.ReadLine());

				//Kết nối Client đến Server trên địa chỉ IP, port
				client.ConnectToServer(serverIp, port);
			}
			else
			{
				Console.WriteLine("Invalid option.");
			}
		}
	}
}