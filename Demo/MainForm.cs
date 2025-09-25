using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Demo
{
    public partial class MainForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DataGridView dgvPartners;
        private Button btnAddPartner;
        private Button btnEditPartner;
        private Button btnViewHistory;
        private PictureBox pbLogo;
        private Label lblTitle;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadPartnersData();
        }

        

        /// <summary>
        /// Загрузка данных о партнерах из базы данных
        /// </summary>
        private void LoadPartnersData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            PartnerId,
                            [Наименование партнера] AS 'Наименование',
                            [Тип партнера] AS 'Тип',
                            [Руководитель],
                            [Телефон партнера] AS 'Телефон',
                            [Электронная почта партнера] AS 'Email',
                            [Рейтинг]
                        FROM Partners_import
                        ORDER BY [Рейтинг] DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvPartners.DataSource = dataTable;

                    // Скрываем колонку ID
                    dgvPartners.Columns["PartnerId"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddPartner_Click(object sender, EventArgs e)
        {
            var addEditForm = new AddEditPartnerForm(connectionString);
            if (addEditForm.ShowDialog() == DialogResult.OK)
            {
                LoadPartnersData(); // Обновляем данные после добавления
            }
        }

        private void BtnEditPartner_Click(object sender, EventArgs e)
        {
            EditSelectedPartner();
        }

        private void DgvPartners_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                EditSelectedPartner();
            }
        }

        /// <summary>
        /// Редактирование выбранного партнера
        /// </summary>
        private void EditSelectedPartner()
        {
            if (dgvPartners.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите партнера для редактирования.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int partnerId = Convert.ToInt32(dgvPartners.SelectedRows[0].Cells["PartnerId"].Value);
            var addEditForm = new AddEditPartnerForm(connectionString, partnerId);

            if (addEditForm.ShowDialog() == DialogResult.OK)
            {
                LoadPartnersData(); // Обновляем данные после редактирования
            }
        }

        private void BtnViewHistory_Click(object sender, EventArgs e)
        {
            if (dgvPartners.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите партнера для просмотра истории услуг.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int partnerId = Convert.ToInt32(dgvPartners.SelectedRows[0].Cells["PartnerId"].Value);
            string partnerName = dgvPartners.SelectedRows[0].Cells["Наименование"].Value.ToString();

            var historyForm = new ServiceHistoryForm(connectionString, partnerId, partnerName);
            historyForm.Show();
        }
    }
}
