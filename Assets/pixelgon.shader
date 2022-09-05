Shader "custom/pixelgon"
{
  Properties
  {
    _MainTex ("Main Texture", 2D) = ""
    _TileX ("Tile X", Float) = 1
    _TileY ("Tile Y", Float) = 1
    _OffsetX ("Offset X", Float) = 0
    _OffsetY ("Offset Y", Float) = 0
  }
  SubShader
  {
    Tags { "Queue" = "Geometry" }
    
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
      float _TileY;
      float _OffsetX;
      float _OffsetY;

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
        float4 t = tex2D(_MainTex, 
          float2(
            _OffsetX + (i.uv.x * _TileX), 
            _OffsetY + (i.uv.y * _TileY)
          )
        );
        clip(t.a - 0.5);
        return i.color * t;
      }
      ENDCG
    }
  }
}