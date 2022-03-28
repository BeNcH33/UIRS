using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Accord.Math;

namespace Laboratory_Work_One
{
    public partial class ImageProcessing
    {
        public const int WIDTH = 1024;
        public const int HEIGHT = 1024;

        public const int START_RANGE_X = 400;
        public const int END_RANGE_X = 600;

        public const int START_RANGE_Y = 400;
        public const int END_RANGE_Y = 600;

        public static void PerformConversion(string filePath, string resultPath)
        {
            var image = GetImageFromPath(filePath);

            Image<Gray, byte> greyImage = ToGray(image);
            greyImage = GetMedianFilter(greyImage);

            Image<Gray, byte> binarizedImage = GetBinarizedImage(greyImage);

            Gray color = new Gray(255);
            Gray black = new Gray(0);

            for (int i = 0; i < greyImage.Height; i++)
            {
                for (int j = 0; j < greyImage.Width; j++)
                {
                    if (!binarizedImage[j, i].Equals(black))
                    {
                        greyImage[j, i] = color;
                    }
                }
            }

            Directory.CreateDirectory(resultPath);

            for (int i = 0; i <= 360; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var random = new Random();
                    var movedToX = random.Next(START_RANGE_X, END_RANGE_X);
                    var movedToY = random.Next(START_RANGE_Y, END_RANGE_Y);

                    var resultImage = GetMovedImage(Rotate(greyImage, i), movedToX, movedToY);

                    resultImage.Save($"{resultPath}\\{i}-{movedToX}-{movedToY}.png");
                }
            }
        }

        private static Bitmap GetMovedImage(Image<Gray, byte> img, int movedToX, int movedToY)
        {
            Gray black = new Gray(0);

            Image<Gray, byte> binarizedImage = GetBinarizedImage(img);

            var sumX = 0;
            var sumY = 0;
            var count = 0;

            var originalShapeParams = new ShapeParams(WIDTH, HEIGHT);

            for (int width = 0; width < img.Width; width++)
            {
                for (int height = 0; height < img.Height; height++)
                {
                    if (binarizedImage[row: width, column: height].Equals(black))
                    {
                        count++;
                        sumX += width;
                        sumY += height;

                        originalShapeParams.SetShapeParams(byWidth: width, byHeight: height);
                    }
                }
            }

            var centerX = sumX / count;
            var centerY = sumY / count;

            var oldBitmap = img.Bitmap;
            var newBitmap = new Bitmap(WIDTH, HEIGHT);

            using (Graphics g = Graphics.FromImage(newBitmap)) { g.Clear(Color.White); }

            for (int width = originalShapeParams.Top; width <= originalShapeParams.Bottom; width++)
            {
                for (int height = originalShapeParams.Left; height <= originalShapeParams.Right; height++)
                {
                    var calcWidth = width + -(centerX - movedToX);
                    var calcHeight = height + -(centerY - movedToY);

                    var isOverRight = calcWidth >= WIDTH;
                    var isOverLeft = calcWidth < 0;

                    var isOverTop = calcHeight < 0;
                    var isOverBottom = calcHeight >= HEIGHT;

                    if (isOverRight || isOverLeft || isOverTop || isOverBottom) break;

                    newBitmap.SetPixel(calcWidth, calcHeight, oldBitmap.GetPixel(width, height));
                }
            }

            return newBitmap;
        }

        public static Image<Bgr, byte> GetImageFromPath(string path)
        {
            return new Image<Bgr, byte>(path);
        }

        public static Image<Gray, byte> ToGray(Image<Bgr, byte> img)
        {
            return img.Convert<Gray, byte>();
        }

        public static Image<Gray, byte> GetMedianFilter(Image<Gray, byte> img)
        {
            CvInvoke.MedianBlur(img, img, 5);

            return img;
        }

        public static Image<Gray, byte> GetBinarizedImage(Image<Gray, byte> img)
        {
            return img.ThresholdBinary(new Gray(150), new Gray(250));
        }

        public static Image<Gray, byte> Rotate(Image<Gray, byte> img, double angle)
        {
            return img.Rotate(angle, new Gray(255));
        }

        //======================= Laboratyro Work 2 ==============================

