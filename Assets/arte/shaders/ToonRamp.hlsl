void ToonShading_float(in float3 Normal, in float ToonRampSmoothness, in float3 ClipSpacePos, in float3 WorldPos, in float4 ToonRampTinting,
in float ToonRampOffset, out float3 ToonRampOutput, out float3 Direction)
{

	// preview de shader
	#ifdef SHADERGRAPH_PREVIEW
		ToonRampOutput = float3(0.5,0.5,0);
		Direction = float3(0.5,0.5,0);
	#else

		// sombras
		#if SHADOWS_SCREEN
			half4 shadowCoord = ComputeScreenPos(ClipSpacePos);
		#else
			half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
		#endif 

		// luz ppal
		#if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
			Light light = GetMainLight(shadowCoord);
		#else
			Light light = GetMainLight();
		#endif

		//luz adicional
		#ifdef _ADDITIONAL_LIGHTS
			uint numAdditionalLights = GetAdditionalLightsCount();
			for (uint lightI = 0; lightI < numAdditionalLights; lightI++)
			{
				Light addLight = GetAdditionalLight(lightI, WorldPos, 0);
				float3 lightIColor = addLight.color * (addLight.distanceAttenuation * addLight.shadowAttenuation);

				light.color += lightIColor;
			}
		#endif

		// fall off
		half d = dot(Normal, light.direction) * 0.5 + 0.5;
		
		// step!! controla suavidad de el sombreado
		half toonRamp = smoothstep(ToonRampOffset, ToonRampOffset+ ToonRampSmoothness, d );
		// sombras de luz ppal
		toonRamp *= light.shadowAttenuation;
		// tinte de luz
		ToonRampOutput = light.color * (toonRamp + ToonRampTinting) ;
		// luz de contorno
		Direction = light.direction;
	#endif

}