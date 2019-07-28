using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MJR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            int fcount = 0;
            bool stFl1 = false;
            bool stFl2 = false;
            bool wrFl = false;
            bool endFl = false;
            var writePos = 0;
            int curPos = 0;
            var writeBufferLength = 20000000;
            byte[] buffer = new byte[512000];
            byte[] dataArray = new byte[writeBufferLength];
            using (BinaryReader reader = new BinaryReader(File.Open(@"c:\2\test.dat", FileMode.Open)))
            {
                int bytesRead;
                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    curPos = 0;
                    while (curPos < bytesRead)
                    {
                        if (writePos == writeBufferLength && wrFl)
                        {
                            using (FileStream writeStream =
                                new FileStream($"c:\\2\\test{fcount++}.jpeg", FileMode.Create))
                            {
                                BinaryWriter bw = new BinaryWriter(writeStream);
                                bw.Write(dataArray, 0, writePos);
                            }

                            wrFl = false;
                            endFl = false;
                        }

                        var read = buffer[curPos++];
                        if (read == 0xFF && !stFl2 && !wrFl)
                        {
                            stFl1 = true;
                            continue;
                        }

                        if (read == 0xD8 && stFl1 && !stFl2 && !wrFl)
                        {
                            stFl2 = true;
                            continue;
                        }

                        if (read == 0xFF && stFl2 && !wrFl)
                        {
                            wrFl = true;
                            dataArray = new byte[writeBufferLength];
                            writePos = 3;
                            dataArray[0] = 0xFF; dataArray[1] = 0xD8; dataArray[2] = 0xFF;

                            stFl1 = false;
                            stFl2 = false;
                            continue;
                        }

                        if (wrFl && read == 0xFF)
                        {
                            endFl = true;
                            dataArray[writePos++] = read;
                            continue;
                        }

                        if (wrFl && endFl && read == 0xD9)
                        {
                            //write 
                            wrFl = false;
                            endFl = false;
                            dataArray[writePos++] = read;

                            using (FileStream writeStream =
                                new FileStream($"c:\\2\\test{fcount++}.jpeg", FileMode.Create))
                            {
                                BinaryWriter bw = new BinaryWriter(writeStream);
                                bw.Write(dataArray, 0, writePos);
                            }
                        }

                        stFl1 = false;
                        stFl2 = false;
                        endFl = false;

                        if (wrFl)
                        {
                            dataArray[writePos++] = read;
                            continue;
                        }
                    }
                }
            }

            MessageBox.Show($"{fcount} Files written.");

        }

        private void ButtonSelectFile(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonOutputFolder(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
