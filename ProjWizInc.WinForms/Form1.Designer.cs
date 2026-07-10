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
            labelTimer = new Label();
            button1 = new Button();
            button2 = new Button();
            progressBar1 = new ProgressBar();
            progressBar2 = new ProgressBar();
            labelGold = new Label();
            SuspendLayout();
            // 
            // labelTimer
            // 
            labelTimer.AutoSize = true;
            labelTimer.Location = new Point(67, 40);
            labelTimer.Name = "labelTimer";
            labelTimer.Size = new Size(57, 20);
            labelTimer.TabIndex = 0;
            labelTimer.Text = "Time: 0";
            // 
            // button1
            // 
            button1.Location = new Point(82, 160);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 1;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(82, 195);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 2;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(182, 160);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(125, 29);
            progressBar1.TabIndex = 3;
            // 
            // progressBar2
            // 
            progressBar2.Location = new Point(182, 195);
            progressBar2.Name = "progressBar2";
            progressBar2.Size = new Size(125, 29);
            progressBar2.TabIndex = 4;
            // 
            // labelGold
            // 
            labelGold.AutoSize = true;
            labelGold.Location = new Point(67, 60);
            labelGold.Name = "labelGold";
            labelGold.Size = new Size(44, 20);
            labelGold.TabIndex = 5;
            labelGold.Text = "Gold:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(labelGold);
            Controls.Add(progressBar2);
            Controls.Add(progressBar1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(labelTimer);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelTimer;
        private Button button1;
        private Button button2;
        private ProgressBar progressBar1;
        private ProgressBar progressBar2;
        private Label labelGold;
    }
}
