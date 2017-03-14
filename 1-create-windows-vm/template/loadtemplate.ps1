#login to Azure
Login-AzureRmAccount

#set your subscription if you have multiple ones
#Set-AzureRmContext -SubscriptionID <YourSubscriptionId>


#create a resource group in west us
$resourceGroupName = "AzureTrainingDemo1"
New-AzureRmResourceGroup -Name $resourceGroupName -Location "West Europe"

# The Test-AzureRmResourceGroupDeployment cmdlet enables you to find problems before creating actual resources.
# The following example shows how to validate a deployment.
Test-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile azuredeploy.json

# Deploy the template
New-AzureRmResourceGroupDeployment -Name "demoniyuen1234" -ResourceGroupName $resourceGroupName -TemplateFile azuredeploy.json
