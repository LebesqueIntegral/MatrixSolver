using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpLibrary;
using System.Runtime.InteropServices;

namespace MySolution
{
    
    class Program
    {
        [DllImport("C:\\Users\\1\\Desktop\\Projects\\MySolution\\Debug\\CppLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void extern_func(int count, int order, double[] matrix, double[] vec, double[] sol, ref double time);

        public static void Test()
        {
            double[] matrix = new double[12] { 2.0, 6.0, 1.0, 1.0, 2.0, 3.0, 3.0, 4.0, 5.0, 7.0, 4.0, 2.0};
            double[] vec = new double[6] { 2.0, 4.0, 3.0, 7.0, 5.0, 1.0 };
            double[] sol = new double[6];
            double time = 0;


            double[] e1 = { 1.0, 2.0, 3.0 };
            double[] e2 = { 3.0, 4.0, 5.0 };
            double[] e3 = { 2.0, 6.0, 1.0 };
            double[] e4 = { 7.0, 4.0, 2.0 };

            double[][] e5 = new double[2][];
            e5[0] = new double[3] { 2.0, 4.0, 6.0 };
            e5[1] = new double[3] { 1.0, 2.0, 3.0 };


            ElemMat[] diag = new ElemMat[2];
            diag[0] = new ElemMat(e1);
            diag[1] = new ElemMat(e2);


            ElemMat[] above = new ElemMat[1];
            above[0] = new ElemMat(e3);
            ElemMat[] under = new ElemMat[1];
            under[0] = new ElemMat(e4);


            Vector[] F = new Vector[2];
            F[0] = new Vector(new double[3] { 2.0, 4.0, 3.0 });
            F[1] = new Vector(new double[3] { 7.0, 5.0, 1.0 });

            MainMat mainMat = new MainMat(2, 3, diag, above, under);
            MainMat.Solve(mainMat, F);
            mainMat.ToFile("Tets.txt");

            Console.WriteLine("Test C#:");
            foreach (var item in mainMat.Test)
            Console.Write(item + " ");
            Console.WriteLine();


            extern_func(2, 3, matrix, vec, sol, ref time);

            Console.WriteLine("Solution C++:");
            for (int i = 0; i < 2 * 3; i++)
            {
                Console.Write(sol[i] + " ");
            }
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            Test();
            TestTime test = new TestTime();
            TestTime.Load("Serialized", ref test);

            int order = 1;
            int count = 1;
            int check = 1;

            Console.WriteLine("Приложение <Решатель Блочных Трехдиагональных матриц> готово для решения любых задач (ну почти)");
Label1:     while (check == 1)
            {
                Console.Write("\nВведите порядок матриц-блоков - order: ");
                order = Int32.Parse(Console.ReadLine());
                Console.WriteLine();

                if (order <= 0)
                {
                    Console.WriteLine("Порядок матрицы не может быть нулевым или отрицательным");
                    Console.WriteLine("Попробуйте заново");
                    goto Label1;
                }

Label2:         Console.Write("Введите количество матриц-блоков - count: ");
                count = Int32.Parse(Console.ReadLine());
                Console.WriteLine();

                if (count <= 0)
                {
                    Console.WriteLine("Количество матриц-блоков не может быть нулевым или отрицательным");
                    Console.WriteLine("Попробуйте заново");
                    goto Label2;
                }

                MainMat mainMat = new MainMat(count, order);

                var before = DateTime.Now;
                MainMat.Solve(mainMat, mainMat.F);
                var after = DateTime.Now;

                var DeltaTime = (after - before).TotalSeconds;

                double time = 0;
                double[] sol = new double[count * order];
                extern_func(count, order, null, null, sol, ref time);
                string s = "Blocks Order: " + order + ";   " + "Blocks Counts: " + count + "\n" + "C# Time: " + DeltaTime + ";   " + "C++ Time: " + time + "\n" + "Coefficient: " + DeltaTime / time + "\n" + "||||||||||||||||||||||||||||||||||||||" + "\n";
                test.Add(s);

                Console.WriteLine("Если хотите продолжить работу, введите - 1");
                Console.WriteLine("Если хотите завершить работу, введите - 0");
                check = Int32.Parse(Console.ReadLine());
            }

            TestTime.Save("Serialized", test);
            Console.WriteLine(test);
            Console.ReadKey();
            return;
        }
    }
}
