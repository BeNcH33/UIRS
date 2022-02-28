using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Threading;

namespace Laboratory_Work_One
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Началась обработка и запись изображений на диск! ...");

            Thread q1Thr = new Thread(new ThreadStart(q1));
            q1Thr.Start();
            Thread q2Thr = new Thread(new ThreadStart(q2));
            q2Thr.Start();

            Thread q3Thr = new Thread(new ThreadStart(q6));
            q3Thr.Start();

            Thread q4Thr = new Thread(new ThreadStart(q8));
            q4Thr.Start();

            Thread q5Thr = new Thread(new ThreadStart(q9));
            q5Thr.Start();

            Console.WriteLine("Завершено!");

            Console.ReadKey();
        }

        static void q1 ()
        {
            ImageProcessing.PerformConversion("1.png", "Photo 1");
        }
        static void q2()
        {
            ImageProcessing.PerformConversion("2.png", "Photo 2");
        }
        static void q6()
        {
            ImageProcessing.PerformConversion("6.png", "Photo 6");
        }
        static void q8()
        {
            ImageProcessing.PerformConversion("8.png", "Photo 8");
        }
        static void q9()
        {
            ImageProcessing.PerformConversion("9.png", "Photo 9");
        }
    }
}
