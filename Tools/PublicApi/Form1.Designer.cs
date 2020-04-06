namespace PublicApi
{
  partial class Form1
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
      this.tboAssembly = new System.Windows.Forms.TextBox();
      this.tboApi = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cmdLoad = new System.Windows.Forms.Button();
      this.cmdSave = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // tboAssembly
      // 
      this.tboAssembly.BackColor = System.Drawing.Color.White;
      this.tboAssembly.Location = new System.Drawing.Point(69, 12);
      this.tboAssembly.Name = "tboAssembly";
      this.tboAssembly.ReadOnly = true;
      this.tboAssembly.Size = new System.Drawing.Size(536, 20);
      this.tboAssembly.TabIndex = 0;
      // 
      // tboApi
      // 
      this.tboApi.BackColor = System.Drawing.Color.White;
      this.tboApi.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tboApi.Location = new System.Drawing.Point(12, 39);
      this.tboApi.Multiline = true;
      this.tboApi.Name = "tboApi";
      this.tboApi.ReadOnly = true;
      this.tboApi.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.tboApi.Size = new System.Drawing.Size(651, 384);
      this.tboApi.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(51, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Assembly";
      // 
      // cmdLoad
      // 
      this.cmdLoad.Location = new System.Drawing.Point(611, 10);
      this.cmdLoad.Name = "cmdLoad";
      this.cmdLoad.Size = new System.Drawing.Size(52, 23);
      this.cmdLoad.TabIndex = 3;
      this.cmdLoad.Text = "Load";
      this.cmdLoad.UseVisualStyleBackColor = true;
      this.cmdLoad.Click += new System.EventHandler(this.cmdLoad_Click);
      // 
      // cmdSave
      // 
      this.cmdSave.Enabled = false;
      this.cmdSave.Location = new System.Drawing.Point(12, 430);
      this.cmdSave.Name = "cmdSave";
      this.cmdSave.Size = new System.Drawing.Size(51, 23);
      this.cmdSave.TabIndex = 4;
      this.cmdSave.Text = "Save";
      this.cmdSave.UseVisualStyleBackColor = true;
      this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(675, 456);
      this.Controls.Add(this.cmdSave);
      this.Controls.Add(this.cmdLoad);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.tboApi);
      this.Controls.Add(this.tboAssembly);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.Resize += new System.EventHandler(this.Form1_Resize);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox tboAssembly;
    private System.Windows.Forms.TextBox tboApi;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button cmdLoad;
    private System.Windows.Forms.Button cmdSave;
  }
}

