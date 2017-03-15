#login to Azure
Login-AzureRmAccount

#create a resource Group
New-AzureRmResourceGroup –Name “pdufrancestorage” –Location “West Europe”

New-AzureRmStorageAccount –ResourceGroup “pdufrancestorage” –StorageAccountName “pdudemostorage”
 –Location “West Europe” –Type “Standard_LRS”