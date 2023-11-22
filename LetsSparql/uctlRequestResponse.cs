using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LetsSparql.Common.Dto;
using LetsSparql.Service;

namespace LetsSparql
{
    public partial class uctlRequestResponse : UserControl
    {
        private readonly ISparqlExecuter _sparqlExecuter;
        private bool _gridSearchIntiated;

        public uctlRequestResponse(ISparqlExecuter sparqlExecuter)
        {
            InitializeComponent();
            this._sparqlExecuter = sparqlExecuter;
        }

        private async void btnExecute_Click(object sender, EventArgs e)
        {            
            // Set cursor as hourglass
            this.lblResponse.Text = $"Response :";
            this.rtfResponse.Text = "Request sent......";
            this.dgdResponse.DataSource = null;
            this.Cursor = Cursors.WaitCursor;
            var resp = await _sparqlExecuter.GetResponse(this.cmbEndpoint.Text, this.txtQuery.Text, chbExplain.Checked, chbUpdate.Checked);
            // Set cursor as default arrow
            this.Cursor = Cursors.Default;
            
            this.rtfResponse.Text = (resp.Error == null ? resp.Response : resp.Error);
            this.dgdResponse.DataSource = resp.dataTableFormat;
            this.lblResponse.Text = $"Response : {resp.TotalRecords} records in {resp.TimeTaken} milli seconds" ;
            //Expand column width
            for (int i = 0; i < dgdResponse.Columns.Count; i++)
            {
                this.dgdResponse.Columns[i].Width = 500;
            }
        }

        public uctlProperties GetProperties()
        {
            uctlProperties rtrnValue = new uctlProperties
            {
                Query = this.txtQuery.Text,
                Url = this.cmbEndpoint.Text,
                Explain = this.chbExplain.Checked
            };

            return rtrnValue;
        }

        public void SetProperties(uctlProperties newValues)
        {
            this.txtQuery.Text = newValues.Query;
            this.cmbEndpoint.Text = newValues.Url;
            this.chbExplain.Checked = newValues.Explain;
            
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.KeyCode == Keys.Enter)
            {               
                if (tbcResponse.SelectedIndex == 0)
                {
                    this.rtfResponse.SelectionBackColor = Color.White;
                    this.rtfResponse.Find(this.txtSearch.Text, (this.rtfResponse.SelectionStart == 0 ? this.rtfResponse.SelectionStart : this.rtfResponse.SelectionStart + 1), RichTextBoxFinds.None);                    
                    this.rtfResponse.SelectionBackColor = Color.Blue;
                    this.rtfResponse.ScrollToCaret();
                }
                else
                {
                    int colCount = dgdResponse.ColumnCount;
                    this.dgdResponse.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    this.dgdResponse.ClearSelection();
                    int rowFound = -1;
                    try
                    {                        
                        for (int curRow = (_gridSearchIntiated ? this.dgdResponse.FirstDisplayedScrollingRowIndex + 1 : 0) ; curRow < this.dgdResponse.Rows.Count; curRow++)
                        {
                            for (int curCol = 0; curCol < colCount; curCol++)
                            {
                                if (this.dgdResponse.Rows[curRow].Cells[curCol].Value != null && this.dgdResponse.Rows[curRow].Cells[curCol].Value.ToString().Contains(this.txtSearch.Text))
                                {
                                    this.dgdResponse.Rows[curRow].Selected = true;
                                    rowFound = curRow;
                                    break;
                                }
                            }

                            if (rowFound > 0)
                                break;
                        }
                        this.dgdResponse.FirstDisplayedScrollingRowIndex = rowFound;
                    }
                    catch
                    {
                        MessageBox.Show("Search Complete.");
                        this._gridSearchIntiated = false;
                    }
                }
                this._gridSearchIntiated = true;
            }            
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            this.rtfResponse.SelectionBackColor = Color.White;
            this.rtfResponse.SelectionStart = 0;
            this._gridSearchIntiated = false;
        }
    }
}
