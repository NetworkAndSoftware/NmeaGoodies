using System;
using System.Linq;
using Nmea0183.Constants;

namespace Nmea0183.Messages
{
  public abstract class MessageBase
  {
    protected const string DATETIME_DDMMYY = "ddMMyy";
    protected const string DATETIME_HHMMSS = "HHmmss";
    protected const string DATETIME_HHMMSSfff = "HHmmss.FFF";
    protected const string TIMESPAN_HHMMSSfff = "hhmmss\\.FFF";
    protected const string TIMESPAN_HHMMSS = "hhmmss";

    protected MessageBase(string talkerId)
    {
      TalkerId = talkerId;
    }

    public virtual string CommandName => GetCommandNameAttribute();

    public virtual MessageName Name
    {
      get
      {
        try
        {
          return MessageFormatting.ParseEnum<MessageName>(GetType().Name);
        }
        catch (Exception)
        {
          return MessageName.Unknown;
        }
      }
    }

    private string GetCommandNameAttribute()
    {
      var attributes = GetType().GetCustomAttributes(typeof (CommandNameAttribute), true);
      return ((CommandNameAttribute) attributes.First()).Name.ToString();
    }

    public string TalkerId { get; set; }

    protected abstract string CommandBody { get; }

    private static string Checksum(string s)
    {
      var sum = s.ToCharArray().Aggregate<char, byte>(0, (current, c) => (byte) (current ^ Convert.ToByte(c)));
      return BitConverter.ToString(new[] {sum});
    }

    public override string ToString()
    {
      var line = TalkerId + CommandName + "," + CommandBody;
      return "$" + line + '*' + Checksum(line);
    }

    private static bool IsChecksumValid(string line)
    {
      if ('*' != line[line.Length - 3])
        return false;
      var checksum = line.Substring(line.Length - 2);
      var rest = line.Substring(1, line.Length - 4);
      var coolchecksum = Checksum(rest);
      return checksum == coolchecksum;
    }

    public static MessageBase Parse(string line)
    {
      line = line.Trim();

      if (string.IsNullOrWhiteSpace(line))
        throw new FormatException("Line is null or whitespace");

      if (line[0] != '$')
        throw new FormatException("Does not look like NMEA0183 message");

      if (!IsChecksumValid(line))
        throw new FormatException($"Checksum error: in line {line}");

      var parts = line.Substring(0, line.Length - 3).Split(',');

      try
      {
        var talkerId = parts[0].Substring(1, 2);
        var commandName = parts[0].Substring(3);
        var commandbody = string.Join(",", parts.Skip(1).Take(parts.Length - 1));
        var bodyparts = commandbody.Split(',');

        switch (commandName)
        {
          case "HDM":
            return new HDM(talkerId, bodyparts);
          case "HDT":
            return new HDT(talkerId, bodyparts);
          case "APB":
            return new APB(talkerId, bodyparts);
          case "RMC":
            return new RMC(talkerId, bodyparts);
          case "GBS":
            return new GBS(talkerId, bodyparts);
          case "GLL":
            return new GLL(talkerId, bodyparts);
          case "GGA":
            return new GGA(talkerId, bodyparts);
          case "HDG":
            return new HDG(talkerId, bodyparts);
          default:
            return new UnknownMessage(talkerId, commandName, commandbody);
        }
      }
      catch (Exception x)
      {
        throw new FormatException("Unknown parsing error. Check inner exception", x);
      }
    }

  }


}