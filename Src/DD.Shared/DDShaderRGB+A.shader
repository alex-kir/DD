Shader "DDShaderRGB+A"
{
    Properties
    {
	    _MainTex ("Texture", 2D) = "white" {}
	    _Alpha ("Alpha", 2D) = "white" {}
    }

    Category
    {
	    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	    Blend SrcAlpha OneMinusSrcAlpha
	    Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	    BindChannels {
	    	Bind "vertex", vertex
	    	Bind "Color", color
	    	Bind "TexCoord", texcoord
            Bind "TexCoord1", texcoord1
	    }
	    
	    SubShader
        {
		    Pass
            {
                CGPROGRAM
                
                #pragma target 2.0

                #pragma vertex vertex_func
			    #pragma fragment fragment_color_func
        
                uniform sampler2D _MainTex;
                uniform sampler2D _Alpha;

                struct data_context
                {
                    float4 position : POSITION;
                    half2 uv : TEXCOORD0;
                    float3 uv2 : TEXCOORD1;
                    fixed4 color : COLOR0;
                };

			    data_context vertex_func(data_context input)
                {
                    input.position = mul(UNITY_MATRIX_MVP, input.position);
                    return input;
			    }
                
			    fixed4 fragment_color_func(data_context input) : COLOR
                {
                	fixed3 tex = lerp(float3(input.uv2.xyz), input.color.rgb, tex2D(_MainTex, input.uv).rgb);
                    fixed alpha = tex2D(_Alpha, input.uv).a;
            		return fixed4(tex, alpha);
			    }
        
			    ENDCG
		    }
	    }

        
        SubShader
        {
		    Pass
            {
                SetTexture [_Alpha] { combine primary, texture * primary }
				SetTexture [_MainTex] { combine texture * previous }
			}
        }

    } // Category

} // Shader
