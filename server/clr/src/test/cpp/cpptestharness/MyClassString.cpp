#include "stdafx.h"

#include "MyClassString.h"

MyClassString::MyClassString() {
	context = gcnew marshal_context();
	myClassString = new MyClassStringUnmanaged();
}

MyClassString::~MyClassString() {
	delete context;
	delete myClassString;
}

String^ MyClassString::Concat(String^ s1, String^ s2) {
	LPCTSTR us1 = context->marshal_as<LPCTSTR>(s1);
	LPCTSTR us2 = context->marshal_as<LPCTSTR>(s2);
	LPTSTR uresult = myClassString->Concat(us1, us2);
	String^ result = gcnew String(uresult);
	delete uresult;
	return result;
}

String^ MyClassString::Concat(String^ s1, String^ s2, String^ s3) {
	LPCTSTR us1 = context->marshal_as<LPCTSTR>(s1);
	LPCTSTR us2 = context->marshal_as<LPCTSTR>(s2);
	LPCTSTR us3 = context->marshal_as<LPCTSTR>(s3);
	LPTSTR uresult = myClassString->Concat(us1, us2, us3);
	String^ result = gcnew String(uresult);
	delete uresult;
	return result;
}

String^ MyClassString::StringIntBool(String^ s, int i, bool b) {
	LPCTSTR us = context->marshal_as<LPCTSTR>(s);
	LPTSTR uresult = myClassString->StringIntBool(us, i, b);
	String^ result = gcnew String(uresult);
	delete uresult;
	return result;
}
