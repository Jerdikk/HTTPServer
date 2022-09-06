using System;
using System.Collections.Generic;
using System.Threading;

namespace HTTPServer
{
    partial class Program
    {
        public class Demo1
        {
            public void RunTCPServer()
            {
                // Определим нужное максимальное количество потоков
                // Пусть будет по 4 на каждый процессор
                int MaxThreadsCount = Environment.ProcessorCount * 4;
                // Установим максимальное количество рабочих потоков
                ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
                // Установим минимальное количество рабочих потоков
                ThreadPool.SetMinThreads(2, 2);
                // Создадим новый сервер на порту 80
                new TCPServer(8081);

            }
        }

        static void Main(string[] args)
        {
            /*Demo1 demo1 = new Demo1();
             demo1.RunTCPServer();*/
            HttpServer httpServer = new HttpServer();
            httpServer.Run();

        }
    }

}
