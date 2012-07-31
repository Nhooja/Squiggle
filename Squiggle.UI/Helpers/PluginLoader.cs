﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squiggle.Activities;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using Squiggle.Core.Chat;

namespace Squiggle.UI.Helpers
{
    class PluginLoader
    {
        [ImportMany(typeof(IActivityHandlerFactory))]
        public IEnumerable<IActivityHandlerFactory> ActivityHandlerFactories { get; set; }

        public bool VoiceChat { get; private set; }
        public bool FileTransfer { get; private set; }

        public PluginLoader(ComposablePartCatalog catalog)
        {
            var container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);

            VoiceChat = GetHandlerFactory(SquiggleActivities.VoiceChat) != null;
            FileTransfer = GetHandlerFactory(SquiggleActivities.FileTransfer) != null;
        }

        public IActivityHandler GetHandler(Guid activityId, Func<IActivityHandlerFactory, IActivityHandler> getAction)
        {
            IActivityHandlerFactory factory = GetHandlerFactory(activityId);
            if (factory == null)
                return null;
            IActivityHandler handler = getAction(factory);
            return handler;
        }

        IActivityHandlerFactory GetHandlerFactory(Guid activityId)
        {
            IActivityHandlerFactory factory = ActivityHandlerFactories.FirstOrDefault(f => f.ActivityId.Equals(activityId));
            return factory;
        }
    }
}