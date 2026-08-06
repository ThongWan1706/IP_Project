// Made with Amplify Shader Editor v1.9.9.9
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Tree Creator/BiRP/leaves (grass)"
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
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
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
			float2 uv_MainTex = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			float3 ase_positionOS = v.vertex.xyz;
			float2 appendResult78 = (float2(( uv_MainTex.x * sin( ( ( ase_positionOS.x + 0.0 + ( _Time.y * _LocalWindSpeed ) ) / _LocalWindScale ) ) ) , 0.0));
			v.vertex.xyz += float3( ( appendResult78 * _LocalWindIntensity ) ,  0.0 );
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

	Dependency "OptimizedShader"="Tree Creator/BiRP/leaves (grass)"
	Dependency "BillboardShader"="Tree Creator/BiRP/leaves (grass)1"
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19909
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;67;-785.7,811.0025;Inherit;False;1512.816;522.3508;Leaf Animation;13;80;79;78;77;76;75;74;73;72;71;70;68;82;Leaf Animation;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;68;-652.494,1039.785;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;82;-690.7825,1158.265;Inherit;False;Property;_LocalWindSpeed;Local Wind Speed;10;0;Create;True;0;0;0;False;0;False;0.43;0.43;0;7;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;70;-413.3348,1068.048;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;71;-625.0398,861.0024;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;72;-276.0372,945.6144;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;73;-376.5129,1195.024;Inherit;False;Property;_LocalWindScale;Local Wind Scale;8;0;Create;True;0;0;0;False;0;False;0.354;0.354;0.03;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;74;-113.3671,1030.814;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;94;345.2501,132.0741;Inherit;False;557.7764;374.2971;Temp;5;99;98;97;96;95;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;86;-549.9623,-289.3118;Inherit;False;713.8566;533.4713;Albedo;5;11;15;83;16;84;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;75;-53.28228,864.7184;Inherit;False;0;11;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;76;49.20071,1079.354;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;77;275.6235,893.7924;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;96;395.2501,268.1034;Inherit;False;Property;_ShadowStrength;n2;19;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;95;396.3333,182.0741;Inherit;False;Property;_TranslucencyViewDependency;n1;18;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;11;-499.9623,-60.83265;Inherit;True;Property;_MainTex;Albedo;14;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-464.7936,-239.3118;Inherit;False;Property;_Color;Color;11;0;Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;97;570.6484,212.0359;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;92;431.6947,-183.8605;Inherit;True;Property;_GlossMap;Smoothness Map;17;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;16;-175.6867,-111.9259;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;78;384.7766,1006.521;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;99;469.938,393.3712;Inherit;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;450.3834,-299.728;Inherit;False;Property;_Shininess;Smoothness;13;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;93;-979.288,364.7839;Inherit;False;Property;_NormalPower;Normal Power;16;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;83;-489.9117,128.1595;Inherit;False;Property;_ColorIntensity;Color Intensity;12;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;79;254.8943,1134.33;Inherit;False;Property;_LocalWindIntensity;Local Wind Intensity;9;0;Create;True;0;0;0;False;0;False;0.191;0.191;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;87;762.7786,-291.0627;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;84;1.894313,-6.360741;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;80;565.1161,1019.49;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;98;725.0267,328.8639;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;100;1242.355,266.5962;Inherit;False;Property;_TranslucencyColor;Translucency Color;20;0;Create;False;0;0;0;False;0;False;0.7294118,0.8509805,0.4117647,1;0.7294118,0.8509805,0.4117647,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;102;1356.704,186.6705;Inherit;False;Constant;_Float1;Float 1;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;85;-517.8002,334.4368;Inherit;True;Property;_BumpMap;Normal;15;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;101;1564.606,81.88202;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;Tree Creator/BiRP/leaves (grass);False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;0;False;;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;1;-1;-1;0;False;2;OptimizedShader=Tree Creator/BiRP/leaves (grass);BillboardShader=Tree Creator/BiRP/leaves (grass)1;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;70;0;68;0
WireConnection;70;1;82;0
WireConnection;72;0;71;1
WireConnection;72;2;70;0
WireConnection;74;0;72;0
WireConnection;74;1;73;0
WireConnection;76;0;74;0
WireConnection;77;0;75;1
WireConnection;77;1;76;0
WireConnection;97;0;95;0
WireConnection;97;1;96;0
WireConnection;16;0;15;0
WireConnection;16;1;11;0
WireConnection;78;0;77;0
WireConnection;87;0;17;0
WireConnection;87;1;92;0
WireConnection;84;0;16;0
WireConnection;84;1;83;0
WireConnection;80;0;78;0
WireConnection;80;1;79;0
WireConnection;98;0;97;0
WireConnection;98;1;99;0
WireConnection;85;5;93;0
WireConnection;101;0;84;0
WireConnection;101;1;85;0
WireConnection;101;2;98;0
WireConnection;101;3;102;0
WireConnection;101;4;87;0
WireConnection;101;7;100;0
WireConnection;101;10;11;4
WireConnection;101;11;80;0
ASEEND*/
//CHKSM=A5EB7AEF1EE57A678C9DB14C3CCE3C18A7238F17