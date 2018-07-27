namespace UnityEngine.Rendering.PostProcessing
{
    public static class HaltonSeq
    {
        public static float Get(int index, int radix)
        {
            var result = 0f;
            var fraction = 1f / radix;

            while (index > 0)
            {
                result += index % radix * fraction;

                index /= radix;
                fraction /= radix;
            }

            return result;
        }
    }
}