using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Client
{
	public partial class MainWindow : Window
	{
		private TcpClient _client;
		private StreamWriter _writer;
		private Thread _receiveThread;

		public MainWindow()
		{
			InitializeComponent();
			ConnectToServer();
		}

		private void ConnectToServer()
		{
			try
			{
				string serverAddress = "127.0.0.1";
				int port;

				//Yêu cầu người dùng nhập port
				string input = Microsoft.VisualBasic.Interaction.InputBox("Enter server port:", "Connect to Server", "5000");

				//Nhấn nút "Cancel"
				if (string.IsNullOrWhiteSpace(input))
				{
					MessageBox.Show("Connection canceled by user.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
					Application.Current.Shutdown();
					return;
				}

				if (!int.TryParse(input, out port))
				{
					MessageBox.Show("Invalid port. Please restart the application and enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					Application.Current.Shutdown(); 
					return;
				}

				_client = new TcpClient(serverAddress, port);
				_writer = new StreamWriter(_client.GetStream(), Encoding.UTF8) { AutoFlush = true };
				_receiveThread = new Thread(ReceiveMessages) { IsBackground = true };
				_receiveThread.Start();
				MessageBox.Show($"Connected to server on port {port}");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to connect to server: " + ex.Message);
				Application.Current.Shutdown(); //Đóng ứng dụng
			}
		}

		private void ReceiveMessages()
		{
			try
			{
				StreamReader reader = new StreamReader(_client.GetStream(), Encoding.UTF8);
				while (true)
				{
					string? message = reader.ReadLine();
					if (message == null)
						break;

					Dispatcher.Invoke(() => ChatBox.AppendText(message + Environment.NewLine));
				}
			}
			catch (IOException)
			{
				Dispatcher.Invoke(() => ChatBox.AppendText("Disconnected from server.\n"));
			}
		}

		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(InputBox.Text))
			{
				_writer.WriteLine(InputBox.Text);
				InputBox.Clear();
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_client?.Close();
			_receiveThread?.Abort();
		}
	}
}