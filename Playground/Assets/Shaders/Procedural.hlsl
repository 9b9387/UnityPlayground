#ifndef OWLET_PROCEDURAL
#define OWLET_PROCEDURAL
//
// Code from shader graph.
//

float4 Ellipse(float2 UV, float Width, float Height)
{
    float d = length((UV * 2 - 1) / float2(Width, Height));
    return saturate((1 - d) / fwidth(d));
}

float Rectangle(float2 UV, float Width, float Height)
{
    float2 d = abs(UV * 2 - 1) - float2(Width, Height);
    d = 1 - d / fwidth(d);
    return saturate(min(d.x, d.y));
}

float RoundedRectangle(float2 UV, float Width, float Height, float Radius)
{
    Radius = max(min(min(abs(Radius * 2), abs(Width)), abs(Height)), 1e-5);
    float2 uv = abs(UV * 2 - 1) - float2(Width, Height) + Radius;
    float d = length(max(0, uv)) / Radius;
    return saturate((1 - d) / fwidth(d));
}

float Polygon(float2 UV, float Sides, float Width, float Height)
{
    float pi = 3.14159265359;
    float aWidth = Width * cos(pi / Sides);
    float aHeight = Height * cos(pi / Sides);
    float2 uv = (UV * 2 - 1) / float2(aWidth, aHeight);
    uv.y *= -1;
    float pCoord = atan2(uv.x, uv.y);
    float r = 2 * pi / Sides;
    float distance = cos(floor(0.5 + pCoord / r) * r - pCoord) * length(uv);
    return saturate((1 - distance) / fwidth(distance));
}

float AdvancePolygon(float2 UV, float Sides, float InnerSize, float OuterSize)
{
    float2 uv = UV - float2(0.5, 0.5);

    float pi = 3.14159265359;
    float l = 2 * pi / Sides;

    float a = frac(atan2(uv.x, uv.y) / l);
    a = a + clamp((1 - a) * 2, 1, 2) - 1;
    a = a * l;
    float3 p = float3(cos(a), sin(a), 0) * length(uv);

    float3 p0 = float3(InnerSize * 0.5, 0, 0);
    float3 p1 = float3(cos(l), sin(l), 0) * OuterSize * 0.5;

    float z = cross(p1 - p0, p - p0).z;
    return saturate(z / fwidth(z));
}

float3 Checkerboard(float2 UV, float3 ColorA, float3 ColorB, float2 Frequency)
{
    UV = (UV.xy + 0.5) * Frequency;
    float4 derivatives = float4(ddx(UV), ddy(UV));
    float2 duv_length = sqrt(float2(dot(derivatives.xz, derivatives.xz), dot(derivatives.yw, derivatives.yw)));
    float width = 1.0;
    float2 distance3 = 4.0 * abs(frac(UV + 0.25) - 0.5) - width;
    float2 scale = 0.35 / duv_length.xy;
    float freqLimiter = sqrt(clamp(1.1f - max(duv_length.x, duv_length.y), 0.0, 1.0));
    float2 vector_alpha = clamp(distance3 * scale.xy, -1.0, 1.0);
    float alpha = saturate(0.5f + 0.5f * vector_alpha.x * vector_alpha.y * freqLimiter);
    return lerp(ColorA, ColorB, alpha.xxx);
}

#endif // OWLET_PROCEDURAL
