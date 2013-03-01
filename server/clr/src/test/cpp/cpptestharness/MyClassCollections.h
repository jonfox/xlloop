#include "stdafx.h"

using namespace cliext;

ref class MyClassCollections {
public:
	double Sum(array<double>^ arr);
	double Sum(list<double> lst);
	double SumJ(array<array<double>^>^ arr);

	array<double>^ TimesTwo(array<double>^ arr);
};