#pragma once

class MyClassStringUnmanaged {
public:
	LPTSTR Concat(LPCTSTR s1, LPCTSTR s2);
	LPTSTR Concat(LPCTSTR s1, LPCTSTR s2, LPCTSTR s3);
	LPTSTR StringIntBool(LPCTSTR s, int i, bool b);
};