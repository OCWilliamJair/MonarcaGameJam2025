Shader "Unlit/BlinkShader"
{
    Properties
    {
        _Blink ("Blink Amount", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            Name "BlinkPass"
            ZTest Always ZWrite Off Cull Off Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _Blink;

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings VertDefault(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                // Simulación de párpados
                float y = abs(i.uv.y - 0.5) * 2.0; // distancia al centro
                float mask = smoothstep(0.5 - _Blink * 0.5, 0.5, y);

                return lerp(0, 1, mask).xxxx; // Negro con alpha
            }
            ENDHLSL
        }
    }
}
