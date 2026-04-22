namespace ML {
    public static class AI_Brain_Fast {
        public static double[] Score(double[] input) {
            double[] var0;
            if (input[1] <= 1.0563380420207977) {
                if (input[2] <= 1.6063122749328613) {
                    if (input[0] <= 1.5958132147789001) {
                        if (input[0] <= 0.9533011317253113) {
                            var0 = new double[3] {0.0, 0.2, 0.8};
                        } else {
                            var0 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        var0 = new double[3] {1.0, 0.0, 0.0};
                    }
                } else {
                    if (input[2] <= 6.044019937515259) {
                        if (input[2] <= 1.7732558250427246) {
                            var0 = new double[3] {0.8666666666666667, 0.0, 0.13333333333333333};
                        } else {
                            var0 = new double[3] {0.38461538461538464, 0.038461538461538464, 0.5769230769230769};
                        }
                    } else {
                        if (input[0] <= 109.9975814819336) {
                            var0 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var0 = new double[3] {0.0, 1.0, 0.0};
                        }
                    }
                }
            } else {
                var0 = new double[3] {0.0, 1.0, 0.0};
            }
            return var0;
        }
    }
}
