namespace CZHSoft.Controls
{
    partial class OperateBar
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OperateBar));
            this.tsBar = new System.Windows.Forms.ToolStrip();
            this.tsbAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbMod = new System.Windows.Forms.ToolStripButton();
            this.tsbDel = new System.Windows.Forms.ToolStripButton();
            this.tsbCancel = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsbAudit = new System.Windows.Forms.ToolStripButton();
            this.tsbAbandon = new System.Windows.Forms.ToolStripButton();
            this.tsbNext = new System.Windows.Forms.ToolStripButton();
            this.tsbLast = new System.Windows.Forms.ToolStripButton();
            this.tslRecord = new System.Windows.Forms.ToolStripLabel();
            this.tsbEnable = new System.Windows.Forms.ToolStripButton();
            this.tsbDisable = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tstbQuery = new System.Windows.Forms.ToolStripTextBox();
            this.tsbQuery = new System.Windows.Forms.ToolStripButton();
            this.tsBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsBar
            // 
            this.tsBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAdd,
            this.tsbMod,
            this.tsbDel,
            this.tsbCancel,
            this.tsbSave,
            this.tsbAudit,
            this.tsbAbandon,
            this.tsbLast,
            this.tsbNext,
            this.tslRecord,
            this.tsbEnable,
            this.tsbDisable,
            this.toolStripSeparator1,
            this.tstbQuery,
            this.tsbQuery});
            this.tsBar.Location = new System.Drawing.Point(0, 0);
            this.tsBar.Name = "tsBar";
            this.tsBar.Size = new System.Drawing.Size(619, 35);
            this.tsBar.TabIndex = 0;
            this.tsBar.Text = "Bar";
            // 
            // tsbAdd
            // 
            this.tsbAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsbAdd.Image")));
            this.tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAdd.Name = "tsbAdd";
            this.tsbAdd.Size = new System.Drawing.Size(33, 32);
            this.tsbAdd.Text = "新建";
            this.tsbAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbMod
            // 
            this.tsbMod.Image = ((System.Drawing.Image)(resources.GetObject("tsbMod.Image")));
            this.tsbMod.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMod.Name = "tsbMod";
            this.tsbMod.Size = new System.Drawing.Size(33, 32);
            this.tsbMod.Text = "修改";
            this.tsbMod.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbMod.Click += new System.EventHandler(this.tsbMod_Click);
            // 
            // tsbDel
            // 
            this.tsbDel.Image = ((System.Drawing.Image)(resources.GetObject("tsbDel.Image")));
            this.tsbDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDel.Name = "tsbDel";
            this.tsbDel.Size = new System.Drawing.Size(33, 32);
            this.tsbDel.Text = "删除";
            this.tsbDel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbDel.Click += new System.EventHandler(this.tsbDel_Click);
            // 
            // tsbCancel
            // 
            this.tsbCancel.Image = ((System.Drawing.Image)(resources.GetObject("tsbCancel.Image")));
            this.tsbCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCancel.Name = "tsbCancel";
            this.tsbCancel.Size = new System.Drawing.Size(33, 32);
            this.tsbCancel.Text = "取消";
            this.tsbCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbCancel.Click += new System.EventHandler(this.tsbCancel_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(33, 32);
            this.tsbSave.Text = "保存";
            this.tsbSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // tsbAudit
            // 
            this.tsbAudit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbAudit.Image = ((System.Drawing.Image)(resources.GetObject("tsbAudit.Image")));
            this.tsbAudit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAudit.Name = "tsbAudit";
            this.tsbAudit.Size = new System.Drawing.Size(33, 32);
            this.tsbAudit.Text = "审核";
            this.tsbAudit.Click += new System.EventHandler(this.tsbAudit_Click);
            // 
            // tsbAbandon
            // 
            this.tsbAbandon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbAbandon.Image = ((System.Drawing.Image)(resources.GetObject("tsbAbandon.Image")));
            this.tsbAbandon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAbandon.Name = "tsbAbandon";
            this.tsbAbandon.Size = new System.Drawing.Size(33, 32);
            this.tsbAbandon.Text = "弃审";
            this.tsbAbandon.Click += new System.EventHandler(this.tsbAbandon_Click);
            // 
            // tsbNext
            // 
            this.tsbNext.Image = ((System.Drawing.Image)(resources.GetObject("tsbNext.Image")));
            this.tsbNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNext.Name = "tsbNext";
            this.tsbNext.Size = new System.Drawing.Size(45, 32);
            this.tsbNext.Text = "下一页";
            this.tsbNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbNext.Click += new System.EventHandler(this.tsbNext_Click);
            // 
            // tsbLast
            // 
            this.tsbLast.Image = ((System.Drawing.Image)(resources.GetObject("tsbLast.Image")));
            this.tsbLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLast.Name = "tsbLast";
            this.tsbLast.Size = new System.Drawing.Size(45, 32);
            this.tsbLast.Text = "上一页";
            this.tsbLast.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbLast.Click += new System.EventHandler(this.tsbLast_Click);
            // 
            // tslRecord
            // 
            this.tslRecord.Name = "tslRecord";
            this.tslRecord.Size = new System.Drawing.Size(23, 32);
            this.tslRecord.Text = "0/0";
            // 
            // tsbEnable
            // 
            this.tsbEnable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbEnable.Image = ((System.Drawing.Image)(resources.GetObject("tsbEnable.Image")));
            this.tsbEnable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEnable.Name = "tsbEnable";
            this.tsbEnable.Size = new System.Drawing.Size(33, 32);
            this.tsbEnable.Text = "启用";
            this.tsbEnable.Click += new System.EventHandler(this.tsbEnable_Click);
            // 
            // tsbDisable
            // 
            this.tsbDisable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbDisable.Image = ((System.Drawing.Image)(resources.GetObject("tsbDisable.Image")));
            this.tsbDisable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDisable.Name = "tsbDisable";
            this.tsbDisable.Size = new System.Drawing.Size(33, 32);
            this.tsbDisable.Text = "禁用";
            this.tsbDisable.Click += new System.EventHandler(this.tsbDisable_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // tstbQuery
            // 
            this.tstbQuery.Name = "tstbQuery";
            this.tstbQuery.Size = new System.Drawing.Size(100, 35);
            // 
            // tsbQuery
            // 
            this.tsbQuery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbQuery.Image = ((System.Drawing.Image)(resources.GetObject("tsbQuery.Image")));
            this.tsbQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbQuery.Name = "tsbQuery";
            this.tsbQuery.Size = new System.Drawing.Size(33, 32);
            this.tsbQuery.Text = "查询";
            // 
            // OperateBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tsBar);
            this.Name = "OperateBar";
            this.Size = new System.Drawing.Size(619, 36);
            this.tsBar.ResumeLayout(false);
            this.tsBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsBar;
        private System.Windows.Forms.ToolStripButton tsbAdd;
        private System.Windows.Forms.ToolStripButton tsbMod;
        private System.Windows.Forms.ToolStripButton tsbDel;
        private System.Windows.Forms.ToolStripButton tsbCancel;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripButton tsbNext;
        private System.Windows.Forms.ToolStripButton tsbLast;
        private System.Windows.Forms.ToolStripButton tsbEnable;
        private System.Windows.Forms.ToolStripButton tsbDisable;
        private System.Windows.Forms.ToolStripButton tsbAudit;
        private System.Windows.Forms.ToolStripButton tsbAbandon;
        private System.Windows.Forms.ToolStripLabel tslRecord;
        private System.Windows.Forms.ToolStripTextBox tstbQuery;
        private System.Windows.Forms.ToolStripButton tsbQuery;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
