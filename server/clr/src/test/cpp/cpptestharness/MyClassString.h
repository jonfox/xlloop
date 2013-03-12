#include "MyClassStringUnmanaged.h"

using namespace System;
using namespace msclr::interop;

ref class MyClassString {
private:
	marshal_context^ context;
	MyClassStringUnmanaged* myClassString;
public:
	MyClassString();
	~MyClassString();

	String^ Concat(String^ s1, String^ s2);
	String^ Concat(String^ s1, String^ s2, String^ s3);
	String^ StringIntBool(String^ s, int i, bool b);
};
