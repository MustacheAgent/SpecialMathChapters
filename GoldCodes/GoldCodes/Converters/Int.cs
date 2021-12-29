using System;

namespace GoldCodes.Converters
{
    public static class Int
    {
        /// <summary>
        ///                       Перевод числа в массив бит 
        /// </summary>
        /// <param name="_value"> Число от 0 до 2^N-1 </param>
        /// <param name="_value"> Задает определенный размер массива 
        ///                       Если задать его избыточным, лишние позиции будут заполнены нулями 
        ///                       Если задать его недостаточным, старшие степени будут утеряны </param>
        /// <returns>             Массив бит в представлении int[N]</returns>
        public static int[] ToBits(int _value, int _size = -1)
        {
            int value = _value;
            int[] bits;
            if (_size != -1)
            {
                bits = new int[_size];
            }
            else
            {
                bits = new int[(int)Math.Log(value, 2) + 1];
            }
            for (int i = (int)Math.Log(value, 2); i > -1; i--)
            {
                var minus = (int)Math.Pow(2, i);
                try
                {
                    if (value >= minus)
                    {
                        value -= minus;
                        bits[bits.Length - 1 - i] = 1;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    if (value >= minus)
                    {
                        value -= minus;
                    }
                }
            }
            return bits;
        }
    }
}
