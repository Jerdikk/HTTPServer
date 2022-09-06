/*
 * Filename:  TCPServer.cs    
 * Author: ertaquo @ertaquo https://habr.com/ru/users/ertaquo/
 */

using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace HTTPServer
{
    class TCPServer
    {
        TcpListener Listener; // Объект, принимающий TCP-клиентов

        // Запуск сервера
        public TCPServer(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port); // Создаем "слушателя" для указанного порта
            try
            {
                Listener.Start(); // Запускаем его
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while trying to start server:" + ex.Message + ex.StackTrace);
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return;
            }

            // В бесконечном цикле
            while (true)
            {
                Console.WriteLine("Waiting...");
                // Принимаем новых клиентов. После того, как клиент был принят, он передается в новый поток (ClientThread)
                // с использованием пула потоков.
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());
                Console.WriteLine("New tread is created...");
                /*
                // Принимаем нового клиента
                TcpClient Client = Listener.AcceptTcpClient();
                // Создаем поток
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                Thread.Start(Client);
                */
            }
        }

        static void ClientThread(Object StateInfo)
        {
            // Просто создаем новый экземпляр класса Client и передаем ему приведенный к классу TcpClient объект StateInfo
            new TCPClient((TcpClient)StateInfo);
        }

        // Остановка сервера
        ~TCPServer()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }

    }

}
