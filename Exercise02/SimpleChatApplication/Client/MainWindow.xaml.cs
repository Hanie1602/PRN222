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
				string ipAddressServer = "127.0.0.1";
				int port;

				//Yêu cầu người dùng nhập port
				do
				{
					string input = Microsoft.VisualBasic.Interaction.InputBox("Enter server port: ", "Connect to Server", "12345");
					if (int.TryParse(input, out port))
						break;

					MessageBox.Show("Invalid port. Please enter a valid number.");
				} while (true);

				_client = new TcpClient(ipAddressServer, port);
				_writer = new StreamWriter(_client.GetStream(), Encoding.UTF8) { AutoFlush = true };
				_receiveThread = new Thread(ReceiveMessages) { IsBackground = true };
				_receiveThread.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to connect to server: " + ex.Message);
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