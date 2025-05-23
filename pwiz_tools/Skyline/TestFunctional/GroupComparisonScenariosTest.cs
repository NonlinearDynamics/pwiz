﻿/*
 * Original author: Nicholas Shulman <nicksh .at. u.washington.edu>,
 *                  MacCoss Lab, Department of Genome Sciences, UW
 *
 * Copyright 2016 University of Washington - Seattle, WA
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pwiz.Common.DataBinding;
using pwiz.Common.DataBinding.Controls.Editor;
using pwiz.Skyline.Controls.Databinding;
using pwiz.Skyline.Controls.GroupComparison;
using pwiz.Skyline.Model;
using pwiz.SkylineTestUtil;

namespace pwiz.SkylineTestFunctional
{
    [TestClass]
    public class GroupComparisonScenariosTest : AbstractFunctionalTest
    {
        [TestMethod]
        public void TestGroupComparisonScenarios()
        {
            TestFilesZip = @"TestFunctional\GroupComparisonScenariosTest.zip";
            RunFunctionalTest();
        }

        protected override void DoTest()
        {
            var exportLiveReportDlg = ShowDialog<ExportLiveReportDlg>(SkylineWindow.ShowExportReportDialog);
            var manageViewsForm = ShowDialog<ManageViewsForm>(exportLiveReportDlg.EditList);
            RunUI(() => manageViewsForm.ImportViews(TestFilesDir.GetTestPath("GroupComparisonReports.skyr")));
            OkDialog(manageViewsForm, manageViewsForm.OkDialog);
            OkDialog(exportLiveReportDlg, exportLiveReportDlg.CancelClick);
            var scenarioNames = new[] {"Rat_plasma"};
            foreach (var scenarioName in scenarioNames)
            {
                RunScenario(scenarioName);
            }
        }
        private void RunScenario(string scenarioName)
        {
            RunUI(() => SkylineWindow.OpenSharedFile(TestFilesDir.GetTestPath(scenarioName + ".sky.zip")));

            TestContext.EnsureTestResultsDir();
            string baseName = TestContext.GetTestResultsPath(scenarioName);
            RunUI(() => SkylineWindow.ShareDocument(baseName + ".sky.zip", ShareType.COMPLETE));
            foreach (var groupComparison in SkylineWindow.Document.Settings.DataSettings.GroupComparisonDefs)
            {
                String groupComparisonName = groupComparison.Name;
                FoldChangeGrid foldChangeGrid =
                    ShowDialog<FoldChangeGrid>(() => SkylineWindow.ShowGroupComparisonWindow(groupComparisonName));
                var reports = new[]
                {
                    "GroupComparisonColumns"
                };
                foreach (String report in reports)
                {
                    WaitForConditionUI(() => foldChangeGrid.DataboundGridControl.IsComplete 
                        && null != foldChangeGrid.FoldChangeBindingSource.GroupComparisonModel.Results
                        && null != foldChangeGrid.DataboundGridControl.BindingListSource.ViewContext);

                    // ReSharper disable AccessToForEachVariableInClosure
                    RunUI(() => { foldChangeGrid.DataboundGridControl.ChooseView(report); });
                    // ReSharper restore AccessToForEachVariableInClosure

                    WaitForConditionUI(() => foldChangeGrid.DataboundGridControl.IsComplete);
                    String exportPath = TestContext.GetTestResultsPath(scenarioName + "_" + groupComparisonName + "_" + report + ".csv");

                    RunUI(() =>
                        {
                            var viewContext = (AbstractViewContext) foldChangeGrid.DataboundGridControl.NavBar.ViewContext;

                            viewContext.ExportToFile(foldChangeGrid,
                                foldChangeGrid.DataboundGridControl.BindingListSource, exportPath,
                                ',');
                        }
                    );

                }
                OkDialog(foldChangeGrid, foldChangeGrid.Close);
            }
        }
    }
}
