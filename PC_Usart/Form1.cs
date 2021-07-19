using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Microsoft.VisualBasic;

namespace PC_Usart
{
    public partial class Form1 : Form
    {
        SerialPort s = new SerialPort();  //实例化一个串口对象，最好在后端代码中写代码，这样复制到其他地方不会出错。S是一个串口的句柄。

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //synt = SynchronizationContext.Current;
            //InitializeComponent();

            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            comboBox1.SelectedItem = comboBox1.Items[0];

            int[] item = { 9600, 115200 };  //定义一个Item数组，遍历item中每一个遍量a，增加到combobox的列表中
            foreach (int a in item)
            {
                comboBox2.Items.Add(a.ToString());
            }
            comboBox2.SelectedItem = comboBox2.Items[1];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!s.IsOpen)
                {
                    s.PortName = comboBox1.SelectedItem.ToString(); //配置端口号
                    s.BaudRate = Convert.ToInt32(comboBox2.SelectedItem.ToString()); //配置波特率
                    s.Open();
                    s.DataReceived += s_DataReceived; //接受数据
                }
                else
                {
                    s.Close();
                    s.DataReceived -= s_DataReceived;
                    button1.Text = "点击关闭串口";
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        void s_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int count = s.BytesToRead;  //获取接收的数据的字节数 8位 16位
            string str = null;
            if (count == 8)
            {
                byte[] buff = new byte[count];
                s.Read(buff, 0, count);
                foreach (byte item in buff)
                {
                    str += item.ToString("X2") + "";
                }
                richTextBox1.Text = System.DateTime.Now.ToString() + ":" + str + "\n" + richTextBox1.Text;
                // 这个是跨线程访问richtexbox,原程序和DataReceived事件是两个不同的线程同时在执行
                // 接下来是收到的数据的处理
            }
        }

        private void button3_Click(object sender, EventArgs e) //发送数据
        {
            string[] sendbuff = richTextBox2.Text.Split(); //分割输入的字符串，判断有多少的字节需要发送
            foreach (string item in sendbuff)
            {
                int count = 1;
                byte[] buff = new byte[count];
                buff[0] = byte.Parse(item, System.Globalization.NumberStyles.HexNumber);//格式化字符串为十六进制的数值
                s.Write(buff, 0, count);

            }
        }
        static SynchronizationContext synt;
        
    }
}
