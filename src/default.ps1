# Borrowed ideas from mvccontrib portableareas project 

properties {
  $projectName = "Health"
  $base_dir = resolve-path ..
  $build_dir = "$base_dir\build"
  $source_dir = "$base_dir\src"

  $buildNumber = 99
}

FormatTaskName "-------- {0} --------"

task default -depends Compile

task Build -depends Clean, Compile, Test

task Compile { 
    msbuild /t:build $source_dir\$projectName.sln 
}

task Clean { 
    delete_directory_if_it_exists $build_dir
	delete_directory_if_it_exists "$build_dir\src\bin"
	delete_directory_if_it_exists "$build_dir\src\obj"
    msbuild /t:clean $source_dir\$projectName.sln 
}

task Test {
	
}

############## HELPER FUNCTIONS #############################

function delete_directory_if_it_exists ($dir)
{
	if (Test-Path $dir)
	{
	  rd $directory_name -recurse -force | out-null	
	}
}

