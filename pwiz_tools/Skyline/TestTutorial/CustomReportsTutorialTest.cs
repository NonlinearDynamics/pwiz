﻿/*
 * Original author: Alana Killeen <killea .at. u.washington.edu>,
 *                  MacCoss Lab, Department of Genome Sciences, UW
 *
 * Copyright 2010 University of Washington - Seattle, WA
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pwiz.Common.DataBinding;
using pwiz.Common.DataBinding.Controls;
using pwiz.Common.DataBinding.Controls.Editor;
using pwiz.Common.SystemUtil;
using pwiz.Common.SystemUtil.PInvoke;
using pwiz.Skyline.Controls.Databinding;
using pwiz.Skyline.Controls.Graphs;
using pwiz.Skyline.Controls.SeqNode;
using pwiz.Skyline.EditUI;
using pwiz.Skyline.Model.Databinding;
using pwiz.Skyline.Model.Databinding.Entities;
using pwiz.Skyline.Model.DocSettings;
using pwiz.Skyline.Properties;
using pwiz.Skyline.SettingsUI;
using pwiz.Skyline.Util.Extensions;
using pwiz.SkylineTestUtil;

namespace pwiz.SkylineTestTutorial
{
    /// <summary>
    /// Testing the tutorial for Skyline Custom Reports and Results Grid
    /// </summary>
    // ReSharper disable LocalizableElement
    [TestClass]
    public class CustomReportsTutorialTest : AbstractFunctionalTest
    {
        const string customReportName = "Overview";

        [TestMethod]
        public void TestCustomReportsTutorial()
        {
            // Set true to look at tutorial screenshots.
//            IsPauseForScreenShots = true;
//            IsCoverShotMode = true;
            CoverShotName = "CustomReports";

            LinkPdf = "https://skyline.ms/_webdav/home/software/Skyline/%40files/tutorials/CustomReports-20_2.pdf";

            TestFilesZipPaths = new[]
            {
                @"https://skyline.ms/tutorials/CustomReports.zip",
                @"TestTutorial\CustomReportsViews.zip"
            };
            RunFunctionalTest();
        }

        private new TestFilesDir TestFilesDir
        {
            get { return TestFilesDirs[0]; }
        }

        protected override void DoTest()
        {
            // Data Overview, p. 2
            RunUI(() => SkylineWindow.OpenFile(TestFilesDir.GetTestPath(@"CustomReports\Study7_example.sky")));
                // Not L10N
            RunDlg<FindNodeDlg>(SkylineWindow.ShowFindNodeDlg, findPeptideDlg =>
            {
                findPeptideDlg.SearchString = "HGFLPR"; // Not L10N
                findPeptideDlg.FindNext();
                findPeptideDlg.Close();
            });
            RunUI(() =>
            {
                Assert.AreEqual("HGFLPR", SkylineWindow.SequenceTree.SelectedNode.Text); // Not L10N
                SkylineWindow.ShowPeakAreaReplicateComparison();
            });
            WaitForCondition(() => !SkylineWindow.GraphPeakArea.IsHidden);
            RunUI(() => SkylineWindow.ShowGraphPeakArea(false));
            WaitForCondition(() => SkylineWindow.GraphPeakArea.IsHidden);

            DoLiveReportsTest();
        }

        protected void DoLiveReportsTest()
        {
            DoCreatingASimpleReport();
            DoExportingReportDataToAFile();
            DoSharingManagingModifyingReportTemplates();
            if (DoQualityControlSummaryReports())
                DoResultsGridView();
        }


        protected void DoCreatingASimpleReport()
        {
            // Creating a Simple Custom Report, p. 3
            var exportReportDlg = ShowDialog<ExportLiveReportDlg>(SkylineWindow.ShowExportReportDialog);
            PauseForScreenShot<ExportLiveReportDlg>("Export Report form");

            // p. 4
            var editReportListDlg = ShowDialog<ManageViewsForm>(exportReportDlg.EditList);
            var viewEditor = ShowDialog<ViewEditor>(editReportListDlg.AddView);
            RunUI(() => viewEditor.ViewName = customReportName);
            PauseForScreenShot<ViewEditor.ChooseColumnsView>("Edit Report form");

            // p. 5
            RunUI(() =>
            {
                viewEditor.ChooseColumnsTab.ExpandPropertyPath(PropertyPath.Parse("Proteins!*.Peptides!*"), true);
                viewEditor.ChooseColumnsTab.ExpandPropertyPath(PropertyPath.Parse("Replicates!*"), true);
                // Make the view editor bigger so that these expanded nodes can be seen in the next screenshot
                viewEditor.Height = Math.Max(viewEditor.Height, 600);
                Assert.IsTrue(viewEditor.ChooseColumnsTab.TrySelect(PropertyPath.Parse("Proteins!*.Peptides!*.Sequence"))); // Not L10N
                viewEditor.ChooseColumnsTab.AddSelectedColumn();
                viewEditor.ChooseColumnsTab.ScrollTreeToTop();
            });
            PauseForScreenShot<ViewEditor.ChooseColumnsView>("Edit Report form");

            // p. 7
            RunUI(() =>
            {
                var columnsToAdd = new[]
                { 
                    // Not L10N
                    PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.IsotopeLabelType"),
                    PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Results!*.Value.BestRetentionTime"),
                    PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Results!*.Value.TotalArea"),
                };
                foreach (var id in columnsToAdd)
                {
                    Assert.IsTrue(viewEditor.ChooseColumnsTab.TrySelect(id), "Unable to select {0}", id);
                    viewEditor.ChooseColumnsTab.AddSelectedColumn();
                }
                Assert.AreEqual(4, viewEditor.ChooseColumnsTab.ColumnCount);
                viewEditor.ViewEditorWidgets.OfType<PivotReplicateAndIsotopeLabelWidget>().First().SetPivotReplicate(true);
                viewEditor.ChooseColumnsTab.ScrollTreeToTop();
            });
            PauseForScreenShot<ViewEditor.ChooseColumnsView>("Edit Report form");
            // p. 8
            {
                var previewReportDlg = ShowDialog<DocumentGridForm>(viewEditor.ShowPreview);
                WaitForConditionUI(() => previewReportDlg.IsComplete);
                RunUI(() =>
                {
                    Assert.AreEqual(20, previewReportDlg.RowCount);
                    Assert.AreEqual(58, previewReportDlg.ColumnCount);
                });
                PauseForScreenShot<DocumentGridForm>("Preview form");

                OkDialog(previewReportDlg, previewReportDlg.Close);
            }

            // p. 9-10
            OkDialog(viewEditor, viewEditor.OkDialog);
            PauseForScreenShot<ManageViewsForm>("Edit Reports form");

            OkDialog(editReportListDlg, editReportListDlg.OkDialog);
            PauseForScreenShot<ExportLiveReportDlg>("Export Report form");

            OkDialog(exportReportDlg, exportReportDlg.CancelClick);
        }

        protected void DoExportingReportDataToAFile()
        {
            // Exporting Report Data to a File, p. 9
            RunDlg<ExportLiveReportDlg>(SkylineWindow.ShowExportReportDialog, exportReportDlg0 =>
            {
                exportReportDlg0.ReportName = customReportName; // Not L10N
                exportReportDlg0.OkDialog(TestFilesDir.GetTestPath("Overview_Study7_example.csv"), TextUtil.SEPARATOR_CSV); // Not L10N
            });
        }

        protected void DoSharingManagingModifyingReportTemplates()
        {
            // Sharing Report Templates, p. 11
            var exportReportDlg1 = ShowDialog<ExportLiveReportDlg>(SkylineWindow.ShowExportReportDialog);
            {
                var manageViewsForm = ShowDialog<ManageViewsForm>(exportReportDlg1.EditList);
                PauseForScreenShot<ManageViewsForm>("Save Report Definitions form");

                RunUI(() =>
                {
                    manageViewsForm.SelectView(customReportName);
                    manageViewsForm.ExportViews(TestFilesDir.GetTestPath(@"CustomReports\Overview.skyr"));
                }); // Not L10N
                
                OkDialog(manageViewsForm, manageViewsForm.Close); // Not L10N
            }

            // Managing Report Templates in Skyline, p. 12 & 13
            var editReportListDlg0 = ShowDialog<ManageViewsForm>(exportReportDlg1.EditList);
            RunUI(() =>
            {
                editReportListDlg0.SelectView(customReportName);
            });
            PauseForScreenShot<ManageViewsForm>("Edit Reports form");

            RunUI(() => editReportListDlg0.Remove(false));
            OkDialog(editReportListDlg0, editReportListDlg0.OkDialog);
            PauseForScreenShot<ExportLiveReportDlg>("Export Report form");

            {
                var manageViewsForm = ShowDialog<ManageViewsForm>(exportReportDlg1.EditList);

                RunUI(() =>
                    manageViewsForm.ImportViews(TestFilesDir.GetTestPath(@"CustomReports\Overview.skyr"))
                );
                OkDialog(manageViewsForm, manageViewsForm.Close);
                RunUI(()=>{exportReportDlg1.ReportName = customReportName;});
            }
            var previewDlg = ShowDialog<DocumentGridForm>(exportReportDlg1.ShowPreview);
            var expectedRows = 20;
            WaitForCondition(() => previewDlg.RowCount == expectedRows);
            Assert.AreEqual(expectedRows, previewDlg.RowCount);
            Assert.AreEqual(58, previewDlg.ColumnCount);
            RunUI(previewDlg.Close);

            // Modifying Existing Report Templates, p. 14
            var editReportListDlg1 = ShowDialog<ManageViewsForm>(exportReportDlg1.EditList);
            RunUI(() => editReportListDlg1.SelectView(customReportName)); // Not L10N
            var viewEditor = ShowDialog<ViewEditor>(editReportListDlg1.CopyView);
            PauseForScreenShot<ViewEditor.ChooseColumnsView>("Edit Report form");

            RunUI(() =>
            {
                viewEditor.ViewName = "Study 7"; // Not L10N
                // Not L10N
                var columnsToAdd = new[]
                                       {
                                           // Not L10N
                                           PropertyPath.Parse("Replicates!*.Files!*.FileName"),
                                           PropertyPath.Parse("Replicates!*.Files!*.SampleName"),
                                           PropertyPath.Parse("Replicates!*.Name"),
                                           PropertyPath.Parse("Proteins!*.Name"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.AverageMeasuredRetentionTime"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Results!*.Value.PeptideRetentionTime"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Results!*.Value.RatioToStandard"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Charge"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Mz"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.ProductCharge"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.ProductMz"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.FragmentIon"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Results!*.Value.MaxFwhm"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Results!*.Value.MinStartTime"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Results!*.Value.MaxEndTime"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.Results!*.Value.RetentionTime"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.Results!*.Value.Fwhm"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.Results!*.Value.StartTime"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.Results!*.Value.EndTime"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.Results!*.Value.Area"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.Results!*.Value.Height"),
                                           PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.Transitions!*.Results!*.Value.UserSetPeak"),
                                       };
                foreach (var id in columnsToAdd)
                {
                    Assert.IsTrue(viewEditor.ChooseColumnsTab.TrySelect(id), "Unable to select {0}", id);
                    viewEditor.ChooseColumnsTab.AddSelectedColumn();
                }
                var pivotWidget = viewEditor.ViewEditorWidgets.OfType<PivotReplicateAndIsotopeLabelWidget>().First();
                pivotWidget.SetPivotReplicate(false);
                viewEditor.Height = 627;
            });
            PauseForScreenShot<ViewEditor.ChooseColumnsView>("Edit Report form expanded to show selected columns");

            int columnCount = 0;
            int rowCount = 0;
            {
                var previewReportDlg = ShowDialog<DocumentGridForm>(viewEditor.ShowPreview);
                WaitForCondition(() => previewReportDlg.ColumnCount > 0);
                RunUI(() =>
                {
                    columnCount = previewReportDlg.ColumnCount;
                    rowCount = previewReportDlg.RowCount;
                });
                OkDialog(previewReportDlg, previewReportDlg.Close);
            }
            RunUI(() => viewEditor.ViewEditorWidgets.OfType<PivotReplicateAndIsotopeLabelWidget>().First().SetPivotIsotopeLabel(true));
            {
                var previewReportDlg = ShowDialog<DocumentGridForm>(viewEditor.ShowPreview);
                WaitForCondition(() => previewReportDlg.ColumnCount > 0);
                RunUI(() =>
                {
                    Assert.IsTrue(previewReportDlg.ColumnCount > columnCount);
                    Assert.AreEqual((rowCount / 2), previewReportDlg.RowCount);

                    string heightCol = TextUtil.SpaceSeparate("light", ColumnCaptions.Height);
                    bool foundHeightCol = false;
                    foreach (DataGridViewColumn leftCol in previewReportDlg.DataGridView.Columns)
                    {
                        if (leftCol.HeaderText == heightCol)
                        {
                            previewReportDlg.DataGridView.FirstDisplayedScrollingColumnIndex = leftCol.Index;
                            foundHeightCol = true;
                            break;
                        }
                    }
                    Assert.IsTrue(foundHeightCol);
                });
                PauseForScreenShot<DocumentGridForm>("Adjust the scrollbar so that the first displayed column is \"light Height\" and the last displayed column is \"heavy Product Mz\"");
                OkDialog(previewReportDlg, previewReportDlg.Close);
            }
            RunUI(() => viewEditor.ChooseColumnsTab.RemoveColumn(PropertyPath.Parse("IsotopeLabelType")));
            OkDialog(viewEditor, viewEditor.OkDialog);
            OkDialog(editReportListDlg1, editReportListDlg1.OkDialog);

            PauseForScreenShot<ExportLiveReportDlg>("Export Report form");

            OkDialog(exportReportDlg1, exportReportDlg1.CancelClick);
        }
        protected bool DoQualityControlSummaryReports()
        {
            // Quality Control Summary Reports, p. 20
            RunUI(() =>
            {
                SkylineWindow.OpenFile(TestFilesDir.GetTestPath(@"CustomReports\study9pilot.sky")); // Not L10N
                SkylineWindow.ExpandPeptides();
            });
            RunUI(() => SkylineWindow.ShowDocumentGrid(true));
            RestoreViewOnScreen(20);
            var documentGridForm = FindOpenForm<DocumentGridForm>();
            var manageViewsForm = ShowDialog<ManageViewsForm>(documentGridForm.ManageViews);
            RunUI(() =>
                manageViewsForm.ImportViews(TestFilesDir.GetTestPath(@"CustomReports\Summary_stats.skyr"))
            );
            PauseForScreenShot<ManageViewsForm>("Manage Reports form");
            OkDialog(manageViewsForm, manageViewsForm.Close);
            var formRectNext = Rectangle.Empty;
            if (IsPauseForScreenShots)
            {
                formRectNext = ShowReportsDropdown("Summary Statistics");
                PauseForScreenShot<DocumentGridForm>("Click the Reports dropdown and highlight 'Summary Statistics'");
                HideReportsDropdown();

                RunUI(() => documentGridForm.NavBar.ReportsButton.HideDropDown());
            }
            RunUI(() => documentGridForm.ChooseView("Summary Statistics"));
            WaitForConditionUI(() => documentGridForm.IsComplete);
            RunUI(() => documentGridForm.ExpandColumns());
            
            RunUIForScreenShot(() =>
            {
                documentGridForm.FloatingPane.FloatAt(formRectNext);
                ConfigureDataGridColumns();
            });

            PauseForScreenShot<DocumentGridForm>("Document Grid with summary statistics", processShot: bmp =>
            {
                // Clean-up the border in the normal way
                bmp = bmp.CleanupBorder(true);

                using var g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                g.DrawBoxOnColumn(documentGridForm, 3, 10);
                g.DrawBoxOnColumn(documentGridForm, 5, 10);
                g.DrawBoxOnColumn(documentGridForm, 6, 10);

                g.DrawEllipseOnCell(documentGridForm, 1, 3);
                g.DrawEllipseOnCell(documentGridForm, 3, 3, Color.Orange);
                g.DrawEllipseOnCell(documentGridForm, 1, 5, Color.Orange);
                g.DrawEllipseOnCell(documentGridForm, 3, 5, Color.Orange);

                return bmp;
            });

            if (IsCoverShotMode)
            {
                RestoreCoverViewOnScreen();
                var documentGridFormCover = WaitForOpenForm<DocumentGridForm>();
                RunUI(SkylineWindow.AutoZoomBestPeak);
                WaitForGraphs();
                var viewEditorCover = ShowDialog<ViewEditor>(documentGridFormCover.NavBar.CustomizeView);
                RunUI(() =>
                {
                    viewEditorCover.Top = SkylineWindow.Top + 10;
                    viewEditorCover.Width -= 40;
                    viewEditorCover.Left = SkylineWindow.Right - viewEditorCover.Width;
                    var columnsExpand = new[]
                    {
                        PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.ResultSummary.MaxFwhm.Cv"),
                        PropertyPath.Parse("Proteins!*.Peptides!*.Precursors!*.ResultSummary.TotalArea.Cv"),
                        PropertyPath.Parse("Proteins!*"), 
                    };
                    foreach (var id in columnsExpand)
                    {
                        Assert.IsTrue(viewEditorCover.ChooseColumnsTab.TrySelect(id), "Unable to select {0}", id);
                    }
                });
                TakeCoverShot(viewEditorCover);

                OkDialog(viewEditorCover, viewEditorCover.CancelButton.PerformClick);
                return false;
            }

            var viewEditor = ShowDialog<ViewEditor>(documentGridForm.NavBar.CustomizeView);
            RunUI(() => Assert.AreEqual(11, viewEditor.ChooseColumnsTab.ColumnCount));
            RunUI(() =>
            {
                int indexCvTotalArea =
                    viewEditor.ChooseColumnsTab.ColumnNames.ToList().IndexOf(GetLocalizedCaption("CvTotalArea"));
                Assert.IsFalse(indexCvTotalArea < 0, "{0} < 0", indexCvTotalArea);
                viewEditor.ChooseColumnsTab.ActivateColumn(indexCvTotalArea);
            });
            PauseForScreenShot<ViewEditor.ChooseColumnsView>("Customize View form");
            RunUI(() =>
            {
                viewEditor.TabControl.SelectTab(1);
                viewEditor.FilterTab.AddSelectedColumn();
                Assert.IsTrue(viewEditor.FilterTab.SetFilterOperation(0, FilterOperations.OP_IS_GREATER_THAN));
                viewEditor.FilterTab.SetFilterOperand(0, .2.ToString(CultureInfo.CurrentCulture));
                viewEditor.FilterTab.AvailableFieldsTree.SetScrollPos(Orientation.Horizontal, 45);
            });
            PauseForScreenShot<ViewEditor.FilterView>("Customize View - Filter tab");
            OkDialog(viewEditor, viewEditor.OkDialog);

            RunUIForScreenShot(ConfigureDataGridColumns);
            PauseForScreenShot<DocumentGridForm>("Document Grid filtered");
            RunUI(documentGridForm.Close);
            RunDlg<FindNodeDlg>(SkylineWindow.ShowFindNodeDlg, findPeptideDlg =>
            {
                findPeptideDlg.SearchString = "INDISHTQSVSAK"; // Not L10N
                findPeptideDlg.FindNext();
                findPeptideDlg.Close();
            });
            RunUI(SkylineWindow.ShowPeakAreaReplicateComparison);
            WaitForGraphs();

            PauseForScreenShot<GraphSummary.AreaGraphView>("Peak Areas view");
            return true;    // Continue subsequent tests
        }

        protected void DoResultsGridView()
        {
            // Results Grid View
            RestoreViewOnScreen(26);
            RunUIForScreenShot(() =>
            {
                // position two floating windows relative to Skyline's bottom-right corner
                var backgroundWindow = FindFloatingWindow(SkylineWindow.GraphPeakArea);
                backgroundWindow.Top = SkylineWindow.Bottom - backgroundWindow.Height - 53;
                backgroundWindow.Left = SkylineWindow.Right - backgroundWindow.Width - 128;

                var foregroundWindow = FindFloatingWindow(FindOpenForm<LiveResultsGrid>());
                foregroundWindow.Top = SkylineWindow.Bottom - foregroundWindow.Height - 26;
                foregroundWindow.Left = SkylineWindow.Right - foregroundWindow.Width - 34;
            });

            PauseForScreenShot(SkylineWindow, "Main window under floating windows");
            RestoreViewOnScreen(27);
            PauseForScreenShot(SkylineWindow, "Main window layout");

            // Not understood: WaitForOpenForm occasionally hangs in nightly test runs. Fixed it by calling
            // ShowDialog when LiveResultsGrid cannot be found.
            // var resultsGridForm = WaitForOpenForm<LiveResultsGrid>();
            var resultsGridForm = FindOpenForm<LiveResultsGrid>() ??
                ShowDialog<LiveResultsGrid>(() => SkylineWindow.ShowResultsGrid(true));
            BoundDataGridView resultsGrid = null;
            RunUI(() =>
            {
                resultsGrid = resultsGridForm.DataGridView;
                SkylineWindow.AutoZoomBestPeak();
                SkylineWindow.SelectedPath = ((SrmTreeNode)SkylineWindow.SequenceTree.SelectedNode.Nodes[0]).Path;
            });
            WaitForGraphs();

            RunUI(() =>
            {
                var precursorNoteColumn =
                    resultsGrid.Columns.Cast<DataGridViewColumn>()
                        .First(col => GetLocalizedCaption("PrecursorReplicateNote") == col.HeaderText);
                resultsGrid.CurrentCell = resultsGrid.Rows[0].Cells[precursorNoteColumn.Index];
                resultsGrid.BeginEdit(true);
                // ReSharper disable LocalizableElement
                resultsGrid.EditingControl.Text = "Low signal";   // Not L10N
                // ReSharper restore LocalizableElement
                resultsGrid.EndEdit();
                resultsGrid.CurrentCell = resultsGrid.Rows[1].Cells[resultsGrid.CurrentCell.ColumnIndex];
            });
            WaitForGraphs();
            RunUI(() => SkylineWindow.SelectedResultsIndex = 1);
            WaitForGraphs();
            PauseForScreenShot<LiveResultsGrid>("Results Grid view subsection");

            RunDlg<ViewEditor>(resultsGridForm.NavBar.CustomizeView, resultsGridViewEditor =>
            {
                // TODO: Update tutorial instructions with new name.
                resultsGridViewEditor.ViewName = "NewResultsGridView";
                var chooseColumnTab = resultsGridViewEditor.ChooseColumnsTab;
                foreach (
                    var column in
                        new[]
                        {
                            PropertyPath.Parse("MinStartTime"), PropertyPath.Parse("MaxEndTime"),
                            PropertyPath.Parse("LibraryDotProduct"), PropertyPath.Parse("TotalBackground"),
                            PropertyPath.Parse("TotalAreaRatio")
                        })
                {
                    Assert.IsTrue(chooseColumnTab.ColumnNames.Contains(GetLocalizedCaption(column.Name)));
                    Assert.IsTrue(chooseColumnTab.TrySelect(column), "Unable to select {0}", column);
                    chooseColumnTab.RemoveColumn(column);
                    Assert.IsFalse(chooseColumnTab.ColumnNames.Contains(GetLocalizedCaption(column.Name)));
                }
                resultsGridViewEditor.OkDialog();
            });

            RunUI(() => SkylineWindow.SelectedNode.Expand());

            // Custom Annotations, p. 25
            var chooseAnnotationsDlg = ShowDialog<DocumentSettingsDlg>(SkylineWindow.ShowDocumentSettingsDialog);
            var editListDlg = ShowDialog<EditListDlg<SettingsListBase<AnnotationDef>, AnnotationDef>>(chooseAnnotationsDlg.EditAnnotationList);
            var defineAnnotationDlg = ShowDialog<DefineAnnotationDlg>(editListDlg.AddItem);
            RunUI(() =>
            {
                defineAnnotationDlg.AnnotationName = "Tailing"; // Not L10N
                defineAnnotationDlg.AnnotationType = AnnotationDef.AnnotationType.true_false;
                defineAnnotationDlg.AnnotationTargets = AnnotationDef.AnnotationTargetSet.Singleton(AnnotationDef.AnnotationTarget.precursor_result);
            });
            PauseForScreenShot<DefineAnnotationDlg>("Define Annotation form");

            OkDialog(defineAnnotationDlg, defineAnnotationDlg.OkDialog);
            OkDialog(editListDlg, editListDlg.OkDialog);
            RunUI(() => chooseAnnotationsDlg.AnnotationsCheckedListBox.SetItemChecked(0, true));
            PauseForScreenShot<DocumentSettingsDlg>("Annotation Settings form");

            OkDialog(chooseAnnotationsDlg, chooseAnnotationsDlg.OkDialog);

            FindNode((564.7746).ToString(LocalizationHelper.CurrentCulture) + "++");
            var liveResultsGrid = FindOpenForm<LiveResultsGrid>();
            ViewEditor viewEditor = ShowDialog<ViewEditor>(liveResultsGrid.NavBar.CustomizeView);
            RunUI(() =>
            {
                viewEditor.ChooseColumnsTab.ActivateColumn(2);
                Assert.IsTrue(viewEditor.ChooseColumnsTab.TrySelect(
                    PropertyPath.Root.Property(AnnotationDef.ANNOTATION_PREFIX + "Tailing")));
                viewEditor.ChooseColumnsTab.AddSelectedColumn();
            });
            PauseForScreenShot<ViewEditor.ChooseColumnsView>("Customize Report form showing Tailing annotation checked");
            OkDialog(viewEditor, viewEditor.OkDialog);
            RunUI(() => SkylineWindow.FocusDocument());
            PauseForScreenShot(SkylineWindow, "Main window with Tailing column added to Results Grid");
        }
        private string GetLocalizedCaption(string caption)
        {
            return SkylineDataSchema.GetLocalizedSchemaLocalizer().LookupColumnCaption(new ColumnCaption(caption));
        }

        private void ConfigureDataGridColumns()
        {
            var documentGridForm = FindOpenForm<DocumentGridForm>();
            var dataGridView = documentGridForm.DataGridView;
            var floatingWindow = FindFloatingWindow(documentGridForm);

            // explicitly size floating window to accommodate one set of column widths for EN, ZH, JP
            floatingWindow.Size = new Size(750, 340);

            dataGridView.Columns[0].Width = 70; // wider for ZH/JP than EN
            dataGridView.Columns[1].Width = 95;
            dataGridView.Columns[2].Width = 80;
            dataGridView.Columns[3].Width = 79; // wider for ZH
            dataGridView.Columns[4].Width = 80;
            dataGridView.Columns[5].Width = 92; // wider for ZH than EN
            dataGridView.Columns[6].Width = 102;
            dataGridView.Columns[7].Width = 105;

            // last 3 columns not visible in screenshot but need to 
            // set width to prevent column title from wrapping to 3 lines 
            dataGridView.Columns[8].Width = 105;
            dataGridView.Columns[9].Width = 105;
            dataGridView.Columns[10].Width = 105;
        }
    }
}
