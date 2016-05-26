#version 450
uniform vec3 cam_pos;
/* Входные атрибуты для вершинного шейдера. Ключевое слово in */
in vec3 VertexPosition;

/*Выходная переменная. Ключевое слово out*/
out vec3 dir, org;

void main()
{
	/*Встроенная выходная переменная*/
	gl_Position = vec4(VertexPosition, 1.0);
	dir = normalize(vec3(VertexPosition.x * 1.66667, VertexPosition.y, -2.0));
	org = cam_pos;
}
