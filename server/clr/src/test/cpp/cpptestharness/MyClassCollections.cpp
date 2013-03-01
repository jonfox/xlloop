#include "stdafx.h"

#include "MyClassCollections.h"

double MyClassCollections::Sum(array<double>^ arr) {
	double sum = 0.0;

	for (int i = 0; i < arr->Length; i++)
		sum += arr[i];

	return sum;
}

double MyClassCollections::Sum(list<double> lst) {
	double sum = 0.0;

	for (list<double>::iterator iter = lst.begin(); iter != lst.end(); iter++)
		sum += *iter;

	return sum;
}

double MyClassCollections::SumJ(array<array<double>^>^ arr) {
	double sum = 0.0;

	for (int i = 0; i < arr->Length; i++) {
		for (int j = 0; j < arr[i]->Length; j++)
			sum += arr[i][j];
	}

	return sum;
}

array<double>^ MyClassCollections::TimesTwo(array<double>^ arr) {
	array<double>^ result = gcnew array<double>(arr->Length);
	for (int i = 0; i < arr->Length; i++)
		result[i] = arr[i] * 2.0;

	return result;
}
