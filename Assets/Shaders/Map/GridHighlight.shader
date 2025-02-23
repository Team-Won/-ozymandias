﻿ Shader "Grid/GridHighlight"
{
    Properties
    {
        _Mask("Mask Texture", 2D) = "white" {}
        _AlphaMask ("Mask Texture", 2D) = "white" {}

        _Origin ("World-Space Effect Origin", Vector) = (0, 0, 0, 0)
		_Radius ("World-Space Effect Radius", Float) = 10
		_Exponent ("Exponent", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _Mask;
            sampler2D _AlphaMask;
			float4 _Inactive;
			float4 _Active;
			float4 _Invalid;

			float4 _Origin;
			float _Radius;
			float _Exponent;
            float _GridOpacity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (const v2f i) : SV_Target
            {
                float alpha = tex2D(_AlphaMask, i.uv).r;
                float4 col = i.color;
                col.a = alpha * i.color.a;

                const float dist = distance(i.worldPos, _Origin);
                const float effect = pow(saturate(dist / _Radius), _Exponent);
				
                return lerp(col, float4(0,0,0,0), effect);
            }
            ENDCG
        }
    }
}
