#include "stdafx.h"

#include "MyClassStringUnmanaged.h"

LPTSTR MyClassStringUnmanaged::Concat(LPCTSTR s1, LPCTSTR s2) {
		int len = _tcslen(s1) + _tcslen(s2);
		LPTSTR result = new TCHAR[len + 1];
		result[0] = 0;
		_tcscat(result, s1);
		_tcscat(result, s2);

		return result;
}

LPTSTR MyClassStringUnmanaged::Concat(LPCTSTR s1, LPCTSTR s2, LPCTSTR s3) {
		int len = _tcslen(s1) + _tcslen(s2) + _tcslen(s3);
		LPTSTR result = new TCHAR[len + 1];
		result[0] = 0;
		_tcscat(result, s1);
		_tcscat(result, s2);
		_tcscat(result, s3);

		return result;
}

LPTSTR MyClassStringUnmanaged::StringIntBool(LPCTSTR s, int i, bool b) {
	LPTSTR buffer = new TCHAR[512]; // Unchecked
	LPTSTR boolString = nullptr;
	if (b)
		boolString = _T("true");
	else
		boolString = _T("false");
	_stprintf(buffer, L"%s:%i:%s", s, i, boolString);
	return buffer;
}
