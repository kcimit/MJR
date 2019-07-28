using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MJR
{
    public static class Extract
    {

        private static bool SaveFile(ViewModel vm, ref int fcount, byte[] dataArray, int writePos)
        {
            try
            {
                using (FileStream writeStream =
                    new FileStream($"{vm.OutputFolder}\\{fcount++}.jpeg", FileMode.Create))
                {
                    BinaryWriter bw = new BinaryWriter(writeStream);
                    bw.Write(dataArray, 0, writePos);
                }

                int res = fcount;
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    vm.AddProgress(new ItemVM($"Created {res}.jpeg succesfully.", Colors.White, Colors.Green, true));
                }));
                
            }
            catch (Exception e)
            {
                MessageBox.Show($"Cannot create file {$"{vm.OutputFolder}\\{fcount}.jpeg"}: \r\n{e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static int ProcessExtract(ViewModel vm)
        {
            int fcount = 0;
            bool stFl1 = false;
            bool stFl2 = false;
            bool wrFl = false;
            bool endFl = false;
            var writePos = 0;
            int curPos = 0;
            var writeBufferLength = vm.MaxJPEGSize * 1024;
            byte[] buffer = new byte[512000];
            byte[] dataArray = new byte[writeBufferLength];
            using (BinaryReader reader = new BinaryReader(File.Open(vm.InputFile, FileMode.Open)))
            {
                int bytesRead;
                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    curPos = 0;
                    while (curPos < bytesRead)
                    {
                        if (writePos == writeBufferLength && wrFl)
                        {
                            if (!SaveFile(vm, ref fcount, dataArray, writePos)) return -1;

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
                            dataArray[0] = 0xFF;
                            dataArray[1] = 0xD8;
                            dataArray[2] = 0xFF;

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

                            if (!SaveFile(vm, ref fcount, dataArray, writePos)) return -1;
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

            return fcount;
        }

    }
}
