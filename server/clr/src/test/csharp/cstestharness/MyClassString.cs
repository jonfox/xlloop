using System;

namespace cstestharness {
    class MyClassString {
        public string Concat(string s1, string s2) {
            return s1 + s2;
        }

        public string Concat(string s1, string s2, string s3) {
            return s1 + s2 + s3;
        }

        public string StringIntBool(string s, int i, bool b) {
            return String.Format("{0}:{1}:{2}", s, i, b);
        }
    }
}
