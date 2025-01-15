
namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            ChatServer server = new ChatServer();
            server.StartServer(5000);       //Lắng nghe trên port
           
        }
    }
}
