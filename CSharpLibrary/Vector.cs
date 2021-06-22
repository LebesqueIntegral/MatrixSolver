using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLibrary
{
    public class Vector
    {
        public double[] elements;
        public Vector(int order)
        {
            if (order < 0)
                throw new Exception("Wrong order for Vector");
            else if (order == 0)
            {
                order = 0;
                elements = null;
            }
            else
            {
                elements = new double[order];
                for (int i = 0; i < order; i++)
                    elements[i] = (i + 1) * (i + 1);
            }
        }

        public Vector(double[] vec)
        {
            if (vec == null)
                elements = null;
            else
            {
                elements = new double[vec.Length];
                for (int i = 0; i < vec.Length; i++)
                    elements[i] = vec[i];
            }
        }
        public double this[int index]
        {
            get
            {
                if (index < 0)
                    throw new Exception("Out of range in elements in Vector");
                else
                    return elements[index];
            }
            set
            {
                if (index < 0)
                    throw new Exception("Out of range in elements in Vector");
                else
                    elements[index] = value;
            }
        }
        public int Length
        {
            get
            {
                if (elements == null)
                    return 0;
                else
                    return elements.Length;
            }
        }
        public static double operator*(Vector ob1, Vector ob2)
        {
            
            if (ob1.Length != ob2.Length)
                throw new Exception("Different sizes in Vector * Vector");

            double result = 0;

            for (int i = 0; i < ob1.Length; i++)
                result += ob1[i] * ob2[i];

            return result;
        }
        public static Vector operator+(Vector ob1, Vector ob2)
        {
            if (ob1.Length != ob2.Length)
                throw new Exception("Different sizes in Vector + Vector");

            Vector result = new Vector(ob1.Length);           

            for (int i = 0; i < ob1.Length; i++)
                result[i] = ob1[i] + ob2[i];

            return result;
        }
        public static Vector operator-(Vector ob1, Vector ob2)
        {
            if (ob1.Length != ob2.Length)
                throw new Exception("Different sizes in Vector - Vector");

            Vector result = new Vector(ob1.Length);

            for (int i = 0; i < ob1.Length; i++)
                result[i] = ob1[i] - ob2[i];

            return result;
        }
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < Length; i++)
                    s += elements[i].ToString() + ' ';

            return s;
        }
    }
}
