using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSharpLibrary
{
    public class MainMat
    {
        public int count;
        public ElemMat[] diagonal;
        public ElemMat[] under;
        public ElemMat[] above;
        public Vector[] X;
        public Vector[] F;
        public Vector[] Test;

        public int order
        {
            get
            {
                return diagonal[0].order;
            }
        }
        public MainMat(int blocks_count, int blocks_order)
        {
            if (blocks_count <= 0 || blocks_order <= 0)
                    throw new Exception("Wrong parametrs: count or/and order in MainMat");
            count = blocks_count;
            diagonal = new ElemMat[count];
            under = new ElemMat[count - 1];
            above = new ElemMat[count - 1];

            X = new Vector[count];
            F = new Vector[count];
            Test = new Vector[count];
            int i = 0;

            for ( ; i < count - 1; i++)
            {
                diagonal[i] = new ElemMat(blocks_order);
                under[i] = new ElemMat(blocks_order);
                above[i] = new ElemMat(blocks_order);

                X[i] = new Vector(blocks_order);
                F[i] = new Vector(blocks_order);
                Test[i] = new Vector(blocks_order);
            }

            diagonal[i] = new ElemMat(blocks_order);
            X[i] = new Vector(blocks_order);
            F[i] = new Vector(blocks_order);
            Test[i] = new Vector(blocks_order);
           
            for (i = 0; i < blocks_count - 1; i++)
                for (int j = 0; j < blocks_order; j++)
                {
                    diagonal[i][j] = i + j + 1;
                    above[i][j] = i + 2 + j + 5;
                    under[i][j] = i + 3 + j + 1;
                }


            diagonal[i][blocks_order - 1] = 1;
        }

        public MainMat(int blocks_count, int blocks_order, ElemMat[] diag, ElemMat[] ab, ElemMat[] und)
        {
            if (blocks_count <= 0 || blocks_order <= 0)
                throw new Exception("Wrong parametrs: count or/and order in MainMat");
            count = blocks_count;
            diagonal = new ElemMat[count];
            under = new ElemMat[count - 1];
            above = new ElemMat[count - 1];

            X = new Vector[count];
            F = new Vector[count];
            Test = new Vector[count];
            int i = 0;

            for (; i < count - 1; i++)
            {
                diagonal[i] = new ElemMat(blocks_order);
                under[i] = new ElemMat(blocks_order);
                above[i] = new ElemMat(blocks_order);

                X[i] = new Vector(blocks_order);
                F[i] = new Vector(blocks_order);
                Test[i] = new Vector(blocks_order);
            }

            diagonal[i] = new ElemMat(blocks_order);
            X[i] = new Vector(blocks_order);
            F[i] = new Vector(blocks_order);
            Test[i] = new Vector(blocks_order);

            for (i = 0; i < blocks_count - 1; i++)
                for (int j = 0; j < blocks_order; j++)
                {
                    diagonal[i][j] = diag[i][j];
                    above[i][j] = ab[i][j];
                    under[i][j] = und[i][j];
                }


            diagonal[blocks_count - 1] = diag[blocks_count - 1];
        }

        public static Vector[] operator*(MainMat ob1, Vector[] vec)
        {
            if (ob1.count != vec.Length || ob1.order != vec[0].Length)
                throw new Exception("Wrong parametrs: ob1.count != vec.Length  or  ob1.order != vec[0].Length");
            Vector[] result = new Vector[vec.Length];
            for (int i = 0; i < vec.Length; i++)
                result[i] = new Vector(ob1.order);

            for (int i = 0; i < ob1.order; i++)
                result[0][i] = ob1.diagonal[0][i] * vec[0][i] + ob1.above[0][i] * vec[1][i];

            for (int i = ob1.order; i < ob1.order*ob1.count - ob1.order; i++)
                result[i / ob1.order][i % ob1.order] = ob1.under[i / ob1.order - 1][i % ob1.order] * vec[i / ob1.order - 1][i % ob1.order] + ob1.diagonal[i / ob1.order][i % ob1.order] * vec[i / ob1.order][i % ob1.order] + ob1.above[i / ob1.order][i % ob1.order] * vec[i / ob1.order + 1][i % ob1.order];

            for (int i = 0; i < ob1.order; i++)
                result[vec.Length - 1][i] = ob1.under[vec.Length - 2][i] * vec[vec.Length - 2][i] + ob1.diagonal[vec.Length - 1][i] * vec[vec.Length - 1][i];

            return result;
        }

        public static Vector[] Solve(MainMat mainMat, Vector[] vec)
        {
            if (mainMat.count != vec.Length || mainMat.order != vec[0].Length)
                throw new Exception("Wrong parametrs: ob1.count != vec.Length  or  ob1.order != vec[0].Length");

            int M = mainMat.order;
            int N = mainMat.count;
            ElemMat[] alpha = new ElemMat[N - 1];
            
            Vector[] beta = new Vector[N];
            for (int i = 0; i < N; i++)
                beta[i] = new Vector(M);

            Vector[] X = new Vector[N];
            for (int i = 0; i < N; i++)
                X[i] = new Vector(M);

            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                mainMat.F[i][j] = vec[i][j];

            alpha[0] = ElemMat.reverse(mainMat.diagonal[0]) * mainMat.above[0];
            for (int i = 1; i < N-1; i++)
                alpha[i] = ElemMat.reverse(mainMat.diagonal[i] - (mainMat.under[i-1] * alpha[i - 1])) * mainMat.above[i];

            beta[0] = ElemMat.reverse(mainMat.diagonal[0]) * vec[0];
            for (int i = 1; i < N; i++)
            {
                beta[i] = ElemMat.reverse(mainMat.diagonal[i] - (mainMat.under[i - 1] * alpha[i - 1])) * (vec[i] - (mainMat.under[i - 1] * beta[i - 1]));
            }
        

            X[N - 1] = beta[N - 1];
            for (int i = N - 2; i >= 0; i--)
            {
                    X[i] = beta[i] - (alpha[i] * X[i + 1]);
            }

            mainMat.X = X;
            mainMat.Test = mainMat * X;
            return X;
        }

        public void ToFile(string s)
        {
            using (StreamWriter sw = new StreamWriter(s, false, System.Text.Encoding.Default))
            {
                string str = "";
                str += this.ToString();

                str += "\nVECTOR F\n";
                for (int i = 0; i < F.Length; i++)
                    str += F[i].ToString();

                str += "\nSOLUTION VECTOR\n";
                for (int i = 0; i < X.Length; i++)
                    str += X[i].ToString();

                str += "\n\nMY TEST ANSWER\n";
                for (int i = 0; i < Test.Length; i++)
                    str += Test[i].ToString();

                sw.WriteLine(str);
            }
        }

        public override string ToString()
        {
            string s = "MATRIX \n\n";

            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < i; j++)
                    s += "0" + ' ';
                s += diagonal[0].elements[i].ToString() + ' ';
                for (int j = 0; j < order - 1; j++)
                    s += "0" + ' ';
                s += above[0].elements[i].ToString();
                for (int j = order + i + 1; j < order*count; j++)
                    s +=' ' + "0";
                s += '\n';
            }
            for (int i = order; i < order*count-order; i++)
            {
                for (int j = 0; j < i - order; j++)
                    s += "0" + ' ';
                s += under[i/order - 1].elements[i%order].ToString() + ' ';
                for (int j = 0; j < order - 1; j++)
                    s += "0" + ' ';
                s += diagonal[i/order].elements[i%order].ToString() + ' ';
                for (int j = 0; j < order - 1; j++)
                    s += "0" + ' ';
                s += above[i/order].elements[i%order].ToString();
                for (int j = order + i + 1; j < order * count; j++)
                    s += ' ' + "0";
                s += '\n';
            }

            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < order * (count-2) + i; j++)
                    s += "0" + ' ';
                s += under[count-2].elements[i].ToString() + ' ';
                for (int j = 0; j < order - 1; j++)
                    s += "0" + ' ';
                s += diagonal[count-1].elements[i].ToString();
                for (int j = i; j < order-1; j++)
                    s += ' ' + "0";
                s += '\n';
            }
            return s;
        }
    }

    
}
