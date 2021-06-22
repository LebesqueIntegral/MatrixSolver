// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "pch.h"
#include <string>
#include <fstream>
#include <iostream>
#include <chrono>

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}



class Vector
{
public:
	double* elements;
	int Length;

	Vector(int);
	Vector(double*, int);

	Vector(const Vector&);

	Vector& operator=(const Vector&);

	double& operator[](int);

	double operator*(const Vector& ob2);

	Vector operator+(const Vector& ob1);

	Vector operator-(const Vector& ob1);

	~Vector();
};

class ElemMat
{

public:
	Vector elements;
	int order;

	ElemMat(int);

	ElemMat(double*, int);

	ElemMat(const ElemMat&);

	ElemMat& operator=(const ElemMat&);

	ElemMat& operator=(const Vector&);

	double& operator[](int);

	ElemMat operator+(const ElemMat&);

	ElemMat operator-(const ElemMat&);

	ElemMat operator*(const ElemMat&);

	Vector operator*(const Vector&);

	ElemMat reverse();
};

class MainMat
{
public:
	int count;
	int order;
	ElemMat* diagonal;
	ElemMat* under;
	ElemMat* above;
	Vector* X;
	Vector* F;
	Vector* Test;

	MainMat(int, int);

	MainMat(int, int, ElemMat*, ElemMat*, ElemMat*);

	Vector* operator*(Vector*);

	Vector* Solve(Vector* );

	void ToFile(std::string);

	~MainMat();
};

Vector::Vector(int order = 1)
{
	Length = order;
	elements = new double[order];
	for (int i = 0; i < order; i++)
		elements[i] = (i + 1) * (i + 1);
}

Vector::Vector(double* vec, int len)
{
	Length = len;
	elements = new double[len];
	for (int i = 0; i < len; i++)
		elements[i] = vec[i];
}

double& Vector::operator[](int index)
{
	return elements[index];
}

Vector::Vector(const Vector& ob1)
{
	Length = ob1.Length;
	elements = new double[Length];
	for (int i = 0; i < Length; i++)
		elements[i] = ob1.elements[i];
}

Vector& Vector::operator=(const Vector& ob1)
{
	if (elements)
		delete[] elements;
	elements = new double[ob1.Length];
	Length = ob1.Length;
	for (int i = 0; i < Length; i++)
		elements[i] = ob1.elements[i];
	return *this;
}

double Vector::operator*(const Vector& ob1)
{

	if (Length != ob1.Length)
		throw "Different sizes of vectors";

	double result = 0;

	for (int i = 0; i < ob1.Length; i++)
		result += elements[i] * ob1.elements[i];

	return result;
}

Vector Vector::operator+(const Vector& ob1)
{
	if (Length != ob1.Length)
		throw "Different sizes of vectors";

	Vector result(ob1.Length);

	for (int i = 0; i < ob1.Length; i++)
		result[i] = elements[i] + ob1.elements[i];

	return result;
}

Vector Vector::operator-(const Vector& ob1)
{
	if (Length != ob1.Length)
		throw "Different sizes of vectors";

	Vector result(ob1.Length);

	for (int i = 0; i < ob1.Length; i++)
		result[i] = elements[i] - ob1.elements[i];

	return result;
}

Vector::~Vector()
{
	delete[] elements;
}




ElemMat::ElemMat(int M = 1) : elements(M)
{
	order = M;
}

ElemMat::ElemMat(double* elem, int M) : elements(M)
{
	order = M;
	for (int i = 0; i < order; i++)
		elements[i] = elem[i];
}

double& ElemMat::operator[](int index)
{
	return elements.elements[index];
}

ElemMat::ElemMat(const ElemMat& ob1) : elements(ob1.elements)
{
	order = ob1.order;
}

ElemMat& ElemMat::operator=(const ElemMat& ob1)
{
	order = ob1.order;
	elements = ob1.elements;
	return *this;
}

ElemMat& ElemMat::operator=(const Vector& ob1)
{
	elements = ob1;
	order = ob1.Length;
	return *this;
}

ElemMat ElemMat::operator+(const ElemMat& ob1)
{
	ElemMat res(ob1.order);
	for (int i = 0; i < ob1.order; i++)
		res[i] = elements[i] + ob1.elements.elements[i];
	return res;
}

ElemMat ElemMat::operator-(const ElemMat& ob1)
{
	ElemMat res(ob1.order);
	for (int i = 0; i < ob1.order; i++)
		res[i] = elements[i] - ob1.elements.elements[i];
	return res;
}

ElemMat ElemMat::operator*(const ElemMat& ob1)
{
	ElemMat res(order);
	for (int i = 0; i < order; i++)
		res[i] = elements[i] * ob1.elements.elements[i];
	return res;
}

