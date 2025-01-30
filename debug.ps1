Push-Location
Set-Location $PSScriptRoot

try{
	sudo powershell {
		Start-Job { Stop-Process -Name PowerToys* } | Wait-Job
		sleep 1

		# change this to your PowerToys installation path
		$ptPath = "$env:LOCALAPPDATA\PowerToys"
		$project = 'Its-MyPic'
		$debug = '.\bin\x64\Debug\net8.0-windows'
		$dest = "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins\$project"
		$files = @(
			"Community.PowerToys.Run.Plugin.$project.deps.json",
			"Community.PowerToys.Run.Plugin.$project.dll",
			"Community.PowerToys.Run.Plugin.Update.dll",
			'plugin.json',
			"update.ps1",
			"Images",
			'data'
		)

		Set-Location $debug
		mkdir $dest -Force -ErrorAction Ignore | Out-Null
		Copy-Item $files $dest -Force -Recurse

		& "$ptPath\PowerToys.exe"
	}
}
catch{
	Write-Host $_.Exception.Message
	pause
}

Pop-Location