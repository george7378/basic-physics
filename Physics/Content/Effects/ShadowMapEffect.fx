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
float4x4 LightWorldViewProjection;


//////////////////
//I/O structures//
//////////////////
struct VertexShaderInput
{
	float4 Position : POSITION;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float Depth     : TEXCOORD0;
};


///////////
//Shaders//
///////////
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = mul(input.Position, LightWorldViewProjection);
	output.Depth = output.Position.z;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return float4(input.Depth, input.Depth, input.Depth, 1);
}

technique ShadowMapTechnique
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};