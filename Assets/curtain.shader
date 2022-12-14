Shader "custom/curtain"
{
  Properties
  {
    _Color ("Color", Color) = (0,0,0,1)
  }
  SubShader
  {
    Tags { "Queue" = "Transparent" }

    Blend SrcAlpha OneMinusSrcAlpha
    ZWrite Off
    Cull Off
    
    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      struct appdata
      {
        float4 vertex : POSITION;
      };
      struct v2f
      {
        float4 vertex : SV_POSITION;
      };

      float4 _Color;

      v2f vert(appdata v)
      {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        return o;
      }
      float4 frag(v2f i) : SV_Target
      {
        return _Color;
      }
      ENDCG
    }
  }
}