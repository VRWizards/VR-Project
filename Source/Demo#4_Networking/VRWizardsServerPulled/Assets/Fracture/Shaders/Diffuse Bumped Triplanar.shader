#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Destruction/Diffuse Bumped - Triplanar" 
{
	Properties 
	{
		_Colour			("Main Colour", Color)				= (0.5, 0.5, 0.5, 0.5)
		_MainTex		("Main Texture", 2D)				= "white" {}
		_BumpMap		("Normal Map", 2D)					= "white" {}

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
		sampler2D _BumpMap;
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
            float3 blend_weights = abs( IN.localNormal );
            blend_weights = (blend_weights - 0.2) * 7; 
            blend_weights = max(blend_weights, 0);      // Force weights to sum to 1.0 (very important!) 
            blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z ).xxx;
            
			float2 uvX = IN.localPos.zy * _SideScale.xy + _SideScale.zw;
			float2 uvY = IN.localPos.zx * _TopScale.xy + _TopScale.zw;
			float2 uvZ = IN.localPos.xy * _SideScale.xy + _SideScale.zw;

            half4 col1 = tex2D (_MainTex, uvX);
            half4 col2 = tex2D (_MainTex, uvY);
            half4 col3 = tex2D (_MainTex, uvZ);

            half4 blended_color = col1.xyzw * blend_weights.xxxx + 
            col2.xyzw * blend_weights.yyyy + 
            col3.xyzw * blend_weights.zzzz;
            
            half3 n1 = UnpackNormal(tex2D (_BumpMap, uvX));
            half3 n2 = UnpackNormal(tex2D (_BumpMap, uvY));
            half3 n3 = UnpackNormal(tex2D (_BumpMap, uvZ));

            half3 blended_normal = n1.xyz * blend_weights.xxx + 
            n2.xyz * blend_weights.yyy + 
            n3.xyz * blend_weights.zzz;

			o.Albedo = blended_color.rgb;
            o.Normal = blended_normal;
		}

		ENDCG
	}
}