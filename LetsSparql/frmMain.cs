using LetsSparql.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using LetsSparql.Common.Libraries;
using LetsSparql.Common.Dto;

namespace LetsSparql
{
    public partial class frmMain : Form
    {
        private readonly ISparqlExecuter _sparqlExecuter;

        public frmMain(ISparqlExecuter sparqlExecuter)
        {
            InitializeComponent();
            this._sparqlExecuter = sparqlExecuter;
        }

        private void tabCtrlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabCtrlMain.SelectedTab.Name == "tabNew")
            {
                uctlRequestResponse newUI = new uctlRequestResponse(_sparqlExecuter);
                newUI.Dock = DockStyle.Fill;

                TabPage createdTabPage = new TabPage(GetNewTabName());
                createdTabPage.Controls.Add(newUI);

                this.tabCtrlMain.TabPages.Insert(tabCtrlMain.TabPages.Count - 1, createdTabPage);
                this.tabCtrlMain.SelectedTab = createdTabPage;
            }
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabCtrlMain.SelectedTab.Name != "tabNew")
            {
                uctlRequestResponse newUI = new uctlRequestResponse(_sparqlExecuter);
                newUI.SetProperties(((uctlRequestResponse)this.tabCtrlMain.SelectedTab.Controls[0]).GetProperties());
                newUI.Dock = DockStyle.Fill;

                TabPage createdTabPage = new TabPage(GetNewTabName());
                createdTabPage.Controls.Add(newUI);

                this.tabCtrlMain.TabPages.Insert(tabCtrlMain.TabPages.Count - 1, createdTabPage);
                this.tabCtrlMain.SelectedTab = createdTabPage;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<uctlProperties> allTabValues = new List<uctlProperties>();
            foreach (TabPage curTab in this.tabCtrlMain.TabPages)
            {
                if (curTab.Name == "tabNew")
                    continue;
                var curCntrl = ((uctlRequestResponse)curTab.Controls[0]).GetProperties();
                curCntrl.Id = curTab.Text;
                allTabValues.Add(curCntrl);
            }

            TextFileManager.WriteTextToFile("LetsSparqlData.txt", JsonConvert.SerializeObject(allTabValues));
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            string rawStateData = TextFileManager.ReadTextFromFile("LetsSparqlData.txt");
            List<uctlProperties> stateData = new List<uctlProperties>();
            stateData.Add(new uctlProperties { Id = GetNewTabName(), Url = "", Query = "SELECT DISTINCT ?g WHERE { GRAPH ?g { ?s ?p ?o }}" });
            try
            {
                if (rawStateData != null && rawStateData != String.Empty)
                    stateData = JsonConvert.DeserializeObject<List<uctlProperties>>(rawStateData);
            }
            catch { }

            foreach (uctlProperties prop in stateData)
            {
                uctlRequestResponse newUI = new uctlRequestResponse(_sparqlExecuter);
                newUI.SetProperties(prop);
                newUI.Dock = DockStyle.Fill;

                TabPage createdTabPage = new TabPage(prop.Id);
                createdTabPage.Controls.Add(newUI);

                this.tabCtrlMain.TabPages.Insert(tabCtrlMain.TabPages.Count - 1, createdTabPage);
                this.tabCtrlMain.SelectedTab = createdTabPage;

            }
        }

        /// <summary>
        /// Delete selected tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int curIndex = this.tabCtrlMain.SelectedIndex;
            this.tabCtrlMain.TabPages.Remove(this.tabCtrlMain.SelectedTab);
            if (curIndex > 0)
                this.tabCtrlMain.SelectedTab = this.tabCtrlMain.TabPages[curIndex - 1];
        }

        /// <summary>
        /// Generate name of the next new tab
        /// </summary>
        /// <returns></returns>
        private string GetNewTabName()
        {
            string tabName = "New";
            int maxTabTextCtr = 0;
            foreach (TabPage curTab in this.tabCtrlMain.TabPages)
            {
                if (curTab.Name == "tabNew")
                    continue;
                var tempArray = curTab.Text.Split(" ");
                if (tempArray.Length == 2)
                {
                    if (tempArray[0] == tabName && int.TryParse(tempArray[1], out int rslt))
                    {
                        if (rslt > maxTabTextCtr) maxTabTextCtr = rslt;
                    }
                }                
            }
            return tabName + " " + (maxTabTextCtr + 1).ToString();
        }
    }
}