        public static void SelectOutlineImage(string directoryPath, string savePath, string resultPathsigns)
        {
            Directory.CreateDirectory(savePath);
            Directory.CreateDirectory(resultPathsigns);

            var filesPaths = FileManager.GetFilePathsFromDirectory(directoryPath); //Получили файлы готовой выборки

            foreach (var path in filesPaths)
            {
                var hierarchy = new Mat();
                var image = GetBinarizedImage(new Image<Gray, byte>(path));
                var contours = new VectorOfVectorOfPoint(); //Это из методички

                CvInvoke.FindContours(image.Mat, contours, hierarchy, Emgu.CV.CvEnum.RetrType.List, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                //Записывает контуры в переменную contours

                var vectorOfPoint = contours[0]; //Записываем координаты контуров фигуры

                //string resultPath = $"{savePath}\\{Path.GetFileNameWithoutExtension(path)}.csv"; //Создаем файл для контура
                string resultPathSigns = $"{resultPathsigns}\\{Path.GetFileNameWithoutExtension(path)}.csv"; //Создаем файлы для признаков

                //using (FileStream fs = File.Create(resultPath))
                //{
                //    fs.Close();
                //}
                using (FileStream fs = File.Create(resultPathSigns))
                {
                    fs.Close();
                }

                //using (StreamWriter sw = new StreamWriter(resultPath, false, Encoding.Default)) //В каждой папке записываем в csv файл
                //{
                //    List<Point> points = GetCorrectedVectorOfPoint(vectorOfPoint);    
                //    foreach (var point in points)
                //    {
                //        sw.WriteLine($"{point.X};{point.Y}");
                //    }
                //}

                using (StreamWriter sw = new StreamWriter(resultPathSigns, false, Encoding.Default)) //В каждой папке записываем в csv файл значения признаков
                {
                    List<double> points = CreateSigns(vectorOfPoint);

                    foreach (var point in points)
                    {
                        sw.WriteLine($"{point}");
                    }
                }
            }
        }

        private static List<Point> GetCorrectedVectorOfPoint(VectorOfPoint vectorOfPoint)
        {
            List<Point> listResult = new List<Point>();

            int[,] matrix = new int[WIDTH, HEIGHT];

            for (int i = 0; i < vectorOfPoint.Size; i++)
            {
                matrix[vectorOfPoint[i].X, vectorOfPoint[i].Y] = 1; //Идём по диагонали и закрашиваем чёрным
            }

            for (int x = 1; x < WIDTH - 2; x++)
            {
                for (int y = 1; y < HEIGHT - 2; y++)
                {
                    bool isCorrect = Masks.IsCorrect(new int[,] {
                        { matrix[x - 1, y - 1], matrix[x, y - 1], matrix[x + 1, y - 1] },
                        { matrix[x - 1, y], matrix[x, y], matrix[x + 1, y] },
                        { matrix[x - 1, y + 1], matrix[x, y + 1], matrix[x + 1, y + 1] }
                    });

                    if (isCorrect)
                    {
                        listResult.Add(new Point(x, y));

                    }
                }
            }

            return listResult;
        }



        //***********************Лабораторная работа 4**************************************//

        public static List<double> CreateSigns(VectorOfPoint vectorOfPoint)
        {
            List<double> listResult = new List<double>();
            LineFeatures.LineFeatures characteristicObject = new LineFeatures.LineFeatures(LineFeatures.LineFeatures.filteredPoint(ConvertVectorOfPointToList(vectorOfPoint)));

            //double P = CvInvoke.ArcLength(vectorOfPoint, true);
            //double S = CvInvoke.ContourArea(vectorOfPoint);

            int K = characteristicObject.point4Count;
            int T = characteristicObject.dConectPointCount;

            int M1 = characteristicObject.curv90Count;
            int M2 = characteristicObject.curvM90Count;
            int M3 = characteristicObject.curv135Count;
            int M4 = characteristicObject.curvM135Count;

            //int N1 = characteristicObject.n1Count;
            //int N2 = characteristicObject.n2Count;
            //int N3 = characteristicObject.n3Count;
            //int N4 = characteristicObject.n4Count;
            //int N5 = characteristicObject.n5Count;
            //int N6 = characteristicObject.n6Count;
            //int N7 = characteristicObject.n7Count;
            //int N8 = characteristicObject.n8Count;


            //double k1 = P / S;
            //double k2 = M1 / S;
            //double k3 = M2 / S;
            //double k4 = M3 / S;
            //double k5 = M4 / S;
            //double k6 = T / S;
            //double k7 = K / S;
            //double k8 = (double)M1 / P;
            //double k9 = M2 / P;
            //double k10 = (double)M3 / P;
            //double k11 = M4 / P;
            //double k12 = (double)K / P;
            //double k13 = T / P;
            //double k14 = (double)M1 / (M1 + M2 + M3 + M4);
            //double k15 = (double)M2 / (M1 + M2 + M3 + M4);
            //double k16 = (double)M3 / (M1 + M2 + M3 + M4);
            //double k17 = (double)M4 / (M1 + M2 + M3 + M4);
            //double k18 = (M1 + M2 + M3 + M4) / (P + S + K + T);
            //double k19 = (K + T) / (P + S);
            //double k20 = (M1 + M2 + M3 + M4) / (P + S);
            //double k21 = (double)(M1 + M2 + M3 + M4) * K / (P * S);
            //double k22 = (double)(M1 + M2 + M3 + M4) * T / (P * S);






            


            //********************************************2 СЕМЕСТР***************************************************//








            //double N0 = CvInvoke.ArcLength(vectorOfPoint, true);
            int N1 = characteristicObject.n1Count;
            //int N2 = characteristicObject.n2Count;
            int N3 = characteristicObject.n3Count;
            //int N4 = characteristicObject.n4Count;
            int N5 = characteristicObject.n5Count;
            //int N6 = characteristicObject.n6Count;
            int N7 = characteristicObject.n7Count;
            //int N8 = characteristicObject.n8Count;
            //double N9 = (K + T);
            //double N10 = (M1 + M2);
            //double N11 = (M3 + M4);
            //double N12 = M1;
            //double N13 = M2;
            //double N14 = M3;
            //double N15 = M4;
            double N16 = (M1 + M3);
            //double N17 = (M2 + M4);
            double N18 = K;
            //double N19 = T;

            double Np = N1 + N3 + N5 + N7 + N16 + N18;

            double k1 = N1 / Np;
            double k2 = N3 / Np;
            double k3 = N5 / Np;
            double k4 = N7 / Np;
            double k5 = N16 / Np;
            double k6 = N18 / Np;

            listResult.Add(k1);
            listResult.Add(k2);
            listResult.Add(k3);
            listResult.Add(k4);
            listResult.Add(k5);
            listResult.Add(k6);

            return listResult;
        }

        private static List<Point> ConvertVectorOfPointToList(VectorOfPoint vector)
        {
            List<Point> listPoint = new List<Point>();
            var list = vector.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                listPoint.Add(new Point(list[i].X, list[i].Y));
            }
            return listPoint;
        }

