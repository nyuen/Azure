$rgName = "niyuenvssc"
$vmssName = "niyuenvss"

#Get info about the scale set
$vmss = Get-AzureRmVmss -ResourceGroupName $rgName -VMScaleSetName $vmssName

#update the capacity of the VMSS
#$vmss.sku.capacity = 5
#Update-AzureRmVmss -ResourceGroupName $rgName -Name $vmssName -VirtualMachineScaleSet $vmss

#get autoscale settings
Get-AzureRmAutoscaleSetting -ResourceGroup $rgName -Name "cpuautoscale"

#remove existing autoscale rule
Remove-AzureRmAutoscaleSetting -ResourceGroup $rgName -Name "cpuautoscale" 

#add new autoscale rule
$rule1 = New-AzureRmAutoscaleRule -MetricName "Percentage CPU" -MetricResourceId $vmss.Id -Operator GreaterThan -MetricStatistic Average -Threshold 60 -TimeGrain 00:01:00 -TimeWindow 00:10:00 -ScaleActionCooldown 00:10:00 -ScaleActionDirection Increase -ScaleActionValue 1
$rule2 = New-AzureRmAutoscaleRule -MetricName "Percentage CPU" -MetricResourceId $vmss.Id -Operator LessThan -MetricStatistic Average -Threshold 30 -TimeGrain 00:01:00 -TimeWindow 00:10:00 -ScaleActionCooldown 00:10:00 -ScaleActionDirection Decrease -ScaleActionValue 1
$profile1 = New-AzureRmAutoscaleProfile -DefaultCapacity 2 -MaximumCapacity 10 -MinimumCapacity 2 -Rules $rule1,$rule2 -Name "My_Profile"
Add-AzureRmAutoscaleSetting -Location "West Europe" -Name "mynewautoscale" -ResourceGroup $rgName -TargetResourceId $vmss.Id -AutoscaleProfiles $profile1

