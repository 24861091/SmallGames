#ifndef SKELETON_TINT_COMMON_INCLUDED
#define SKELETON_TINT_COMMON_INCLUDED

float4 fragTintedColor(float4 texColor, float3 darkTintColor, float4 lightTintColorPMA, float lightColorAlpha, float darkColorAlpha) {

	float a = texColor.a * lightTintColorPMA.a;

#if !defined(_STRAIGHT_ALPHA_INPUT)
	float3 texDarkColor = (texColor.a - texColor.rgb);
#else
	float3 texDarkColor = (1 - texColor.rgb);
#endif
	float3 darkColor = texDarkColor * darkTintColor.rgb * lightColorAlpha;
	float3 lightColor = texColor.rgb * lightTintColorPMA.rgb;

	float4 fragColor = float4(darkColor + lightColor, a);
	fragColor.rgb *= texColor.a;

#if defined(_DARK_COLOR_ALPHA_ADDITIVE)
	fragColor.a = a * (1 - darkColorAlpha);
#endif
	return fragColor;
}

#endif
