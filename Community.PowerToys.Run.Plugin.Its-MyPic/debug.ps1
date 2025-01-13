Push-Location
Set-Location $PSScriptRoot

try{
	sudo powershell {
		Start-Job { Stop-Process -Name PowerToys* } | Wait-Job
		sleep 1

		# change this to your PowerToys installation path
		$ptPath = 'C:\Users\jeffp\AppData\Local\PowerToys'
		$project = 'Its-MyPic'
		$debug = '.\bin\x64\Debug\net8.0-windows'
		$dest = "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins\$project"
		$files = @(
			"Community.PowerToys.Run.Plugin.$project.deps.json",
			"Community.PowerToys.Run.Plugin.$project.dll",
			'plugin.json',
			'data.json',
			'Images'
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