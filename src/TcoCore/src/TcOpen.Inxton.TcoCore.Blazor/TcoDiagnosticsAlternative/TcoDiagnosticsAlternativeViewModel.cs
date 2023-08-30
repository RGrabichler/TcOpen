﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcoCore.TcoDiagnosticsAlternative.LoggingToDb;
using TcOpen.Inxton.Input;
using Vortex.Presentation;


namespace TcoCore
{
    public class TcoDiagnosticsAlternativeViewModel : RenderableViewModelBase
    {
        private readonly IMongoLogger _logger;

        public TcoDiagnosticsAlternativeViewModel()
        {
            // Default constructor
        }

        public TcoDiagnosticsAlternativeViewModel(IMongoLogger logger)
        {
            _logger = logger;
        }

        public TcoDiagnosticsAlternativeViewModel(IsTcoObject tcoObject)
        {
            _tcoObject = tcoObject;
        }

        public TcoDiagnosticsAlternativeViewModel(IMongoLogger logger, IsTcoObject tcoObject)
        {
            _logger = logger;
            _tcoObject = tcoObject;
        }



        public RelayCommand UpdateMessagesCommand { get; private set; }

        internal IsTcoObject _tcoObject { get; set; }

        public override object Model { get => this._tcoObject; set => this._tcoObject = value as IsTcoObject; }

        public object Categories
        {
            get
            {
                return Enum.GetValues(typeof(eMessageCategory)).Cast<eMessageCategory>().Skip(1);
            }
        }
        public eMessageCategory MinMessageCategoryFilter
        {
            get;
            set;
        } = DefaulCategory;

        public static eMessageCategory SetDefaultCategory(eMessageCategory item) => DefaulCategory = item;

        public static eMessageCategory DefaulCategory { get; set; } = eMessageCategory.Info;


      
        bool diagnosticsRunning;

        public bool DiagnosticsRunning
    {
        get => diagnosticsRunning;
        internal set
        {
            if (diagnosticsRunning == value)
            {
                return;
            }

            SetProperty(ref diagnosticsRunning, value);
        }
    }

        //========================
        public event Action<PlainTcoMessage> NewMessageReceived;

        private volatile object updatemutex = new object();

        private bool isBusyLogging = false;

        internal void UpdateMessages()
        {
            isBusyLogging = true;

            if (DiagnosticsRunning)
            {
                return;
            }

            lock (updatemutex)
            {
                DiagnosticsRunning = true;

                Task.Run(() =>
                {
                    MessageDisplay = _tcoObject.MessageHandler.GetActiveMessages()
                        .Where(p => p.CategoryAsEnum >= MinMessageCategoryFilter)
                        .OrderByDescending(p => p.Category)
                        .OrderBy(p => p.TimeStamp);
                }).Wait();

                foreach (var message in MessageDisplay)
                {
                    if (!_logger.MessageExistsInDatabase(message.Identity, message.TimeStamp) || !isBusyLogging)
                    {
                        _logger.LogMessage(message);
                    }
                }
                isBusyLogging = false;
                DiagnosticsRunning = false;
            }
        }

        public void AcknowledgeAllMessages()
        {
            foreach (var message in MessageDisplay)
            {
                DateTime currentDateTime = DateTime.Now;
                if (message.TimeStampAcknowledged < new DateTime(1980, 1, 1, 0, 0, 0)) // Or whatever condition you use to check if a message is not acknowledged
                {
                    _logger.UpdateMessage(message.Identity, message.TimeStamp, currentDateTime);
                    RefreshMessageDisplay();
                }
            }
        }

        public void RefreshMessageDisplay()
        {
            MessageDisplay = _logger.ReadMessages();
        }



        public List<PlainTcoMessage> Messages { get; private set; } = new List<PlainTcoMessage>();

         IEnumerable<PlainTcoMessage> messageDisplay = new List<PlainTcoMessage>();

        public IEnumerable<PlainTcoMessage> MessageDisplay
        {
            get => messageDisplay;

            private set
            {
                if (messageDisplay == value)
                {
                    return;
                }

                SetProperty(ref messageDisplay, value);
            }

        }

        public void FetchMessagesFromDb()
        {
            var latestMessages = _logger.ReadMessages(); // Assuming _logger is an instance of MongoLogger
            MessageDisplay = latestMessages; // Update the MessageDisplay
        }

        public void UpdateAndFetchMessages()
        {
            UpdateMessages();
            FetchMessagesFromDb();
        }

    }

}
