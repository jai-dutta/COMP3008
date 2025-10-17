using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    internal class DatabaseGenerator
    {
        readonly Random rand = new Random();

        List<string> firstNames = new List<string> 
        {
            "John", "Steve", "Matthew", "Adam", "Bob", "Bartholomew", "Jack"
        };

        List<string> lastNames = new List<string>
        {
            "Smith", "Green", "Woods", "Black", "Harding", "Morrison"
        };

        private string GetFirstname()
        {
            int index = rand.Next(firstNames.Count);

            return firstNames[index];
        }

        private string GetLastname()
        {
            int index = rand.Next(lastNames.Count);

            return lastNames[index];
        }

        private uint GetPIN()
        {
            return (uint)rand.Next(1000, 10000);
        }

        private uint GetAcctNo()
        {
            return (uint)(rand.Next(1000000, 10000000));
        }

        private int GetBalance()
        {
            return (rand.Next(-1000, 5000));
        }

        private byte[] GetProfilePicture()
        {

            int width = 16;
            int height = 16;


            using (var bitmap = new Bitmap(width, height))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int colour = rand.Next(0, 3);
                        switch (colour)
                        {
                            case 0: 
                                bitmap.SetPixel(x, y, Color.FromArgb(255, 0, 0));
                                break;
                            case 1: 
                                bitmap.SetPixel(x, y, Color.FromArgb(0, 255, 0));
                                break;
                            case 2: 
                                bitmap.SetPixel(x, y, Color.FromArgb(0, 0, 255));
                                break;
                        }
                    }
                }

                using (var memoryStream = new MemoryStream())
                {

                    bitmap.Save(memoryStream, ImageFormat.Jpeg);
                    return memoryStream.ToArray();
                }
            }
        }

        public void GetNextAccount(out string firstName, out string lastName, out uint pin, out uint acctNo, out int balance, out byte[] profilePicture)
        {
            firstName = GetFirstname();
            lastName = GetLastname();
            pin = GetPIN();
            acctNo = GetAcctNo();
            balance = GetBalance();
            profilePicture = GetProfilePicture();
        }
    }
}
