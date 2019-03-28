using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CZHSoft.Controls
{
    /// <summary>
    /// 使用步骤
    /// 1.BtnEnabledControl
    /// 2.BtnVisibleControl
    /// 3.注入操作时间
    /// 4.更改action标记
    /// 5.UIValueChange
    /// </summary>
    public partial class OperateBar : UserControl
    {
        public Action action = Action.Qry;

        private int total = 0;
        private int current = 0;

        public delegate void OperateBarDelegate();

        public event OperateBarDelegate OnTsbAdd;
        public event OperateBarDelegate OnTsbMod;
        public event OperateBarDelegate OnTsbDel;
        public event OperateBarDelegate OnTsbSave;
        public event OperateBarDelegate OnTsbCancel;
        public event OperateBarDelegate OnTsbNext;
        public event OperateBarDelegate OnTsbLast;
        public event OperateBarDelegate OnTsbAudit;
        public event OperateBarDelegate OnTsbAbandon;
        public event OperateBarDelegate OnTsbEnable;
        public event OperateBarDelegate OnTsbDisable;

        public OperateBar()
        {
            InitializeComponent();
        }

        public void BtnEnabledControl(bool add,bool mod,bool del,bool cancel,bool save,
            bool audit,bool abandon,bool next,bool last,bool enable,bool disable)
        {
            tsbAdd.Enabled = add;
            tsbMod.Enabled = mod;
            tsbDel.Enabled = del;
            tsbCancel.Enabled = cancel;
            tsbSave.Enabled = save;
            tsbAudit.Enabled = audit;
            tsbAbandon.Enabled = abandon;
            tsbNext.Enabled = next;
            tsbLast.Enabled = last;
            tsbEnable.Enabled = enable;
            tsbDisable.Enabled = disable;
        }

        public void BtnVisibleControl(bool add, bool mod, bool del, bool cancel, bool save,
            bool audit, bool abandon, bool next, bool last, bool enable, bool disable)
        {
            tsbAdd.Visible = add;
            tsbMod.Visible = mod;
            tsbDel.Visible = del;
            tsbCancel.Visible = cancel;
            tsbSave.Visible = save;
            tsbAudit.Visible = audit;
            tsbAbandon.Visible = abandon;
            tsbNext.Visible = next;
            tsbLast.Visible = last;
            tsbEnable.Visible = enable;
            tsbDisable.Visible = disable;

            tslRecord.Visible = next;
        }

        public void SetTotal(int count)
        {
            if (count  == 10)
            {
                total = 1;
            }
            else if (count / 10 > 0)
            {
                total = count / 10 + 1;
            }
            else if (count / 10 == 0 && count > 0)
            {
                total = 1;
            }
            else if (count == 0)
            {
                total = 0;
            }
        }

        public int GetTotal()
        {
            return total;
        }

        public int GetCurrent()
        {
            return current;
        }

        public void SetTslRecord(int now, int count)
        {
            if (now <= count)
            {
                current = now;
                tslRecord.Text = now.ToString() + "/" + count.ToString();
            }
        }

        public void SetTsbAddText(string text)
        {
            tsbAdd.Text = text;
        }
        public void SetTsbModText(string text)
        {

            tsbMod.Text = text;
        }
        public void SetTsbDelText(string text)
        {

            tsbDel.Text = text;
        }
        public void SetTsbCancelText(string text)
        {

            tsbCancel.Text = text;
        }
        public void SetTsbSaveText(string text)
        {

            tsbSave.Text = text;
        }
        public void SetTsbAuditText(string text)
        {
      
            tsbAudit.Text = text;
        }
        public void SetTsbAbandonText(string text)
        {

            tsbAbandon.Text = text;
        }
        public void SetTsbNexText(string text)
        {

            tsbNext.Text = text;
        }
        public void SetTsbLastText(string text)
        {

            tsbLast.Text = text;
        }
        public void SetTsbEnableText(string text)
        {

            tsbEnable.Text = text;
        }
        public void SetTsbDisableText(string text)
        {

            tsbDisable.Text = text;
        }
        public void SetTsbQueryText(string text)
        {

            tsbQuery.Text = text;
        }
        
        public void UIValueChange()
        {
            switch (action)
            {
                case Action.Qry:
                    tsbAdd.Enabled = true;
                    tsbDel.Enabled = true;
                    tsbMod.Enabled = true;
                    tsbSave.Enabled = false;
                    tsbCancel.Enabled = false;
                    tsbAudit.Enabled = true;
                    tsbAbandon.Enabled = true;
                    tsbNext.Enabled = true;
                    tsbLast.Enabled = true;
                    tsbEnable.Enabled = true;
                    tsbDisable.Enabled = true;
                    break;
                case Action.Add:
                    tsbAdd.Enabled = false;
                    tsbDel.Enabled = false;
                    tsbMod.Enabled = false;
                    tsbSave.Enabled = true;
                    tsbCancel.Enabled = true;
                    tsbAudit.Enabled = false;
                    tsbAbandon.Enabled = false;
                    tsbNext.Enabled = false;
                    tsbLast.Enabled = false;
                    tsbEnable.Enabled = false;
                    tsbDisable.Enabled = false;
                    break;
                case Action.Del:
                    tsbAdd.Enabled = false;
                    tsbDel.Enabled = false;
                    tsbMod.Enabled = false;
                    tsbSave.Enabled = true;
                    tsbCancel.Enabled = true;
                    tsbAudit.Enabled = false;
                    tsbAbandon.Enabled = false;
                    tsbNext.Enabled = false;
                    tsbLast.Enabled = false;
                    tsbEnable.Enabled = false;
                    tsbDisable.Enabled = false;
                    break;
                case Action.Mod:
                    tsbAdd.Enabled = false;
                    tsbDel.Enabled = false;
                    tsbMod.Enabled = false;
                    tsbSave.Enabled = true;
                    tsbCancel.Enabled = true;
                    tsbAudit.Enabled = false;
                    tsbAbandon.Enabled = false;
                    tsbNext.Enabled = false;
                    tsbLast.Enabled = false;
                    tsbEnable.Enabled = false;
                    tsbDisable.Enabled = false;
                    break;
                default:
                    return;
            }
        }

        #region 单击
        private void tsbAdd_Click(object sender, EventArgs e)
        {
            if (OnTsbAdd != null)
            {
                OnTsbAdd();
            }
        }

        private void tsbMod_Click(object sender, EventArgs e)
        {
            if (OnTsbMod != null)
            {
                OnTsbMod();
            }
        }

        private void tsbDel_Click(object sender, EventArgs e)
        {
            if (OnTsbDel != null)
            {
                OnTsbDel();
            }
        }

        private void tsbCancel_Click(object sender, EventArgs e)
        {
            if (OnTsbCancel != null)
            {
                OnTsbCancel();
            }
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            if (OnTsbSave != null)
            {
                OnTsbSave();
            }
        }

        private void tsbNext_Click(object sender, EventArgs e)
        {
            if (current < total)
            {
                if (OnTsbNext != null)
                {
                    OnTsbNext();
                }
            }
        }

        private void tsbLast_Click(object sender, EventArgs e)
        {
            if (current != 0)
            {
                if (OnTsbLast != null)
                {
                    OnTsbLast();
                }
            }
        }

        private void tsbAudit_Click(object sender, EventArgs e)
        {
            if (OnTsbAudit != null)
            {
                OnTsbAudit();
            }
        }

        private void tsbAbandon_Click(object sender, EventArgs e)
        {
            if (OnTsbAbandon != null)
            {
                OnTsbAbandon();
            }
        }

        private void tsbEnable_Click(object sender, EventArgs e)
        {
            if (OnTsbEnable!=null)
            {
                OnTsbEnable();
            }
        }

        private void tsbDisable_Click(object sender, EventArgs e)
        {
            if (OnTsbDisable != null)
            {
                OnTsbDisable();
            }
        }
        #endregion
    }

    public enum Action
    {
        Qry,
        Mod,
        Add,
        Del,
        Cancel,
        Save,
        Next,
        Last
    }
}
