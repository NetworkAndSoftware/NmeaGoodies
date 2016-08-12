using System;
using Nmea0183.Communications;

namespace HelloWorld
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