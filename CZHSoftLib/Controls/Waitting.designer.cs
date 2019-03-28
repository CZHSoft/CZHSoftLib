namespace CZHSoft.Controls
{
    partial class Waitting
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
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.loadingCircle = new LTControls.LoadingCircle();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(107, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "正在操作,请稍候...";
            // 
            // loadingCircle
            // 
            this.loadingCircle.Active = false;
            this.loadingCircle.Color = System.Drawing.Color.Green;
            this.loadingCircle.InnerCircleRadius = 5;
            this.loadingCircle.Location = new System.Drawing.Point(4, 7);
            this.loadingCircle.Name = "loadingCircle";
            this.loadingCircle.NumberSpoke = 12;
            this.loadingCircle.OuterCircleRadius = 20;
            this.loadingCircle.RotationSpeed = 100;
            this.loadingCircle.Size = new System.Drawing.Size(80, 40);
            this.loadingCircle.SpokeThickness = 2;
            this.loadingCircle.StylePreset = LTControls.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircle.TabIndex = 0;
            // 
            // Waitting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 54);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.loadingCircle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Waitting";
            this.Opacity = 0.85;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "请稍等";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Waitting_Load);
            this.DoubleClick += new System.EventHandler(this.Waitting_DoubleClick);
            this.Click += new System.EventHandler(this.Waitting_Click);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Waitting_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LTControls.LoadingCircle loadingCircle;
        private System.Windows.Forms.Label label1;
    }
}