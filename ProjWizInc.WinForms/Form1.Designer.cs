namespace ProjWizInc.WinForms
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
        private void InitializeComponent() {
            button1 = new Button();
            labelGold = new Label();
            barProgress = new ProgressBar();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(498, 285);
            button1.Name = "button1";
            button1.Size = new Size(77, 43);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // labelGold
            // 
            labelGold.AutoSize = true;
            labelGold.Location = new Point(467, 192);
            labelGold.Name = "labelGold";
            labelGold.Size = new Size(56, 20);
            labelGold.TabIndex = 1;
            labelGold.Text = "Gold: 0";
            labelGold.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // barProgress
            // 
            barProgress.Location = new Point(451, 361);
            barProgress.Name = "barProgress";
            barProgress.Size = new Size(222, 44);
            barProgress.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(barProgress);
            Controls.Add(labelGold);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label labelGold;
        private ProgressBar barProgress;
    }
}
