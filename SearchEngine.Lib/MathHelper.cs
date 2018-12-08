using System;

namespace SearchEngine.Lib
{
    public static class MathHelper
    {
        public static double NormalizeWhenSmallValuesAreBetter(
            double value, double minValue, double fallbackDivisor = 0.00001)
        {
            return minValue / Math.Max(value, fallbackDivisor);
        }

        public static double NormalizeWhenLargeValuesAreBetter(double value, double maxValue)
        {
            return value / maxValue;
        }

        public static double Normalize(double value, double minValue, double maxValue, bool smallValuesAreBetter)
        {
            return smallValuesAreBetter
                ? NormalizeWhenSmallValuesAreBetter(value, minValue)
                : NormalizeWhenLargeValuesAreBetter(value, maxValue);
        }
    }
}
