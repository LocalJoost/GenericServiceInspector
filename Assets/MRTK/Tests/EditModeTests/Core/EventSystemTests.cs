﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Diagnostics;
using NUnit.Framework;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode
{
    class EventSystemTests
    {
        /// <summary>
        /// A basic test that validates that access to selectedObject doesn't throw a NullReferenceException
        /// </summary>
        [Test]
        public void TestSelectedObjectAccess()
        {
            Assert.DoesNotThrow(() =>
            {
                DiagnosticsEventData eventData = new DiagnosticsEventData(EventSystem.current);
                eventData.Initialize(null);
                Assert.IsNull(eventData.selectedObject);
            });
        }
    }
}
