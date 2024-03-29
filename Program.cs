﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Drawing;

namespace ImageDownloader
{
    class Program
    {

        public static Icon IconFromImage(Image img)
        {
            var ms = new System.IO.MemoryStream();
            var bw = new System.IO.BinaryWriter(ms);
            // Header
            bw.Write((short)0);   // 0 : reserved
            bw.Write((short)1);   // 2 : 1=ico, 2=cur
            bw.Write((short)1);   // 4 : number of images
                                  // Image directory
            var w = img.Width;
            if (w >= 256) w = 0;
            bw.Write((byte)w);    // 0 : width of image
            var h = img.Height;
            if (h >= 256) h = 0;
            bw.Write((byte)h);    // 1 : height of image
            bw.Write((byte)0);    // 2 : number of colors in palette
            bw.Write((byte)0);    // 3 : reserved
            bw.Write((short)0);   // 4 : number of color planes
            bw.Write((short)0);   // 6 : bits per pixel
            var sizeHere = ms.Position;
            bw.Write((int)0);     // 8 : image size
            var start = (int)ms.Position + 4;
            bw.Write(start);      // 12: offset of image data
                                  // Image data
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var imageSize = (int)ms.Position - start;
            ms.Seek(sizeHere, System.IO.SeekOrigin.Begin);
            bw.Write(imageSize);
            ms.Seek(0, System.IO.SeekOrigin.Begin);

            // And load it
            return new Icon(ms);
        }


        static void Main(string[] args)
        {


            

            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                // client.DownloadFile("https://www.google.com/search?q=bulldog&tbm=isch", @"C:\localfile.html");
                //string[] linhas = File.ReadAllLines(@"C:\localfile.html");

                string pesquisa = Console.ReadLine();

                string[] urls = new string[20];
                string linhas = client.DownloadString("https://www.google.com/search?q="+pesquisa+"&tbm=isch");
                linhas = linhas.Substring(linhas.IndexOf("class=\"images_table\""));

                for (int i = 0; i < 20; i++) {
                    linhas = linhas.Substring(linhas.IndexOf("src=\"") + 5);
                    urls[i] = linhas.Substring(0, linhas.IndexOf("\""));
                    Console.Write(urls[i]);
                    client.DownloadFile(urls[i], @"C:\testimg\image(" + i + ").jpg");
                    Console.Write(" downloaded.\n");
                }

                using (FileStream stream = File.OpenWrite(@"C:\testimg\test.ico"))
                {
                    Image img = Image.FromFile(@"c:\testimg\image(0).jpg");
                    img = (Image)(new Bitmap(img, new Size(32,32)));
                    IconFromImage(img).Save(stream);
                }

                Console.ReadLine();
                
            }



        }
    }
}
