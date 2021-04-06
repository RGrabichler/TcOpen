﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortex.Presentation.Wpf;
using Vortex.Connector;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;
using System.Windows;

namespace TcoCore
{
    public class TcoContextViewModel : RenderableViewModel
    {
        public TcoContextViewModel()
        {
            this.UpdateMessagesCommand = new RelayCommand(a => this.OnPropertyChanged(nameof(ActiveMessages)));
        }
       
        private void Reload()
        {
            Tasks = TcoContext.GetChildren<TcoTask>();
            TcoObjectChildren = TcoContext.GetChildren<TcoObject>(Tasks);
            DiagnosticsViewModel = new TcoDiagnosticsViewViewModel(this.TcoContext);
        }

        public TcoContext TcoContext { get; private set; }

        public override object Model { get => TcoContext; set { TcoContext  = value as TcoContext; Reload(); } }

        IEnumerable<TcoTask> tasks;
        public IEnumerable<TcoTask> Tasks
        {
            get => tasks;

            private set
            {
                if (tasks == value)
                {
                    return;
                }

                SetProperty(ref tasks, value);
            }
        }

        public IEnumerable<TcoObject> TcoObjectChildren { get; private set; }       

        public IEnumerable<PlainTcoMessage> ActiveMessages 
        { 
            get
            {
                TcoContext.RefreshActiveMessages();
                return this.TcoContext?.ActiveMessages;
            }
            
        }

        public RelayCommand UpdateMessagesCommand { get; }
        public TcoDiagnosticsViewViewModel DiagnosticsViewModel { get; private set; }
    }
}
