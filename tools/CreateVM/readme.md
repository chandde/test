A tool to create VM easily in one step,

Usage
1. Download the script
2. in powershell, import it
```poweshell
. .\createvm.ps1
```
3. CreateVM
```powershell
CreateVM -VMName UIServerBuild5 -VMRootFolder D:\hyperv -VHDSize 200GB -VMGen 2 -ISOPath 'F:\ISO\Windows 10 20H2.iso' -CpuCount 6
```

Configuration
1. specify your ISO path, the script also add Dvd to the VM and set the Dvd as boot drive automatically
2. dynamic memory is turned on by default
3. after configuration it'll boot the VM automatically
