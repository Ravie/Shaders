in vec3 Color;
/*Ѕудет содержать значение, полученное в результате интерпол¤ции цветов трех вершин треугольника.*/
out vec4 FragColor;

void main()
{
	FragColor = vec4(Color, 1.0);
}
