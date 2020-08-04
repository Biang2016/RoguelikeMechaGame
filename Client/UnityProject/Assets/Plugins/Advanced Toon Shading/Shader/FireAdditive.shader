// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Max820/FireAdditive" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Distort01 ("Distort01", 2D) = "white" {}
        _Distort02 ("Distort02", 2D) = "white" {}
        _DistortTillingSize ("Distort Tilling Size", Float ) = 0.05
        _Speed ("Speed", Float ) = 1
        _Intensity ("Intensity", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _Speed;
            uniform sampler2D _Distort01; uniform float4 _Distort01_ST;
            uniform sampler2D _Distort02; uniform float4 _Distort02_ST;
            uniform float _DistortTillingSize;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Intensity;
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float4 Time = _Time + _TimeEditor;
                float TimeSpeed = (_Speed*Time.g);
                float2 Panner01 = (i.screenPos.rg+TimeSpeed*float2(0.1,-0.5));
                float4 _Distort01_var = tex2D(_Distort01,TRANSFORM_TEX(Panner01, _Distort01));
                float2 Panner02 = (i.screenPos.rg+TimeSpeed*float2(-0.1,-0.5));
                float4 _Distort02_var = tex2D(_Distort02,TRANSFORM_TEX(Panner02, _Distort02));
                float2 Add = (((_Distort01_var.rgb*_Distort02_var.rgb).rg*_DistortTillingSize)+i.uv0);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(Add, _MainTex));
                clip(((i.vertexColor.a*_MainTex_var.r)*(1.0 - length((i.uv0*2.0+-1.0)))*_Intensity*_Color.a) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = (_MainTex_var.rgb*i.vertexColor.rgb*_Intensity*_Color.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }

}
