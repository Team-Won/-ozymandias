// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UIAnimations"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_DefenseSize("Defense Size", Float) = 0
		_DefenseOffset("Defense Offset", Float) = 1
		_ThreatSize("Threat Size", Float) = 0
		_ThreatOffset("Threat Offset", Float) = 25
		_BarWidth("Bar Width", Float) = 0
		_Cutoff("Cutoff", Float) = 0
		_BarWidthMin("Bar Width Min", Float) = 0
		_BarWidthMax("Bar Width Max", Float) = 0
		_Float3("Float 3", Float) = 15.21
		_Float4("Float 4", Float) = 4.2
		_ShadingPower("Shading Power", Float) = 0
		_ShadingAngle("Shading Angle", Float) = 60
		_ShadingAngle2("Shading Angle 2", Float) = -60
		_ShadingFract("Shading Fract", Float) = 0
		_YShadingOffset("Y Shading Offset", Float) = 0
		_DefenseShadingOffset("Defense Shading Offset", Float) = 0
		_ThreatShadingOffset("Threat Shading Offset", Float) = 0
		_YShadingPower("Y Shading Power", Float) = 0

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform half _DefenseOffset;
			uniform half _DefenseSize;
			uniform half _BarWidthMin;
			uniform half _BarWidthMax;
			uniform half _BarWidth;
			uniform half _ThreatOffset;
			uniform half _ThreatSize;
			uniform half _Cutoff;
			uniform half _ShadingFract;
			uniform half _ThreatShadingOffset;
			uniform half _ShadingAngle2;
			uniform half _DefenseShadingOffset;
			uniform half _ShadingAngle;
			uniform half _ShadingPower;
			uniform half _YShadingOffset;
			uniform half _YShadingPower;
			uniform half _Float4;
			uniform half _Float3;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				half2 appendResult68 = (half2(_DefenseOffset , 2.0));
				half2 texCoord53 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				half temp_output_56_0 = ( texCoord53.x * 26.0 );
				half temp_output_91_0 = ( texCoord53.y * 4.0 );
				half2 appendResult67 = (half2(temp_output_56_0 , temp_output_91_0));
				half temp_output_83_0 = ( distance( appendResult68 , appendResult67 ) - _DefenseSize );
				half clampResult61 = clamp( temp_output_56_0 , _BarWidthMin , _BarWidthMax );
				half2 appendResult64 = (half2(clampResult61 , 2.0));
				half temp_output_92_0 = ( distance( appendResult67 , appendResult64 ) - _BarWidth );
				half2 appendResult84 = (half2(temp_output_56_0 , temp_output_91_0));
				half2 appendResult85 = (half2(_ThreatOffset , 2.0));
				half temp_output_88_0 = ( distance( appendResult84 , appendResult85 ) - _ThreatSize );
				half4 appendResult140 = (half4(( ( 1.0 - temp_output_83_0 ) * texCoord53.y ) , ( ( 1.0 - temp_output_92_0 ) * texCoord53.y ) , ( ( 1.0 - temp_output_88_0 ) * texCoord53.y ) , 1.0));
				half4 break186 = appendResult140;
				half smoothstepResult299 = smoothstep( _Cutoff , 1.0 , ( 1.0 - min( min( temp_output_83_0 , temp_output_92_0 ) , temp_output_88_0 ) ));
				half Mask101 = smoothstepResult299;
				half3 appendResult252 = (half3(( break186.x * Mask101 ) , 0.0 , ( break186.z * Mask101 )));
				half3 PackedMaskedMasks251 = appendResult252;
				half3 break436 = saturate( PackedMaskedMasks251 );
				half2 texCoord439 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				half2 appendResult478 = (half2(frac( ( texCoord439.x * _ShadingFract ) ) , texCoord439.y));
				half2 appendResult499 = (half2(0.0 , _ThreatShadingOffset));
				float cos480 = cos( radians( _ShadingAngle2 ) );
				float sin480 = sin( radians( _ShadingAngle2 ) );
				half2 rotator480 = mul( appendResult478 - appendResult499 , float2x2( cos480 , -sin480 , sin480 , cos480 )) + appendResult499;
				half2 appendResult497 = (half2(0.0 , _DefenseShadingOffset));
				float cos442 = cos( radians( _ShadingAngle ) );
				float sin442 = sin( radians( _ShadingAngle ) );
				half2 rotator442 = mul( appendResult478 - appendResult497 , float2x2( cos442 , -sin442 , sin442 , cos442 )) + appendResult497;
				half2 saferPower458 = abs( ( 1.0 - ( texCoord439.x > 0.5 ? rotator480 : rotator442 ) ) );
				half2 temp_cast_0 = (_ShadingPower).xx;
				half2 appendResult494 = (half2(0.0 , _YShadingOffset));
				half2 texCoord492 = IN.texcoord.xy * float2( 1,1 ) + appendResult494;
				half saferPower502 = abs( texCoord492.y );
				half2 appendResult491 = (half2(( saturate( ( break436.x + break436.z ) ) * pow( saferPower458 , temp_cast_0 ).x ) , saturate( ( 1.0 - pow( saferPower502 , _YShadingPower ) ) )));
				half2 UVTest448 = appendResult491;
				half2 break489 = saturate( UVTest448 );
				half3 break343 = PackedMaskedMasks251;
				half2 texCoord332 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				half temp_output_335_0 = sin( ( ( texCoord332.x * _Float4 ) + _Float3 ) );
				half Shadowmask353 = ( ( saturate( break343.x ) + saturate( break343.z ) ) * temp_output_335_0 );
				half4 appendResult424 = (half4(break489.x , break489.y , Shadowmask353 , Mask101));
				
				half4 color = appendResult424;
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18921
1920;0;1920;1059;-12.42542;722.9374;1.004958;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-3909.661,-1493.105;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;58;-3715.417,-1255.628;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;26;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-3542.556,-1256.22;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-2995.741,-1248.944;Inherit;False;Property;_BarWidthMin;Bar Width Min;9;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;116;-2728.538,-1406.649;Inherit;False;872.2551;349.4607;Bar;6;64;63;92;60;93;61;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;115;-2660.836,-1908.428;Inherit;False;754.5939;386.0146;Defense;7;118;68;67;71;83;76;73;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-2987.741,-1173.944;Inherit;False;Property;_BarWidthMax;Bar Width Max;10;0;Create;True;0;0;0;False;0;False;0;24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-3407.953,-1579.714;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;114;-2684.857,-1026.695;Inherit;False;718.6239;380.481;Threat;7;84;85;86;89;88;87;119;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;61;-2678.538,-1317.557;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;25;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-2647.822,-1829.087;Inherit;False;Property;_DefenseOffset;Defense Offset;4;0;Create;True;0;0;0;False;0;False;1;1.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;68;-2485.863,-1803.091;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-2668.849,-882.3743;Inherit;False;Property;_ThreatOffset;Threat Offset;6;0;Create;True;0;0;0;False;0;False;25;24.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-2447.027,-1259.611;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;67;-2574.866,-1675.808;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode;63;-2273.535,-1306.888;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-2299.273,-1173.189;Inherit;False;Property;_BarWidth;Bar Width;7;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;85;-2570.534,-805.1419;Inherit;False;FLOAT2;4;0;FLOAT;25;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;84;-2592.891,-976.695;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode;73;-2356.345,-1765.574;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-2412.795,-1653.864;Inherit;False;Property;_DefenseSize;Defense Size;3;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;83;-2228.18,-1689.721;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-2435.953,-762.2139;Inherit;False;Property;_ThreatSize;Threat Size;5;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;92;-2148.843,-1303.894;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;86;-2416.336,-883.8401;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;82;-1779.183,-1354.261;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;88;-2288.17,-807.9872;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;94;-1662.54,-1345.604;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;300;-1523.449,-1318.141;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;149;-1737.553,-850.1884;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;147;-1720.865,-1502.666;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;144;-1717.219,-1656.949;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-1587.176,-1230.145;Inherit;False;Property;_Cutoff;Cutoff;8;0;Create;True;0;0;0;False;0;False;0;0.95;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;299;-1349.272,-1320.661;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-1479.123,-896.5204;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-1485.378,-1649.216;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-1540.632,-1499.073;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;140;-1345.08,-1530.686;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-1176.386,-1319.769;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-1231.023,-1668.533;Inherit;False;101;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;439;-5182.782,1002.851;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;468;-5077.385,883.0548;Inherit;False;Property;_ShadingFract;Shading Fract;24;0;Create;True;0;0;0;False;0;False;0;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;186;-1188.061,-1538.231;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-930.435,-1664.604;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;470;-4847.385,897.0547;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-926.3076,-1451.054;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;482;-4881.936,1503.569;Inherit;False;Property;_ShadingAngle2;Shading Angle 2;23;0;Create;True;0;0;0;False;0;False;-60;-49.61;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;444;-4856.366,1388.739;Inherit;False;Property;_ShadingAngle;Shading Angle;22;0;Create;True;0;0;0;False;0;False;60;-56.21;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;498;-4898.338,1181.389;Inherit;False;Property;_DefenseShadingOffset;Defense Shading Offset;27;0;Create;True;0;0;0;False;0;False;0;0.11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;252;-398.2357,-1580.099;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FractNode;469;-4641.628,905.89;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;500;-4924.838,1289.389;Inherit;False;Property;_ThreatShadingOffset;Threat Shading Offset;28;0;Create;True;0;0;0;False;0;False;0;-0.33;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;478;-4508.792,941.5129;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RadiansOpNode;443;-4621.366,1397.739;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;481;-4622.936,1506.569;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;497;-4622.338,1154.389;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;251;-224.5254,-1546.905;Inherit;False;PackedMaskedMasks;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;499;-4642.838,1295.389;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;493;-4331.081,1522.016;Inherit;False;Property;_YShadingOffset;Y Shading Offset;26;0;Create;True;0;0;0;False;0;False;0;0.54;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;434;-3959.007,662.2331;Inherit;True;251;PackedMaskedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RotatorNode;442;-4310.365,1087.739;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;480;-4292.935,1332.569;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;435;-3730.162,658.9861;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Compare;475;-4083.415,1031.585;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;494;-4096.745,1452.283;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;492;-3888.999,1332.893;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;457;-3840.347,1203.201;Inherit;False;Property;_ShadingPower;Shading Power;21;0;Create;True;0;0;0;False;0;False;0;-10.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;503;-3910.655,1486.715;Inherit;False;Property;_YShadingPower;Y Shading Power;29;0;Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;455;-3915.02,1034.475;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;436;-3576.162,664.9861;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;460;-3436.672,685.6075;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;458;-3672.539,1071.429;Inherit;False;True;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;352;-3258.282,37.28274;Inherit;False;907.8655;574.8445;Shadow Mask;12;338;336;339;337;335;341;328;343;348;351;350;332;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;502;-3593.748,1359.974;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;328;-3227.393,81.78854;Inherit;True;251;PackedMaskedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;332;-3208.282,289.9785;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;338;-3129.858,424.9841;Inherit;False;Property;_Float4;Float 4;17;0;Create;True;0;0;0;False;0;False;4.2;5.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;490;-3513.624,1042.963;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.OneMinusNode;504;-3487.942,1238.195;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;461;-3318.672,716.6075;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;505;-3382.266,1177.546;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;336;-3109.724,496.1272;Inherit;False;Property;_Float3;Float 3;16;0;Create;True;0;0;0;False;0;False;15.21;14.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;452;-3168.555,819.0541;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;343;-2917.717,106.2827;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;339;-2970.122,294.7789;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;351;-2784.033,154.8387;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;337;-2776.828,280.0134;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;348;-2781.717,87.28274;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;491;-3030.042,902.2751;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;448;-2858.887,835.5598;Inherit;False;UVTest;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;335;-2653.676,278.9055;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;350;-2635.033,140.8388;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;449;1705.311,435.8248;Inherit;False;448;UVTest;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;341;-2535.417,219.8827;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;472;1954.288,371.0109;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;353;-2298.212,216.7157;Inherit;False;Shadowmask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;489;2156.384,370.9557;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;510;1667.969,566.5967;Inherit;False;353;Shadowmask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;1157.896,-14.9985;Inherit;False;101;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;397;616.7174,85.03387;Inherit;False;211;PackedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-1886.767,281.577;Inherit;False;Property;_SliderMin;Slider Min;0;0;Create;True;0;0;0;False;0;False;0.075;0.075;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;301;-2367.727,1013.999;Inherit;False;Property;_PanSpeed;Pan Speed;12;0;Create;True;0;0;0;False;0;False;0;0.15;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-2094.503,789.2147;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;50;-1988.933,995.326;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1512.464,923.8068;Inherit;False;Property;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;0.1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;49;-1789.234,977.8255;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;-2066.367,570.1153;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-2034.655,426.0905;Inherit;False;Property;_SliderMax;Slider Max;1;0;Create;True;0;0;0;False;0;False;0.925;0.875;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;44;-1569.434,1099.026;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;-1814.106,445.6816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1880.631,593.0428;Inherit;False;Global;Stability;Stability;0;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;210;-1739.619,-1111.704;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;305;-893.6731,463.342;Inherit;False;302;DefenceColors;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;291;-135.1198,12.37366;Inherit;False;Property;_ThreatColor;Threat Color;14;0;Create;True;0;0;0;False;0;False;0.7924528,0,0,1;0.7924528,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;285;-144.9173,-838.5121;Inherit;False;Property;_DefenseColor;Defense Color;13;0;Create;True;0;0;0;False;0;False;0,0.3496235,0.702,1;0,0.3496234,0.702,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;108;-1452.914,501.3542;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1295.993,953.4172;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;211;-1544.409,-1109.75;Inherit;False;PackedMasks;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;424;2263.649,320.5271;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;302;294.9445,-459.0518;Inherit;False;DefenceColors;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;462;-93.3915,993.7204;Inherit;False;448;UVTest;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;190;-425.5276,734.3361;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;430;2008.559,-0.05096054;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;195;802.592,666.6582;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;218;983.665,-202.3562;Inherit;False;100;Color;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;410;803.9075,249.0453;Inherit;False;Property;_ShadowMask;Shadow Mask;18;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;413;808.5056,110.7263;Inherit;False;True;True;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;316;-258.4873,717.9601;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;485;-275.0305,846.2801;Inherit;False;Property;_ShadingMultiplier;Shading Multiplier;25;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;487;104.8445,1000.086;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SmoothstepOpNode;416;1154.559,97.94904;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;508;322.9182,906.0835;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;418;1306.559,97.94904;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;486;-40.03052,793.2801;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;-1052.133,961.3259;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;459;304.3743,645.1786;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;506;302.9949,761.7993;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;355;198.5337,512.5403;Inherit;False;353;Shadowmask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;509;463.9182,687.0835;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;356;436.6736,441.2546;Inherit;False;Constant;_Vector0;Vector 0;15;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;419;1550.559,97.949;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;421;1676.559,98.949;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;426;1673.559,8.949039;Inherit;False;Property;_Shadow1;Shadow 1;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;354;628.7952,633.4672;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;431;1831.559,27.94904;Inherit;False;Property;_Shadow2;Shadow 2;20;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;429;1830.559,118.949;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;420;1432.559,97.94904;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;304;-887.6731,550.342;Inherit;False;303;ThreatColors;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;87;-2099.641,-732.6981;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;1016.072,669.6618;Inherit;False;Color;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FractNode;130;-3524.132,-1728.579;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;279;-441.8491,-338.9123;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;349;-688.3105,-1300.809;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;466;-3821.251,870.1798;Inherit;False;Test;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;124;-3333.857,-1819.783;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;290;178.1355,-92.28038;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FractNode;129;-3625.869,-1806.379;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;-150.3055,-351.7923;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;293;-606.7209,-333.4338;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;292;-133.8255,183.0058;Inherit;False;Constant;_Color5;Color 5;12;0;Create;True;0;0;0;False;0;False;0.404,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;306;-1089.911,-1007.1;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;272;-1229.205,125.5453;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;297;-1062.717,-247.8733;Inherit;False;Property;_BarSmoothing;Bar Smoothing;11;0;Create;True;0;0;0;False;0;False;0;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;310;-586.5956,957.4507;Inherit;False;101;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;408;708.9075,36.04535;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;-1218.922,-428.4159;Inherit;False;211;PackedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DistanceOpNode;365;1334.002,314.6362;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;286;-158.9173,-624.5121;Inherit;False;Constant;_Color3;Color 3;12;0;Create;True;0;0;0;False;0;False;0,0.2221255,0.446,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;412;795.5056,173.7263;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;278;-848.3152,-401.2;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.StepOpNode;287;-390.3845,-50.8977;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;422;1262.559,-196.051;Inherit;False;3;0;FLOAT3;0.01,0.01,0.01;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SmoothstepOpNode;42;-919.3307,785.843;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;131;1515.279,407.6309;Inherit;False;True;True;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;326;-309.6021,981.9026;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;303;342.9445,-99.05182;Inherit;False;ThreatColors;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;427;1775.559,-54.05096;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;281;-1102.675,-124.7487;Inherit;False;251;PackedMaskedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;359;896.5851,338.3596;Inherit;False;357;ShadowMaskDebug;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;125;-3005.599,-1805.42;Inherit;False;DebugUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;294;-613.3519,-88.24643;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;-2036.488,-1749.573;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;432;1511.188,288.0194;Inherit;False;211;PackedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;433;1784.904,295.742;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;248;-1503.636,77.68979;Inherit;False;251;PackedMaskedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;327;-461.6021,1043.903;Inherit;False;Property;_Float0;Float 0;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;411;-999.1029,-402.3314;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;357;-2287.706,385.1952;Inherit;False;ShadowMaskDebug;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;283;116.7796,-468.7107;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;288;-67.79517,-91.70642;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;495;-5174.338,1131.389;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;126;781.2161,-198.1193;Inherit;False;125;DebugUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;330;-827.4614,-1786.274;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;295;-359.9323,112.9685;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;60;-2031.815,-1107.884;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;282;-234.9018,-170.3535;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;120;1776.613,-151.567;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;2343.217,-96.65594;Half;False;True;-1;2;ASEMaterialInspector;0;6;UIAnimations;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;56;0;53;1
WireConnection;56;1;58;0
WireConnection;91;0;53;2
WireConnection;61;0;56;0
WireConnection;61;1;132;0
WireConnection;61;2;133;0
WireConnection;68;0;118;0
WireConnection;64;0;61;0
WireConnection;67;0;56;0
WireConnection;67;1;91;0
WireConnection;63;0;67;0
WireConnection;63;1;64;0
WireConnection;85;0;119;0
WireConnection;84;0;56;0
WireConnection;84;1;91;0
WireConnection;73;0;68;0
WireConnection;73;1;67;0
WireConnection;83;0;73;0
WireConnection;83;1;76;0
WireConnection;92;0;63;0
WireConnection;92;1;93;0
WireConnection;86;0;84;0
WireConnection;86;1;85;0
WireConnection;82;0;83;0
WireConnection;82;1;92;0
WireConnection;88;0;86;0
WireConnection;88;1;89;0
WireConnection;94;0;82;0
WireConnection;94;1;88;0
WireConnection;300;0;94;0
WireConnection;149;0;88;0
WireConnection;147;0;92;0
WireConnection;144;0;83;0
WireConnection;299;0;300;0
WireConnection;299;1;95;0
WireConnection;150;0;149;0
WireConnection;150;1;53;2
WireConnection;141;0;144;0
WireConnection;141;1;53;2
WireConnection;148;0;147;0
WireConnection;148;1;53;2
WireConnection;140;0;141;0
WireConnection;140;1;148;0
WireConnection;140;2;150;0
WireConnection;101;0;299;0
WireConnection;186;0;140;0
WireConnection;175;0;186;0
WireConnection;175;1;173;0
WireConnection;470;0;439;1
WireConnection;470;1;468;0
WireConnection;177;0;186;2
WireConnection;177;1;173;0
WireConnection;252;0;175;0
WireConnection;252;2;177;0
WireConnection;469;0;470;0
WireConnection;478;0;469;0
WireConnection;478;1;439;2
WireConnection;443;0;444;0
WireConnection;481;0;482;0
WireConnection;497;1;498;0
WireConnection;251;0;252;0
WireConnection;499;1;500;0
WireConnection;442;0;478;0
WireConnection;442;1;497;0
WireConnection;442;2;443;0
WireConnection;480;0;478;0
WireConnection;480;1;499;0
WireConnection;480;2;481;0
WireConnection;435;0;434;0
WireConnection;475;0;439;1
WireConnection;475;2;480;0
WireConnection;475;3;442;0
WireConnection;494;1;493;0
WireConnection;492;1;494;0
WireConnection;455;0;475;0
WireConnection;436;0;435;0
WireConnection;460;0;436;0
WireConnection;460;1;436;2
WireConnection;458;0;455;0
WireConnection;458;1;457;0
WireConnection;502;0;492;2
WireConnection;502;1;503;0
WireConnection;490;0;458;0
WireConnection;504;0;502;0
WireConnection;461;0;460;0
WireConnection;505;0;504;0
WireConnection;452;0;461;0
WireConnection;452;1;490;0
WireConnection;343;0;328;0
WireConnection;339;0;332;1
WireConnection;339;1;338;0
WireConnection;351;0;343;2
WireConnection;337;0;339;0
WireConnection;337;1;336;0
WireConnection;348;0;343;0
WireConnection;491;0;452;0
WireConnection;491;1;505;0
WireConnection;448;0;491;0
WireConnection;335;0;337;0
WireConnection;350;0;348;0
WireConnection;350;1;351;0
WireConnection;341;0;350;0
WireConnection;341;1;335;0
WireConnection;472;0;449;0
WireConnection;353;0;341;0
WireConnection;489;0;472;0
WireConnection;50;1;301;0
WireConnection;49;0;19;0
WireConnection;49;2;50;0
WireConnection;113;0;41;0
WireConnection;44;0;49;0
WireConnection;112;0;107;0
WireConnection;112;1;113;0
WireConnection;210;0;83;0
WireConnection;210;1;92;0
WireConnection;210;2;88;0
WireConnection;108;0;106;0
WireConnection;108;1;112;0
WireConnection;108;2;20;0
WireConnection;45;0;41;0
WireConnection;45;1;44;0
WireConnection;211;0;210;0
WireConnection;424;0;489;0
WireConnection;424;1;489;1
WireConnection;424;2;510;0
WireConnection;424;3;103;0
WireConnection;302;0;285;0
WireConnection;190;0;305;0
WireConnection;190;1;304;0
WireConnection;190;2;42;0
WireConnection;430;0;429;0
WireConnection;430;1;431;0
WireConnection;195;0;354;0
WireConnection;413;0;397;0
WireConnection;316;0;190;0
WireConnection;487;0;462;0
WireConnection;416;0;413;0
WireConnection;416;2;410;0
WireConnection;508;0;487;0
WireConnection;508;1;487;1
WireConnection;418;0;416;0
WireConnection;486;0;316;0
WireConnection;486;1;485;0
WireConnection;43;0;108;0
WireConnection;43;1;45;0
WireConnection;459;0;316;0
WireConnection;459;1;486;0
WireConnection;459;2;487;0
WireConnection;506;0;316;0
WireConnection;506;1;486;0
WireConnection;506;2;508;0
WireConnection;509;0;459;0
WireConnection;509;1;506;0
WireConnection;419;0;420;0
WireConnection;419;1;420;1
WireConnection;419;2;420;2
WireConnection;421;0;419;0
WireConnection;354;0;509;0
WireConnection;354;1;356;0
WireConnection;354;2;355;0
WireConnection;429;0;421;0
WireConnection;429;1;426;0
WireConnection;420;0;418;0
WireConnection;100;0;195;0
WireConnection;130;0;91;0
WireConnection;279;0;293;0
WireConnection;124;0;129;0
WireConnection;124;1;130;0
WireConnection;290;1;292;0
WireConnection;290;2;288;0
WireConnection;129;0;56;0
WireConnection;280;0;279;0
WireConnection;280;1;282;0
WireConnection;293;0;278;0
WireConnection;293;1;297;0
WireConnection;272;0;248;0
WireConnection;278;0;411;0
WireConnection;287;0;294;0
WireConnection;422;1;218;0
WireConnection;422;2;427;0
WireConnection;42;0;19;1
WireConnection;42;1;108;0
WireConnection;42;2;43;0
WireConnection;326;0;310;0
WireConnection;326;1;327;0
WireConnection;303;0;291;0
WireConnection;427;0;430;0
WireConnection;125;0;124;0
WireConnection;294;0;278;2
WireConnection;294;1;297;0
WireConnection;433;0;432;0
WireConnection;411;0;213;0
WireConnection;357;0;335;0
WireConnection;283;1;286;0
WireConnection;283;2;280;0
WireConnection;288;0;287;0
WireConnection;288;1;282;2
WireConnection;282;0;281;0
WireConnection;120;0;422;0
WireConnection;120;3;103;0
WireConnection;3;0;424;0
ASEEND*/
//CHKSM=7982CFB88E7D63DDD2A5C3CC95661577E95F075C