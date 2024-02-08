
Shader "Custom/InstancedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 80

        Pass
        {
            Tags{ "RenderType" = "Opaque" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };

            struct Input
            {
                float2 uv_MainTex;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            StructuredBuffer<float3> positionBuffer;
            StructuredBuffer<float4> colorBuffer;

            v2f vert(appdata_base v, uint svInstanceID : SV_InstanceID)
            {
                InitIndirectDrawArgs(0);
                v2f o;
                uint instanceID = GetIndirectInstanceID(svInstanceID);
                float3 pos = positionBuffer[instanceID];
                float4 color = colorBuffer[instanceID];
                o.pos = mul(UNITY_MATRIX_VP, float4(v.vertex.xyz + pos, 1.0f));
                o.color = color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"
}
