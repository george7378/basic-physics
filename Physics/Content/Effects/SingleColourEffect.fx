#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif


////////////////////
//Global variables//
////////////////////
float4x4 World;
float4x4 WorldViewProjection;
float4x4 LightWorldViewProjection;

float LightPower;
float AmbientLightPower;
float SpecularExponent;
float ShadowMapSize;

float3 CameraPosition;
float3 LightDirection;
float3 BaseColour;

Texture ShadowMapTexture;


//////////////////
//Sampler states//
//////////////////
sampler ShadowMapTextureSampler = sampler_state
{
	texture = <ShadowMapTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Border; 
	AddressV = Border;
	BorderColor = 0xffffffff;
};


//////////////////
//I/O structures//
//////////////////
struct VertexShaderInput
{
	float4 Position	: POSITION;
	float3 Normal	: NORMAL;
};

struct VertexShaderOutput
{
	float4 Position               : POSITION;
	float3 Normal                 : TEXCOORD0;
	float4 PositionWorld          : TEXCOORD1;
	float4 ProjectedPositionLight : TEXCOORD2;
};


///////////
//Shaders//
///////////
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = mul(input.Position, WorldViewProjection);
	output.Normal = normalize(mul(input.Normal, (float3x3)World));
	output.PositionWorld = mul(input.Position, World);
	output.ProjectedPositionLight = mul(input.Position, LightWorldViewProjection);

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float diffuseLightingFactor = saturate(dot(-normalize(LightDirection), input.Normal))*LightPower;

	float3 reflectionVector = normalize(reflect(LightDirection, input.Normal));
	float3 eyeVector = normalize(CameraPosition - input.PositionWorld.xyz);
    float specularLightingFactor = pow(saturate(dot(reflectionVector, eyeVector)), SpecularExponent);

	float2 projectedTextureCoordinatesLight;
	projectedTextureCoordinatesLight.x = (input.ProjectedPositionLight.x/input.ProjectedPositionLight.w + 1)/2;
	projectedTextureCoordinatesLight.y = (-input.ProjectedPositionLight.y/input.ProjectedPositionLight.w + 1)/2;
    
	int shadowFactor = 0;
	for (int y = -1; y <= 1; y++)
	{
		for (int x = -1; x <= 1; x++)
		{
			float2 shadowSampleOffset = float2(x/ShadowMapSize, y/ShadowMapSize);
			if (tex2D(ShadowMapTextureSampler, projectedTextureCoordinatesLight + shadowSampleOffset).r < input.ProjectedPositionLight.z - 0.002f)
			{
				shadowFactor += 1;
			}
		}
	}
	float lightingCorrectionFactor = 1 - shadowFactor/9.0f;
	diffuseLightingFactor *= lightingCorrectionFactor;
	specularLightingFactor *= lightingCorrectionFactor;

	/*
	if (tex2D(ShadowMapTextureSampler, projectedTextureCoordinatesLight).r < input.ProjectedPositionLight.z - 0.002f)
    {
    	diffuseLightingFactor = 0;
    	specularLightingFactor = 0;
	}
	*/

	float4 finalColour = float4(BaseColour*(AmbientLightPower + diffuseLightingFactor) + float3(1, 1, 1)*specularLightingFactor, 1);

	return finalColour;
}

technique SingleColourTechnique
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}