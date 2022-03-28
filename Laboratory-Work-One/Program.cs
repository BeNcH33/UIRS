using System;
using System.Collections.Generic;

namespace Laboratory_Work_One
{
    class Program
    {

        public const string ORIGIN_PATH = @"Z:\UIRS\PHOTO";

        public const string RESULT_PATH = @"Z:\UIRS\CSV";
        public const string RESULTSIGNS_PATH = @"Z:\UIRS\SIGNS";

        static void Main(string[] args)
        {
            Console.WriteLine("Началась обработка и запись изображений на диск! ...");

           // ImageProcessing.SelectOutlineImage($"{ORIGIN_PATH}\\Photo 9", $"{RESULT_PATH}\\Report 9", $"{RESULTSIGNS_PATH}\\Signs 9");

            
            List<List<List<double>>> listTestOfPoints = ImageProcessing.GetListPoin(new List<string>()
            {
                RESULTSIGNS_PATH + @"\Signs 1",
                RESULTSIGNS_PATH + @"\Signs 3",
                RESULTSIGNS_PATH + @"\Signs 5",
                RESULTSIGNS_PATH + @"\Signs 7",
                RESULTSIGNS_PATH + @"\Signs 9",
            }, "test");

            List<List<List<double>>> listCheckOfPoints = ImageProcessing.GetListPoin(new List<string>()
            {
                RESULTSIGNS_PATH + @"\Signs 1",
                RESULTSIGNS_PATH + @"\Signs 3",
                RESULTSIGNS_PATH + @"\Signs 5",
                RESULTSIGNS_PATH + @"\Signs 7",
                RESULTSIGNS_PATH + @"\Signs 9",
            }, "point");

            //ImageProcessing.CheckClassPointOfList(listCheckOfPoints, listTestOfPoints);

            ImageProcessing.PerformLaboratoryWorkThreeSemesterTwo(listTestOfPoints, listCheckOfPoints);

            Console.WriteLine("Завершено!");
            Console.WriteLine();

            Console.ReadKey();
        }
    }
}
