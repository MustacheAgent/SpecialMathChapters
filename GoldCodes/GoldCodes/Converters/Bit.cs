using System;
using System.Collections.Generic;
using System.Linq;

namespace GoldCodes.Converters
{
    public static class Bit
    {
        /// <summary>
        ///                      Перевод массива битов в число
        /// </summary>
        /// <param name="_bits"> Массив бит в представлении int[N] </param>
        /// <param name="_size"> По умолчанию входной массив полностью копируется 
        ///                      При произвольном _size входной массив будет расширен или сокращен до int[_size] </param>
        /// <returns>            Число в диапазоне от 0 до 2^N-1 в случае _size = 1
        ///                      Число в диапазоне от 0 до 2^_size-1 в случае задания произвольного _size </returns>
        public static int ToInt(int[] _bits, int _size = -1)
        {
            int[] bits;
            if (_size != -1)
            {
                bits = new int[_size];
                for (int i = 0; i < _size; i++)
                {
                    try
                    {
                        bits[i] = _bits[i];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        bits[i] = 0;
                    }
                }
            }
            else
            {
                bits = _bits;
            }
            int value = 0;
            for (int i = 0; i < bits.Length; i++)
            {
                value += bits[i] * (int)Math.Pow(2, bits.Length - 1 - i);
            }
            return value;
        }

        public static int[] FromString(string _bits)
        {
            List<int> bits = new List<int>();
            if (_bits.Length > 2)
            {
                if (_bits[0] + "" + _bits[1] == "0x")
                {
                    for (int i = 2; i < _bits.Length; i++)
                    {
                        if (_bits[i] > 47 && _bits[i] < 58)
                        {
                            bits.AddRange(Int.ToBits(_bits[i] - 48, 4));
                        }
                        if (_bits[i] > 96 && _bits[i] < 103)
                        {
                            bits.AddRange(Int.ToBits(_bits[i] - 97 + 10, 4));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < _bits.Length; i++)
                {
                    bits[i] = _bits[i] == '0' ? 0 : 1;
                }
            }
            return bits.ToArray<int>();
        }

        public static string ToString(int[] _bits, int _base = 2)
        {
            string bits = "";
            if (_base == 16)
            {
                bits = "0x";
                int[] buf = new int[4];
                for (int i = 0; i < _bits.Length; i += 4)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        buf[j] = _bits[i + j];
                    }
                    int res = Bit.ToInt(buf);
                    switch (res)
                    {
                        case 10:
                            bits += 'a';
                            break;
                        case 11:
                            bits += 'b';
                            break;
                        case 12:
                            bits += 'c';
                            break;
                        case 13:
                            bits += 'd';
                            break;
                        case 14:
                            bits += 'e';
                            break;
                        case 15:
                            bits += 'f';
                            break;
                        default:
                            bits += res.ToString();
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _bits.Length; i++)
                    bits += _bits[i] == 0 ? '0' : '1';
            }
            return bits;
        }
    }
}
