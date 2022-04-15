// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace ManageResourceGroup
{
    public class Program
    {
        private static ResourceIdentifier? _resourceGroupId = null;
        /**
         * Azure Resource sample for managing resource groups -
         * - Create a resource group
         * - Update a resource group
         * - Create another resource group
         * - List resource groups
         * - Delete a resource group.
         */
        public static async Task RunSample(ArmClient client)
        {
            // change the value here for your own
            var rgName = "rgRSMA";
            var rgName2 = "rgRSMA";
            var resourceTagName = "rgRSTN";
            var resourceTagValue = "rgRSTV";
            
            try
            {
                //=============================================================
                // Create resource group.

                Console.WriteLine($"Creating a resource group with name: {rgName}");

                var subscription = await client.GetDefaultSubscriptionAsync();
                var rgLro = await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, rgName, new ResourceGroupData(AzureLocation.WestUS));
                var resourceGroup = rgLro.Value;
                _resourceGroupId = resourceGroup.Id;

                Console.WriteLine($"Created a resource group with name: {resourceGroup.Id}");

                //=============================================================
                // Update the resource group.

                Console.WriteLine($"Updating the resource group: {resourceGroup.Id}");

                resourceGroup = await resourceGroup.AddTagAsync(resourceTagName, resourceTagValue);

                Console.WriteLine($"Updated the resource group: {resourceGroup.Id}");

                //=============================================================
                // Create another resource group.

                Console.WriteLine($"Creating another resource group with name: {rgName2}");

                rgLro = await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, rgName2, new ResourceGroupData(AzureLocation.WestUS));
                var resourceGroup2 = rgLro.Value;

                Console.WriteLine($"Created another resource group: {resourceGroup2.Id}");

                //=============================================================
                // List resource groups.

                Console.WriteLine("Listing all resource groups");

                foreach (var rGroup in subscription.GetResourceGroups())
                {
                    Console.WriteLine($"Resource group: {rGroup.Id}");
                }

                //=============================================================
                // Delete a resource group.

                Console.WriteLine($"Deleting resource group: {resourceGroup2.Id}");

                await resourceGroup2.DeleteAsync(WaitUntil.Completed);

                Console.WriteLine($"Deleted resource group: {resourceGroup2.Id}");
            }
            finally
            {
                try
                {
                    if (_resourceGroupId is not null)
                    {
                        Console.WriteLine($"Deleting Resource Group: {_resourceGroupId}");
                        await client.GetResourceGroupResource(_resourceGroupId).DeleteAsync(WaitUntil.Completed);
                        Console.WriteLine($"Deleted Resource Group: {_resourceGroupId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static async Task Main(string[] args)
        {
            try
            {
                //=================================================================
                // Authenticate
                var credential = new DefaultAzureCredential();

                var subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");
                // you can also use `new ArmClient(credential)` here, and the default subscription will be the first subscription in your list of subscription
                var client = new ArmClient(credential, subscriptionId);

                await RunSample(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}