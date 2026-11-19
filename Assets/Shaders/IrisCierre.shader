Shader "UI/IrisCierre"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _Radio ("Radio", Float) = 1.2
        _Suavizado ("Suavizado", Float) = 0.01
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; };

            fixed4 _Color;
            float _Radio;
            float _Suavizado;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.pos.xy / _ScreenParams.xy;
                float aspecto = _ScreenParams.x / _ScreenParams.y;
                float2 p = (uv - 0.5) * float2(aspecto, 1);
                float d = length(p);
                float a = smoothstep(_Radio - _Suavizado, _Radio, d);
                return fixed4(_Color.rgb, a * _Color.a);
            }
            ENDCG
        }
    }
}