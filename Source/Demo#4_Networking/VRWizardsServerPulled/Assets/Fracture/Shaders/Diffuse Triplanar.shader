#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Destruction/Diffuse - Triplanar" 
{
	Properties 
	{
		_Colour			("Main Colour", Color)				= (0.5, 0.5, 0.5, 0.5)
		_MainTex		("Main Texture", 2D)				= "white" {}

		_BlendFactor	("Blend Factor", Range(0.01, 20))	= 5

		_SideScale		("Side Scale", Vector)				= (0.5, 0.5, 0.5, 0.5)
		_TopScale		("Top Scale", Vector)				= (0.5, 0.5, 0.5, 0.5)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM

		#pragma surface surf Lambert vertex:vert addshadow	
		#pragma target 3.0

		float4 _Colour;
		sampler2D _MainTex;
		float _BlendFactor;
		float4 _SideScale;
		float4 _TopScale;

		struct Input 
		{
			float3 localPos;
			float3 localNormal;
		}; 
      
		void vert (inout appdata_full v, out Input o) 
		{
			o.localPos = v.vertex / 1.0;
			o.localNormal = v.normal;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float3 projNormal = saturate(pow(IN.localNormal, _BlendFactor));

			float2 uvX = IN.localPos.zy * _SideScale.xy + _SideScale.zw;
			float3 x = tex2D(_MainTex, uvX) * abs(IN.localNormal.x);

			float2 uvY = IN.localPos.zx * _TopScale.xy + _TopScale.zw;
			float3 y = tex2D(_MainTex, uvY) * abs(IN.localNormal.y);

			float2 uvZ = IN.localPos.xy * _SideScale.xy + _SideScale.zw;
			float3 z = tex2D(_MainTex, uvZ) * abs(IN.localNormal.z);

			o.Albedo = z;
			o.Albedo = lerp(o.Albedo, x, projNormal.x);
			o.Albedo = lerp(o.Albedo, y, projNormal.y);
			o.Albedo *= _Colour;
		}

		ENDCG
	}
}