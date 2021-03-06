﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Shared;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Client.Samples
{
    public class TwinSample
    {
        private readonly DeviceClient _deviceClient;

        public TwinSample(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient ?? throw new ArgumentNullException(nameof(deviceClient));
        }

        public async Task RunSampleAsync()
        {
            await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChangedAsync, null).ConfigureAwait(false);

            Console.WriteLine("Retrieving twin...");
            Twin twin = await _deviceClient.GetTwinAsync().ConfigureAwait(false);

            Console.WriteLine("\tInitial twin value received:");
            Console.WriteLine($"\t{twin.ToJson()}");

            Console.WriteLine("Sending sample reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["RoadGoesOnForever"] = true;

            await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);

            Console.WriteLine("Waiting 90 seconds for IoT Hub Twin updates...");
            Console.WriteLine($"Use the IoT Hub Azure Portal to change the Twin desired properties within this time.");

            await Task.Delay(90 * 1000);
        }

        private async Task OnDesiredPropertyChangedAsync(TwinCollection desiredProperties, object userContext)
        {
            Console.WriteLine("\tDesired property changed:");
            Console.WriteLine($"\t{desiredProperties.ToJson()}");

            Console.WriteLine("\tSending new desired propery as reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["PartyEnds"] = desiredProperties["PartyEnds"];

            await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
        }
    }
}