        //***********************Лабораторная работа 5**************************************//

        public static void CheckClassPointOfList(List<List<List<double>>> listCheck, List<List<List<double>>> listTest)
        {
            double p = 0;

            List<List<double>> check = new List<List<double>>();

            foreach (var item in listTest)
            {
                double item0 = 0;
                double item1 = 0;
                double item2 = 0;
                double item3 = 0;
                double item4 = 0;
                double item5 = 0;

                foreach (var it in item)
                {
                    if (it.Count > 0)
                    {
                        item0 += it[0];
                        item1 += it[1];
                        item2 += it[2];
                        item3 += it[3];
                        item4 += it[4];
                        item5 += it[5];
                    }
                }
                check.Add(new List<double>() { item0/1600, item1/1600, item2/1600, item3/1600, item4/1600, item5/1600 });

            }

            int T = 0;
            for (int i = 0; i < listCheck.Count; i++)
            {
                int D = 0;
                for (int j = 0; j < listCheck[i].Count; j++)
                {
                    var itemPoint = listCheck[i][j];

                    if (itemPoint.Count > 0)
                    {
                        if (GetMinLastCount(itemPoint, check) == i) D++;
                    }
                }
                T += D;
                Console.WriteLine((double)D /400);
            }
            Console.WriteLine((double)T / 2000);

        }
        private static int GetMinLastCount(List<double> listCheck, List<List<double>> listTest)
        {
            List<double> results = new List<double>();

            foreach (var item in listTest)
            {
                results.Add(GetDistances(listCheck, item));
            }

            return results.IndexOf(results.Min());
        }
        private static int GetMinCount(List<List<List<double>>> listTest, List<double> listCheck)
        {
            List<double> results = new List<double>();

            foreach (var item in listTest)
            {
                List<double> listResult = new List<double>();

                for (int index = 0; index < item.Count; index++)
                {
                    listResult.Add(GetDistances(listCheck, item[index]));
                }

                results.Add(listResult.Min());
            }

            return results.IndexOf(results.Min());
        }
        private static double GetDistances(List<double> listCheck, List<double> listTest)
        {
            var doubleSum = 0.0;

            for (int x = 0; x < 6; x++)
            {
                //if (x == 1) continue;// исключаем признаки
                doubleSum += (listCheck[x] - listTest[x]) * (listCheck[x] - listTest[x]);
            }

            return Math.Sqrt(doubleSum);
        }

