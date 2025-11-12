Shader "Custom/JellyfishShader"
{
    Properties
    {
        _MainTex ("水母贴图", 2D) = "white" {}
        _Color ("颜色", Color) = (1,1,1,1)
        _RippleSpeed ("涟漪速度", Range(0.1, 10.0)) = 2.0
        _RippleAmount ("涟漪强度", Range(0.001, 0.1)) = 0.01
        _WaveIntensity ("波动强度", Range(0.0, 0.1)) = 0.02
        _TentacleSwaySpeed ("触须摇摆速度", Range(0.1, 10.0)) = 3.0
        _TentacleSwayAmount ("触须摇摆幅度", Range(0.01, 0.5)) = 0.1
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _RippleSpeed;
            float _RippleAmount;
            float _WaveIntensity;
            float _TentacleSwaySpeed;
            float _TentacleSwayAmount;

            v2f vert(appdata v)
            {
                v2f o;
                
                // 获取原始顶点位置
                float4 vertPos = v.vertex;
                
                // 水母上半部分（头部）的涟漪效果
                if (v.uv.y > 0.5) {
                    float wave = sin(_Time.y * _RippleSpeed + v.vertex.x * 10) * _RippleAmount;
                    wave += cos(_Time.y * _RippleSpeed * 0.7 + v.vertex.z * 10) * _RippleAmount;
                    
                    // 上部的波动更强
                    float domeEffect = (v.uv.y - 0.5) * 2.0; // 从0到1映射
                    vertPos.y += wave * domeEffect;
                    
                    // 顶部向内收缩的脉动效果
                    float pulse = sin(_Time.y * 0.8) * 0.5 + 0.5; // 0到1之间
                    vertPos.xz *= 1.0 + pulse * _WaveIntensity * domeEffect;
                }
                
                // 水母下半部分（触须）的摇摆效果
                if (v.uv.y < 0.5) {
                    // 触须左右摇摆
                    float tentacleSwing = sin(_Time.y * _TentacleSwaySpeed + v.uv.y * 10) * _TentacleSwayAmount;
                    
                    // 摇摆幅度随着向下距离增加
                    float swayMultiplier = (0.5 - v.uv.y) * 2.0; // 从0到1映射
                    vertPos.x += tentacleSwing * swayMultiplier;
                    
                    // 触须上下飘动
                    float tentacleFloat = cos(_Time.y * _TentacleSwaySpeed * 0.6 + v.uv.y * 8) * _TentacleSwayAmount * 0.7;
                    vertPos.y += tentacleFloat * swayMultiplier;
                }
                
                o.vertex = UnityObjectToClipPos(vertPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 采样纹理并应用颜色
                fixed4 col = tex2D(_MainTex, i.uv) * i.color * _Color;
                
                // 添加轻微发光效果
                if (i.uv.y > 0.5) {
                    float pulse = sin(_Time.y * 1.2) * 0.2 + 0.8;
                    col.rgb *= pulse;
                }
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
} 