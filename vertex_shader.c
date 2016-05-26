#version 450
/* Входные атрибуты для вершинного шейдера. Ключевое слово in */
layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexColor;

/*Выходная переменная. Ключевое слово out*/
out vec3 Color;

void main()
{
	Color = VertexColor;
	/*Встроенная выходная переменная*/
	gl_Position = vec4(VertexPosition, 1.0);
}
