using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Demo
{
    public partial class ServiceHistoryForm : Form
    {
        private string connectionString;
        private int partnerId;
        private string partnerName;
        private DataGridView dgvHistory;
        private Button btnBack;
        private Label lblPartnerInfo;

        public ServiceHistoryForm(string connectionString, int partnerId, string partnerName)
        {
            this.connectionString = connectionString;
            this.partnerId = partnerId;
            this.partnerName = partnerName;

            InitializeComponent();
            InitializeCustomComponents();
            LoadServiceHistory();
        }

        private void InitializeCustomComponents()
        {
            // Информация о партнере
            lblPartnerInfo = new Label
            {
                Text = $"История услуг партнера: {partnerName}",
                Font = new Font("Franklin Gothic Medium", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(800, 30),
                ForeColor = Color.FromArgb(0, 206, 209)
            };

            // DataGridView для отображения истории услуг
            dgvHistory = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(840, 450),
                BackgroundColor = Color.FromArgb(224, 255, 255),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Кнопка "Назад"
            btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(20, 530),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 206, 209),
                ForeColor = Color.White,
                Font = new Font("Franklin Gothic Medium", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += BtnBack_Click;

            this.Controls.AddRange(new Control[]
            {
                lblPartnerInfo, dgvHistory, btnBack
            });
        }

        /// <summary>
        /// Загрузка истории услуг партнера
        /// </summary>
        private void LoadServiceHistory()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            psi.[Услуга] AS 'Наименование услуги',
                            psi.[Количество услуг] AS 'Количество',
                            psi.[Дата выполнения] AS 'Дата выполнения',
                            si.[Минимальная стоимость для партнера] AS 'Стоимость за единицу',
                            (psi.[Количество услуг] * si.[Минимальная стоимость для партнера]) AS 'Общая стоимость',
                            si.[Тип услуги] AS 'Тип услуги'
                        FROM Partner_services_import psi
                        INNER JOIN Partners_import pi ON psi.[Наименование партнера] = pi.[Наименование партнера]
                        INNER JOIN Services_import si ON psi.[Услуга] = si.[Наименование услуги]
                        WHERE pi.PartnerId = @PartnerId
                        ORDER BY psi.[Дата выполнения] DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@PartnerId", partnerId);

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvHistory.DataSource = dataTable;

                    // Форматирование колонок
                    if (dgvHistory.Columns["Дата выполнения"] != null)
                    {
                        dgvHistory.Columns["Дата выполнения"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    }

                    if (dgvHistory.Columns["Стоимость за единицу"] != null)
                    {
                        dgvHistory.Columns["Стоимость за единицу"].DefaultCellStyle.Format = "C2";
                    }

                    if (dgvHistory.Columns["Общая стоимость"] != null)
                    {
                        dgvHistory.Columns["Общая стоимость"].DefaultCellStyle.Format = "C2";
                    }

                    // Обновление заголовка с количеством записей
                    lblPartnerInfo.Text = $"История услуг партнера: {partnerName} (найдено записей: {dataTable.Rows.Count})";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории услуг: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
