// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "UnlitShadows/UnlitShadowReceive" { 
     Properties { 
         _MainTex("Base (RGB)", 2D) = "white" {} 
		 _Color ("Tint", Color) = (1,1,1,1)
		 _Outline("Outline Thickness", Range(0.0, 1)) = 0.0
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		[MaterialToggle] _CastShadow ( "Cast Shadow", Float ) = 0
     } 
 
     SubShader {     
         Pass { 
             Tags { "LightMode" = "ForwardBase" } 
             CGPROGRAM 
             #pragma vertex vert 
             #pragma fragment frag 
             #include "UnityCG.cginc" 
             #pragma multi_compile_fwdbase 
             #include "AutoLight.cginc" 
             sampler2D _MainTex;
             float4 _MainTex_ST;
			 float _CastShadow;
			 fixed4 _Color;
			
             struct v2f { 
                 float4 pos : SV_POSITION; 
                 LIGHTING_COORDS(0,1) 
                 float2 uv : TEXCOORD2;
             }; 
 
             v2f vert(appdata_base v) { 
                 v2f o; 
                 o.pos = UnityObjectToClipPos(v.vertex); 
                 o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                 TRANSFER_VERTEX_TO_FRAGMENT(o); 
                 return o; 
             } 
 
             fixed4 frag(v2f i) : COLOR
             { 
                 float attenuation = lerp(1, LIGHT_ATTENUATION(i), _CastShadow );; 
                 return tex2D (_MainTex, i.uv) *attenuation*_Color; 
             } 
             ENDCG 
         } 
 
         Pass { 
             Blend One One
             Tags { "LightMode" = "ForwardAdd" } 
             CGPROGRAM 
             #pragma vertex vert 
             #pragma fragment frag 
             #include "UnityCG.cginc" 
             #pragma multi_compile_fwdadd_fullshadows 
             #include "AutoLight.cginc" 
             sampler2D _MainTex;
             float4 _MainTex_ST;
			 float _CastShadow;
 
             struct v2f { 
                 float4 pos : SV_POSITION; 
                 LIGHTING_COORDS(0,1) 
                 float2 uv : TEXCOORD2;
             }; 
 
             v2f vert(appdata_base v) { 
                 v2f o; 
                 o.pos = UnityObjectToClipPos(v.vertex); 
                 o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                 TRANSFER_VERTEX_TO_FRAGMENT(o); 
                 return o; 
             } 
 
             fixed4 frag(v2f i) : COLOR
             { 
                float attenuation = lerp(1, LIGHT_ATTENUATION(i), _CastShadow );; 
                 return tex2D (_MainTex, i.uv) * attenuation; 
             } 
             ENDCG 
         } 

		 Pass{
			Name "OUTLINE"
 
			Cull Front
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" 
			half _Outline;
			half4 _OutlineColor;

		   struct v2f {
				half4 pos : POSITION;
				half2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				half3 norm = mul((half3x3)UNITY_MATRIX_IT_MV, v.normal);
				half2 offset = TransformViewToProjection(norm.xy);
				o.pos.xy += offset * o.pos.z * _Outline;
				o.color = _OutlineColor;
				return o;
			}
 
			fixed4 frag(v2f i) : COLOR
			{
				fixed4 o;
				o = i.color;
				return o;
			}
			ENDCG
		}
     } 
     Fallback "VertexLit" 
 }