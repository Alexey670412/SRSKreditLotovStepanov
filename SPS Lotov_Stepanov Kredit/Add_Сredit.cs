using System;
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
    public partial class Add_Сredit : Form
    {
        public Add_Сredit()
        {
            InitializeComponent();
        }

        private string ChekStatus(int index)
        {
            switch (index)
            {
                case 0:
                    return "Выплачен(без_просрочек)";
                case 1:
                    return "Выплачен(с_просрочками)";
                case 2:
                    return "Не_выплачен(без_просрочек)";
                case 3:
                    return "Не_выплачен(с_просрочками)";
                default:
                    return "none";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int prov_zapol = 0;
            if (numericUpDown1.Value == 0) prov_zapol++;
            if (numericUpDown6.Value == 0) prov_zapol++;
            int chekStatus = -1;
            foreach (int indexChecked in checkedListBox1.CheckedIndices)
            {
                chekStatus = indexChecked;
            }
            if (chekStatus == -1) prov_zapol++;

            if (prov_zapol != 0)
            {
                MessageBox.Show("Ошибка: заполните все данные");
            }
            else
            {
                string strStatus = ChekStatus(chekStatus);
                NpgsqlConnection connection = new NpgsqlConnection("host=localhost;user=postgres;password=1;database=Scoring;encoding=unicode");
                string commandinsert = "INSERT INTO Кредиты (Код_заёмщика, Статус, Код_банка)" +
                    "VALUES('"+numericUpDown1.Value+"','"+strStatus+"','"+numericUpDown6.Value+"')";
                NpgsqlCommand command = new NpgsqlCommand(commandinsert, connection);

                int dobavlenie = 0;
                connection.Open();

                try
                {
                    dobavlenie = command.ExecuteNonQuery();
                }
                catch
                {
                    MessageBox.Show("Ошибка: Заёмщик с таким кодом уже существует");
                }

                connection.Close();

                if (dobavlenie > 0)
                {
                    MessageBox.Show("Кредит успешно добавлен");
                    numericUpDown1.Value = 0;
                    numericUpDown6.Value = 0;
                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        checkedListBox1.SetItemChecked(i, false);
                    }
                }
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
            checkedListBox1.SetItemChecked(checkedListBox1.SelectedIndex, true);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
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
                string commandFind = "select Код_заёмщика, Фамилия, Имя, Отчество from Заёмщики where Имя = '" + textBox1.Text + "' and Фамилия = '" + textBox2.Text + "' and Отчество = '" + textBox3.Text + "' and Возраст = '" + numericUpDown2.Value + "'";

                NpgsqlDataAdapter command = new NpgsqlDataAdapter(commandFind, connection);

                connection.Open();
                command.Fill(ds, "Заёмщик");
                connection.Close();

                

                DataTable dtZaem = ds.Tables["Заёмщик"];
                DataRow[] drZaem = dtZaem.Select();

                if (ds.Tables["Заёмщик"].Rows.Count == 0)
                {
                    MessageBox.Show("Такого заёмщика не существует");
                }
                else
                {
                    foreach (DataRow row in drZaem)
                    {
                        numericUpDown1.Value = int.Parse(row["Код_заёмщика"].ToString());
                        MessageBox.Show("Код заёмщика: " + row["Код_заёмщика"].ToString());
                    }
                }
            }
        }
    }
}
