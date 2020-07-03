// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Max820/Glow" {

Properties {
	_MainTex ("Base Texture", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
	_Multiplier("Color Multiplier", float) = 1
	_Bias("Bias",float) = 0
	_TimeOnDuration("ON Duration",float) = 0.5
	_TimeOffDuration("OFF Duration",float) = 0.5
	_BlinkingTimeOffsScale("Blinking Time",float) = 5
	_NoiseAmount("Noise Amount", Range(0,1)) = 0

}
	
SubShader {
		
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	Blend One One
//	Blend One OneMinusSrcColor
	Cull Off 
	Lighting Off 
	ZWrite Off 
	Fog { Color (0,0,0,0) }
	
	LOD 100
	
	CGINCLUDE	
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _Color;
	float _Multiplier;
	float _Bias;
	float _TimeOnDuration;
	float _TimeOffDuration;
	float _BlinkingTimeOffsScale;
	float _NoiseAmount;

	
	
	struct v2f {
		float4	pos	: SV_POSITION;
		float2	uv		: TEXCOORD0;
		fixed4	color	: TEXCOORD1;
	};

	
	v2f vert (appdata_full v)
	{
		v2f 		o;
		
		float		time 			= _Time.y + _BlinkingTimeOffsScale * v.color.b;		
		float		fracTime	= fmod(time,_TimeOnDuration + _TimeOffDuration);
		float		wave			= smoothstep(0,_TimeOnDuration * 0.25,fracTime)  * (1 - smoothstep(_TimeOnDuration * 0.75,_TimeOnDuration,fracTime));
		float		noiseTime	= time *  (6.2831853f / _TimeOnDuration);
		float		noise			= sin(noiseTime) * (0.5f * cos(noiseTime * 0.6366f + 56.7272f) + 0.5f);
		float		noiseWave	= _NoiseAmount * noise + (1 - _NoiseAmount);
			
		wave = _NoiseAmount < 0.01f ? wave : noiseWave;
		
		wave += _Bias;

		float4	mdlPos = v.vertex;
				
		o.uv		= v.texcoord.xy;
		o.pos	= UnityObjectToClipPos(mdlPos);
		o.color	=  _Color * _Multiplier * wave; 
						
		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{		
			return tex2D (_MainTex, i.uv.xy) * i.color;
		}
		ENDCG 
	}	
}


}