using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

/*
Author: Halil Kemal TASKIN
Web: http://hkt.me
*/

namespace AdvancedSerpentOperations
{
    [Serializable()]
    public class Ayarlar
    {
        public Color RenkOne { get; set; }
        public Color RenkZero { get; set; }
        public Color RenkQuestion { get; set; }

        public string W0 { get; set; }
        public string W1 { get; set; }
        public string W2 { get; set; }
        public string W3 { get; set; }

        public string O0 { get; set; }
        public string O1 { get; set; }
        public string O2 { get; set; }
        public string O3 { get; set; }

        public decimal RotW { get; set; }
        public decimal RotO { get; set; }
        public decimal SBox { get; set; }

        public bool AOT { get; set; }

        public string Log { get; set; }

        public string[] AutoOpList { get; set; }
    }

    public static class AyarlarTools
    {
        private static string Dosya = Path.GetDirectoryName(Application.ExecutablePath) + @"\Ayarlar.bin";
        private static BinaryFormatter bf = new BinaryFormatter();

        public static bool DosyayaAyarKaydet(Ayarlar a)
        {
            FileStream fs = new FileStream(Dosya, FileMode.Create);
            try
            {
                bf.Serialize(fs, a);
                fs.Close();
                return true;
            }
            catch
            {
                fs.Close();
                return false;
            }

        }

        public static Ayarlar DosyadanAyarYukle()
        {
            bool DosyadanYukle = File.Exists(Dosya);

            if (DosyadanYukle)
            {
                FileStream fs = new FileStream(Dosya, FileMode.Open);
                try
                {

                    object o = bf.Deserialize(fs);
                    fs.Close();
                    return (o as Ayarlar);
                }
                catch
                {
                    fs.Close();
                    return VarsayilanAyarlar;
                }
            }
            else
            {
                return VarsayilanAyarlar;
            }
        }

        private static Ayarlar VarsayilanAyarlar
        {
            get
            {
                Ayarlar a = new Ayarlar();

                a.RenkOne = Color.OrangeRed;
                a.RenkZero = Color.Black;
                a.RenkQuestion = Color.DeepSkyBlue;

                a.W0 = "";
                a.W1 = "";
                a.W2 = "";
                a.W3 = "";

                a.O0 = "";
                a.O1 = "";
                a.O2 = "";
                a.O3 = "";

                a.RotO = 1;
                a.RotW = 1;
                a.SBox = 0;

                a.AOT = true;

                a.Log = "";

                a.AutoOpList = new string[0];

                return a;
            }
        }
    }
}
