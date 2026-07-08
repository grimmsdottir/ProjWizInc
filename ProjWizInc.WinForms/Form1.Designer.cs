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
            labelClickCount = new Label();
            buttonClickMe = new Button();
            SuspendLayout();
            // 
            // labelClickCount
            // 
            labelClickCount.AutoSize = true;
            labelClickCount.Location = new Point(86, 135);
            labelClickCount.Name = "labelClickCount";
            labelClickCount.Size = new Size(61, 20);
            labelClickCount.TabIndex = 4;
            labelClickCount.Text = "Clicks: 0";
            // 
            // buttonClickMe
            // 
            buttonClickMe.Location = new Point(91, 192);
            buttonClickMe.Name = "buttonClickMe";
            buttonClickMe.Size = new Size(82, 46);
            buttonClickMe.TabIndex = 5;
            buttonClickMe.Text = "Click Me";
            buttonClickMe.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonClickMe);
            Controls.Add(labelClickCount);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label labelGold;
        private ProgressBar barProgress;
        private Button button2;
        private Label labelClickCount;
        private Button buttonClickMe;
    }
}
