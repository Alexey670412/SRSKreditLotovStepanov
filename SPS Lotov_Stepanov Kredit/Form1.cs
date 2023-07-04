using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;


namespace SPS_Lotov_Stepanov_Kredit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void добавитьЗаёмщикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add_Borrower add_Borrower = new Add_Borrower();
            add_Borrower.Show();
        }

        private void добавитьКредитToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add_Сredit add_Сredit = new Add_Сredit();
            add_Сredit.Show();
        }

        private int VozrastScore(int vozrast)
        {
            if (vozrast < 20) return 20;
            else if (vozrast >= 20 && vozrast < 25) return 30;
            else if (vozrast >= 25 && vozrast < 30) return 50;
            else if (vozrast >= 30 && vozrast < 35) return 80;
            else if (vozrast >= 35 && vozrast < 50) return 95;
            else if (vozrast >= 50 && vozrast < 60) return 110;
            else if (vozrast >= 60) return 25;
            return 0;
        }
        private int StajWork(int Staj)
        {
            if (Staj <= 1) return 20;
            else if (Staj >= 2) return 40;
            else if (Staj >= 3) return 60;
            else if (Staj >= 5) return 90;
            else if (Staj > 5) return 130;
            return 0;
        }
        private int WorkState(string state)
        {
            switch(state){
                case "Частный_директор":
                    return 170;
                case "Учащийся":
                    return 50;
                case "Пенсионер":
                    return 30;
                case "Специалист":
                    return 60;
                case "Руководитель":
                    return 140;
                default: 
                    return 0;
            }
        }

        private int Dohod(int zarp)
        {
            if (zarp < 5000) return 15;
            else if (zarp >= 5000 && zarp < 15000) return 85;
            else if (zarp >= 15000 && zarp < 25000) return 100;
            else if (zarp >= 25000 && zarp < 50000) return 100;
            else if (zarp >= 50000) return 210;
            return 0;
        }
        private int FamilyState(string state)
        {
            switch (state)
            {
                case "Холост(не_замужем)":
                    return 70;
                case "Женат(замужем)":
                    return 100;
                case "Женат(замужем),_но_живут_отдельно":
                    return 65;
                case "Разведен(а)":
                    return 90;
                    
                case "Вдова(вдовец)":
                    return 85;

                default:
                    return 0;
            }
        }
        private int KreditStatus(string status)
        {
            switch (status)
            {
                case "Выплачен(без_просрочек)":
                    return 200;
   
                case "Выплачен(с_просрочками)":
                    return 50;

                case "Не_выплачен(без_просрочек)":

                case "Не_выплачен(с_просрочками)":
                    return -200;

                default:
                    return 0;
            }
        }

        private int CheckKreditAndBank(DataRow row)
        {
            DataSet ds = new DataSet();
            string strCodeBank = row["Код_банка"].ToString();
            string commandFindRating = "select Рейтинг from Банки where Код_банка = '"+strCodeBank+"'";
            NpgsqlConnection connection = new NpgsqlConnection("host=localhost;user=postgres;password=1;database=Scoring;encoding=unicode");
            NpgsqlDataAdapter adapterFindRating = new NpgsqlDataAdapter(commandFindRating, connection);
            string strKreditStatus = row["Статус"].ToString();
            connection.Open();
            adapterFindRating.Fill(ds, "Рейтинг");
            connection.Close();

            DataTable dtBankRating = ds.Tables["Рейтинг"];
            DataRow[] drBankRating = dtBankRating.Select();

            foreach (DataRow row1 in drBankRating)
            {
                return KreditStatus(strKreditStatus) / (10-int.Parse(row1["Рейтинг"].ToString()));
            }
            return 0;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            int prov_zapol = 0;
            if (textBox1.Text == "") prov_zapol++;
            if (textBox2.Text == "") prov_zapol++;
            if (textBox3.Text == "") prov_zapol++;
            if (numericUpDown2.Value < 18) prov_zapol++;
            if (prov_zapol != 0)
            {
                MessageBox.Show("Ошибка: заполните все данные");
            }
            else
            {
                NpgsqlConnection connection = new NpgsqlConnection("host=localhost;user=postgres;password=1;database=Scoring;encoding=unicode");
                string commandFindZaemInfo = "select Код_заёмщика, Возраст, Семейное_положение, Сфера_деятельности, Среднемесячный_доход, Стаж_работы from Заёмщики where Имя = '" + textBox1.Text + "' and Фамилия = '" + textBox2.Text + "' and Отчество = '" + textBox3.Text + "' and Возраст = '" + numericUpDown2.Value + "'";
                

                NpgsqlDataAdapter commandZaem = new NpgsqlDataAdapter(commandFindZaemInfo, connection);

                connection.Open();
                commandZaem.Fill(ds, "Заёмщики");
                connection.Close();

                DataTable dtZaemInfo = ds.Tables["Заёмщики"];
                DataRow[] drZaemInfo = dtZaemInfo.Select();

                if (ds.Tables["Заёмщики"].Rows.Count == 0)
                {
                    MessageBox.Show("Ошибка: Такого заёмщика не существует");
                }
                else
                {
                    string strZaemInfo = "";
                    foreach (DataRow row in drZaemInfo)
                    {
                        strZaemInfo += "" + row["Код_заёмщика"] + " " + row["Возраст"] + " " + row["Семейное_положение"] + " " + row["Сфера_деятельности"] + " " + row["Среднемесячный_доход"] + " " + row["Стаж_работы"];
                    }
                    string[] arrStrZaemInfo = strZaemInfo.Split(' ');

                    int kodZaem = int.Parse(arrStrZaemInfo[0]);
                    int vozrastZaem = int.Parse(arrStrZaemInfo[1]);
                    string familyState = arrStrZaemInfo[2];
                    string workZaem = arrStrZaemInfo[3];
                    int zarpZaem = int.Parse(arrStrZaemInfo[4]);
                    int stazWorkZaem = int.Parse(arrStrZaemInfo[5]);

                    string commandFindKredit = "select Код_заёмщика, Статус, Код_банка from Кредиты where Код_заёмщика = '" + kodZaem + "'";
                    NpgsqlDataAdapter adapterFindKredit = new NpgsqlDataAdapter(commandFindKredit, connection);

                    connection.Open();
                    adapterFindKredit.Fill(ds, "Кредит");
                    connection.Close();

                    DataTable dtKreditInfo = ds.Tables["Кредит"];
                    DataRow[] drKreditINfo = dtKreditInfo.Select();
                    int BankAndRatingScore = 0;
                    int KreditCount = 0;
                    foreach (DataRow row in drKreditINfo)
                    {
                        BankAndRatingScore += CheckKreditAndBank(row);
                        KreditCount++;
                    }
                    if (KreditCount == 0) { KreditCount = 1; }
                    int Score = VozrastScore(vozrastZaem) + StajWork(stazWorkZaem) + WorkState(workZaem) + Dohod(zarpZaem) + FamilyState(familyState) + (int)BankAndRatingScore / KreditCount;
                    if (Score < 0)
                    {
                        Score = 0;
                    }
                    trackBar1.Value = Score;
                    label10.Text = Score.ToString();
                }
            }
        }
    }
}
