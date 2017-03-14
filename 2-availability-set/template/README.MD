This template allows you to create 2 Virtual Machines under an Internal Load balancer

This template also deploys a Storage Account, Virtual Network, Availability Set and Network Interfaces.

The Azure Load Balancer is assigned a static IP in the Virtual Network and is configured to load balance on Port 80.

Steps to deploy manually

Create a resource group: New-AzureRMResourceGroup -Name "ILB-DemoRG" -Location "West Europe"

Modify the 2VMsinVnetWithILB-parameters file to update your subscriptionId and change naming convention

Deploy: New-AzureRMResourceGroupDeployment –Name IPDep1 -ResourceGroupName ILB-DemoRG -TemplateFile 2VMsinVnetWithILB.json -TemplateParameterFile 2VMsinVnetWithILB-parameters.json