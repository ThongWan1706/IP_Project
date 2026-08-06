// Made with Amplify Shader Editor v1.9.9.9
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Tree Creator/BiRP/leaves"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[Header(Translucency)]
		_Translucency( "Strength", Range( 0, 50 ) ) = 1
		_TransNormalDistortion( "Normal Distortion", Range( 0, 1 ) ) = 0.1
		_TransScattering( "Scaterring Falloff", Range( 1, 50 ) ) = 2
		_TransDirect( "Direct", Range( 0, 1 ) ) = 1
		_TransAmbient( "Ambient", Range( 0, 1 ) ) = 0.2
		_TransShadow( "Shadow", Range( 0, 1 ) ) = 0.9
		_LocalWindScale( "Local Wind Scale", Range( 0.03, 1 ) ) = 0.354
		_LocalWindIntensity( "Local Wind Intensity", Range( 0, 1 ) ) = 0.191
		_LocalWindSpeed( "Local Wind Speed", Range( 0, 7 ) ) = 0.43
		_Color( "Color", Color ) = ( 1, 1, 1, 1 )
		_ColorIntensity( "Color Intensity", Range( 0, 10 ) ) = 1
		_Shininess( "Smoothness", Range( 0, 1 ) ) = 0
		_MainTex( "Albedo", 2D ) = "white" {}
		_BumpMap( "Normal", 2D ) = "bump" {}
		_NormalPower( "Normal Power", Range( 0, 2 ) ) = 0
		_GlossMap( "Smoothness Map", 2D ) = "white" {}
		_TranslucencyViewDependency( "n1", Float ) = 0
		_ShadowStrength( "n2", Float ) = 0
		_TranslucencyColor( "Translucency Color", Color ) = ( 0.7294118, 0.8509805, 0.4117647, 1 )
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TreeTransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19909
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform float WorldFrequency;
		uniform float WindSpeed;
		uniform float BendAmount;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _LocalWindSpeed;
		uniform float _LocalWindScale;
		uniform float _LocalWindIntensity;
		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float _NormalPower;
		uniform float4 _Color;
		uniform float _ColorIntensity;
		uniform float _TranslucencyViewDependency;
		uniform float _ShadowStrength;
		uniform float _Shininess;
		uniform sampler2D _GlossMap;
		uniform float4 _GlossMap_ST;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float4 _TranslucencyColor;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_positionOS = v.vertex.xyz;
			float3 ase_positionWS = mul( unity_ObjectToWorld, v.vertex );
			float temp_output_51_0 = ( ( ase_positionOS.y * cos( ( ( ( ase_positionWS.x + ase_positionWS.z ) * WorldFrequency ) + ( _Time.z * WindSpeed ) ) ) ) * BendAmount );
			float4 appendResult52 = (float4(temp_output_51_0 , 0.0 , temp_output_51_0 , 0.0));
			float4 break60 = mul( appendResult52, unity_ObjectToWorld );
			float4 appendResult58 = (float4(break60.x , 0 , break60.z , 0.0));
			float2 uv_MainTex = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 appendResult78 = (float2(0.0 , ( uv_MainTex.x * sin( ( ( ase_positionOS.x + 0.0 + ( _Time.y * _LocalWindSpeed ) ) / _LocalWindScale ) ) )));
			v.vertex.xyz += ( appendResult58 + float4( ( appendResult78 * _LocalWindIntensity ), 0.0 , 0.0 ) ).xyz;
			v.vertex.w = 1;
		}

		inline half4 LightingStandardCustom( SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !defined(DIRECTIONAL)
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _BumpMap, uv_BumpMap ), _NormalPower );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode11 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( ( _Color * tex2DNode11 ) * _ColorIntensity ).rgb;
			float3 temp_cast_1 = (( ( _TranslucencyViewDependency * _ShadowStrength ) * 0.0 )).xxx;
			o.Emission = temp_cast_1;
			o.Metallic = 0.0;
			float2 uv_GlossMap = i.uv_texcoord * _GlossMap_ST.xy + _GlossMap_ST.zw;
			o.Smoothness = ( _Shininess * tex2D( _GlossMap, uv_GlossMap ) ).r;
			o.Translucency = _TranslucencyColor.rgb;
			o.Alpha = 1;
			clip( tex2DNode11.a - _Cutoff );
		}

		ENDCG
	}

	Dependency "OptimizedShader"="Tree Creator/BiRP/leaves"
	Dependency "BillboardShader"="Tree Creator/BiRP/leaves"
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19909
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;62;-2117.218,843.7501;Inherit;False;2402.447;754.1009;Bark Wind;19;39;40;42;41;43;49;45;46;50;51;52;54;53;61;60;58;64;65;66;Bark Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;39;-2067.218,933.8421;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;65;-2085.273,1410.69;Inherit;False;Global;WindSpeed;Wind Speed;5;0;Create;True;0;0;0;False;0;False;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;42;-2053.652,1106.473;Inherit;False;Global;WorldFrequency;World Frequency;2;0;Create;True;0;0;0;False;0;False;0.07;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-1851.274,957.4207;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;64;-2082.928,1241.38;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;94;-1019.582,1847.431;Inherit;False;1786.098;693.2806;Leaf Animation;13;68;82;70;71;72;73;74;75;76;77;78;80;79;Leaf Animation;0.5422678,1,0.5235849,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;66;-1745.272,1326.69;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;41;-1708.326,1008.002;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;43;-1550.855,1122.787;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;68;-931.2938,2076.214;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;82;-969.5823,2194.694;Inherit;False;Property;_LocalWindSpeed;Local Wind Speed;10;0;Create;True;0;0;0;False;0;False;0.43;0.43;0;7;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;49;-1461.106,893.7501;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CosOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;45;-1468.049,1243.294;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;70;-692.1348,2104.477;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;71;-903.8397,1897.431;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;46;-1279.556,1087.703;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;50;-1333.559,1260.473;Inherit;False;Global;BendAmount;Bend Amount;3;0;Create;True;0;0;0;False;0;False;0.04;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;72;-554.8371,1982.043;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;73;-655.3129,2231.453;Inherit;False;Property;_LocalWindScale;Local Wind Scale;8;0;Create;True;0;0;0;False;0;False;0.354;0.354;0.03;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;51;-1061.199,1119.43;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;74;-392.1668,2067.243;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;54;-899.4432,1346.247;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;52;-883.3931,1072.237;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;75;-332.0819,1901.147;Inherit;False;0;11;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;76;-266.1944,2115.782;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;102;432.5289,154.1583;Inherit;False;557.7764;374.2971;Temp;5;97;98;99;100;101;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;86;-549.9623,-289.3118;Inherit;False;713.8566;533.4713;Albedo;5;11;15;83;16;84;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;53;-659.1248,1249.514;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;77;299.0229,1919.263;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;97;483.612,202.3718;Inherit;False;Property;_TranslucencyViewDependency;n1;18;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;98;482.5289,288.1335;Inherit;False;Property;_ShadowStrength;n2;19;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-464.7936,-239.3118;Inherit;False;Property;_Color;Color;11;0;Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;11;-495.7109,-60.83265;Inherit;True;Property;_MainTex;Albedo;14;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.Vector3Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;61;-603.8931,1016.094;Inherit;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;60;-485.4574,1253.968;Inherit;True;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;78;408.1763,2031.992;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;79;171.3225,2148.542;Inherit;False;Property;_LocalWindIntensity;Local Wind Intensity;9;0;Create;True;0;0;0;False;0;False;0.191;0.191;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;96;-810.5325,407.6149;Inherit;False;Property;_NormalPower;Normal Power;16;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;99;657.9268,234.1201;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;450.3834,-299.728;Inherit;False;Property;_Shininess;Smoothness;13;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;95;452.9453,-220.4111;Inherit;True;Property;_GlossMap;Smoothness Map;17;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;101;557.2166,415.4553;Inherit;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;16;-175.6867,-111.9259;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;83;-489.9117,128.1595;Inherit;False;Property;_ColorIntensity;Color Intensity;12;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;58;-204.4043,1157.978;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;80;588.5162,2044.96;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;87;762.7786,-291.0627;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;84;1.894313,-6.360741;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;81;645.7957,1124.966;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;100;812.3053,350.948;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;104;1483.813,586.1253;Inherit;False;Property;_TranslucencyColor;Translucency Color;20;0;Create;False;0;0;0;False;0;False;0.7294118,0.8509805,0.4117647,1;0.7294118,0.8509805,0.4117647,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;106;1553.519,308.268;Inherit;False;Constant;_Float1;Float 1;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;85;-517.8002,333.4368;Inherit;True;Property;_BumpMap;Normal;15;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;105;1731.701,188.597;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;Tree Creator/BiRP/leaves;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;0;False;;0;Custom;0.5;True;True;0;True;TreeTransparentCutout;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;1;-1;-1;0;False;2;OptimizedShader=Tree Creator/BiRP/leaves;BillboardShader=Tree Creator/BiRP/leaves;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;39;1
WireConnection;40;1;39;3
WireConnection;66;0;64;3
WireConnection;66;1;65;0
WireConnection;41;0;40;0
WireConnection;41;1;42;0
WireConnection;43;0;41;0
WireConnection;43;1;66;0
WireConnection;45;0;43;0
WireConnection;70;0;68;0
WireConnection;70;1;82;0
WireConnection;46;0;49;2
WireConnection;46;1;45;0
WireConnection;72;0;71;1
WireConnection;72;2;70;0
WireConnection;51;0;46;0
WireConnection;51;1;50;0
WireConnection;74;0;72;0
WireConnection;74;1;73;0
WireConnection;52;0;51;0
WireConnection;52;2;51;0
WireConnection;76;0;74;0
WireConnection;53;0;52;0
WireConnection;53;1;54;0
WireConnection;77;0;75;1
WireConnection;77;1;76;0
WireConnection;60;0;53;0
WireConnection;78;1;77;0
WireConnection;99;0;97;0
WireConnection;99;1;98;0
WireConnection;16;0;15;0
WireConnection;16;1;11;0
WireConnection;58;0;60;0
WireConnection;58;1;61;2
WireConnection;58;2;60;2
WireConnection;80;0;78;0
WireConnection;80;1;79;0
WireConnection;87;0;17;0
WireConnection;87;1;95;0
WireConnection;84;0;16;0
WireConnection;84;1;83;0
WireConnection;81;0;58;0
WireConnection;81;1;80;0
WireConnection;100;0;99;0
WireConnection;100;1;101;0
WireConnection;85;5;96;0
WireConnection;105;0;84;0
WireConnection;105;1;85;0
WireConnection;105;2;100;0
WireConnection;105;3;106;0
WireConnection;105;4;87;0
WireConnection;105;7;104;0
WireConnection;105;10;11;4
WireConnection;105;11;81;0
ASEEND*/
//CHKSM=53AE06DF3704E97C5EFE089026F9F37EC6CCB0CD