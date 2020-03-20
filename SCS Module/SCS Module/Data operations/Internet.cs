using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.IO;
namespace SCS_Module
{
    public enum RequestType
    {
        GetCategories, GetByCategory, GetByString, GetWholeModel, UploadModel
    }
    public enum ResponseType
    {
        GetCategories, GetByCategory, GetByString, GetWholeModel
    }
    class Internet
    {

        NetworkStream receive_stream, send_stream;
        byte[] receive_buff = new byte[1000000], send_buff = null;
        int received;
        dynamic receive_result;
        bool send_result;
        public TcpClient client;
        public Internet(TcpClient cl)
        {
            client = cl;
        }
        public string receive()
        {
            receive_result = null;
            try
            {
                receive_stream = client.GetStream();
                received = receive_stream.Read(receive_buff, 0, receive_buff.Length);
                receive_result = Encoding.ASCII.GetString(receive_buff, 0, received);
            }
            catch (Exception ex)
            {
                receive_result = null;
            }
            return receive_result;
        }
        public T receive<T>()
        {
            receive_result = null;
            try
            {
                receive_stream = client.GetStream();
                received = receive_stream.Read(receive_buff, 0, receive_buff.Length);
                receive_result = Encoding.ASCII.GetString(receive_buff, 0, received);
                receive_result = JsonConvert.DeserializeObject<T>(receive_result);
            }
            catch (Exception ex)
            {
                receive_result = null;
            }
            return receive_result;
        }
        public bool send(string mess)
        {
            send_result = true;
            try
            {
                send_stream = client.GetStream();
                send_buff = Encoding.ASCII.GetBytes(mess);
                send_stream.Write(send_buff, 0, send_buff.Length);
            }
            catch (Exception ex)
            {
                send_result = false;
            }
            return send_result;
        }
        public bool send<T>(T mess)
        {
            send_result = true;
            try
            {
                send_stream = client.GetStream();
                string hui = JsonConvert.SerializeObject(mess);
                send_buff = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(mess));
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
