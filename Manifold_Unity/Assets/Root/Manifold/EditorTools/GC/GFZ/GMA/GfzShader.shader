Shader "Custom/GfzShader"
{
	Properties
	{
		[Header(Colors)][Space]
		_Color("Color", Color) = (1,1,1,1)
		_AmbientColor("AmbientColor", Color) = (1,1,1,1)
		_SpecularColor("SpecularColor", Color) = (1,1,1,1)
		_Alpha("Alpha", Range(0,1)) = 1

		[Header(Textures)][Space]
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Tex1("Tex1", 2D) = "white" {}
		_Tex2("Tex2", 2D) = "white" {}

		[Header(Mat Flags 0x10)][Space]
		[Toggle] _Flag0_0("Unk 0", Float) = 0
		[Toggle] _Flag0_1("Unk 1", Float) = 0
		[Toggle] _Flag0_2("Unk 2", Float) = 0
		[Toggle] _Flag0_3("Unk 3", Float) = 0
		[Toggle] _Flag0_4("Unk 4", Float) = 0
		[Toggle] _Flag0_5("Unk 5", Float) = 0
		[Toggle] _Flag0_6("Unk 6", Float) = 0
		[Toggle] _Flag0_7("Unk 7", Float) = 0
		
		[Header(Mat Flags 0x15)][Space]
		[Toggle] _Flag1_0("Unk 0", Float) = 0
		[Toggle] _Flag1_1("Unk 1", Float) = 0
		[Toggle] _Flag1_2("Unk 2", Float) = 0
		[Toggle] _Flag1_3("Unk 3", Float) = 0
		//[Toggle] _Flag1_4("Unk 4", Float) = 0
		[Toggle] _Flag1_5("Unk 5", Float) = 0
		[Toggle] _Flag1_6("Unk 6", Float) = 0
		//[Toggle] _Flag1_7("Unk 7", Float) = 0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
			};

			//half _Glossiness;
			//half _Metallic;
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				//o.Metallic = _Metallic;
				//o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
