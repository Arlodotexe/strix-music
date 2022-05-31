#####################################
# About
#####################################
# This is a PowerShell port of Aaron Bernstein's NodeJS code by Arlo Godfrey.
# Unflattens and nests the namespaces in a standard DocFx toc.yaml 
# Source: https://github.com/dotnet/docfx/issues/274#issuecomment-456168196
# Last updated 1/19/2022.
# Licensed under MIT.
#####################################


#####################################
# Usage
#####################################
# docfx metadata ./path/to/docfx.json
# ./unflatten-namespaces.ps1 ./path/to/toc.yaml
# docfx build ./path/to/docfx.json
#####################################


#####################################
# Parameters
#####################################
param ($tocLocation)

$yamlModuleName = "powershell-yaml";

#####################################
# Functions
#####################################

# LoadYml function that will read YML file and deserialize it
function LoadYml {
    param ($FileName)

    # Load file content to a string array containing all YML file lines
    [string[]]$fileContent = Get-Content $FileName
    $content = ''
    # Convert a string array to a string
    foreach ($line in $fileContent) { $content = $content + "`n" + $line }
    # Deserialize a string to the PowerShell object
    $toc = ConvertFrom-YAML $content -Ordered
    # return the object
    return $toc
}

# WriteYml function that writes the YML content to a file
function WriteYml {
    param ($FileName, $Content)

    #Serialize a PowerShell object to string
    Write-Verbose "Serializing content to YAML."
    $result = ConvertTo-YAML $Content

    # Some YAML libraries improperly produce a MappingStart
    # instead of a SequenceStart by excluding the leading "-"
    # This fixes it with regex.
    Write-Verbose "Fixing missing SequenceStart identifiers."
    $result = $result -replace '(items:\s+)  ([^-]+?:)', '$1- $2';

    #write to a file
    Write-Verbose "Saving content to $FileName.";
    Set-Content -Path $FileName -Value $result
}

#####################################
# YAML/Module Loading
#####################################

$isModuleInstalled = Get-InstalledModule $yamlModuleName -ErrorAction silentlycontinue;

if (-not $isModuleInstalled) {
    Write-Verbose "Module $yamlModuleName is not installed, installing."

    # Install and import the yaml reading module
    # Install module has a -Force -Verbose -Scope CurrentUser arguments
    # which might be necessary in your CI/CD environment to install the module
    Install-PackageProvider -Name NuGet -Force -Scope CurrentUser
    Install-Module -Name $yamlModuleName -Confirm:$False -Force -Verbose -Scope CurrentUser
    Import-Module $yamlModuleName -Force -Scope Local
}
else {
    Write-Verbose "Module $yamlModuleName is installed."
}

$tocExists = Test-Path -Path $tocLocation -PathType Leaf

if (-not $tocExists) {
    Write-Output "toc.yml not found at $tocLocation."
    Exit;
}
else {
    Write-Verbose "Found toc.yml."
}

$toc = LoadYml $tocLocation;

#####################################
# Main code
#####################################

$namespaces = @{};

for ($i = 0; $i -lt $toc.Length; $i++) {
    $fullnamespace = $toc[$i].uid;
    $splitnamespace = $fullnamespace.split('.');

    $parent = $namespaces;

    for ($j = 0; $j -lt $splitnamespace.Length; $j++) {
        $partialNamespace = $splitnamespace[$j];

        if ($null -eq $parent[$partialnamespace]) {
            $parent[$partialnamespace] = @{};
        }

        $parent = $parent[$partialnamespace];
    }

    if ($null -eq $parent.items) {
        $parent.items = $toc[$i].items; 
    }
    else {
        $parent.items.push($toc[$i])
    }
}

function recurse {
    param ($obj, $path)

    $items = @(); # Empty array

    foreach ($key in $obj.Keys) {
        $value = $obj[$key];

        if (!($key -eq "items")) {

            $newPath = "";

            if ($null -eq $path) {
                $newPath = $key;
            }
            else {
                $newPath = $path + '.' + $key;
            }

            # null coalescing only available in powershell 7+
            if ($null -eq $value.items) {
                $value.items = @();
            }

            Write-Verbose "Processing $newPath";
            $newObj = [PSCustomObject]@{
                name  = $newPath
                uid   = $newPath
                items = $value.items 
            }

            foreach ($recursedItem in recurse $value $newPath) {
                    $newObj.items = $newObj.items + $recursedItem;
            }

            $items = $items + $newObj;
        }
    }

    return $items;
}

$result = recurse $namespaces;

Write-Verbose "Finished building nested namespace data."

WriteYml $tocLocation $result;

Write-Output "Done."