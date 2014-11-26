// ----------------------------------------------------------------------------
// <copyright file="DistributionEditor.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using HIVacSim.Utility;

    /// <summary>
    /// GUI to help the definition of probability distributions
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("Value")]	
    public class DistributionEditor : System.Windows.Forms.UserControl
    {
        private Stochastic _retValue;
        private EventHandler onValueChanged;
        private System.Windows.Forms.TextBox txtParameter1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.TextBox txtParameter2;
        private System.Windows.Forms.TextBox txtParameter3;
        private System.Windows.Forms.TextBox txtParameter4;
        private System.Windows.Forms.ComboBox cmbDistribution;
        private System.Windows.Forms.Label lblParameter1;
        private System.Windows.Forms.Label lblParameter2;
        private System.Windows.Forms.Label lblParameter3;
        private System.Windows.Forms.Label lblParameter4;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DistributionEditor():this(new Stochastic(DeviateFunction.Average, 0.0))
        {
        }

        /// <summary>
        /// Customised constructor
        /// </summary>
        /// <param name="dist">The default <see cref="Stochastic"/> data type</param>
        public DistributionEditor(Stochastic dist)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            this._retValue = dist;
            this.cmbDistribution.DataSource = Enum.GetNames(typeof(DeviateFunction));
            this.cmbDistribution.Refresh();
            this.cmbDistribution.SelectedIndex = cmbDistribution.FindString(this._retValue.Distribution.ToString());
            this.txtParameter1.Text = this._retValue.Parameter1.ToString();
            this.txtParameter2.Text = this._retValue.Parameter2.ToString();
            this.txtParameter3.Text = this._retValue.Parameter3.ToString();
            this.txtParameter4.Text = this._retValue.Parameter4.ToString();
            string disc = Stochastic.Parameters(this._retValue.Distribution);
            cmbDistribution_SelectedIndexChanged(this,EventArgs.Empty);
        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbDistribution = new System.Windows.Forms.ComboBox();
            this.txtParameter1 = new System.Windows.Forms.TextBox();
            this.txtParameter2 = new System.Windows.Forms.TextBox();
            this.txtParameter3 = new System.Windows.Forms.TextBox();
            this.txtParameter4 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblParameter1 = new System.Windows.Forms.Label();
            this.lblParameter2 = new System.Windows.Forms.Label();
            this.lblParameter3 = new System.Windows.Forms.Label();
            this.lblParameter4 = new System.Windows.Forms.Label();
            this.cmdApply = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbDistribution
            // 
            this.cmbDistribution.Location = new System.Drawing.Point(128, 8);
            this.cmbDistribution.Name = "cmbDistribution";
            this.cmbDistribution.Size = new System.Drawing.Size(144, 24);
            this.cmbDistribution.TabIndex = 0;
            this.cmbDistribution.SelectionChangeCommitted += new System.EventHandler(this.cmbDistribution_SelectedIndexChanged);
            // 
            // txtParameter1
            // 
            this.txtParameter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtParameter1.Location = new System.Drawing.Point(128, 40);
            this.txtParameter1.Name = "txtParameter1";
            this.txtParameter1.Size = new System.Drawing.Size(72, 22);
            this.txtParameter1.TabIndex = 1;
            this.txtParameter1.Text = "0.0";
            this.txtParameter1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtParameter2
            // 
            this.txtParameter2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtParameter2.Location = new System.Drawing.Point(128, 72);
            this.txtParameter2.Name = "txtParameter2";
            this.txtParameter2.Size = new System.Drawing.Size(72, 22);
            this.txtParameter2.TabIndex = 2;
            this.txtParameter2.Text = "0.0";
            this.txtParameter2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtParameter3
            // 
            this.txtParameter3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtParameter3.Location = new System.Drawing.Point(128, 104);
            this.txtParameter3.Name = "txtParameter3";
            this.txtParameter3.Size = new System.Drawing.Size(72, 22);
            this.txtParameter3.TabIndex = 3;
            this.txtParameter3.Text = "0.0";
            this.txtParameter3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtParameter4
            // 
            this.txtParameter4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtParameter4.Location = new System.Drawing.Point(128, 136);
            this.txtParameter4.Name = "txtParameter4";
            this.txtParameter4.Size = new System.Drawing.Size(72, 22);
            this.txtParameter4.TabIndex = 4;
            this.txtParameter4.Text = "0.0";
            this.txtParameter4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 22);
            this.label1.TabIndex = 5;
            this.label1.Text = "Distribution:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblParameter1
            // 
            this.lblParameter1.Location = new System.Drawing.Point(8, 40);
            this.lblParameter1.Name = "lblParameter1";
            this.lblParameter1.Size = new System.Drawing.Size(112, 22);
            this.lblParameter1.TabIndex = 6;
            this.lblParameter1.Text = "Parameter #1:";
            this.lblParameter1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblParameter2
            // 
            this.lblParameter2.Location = new System.Drawing.Point(8, 72);
            this.lblParameter2.Name = "lblParameter2";
            this.lblParameter2.Size = new System.Drawing.Size(112, 22);
            this.lblParameter2.TabIndex = 7;
            this.lblParameter2.Text = "Parameter #2:";
            this.lblParameter2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblParameter3
            // 
            this.lblParameter3.Location = new System.Drawing.Point(8, 104);
            this.lblParameter3.Name = "lblParameter3";
            this.lblParameter3.Size = new System.Drawing.Size(112, 22);
            this.lblParameter3.TabIndex = 8;
            this.lblParameter3.Text = "Parameter #3:";
            this.lblParameter3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblParameter4
            // 
            this.lblParameter4.Location = new System.Drawing.Point(8, 136);
            this.lblParameter4.Name = "lblParameter4";
            this.lblParameter4.Size = new System.Drawing.Size(112, 22);
            this.lblParameter4.TabIndex = 9;
            this.lblParameter4.Text = "Parameter #4:";
            this.lblParameter4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmdApply
            // 
            this.cmdApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdApply.Location = new System.Drawing.Point(208, 40);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(64, 56);
            this.cmdApply.TabIndex = 5;
            this.cmdApply.Text = "&Apply";
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Location = new System.Drawing.Point(208, 104);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(64, 56);
            this.cmdCancel.TabIndex = 6;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // DistributionEditor
            // 
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.lblParameter4);
            this.Controls.Add(this.lblParameter3);
            this.Controls.Add(this.lblParameter2);
            this.Controls.Add(this.lblParameter1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtParameter4);
            this.Controls.Add(this.txtParameter3);
            this.Controls.Add(this.txtParameter2);
            this.Controls.Add(this.txtParameter1);
            this.Controls.Add(this.cmbDistribution);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.Name = "DistributionEditor";
            this.Size = new System.Drawing.Size(280, 168);
            this.ResumeLayout(false);

        }
        #endregion


        /// <summary>
        /// Gets the current <see cref="Stochastic"/> value in the dialogue
        /// </summary>
        public Stochastic Value
        {
            get{return this._retValue;}
        }

        /// <summary>
        /// Raised when the Value displayed changes
        /// </summary>
        [Description("Raised when the Value displayed changes")]
        public event EventHandler ValueChanged 
        {
            add {onValueChanged += value;}
            remove {onValueChanged -= value;}
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnValueChanged(EventArgs e) 
        {
            if (onValueChanged != null) 
            {
                onValueChanged.Invoke(this, e);
            }
        }


        private void cmdApply_Click(object sender, System.EventArgs e)
        {
            this._retValue.Distribution = (DeviateFunction)Enum.Parse(typeof(DeviateFunction),this.cmbDistribution.Text);
            this._retValue.Parameter1	= double.Parse(this.txtParameter1.Text);
            this._retValue.Parameter2	= double.Parse(this.txtParameter2.Text);
            this._retValue.Parameter3	= double.Parse(this.txtParameter3.Text);
            this._retValue.Parameter4	= double.Parse(this.txtParameter4.Text);
            OnValueChanged(EventArgs.Empty);
        }


        private void cmdCancel_Click(object sender, System.EventArgs e)
        {
            OnValueChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Processes the keyboard entries in the dialogue 
        /// </summary>
        /// <param name="keyData">The key possessed</param>
        /// <returns>True if it was a command key or False otherwise</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Return)
            {
                cmdApply_Click(this,EventArgs.Empty);
                return true;
            }
            else if(keyData == Keys.Escape)
            {
                cmdCancel_Click(this,EventArgs.Empty);
                return true;
            }
            else
            {
                return false;
            }
        }


        private void cmbDistribution_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string info = Stochastic.Parameters((DeviateFunction)Enum.Parse(typeof(DeviateFunction),
                                          cmbDistribution.SelectedItem.ToString()));
            string[] data = info.Split((char)',');
            if (data.Length == 1)
            {
                lblParameter1.Text = data[0];
                lblParameter2.Text = "N/A:";
                lblParameter3.Text = "N/A:";
                lblParameter4.Text = "N/A:";

                txtParameter2.Text = "0";
                txtParameter3.Text = "0";
                txtParameter4.Text = "0";
                
                txtParameter1.Enabled = true;
                txtParameter2.Enabled = false;
                txtParameter3.Enabled = false;
                txtParameter4.Enabled = false;
            }
            else if (data.Length == 2)
            {
                lblParameter1.Text = data[0];
                lblParameter2.Text = data[1];
                lblParameter3.Text = "N/A:";
                lblParameter4.Text = "N/A:";

                txtParameter3.Text = "0";
                txtParameter4.Text = "0";

                txtParameter1.Enabled = true;
                txtParameter2.Enabled = true;
                txtParameter3.Enabled = false;
                txtParameter4.Enabled = false;
            }
            else if (data.Length == 3)
            {
                lblParameter1.Text = data[0];
                lblParameter2.Text = data[1];
                lblParameter3.Text = data[2];
                lblParameter4.Text = "N/A:";
                txtParameter4.Text = "0";

                txtParameter1.Enabled = true;
                txtParameter2.Enabled = true;
                txtParameter3.Enabled = true;
                txtParameter4.Enabled = false;
            }
            else if (data.Length == 4)
            {
                lblParameter1.Text = data[0];
                lblParameter2.Text = data[1];
                lblParameter3.Text = data[2];
                lblParameter4.Text = data[3];

                txtParameter1.Enabled = true;
                txtParameter2.Enabled = true;
                txtParameter3.Enabled = true;
                txtParameter4.Enabled = true;
            }
            else
            {
                lblParameter1.Text = "Parameter #1:";
                lblParameter2.Text = "Parameter #2:";
                lblParameter3.Text = "Parameter #3:";
                lblParameter4.Text = "Parameter #4:";

                txtParameter1.Text = "0";
                txtParameter2.Text = "0";
                txtParameter3.Text = "0";
                txtParameter4.Text = "0";

                txtParameter1.Enabled = true;
                txtParameter2.Enabled = true;
                txtParameter3.Enabled = true;
                txtParameter4.Enabled = true;
            }
        }
    }
    


    /// <summary>
    /// Class to support GUIs editing probability distributions
    /// </summary>
    public class DistributionValueEditor : UITypeEditor 
    {
        private IWindowsFormsEditorService edSvc = null;

        /// <summary>
        /// Edits the value of the specified object using the editor style 
        /// indicated by GetEditStyle.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that 
        /// can be used to gain additional context information</param>
        /// <param name="provider">An <see cref="IServiceProvider"/> that this 
        /// editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>The new value of the object</returns>
        public override object EditValue(
                                        ITypeDescriptorContext context, 
                                        IServiceProvider provider, 
                                        object value) 
        {
            if (context != null	&& context.Instance != null	&& provider != null) 
            {
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null) 
                {
                    DistributionEditor dEdt = new DistributionEditor((Stochastic)value);
                    dEdt.ValueChanged += new EventHandler(this.ValueChanged);
                    edSvc.DropDownControl(dEdt);
                    value = dEdt.Value;
                }
            }
            return value;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="EditValue"/> method
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that 
        /// can be used to gain additional context information.</param>
        /// <returns>A <see cref="UITypeEditorEditStyle"/> value that indicates 
        /// the style of editor used by <see cref="EditValue"/>.</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
        {
            if (context != null && context.Instance != null) 
            {
                return UITypeEditorEditStyle.DropDown;
            }
            return base.GetEditStyle(context);
        }

        private void ValueChanged(object sender, EventArgs e) 
        {
            if (edSvc != null) 
            {
                edSvc.CloseDropDown();
            }
        }
    }
}
