Shader "Max820/DecalBlended" {
Properties {
	_MainColor ("Main Color", Color) = (.2,.2,.2,0)
	_MainTex ("Main Texture", 2D) = "white" {}
}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	Tags { "LightMode" = "Vertex" }
	Cull Off
	Lighting On
	Material { Emission [_MainColor ] }
	ColorMaterial AmbientAndDiffuse
	ZWrite Off
	ColorMask RGB
	Blend SrcAlpha OneMinusSrcAlpha
	Pass {
		SetTexture [_MainTex] { combine primary * texture DOUBLE}
	}
}
}