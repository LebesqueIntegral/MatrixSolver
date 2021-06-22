using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLibrary
{
    public class ElemMat
    {

        public Vector elements;
        public int order
        {
            get
            {
                if (elements == null)
                    return 0;
                else
                    return elements.Length;
            }
        }

        public ElemMat(int M)
        {
            if (M < 0)
                throw new Exception("Wrong order for Vector");
            if (M == 0)
                elements = null;
            else
                elements = new Vector(M);
        }

        public ElemMat(double[] elem)
        {
            if (elem == null)
                elements = null;
            else
            {
                elements = new Vector(elem.Length);
                for (int i = 0; i < elem.Length; i++)
                    elements[i] = elem[i];
            }
        }
        public double this[int index]
        {
            get
            {
                if (index < 0)
                    throw new Exception("Out of range in elements in ElemMat");
                else
                    return elements[index];
            }
            set
            {
                if (index < 0)
                    throw new Exception("Out of range in elements in ElemMat");
                else
                    elements[index] = value;
            }
        }

        public static ElemMat operator+(ElemMat ob1, ElemMat ob2)
        {
            if (ob1.order != ob2.order)
                throw new Exception("Different sizes in ElemMat + ElemMat");
            ElemMat res = new ElemMat(ob1.order);
            for (int i = 0; i < ob1.order; i++)
                res[i] = ob1[i] + ob2[i];
            return res;
        }

        public static ElemMat operator-(ElemMat ob1, ElemMat ob2)
        {
            if (ob1.order != ob2.order)
                throw new Exception("Different sizes in ElemMat - ElemMat");
            ElemMat res = new ElemMat(ob1.order);
            for (int i = 0; i < ob1.order; i++)
                res[i] = ob1[i] - ob2[i];
            return res;
        }

        public static ElemMat operator*(ElemMat ob1, ElemMat ob2)
        {
            if (ob1.order != ob2.order)
                throw new Exception("Different sizes in ElemMat * ElemMat");
            ElemMat res = new ElemMat(ob1.order);
            for (int i = 0; i < ob1.order; i++)
                res[i] = ob1[i] * ob2[i];
            return res;
        }

        public static Vector operator*(ElemMat ob1, Vector vec)
        {
            if (ob1.order != vec.Length)
                throw new Exception("Different sizes in ElemMat * Vector");
            Vector res = new Vector(ob1.order);

            for (int i = 0; i < ob1.order; i++)
                res[i] = ob1[i] * vec[i];

            return res;
        }

        public static ElemMat reverse(ElemMat ob1)
        {
            ElemMat res = new ElemMat(ob1.order);
            for (int i = 0; i < ob1.order; i++)
            {
                if (ob1[i] == 0)
                    throw new Exception("Couldn't solve SLAY with elements == 0");
                res[i] = 1 / ob1[i];
            }
            return res;
        }


    }
}
