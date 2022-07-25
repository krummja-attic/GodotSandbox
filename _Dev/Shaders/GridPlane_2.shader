shader_type canvas_item;
render_mode unshaded;

uniform vec4 in_position = vec4(0.5);
uniform vec4 lineColor : hint_color = vec4(vec3(1.0), 1.0);

void vertex () 
{
	// in 	 mat4 WORLD_MATRIX;   		<-- model-view matrix
	// in 	 mat4 EXTRA_MATRIX;
	// in 	 mat4 PROJECTION_MATRIX;
	// in 	 vec4 INSTANCE_CUSTOM;
		
	// inout vec2 UV;
	// inout vec2 VERTEX;
	// inout vec4 COLOR;
	
	// override VERTEX with:
	//   VERTEX = (EXTRA_MATRIX * (WORLD_MATRIX * vec4(VERTEX, 0.0, 1.0))).xy;
	
	// vec4 in_position
	// VERTEX = (PROJECTION_MATRIX * WORLD_MATRIX * in_position).xy;
	
	// GLSL gl_Position is roughly equivalent to VERTEX
}

void fragment ()
{
	// in	 vec2 UV
	// in 	 vec4 FRAGCOORD
	// in	 vec2 SCREEN_PIXEL_SIZE		<-- equal to inverse of resolution!
	// in	 vec2 POINT_COORD			<-- coordinate for drawing points
	
	// inout vec3 NORMAL
	// inout vec4 COLOR
	
	//   out vec3 NORMALMAP
	
	if (fract(UV.x / 0.001f) < 0.01f || fract(UV.y / 0.001f) < 0.01f) 
		COLOR = lineColor;
	else
		COLOR = vec4(0);
}