Vector ElemMat::operator*(const Vector& vec)
{
	Vector res(order);
	for (int i = 0; i < order; i++)
		res[i] = elements[i] * vec.elements[i];
	return res;
}

ElemMat ElemMat::reverse()
{
	ElemMat res(order);
	for (int i = 0; i < order; i++)
		res[i] = 1 / elements.elements[i];
	return res;
}



MainMat::MainMat(int blocks_count, int blocks_order)
{
	order = blocks_order;
	count = blocks_count;
	diagonal = new ElemMat[count];
	under = new ElemMat[count - 1];
	above = new ElemMat[count - 1];

	Test = new Vector[count];
	int i;

	Vector vec(order);
	for (i = 0; i < count - 1; i++)
	{
		diagonal[i] = vec;
		under[i] = vec;
		above[i] = vec;
		Test[i] = vec;
	}

	diagonal[i] = vec;
	Test[i] = vec;

	for (i = 0; i < blocks_count - 1; i++)
		for (int j = 0; j < blocks_order; j++)
		{
			diagonal[i][j] = i + j + 1;
			above[i][j] = i + 2 + j + 5;
			under[i][j] = i + 3 + j + 1;
		}

	diagonal[blocks_count - 1][blocks_order - 1] = 1;
}

MainMat::MainMat(int blocks_count, int blocks_order, ElemMat* diag, ElemMat* ab, ElemMat* und)
{
	order = blocks_order;
	count = blocks_count;
	diagonal = new ElemMat[count];
	under = new ElemMat[count - 1];
	above = new ElemMat[count - 1];

	X = new Vector[count];
	F = new Vector[count];
	Test = new Vector[count];
	int i = 0;

	Vector vec(order);
	for (; i < count - 1; i++)
	{
		diagonal[i] = vec;
		under[i] = vec;
		above[i] = vec;

		X[i] = vec;
		F[i] = vec;
		Test[i] = vec;
	}

	diagonal[i] = vec;
	X[i] = vec;
	F[i] = vec;
	Test[i] = vec;

	for (i = 0; i < blocks_count - 1; i++)
		for (int j = 0; j < blocks_order; j++)
		{
			diagonal[i][j] = diag[i][j];
			above[i][j] = ab[i][j];
			under[i][j] = und[i][j];
		}

	diagonal[blocks_count - 1] = diag[blocks_count - 1];
}

Vector* MainMat::operator*(Vector* vec)
{
	Vector* result = new Vector[count];
	for (int i = 0; i < count; i++)
		result[i] = vec[i];

	for (int i = 0; i < order; i++)
		result[0][i] = diagonal[0][i] * vec[0][i] + above[0][i] * vec[1][i];

	for (int i = order; i < count*order - order; i++)
		result[i / order][i % order] = under[i / order - 1][i % order] * vec[i / order - 1][i % order] + diagonal[i / order][i % order] * vec[i / order][i % order] + above[i / order][i % order] * vec[i / order + 1][i % order];

	for (int i = 0; i < order; i++)
		result[count - 1][i] = under[count - 2][i] * vec[count - 2][i] + diagonal[count - 1][i] * vec[count - 1][i];

	return result;
}

Vector* MainMat::Solve(Vector* vec)
{
	int M = order;
	int N = count;

	F = vec;
	
	ElemMat* alpha = new ElemMat[N - 1];

	Vector* beta = new Vector[N];

	X = new Vector[N];

	

	alpha[0] = diagonal[0].reverse() * above[0];
	for (int i = 1; i < N - 1; i++)
		alpha[i] = (diagonal[i] - (under[i - 1] * alpha[i - 1])).reverse() * above[i];

	beta[0] = diagonal[0].reverse() * F[0];

	for (int i = 1; i < N; i++)
	{
		beta[i] = (diagonal[i] - under[i - 1] * alpha[i - 1]).reverse() * (vec[i] - under[i - 1] * beta[i - 1]);
	}

	X[N - 1] = beta[N - 1];
	for (int i = N - 2; i >= 0; i--)
	{
		X[i] = beta[i] - (alpha[i] * X[i + 1]);
	}
	

	Test = (*this) * X;
	return X;
}

