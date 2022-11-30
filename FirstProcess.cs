using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("mappedFile", 1000000000))
        {
            bool mutexCreated;
            Mutex mutex = new Mutex(true, "mutex", out mutexCreated);
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                List<string> firstProcessNames = new List<string>
                { "Stefan Kalenderov", "Vasil Yordanov", "Dimitar Berbatov" };

                 writer.Write(string.Join(", ", firstProcessNames));
            }

            mutex.ReleaseMutex();
            Console.WriteLine("Start SecondProcess and press 'ENTER' to proceed.");
            Console.ReadLine();

            mutex.WaitOne();

            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                Console.WriteLine("Names from FirstProcess file: {0}", reader.ReadString());
                Console.WriteLine("Names from SecondProcess file: {0}", reader.ReadString());

            }

            mutex.ReleaseMutex();
        }
    }
}