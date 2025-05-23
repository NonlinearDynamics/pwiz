/*
 * Original author: Nicholas Shulman <nicksh .at. u.washington.edu>,
 *                  MacCoss Lab, Department of Genome Sciences, UW
 *
 * Copyright 2011 University of Washington - Seattle, WA
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using pwiz.Common.Collections;
using pwiz.Common.Colors;
using pwiz.Common.Controls;
using pwiz.Common.DataBinding.Attributes;
using pwiz.Common.DataBinding.Clustering;
using pwiz.Common.DataBinding.Internal;
using pwiz.Common.DataBinding.Layout;
using pwiz.Common.SystemUtil;

namespace pwiz.Common.DataBinding.Controls
{
    /// <summary>
    /// Enhancement ot a DataGridView which automatically creates a 
    /// <see cref="BindingListView" /> to use as its DataSource.
    /// Setting the DataSource of a BoundDataGridView to a BindingSource automatically causes
    /// the BindingSource's DataSource to be set to the BindingListView.
    /// 
    /// </summary>
    public class BoundDataGridView : CommonDataGridView
    {
        private BindingListSource _bindingListSource;
        private IViewContext _viewContext;
        private ItemProperties _itemProperties;
        private ImmutableList<ColumnFormat> _columnFormats;
        private CancellationTokenSource _colorSchemeCancellationTokenSource;

        public BoundDataGridView()
        {
            AutoGenerateColumns = false;
            MaximumColumnCount = 2000;
        }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            var bindingListSource = DataSource as BindingListSource;
            var newViewContext = bindingListSource == null ? null : bindingListSource.ViewContext;
            if (!ReferenceEquals(_viewContext, newViewContext))
            {
                if (_viewContext != null)
                {
                    DataError -= _viewContext.OnDataError;
                }
                _viewContext = newViewContext;
                if (_viewContext != null)
                {
                    DataError += _viewContext.OnDataError;
                }
            }
            if (!ReferenceEquals(_bindingListSource, bindingListSource))
            {
                if (_bindingListSource != null)
                {
                    _bindingListSource.AllRowsChanged -= BindingListSourceOnAllRowsChanged;
                    _bindingListSource.ColumnFormats.FormatsChanged -= OnFormatsChanged;
                }
                _bindingListSource = bindingListSource;
                if (_bindingListSource != null)
                {
                    _bindingListSource.AllRowsChanged += BindingListSourceOnAllRowsChanged;
                    _bindingListSource.ColumnFormats.FormatsChanged += OnFormatsChanged;
                }
            }
            UpdateColumns();
            base.OnDataBindingComplete(e);
        }

        private void BindingListSourceOnAllRowsChanged(object sender, EventArgs eventArgs)
        {
            Invalidate();
            UpdateColorScheme();
        }

        private bool _inUpdateColumns;
        protected virtual void UpdateColumns()
        {
            if (_inUpdateColumns)
            {
                return;
            }

            try
            {
                _inUpdateColumns = true;
                var bindingListSource = DataSource as BindingListSource;
                if (DesignMode)
                {
                    return;
                }

                if (null == bindingListSource || null == _viewContext)
                {
                    return;
                }

                var columnsToHide = new HashSet<string>(GetColumnsToHide(bindingListSource.ReportResults).Select(pd=>pd.Name));
                var newItemProperties = bindingListSource.ItemProperties;
                if (!Equals(newItemProperties, _itemProperties))
                {
                    var newColumns = new List<DataGridViewColumn>();
                    for (int i = 0; i < newItemProperties.Count; i++)
                    {
                        var propertyDescriptor = newItemProperties[i];

                        var column = _viewContext.CreateGridViewColumn(propertyDescriptor);
                        if (null != column)
                        {
                            newColumns.Add(column);
                            if (columnsToHide.Contains(propertyDescriptor.Name))
                            {
                                column.Visible = false;
                            }
                        }
                    }

                    if (newColumns.Count > 0)
                    {
                        Columns.Clear();
                        AddColumns(newColumns.ToArray());
                    }

                    _itemProperties = newItemProperties;
                }

                UpdateColumnFormats(false);
                UpdateColorScheme();
                UpdateColumnsFrozen();

            }
            finally
            {
                _inUpdateColumns = false;
            }
        }

        protected virtual IEnumerable<PropertyDescriptor> GetColumnsToHide(ReportResults reportResults)
        {
            if (reportResults is ClusteredReportResults clusteredReportResults)
            {
                return clusteredReportResults.ClusteredProperties.GetAllColumnHeaderProperties();
            }

            return Array.Empty<PropertyDescriptor>();
        }

        protected override void OnCellContentClick(DataGridViewCellEventArgs e)
        {
            base.OnCellContentClick(e);
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                var value = Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                var linkValue = value as ILinkValue;
                if (linkValue != null)
                {
                    linkValue.ClickEventHandler(this, e);
                }
            }
        }

        protected virtual DataPropertyDescriptor GetPropertyDescriptor(DataGridViewColumn column)
        {
            var propertyName = column.DataPropertyName;
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }
            return _itemProperties.FindByName(column.DataPropertyName);
        }

        protected override void OnColumnDividerDoubleClick(DataGridViewColumnDividerDoubleClickEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.ColumnIndex < Columns.Count)
            {
                var propertyDescriptor = GetPropertyDescriptor(Columns[e.ColumnIndex]);
                if (propertyDescriptor != null && propertyDescriptor.Attributes[typeof(ExpensiveAttribute)] != null)
                {
                    // If the property is expensive to calculate, then prevent double-clicking on 
                    // column header to resize.
                    e.Handled = true;
                    return;
                }
            }
            base.OnColumnDividerDoubleClick(e);
        }

        protected override void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
        {
            base.OnColumnWidthChanged(e);
            var pd = GetPropertyDescriptor(e.Column);
            if (pd == null)
            {
                return;
            }
            var columnId = ColumnId.GetColumnId(pd);
            if (columnId != null)
            {
                var columnFormat = _bindingListSource.ColumnFormats.GetFormat(columnId);
                columnFormat = columnFormat.ChangeWidth(e.Column.Width);
                _bindingListSource.ColumnFormats.SetFormat(columnId, columnFormat);
            }
        }

        protected void UpdateColumnFormats(bool restoreDefaultFormats)
        {
            var bindingListSource = DataSource as BindingListSource;
            if (bindingListSource == null)
            {
                return;
            }
            var newColumnFormats = ImmutableList.ValueOf(_itemProperties.Select(prop=>bindingListSource.ColumnFormats.GetFormat(new ColumnId(prop.ColumnCaption))));
            if (Equals(newColumnFormats, _columnFormats))
            {
                return;
            }
            _columnFormats = newColumnFormats;
            foreach (var column in Columns.OfType<DataGridViewColumn>())
            {
                if (string.IsNullOrEmpty(column.DataPropertyName))
                {
                    continue;
                }
                DataPropertyDescriptor pd = null;
                ColumnFormat columnFormat = null;
                if (column.Index < _itemProperties.Count && _itemProperties[column.Index].Name == column.DataPropertyName)
                {
                    pd = _itemProperties[column.Index];
                    columnFormat = _columnFormats[column.Index];
                }
                else
                {
                    for (int i = 0; i < _itemProperties.Count; i++)
                    {
                        if (_itemProperties[i].Name == column.DataPropertyName)
                        {
                            pd = _itemProperties[i];
                            columnFormat = _columnFormats[i];
                        }
                    }
                }
                if (pd == null)
                {
                    continue;
                }
                if (null != columnFormat.Format)
                {
                    column.DefaultCellStyle.Format = columnFormat.Format;
                }
                else
                {
                    if (restoreDefaultFormats)
                    {
                        var originalColumn = _viewContext.CreateGridViewColumn(pd);
                        column.DefaultCellStyle.Format = originalColumn.DefaultCellStyle.Format;
                    }
                }
                if (columnFormat.Width.HasValue)
                {
                    column.Width = columnFormat.Width.Value;
                }
            }
        }

        private void UpdateColumnsFrozen()
        {
            var lastFrozenColumn = GetSelectedFrozenColumn();
            var shouldFreeze = lastFrozenColumn != null;
            foreach (DataGridViewColumn column in Columns)
            {
                var currentPropertyDescriptor = GetPropertyDescriptor(column);
                if (currentPropertyDescriptor == null)
                {
                    shouldFreeze = false;
                    column.Frozen = false;
                    continue;
                }

                var currentColumnId = ColumnId.GetColumnId(currentPropertyDescriptor);
                column.Frozen = shouldFreeze;

                // Set Frozen to false once we reach target column. 
                if (currentColumnId.Equals(lastFrozenColumn))
                {
                    shouldFreeze = false;
                }
            }
        }

        private ColumnId GetSelectedFrozenColumn()
        {
            if (_bindingListSource.ColumnFormats.DefaultFrozenEnabled)
            {
                return GetDefaultFrozenColumn();
            }
            else
            {
                return GetColumnFormatSelectedFrozenColumn();
            }
        }

        private ColumnId GetColumnFormatSelectedFrozenColumn()
        {
            return _bindingListSource.ItemProperties
                .Select(ColumnId.GetColumnId)
                .LastOrDefault(columnId => _bindingListSource.ColumnFormats.GetFormat(columnId).Frozen ?? false);
        }

        private ColumnId GetDefaultFrozenColumn()
        {
            ColumnId defaultFrozenColumn = null;
            var defaultFrozenColumnIndex = _bindingListSource.ColumnFormats.DefaultFrozenColumnCount - 1;
            if (defaultFrozenColumnIndex >= 0 && defaultFrozenColumnIndex < _bindingListSource.ItemProperties.Count)
            {
                var defaultFrozenColumnPropertyDescriptor = _bindingListSource.ItemProperties[defaultFrozenColumnIndex];
                defaultFrozenColumn = ColumnId.GetColumnId(defaultFrozenColumnPropertyDescriptor);
            }
            return defaultFrozenColumn;
        }

        protected void OnFormatsChanged()
        {
            UpdateColumnFormats(true);
            UpdateColumnsFrozen();
        }

        protected override void OnRowValidating(DataGridViewCellCancelEventArgs e)
        {
            base.OnRowValidating(e);
            if (!e.Cancel)
            {
                if (_bindingListSource != null && !_bindingListSource.ValidateRow(e.RowIndex, out _))
                {
                    e.Cancel = true;
                }
            }
        }
        protected override void OnCellErrorTextNeeded(DataGridViewCellErrorTextNeededEventArgs e)
        {
            e.ErrorText = GetErrorText(e);
            base.OnCellErrorTextNeeded(e);
        }

        private string GetErrorText(DataGridViewCellEventArgs cellArgs)
        {
            var column = Columns[cellArgs.ColumnIndex];
            var bindingSource = DataSource as BindingListSource;
            if (bindingSource == null)
            {
                return null;
            }

            var dataSchema = bindingSource.ViewInfo?.DataSchema;
            if (dataSchema == null)
            {
                return null;
            }
            var propertyDescriptor =
                bindingSource.FindDataProperty(column.DataPropertyName) as ColumnPropertyDescriptor;
            if (propertyDescriptor == null)
            {
                return null;
            }
            var propertyType = dataSchema.GetWrappedValueType(propertyDescriptor.PropertyType);
            if (typeof(IAnnotatedValue).IsAssignableFrom(propertyType))
            {
                var value = dataSchema.UnwrapValue(propertyDescriptor.GetValue((RowItem)bindingSource[cellArgs.RowIndex])) as IAnnotatedValue;
                if (value != null)
                {
                    return value.GetErrorMessage();
                }
            }
            var parentColumn = propertyDescriptor.DisplayColumn?.ColumnDescriptor?.Parent;
            if (parentColumn == null)
            {
                return null;
            }
            var parentColumnPropertyType = dataSchema.GetWrappedValueType(parentColumn.PropertyType);
            if (!typeof(IErrorTextProvider).IsAssignableFrom(parentColumnPropertyType))
            {
                return null;
            }

            var parentValue = dataSchema.UnwrapValue(parentColumn.GetPropertyValue((RowItem)bindingSource[cellArgs.RowIndex], null));
            if (parentValue is IErrorTextProvider errorTextProvider)
            {
                return errorTextProvider.GetErrorText(propertyDescriptor.DisplayColumn.PropertyPath.Name);
            }

            return null;
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            base.OnCellFormatting(e);
            if (ReportColorScheme == null)
            {
                return;
            }

            if (e.ColumnIndex < 0 || e.ColumnIndex >= ColumnCount)
            {
                return;
            }
            var reportResults = (DataSource as BindingListSource)?.ReportResults;
            if (reportResults == null)
            {
                return;
            }

            if (e.RowIndex < 0 || e.RowIndex >= reportResults.RowCount)
            {
                return;
            }

            var column = Columns[e.ColumnIndex];
            if (column == null)
            {
                return;
            }
            var propertyDescriptor = reportResults.ItemProperties.FindByName(Columns[e.ColumnIndex].DataPropertyName);
            if (propertyDescriptor == null)
            {
                return;
            }

            var color = ReportColorScheme.GetColor(propertyDescriptor, reportResults.RowItems[e.RowIndex]);
            if (color.HasValue)
            {
                e.CellStyle.BackColor = color.Value;
                var linkColumn = column as DataGridViewLinkColumn;
                if (ColorPalettes.GetColorBrightness(color.Value) < .5)
                {
                    e.CellStyle.ForeColor = Color.White;
                    if (linkColumn != null)
                    {
                        var row = Rows[e.RowIndex];
                        // "row" has now been unshared, and we can safely change its LinkColor
                        var linkCell = row.Cells[e.ColumnIndex] as DataGridViewLinkCell;
                        if (linkCell != null)
                        {
                            linkCell.LinkColor = Color.White;
                        }
                    }
                }
                else
                {
                    if (linkColumn != null)
                    {
                        var sharedRow = Rows.SharedRow(e.RowIndex);
                        // If the row has been unshared (i.e. its index is not -1), then we might need to set its LinkColor back to its original value
                        if (sharedRow.Index != -1)
                        {
                            var linkCell = sharedRow.Cells[e.ColumnIndex] as DataGridViewLinkCell;
                            if (linkCell != null)
                            {
                                linkCell.LinkColor = linkColumn.LinkColor;
                            }
                        }
                    }
                }
            }
        }

        public void UpdateColorScheme()
        {
            _colorSchemeCancellationTokenSource?.Cancel();
            _colorSchemeCancellationTokenSource = null;
            var reportResults = (DataSource as BindingListSource)?.ReportResults as ClusteredReportResults;
            if (reportResults == null)
            {
                ReportColorScheme = null;
            }
            else
            {
                var dataSchema = (DataSource as BindingListSource)?.ViewInfo?.DataSchema;
                if (dataSchema == null)
                {
                    return;
                }

                _colorSchemeCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(dataSchema.QueryLock.CancellationToken);
                var cancellationToken = _colorSchemeCancellationTokenSource.Token;
                CommonActionUtil.RunAsync(() =>
                {
                    using (dataSchema.QueryLock.GetReadLock())
                    {
                        GetColorScheme(cancellationToken, reportResults);
                    }
                });
            }
        }

        private void GetColorScheme(CancellationToken cancellationToken, ClusteredReportResults reportResults)
        {
            try
            {
                var reportColorScheme = ReportColorScheme.FromClusteredResults(cancellationToken, reportResults);
                if (!cancellationToken.IsCancellationRequested)
                {
                    CommonActionUtil.SafeBeginInvoke(this, () =>
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            ReportColorScheme = reportColorScheme;
                            Invalidate();
                        }
                    });
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public ReportColorScheme ReportColorScheme { get; set; }
    }
}
