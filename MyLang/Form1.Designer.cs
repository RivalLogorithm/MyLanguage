namespace MyLang
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Start = new System.Windows.Forms.Button();
            this.tb_Program = new System.Windows.Forms.RichTextBox();
            this.tb_Example = new System.Windows.Forms.RichTextBox();
            this.tb_Strings = new System.Windows.Forms.RichTextBox();
            this.tb_Debug = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(410, 3);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(75, 23);
            this.btn_Start.TabIndex = 3;
            this.btn_Start.Text = "Выполнить";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // tb_Program
            // 
            this.tb_Program.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_Program.Location = new System.Drawing.Point(77, 32);
            this.tb_Program.Name = "tb_Program";
            this.tb_Program.Size = new System.Drawing.Size(368, 295);
            this.tb_Program.TabIndex = 5;
            this.tb_Program.Text = "";
            this.tb_Program.WordWrap = false;
            this.tb_Program.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.tb_Program_ContentsResized);
            this.tb_Program.VScroll += new System.EventHandler(this.tb_Program_VScroll);
            this.tb_Program.TextChanged += new System.EventHandler(this.tb_Program_TextChanged_1);
            this.tb_Program.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tb_Program_MouseDown);
            // 
            // tb_Example
            // 
            this.tb_Example.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tb_Example.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_Example.Location = new System.Drawing.Point(451, 32);
            this.tb_Example.Name = "tb_Example";
            this.tb_Example.ReadOnly = true;
            this.tb_Example.Size = new System.Drawing.Size(363, 295);
            this.tb_Example.TabIndex = 6;
            this.tb_Example.Text = "";
            // 
            // tb_Strings
            // 
            this.tb_Strings.BackColor = System.Drawing.SystemColors.Window;
            this.tb_Strings.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_Strings.Location = new System.Drawing.Point(12, 32);
            this.tb_Strings.Name = "tb_Strings";
            this.tb_Strings.ReadOnly = true;
            this.tb_Strings.Size = new System.Drawing.Size(42, 295);
            this.tb_Strings.TabIndex = 7;
            this.tb_Strings.Text = "";
            // 
            // tb_Debug
            // 
            this.tb_Debug.BackColor = System.Drawing.SystemColors.Window;
            this.tb_Debug.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_Debug.Location = new System.Drawing.Point(77, 345);
            this.tb_Debug.Name = "tb_Debug";
            this.tb_Debug.ReadOnly = true;
            this.tb_Debug.Size = new System.Drawing.Size(737, 96);
            this.tb_Debug.TabIndex = 8;
            this.tb_Debug.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 484);
            this.Controls.Add(this.tb_Debug);
            this.Controls.Add(this.tb_Strings);
            this.Controls.Add(this.tb_Example);
            this.Controls.Add(this.tb_Program);
            this.Controls.Add(this.btn_Start);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.RichTextBox tb_Program;
        private System.Windows.Forms.RichTextBox tb_Example;
        private System.Windows.Forms.RichTextBox tb_Strings;
        private System.Windows.Forms.RichTextBox tb_Debug;
    }
}

