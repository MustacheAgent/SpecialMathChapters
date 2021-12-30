using OxyPlot;
using System.Collections.Generic;

namespace GoldCodes.Converters
{
    public static class Points
    {
        public static void ToPoints(this double[] array, List<DataPoint> list)
        {
            list.Clear();
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(new DataPoint(i, array[i]));
            }
        }
        public static void ToPoints(this int[] array, List<DataPoint> list)
        {
            list.Clear();
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(new DataPoint(i, array[i]));
            }
        }
    }
}
