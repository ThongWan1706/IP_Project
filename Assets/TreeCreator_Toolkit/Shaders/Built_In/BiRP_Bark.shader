// Made with Amplify Shader Editor v1.9.9.9
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Tree Creator/BiRP/bark"
{
	Properties
	{
		_BaseColor( "Color", Color ) = ( 1, 1, 1, 1 )
		_MainTex( "Albedo", 2D ) = "white" {}
		[Normal] _BumpSpecMap( "Normal", 2D ) = "bump" {}
		_NormalPower( "Normal Power", Range( 0, 2 ) ) = 0
		_SmoothnessMap( "Smoothness Map", 2D ) = "white" {}
		_Smoothness( "Smoothness", Range( 0, 1 ) ) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19909
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float WorldFrequency;
		uniform float WindSpeed;
		uniform float BendAmount;
		uniform sampler2D _BumpSpecMap;
		uniform float4 _BumpSpecMap_ST;
		uniform float _NormalPower;
		uniform float4 _BaseColor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _SmoothnessMap;
		uniform float4 _SmoothnessMap_ST;
		uniform float _Smoothness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_positionOS = v.vertex.xyz;
			float3 ase_positionWS = mul( unity_ObjectToWorld, v.vertex );
			float temp_output_51_0 = ( ( ase_positionOS.y * cos( ( ( ( ase_positionWS.x + ase_positionWS.z ) * WorldFrequency ) + ( _Time.z * WindSpeed ) ) ) ) * BendAmount );
			float4 appendResult52 = (float4(temp_output_51_0 , 0.0 , temp_output_51_0 , 0.0));
			float4 break60 = mul( appendResult52, unity_ObjectToWorld );
			float4 appendResult58 = (float4(break60.x , 0 , break60.z , 0.0));
			v.vertex.xyz += appendResult58.xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpSpecMap = i.uv_texcoord * _BumpSpecMap_ST.xy + _BumpSpecMap_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _BumpSpecMap, uv_BumpSpecMap ), _NormalPower );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Albedo = ( _BaseColor * tex2D( _MainTex, uv_MainTex ) ).rgb;
			float2 uv_SmoothnessMap = i.uv_texcoord * _SmoothnessMap_ST.xy + _SmoothnessMap_ST.zw;
			o.Smoothness = ( tex2D( _SmoothnessMap, uv_SmoothnessMap ) * _Smoothness ).r;
			o.Alpha = 1;
		}

		ENDCG
	}

	Dependency "OptimizedShader"="Tree Creator/BiRP/bark"
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19909
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;62;-2102.425,412.5636;Inherit;False;2402.447;754.1009;Bark Wind;19;39;40;42;41;44;43;49;45;46;50;51;52;54;53;61;60;58;64;63;Bark Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;39;-2052.425,502.6556;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;44;-1952.367,789.0685;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-1836.481,526.2342;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;64;-1954.712,958.3776;Inherit;False;Global;WindSpeed;Wind Speed;5;0;Create;True;0;0;0;False;0;False;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;42;-2038.859,675.2864;Inherit;False;Global;WorldFrequency;World Frequency;7;0;Create;True;0;0;0;False;0;False;0.07;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;63;-1614.712,874.3776;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;41;-1693.533,576.8155;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;43;-1536.062,691.6006;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;49;-1446.313,462.5636;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CosOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;45;-1453.256,812.1072;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;46;-1264.763,656.5163;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;50;-1318.766,829.2862;Inherit;False;Global;BendAmount;Bend Amount;6;0;Create;True;0;0;0;False;0;False;0.04;0;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;51;-1046.406,688.2439;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;52;-868.6003,641.0502;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;54;-884.6503,915.0601;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;74;-931.7793,201.429;Inherit;False;652.0133;186.3524;Smoothness;2;71;72;Smoothness;0,0.7815788,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;53;-644.332,818.3277;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;38;-918.8992,-863.7786;Inherit;False;620.871;466.0933;Albedo;3;15;11;16;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;75;-927.4517,-365.7581;Inherit;False;370;280;Normal Map;1;65;Normal;1,0.3221376,0.0235849,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;71;-881.7793,271.7813;Inherit;False;Property;_Smoothness;Smoothness;5;0;Create;False;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;79;-1263.885,-294.9929;Inherit;False;Property;_NormalPower;Normal Power;3;0;Create;True;0;0;0;False;0;False;0;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;60;-470.6645,822.7811;Inherit;True;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.Vector3Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;61;-589.1002,584.9073;Inherit;False;Constant;_Vector0;Vector 0;4;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-821.5477,-813.7786;Inherit;False;Property;_BaseColor;Color;0;0;Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;11;-866.1211,-627.6852;Inherit;True;Property;_MainTex;Albedo;1;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;80;-805.4141,6.859802;Inherit;True;Property;_SmoothnessMap;Smoothness Map;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;16;-460.0282,-657.8006;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;72;-441.766,251.429;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;58;-189.6114,726.7914;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;65;-879.2642,-315.2754;Inherit;True;Property;_BumpSpecMap;Normal;2;1;[Normal];Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;81;371.4136,224.1944;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;Tree Creator/BiRP/bark;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;0;False;;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;1;OptimizedShader=Tree Creator/BiRP/bark;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;39;1
WireConnection;40;1;39;3
WireConnection;63;0;44;3
WireConnection;63;1;64;0
WireConnection;41;0;40;0
WireConnection;41;1;42;0
WireConnection;43;0;41;0
WireConnection;43;1;63;0
WireConnection;45;0;43;0
WireConnection;46;0;49;2
WireConnection;46;1;45;0
WireConnection;51;0;46;0
WireConnection;51;1;50;0
WireConnection;52;0;51;0
WireConnection;52;2;51;0
WireConnection;53;0;52;0
WireConnection;53;1;54;0
WireConnection;60;0;53;0
WireConnection;16;0;15;0
WireConnection;16;1;11;0
WireConnection;72;0;80;0
WireConnection;72;1;71;0
WireConnection;58;0;60;0
WireConnection;58;1;61;2
WireConnection;58;2;60;2
WireConnection;65;5;79;0
WireConnection;81;0;16;0
WireConnection;81;1;65;0
WireConnection;81;4;72;0
WireConnection;81;11;58;0
ASEEND*/
//CHKSM=1060339703B6E2E86A17C6BAB91F85E18E86B095