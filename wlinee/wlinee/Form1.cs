using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace wlinee
{

    public partial class wlinee : Form
    {

        [DllImport("lineengin.dll")]
        extern static int InitializeLinee();
        [DllImport("lineengin.dll")]
        extern static int GetVer(StringBuilder lpName, int len);
        [DllImport("lineengin.dll")]
        extern static int GetInfName(int index, int len, StringBuilder lpName, StringBuilder lpDesc);
        [DllImport("lineengin.dll")]
        extern static int GetStatus(out int recv0, out int recv1, out int drop0, out int drop1, out int qlen0, out int qlen1);
        [DllImport("lineengin.dll")]
        extern static int SetInterface(string if0, string if1);
        [DllImport("lineengin.dll")]
        extern static int StopWork();
        [DllImport("lineengin.dll")]
        extern static int StartWork();
        [DllImport("lineengin.dll")]
        extern static void SetParams(int direction, int param, int stat,double param1,double param2);

        /*******************************
         * 定数
         * *****************************/
        const   int UP_STREAM   =0;
        const   int DOWN_STREAM =1;

        const   int POLICING    =1;
        const   int FRAMELOSS   =2;
        const   int FRAMEDELAY  =3;
        const   int NONE        =0;
        const   int BERNOULLI   =1;
        const   int BURST       =2;
        const   int UNIFORMAL   =3;
        const   int GAUSSIAN    =4;
        const   int CONSTANT    =5;



        /******************************
         * 変数
         * ****************************/
        Hashtable infs = new Hashtable();
        bool set_inf = false;
        int dll_status=0;
        /******************************
         * メソッド
         * ****************************/

        public wlinee()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int     ret;
            int     ifn;

            StringBuilder name = new StringBuilder(256);
            StringBuilder desc = new StringBuilder(256);
            ifn=InitializeLinee();
            GetVer(name,name.Capacity);
            dllName.Text = name.ToString();
            
            for (int i = 1; i <=ifn; i++)
            {
                ret = GetInfName(i, name.Capacity  , name,   desc);
                infs.Add(desc.ToString(), name.ToString());
                comboBox1.Items.Add(desc.ToString());
                comboBox2.Items.Add(desc.ToString());
            }
            comboBox2.EndUpdate();
            comboBox1.EndUpdate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StopWork();
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int recv0, recv1;
            int qlen0, qlen1;
            int drop0, drop1;
            dll_status = GetStatus(out recv0, out recv1, out drop0, out drop1, out qlen0, out qlen1);
            if (dll_status == 1)
            {
                txt_status.Text = "Active";
                //数値取得を表示
                txt_recv0.Text = recv0.ToString();
                txt_recv1.Text = recv1.ToString();
                txt_drop0.Text = drop0.ToString();
                txt_drop1.Text = drop1.ToString(); 
                txt_qlen0.Text = qlen0.ToString();
                txt_qlen1.Text = qlen1.ToString();
            }
            else
            {
                txt_status.Text = "Stop";
            }
            
        }

        private void btn_para_Click(object sender, EventArgs e)
        {
            double  para1,para2;
            //ポリシング=======================================================
            //0方向-------------------------------------------
            if (ra_po_0_1.Checked == true)
            {
                para1 = (double)nu_po_0.Value;
                SetParams(UP_STREAM, POLICING, 0, para1, 0);
            }
            else
            {
                //解除
                SetParams(UP_STREAM, POLICING, 0, 0, 0);
            }
            //1方向-------------------------------------------
            if (ra_po_1_1.Checked == true)
            {
                para1 = (double)nu_po_1.Value;
                SetParams(DOWN_STREAM, POLICING, 0, para1, 0);
            }
            else
            {
                //解除
                SetParams(DOWN_STREAM, POLICING, 0, 0, 0);
            }
            //フレームロス=======================================================
            //0方向-------------------------------------------
            if (ra_lo_0_1.Checked == true)
            {
                //ランダム
                para1 = (double)nu_lo_0_1.Value;
                SetParams(UP_STREAM, FRAMELOSS, BERNOULLI, para1/100.0, 0);
            }
            else if (ra_lo_0_2.Checked == true)
            {
                //バースト
                para1 = (double)nu_lo_0_2.Value;
                SetParams(UP_STREAM, FRAMELOSS, BURST, para1, 0);
            }
            else
            {
                //解除
                SetParams(UP_STREAM, FRAMELOSS, NONE, 0, 0);
            }
            //1方向-------------------------------------------
            if (ra_lo_1_1.Checked == true)
            {
                //ランダム
                para1 = (double)nu_lo_1_1.Value;
                SetParams(DOWN_STREAM, FRAMELOSS, BERNOULLI, para1 / 100.0, 0);
            }
            else if (ra_lo_1_2.Checked == true)
            {
                //バースト
                para1 = (double)nu_lo_1_2.Value;
                SetParams(DOWN_STREAM, FRAMELOSS, BURST, para1, 0);
            }
            else
            {
                //解除
                SetParams(DOWN_STREAM, FRAMELOSS, NONE, 0, 0);
            }
            //遅延=======================================================
            //0方向-------------------------------------------
            if (ra_de_0_1.Checked == true)
            {
                //固定
                para1 = (double)nu_de_0_1.Value;
                SetParams(UP_STREAM, FRAMEDELAY, CONSTANT, para1, 0);
            }
            else if (ra_de_0_2.Checked == true)
            {
                //一様分布
                para1 = (double)nu_de_0_2.Value;
                SetParams(UP_STREAM, FRAMEDELAY, UNIFORMAL, para1, 0);
            }
            else if (ra_de_0_3.Checked == true)
            {
                //正規分布
                para1 = (double)nu_de_0_3.Value;
                para2 = (double)nu_de_0_4.Value;
                SetParams(UP_STREAM, FRAMEDELAY, GAUSSIAN, para1, para2);
            }
            else
            {
                //解除
                SetParams(UP_STREAM, FRAMEDELAY, NONE, 0, 0);
            }
            //1方向-------------------------------------------
            if (ra_de_1_1.Checked == true)
            {
                //固定
                para1 = (double)nu_de_1_1.Value;
                SetParams(DOWN_STREAM, FRAMEDELAY, CONSTANT, para1, 0);
            }
            else if (ra_de_1_2.Checked == true)
            {
                //一様分布
                para1 = (double)nu_de_1_2.Value;
                SetParams(DOWN_STREAM, FRAMEDELAY, UNIFORMAL, para1, 0);
            }
            else if (ra_de_1_3.Checked == true)
            {
                //正規分布
                para1 = (double)nu_de_1_3.Value;
                para2 = (double)nu_de_1_4.Value;
                SetParams(DOWN_STREAM, FRAMEDELAY, GAUSSIAN, para1, para2);
            }
            else
            {
                //解除
                SetParams(DOWN_STREAM, FRAMEDELAY, NONE, 0, 0);
            }
        }

        private void bt_start_Click(object sender, EventArgs e)
        {
            if (set_inf == true && dll_status==0)
            {
                StartWork();
                bt_start.Enabled = false;
                bt_set_inf.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int ret;
            ret = SetInterface(
                (string)infs[(string)comboBox1.SelectedItem.ToString()],
                (string)infs[(string)comboBox2.SelectedItem.ToString()]);
            if (ret != 0)
            {
                set_inf = false;
            }
            else
            {
                set_inf = true;
                bt_start.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StopWork();
            txt_recv0.Text = "";
            txt_recv1.Text = "";
            txt_drop0.Text = "";
            txt_drop1.Text = "";
            txt_qlen0.Text = "";
            txt_qlen1.Text = "";
            if (set_inf == true)
            {
                bt_start.Enabled = true;
            }
            bt_set_inf.Enabled = true;
        }
    }
}