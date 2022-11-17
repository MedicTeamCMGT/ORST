Shader "Outline" 
{
	Properties 
	{
		_Color ("Mesh color", Color) = (0,0,1,1)
		[Toggle] _enable("Outline enable", Float) = 1
		_outline_thickness ("Outline thickness", Float ) = 0.05
		_outline_color ("Outline color", Color) = (0,0,0,1)
        _MainTex ("Base (RGB)", 2D) = "white" { }

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0


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
		Tags {"Queue" = "Transparent"}
		Pass 
		{
			Name "Outline"
			//Tags { "LightMode" = "Always" }
			Cull Off
			ZWrite Off
			ZTest Always
			ColorMask RGB

            
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
				Pass 
		{
			Name "BASE"
			ZWrite On
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha

			Material 
			{
				Diffuse [_Color]
				Ambient [_Color]
			}

			Lighting On

			SetTexture [_MainTex] 
			{
				ConstantColor [_Color]
				Combine texture * constant
			}

			SetTexture [_MainTex] 
			{
				Combine previous * primary DOUBLE
			}
		}
	}

	SubShader 
	{
		Tags { "Queue" = "Transparent" }
 
		Pass 
		{
			Name "OUTLINE"
			//Tags { "LightMode" = "Always" }
			Cull Front
			ZWrite Off
			ZTest Always
			ColorMask RGB
 
			// you can choose what kind of blending mode you want for the outline
			//Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			//Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
 
			CGPROGRAM
			#pragma vertex vert
			#pragma exclude_renderers gles xbox360 ps3
			ENDCG
			SetTexture [_MainTex] { combine primary }
		}
 
		Pass 
		{
			Name "BASE"
			ZWrite On
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			Material 
			{
				Diffuse [_Color]
				Ambient [_Color]
			}
			Lighting On
			SetTexture [_MainTex] 
			{
				ConstantColor [_Color]
				Combine texture * constant
			}

			SetTexture [_MainTex] 
			{
				Combine previous * primary DOUBLE
			}
		}
	
	
	}
        FallBack "Diffuse"
}