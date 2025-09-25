using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Demo
{
    public partial class AddEditPartnerForm : Form
    {
        private string connectionString;
        private int? partnerId; // null для добавления, ID для редактирования
            
        // Контролы формы
        private TextBox txtPartnerName;
        private ComboBox cmbPartnerType;
        private TextBox txtDirector;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtINN;
        private NumericUpDown nudRating;
        private Button btnSave;
        private Button btnCancel;

        public AddEditPartnerForm(string connectionString, int? partnerId = null)
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            this.partnerId = partnerId;

            InitializeComponent();
            InitializeCustomComponents();
            LoadPartnerTypes();

            if (partnerId.HasValue)
            {
                LoadPartnerData();
            }
        }

        private void InitializeCustomComponents()
        {
            int yPosition = 20;
            int labelWidth = 120;
            int textBoxWidth = 300;
            int spacing = 50;

            // Наименование партнера
            var lblPartnerName = new Label
            {
                Text = "Наименование:",
                Location = new Point(20, yPosition),
                Size = new Size(labelWidth, 20)
            };
            txtPartnerName = new TextBox
            {
                Location = new Point(150, yPosition),
                Size = new Size(textBoxWidth, 25)
            };

            yPosition += spacing;

            // Тип партнера
            var lblPartnerType = new Label
            {
                Text = "Тип партнера:",
                Location = new Point(20, yPosition),
                Size = new Size(labelWidth, 20)
            };
            cmbPartnerType = new ComboBox
            {
                Location = new Point(150, yPosition),
                Size = new Size(textBoxWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            yPosition += spacing;

            // Руководитель
            var lblDirector = new Label
            {
                Text = "Руководитель:",
                Location = new Point(20, yPosition),
                Size = new Size(labelWidth, 20)
            };
            txtDirector = new TextBox
            {
                Location = new Point(150, yPosition),
                Size = new Size(textBoxWidth, 25)
            };

            yPosition += spacing;

            // Email
            var lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(20, yPosition),
                Size = new Size(labelWidth, 20)
            };
            txtEmail = new TextBox
            {
                Location = new Point(150, yPosition),
                Size = new Size(textBoxWidth, 25)
            };

            yPosition += spacing;

            // Телефон
            var lblPhone = new Label
            {
                Text = "Телефон:",
                Location = new Point(20, yPosition),
                Size = new Size(labelWidth, 20)
            };
            txtPhone = new TextBox
            {
                Location = new Point(150, yPosition),
                Size = new Size(textBoxWidth, 25)
            };

            yPosition += spacing;

            // Адрес
            var lblAddress = new Label
            {
                Text = "Адрес:",
                Location = new Point(20, yPosition),
                Size = new Size(labelWidth, 20)
            };
            txtAddress = new TextBox
            {
                Location = new Point(150, yPosition),
                Size = new Size(textBoxWidth, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            yPosition += 80;

            // ИНН
            var lblINN = new Label
            {
                Text = "ИНН:",
                Location = new Point(20, yPosition),
                Size = new Size(labelWidth, 20)
            };
            txtINN = new TextBox
            {
                Location = new Point(150, yPosition),
                Size = new Size(textBoxWidth, 25),
                MaxLength = 12
            };

            yPosition += spacing;

            // Рейтинг
            var lblRating = new Label
            {
                Text = "Рейтинг:",
                Location = new Point(20, yPosition),
                Size = new Size(labelWidth, 20)
            };
            nudRating = new NumericUpDown
            {
                Location = new Point(150, yPosition),
                Size = new Size(100, 25),
                Minimum = 0,
                Maximum = 10,
                Value = 5
            };

            yPosition += 60;

            // Кнопки
            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(150, yPosition),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 206, 209),
                ForeColor = Color.White,
                Font = new Font("Franklin Gothic Medium", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(270, yPosition),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(224, 255, 255),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += BtnCancel_Click;

            // Добавление контролов на форму
            this.Controls.AddRange(new Control[]
            {
                lblPartnerName, txtPartnerName,
                lblPartnerType, cmbPartnerType,
                lblDirector, txtDirector,
                lblEmail, txtEmail,
                lblPhone, txtPhone,
                lblAddress, txtAddress,
                lblINN, txtINN,
                lblRating, nudRating,
                btnSave, btnCancel
            });
        }

        /// <summary>
        /// Загрузка типов партнеров в ComboBox
        /// </summary>
        private void LoadPartnerTypes()
        {
            cmbPartnerType.Items.AddRange(new string[]
            {
                "ИП", "ООО", "ЗАО", "АО", "ПАО"
            });

            if (cmbPartnerType.Items.Count > 0)
                cmbPartnerType.SelectedIndex = 0;
        }

        /// <summary>
        /// Загрузка данных партнера для редактирования
        /// </summary>
        private void LoadPartnerData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT [Наименование партнера], [Тип партнера], [Руководитель],
                               [Электронная почта партнера], [Телефон партнера],
                               [Юридический адрес партнера], [ИНН], [Рейтинг]
                        FROM Partners_import 
                        WHERE PartnerId = @PartnerId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PartnerId", partnerId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtPartnerName.Text = reader["Наименование партнера"].ToString();
                                cmbPartnerType.Text = reader["Тип партнера"].ToString();
                                txtDirector.Text = reader["Руководитель"].ToString();
                                txtEmail.Text = reader["Электронная почта партнера"].ToString();
                                txtPhone.Text = reader["Телефон партнера"].ToString();
                                txtAddress.Text = reader["Юридический адрес партнера"].ToString();
                                txtINN.Text = reader["ИНН"].ToString();
                                nudRating.Value = Convert.ToDecimal(reader["Рейтинг"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных партнера: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query;

                    if (partnerId.HasValue)
                    {
                        // Обновление существующего партнера
                        query = @"
                            UPDATE Partners_import SET
                                [Наименование партнера] = @PartnerName,
                                [Тип партнера] = @PartnerType,
                                [Руководитель] = @Director,
                                [Электронная почта партнера] = @Email,
                                [Телефон партнера] = @Phone,
                                [Юридический адрес партнера] = @Address,
                                [ИНН] = @INN,
                                [Рейтинг] = @Rating,
                                ModifiedDate = GETDATE()
                            WHERE PartnerId = @PartnerId";
                    }
                    else
                    {
                        // Добавление нового партнера
                        query = @"
                            INSERT INTO Partners_import 
                            ([Наименование партнера], [Тип партнера], [Руководитель],
                             [Электронная почта партнера], [Телефон партнера], 
                             [Юридический адрес партнера], [ИНН], [Рейтинг])
                            VALUES 
                            (@PartnerName, @PartnerType, @Director, @Email, 
                             @Phone, @Address, @INN, @Rating)";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PartnerName", txtPartnerName.Text.Trim());
                        command.Parameters.AddWithValue("@PartnerType", cmbPartnerType.Text);
                        command.Parameters.AddWithValue("@Director", txtDirector.Text.Trim());
                        command.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        command.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                        command.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                        command.Parameters.AddWithValue("@INN", txtINN.Text.Trim());
                        command.Parameters.AddWithValue("@Rating", nudRating.Value);

                        if (partnerId.HasValue)
                        {
                            command.Parameters.AddWithValue("@PartnerId", partnerId);
                        }

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные партнера успешно сохранены!",
                                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось сохранить данные партнера.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627) // Нарушение уникальности
            {
                if (ex.Message.Contains("INN"))
                {
                    MessageBox.Show("Партнер с таким ИНН уже существует. Пожалуйста, проверьте корректность данных.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Партнер с таким наименованием уже существует. Пожалуйста, введите другое наименование.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Валидация введенных данных
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtPartnerName.Text))
            {
                MessageBox.Show("Пожалуйста, введите наименование партнера.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPartnerName.Focus();
                return false;
            }

            if (cmbPartnerType.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите тип партнера.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbPartnerType.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDirector.Text))
            {
                MessageBox.Show("Пожалуйста, введите ФИО руководителя.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDirector.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный email адрес.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Пожалуйста, введите телефон партнера.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtINN.Text) || txtINN.Text.Length < 10)
            {
                MessageBox.Show("Пожалуйста, введите корректный ИНН (минимум 10 символов).",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtINN.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверка валидности email адреса
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
