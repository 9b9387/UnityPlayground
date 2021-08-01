TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

void DepthTexture_float(float2 UV, out float2 Out)
{
    Out = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, UV);
}