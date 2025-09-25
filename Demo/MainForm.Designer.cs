using System.Drawing;
using System.Windows.Forms;

namespace Demo
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        //private void InitializeComponent()
        //{
        //    this.components = new System.ComponentModel.Container();
        //    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    this.ClientSize = new System.Drawing.Size(800, 450);
        //    this.Text = "MainForm";
        //}
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Настройка формы согласно руководству по стилю
            this.Text = "Система управления партнерами - Чистая Планета";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White; // Основной фон белый
            this.Font = new Font("Franklin Gothic Medium", 10F); // Шрифт Franklin Gothic Medium

            this.Icon = new Icon("H:\\Demo/Demo/Resources/Иконка.ico");


            this.ResumeLayout(false);
        }
        private void InitializeCustomComponents()
        {
            // Создание и настройка логотипа
            pbLogo = new PictureBox
            {
                Location = new Point(20, 20),
                Size = new Size(150, 80),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            try
            {
                pbLogo.Image = Image.FromFile("H:\\Demo\\Demo\\Resources/лого.jpg");
            }
            catch
            {
                // Если логотип не найден, создаём текстовую заглушку
                pbLogo.BackColor = Color.FromArgb(224, 255, 255); // #E0FFFF дополнительный фон
            }

            // Заголовок
            lblTitle = new Label
            {
                Text = "Управление партнерами",
                Font = new Font("Franklin Gothic Medium", 18F, FontStyle.Bold),
                Location = new Point(200, 40),
                Size = new Size(400, 40),
                ForeColor = Color.FromArgb(0, 206, 209) // #00CED1 акцентирование
            };

            // DataGridView для отображения партнеров
            dgvPartners = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(1000, 400),
                BackgroundColor = Color.FromArgb(224, 255, 255), // #E0FFFF дополнительный фон
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvPartners.CellDoubleClick += DgvPartners_CellDoubleClick;

            // Кнопка добавления партнера
            btnAddPartner = new Button
            {
                Text = "Добавить партнера",
                Location = new Point(20, 540),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(0, 206, 209), // #00CED1 акцентирование
                ForeColor = Color.White,
                Font = new Font("Franklin Gothic Medium", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnAddPartner.Click += BtnAddPartner_Click;

            // Кнопка редактирования партнера
            btnEditPartner = new Button
            {
                Text = "Редактировать",
                Location = new Point(240, 540),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(0, 206, 209),
                ForeColor = Color.White,
                Font = new Font("Franklin Gothic Medium", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnEditPartner.Click += BtnEditPartner_Click;

            // Кнопка просмотра истории услуг
            btnViewHistory = new Button
            {
                Text = "История услуг",
                Location = new Point(460, 540),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(0, 206, 209),
                ForeColor = Color.White,
                Font = new Font("Franklin Gothic Medium", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnViewHistory.Click += BtnViewHistory_Click;

            // Добавление контролов на форму
            this.Controls.AddRange(new Control[]
            {
                pbLogo, lblTitle, dgvPartners,
                btnAddPartner, btnEditPartner, btnViewHistory
            });
        }

        #endregion
    }
}