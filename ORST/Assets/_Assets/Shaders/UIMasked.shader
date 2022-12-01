
Shader "UIMasked"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("PixelSnap", Float) = 0

     	_StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 1
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
 
        _ColorMask ("Color Mask", Float) = 15
 
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

 
	}
 
	SubShader {
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" "PreviewType"="Plane"}
		LOD 200
		
        Cull Off
        Lighting Off
        ZWrite Off
		
        //ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]



         Stencil
        {
		    Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]

		}
			ColorMask [_ColorMask]



			Pass
	{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;

			struct appdata
			{
			    float4 vertex : POSITION;
			    float2 uv : TEXCOORD0;

			    UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
			};


			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _MainTex_ST;

			v2f vert(appdata_base v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v); //Insert
    			UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
    			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 texcol = tex2D(_MainTex, i.uv);
				
				return texcol * _Color;
			}
			ENDCG
			}
	}

}