Shader "Sprites/Diffuse Outline Shadows" 
  {
       Properties 
       {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineOffSet ("Outline OffSet", Float) = 1
        _Color2("Outline Color", Color) = (0,0,0,1)
        _Color3 ("Tint Inner", Color) = (1,1,1,1)
        _Color ("Tint Overall", Color) = (1,1,1,1)

		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

       }
       SubShader
       {
       	Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		LOD 300 //only for Bumped

           ZWrite Off
           Blend One OneMinusSrcAlpha 
           Cull Off 
           Lighting On 
    
           CGPROGRAM
           #pragma surface surf Lambert alpha nofog
		   #pragma target 3.0
    	#pragma surface surf Lambert alpha vertex:vert  alphatest:_Cutoff fullforwardshadows

           struct Input 
           {
               float2 uv_MainTex;
               float4 _MainTex_TexelSize ;
               fixed4 color : COLOR;
           };
    
          sampler2D _MainTex;
          float _OutlineOffSet;
          float4 _Color2;
       	  float4 _Color3;
       	  float4 _Color;

       	  uniform float4 _MainTex_TexelSize;
		         	  
			// Handles Shadows
		  	void vert(inout appdata_full v, out Input o)
			{
				#if defined(PIXELSNAP_ON) && !defined(SHADER_API_FLASH)
				v.vertex = UnityPixelSnap(v.vertex);
				#endif
				v.normal = float3(0,0,-1);
				v.tangent = float4(1, 0, 0, 1);
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.color = _Color;
			}

		// Outline
           void surf(Input IN, inout SurfaceOutput o)
           {
               fixed4 TempColor = tex2D(_MainTex, IN.uv_MainTex+float2(-_MainTex_TexelSize.x * _OutlineOffSet ,0.0)) + tex2D(_MainTex, IN.uv_MainTex-float2(-_MainTex_TexelSize.x * _OutlineOffSet ,0.0));
               TempColor = TempColor + tex2D(_MainTex, IN.uv_MainTex + float2(0.0,-_MainTex_TexelSize.y * _OutlineOffSet )) + tex2D(_MainTex, IN.uv_MainTex - float2(0.0,-_MainTex_TexelSize.y * _OutlineOffSet ));
               if(TempColor.a > 0.1){
                   TempColor.a = 1;
               }
               fixed4 AlphaColor = fixed4(TempColor.r,TempColor.g,TempColor.b,TempColor.a);
               fixed4 mainColor = AlphaColor  * _Color2.rgba;
               fixed4 addcolor = tex2D(_MainTex, IN.uv_MainTex) * IN.color * _Color3.rgba;
    
               if(addcolor.a > 0){
                   mainColor = addcolor;
               }

               o.Albedo = mainColor.rgb * _Color.rgb;
               o.Alpha = mainColor.a * _Color.a;

           }
           ENDCG       
       }       
     Fallback "Transparent/Cutout/Diffuse"
   }
