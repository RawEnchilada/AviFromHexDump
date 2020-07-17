using System;
using System.IO;
using System.Reflection;

namespace Avi_from_hex_dump
{
    class Program
    {
        static bool ohshit = false;
        static long position;
        static long limit = 31438405631;
        static DateTime lasttime;
        static bool little = true;
        static void Main(string[] args)
        {
            if(Read()){
                Console.WriteLine("Reading was successfull");
            }
            else{
                Console.WriteLine("Emergency exit, position = "+position);
            }
            Console.ReadKey();
        }

        static bool Read(){
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            lasttime = DateTime.Now;
            string filename = "hex";  

            


            FileStream file = new FileStream(@path+"/"+filename,FileMode.Open);

            position = 0;
            int name = 0;
            int[] block = new int[4];
            int[] temp = new int[4];
            bool running = true;

            while(running)
            {
                block = ReadBlock(file);
                if(block[0] == -1 || block[1] == -1 || block[2] == -1 || block[3] == -1){
                    running = false;
                    break;
                }
                if(isRiff(block)){
                    temp = ReadBlock(file);
                    uint num = asInt(temp);
                    
                    Console.WriteLine("\"Riff\" found, Size = "+num);
                    if(num == 0)continue;
                    FileStream saving = new FileStream(@path+"/Videos/"+name+".avi",FileMode.Create);
                    Console.Write(".");
                    long end = position+(long)num;
                    for (int i = 0; i < 4; i++)
                    {
                        saving.WriteByte((byte)block[i]);
                    }
                    Console.Write(".");
                    for (int i = 0; i < 4; i++)
                    {
                        saving.WriteByte((byte)temp[i]);
                    }
                    Console.Write(".");
                    while(position < end){
                        if(ohshit){
                            return false;
                        }
                        temp = ReadBlock(file);

                        for (int i = 0; i < 4; i++){
                            saving.WriteByte((byte)temp[i]);
                        }
                    }
                    Console.WriteLine(".");
                    saving.Close();
                    Console.Write("Saved "+name+".avi,  ");
                    Console.WriteLine("Time Spent: "+(DateTime.Now-lasttime));
                    Console.WriteLine("Cursor at "+position);
                    name++;
                }
            }
            Console.WriteLine("Read "+filename+", ends at: "+position+" Hex: "+position.ToString("X"));
            return true;
        }


        static int[] ReadBlock(FileStream stream){
            int[] block = new int[4];
            for (int i = 0; i < 4; i++)
            {
                block[i] = (int)stream.ReadByte();
            }
            if(position == 0)position = 3;
            else position += 4;
            if(position > limit+4)ohshit = true;
            return block;
        }
        static uint asInt(int[] bytes){
            string[] hexs = new string[4];
            for (int i = 0; i < 4; i++)
            {
                hexs[i] = bytes[i].ToString("X");
                if(hexs[i] == "0")hexs[i] = "00";
            }   
            uint newNumber;         
            if(little)newNumber = uint.Parse(LittleEndian(hexs[0]+hexs[1]+hexs[2]+hexs[3]),System.Globalization.NumberStyles.HexNumber);
            else newNumber = uint.Parse(hexs[0]+hexs[1]+hexs[2]+hexs[3],System.Globalization.NumberStyles.HexNumber);
            return newNumber;
        }
        static bool isRiff(int[] bytes){
            return (bytes[0] == 82 && bytes[1] == 73 && bytes[2] == 70 && bytes[3] == 70);
        }
        static string LittleEndian(string num)
        {
            int number = Convert.ToInt32(num, 16);
            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }
    }
}
