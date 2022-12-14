using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;



namespace EmpaticaBLEClient
{
    public static class AsynchronousClient
    {
        // The port number for the remote device.
        private const string ServerAddress = "127.0.0.1";
        private const int ServerPort = 28000;

        // ManualResetEvent instances signal completion.
        private static readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent SendDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent ReceiveDone = new ManualResetEvent(false);

        // The response from the remote device.
        private static String _response = String.Empty;
        private static StringBuilder csv = new StringBuilder();

        public static void StartClient()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                var ipHostInfo = new IPHostEntry { AddressList = new[] { IPAddress.Parse(ServerAddress) } };
                var ipAddress = ipHostInfo.AddressList[0];
                var remoteEp = new IPEndPoint(ipAddress, ServerPort);

                // Create a TCP/IP socket.
                var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEp, (ConnectCallback), client);
                ConnectDone.WaitOne();

                Send(client, "device_connect 92E2CC" + Environment.NewLine);
                SendDone.WaitOne();
                Receive(client);
                ReceiveDone.WaitOne();

                Console.WriteLine("Connection with Empatica established");

                var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                String time = timestamp.ToString() + '\n';
                String header = "System Epoch,Device,Category,E4 Epoch,Value\n";

                //If you don't put the thread to sleep for a bit it locks the file and gives you an error later
                try
                {
                    File.WriteAllText("..\\..\\data2.csv", header);
                }
                catch
                {
                    System.Threading.Thread.Sleep(100);
                }

                //If you want to turn on any other sensor then just copy and paste the 4 lines down below
                //and just change the 3 letters for the sensor that you want

                //If you want to mess with the formatting look at the HandleResponseFromEmpaticaServer function

                Console.WriteLine("BVP and GSR collection has started. Type 'device_disconnect' to exit");

                Send(client, "device_subscribe gsr ON" + Environment.NewLine);
                SendDone.WaitOne();
                Receive(client);
                ReceiveDone.WaitOne();

                //Send(client, "device_subscribe bvp ON" + Environment.NewLine);
                //SendDone.WaitOne();
                //Receive(client);
                //ReceiveDone.WaitOne();


                while (true)
                {
                    var msg = Console.ReadLine();
                    Send(client, msg + Environment.NewLine);
                    SendDone.WaitOne();
                    Receive(client);
                    ReceiveDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                var client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint);

                // Signal that the connection has been made.
                ConnectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                var state = new StateObject { WorkSocket = client };

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                var state = (StateObject)ar.AsyncState;
                var client = state.WorkSocket;

                // Read data from the remote device.
                var bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                    _response = state.Sb.ToString();

                    HandleResponseFromEmpaticaBLEServer(_response);

                                        
                    state.Sb.Clear();

                    ReceiveDone.Set();

                    // Get the rest of the data.
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.Sb.Length > 1)
                    {
                        _response = state.Sb.ToString();
                    }
                    // Signal that all bytes have been received.

                    ReceiveDone.Set();
                    //File.AppendAllText("..\\..\\data2.csv", csv.ToString());
                    Environment.Exit(0);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                var client = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                client.EndSend(ar);
                // Signal that all bytes have been sent.
                SendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static Object thisLock = new Object();

        private static void HandleResponseFromEmpaticaBLEServer(string response)
        {
                        
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            String time =timestamp.ToString()+',';
            response = response.Replace(" ", ",");
            response = response.Insert(0, time);
            Console.Write(response);
            //csv.Append(response + time);
            lock (thisLock)
            {
                File.AppendAllText("..\\..\\data2.csv", response);
            }

        }

        public static void Wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            // Console.WriteLine("start wait timer");
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                // Console.WriteLine("stop wait timer");
            };

            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }

        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }
    }
}