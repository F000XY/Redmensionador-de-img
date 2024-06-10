using System;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace didaticos.redimensionador
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Redimensionador");

            Thread thread = new Thread(Redimensionar);
            thread.Start();

            Console.Read();
        }

        static void Redimensionar()
        {
            #region "Diretorios"
            string diretorioEntrada = "C:\\Users\\55119\\source\\repos\\didaticos.redimensionador\\didaticos.redimensionador\\arquivosEntrada";
            string diretorioRedimensionado = "C:\\Users\\55119\\source\\repos\\didaticos.redimensionador\\didaticos.redimensionador\\arquivosFinalizados";
            string diretorioFinalizado = "C:\\Users\\55119\\source\\repos\\didaticos.redimensionador\\didaticos.redimensionador\\arquivosRedimensionados";

            if (!Directory.Exists(diretorioEntrada))
            {
                Directory.CreateDirectory(diretorioEntrada);
            }

            if (!Directory.Exists(diretorioRedimensionado))
            {
                Directory.CreateDirectory(diretorioRedimensionado);
            }


            if (!Directory.Exists(diretorioFinalizado))
            {
                Directory.CreateDirectory(diretorioFinalizado);
            }
            #endregion

            FileInfo fileInfo;
            string caminhoRedimensionado;
            string caminhoFinalizado;

            while (true)
            {
                var arquivosEntrada = Directory.EnumerateFiles(diretorioEntrada);

                foreach (var arquivo in arquivosEntrada)
                {
                    fileInfo = new FileInfo(arquivo);
                    caminhoRedimensionado = Path.Combine(diretorioRedimensionado, fileInfo.Name);

                    if (!Directory.Exists(diretorioRedimensionado))
                    {
                        Directory.CreateDirectory(diretorioRedimensionado);
                    }

                    using (FileStream fs = new FileStream(arquivo, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        Redimensionador(Image.FromStream(fs), 214, 120, caminhoRedimensionado);
                        fs.Close();
                    }

                    caminhoFinalizado = Path.Combine(diretorioFinalizado, fileInfo.Name);
                    fileInfo.MoveTo(caminhoFinalizado);
                }

                Thread.Sleep(new TimeSpan(0, 0, 1));
            }
        }

        static void Redimensionador(Image imagem, int larguraFinal, int alturaFinal, string caminho)
        {
            // Proporção desejada 16:9
            double proporcaoDesejada = 16.0 / 9.0;

            // Calcular a nova largura e altura mantendo a proporção 16:9
            int larguraCalculada = larguraFinal;
            int alturaCalculada = (int)(larguraCalculada / proporcaoDesejada);

            if (alturaCalculada > alturaFinal)
            {
                alturaCalculada = alturaFinal;
                larguraCalculada = (int)(alturaCalculada * proporcaoDesejada);
            }

            // Redimensionar a imagem mantendo a proporção 16:9
            Bitmap imagemRedimensionada = new Bitmap(larguraCalculada, alturaCalculada);
            using (Graphics g = Graphics.FromImage(imagemRedimensionada))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.DrawImage(imagem, 0, 0, larguraCalculada, alturaCalculada);
            }

            // Criar uma imagem final com o tamanho exato 214x120 e fundo transparente
            Bitmap imagemFinal = new Bitmap(larguraFinal, alturaFinal, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(imagemFinal))
            {
                g.Clear(Color.Transparent); // Preencher com transparente
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                int offsetX = (larguraFinal - larguraCalculada) / 2;
                int offsetY = (alturaFinal - alturaCalculada) / 2;
                g.DrawImage(imagemRedimensionada, offsetX, offsetY, larguraCalculada, alturaCalculada);
            }

            imagemFinal.Save(caminho, ImageFormat.Png); // Salvar como PNG para manter a transparência
            imagem.Dispose();
            imagemRedimensionada.Dispose();
        }
    }
}
