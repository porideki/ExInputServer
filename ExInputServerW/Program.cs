using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using System.Net;     //IPEndPointクラス
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.Sockets; //TCPListener、TCPClientクラス
using System.IO;

namespace ExInputServerW {
    
    class Entry {

        public static string IP = "172.18.4.158";
        public static int PORT = 14230;
        public static string PROXY_URL = "wwwproxy.kanazawa-it.ac.jp";
        public static int PROXY_PORT = 8080;

        [STAThread]
        static void Main(string[] args) {

            //ソケット作成
            IPEndPoint ipAdd = new IPEndPoint(IPAddress.Parse(IP), PORT);   //サーバー情報
            TcpListener listener = new TcpListener(ipAdd);  //サーバーソケット

            Console.WriteLine("Server > Opened server");

            //IP取得
            /*
            IPHostEntry ipentry = Dns.GetHostEntry(Dns.GetHostName());
            IEnumerable<IPAddress> ips = ipentry.AddressList.Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            foreach (IPAddress ip in ips) Console.WriteLine("Server > " + ip.ToString());
            */
            //var Interfaces = NetworkInterface.GetAllNetworkInterfaces().Where(adp => adp.OperationalStatus != OperationalStatus.Up);
            

                while (true) {

                try {

                    //接続待ち
                    listener.Start(0);

                    //接続確立
                    TcpClient client = listener.AcceptTcpClient();  //ソケット
                    //client.Connect(PROXY_URL, PROXY_PORT);

                    //入力受付
                    TcpConnect tcpConnect = new TcpConnect(client);
                    Thread thread = new Thread(new ThreadStart(tcpConnect.run));
                    thread.Start();
                    //tcpConnect.run();

                } catch(Exception e) {
                    Console.WriteLine(e.ToString());
                }

            }

            Console.WriteLine("終了するには、Enterキーを押してください");
            Console.ReadLine();

        }
    }

    public class TcpConnect {

        private TcpClient socket;
        private StreamReader streamReader;

        public TcpConnect(TcpClient socket) {

            this.socket = socket;

        }

        public void run() {

            try {
                this.streamReader = new StreamReader(socket.GetStream(), Encoding.UTF8);
            } catch(Exception e) {
                Console.WriteLine(e.ToString());
            }

            while (socket.Connected) {

                try {

                    String str;
                    while ((str = streamReader.ReadLine()) == null) Thread.Sleep(100);

                    Thread.Sleep(100);

                    //入力処理
                    KeyDispacher.StringDispach(str);
                    Console.WriteLine("Client > " + str);

                }catch(Exception e) {
                    Console.WriteLine(e.ToString());
                }

            }

            this.Disconnect();

        }

        public void Disconnect() {

            try {

                this.streamReader.Close();
                this.socket.Close();

                Console.WriteLine("Server > Disconnect.");

            } catch(Exception e) {
                Console.WriteLine(e.ToString());
            }

        }

    }

    public class KeyDispacher {

        public static void StringDispach(String str) {

            SendKeys.SendWait(str);

        }

    }

}
