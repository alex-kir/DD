Shader "DDShader"
{
    Properties
    {
	    _MainTex ("Texture", 2D) = "white" {}
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
                    return lerp(float4(input.uv2.xyz, 0), input.color, tex2D(_MainTex, input.uv));
			    }
        
			    ENDCG
		    }
	    }

        
        SubShader
        {
		    Pass
            {
            	SetTexture [_MainTex]
                {
                    //constantColor (1, 0.3, 0.3, 1)
				    //combine texture * constant
                    combine texture * primary
                }
			}
        }

    } // Category

} // Shader
