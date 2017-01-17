// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Volumetric Audio/Glow"
{
	Properties
	{
		_Colour("Colour", Color) = (0, 0, 1, 1)
		_Power("Power", Float) = 3
	}
	SubShader
	{
		Pass
		{
			Cull Off
			
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				
				sampler2D _Texture;
				
				float4 _Colour;
				
				float _Power;
				
				struct a2v
				{
					float4 vertex   : POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal   : NORMAL;
				};
				
				struct v2f
				{
					float4 pos  : SV_POSITION;
					float3 wNrm : TEXCOORD0;
					float3 wPos : TEXCOORD1;
				};
				
				struct f2g
				{
					half4 col : COLOR;
				};
				
				void Vert(a2v i, out v2f o)
				{
					float3 wPos = mul(unity_ObjectToWorld, i.vertex).xyz;
					
					o.pos  = mul(UNITY_MATRIX_MVP, i.vertex);
					o.wNrm = mul((float3x3)unity_ObjectToWorld, i.normal).xyz;
					o.wPos = mul(unity_ObjectToWorld, i.vertex).xyz;
				}
				
				void Frag(v2f i, out f2g o)
				{
					float3 a = normalize(i.wNrm);
					float3 b = normalize(i.wPos - _WorldSpaceCameraPos);
					float  d = pow(dot(a, b), _Power);
					
					o.col = _Colour * abs(d);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader