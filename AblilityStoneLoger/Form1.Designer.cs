namespace AblilityStoneLoger
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.not_supported_text = new System.Windows.Forms.Label();
            this.MousePos = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.engraving1 = new System.Windows.Forms.Label();
            this.engraving2 = new System.Windows.Forms.Label();
            this.engraving3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(24, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "프로세스 탐지중";
            // 
            // not_supported_text
            // 
            this.not_supported_text.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.not_supported_text.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.not_supported_text.Location = new System.Drawing.Point(1, -4);
            this.not_supported_text.Name = "not_supported_text";
            this.not_supported_text.Size = new System.Drawing.Size(426, 235);
            this.not_supported_text.TabIndex = 3;
            this.not_supported_text.Text = "해당 프로그램은 21:9 , 16:9 해상도를 지원합니다.";
            this.not_supported_text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.not_supported_text.Visible = false;
            // 
            // MousePos
            // 
            this.MousePos.AutoSize = true;
            this.MousePos.Location = new System.Drawing.Point(24, 54);
            this.MousePos.Name = "MousePos";
            this.MousePos.Size = new System.Drawing.Size(71, 15);
            this.MousePos.TabIndex = 4;
            this.MousePos.Text = "마우스 좌표";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "퍼센트";
            // 
            // engraving1
            // 
            this.engraving1.AutoSize = true;
            this.engraving1.Location = new System.Drawing.Point(24, 114);
            this.engraving1.Name = "engraving1";
            this.engraving1.Size = new System.Drawing.Size(38, 15);
            this.engraving1.TabIndex = 6;
            this.engraving1.Text = "각인1";
            // 
            // engraving2
            // 
            this.engraving2.AutoSize = true;
            this.engraving2.Location = new System.Drawing.Point(24, 145);
            this.engraving2.Name = "engraving2";
            this.engraving2.Size = new System.Drawing.Size(38, 15);
            this.engraving2.TabIndex = 6;
            this.engraving2.Text = "각인2";
            // 
            // engraving3
            // 
            this.engraving3.AutoSize = true;
            this.engraving3.Location = new System.Drawing.Point(24, 178);
            this.engraving3.Name = "engraving3";
            this.engraving3.Size = new System.Drawing.Size(38, 15);
            this.engraving3.TabIndex = 6;
            this.engraving3.Text = "각인3";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 227);
            this.Controls.Add(this.engraving3);
            this.Controls.Add(this.engraving2);
            this.Controls.Add(this.engraving1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MousePos);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.not_supported_text);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label label1;
        private Label not_supported_text;
        private Label MousePos;
        private Label label2;
        private Label engraving1;
        private Label engraving2;
        private Label engraving3;
    }
}