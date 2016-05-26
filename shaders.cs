using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

namespace Shaders
{
    class shader : GameWindow
    {
        int BasicProgramID;
        int BasicVertexShader;              // Address vs
        int BasicFragmentShader;            // Address fs

        int vaoHandle;                      // Vertex Array Object
        int[] vboHandlers = new int[2];     // Vertex Buffer Objects
        Vector3 camera_position;    // Позиция камеры
        int uniform_pos;
        int attribute_vpos;         // Адрес параметра позиции
        int attribute_vcol;         // Адрес параметра цвета

        float latitude = 0.0f;      // Углы
        float longitude = 0.0f;     //  наклона
        int mouseX = 0;             // Положения
        int mouseY = 0;             //  мышки

        float[] positionData = { -1f, -1f, 0f, 1f, -1f, 0f, 1f, 1f, 0f, -1f, 1f, 0f };
        float[] colorData = { 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f };

        void loadShader(String filename, ShaderType type, int program, out int address)
        {
            string glVersion = GL.GetString(StringName.Version);
            string glslVersion = GL.GetString(StringName.ShadingLanguageVersion);
            address = GL.CreateShader(type);
            if (address == 0)
            {
                string message = String.Format("Can't create shader");
                throw new ArgumentOutOfRangeException(message);
            }
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        private void InitShaders()
        {
            // создание объекта программы 
            BasicProgramID = GL.CreateProgram();
            loadShader("..\\..\\vertex_shader.c", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
            loadShader("..\\..\\fragment_shader.c", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
            //Компоновка программы
            GL.LinkProgram(BasicProgramID);

            // Проверить успех компоновки
            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);

            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));

            uniform_pos = GL.GetUniformLocation(BasicProgramID, "cam_pos");
            attribute_vpos = GL.GetAttribLocation(BasicProgramID, "VertexPosition");
            attribute_vcol = GL.GetAttribLocation(BasicProgramID, "VertexColor");

            GL.GenBuffers(2, vboHandlers);

            vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(vaoHandle);
        }

        protected override void OnLoad(EventArgs e)   //Вызывает событие Load
        {
            base.OnLoad(e);

            InitShaders();

            camera_position = new Vector3(1, 1, 3);
        }

        protected override void OnRenderFrame(FrameEventArgs e)   //Отвечает за перерисовку
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);  //Очищать буфер
            GL.Enable(EnableCap.DepthTest);    //Дальние элементы перекрываются ближними

            camera_position = new Vector3((float)Math.Sin(latitude / 180 * Math.PI), (float)Math.Tan(longitude / 180 * Math.PI),
                (float)Math.Cos(longitude / 180 * Math.PI));

            GL.EnableVertexAttribArray(attribute_vpos);   //Активация атрибутов вершин
            GL.EnableVertexAttribArray(attribute_vcol);   //Или вкл. режима отрисовки

            GL.DrawArrays(PrimitiveType.Quads, 0, 4);  //Отображает нашу фигуру

            GL.DisableVertexAttribArray(attribute_vpos);  //Выключаем режим отрисовки
            GL.DisableVertexAttribArray(attribute_vcol);

            SwapBuffers();   //Быстро скопировать содержимое заднего буфера окна в передний буфер
        }

        protected override void OnUpdateFrame(FrameEventArgs e)   //Отвечает за обновление Update
        {
            base.OnUpdateFrame(e);
            // Подсчет углов наклона камеры
            var mouse = OpenTK.Input.Mouse.GetState();
            mouseX = mouse.X;
            mouseY = mouse.Y;
            int xc = Width / 2;
            int yc = Height / 2;
            latitude = (xc - mouseX) / 8;
            longitude = (yc - mouseY) / 8;
            if (longitude < -89.0f)
                longitude = -89.0f;
            if (longitude > 89.0f)
                longitude = 89.0f;
            // Заполнение вертекс буферов данными
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandlers[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * positionData.Length), positionData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandlers[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * colorData.Length), colorData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vcol, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.UseProgram(BasicProgramID);

            GL.Uniform3(uniform_pos, camera_position);
        }
    }
}

