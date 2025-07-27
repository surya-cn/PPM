namespace PowerPlanViewer
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btnUltimate;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnActivate = new System.Windows.Forms.Button();
            this.btnUltimate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // listBox1
            //
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(550, 199);
            this.listBox1.TabIndex = 0;
            //
            // btnActivate
            //
            this.btnActivate.Location = new System.Drawing.Point(12, 230);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(170, 30);
            this.btnActivate.TabIndex = 1;
            this.btnActivate.Text = "Activate Selected Plan";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            //
            // btnUltimate
            //
            this.btnUltimate.Location = new System.Drawing.Point(200, 230);
            this.btnUltimate.Name = "btnUltimate";
            this.btnUltimate.Size = new System.Drawing.Size(200, 30);
            this.btnUltimate.TabIndex = 2;
            this.btnUltimate.Text = "Enable Ultimate Performance";
            this.btnUltimate.UseVisualStyleBackColor = true;
            this.btnUltimate.Click += new System.EventHandler(this.btnUltimate_Click);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(574, 281);
            this.Controls.Add(this.btnUltimate);
            this.Controls.Add(this.btnActivate);
            this.Controls.Add(this.listBox1);
            this.Name = "MainForm";
            this.Text = "Power Plan Manager";
            this.ResumeLayout(false);
        }
    }
}
