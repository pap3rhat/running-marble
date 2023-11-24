//------------------------------------------------------------------------------------------------------
// Color conversions. Taken from: 
//------------------------------------------------------------------------------------------------------

/* 
* Converts RGB to gray value.
*  formula is the same as OpenCV uses: https://docs.opencv.org/2.4/modules/imgproc/doc/miscellaneous_transformations.html#void%20cvtColor%28InputArray%20src,%20OutputArray%20dst,%20int%20code,%20int%20dstCn%29
*/ 
void RGBtoGray_float(float4 col, out float grayCol)
{
    grayCol =  0.299 * col.x + 0.587 * col.y + 0.114 * col.z;
}

void RGBtoGray_half(half4 col, out half grayCol)
{
    grayCol = 0.299 * col.x + 0.587 * col.y + 0.114 * col.z;
}