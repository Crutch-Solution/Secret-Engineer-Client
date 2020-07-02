using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Net;

namespace SCS_Module
{
    class Internet
    {
        static NetworkStream receive_stream, send_stream;
        static byte[] receive_buff = new byte[10000], send_buff = null;
        static int received;
        static dynamic receive_result;
        static bool send_result;
        static public TcpClient client=null;
        public Internet(TcpClient cl)
        {
            client = cl;
        }
       public static void Establish()
        {
         if(client==null)
                client = new TcpClient(Dns.GetHostEntry(Dns.GetHostName()).AddressList[2].ToString(), 9097);
        }
        static public ServerToExecutive receive()
        {
            List<byte> list = new List<byte>();
            receive_result = null;
            try
            {
                receive_stream = client.GetStream();
                received = receive_stream.Read(receive_buff, 0, receive_buff.Length);
                while (true)
                {
                    for (int i = 0; i < received; i++) list.Add(receive_buff[i]);
                    if (!receive_stream.DataAvailable) break;
                    received = receive_stream.Read(receive_buff, 0, receive_buff.Length);
                }
                receive_result = Encoding.GetEncoding(1251).GetString(list.ToArray(), 0, list.Count);
                receive_result = JsonConvert.DeserializeObject<ServerToExecutive > (receive_result);
            }
            catch (Exception ex)
            {
                receive_result = null;
            }
            return receive_result;
        }
        static public bool send(ExecutiveToServer mess)
        {
            send_result = true;
            try
            {
                send_stream = client.GetStream();
                send_buff = Encoding.GetEncoding(1251).GetBytes(JsonConvert.SerializeObject(mess));
                send_stream.Write(send_buff, 0, send_buff.Length);
            }
            catch (Exception ex)
            {
                send_result = false;
            }
            return send_result;
        }
    }
}
