using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("mappedFile"))
            {

                Mutex mutex = Mutex.OpenExisting("mutex");
                mutex.WaitOne();

                using (MemoryMappedViewStream stream = mmf.CreateViewStream(43, 0))
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    List<string> secondProcessNames = new List<string>
                    { "Georgi Nikolov", "Nevena Petrova", "Yordan Grigorov" };

                    writer.Write(string.Join(", ", secondProcessNames));
                }
                mutex.ReleaseMutex();
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Memory-mapped file doesn't exist! Run FirstProcess file first!");
        }
    }
}