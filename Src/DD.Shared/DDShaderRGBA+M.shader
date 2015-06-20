Shader "DDShaderRGBA+M"
{
    Properties
    {
	    _MainTex ("Texture", 2D) = "white" {}
	    _Mask ("Mask", 2D) = "white" {}
	    _MaskFrom ("Mask from", Float) = 0 
	    _MaskTo ("Mask to", Float) = 1 
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
                uniform sampler2D _Mask;
                float4x4 _MaskMatrix;
                float _MaskFrom; 
	    		float _MaskTo;

                struct data_context
                {
                    float4 position : POSITION;
                    half2 uv : TEXCOORD0;
                    float3 uv2 : TEXCOORD1;
                    fixed4 color : COLOR0;
                    float2 mask_position;
                };

			    data_context vertex_func(data_context input)
                {
                    input.position = mul(UNITY_MATRIX_MVP, input.position);
                    float4 maskPos = mul(_MaskMatrix, input.position);
                    input.mask_position = float2(maskPos.x, maskPos.y);
                    return input;
			    }
                
			    fixed4 fragment_color_func(data_context input) : COLOR
                {
                	fixed3 tex = lerp(float3(input.uv2.xyz), input.color.rgb, tex2D(_MainTex, input.uv).rgb);
					fixed alpha = tex2D(_Mask, input.mask_position).a * tex2D(_MainTex, input.uv).a;
					alpha = lerp(_MaskFrom, _MaskTo, alpha);
            		return fixed4(tex, alpha);
			    }
        
			    ENDCG
		    }
	    }

        
        SubShader
        {
		    Pass
            {
           		SetTexture [_MainTex] { combine texture * primary }
			}
        }

    } // Category

} // Shader
