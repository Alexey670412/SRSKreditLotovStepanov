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
    public partial class Add_Borrower : Form
    {
        public Add_Borrower()
        {
            InitializeComponent();
        }

        private string ChekFamily(int index)
        {
            switch (index)
            {
                case 0:
                    return "Холост(не_замужем)";
                    break;
                case 1:
                    return "Женат(замужем)";
                    break;
                case 2:
                    return "Женат(замужем),_но_живут_отдельно";
                    break;
                case 3:
                    return "Разведен(а)";
                    break;
                case 4:
                    return "Вдова(вдовец)";
                    break;
                default:
                    return "none";
                    break;
            }
        }
        private string ChekWork(int index)
        {
            switch (index)
            {
                case 0:
                    return "Частный_директор";
                    break;
                case 1:
                    return "Учащийся";
                    break;
                case 2:
                    return "Пенсионер";
                    break;
                case 3:
                    return "Специалист";
                    break;
                case 4:
                    return "Руководитель";
                    break;
                default:
                    return "none";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int prov_zapol = 0;
            if (textBox1.Text == "") prov_zapol++;
            if (textBox2.Text == "") prov_zapol++;
            if (textBox3.Text == "") prov_zapol++;
            if (numericUpDown2.Value < 18) prov_zapol++;

            int chekFamily = -1;
            foreach (int indexChecked in checkedListBox1.CheckedIndices)
            {
                chekFamily = indexChecked;   
            }
            if (chekFamily == -1) prov_zapol++;

            int chekWork = -1;
            foreach (int indexChecked in checkedListBox2.CheckedIndices)
            {
                chekWork = indexChecked;
            }
            if (chekWork == -1) prov_zapol++;


            if (prov_zapol != 0)
            {
                MessageBox.Show("Ошибка: заполните все данные");
            }
            else
            {
                DataSet ds = new DataSet();
                NpgsqlConnection connection = new NpgsqlConnection("host=localhost;user=postgres;password=1;database=Scoring;encoding=unicode");

                string commandProvZaem = "select * from Заёмщики where Имя like '" + textBox1.Text + "' and Фамилия like '" + textBox2.Text + "' and Отчество like '"+textBox3.Text+"'";
                NpgsqlDataAdapter adapterProvZaem = new NpgsqlDataAdapter(commandProvZaem, connection);

                connection.Open();
                adapterProvZaem.Fill(ds, "Проверка заёмщика");
                connection.Close();
                
                if(ds.Tables["Проверка заёмщика"].Rows.Count!=0)
                {
                    MessageBox.Show("Этот заёмщик уже существует");
                }
                else
                {
                    string commandMaxIdZaem = "select max(Код_заёмщика) from Заёмщики";
                    NpgsqlDataAdapter adapterMaxIdZaem = new NpgsqlDataAdapter(commandMaxIdZaem, connection);

                    connection.Open();
                    adapterMaxIdZaem.Fill(ds, "Макс_код_заёмщика");
                    connection.Close();

                    DataTable dtMaxIdZaem = ds.Tables["Макс_код_заёмщика"];
                    DataRow[] drMaxIdZaem = dtMaxIdZaem.Select();

                    string strMaxIdZaem = "";
                    foreach (DataRow row in drMaxIdZaem)
                    {
                        strMaxIdZaem += "" + row["max"];
                    }
                    int maxIdZaem;
                    if (strMaxIdZaem == "")
                    {
                        maxIdZaem = 1;
                    }
                    else
                    {

                        maxIdZaem = int.Parse(strMaxIdZaem);
                        maxIdZaem++;
                    }

                    string strFamily = ChekFamily(chekFamily);
                    string strWork = ChekWork(chekWork);

                    string commandinsert = "INSERT INTO Заёмщики (Код_заёмщика, Имя, Фамилия, Отчество, Возраст, Семейное_положение, Сфера_деятельности, Среднемесячный_доход, Стаж_работы)" +
                        "VALUES('" + maxIdZaem + "','" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + numericUpDown2.Value + "','" + strFamily + "','" + strWork + "','" + numericUpDown3.Value + "','" + numericUpDown4.Value + "')";
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
                        MessageBox.Show("Заёмщик успешно добавлен");
                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox3.Text = "";
                        numericUpDown2.Value = 0;
                        for (int i = 0; i < checkedListBox1.Items.Count; i++)
                        {
                            checkedListBox1.SetItemChecked(i, false);
                            checkedListBox2.SetItemChecked(i, false);
                        }
                        numericUpDown3.Value = 0;
                        numericUpDown4.Value = 0;
                    }
                }
            }   
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for(int i = 0; i<checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
            checkedListBox1.SetItemChecked(checkedListBox1.SelectedIndex, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, false);
            }
            checkedListBox2.SetItemChecked(checkedListBox2.SelectedIndex, true);
        }
    }
}
