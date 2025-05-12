Shader "Custom/ScrollShader" {
    Properties{
    _Color("Color", Color) = (1,1,1,1)
    _MainTex("Texture", 2D) = "white" {}
    _SubTex("Texture", 2D) = "white" {}
    _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower("Rim Power", Range(0.1,2.0)) = 3.0
    _ScrollXSpeed("X", Range(0,10)) = 2
    _ScrollYSpeed("Y", Range(0,10)) = 3

    }
        SubShader{
        Tags{ "RenderType" = "Opaque" }
        CGPROGRAM
#pragma surface surf Lambert
    struct Input {
        float2 uv_MainTex;
        float2 uv_SubTex;
        float3 viewDir;
        
    };
    sampler2D _MainTex;
    sampler2D _SubTex;
    float4 _RimColor;
    float _RimPower;
    fixed4 _Color;
    fixed _ScrollXSpeed;
    fixed _ScrollYSpeed;

    void surf(Input IN, inout SurfaceOutput o) {
        fixed2 scrolledUV = IN.uv_MainTex;

        fixed xScrollValue = _ScrollXSpeed * _Time;
        fixed yScrollValue = _ScrollYSpeed * _Time;

        scrolledUV += fixed2(xScrollValue, yScrollValue);
        half4 c = tex2D(_MainTex, scrolledUV);

        o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        o.Albedo += c.rbg;

        half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
        o.Emission = _RimColor.rgb * pow(rim, _RimPower);
    }
    ENDCG
    }
        Fallback "Cutout"
}