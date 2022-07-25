shader_type spatial;
render_mode shadows_disabled, cull_disabled;

uniform vec3 Color = vec3(1.0, 1.0, 1.0);
uniform float Opacity = 1.0;
uniform sampler2D RadialMask;
uniform vec2 Position;
uniform float PrimarySubdivisions = 4.0;
uniform float PrimaryLineWidth : hint_range(0, 10) = 0.5;
uniform float SecondarySubdivisions = 8.0;
uniform float SecondaryLineWidth : hint_range(0, 10) = 0.5;

float saturate(float val)
{
	return clamp(val, 0.0, 1.0);
}

void vertex()
{
	UV = VERTEX.xz;
}

void fragment()
{
	EMISSION = Color.rgb;
	
	vec2 subdiv = vec2(SecondarySubdivisions, SecondarySubdivisions);
	float lineWidth1 = 1.0 + SecondaryLineWidth * (0.9 - 1.0) / 1.0;
	vec2 gridVec1 = vec2(lineWidth1, lineWidth1);
	vec2 temp1 = (abs(fract(UV * subdiv + vec2(0, 0)) * 2.0 + -1.0) - gridVec1);
	vec2 grid_01 = (1.0 - (temp1 / fwidth(temp1)));

	vec2 subdiv2 = vec2(PrimarySubdivisions, PrimarySubdivisions);
	float lineWidth2 = (1.0 + PrimaryLineWidth * (0.9 - 1.0) / 1.0);
	vec2 gridVec2 = vec2(lineWidth2, lineWidth2);
	vec2 temp2 = (abs(fract(UV * subdiv2 + vec2(0, 0)) * 2.0 + -1.0) - gridVec2);
	vec2 grid_02 = (1.0 - (temp2 / fwidth(temp2)));
	
	float textureAlpha = Opacity * texture(RadialMask, (Position + UV)).a;
	
	float gridAlpha = 1.0 + (
		saturate(min(grid_01.x, grid_01.y)) *
		saturate(min(grid_02.x, grid_02.y))
	) * -1.0;
	
	ALPHA = (textureAlpha * gridAlpha);
}
