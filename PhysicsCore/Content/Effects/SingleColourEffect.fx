#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


////////////////////
//Global variables//
////////////////////
float4x4 World;
float4x4 WorldViewProjection;
float4x4 LightWorldViewProjection;

float LightPower;
float AmbientLightPower;
float ShadowMapSize;

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
	float4 Position	: POSITION0;
	float3 Normal   : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position               : POSITION0;
	float3 Normal                 : TEXCOORD0;
	float4 ProjectedPositionLight : TEXCOORD1;
};


///////////
//Shaders//
///////////
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = mul(input.Position, WorldViewProjection);
	output.Normal = normalize(mul(input.Normal, (float3x3)World));
	output.ProjectedPositionLight = mul(input.Position, LightWorldViewProjection);

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float diffuseLightingFactor = saturate(dot(-LightDirection, input.Normal))*LightPower;

	float2 projectedTextureCoordinatesLight;
	projectedTextureCoordinatesLight.x = (input.ProjectedPositionLight.x/input.ProjectedPositionLight.w + 1.0f)/2.0f;
	projectedTextureCoordinatesLight.y = (-input.ProjectedPositionLight.y/input.ProjectedPositionLight.w + 1.0f)/2.0f;
	
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
	diffuseLightingFactor *= 1.0f - shadowFactor/9.0f;

	float4 finalColour = float4(BaseColour*(AmbientLightPower + diffuseLightingFactor), 1.0f);

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