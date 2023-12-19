//------------------------------------------------------------------------------------------------------
// Color conversions. Taken from: https://www.chilliant.com/rgb2hsv.html
//------------------------------------------------------------------------------------------------------


float Epsilon = 1e-10;

/*
 * Converts RGB to HCV.
 */
float3 RGBtoHCV(float3 RGB)
{
    float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
    float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
    float C = Q.x - min(Q.w, Q.y);
    float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
    return float3(H, C, Q.x);
}

/*
 * Converts RGB to HSV.
 */
float3 RGBtoHSV(float3 RGB)
{
    float3 HCV = RGBtoHCV(RGB);
    float S = HCV.y / (HCV.z + Epsilon);
    return float3(HCV.x, S, HCV.z);
}

/*
* Converts HUE to RGB.
*/
float3 HUEtoRGB(float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R, G, B));
}

/*
* Converts HSV to RGB.
*/
float3 HSVtoRGB(float3 HSV)
{
    float3 RGB = HUEtoRGB(HSV.x);
    return ((RGB - 1) * HSV.y + 1) * HSV.z;
}


//------------------------------------------------------------------------------------------------------
// Utility functions
//------------------------------------------------------------------------------------------------------

/*
* Lerps between two colors in HSV space. Leads to better results than lerping in RGB space, see https://www.alanzucconi.com/2016/01/06/colour-interpolation/  (code is also from there)
* Input: Two colors in RGB space, float where to lerp
*/

void HSVLerp_float(float4 Col1, float4 Col2, float T, out float4 ColRes)
{
    // Converting colors into HSV space
    Col1.xyz = RGBtoHSV(Col1.xyz);
    Col2.xyz = RGBtoHSV(Col2.xyz);

    float t = T; // used to lerp alpha

    float hue;
    float d = Col2.x - Col1.x; // hue difference

    if (Col1.x > Col2.x)
    {
        // Swapping hues of colors
        float temp = Col2.x;
        Col2.x = Col1.x;
        Col1.x = temp;

        d = -d;
        T = 1 - T;
    }

    if (d > 0.5)
    {
        Col1.x = Col1.x + 1;
        hue = (Col1.x + T * (Col2.x - Col1.x)) % 1;
    }

    if (d <= 0.5) hue = Col1.x + T * d;

    float sat = Col1.y + T * (Col2.y - Col1.y);
    float val = Col1.z + T * (Col2.z - Col1.z);
    float alpha = Col1.w + t * (Col2.w - Col1.w);

    // Converting result back into RGB space
    float3 rgb = HSVtoRGB(float3(hue, sat, val));

    ColRes = float4(rgb, alpha);
}

void HSVLerp_half(half4 Col1, half4 Col2, half T, out half4 ColRes)
{
    // Converting colors into HSV space
    Col1.xyz = RGBtoHSV(Col1.xyz);
    Col2.xyz = RGBtoHSV(Col2.xyz);

    half t = T; // used to lerp alpha, needs to remain unchanged

    half hue;
    half d = Col2.x - Col1.x; // hue difference

    if (Col1.x > Col2.x)
    {
        // Swapping hues of colors
        half temp = Col2.x;
        Col2.x = Col1.x;
        Col1.x = temp;

        d = -d;
        T = 1 - T;
    }

    if (d > 0.5)
    {
        Col1.x = Col1.x + 1;
        hue = (Col1.x + T * (Col2.x - Col1.x)) % 1;
    }

    if (d <= 0.5) hue = Col1.x + T * d;

    half sat = Col1.y + T * (Col2.y - Col1.y);
    half val = Col1.z + T * (Col2.z - Col1.z);
    half alpha = Col1.w + t * (Col2.w - Col1.w);

    // Converting result back into RGB space
    half3 rgb = HSVtoRGB(half3(hue, sat, val));

    ColRes = half4(rgb, alpha);
}


/*
* Lightens a color based on saturation and value factors.
*/
void LightenColor_float(float4 Col, float SatFac, float ValFac, out float4 ColRes) 
{
    // Converting color into HSV space
    Col.xyz = RGBtoHSV(Col.xyz);


    float hue = Col.x;
    float sat = saturate(Col.y * (1 - SatFac));
    float val = saturate(Col.z * (1 + ValFac));
    float alpha = Col.w;

    // Converting color back into RGB space
    float3 rgb = HSVtoRGB(float3(hue, sat, val));

    ColRes = float4(rgb, alpha);
}

void LightenColor_float(half4 Col, half SatFac, half ValFac, out half4 ColRes)
{
    // Converting color into HSV space
    Col.xyz = RGBtoHSV(Col.xyz);


    half hue = Col.x;
    half sat = saturate(Col.y * (1 - SatFac));
    half val = saturate(Col.z * (1 + ValFac));
    float alpha = Col.w;

    // Converting color back into RGB space
    half3 rgb = HSVtoRGB(half3(hue, sat, val));

    ColRes = half4(rgb, alpha);
}