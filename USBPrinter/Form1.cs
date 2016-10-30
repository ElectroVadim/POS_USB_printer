using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USBPrinter
{
    public partial class Form1 : Form
    {

        Printer print = new Printer();


        public Form1()
        {
            InitializeComponent();

        } 

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == null)
            {
                button1.Visible = false;
            }
            else
            {
                button1.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                print.Print(Win1251ToUTF8(textBox1.Text));
            }
            else
            {
                MessageBox.Show("введите что нибуть");
            }


        }

        private string Win1251ToUTF8(string source)
        {

            Encoding utf8 = Encoding.GetEncoding(866);
            Encoding win1251 = Encoding.GetEncoding("windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(source);
            byte[] win1251Bytes = Encoding.Convert(win1251, utf8, utf8Bytes);
            source = win1251.GetString(win1251Bytes);
            return source;

        }

        // static string UTF8ToWin1251(string sourceStr)
        //{
        //    Encoding utf8 = Encoding.UTF8;
        //    Encoding win1251 = Encoding.GetEncoding(866);
        //    byte[] utf8Bytes = utf8.GetBytes(sourceStr);

        //    byte[] win1251Bytes = Encoding.Convert(utf8,win1251, utf8Bytes);
        //    return win1251.GetString(win1251Bytes);
        //}

    }
}
