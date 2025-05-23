﻿/*
 * Original author: Brendan MacLean <brendanx .at. u.washington.edu>,
 *                  MacCoss Lab, Department of Genome Sciences, UW
 *
 * Copyright 2009 University of Washington - Seattle, WA
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
using pwiz.Skyline.Util;

namespace pwiz.Skyline.Alerts
{
    public partial class PasteTypeDlg : ModeUIInvariantFormEx  // This dialog is inherently proteomic, never needs to be adapted for small mol or mixed UI mode
    {
        public PasteTypeDlg()
        {
            InitializeComponent();

            if (PeptideList)
                radioPeptides.Checked = true;
            else
                radioProtein.Checked = true;
        }

        public bool PeptideList { get; set; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            PeptideList = radioPeptides.Checked;
        }
    }
}
