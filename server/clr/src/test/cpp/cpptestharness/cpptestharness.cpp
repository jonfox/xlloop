// cpptestharness.cpp : main project file.

#include "stdafx.h"

#include "MyClassInt.h"
#include "MyClassString.h"
#include "MyClassDouble.h"
#include "MyClassCollections.h"

using namespace System;

using namespace Trafigura::XLLoop;

int main(array<System::String ^> ^args) {

	MyClassInt^ myClassInt = gcnew MyClassInt();
	MyClassString^ myClassString = gcnew MyClassString();
	MyClassDouble^ myClassDouble = gcnew MyClassDouble();
	MyClassCollections^ myClassCollection = gcnew MyClassCollections();

	ReflectFunctionHandler^ rfh = gcnew ReflectFunctionHandler("MyClassInt.", myClassInt);
	ReflectFunctionHandler^ rfh1 = rfh->AddInstanceMethods("MyClassString.", myClassString);
	ReflectFunctionHandler^ rfh2 = rfh1->AddInstanceMethods("MyClassDouble.", myClassDouble);
	ReflectFunctionHandler^ rfh3 = rfh2->AddInstanceMethods("MyClassCollections.", myClassCollection);
	FunctionServer^ fs = gcnew FunctionServer(5455, rfh3);
	fs->Start();
	Console::WriteLine("Started server...\nPress enter to terminate");
	Console::Read();
	fs->Stop();
	return 0;
}
