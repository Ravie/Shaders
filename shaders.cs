using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shaders
{
    class shader : GameWindow
    {
        int BasicProgramID;
        int BasicVertexShader;
        int BasicFragmentShader;

        int vaoHandle;
        int[] vboHandlers = new int[2];
        float[] positionData = { -0.8f, -0.8f, 0.0f, 0.8f, -0.8f, 0.0f, 0.0f, 0.8f, 0.0f };
        float[] colorData = { 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f };

        void loadShader(String filename, ShaderType type, int program, out int address)
        {
            //string glVersion = GL.GetString(StringName.Version);
            //string glslVersion = GL.GetString(StringName.ShadingLanguageVersion);
            address = GL.CreateShader(type);
            /*if (address == 0)
            {
                string message = String.Format("Can't create shader");
                throw new ArgumentOutOfRangeException(message);
            }*/
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
            loadShader("..\\..\\basic.vs", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
            loadShader("..\\..\\basic.fs", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
            //Компоновка программы
            GL.LinkProgram(BasicProgramID);

            // Проверить успех компоновки
            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);

            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));

            GL.GenBuffers(2, vboHandlers);

            vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(vaoHandle);
        }

        protected override void OnLoad(EventArgs e)   //Вызывает событие Load
        {
            base.OnLoad(e);

            InitShaders();
        }

        protected override void OnRenderFrame(FrameEventArgs e)   //Отвечает за перересовку
        {
            base.OnRenderFrame(e);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);  //Очищать буфер
            GL.Enable(EnableCap.DepthTest);    //Дальние элементы перекрываются ближними
            
            GL.EnableVertexAttribArray(0);   //Активация атрибутов вершин
            GL.EnableVertexAttribArray(1);   //Или вкл. режима отрисовки

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);  //Отображает нашу фигуру

            GL.DisableVertexAttribArray(0);  //Выключаем режим отрисовки
            GL.DisableVertexAttribArray(1);

            SwapBuffers();   //Быстро скопировать содержимое заднего буфера окна в передний буфер
        }

        protected override void OnUpdateFrame(FrameEventArgs e)   //Отвечает за обновление Update
        {
            base.OnUpdateFrame(e);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandlers[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * positionData.Length), positionData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandlers[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * colorData.Length), colorData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.UseProgram(BasicProgramID);
        }
    }
}