        //*****************************************************************************//


        public static List<List<List<double>>> GetListPoin(List<string> filePaths, string typeMethod)
        {
            List<List<List<double>>> listOfPoints = new List<List<List<double>>>();

            for (int i = 0; i < filePaths.Count; i++)
            {
                if (typeMethod == "test")
                {
                    var vectors = GetVectorsTest(filePaths[i]);
                    listOfPoints.Add(vectors);
                }
                else
                {
                    var vectors = GetVectorsCheck(filePaths[i]);
                    listOfPoints.Add(vectors);
                }
            }

            return listOfPoints;
        }
        public static List<List<double>> GetVectorsTest(string folderPath)
        {
            List<List<double>> listResult = new List<List<double>>();
            var filesPaths = FileManager.GetFilePathsFromDirectory(folderPath);

            for (int i = 400; i < 2000; i++)
            {
                listResult.Add(GetVectorsByFilePath(filesPaths[i]));
            }

            return listResult;
        }
        public static List<List<double>> GetVectorsCheck(string folderPath)
        {
            List<List<double>> listResult = new List<List<double>>();
            var filesPaths = FileManager.GetFilePathsFromDirectory(folderPath);

            for (int i = 0; i < 400; i++)
            {
                listResult.Add(GetVectorsByFilePath(filesPaths[i]));
            }

            return listResult;
        }
        public static List<double> GetVectorsByFilePath(string path)
        {
            List<double> vectors = new List<double>();

            using (FileStream fileStream = File.OpenRead(path))
            {
                byte[] array = new byte[fileStream.Length];

                fileStream.Read(array, 0, array.Length);

                string text = Encoding.Default.GetString(array);

                string[] splitText = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                foreach (var line in splitText)
                {
                    if (line == "") continue;

                    var splitBySeparator = line.Split(new char[] { ';' });

                    foreach (var value in splitBySeparator)
                    {
                        if (value == "") continue;

                        vectors.Add(double.Parse(value));
                    }
                }
            }
            return vectors;
        }

        //*****************************New_Lab_3***********************************//
        //public static void CalculateLab3(List<List<List<double>>> listOfTestVectors, List<List<List<double>>> listOfCheckVectors)
        //{
        //    double[,] X = CountAvgX(listOfTestVectors);
        //    double[,] matrixA = CountMatrixA(listOfTestVectors, X);

        //    int g = listOfTestVectors.Count;
        //    int nkTest = listOfTestVectors[0].Count;
        //    int nkCheck = listOfCheckVectors[0].Count;
        //    int p = listOfTestVectors[0][0].Count;

        //    double[,] B = CountBki(matrixA, X, g, nkTest, p);
        //    double[] B0 = CountBk0(X, g, p, B);

        //    double[,] recognizeArray = new double[g, nkCheck];

        //    for (int k = 0; k < g; k++)
        //    {
        //        for (int n = 0; n < nkCheck; n++)
        //        {
        //            for (int i = 0; i < p; i++)
        //            {
        //                recognizeArray[k, n] = CountMaxHk(X, g, p, B, B0, listOfCheckVectors[k][n].ToArray());
        //            }
        //        }
        //    }

        //}

        //public static double[,] CountAvgX(List<List<List<double>>> listOfVectors)
        //{
        //    double[,] X = new double[listOfVectors.Count, listOfVectors[0][0].Count];
        //    for (int a = 0; a < listOfVectors.Count; a++)
        //    {
        //        for (int y = 0; y < listOfVectors[0].Count; y++)
        //        {
        //            for (int u = 0; u < listOfVectors[0][0].Count; u++)
        //            {
        //                if (listOfVectors[a][y].Count > 0)
        //                {
        //                    X[a, u] += listOfVectors[a][y][u];
        //                }
        //            }
        //        }
        //    }
        //    for (int a = 0; a < listOfVectors.Count; a++)
        //    {
        //        for (int u = 0; u < listOfVectors[0][0].Count; u++)
        //        {
        //            X[a, u] = X[a, u] / listOfVectors[0].Count;
        //        }
        //    }
        //    return X;
        //}
        //public static double[,] CountMatrixA(List<List<List<double>>> listOfVectors, double[,] X)
        //{

