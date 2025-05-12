Shader "Custom/World UV Test" {

    Properties{
    _Color("Main Color", Color) = (1,1,1,1)
    _MainTexWall2("Wall Side Texture (RGB)", 2D) = "surface" {}
    _MainTexWall("Wall Front Texture (RGB)", 2D) = "surface" {}
    _MainTexFlr2("Floor Texture", 2D) = "surface" {}
    _OffsetX("OffsetX", Range(-1, 1)) = 0
    _OffsetY("OffsetY", Range(-1, 1)) = 0
    _OffsetZ("OffsetZ", Range(-1, 1)) = 0
    _Scale("Texture Scale", Float) = 0.1
    }

        SubShader{

        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input {
        float3 worldNormal;
        float3 worldPos;
        };

        sampler2D _MainTexWall;
        sampler2D _MainTexWall2;
        sampler2D _MainTexFlr2;
        float4 _Color;
        float _Scale;
        float _OffsetX;
        float _OffsetY;
        float _OffsetZ;

        void surf(Input IN, inout SurfaceOutput o) {
        float2 UV;
        fixed4 c;
        float2 offsetUV = float2(_OffsetX, _OffsetY);
        float2 offsetWallUV = float2(_OffsetZ, _OffsetY);
       
        if (abs(IN.worldNormal.x) > 0.5) {
        UV = IN.worldPos.yz; // side
        c = tex2D(_MainTexWall2, (UV * _Scale) + offsetWallUV); // use WALLSIDE texture
        }
         else if (abs(IN.worldNormal.z) > 0.5) {
      UV = IN.worldPos.xy; // front
      c = tex2D(_MainTexWall, (UV * _Scale) + offsetUV); // use WALL texture
      }
       else {
    UV = IN.worldPos.xz; // top
    c = tex2D(_MainTexFlr2, (UV * _Scale) + offsetUV); // use FLR texture
    }

    o.Albedo = c.rgb * _Color;
    }

    ENDCG
    }

        Fallback "VertexLit"
}