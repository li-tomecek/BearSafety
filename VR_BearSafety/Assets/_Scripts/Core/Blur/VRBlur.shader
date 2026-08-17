Shader "Custom/VRBlur"
{
    Properties
    {
        _BlurAmount ("Blur Amount", Range(0, 15)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        ZWrite Off
        Cull Off

        Pass
        {
            Name "VRBlur"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            // IMPORTANT:
            // Core.hlsl must come before Blit.hlsl.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _BlurAmount;

            half4 Frag(Varyings input) : SV_Target
            {
                // Required for VR single-pass instanced rendering
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord.xy;

                // Pixel size of the screen
                float2 texelSize = _BlitTexture_TexelSize.xy;

                float2 offset = texelSize * _BlurAmount;

                half4 color = 0;

                // Center
                color += SAMPLE_TEXTURE2D_X_LOD(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv,
                    _BlitMipLevel
                ) * 0.20;

                // Left
                color += SAMPLE_TEXTURE2D_X_LOD(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + float2(-offset.x, 0),
                    _BlitMipLevel
                ) * 0.15;

                // Right
                color += SAMPLE_TEXTURE2D_X_LOD(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + float2(offset.x, 0),
                    _BlitMipLevel
                ) * 0.15;

                // Up
                color += SAMPLE_TEXTURE2D_X_LOD(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + float2(0, offset.y),
                    _BlitMipLevel
                ) * 0.15;

                // Down
                color += SAMPLE_TEXTURE2D_X_LOD(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + float2(0, -offset.y),
                    _BlitMipLevel
                ) * 0.15;

                // Diagonal 1
                color += SAMPLE_TEXTURE2D_X_LOD(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + float2(offset.x, offset.y),
                    _BlitMipLevel
                ) * 0.05;

                // Diagonal 2
                color += SAMPLE_TEXTURE2D_X_LOD(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + float2(-offset.x, -offset.y),
                    _BlitMipLevel
                ) * 0.05;

                // Diagonal 3
                color += SAMPLE_TEXTURE2D_X_LOD(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + float2(offset.x, -offset.y),
                    _BlitMipLevel
                ) * 0.05;

                // Diagonal 4
                color += SAMPLE_TEXTURE2D_X_LOD(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + float2(-offset.x, offset.y),
                    _BlitMipLevel
                ) * 0.05;

                return color;
            }

            ENDHLSL
        }
    }
}