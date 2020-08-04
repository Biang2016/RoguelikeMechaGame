//BLURS-------------------------------------------------------------------------
fixed4 Blur(fixed2 uv, sampler2D source, fixed Intensity)
{
	fixed step = 0.00390625f * Intensity;
	fixed4 result = fixed4 (0, 0, 0, 0);
	fixed2 texCoord = fixed2(0, 0);
	texCoord = uv + fixed2(-step, -step);
	result += tex2D(source, texCoord);
	texCoord = uv + fixed2(-step, 0);
	result += 2.0 * tex2D(source, texCoord);
	texCoord = uv + fixed2(-step, step);
	result += tex2D(source, texCoord);
	texCoord = uv + fixed2(0, -step);
	result += 2.0 * tex2D(source, texCoord);
	texCoord = uv;
	result += 4.0 * tex2D(source, texCoord);
	texCoord = uv + fixed2(0, step);
	result += 2.0 * tex2D(source, texCoord);
	texCoord = uv + fixed2(step, -step);
	result += tex2D(source, texCoord);
	texCoord = uv + fixed2(step, 0);
	result += 2.0* tex2D(source, texCoord);
	texCoord = uv + fixed2(step, -step);
	result += tex2D(source, texCoord);
	result = result * 0.0625;
	return result;
}
fixed BlurHD_G(fixed bhqp, fixed x)
{
	return exp(-(x * x) / (2.0 * bhqp * bhqp));
}
fixed4 BlurHD(fixed2 uv, sampler2D source, fixed Intensity)
{
	int iterations = 16;
	int halfIterations = iterations / 2;
	half sigmaX = 0.1 + Intensity * 0.5;
	half sigmaY = sigmaX;
	half total = 0.0;
	half4 ret = fixed4(0, 0, 0, 0);
	for (int iy = 0; iy < iterations; ++iy)
	{
		half fy = BlurHD_G(sigmaY, half(iy) -half(halfIterations));
		half offsety = half(iy - halfIterations) * 0.00390625;
		for (int ix = 0; ix < iterations; ++ix)
		{
			half fx = BlurHD_G(sigmaX, fixed(ix) - half(halfIterations));
			half offsetx = half(ix - halfIterations) * 0.00390625;
			total += fx * fy;
			ret += tex2D(source, uv + half2(offsetx, offsety)) * fx * fy;
		}
	}
	return ret / total;
}
//-----------------------------------------------------------------------
fixed rand(fixed2 seed) {
	return frac(sin(dot(seed, fixed2(12.9898, 78.233))) * 43758.5453);
}

fixed rand2(fixed2 seed) {
	return frac(sin(dot(seed * floor(50 + (_Time + 0.1) * 12.), fixed2(127.1, 311.7))) * 43758.5453123);
}