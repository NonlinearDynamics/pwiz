namespace pwiz.Skyline.FileUI.PeptideSearch
{
    partial class BuildPeptideSearchLibraryControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildPeptideSearchLibraryControl));
            this.grpWorkflow = new System.Windows.Forms.GroupBox();
            this.radioDIA = new System.Windows.Forms.RadioButton();
            this.radioPRM = new System.Windows.Forms.RadioButton();
            this.radioDDA = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButtonNewLibrary = new System.Windows.Forms.RadioButton();
            this.radioExistingLibrary = new System.Windows.Forms.RadioButton();
            this.panelChooseFile = new System.Windows.Forms.Panel();
            this.tbxLibraryPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblLibraryPath = new System.Windows.Forms.Label();
            this.panelPeptideSearch = new System.Windows.Forms.Panel();
            this.lblStandardPeptides = new System.Windows.Forms.Label();
            this.comboStandards = new System.Windows.Forms.ComboBox();
            this.cbIncludeAmbiguousMatches = new System.Windows.Forms.CheckBox();
            this.cbFilterForDocumentPeptides = new System.Windows.Forms.CheckBox();
            this.btnRemFile = new System.Windows.Forms.Button();
            this.lblFileCaption = new System.Windows.Forms.Label();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.helpTip = new System.Windows.Forms.ToolTip(this.components);
            this.gridSearchFiles = new pwiz.Skyline.FileUI.PeptideSearch.BuildLibraryGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpWorkflow.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelChooseFile.SuspendLayout();
            this.panelPeptideSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSearchFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // grpWorkflow
            // 
            this.grpWorkflow.Controls.Add(this.radioDIA);
            this.grpWorkflow.Controls.Add(this.radioPRM);
            this.grpWorkflow.Controls.Add(this.radioDDA);
            resources.ApplyResources(this.grpWorkflow, "grpWorkflow");
            this.grpWorkflow.Name = "grpWorkflow";
            this.grpWorkflow.TabStop = false;
            // 
            // radioDIA
            // 
            resources.ApplyResources(this.radioDIA, "radioDIA");
            this.radioDIA.Name = "radioDIA";
            this.helpTip.SetToolTip(this.radioDIA, resources.GetString("radioDIA.ToolTip"));
            this.radioDIA.UseVisualStyleBackColor = true;
            this.radioDIA.CheckedChanged += new System.EventHandler(this.radioDIA_CheckedChanged);
            // 
            // radioPRM
            // 
            resources.ApplyResources(this.radioPRM, "radioPRM");
            this.radioPRM.Name = "radioPRM";
            this.helpTip.SetToolTip(this.radioPRM, resources.GetString("radioPRM.ToolTip"));
            this.radioPRM.UseVisualStyleBackColor = true;
            this.radioPRM.CheckedChanged += new System.EventHandler(this.radioPRM_CheckedChanged);
            // 
            // radioDDA
            // 
            resources.ApplyResources(this.radioDDA, "radioDDA");
            this.radioDDA.Checked = true;
            this.radioDDA.Name = "radioDDA";
            this.radioDDA.TabStop = true;
            this.helpTip.SetToolTip(this.radioDDA, resources.GetString("radioDDA.ToolTip"));
            this.radioDDA.UseVisualStyleBackColor = true;
            this.radioDDA.CheckedChanged += new System.EventHandler(this.radioDDA_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButtonNewLibrary);
            this.panel1.Controls.Add(this.radioExistingLibrary);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // radioButtonNewLibrary
            // 
            resources.ApplyResources(this.radioButtonNewLibrary, "radioButtonNewLibrary");
            this.radioButtonNewLibrary.Checked = true;
            this.radioButtonNewLibrary.Name = "radioButtonNewLibrary";
            this.radioButtonNewLibrary.TabStop = true;
            this.helpTip.SetToolTip(this.radioButtonNewLibrary, resources.GetString("radioButtonNewLibrary.ToolTip"));
            this.radioButtonNewLibrary.UseVisualStyleBackColor = true;
            this.radioButtonNewLibrary.CheckedChanged += new System.EventHandler(this.radioButtonLibrary_CheckedChanged);
            // 
            // radioExistingLibrary
            // 
            resources.ApplyResources(this.radioExistingLibrary, "radioExistingLibrary");
            this.radioExistingLibrary.Name = "radioExistingLibrary";
            this.radioExistingLibrary.UseVisualStyleBackColor = true;
            this.radioExistingLibrary.CheckedChanged += new System.EventHandler(this.radioButtonLibrary_CheckedChanged);
            // 
            // panelChooseFile
            // 
            this.panelChooseFile.Controls.Add(this.tbxLibraryPath);
            this.panelChooseFile.Controls.Add(this.btnBrowse);
            this.panelChooseFile.Controls.Add(this.lblLibraryPath);
            resources.ApplyResources(this.panelChooseFile, "panelChooseFile");
            this.panelChooseFile.Name = "panelChooseFile";
            // 
            // tbxLibraryPath
            // 
            resources.ApplyResources(this.tbxLibraryPath, "tbxLibraryPath");
            this.tbxLibraryPath.Name = "tbxLibraryPath";
            this.helpTip.SetToolTip(this.tbxLibraryPath, resources.GetString("tbxLibraryPath.ToolTip"));
            this.tbxLibraryPath.TextChanged += new System.EventHandler(this.tbxLibraryPath_TextChanged);
            // 
            // btnBrowse
            // 
            resources.ApplyResources(this.btnBrowse, "btnBrowse");
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblLibraryPath
            // 
            resources.ApplyResources(this.lblLibraryPath, "lblLibraryPath");
            this.lblLibraryPath.Name = "lblLibraryPath";
            // 
            // panelPeptideSearch
            // 
            this.panelPeptideSearch.Controls.Add(this.gridSearchFiles);
            this.panelPeptideSearch.Controls.Add(this.lblStandardPeptides);
            this.panelPeptideSearch.Controls.Add(this.comboStandards);
            this.panelPeptideSearch.Controls.Add(this.cbIncludeAmbiguousMatches);
            this.panelPeptideSearch.Controls.Add(this.cbFilterForDocumentPeptides);
            this.panelPeptideSearch.Controls.Add(this.btnRemFile);
            this.panelPeptideSearch.Controls.Add(this.lblFileCaption);
            this.panelPeptideSearch.Controls.Add(this.btnAddFile);
            resources.ApplyResources(this.panelPeptideSearch, "panelPeptideSearch");
            this.panelPeptideSearch.Name = "panelPeptideSearch";
            // 
            // lblStandardPeptides
            // 
            resources.ApplyResources(this.lblStandardPeptides, "lblStandardPeptides");
            this.lblStandardPeptides.Name = "lblStandardPeptides";
            // 
            // comboStandards
            // 
            resources.ApplyResources(this.comboStandards, "comboStandards");
            this.comboStandards.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStandards.FormattingEnabled = true;
            this.comboStandards.Name = "comboStandards";
            this.helpTip.SetToolTip(this.comboStandards, resources.GetString("comboStandards.ToolTip"));
            this.comboStandards.SelectedIndexChanged += new System.EventHandler(this.comboStandards_SelectedIndexChanged);
            // 
            // cbIncludeAmbiguousMatches
            // 
            resources.ApplyResources(this.cbIncludeAmbiguousMatches, "cbIncludeAmbiguousMatches");
            this.cbIncludeAmbiguousMatches.Name = "cbIncludeAmbiguousMatches";
            this.helpTip.SetToolTip(this.cbIncludeAmbiguousMatches, resources.GetString("cbIncludeAmbiguousMatches.ToolTip"));
            this.cbIncludeAmbiguousMatches.UseVisualStyleBackColor = true;
            // 
            // cbFilterForDocumentPeptides
            // 
            resources.ApplyResources(this.cbFilterForDocumentPeptides, "cbFilterForDocumentPeptides");
            this.cbFilterForDocumentPeptides.Name = "cbFilterForDocumentPeptides";
            this.helpTip.SetToolTip(this.cbFilterForDocumentPeptides, resources.GetString("cbFilterForDocumentPeptides.ToolTip"));
            this.cbFilterForDocumentPeptides.UseVisualStyleBackColor = true;
            // 
            // btnRemFile
            // 
            resources.ApplyResources(this.btnRemFile, "btnRemFile");
            this.btnRemFile.Name = "btnRemFile";
            this.btnRemFile.UseVisualStyleBackColor = true;
            this.btnRemFile.Click += new System.EventHandler(this.btnRemFile_Click);
            // 
            // lblFileCaption
            // 
            resources.ApplyResources(this.lblFileCaption, "lblFileCaption");
            this.lblFileCaption.Name = "lblFileCaption";
            // 
            // btnAddFile
            // 
            resources.ApplyResources(this.btnAddFile, "btnAddFile");
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // gridSearchFiles
            // 
            this.gridSearchFiles.AllowUserToAddRows = false;
            resources.ApplyResources(this.gridSearchFiles, "gridSearchFiles");
            this.gridSearchFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSearchFiles.Name = "gridSearchFiles";
            this.gridSearchFiles.SelectionChanged += new System.EventHandler(this.gridSearchFiles_SelectedIndexChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Frozen = true;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.Frozen = true;
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridViewTextBoxColumn3.Frozen = true;
            resources.ApplyResources(this.dataGridViewTextBoxColumn3, "dataGridViewTextBoxColumn3");
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            resources.ApplyResources(this.dataGridViewTextBoxColumn4, "dataGridViewTextBoxColumn4");
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // BuildPeptideSearchLibraryControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.grpWorkflow);
            this.Controls.Add(this.panelPeptideSearch);
            this.Controls.Add(this.panelChooseFile);
            this.Controls.Add(this.panel1);
            this.Name = "BuildPeptideSearchLibraryControl";
            this.grpWorkflow.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelChooseFile.ResumeLayout(false);
            this.panelChooseFile.PerformLayout();
            this.panelPeptideSearch.ResumeLayout(false);
            this.panelPeptideSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSearchFiles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox grpWorkflow;
        private System.Windows.Forms.RadioButton radioDDA;
        private System.Windows.Forms.RadioButton radioDIA;
        private System.Windows.Forms.RadioButton radioPRM;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonNewLibrary;
        private System.Windows.Forms.RadioButton radioExistingLibrary;
        private System.Windows.Forms.Panel panelChooseFile;
        private System.Windows.Forms.TextBox tbxLibraryPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblLibraryPath;
        private System.Windows.Forms.Panel panelPeptideSearch;
        private System.Windows.Forms.Label lblStandardPeptides;
        private System.Windows.Forms.ComboBox comboStandards;
        private System.Windows.Forms.CheckBox cbIncludeAmbiguousMatches;
        private System.Windows.Forms.CheckBox cbFilterForDocumentPeptides;
        private System.Windows.Forms.Button btnRemFile;
        private System.Windows.Forms.Label lblFileCaption;
        private System.Windows.Forms.Button btnAddFile;
        private BuildLibraryGridView gridSearchFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.ToolTip helpTip;
    }
}