        //    double[,] matrixW = new double[listOfVectors[0][0].Count, listOfVectors[0][0].Count];

        //    double temp1 = X.GetLength(0);
        //    double temp2 = X.GetLength(1);

        //    for (int i = 0; i < listOfVectors[0][0].Count; i++)
        //    {
        //        for (int j = 0; j < listOfVectors[0][0].Count; j++)
        //        {
        //            double sumK = 0;
        //            for (int k = 0; k < listOfVectors.Count; k++)
        //            {
        //                double sumM = 0;
        //                for (int m = 0; m < listOfVectors[0].Count; m++)
        //                {
        //                    if (listOfVectors[k][m].Count > 0)
        //                    {
        //                        sumM += (listOfVectors[k][m][i] - X[k, i]) * (listOfVectors[k][m][j] - X[k, j]);
        //                    }
        //                }
        //                sumK += sumM;
        //            }
        //            matrixW[i, j] = sumK;
        //        }
        //    }

        //    double[,] matrixA = matrixW.Inverse();

        //    return matrixA;
        //}

        //public static double[,] CountBki(double[,] matrixA, double[,] X, int g, int nk, int p)
        //{
        //    int n = nk * g;
        //    double[,] B = new double[g, p];
        //    for (int k = 0; k < g; k++)
        //    {
        //        for (int i = 0; i < p; i++)
        //        {
        //            double sum = 0;
        //            for (int j = 0; j < p; j++)
        //            {
        //                sum += matrixA[i, j] * X[k, j];
        //            }
        //            B[k, i] = (n - g) * sum;
        //        }
        //    }
        //    return B;
        //}

        //public static double[] CountBk0(double[,] X, int g, int p, double[,] B)
        //{
        //    double[] B0 = new double[g];
        //    for (int k = 0; k < g; k++)
        //    {
        //        double sum = 0;
        //        for (int i = 0; i < p; i++)
        //        {
        //            for (int j = 0; j < p; j++)
        //            {
        //                sum += B[k, i] *  X[k, j];
        //            }
        //        }
        //        B0[k] = -0.5 * sum;
        //    }
        //    return B0;
        //}

        //public static double CountMaxHk(double[,] X, int g, int p, double[,] B, double[] B0, double[] x)
        //{
        //    double[] H = new double[g];
        //    for (int k = 0; k < g; k++)
        //    {
        //        H[k] = B0[k];
        //        double sum = 0;
        //        for (int i = 0; i < p; i++)
        //        {
        //            sum += B[k, i] * x[i];  
        //        }
        //    }
        //    int indexOfMaxElementOfH = -1;
        //    double maxH = Double.MinValue;

        //    for (int l = 0; l < H.Length; l++)
        //    {
        //        if (H[l] > maxH)
        //        {
        //            maxH = H[l];
        //            indexOfMaxElementOfH = l;
        //        }
        //    }

        //    return indexOfMaxElementOfH;
        //}


        public static void PerformLaboratoryWorkThreeSemesterTwo(List<List<List<double>>> trainingSample, List<List<List<double>>> recognitionSample)
        {
            var X = GetAverageValues(trainingSample);

            var matrixA = GetMatrixA(trainingSample, X);

            int numberOfClasses = trainingSample.Count;
            int numberOfObservations = trainingSample[0].Count;
            int numberOfFeatures = trainingSample[0][0].Count;

            var Bki = GetBki(matrixA, X, numberOfClasses, numberOfObservations, numberOfFeatures);

            var Bk0 = GetBk0(X, numberOfClasses, numberOfFeatures, Bki);

            List<int[]> recognitionArray = new List<int[]>();
            for (int k = 0; k < numberOfClasses; k++)
            {
                for (int m = 0; m < recognitionSample[0].Count; m++)
                {
                    var result = GetNumberOfClassTestSample(X, numberOfClasses, numberOfFeatures, Bki, Bk0, recognitionSample[k][m]);
                    Console.WriteLine("(" + k + ";" + m + "): " + result);
                    recognitionArray.Add(new int[2] {k, (int)result} );
                }
            }

            CountRecognitionStatistics(numberOfClasses, recognitionSample[0].Count, recognitionArray);

            return;
        }

