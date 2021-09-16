#ifndef OWLET_SIMPLE_NOISE
#define OWLET_SIMPLE_NOISE
//
// Generate a simple noise
// Code from shader graph.
//
inline float SimpleNoise_RandomValue_float (float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
}

inline float SimpleNnoise_Interpolate_float (float a, float b, float t)
{
    return (1.0-t)*a + (t*b);
}

inline float SimpleNoise_ValueNoise_float (float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = SimpleNoise_RandomValue_float(c0);
    float r1 = SimpleNoise_RandomValue_float(c1);
    float r2 = SimpleNoise_RandomValue_float(c2);
    float r3 = SimpleNoise_RandomValue_float(c3);

    float bottomOfGrid = SimpleNnoise_Interpolate_float(r0, r1, f.x);
    float topOfGrid = SimpleNnoise_Interpolate_float(r2, r3, f.x);
    float t = SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
    return t;
}

float SimpleNoise(float2 UV, float Scale)
{
    float t = 0.0;

    float freq = pow(2.0, float(0));
    float amp = pow(0.5, float(3-0));
    t += SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3-1));
    t += SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3-2));
    t += SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    return t;
}

#endif // OWLET_SIMPLE_NOISE
