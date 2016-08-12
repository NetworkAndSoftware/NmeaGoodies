// This is like a Hello World for reading NMEA messages. 


using System;
using Nmea0183.Communications;

namespace ShowMessages
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      using (var reader = new MessageReader("localhost", "serialin"))
      {
        reader.Message += (message, datetime) => Console.WriteLine(message.ToString());
        Console.ReadLine();
      }
    }
  }
}