        private static void CountRecognitionStatistics(int numberOfClasses, int numberOfObservations, List<int[]> recognitionArray)
        {
            double[] statisticsArray = new double[numberOfClasses + 1];
            for (int k = 0; k < numberOfClasses; k++)
            {
                for (int m = k * numberOfObservations; m < (k + 1) * numberOfObservations; m++)
                {
                    if (recognitionArray[m][0] == recognitionArray[m][1])
                    {
                        statisticsArray[k] += 1;
                        statisticsArray[statisticsArray.Length - 1] += 1;
                    }
                }
            }
            for (int k = 0; k < numberOfClasses; k++)
            {
                statisticsArray[k] = statisticsArray[k] / numberOfObservations;
                Console.WriteLine(statisticsArray[k]);
            }
            statisticsArray[statisticsArray.Length - 1] = statisticsArray[statisticsArray.Length - 1] / (numberOfClasses * numberOfObservations);
            Console.WriteLine(statisticsArray[statisticsArray.Length - 1]);
        }

        private static double[,] GetMatrixA(List<List<List<double>>> trainingSample, double[,] X)
        {
            int countOfFeatures = trainingSample[0][0].Count;

            double[,] matrixW = new double[countOfFeatures, countOfFeatures];

            for (int i = 0; i < countOfFeatures; i++)
            {
                for (int j = 0; j < countOfFeatures; j++)
                {
                    double sumK = 0;
                    for (int k = 0; k < trainingSample.Count; k++)
                    {
                        double sumM = 0;
                        for (int m = 0; m < trainingSample[0].Count; m++)
                        {
                            if (trainingSample[k][m].Count > 0)
                            {
                                sumM += (trainingSample[k][m][i] - X[k, i]) * (trainingSample[k][m][j] - X[k, j]);
                            }
                        }
                        sumK += sumM;
                    }

                    matrixW[i, j] = sumK;
                }
            }

            return matrixW.Inverse();
        }

        private static double[,] GetAverageValues(List<List<List<double>>> trainingSample)
        {
            double[,] result = new double[trainingSample.Count, trainingSample[0][0].Count];
            for (int m = 0; m < trainingSample[0][0].Count; m++)
            {
                for (int i = 0; i < trainingSample.Count; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < trainingSample[i].Count; j++)
                    {
                        if (trainingSample[i][j].Count > 0)
                        {
                            sum += trainingSample[i][j][m];
                        }
                    }

                    result[i, m] = sum / trainingSample[0].Count;
                }
            }

            return result;
        }

        private static double[,] GetBki(double[,] matrixA, double[,] averageValues, int numberOfClasses, int numberOfObservations, int numberOfFeatures)
        {
            int n = numberOfObservations * numberOfClasses;

            double[,] result = new double[numberOfClasses, numberOfFeatures];

            for (int k = 0; k < numberOfClasses; k++)
            {
                for (int i = 0; i < numberOfFeatures; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < numberOfFeatures; j++)
                    {
                        sum += matrixA[i, j] * averageValues[k, j];
                    }

                    result[k, i] = (n - numberOfClasses) * sum;
                }
            }

            return result;
        }

        private static double[] GetBk0(double[,] averageValues, int numberOfClasses, int numberOfFeatures, double[,] Bki)
        {
            double[] result = new double[numberOfClasses];
            for (int k = 0; k < numberOfClasses; k++)
            {
                double sum = 0;
                for (int i = 0; i < numberOfFeatures; i++)
                {
                    sum += Bki[k, i] * averageValues[k, i];
                }

                result[k] = -0.5 * sum;
            }

            return result;
        }

        public static double GetNumberOfClassTestSample(double[,] X, int numberOfClasses, int numberOfFeatures, double[,] Bki, double[] Bk0, List<double> testSample)
        {
            double[] H = new double[numberOfClasses];
            for (int k = 0; k < numberOfClasses; k++)
            {
                H[k] = Bk0[k];
                for (int i = 0; i < numberOfFeatures; i++)
                {
                    if (testSample.Count > 0)
                    {
                        H[k] += Bki[k, i] * testSample[i];
                    }
                    else
                    {
                        H[k] = 0;
                    }
                }
            }

            return H.IndexOf(H.Max());
        }
    }
}
