Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _InnerColor ("Inner Color", Color) = (0.8,0.95,1,1)
        _OuterColor ("Outer Color", Color) = (0.6,0.8,1,0)
        _NoiseTex ("Noise Tex", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 2.0
        _NoiseSpeed ("Noise Speed", Float) = 1.0
        _Distortion ("Distortion", Float) = 0.5
        _Radius ("Radius", Float) = 1.0
        _EdgeSoft ("Edge Softness", Float) = 0.15
        _Alpha ("Alpha", Float) = 0.9
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _NoiseTex;
            float4 _InnerColor, _OuterColor;
            float _NoiseScale, _NoiseSpeed, _Distortion, _Radius, _EdgeSoft, _Alpha;
            // float4 _Time;

            struct app { float4 vertex : POSITION; };
            struct v2f {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD0;
            };

            v2f vert (app v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.objPos = v.vertex.xyz; // object space pos
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Use XZ plane for radial
                float2 p = i.objPos.xz;
                float radial = length(p);
                float radialNorm = saturate(radial / _Radius);

                // noise uv (object-space)
                float2 uv = p * _NoiseScale;
                uv += _Time.y * _NoiseSpeed; // _Time.y is time
                float noise = tex2D(_NoiseTex, uv).r;

                // color lerp
                float3 col = lerp(_InnerColor.rgb, _OuterColor.rgb, radialNorm + (noise - 0.5)*0.2);

                // alpha mask with edge softness + noise modulation
                float mask = 1.0 - smoothstep(1.0 - _EdgeSoft, 1.0, radialNorm);
                mask *= saturate((noise * 1.5));
                mask *= _Alpha;
                
                // optional distortion: for visual only (not displacing verts)
                col += (noise - 0.5) * _Distortion;
                
                return float4(col * mask, mask);
            }
            ENDCG
        }
    }
}
