#ifndef COLORSPACE_CGINC
#define COLORSPACE_CGINC

#define Epsilon 0.0001

inline float3 RGBtoHCV(float3 rgb)
{
	float4 p = (rgb.g < rgb.b) ? float4(rgb.bg, -1.0, 2.0 / 3.0) : float4(rgb.gb, 0.0, -1.0 / 3.0);
	float4 q = (rgb.r < p.x) ? float4(p.xyw, rgb.r) : float4(rgb.r, p.yzx);
	float c = q.x - min(q.w, q.y);
	float h = abs((q.w - q.y) / (6 * c + Epsilon) + q.z);
	return float3(h, c, q.x);
}

inline float3 HUEtoRGB(float h)
{
	float r = abs(h * 6 - 3) - 1;
	float g = 2 - abs(h * 6 - 2);
	float b = 2 - abs(h * 6 - 4);
	return saturate(float3(r, g, b));
}

inline float3 RGBtoHSL(float3 rgb)
{
	float3 hcv = RGBtoHCV(rgb);
	float l = hcv.z - hcv.y * 0.5;
	float s = hcv.y / (1 - abs(l * 2 - 1) + Epsilon);
	return float3(hcv.x, s, l);
}

inline float3 HSLtoRGB(float3 hsl)
{
	float3 rgb = HUEtoRGB(hsl.x);
	float c = (1 - abs(2 * hsl.z - 1)) * hsl.y;
	return (rgb - 0.5) * c + hsl.z;
}

inline float3 RGBtoHSV(in float3 rgb)
{
	float3 hcv = RGBtoHCV(rgb);
	float s = hcv.y / (hcv.z + Epsilon);
	return float3(hcv.x, s, hcv.z);
}

inline float3 HSVtoRGB(in float3 hsv)
{
	float3 rgb = HUEtoRGB(hsv.x);
	return ((rgb - 1) * hsv.y + 1) * hsv.z;
}

#endif
