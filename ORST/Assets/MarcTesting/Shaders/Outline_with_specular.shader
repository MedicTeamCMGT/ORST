Shader "Outline with specular" 
{
	Properties 
	{
		_Color ("Mesh color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}

		_SpecTex ("Specular (Texture)", 2D) = "white" {}
        _SpecularColor("Specular Color", Color) = (0.2,0.2,0.2)

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0

		[Toggle] _enable("Outline enable", Float) = 1
		_outline_thickness ("Outline thickness", Float ) = 0.05
		_outline_color ("Outline color", Color) = (0,0,0,1)

	}
    	CGINCLUDE
	#include "UnityCG.cginc"
	 
	struct appdata 
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	 
	struct v2f 
	{
		float4 pos : POSITION;
		float4 color : COLOR;

	};

	 
	uniform float _Outline;
	float _enable;
	uniform float4 _OutlineColor;
	 
	v2f vert(appdata v) 
	{
		// just make a copy of incoming vertex data but scaled according to normal direction
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
	 
		float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);
	 
		o.pos.xy += offset * o.pos.z * _Outline;
		o.color = _OutlineColor;
		return o;
	}
	ENDCG

	SubShader 
	{
		Pass 
		{
			Name "Outline"
			Cull Front   
            
			CGPROGRAM
			#pragma vertex vertex_shader
			#pragma fragment pixel_shader
			#pragma target 3.0
			
			float _outline_thickness;
            //float _enable;
			float4 _outline_color;
            				
			float4 vertex_shader (float4 vertex:POSITION,float3 normal:NORMAL):SV_POSITION
			{
				return UnityObjectToClipPos(float4(vertex.xyz+normal*_outline_thickness,1));
			}
            
			float4 pixel_shader(float4 vertex:SV_POSITION):COLOR 
			{
				if (_enable==1)
				{
					return float4(_outline_color.rgb,0);
				}
				else
				{
					discard;
					return 0;
				}						
			}          
			ENDCG
		}
	
		Tags {"Queue" = "Transparent"}
		LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #pragma shader_feature_local _SPECTEX

        sampler2D _MainTex;
        sampler2D _SpecTex;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_SpecTex;

        };


        half _Glossiness;
		fixed4 _SpecularColor;
        //half _Spec;
        fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

			fixed4 m = tex2D (_SpecTex, IN.uv_SpecTex) * _SpecularColor;
            o.Specular = m.rgb;

            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
	}
}
