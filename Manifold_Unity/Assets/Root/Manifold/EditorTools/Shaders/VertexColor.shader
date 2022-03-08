// Source: https://emilesonneveld.be/fast-vertex-color-shader-in-unity/
Shader "Unlit/VertexColor"
{
    Properties
    {
    _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
    Tags { "RenderType" = "Opaque" }
    LOD 100

    Pass
    {
    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    // make fog work
    #pragma multi_compile_fog

    #include "UnityCG.cginc"

    struct appdata
    {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float4 col:COLOR;
    };

    struct v2f
    {
    float2 uv : TEXCOORD0;
    UNITY_FOG_COORDS(1)
    float4 vertex : SV_POSITION;
    float4 col:COLOR;
    float4 screenPos : TEXCOORD1;
    };

    sampler2D _MainTex;
    float4 _MainTex_ST;

    v2f vert(appdata v)
    {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.col = v.col;
    o.screenPos = ComputeScreenPos(o.vertex);
    UNITY_TRANSFER_FOG(o,o.vertex);
    return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        // sample the texture
        fixed4 col = i.col; //tex2D(_MainTex, i.uv);


        // Screen-door transparency: Discard pixel if below threshold. 
        float4x4 thresholdMatrix =
        { 1.0 / 17.0, 9.0 / 17.0, 3.0 / 17.0, 11.0 / 17.0,
        13.0 / 17.0, 5.0 / 17.0, 15.0 / 17.0, 7.0 / 17.0,
        4.0 / 17.0, 12.0 / 17.0, 2.0 / 17.0, 10.0 / 17.0,
        16.0 / 17.0, 8.0 / 17.0, 14.0 / 17.0, 6.0 / 17.0
        };
        float4x4 _RowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
        float2 pos = i.screenPos.xy / i.screenPos.w;
        pos *= _ScreenParams.xy; // pixel position 
        //pos.x += (fmod(pos.x, 6)<1)*1;
        //pos.x *= 1.1;
        clip(i.col.a - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);



        // apply fog
        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;
        }
        ENDCG
        }
    }
}