void MainMat::ToFile(std::string path)
{
	std::ofstream s(path);

	s << "MATRIX\n";
	for (int i = 0; i < order; i++)
	{
		for (int j = 0; j < i; j++)
			s << "0 ";
		s << diagonal[0].elements[i] << ' ';
		for (int j = 0; j < order - 1; j++)
			s << "0 ";
		s << above[0].elements[i];
		for (int j = order + i + 1; j < order * count; j++)
			s << " 0";
		s << '\n';
	}
	for (int i = order; i < order * count - order; i++)
	{
		for (int j = 0; j < i - order; j++)
			s << "0 ";
		s << under[i / order - 1].elements[i % order] << ' ';
		for (int j = 0; j < order - 1; j++)
			s << "0 ";
		s << diagonal[i / order].elements[i % order] << ' ';
		for (int j = 0; j < order - 1; j++)
			s << "0 ";
		s << above[i / order].elements[i % order];
		for (int j = order + i + 1; j < order * count; j++)
			s << " 0";
		s << '\n';
	}

	for (int i = 0; i < order; i++)
	{
		for (int j = 0; j < order * (count - 2) + i; j++)
			s << "0 ";
		s << under[count - 2].elements[i] << ' ';
		for (int j = 0; j < order - 1; j++)
			s << "0 ";
		s << diagonal[count - 1].elements[i];
		for (int j = i; j < order - 1; j++)
			s << " 0";
		s << '\n';
	}

	s << "\nVECTOR F\n";
	for (int i = 0; i < this->count; i++)
	{
		for (int j = 0; j < this->order; j++)
			s << F[i][j]<<' ';
		
	}
	s << '\n';
	s << "\nSOLUTION VECTOR\n";
	for (int i = 0; i < this->count; i++)
	{
		for (int j = 0; j < this->order; j++)
			s << X[i][j]<<' ';
	}
	s << '\n';
	s << "\n\nMY TEST ANSWER\n";
	for (int i = 0; i < this->count; i++)
	{
		for (int j = 0; j < this->order; j++)
			s << Test[i][j];
		s << '\n';
	}
}

MainMat::~MainMat()
{
	delete[] diagonal;
	delete[] under;
	delete[] above;
	delete[] X;
	delete[] F;
	delete[] Test;
}



extern "C" _declspec(dllexport) void __cdecl extern_func(int count, int order, double* matrix, double* vec, double* sol, double* time)
{
	auto F = new Vector[count];
	for (int i = 0; i < count; i++)
		F[i] = Vector(order);
	if (vec)
	{
		for (int i = 0; i < count * order; i++)
			F[i / order][i % order] = vec[i];

	}

	if (matrix == NULL)
	{
		MainMat mainMat(count, order);

		auto before = std::chrono::system_clock::now();
		mainMat.Solve(F);
		auto after = std::chrono::system_clock::now();
		//mainMat.ToFile("CppTest.txt");

		for (int i = 0; i < count; i++)
			for (int j = 0; j < order; j++)
				sol[i * order + j] = mainMat.X[i][j];

		std::chrono::duration<double> my_time = after - before;
		*time = my_time.count();
	}
	else
	{
		double** diag = new double* [count];
		double** above = new double* [count - 1];
		double** under = new double* [count - 1];

		for (int i = 0; i < count; i++)
			diag[i] = new double[order];

		for (int i = 0; i < count - 1; i++)
		{
			under[i] = new double[order];
			above[i] = new double[order];
		}
		int i;
		for (i = 0; i < order * (count - 1); i++)
			above[i/order][i % order] = matrix[i];
		for (; i < order * (count - 1) + order * count; i++)
			diag[i/order - (count - 1)][i % order] = matrix[i];
		for (; i < (2 * order * (count - 1) + order * count); i++)
			under[i/order - (2 * count - 1)][i % order] = matrix[i];

		ElemMat* d = new ElemMat[count];
		ElemMat* a = new ElemMat[count - 1];
		ElemMat* u = new ElemMat[count - 1];
		for (i = 0; i < count; i++)
		{
			d[i] = Vector(order);
			for (int j = 0; j < order; j++)
				d[i][j] = diag[i][j];
		}

		for (i = 0; i < count - 1; i++)
		{
			a[i] = Vector(order);
			u[i] = Vector(order);
			for (int j = 0; j < order; j++)
			{
				a[i][j] = above[i][j];
				u[i][j] = under[i][j];
			}
		}

		MainMat mainMat(count, order, d, a, u);

		auto before = std::chrono::system_clock::now();
		mainMat.Solve(F);
		auto after = std::chrono::system_clock::now();
		
		mainMat.ToFile("CppTest.txt");

		for (i = 0; i < count; i++)
			delete[] diag[i];

		for (i = 0; i < count - 1; i++)
		{
			delete[] under[i];
			delete[] above[i];
		}
		delete[] diag;
		delete[] above;
		delete[] under;
		delete[] d;
		delete[] a;
		delete[] u;
		for (i = 0; i < count; i++)
			for (int j = 0; j < order; j++)
				sol[i * order + j] = mainMat.X[i][j];
		
		std::chrono::duration<double> my_time = after - before;
		*time = my_time.count();
	}

}