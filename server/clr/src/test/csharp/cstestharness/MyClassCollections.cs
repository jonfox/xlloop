using System.Linq;

namespace cstestharness {
    class MyClassCollections {
        public double Sum(double[] arr) {
            return arr.Sum();
        }

        public double SumJ(double[][] arr) {
            return arr.Select(a => a.Sum()).Sum();
        }
    }
}
