Shader "Volumetric Audio/Water"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_Colour("Colour", Color) = (0, 0, 1, 1)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				
				sampler2D _Texture;
				
				float4 _Colour;
				
				struct a2v
				{
					float4 vertex   : POSITION;
					float2 texcoord : TEXCOORD0;
				};
				
				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD2;
				};
				
				struct f2g
				{
					half4 col : COLOR;
				};
				
				void Vert(a2v i, out v2f o)
				{
					o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
					o.uv0 = i.texcoord;
				}
				
				void Frag(v2f i, out f2g o)
				{
					o.col = tex2D(_Texture, i.uv0);
					o.col = tex2D(_Texture, -i.uv0 + o.col.yy + _Time.x);
					
					o.col *= _Colour;
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader