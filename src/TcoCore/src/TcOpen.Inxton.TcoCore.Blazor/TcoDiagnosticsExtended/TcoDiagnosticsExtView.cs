﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Vortex.Connector;

using static TcoCore.TcoDiagnosticsExtViewModel;

namespace TcoCore
{
    public partial class TcoDiagnosticsExtView
    {
        protected override void OnInitialized()
        {
            UpdateValuesOnChange(ViewModel._tcoObject);
            DiagnosticsUpdateTimer();
        }

        public string DiagnosticsStatus { get; set; } = "Diagnostics is not running!";
        private IEnumerable<eMessageCategory> eMessageCategories =>
            Enum.GetValues(typeof(eMessageCategory)).Cast<eMessageCategory>().Skip(1);
        private string Symbol { get; set; }

        public void OnSelectedMessage(PlainTcoMessage message)
        {
            ViewModel.SelectedMessage = message;
            if (ViewModel.SelectedMessage != null)
            {
                ViewModel.AffectedObject = (IVortexObject)ViewModel._tcoObject.GetConnector().IdentityProvider.GetVortexerByIdentity(ViewModel.SelectedMessage.Identity);
            }


        }

        public static int SetDiagnosticsUpdateInterval(int value) =>
            _diagnosticsUpdateInterval = value;

        private static int _diagnosticsUpdateInterval { get; set; } = 500;
        private Timer messageUpdateTimer;

        private void DiagnosticsUpdateTimer()
        {
            if (messageUpdateTimer == null)
            {
                messageUpdateTimer = new Timer(_diagnosticsUpdateInterval);
                messageUpdateTimer.Elapsed += MessageUpdateTimer_Elapsed;
                messageUpdateTimer.AutoReset = true;
                messageUpdateTimer.Enabled = true;
            }
        }

        private async void MessageUpdateTimer_Elapsed(
            object sender,
            System.Timers.ElapsedEventArgs e
        )
        {
            ViewModel.UpdateMessages();
            await InvokeAsync(StateHasChanged);
        }

        public string DiagnosticsMessage() => "Diag depth : " + DepthValue;

        public int MaxDiagnosticsDepth { get; set; } = 20;
        public static int _depthValue;

        public static int SetDefaultDepth(int item) => _depthValue = item;

        public int DepthValue
        {
            get
            {
                if (_depthValue == 0)
                    _depthValue = ViewModel._tcoObject.MessageHandler.DiagnosticsDepth;
                return _depthValue;
            }
            set
            {
                _depthValue = value;
                ViewModel._tcoObject.MessageHandler.DiagnosticsDepth = value;
            }
        }
    }
}
