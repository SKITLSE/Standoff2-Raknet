using System;
using UnityEngine;

namespace RiseRakNet.Misc
{
    public class TypeCompressor
    {
        public static sbyte FloatToByte_m100_100(float value)
        {
            return (sbyte)(Mathf.RoundToInt(value) % 100);
        }

        public static float ByteToFloat_n100_100(sbyte value)
        {
            return value;
        }

        public static short FloatToShort_n100_100(float value)
        {
            value = Mathf.Clamp(value, -100f, 100f);
            return (short)(value * 100f);
        }

        public static float ShortToFloat_n100_100(short value)
        {
            return value / 100f;
        }

        public static sbyte FloatToByte_n1_1(float value)
        {
            return (sbyte)(Mathf.RoundToInt(value * 100f) % 110);
        }

        public static float ByteToFloat_n1_1(sbyte value)
        {
            return value / 100f;
        }

        public static sbyte FloatToByte_n5_5(float value)
        {
            var num = (int)(value * 100f) % 510 / 5;
            return (sbyte)num;
        }

        public static float ByteToFloat_n5_5(sbyte value)
        {
            return value * 5 / 100f;
        }

        public static short FloatToShort_angle(float value)
        {
            var num = (float)Math.Truncate(value);
            var num2 = value - num;
            num = (int)num % 360;
            num += num2;
            num = ((num > 180f) ? (num - 360f) : ((num < -180f) ? (num + 360f) : num));
            return (short)(num * 100f);
        }

        public static float ShortToFloat_angle(short value)
        {
            return value / 100f;
        }

        public static sbyte FloatToByte_angle(float value)
        {
            var num = (float)Math.Truncate(value);
            var num2 = value - num;
            num = (int)num % 360;
            num += num2;
            num = ((num > 180f) ? (num - 360f) : ((num < -180f) ? (num + 360f) : num));
            return FloatToByte_n1_1(num / 180f);
        }

        public static float ByteToFloat_angle(sbyte value)
        {
            return ByteToFloat_n1_1(value) * 180f;
        }

        public static void Test()
        {
            Debug.Log("Float to byte -100 100");
            Debug.Log(ByteToFloat_n100_100(FloatToByte_m100_100(32.4f)));
            Debug.Log(ByteToFloat_n100_100(FloatToByte_m100_100(-36.4f)));
            Debug.Log(ByteToFloat_n100_100(FloatToByte_m100_100(0.4f)));
            Debug.Log(ByteToFloat_n100_100(FloatToByte_m100_100(93.4f)));
            Debug.Log("Float to byte -1 1");
            Debug.Log(ByteToFloat_n1_1(FloatToByte_n1_1(0.231f)));
            Debug.Log(ByteToFloat_n1_1(FloatToByte_n1_1(-0.431f)));
            Debug.Log(ByteToFloat_n1_1(FloatToByte_n1_1(-0.013f)));
            Debug.Log(ByteToFloat_n1_1(FloatToByte_n1_1(-0.2f)));
            Debug.Log(ByteToFloat_n1_1(FloatToByte_n1_1(3.162f)));
            Debug.Log("Float to byte -5 5");
            Debug.Log(ByteToFloat_n5_5(FloatToByte_n5_5(4.434f)));
            Debug.Log(ByteToFloat_n5_5(FloatToByte_n5_5(-1.0643f)));
            Debug.Log(ByteToFloat_n5_5(FloatToByte_n5_5(-0.4243f)));
            Debug.Log(ByteToFloat_n5_5(FloatToByte_n5_5(0.9243f)));
            Debug.Log(ByteToFloat_n5_5(FloatToByte_n5_5(0.0143f)));
            Debug.Log(ByteToFloat_n5_5(FloatToByte_n5_5(6.0553f)));
            Debug.Log("Float to short angle");
            Debug.Log(ShortToFloat_angle(FloatToShort_angle(17.343f)));
            Debug.Log(ShortToFloat_angle(FloatToShort_angle(-1.023f)));
            Debug.Log(ShortToFloat_angle(FloatToShort_angle(-132.023f)));
            Debug.Log(ShortToFloat_angle(FloatToShort_angle(-182.023f)));
            Debug.Log(ShortToFloat_angle(FloatToShort_angle(-722.023f)));
            Debug.Log(ShortToFloat_angle(FloatToShort_angle(740.243f)));
            Debug.Log("Float to Byte angle");
            Debug.Log(ByteToFloat_angle(FloatToByte_angle(17.343f)));
            Debug.Log(ByteToFloat_angle(FloatToByte_angle(-14.343f)));
            Debug.Log(ByteToFloat_angle(FloatToByte_angle(170.023f)));
            Debug.Log(ByteToFloat_angle(FloatToByte_angle(183.343f)));
            Debug.Log(ByteToFloat_angle(FloatToByte_angle(-366.343f)));
        }
    }
}