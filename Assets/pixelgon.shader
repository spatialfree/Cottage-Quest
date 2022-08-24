// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// basic unlit vertex frag unity shader
Shader "custom/pixelgon"
{
  Properties
  {
    _MainTex ("Main Texture", 2D) = ""
    _TileX ("Tile X", Float) = 1
  }
  SubShader
  {
    Tags { "Queue" = "Geometry" }

    // Cull Off
    // ZTest Always
    // ZWrite Off
    // Blend SrcAlpha
    // ColorMask RGB
    
    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      struct appdata
      {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float2 uv : TEXCOORD0;
      };
      struct v2f
      {
        float4 vertex : SV_POSITION;
        float4 color : COLOR;
        float2 uv : TEXCOORD0;
      };

      sampler2D _MainTex;
      float _TileX;

      v2f vert(appdata v)
      {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.color = v.color;
        o.uv = v.uv;
        return o;
      }
      float4 frag(v2f i) : SV_Target
      {
        return i.color * tex2D(_MainTex, float2(i.uv.x * _TileX, i.uv.y));
      }
      ENDCG
    }
  }
}