using System;
using System.IO;
using System.Text;

namespace CRC32
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] sourceBytes = new byte[0];
            if (args.Length > 0)
            {
                if (args.Length == 1)
                {
                    sourceBytes = Encoding.UTF8.GetBytes(args[0]);
                }
                if (args.Length >= 2)
                {
                    switch (args[0].ToLower())
                    {
                        case "/t":
                        case "-t":
                        case "--text":
                            sourceBytes = Encoding.UTF8.GetBytes(args[1]);
                            break;
                        case "/f":
                        case "-f":
                        case "--file":
                            if (File.Exists(args[1]))
                            {
                                using (FileStream fs = new FileStream(args[1], FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    sourceBytes = new byte[fs.Length];
                                    fs.Read(sourceBytes, 0, sourceBytes.Length);
                                }
                            }
                            else
                            {
                                sourceBytes = Encoding.UTF8.GetBytes(args[1]);
                            }
                            break;
                        default:
                            sourceBytes = Encoding.UTF8.GetBytes(args[0]);
                            break;
                    }
                }
                Console.WriteLine(GetCRC32(sourceBytes));
            }
        }

        static string GetCRC32(byte[] sourceBytes)
        {
            int CRC32_TABLELENGTH = 256;
            int CRC32_BUFLENGTH = 255;
            uint[] crcTable = new uint[CRC32_TABLELENGTH];
            for (uint i = 0; i < CRC32_TABLELENGTH; i++)
            {
                var x = i;
                for (int j = 0; j < 8; j++)
                {
                    x = (uint)((x & 1) == 0 ? x >> 1 : -306674912 ^ x >> 1);
                }
                crcTable[i] = x;
            }
            uint num = uint.MaxValue;
            for (int i = 0; i < sourceBytes.Length; i++)
            {
                num = crcTable[(num ^ sourceBytes[i]) & CRC32_BUFLENGTH] ^ num >> 8;
            }
            byte[] retBytes = BitConverter.GetBytes((uint)(num ^ -1));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(retBytes);
            }
            return BitConverter.ToString(retBytes).Replace("-", "");
        }
    }
}
