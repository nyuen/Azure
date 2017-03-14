#login to azure
Login-AzureRmAccount

#create a resource group
$resourceGroupName = "AzureDemo2"

New-AzureRmResourceGroup -Name $resourceGroupName -Location 'West Europe'

#Create a subnet definition
$newSubnetParams = @{
    'Name' = 'MySubnet'
    'AddressPrefix' = '10.0.1.0/24'
}
$subnet = New-AzureRmVirtualNetworkSubnetConfig @newSubnetParams

#create a Vnet
$newVNetParams = @{
    'Name' = 'MyNetwork'
    'ResourceGroupName' = $resourceGroupName
    'Location' = 'West Europe'
    'AddressPrefix' = '10.0.0.0/16'
}
$vNet = New-AzureRmVirtualNetwork @newVNetParams -Subnet $subnet

#create a storage account
$storageAccountName = "pdufrancedemo2"
$newStorageAcctParams = @{
    'Name' = $storageAccountName ## Must be globally unique and all lowercase
    'ResourceGroupName' = $resourceGroupName
    'Type' = 'Standard_LRS'
    'Location' = 'West Europe'
}
$storageAccount = New-AzureRmStorageAccount @newStorageAcctParams

#create a new public IP
$newPublicIpParams = @{
    'Name' = 'MyPublicIP'
    'ResourceGroupName' = $resourceGroupName
    'AllocationMethod' = 'Dynamic' ## Dynamic or Static
    'DomainNameLabel' = 'pdufrancedemoazurevm2'
    'Location' = 'West Europe'
}
$publicIp = New-AzureRmPublicIpAddress @newPublicIpParams

#create a NIC for the VM, the nic is bound to the subnet
$newVNicParams = @{
    'Name' = 'MyNic'
    'ResourceGroupName' = $resourceGroupName
    'Location' = 'West Europe'
}
$vNic = New-AzureRmNetworkInterface @newVNicParams -SubnetId $vNet.Subnets[0].Id -PublicIpAddressId $publicIp.Id

#create the VM configuration
$newConfigParams = @{
    'VMName' = 'MyVM'
    'VMSize' = 'Standard_A2'
}
$vmConfig = New-AzureRmVMConfig @newConfigParams

#configure the OS parameters and authentication
$newVmOsParams = @{
    'Windows' = $true
    'ComputerName' = 'MyVM'
    'Credential' = (Get-Credential -Message 'Type the name and password of the local administrator account.')
    'ProvisionVMAgent' = $true
    'EnableAutoUpdate' = $true
}

$vm = Set-AzureRmVMOperatingSystem @newVmOsParams -VM $vmConfig

#configure the OS image
$newSourceImageParams = @{
    'PublisherName' = 'MicrosoftWindowsServer'
    'Version' = 'latest'
    'Skus' = '2012-R2-Datacenter'
}
 
$offer = Get-AzureRmVMImageOffer -Location 'West Europe' –PublisherName 'MicrosoftWindowsServer'
 
$vm = Set-AzureRmVMSourceImage @newSourceImageParams -VM $vm -Offer $offer.Offer[1]

#attache the NIC to the VM
$vm = Add-AzureRmVMNetworkInterface -VM $vm -Id $vNic.Id

#configure the Disk settings to point the vhd to the storage account and use the OS
$osDiskName = 'myDisk'
$osDiskUri = $storageAccount.PrimaryEndpoints.Blob.ToString() + "vhds/" + $vmName + $osDiskName + ".vhd"
 
$newOsDiskParams = @{
    'Name' = 'OSDisk'
    'CreateOption' = 'fromImage'
}
 
$vm = Set-AzureRmVMOSDisk @newOsDiskParams -VM $vm -VhdUri $osDiskUri

#last step, create the vm
New-AzureRmVM -VM $vm -ResourceGroupName $resourceGroupName -Location 'West Europe'
