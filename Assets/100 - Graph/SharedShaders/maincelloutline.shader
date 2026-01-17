Shader "URP/CelShadingWithOutline"
{
    Properties
    {
        [Header(Base)]
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        
        [Header(Cel Shading)]
        _RampTex("Ramp Texture", 2D) = "white" {}
        _ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowSmooth("Shadow Smoothness", Range(0, 1)) = 0.05
        
        [Header(Outline)]
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0, 0.1)) = 0.01
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        // Main Pass - Cel Shading
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_RampTex);
            SAMPLER(sampler_RampTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _ShadowThreshold;
                float _ShadowSmooth;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = posInputs.positionCS;
                OUT.positionWS = posInputs.positionWS;
                OUT.normalWS = normInputs.normalWS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Sample textures
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                
                // Lighting
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(IN.positionWS));
                
                // Calculate NdotL
                float3 normalWS = normalize(IN.normalWS);
                float NdotL = dot(normalWS, mainLight.direction);
                
                // Cel shading with ramp
                float lightIntensity = smoothstep(_ShadowThreshold - _ShadowSmooth, 
                                                   _ShadowThreshold + _ShadowSmooth, 
                                                   NdotL);
                
                // Sample ramp texture
                float2 rampUV = float2(lightIntensity, 0.5);
                half3 ramp = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, rampUV).rgb;
                
                // Apply shadows
                float shadow = mainLight.shadowAttenuation;
                lightIntensity *= shadow;
                
                // Final color
                half3 lighting = mainLight.color * ramp * lightIntensity;
                half3 color = albedo.rgb * _Color.rgb * lighting;
                
                // Add ambient
                color += albedo.rgb * _Color.rgb * 0.2;
                
                return half4(color, albedo.a);
            }
            ENDHLSL
        }

        // Outline Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // Extrude vertices along normal
                float3 positionOS = IN.positionOS.xyz;
                float3 normalOS = normalize(IN.normalOS);
                positionOS += normalOS * _OutlineWidth;
                
                VertexPositionInputs posInputs = GetVertexPositionInputs(positionOS);
                OUT.positionCS = posInputs.positionCS;
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float3 _LightDirection;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                
                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif
                
                OUT.positionCS = positionCS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}