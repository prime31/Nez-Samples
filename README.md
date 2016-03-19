# Nez-Samples
Samples and demos of various Nez features


Setup
----
Pull a fresh copy of the Nez-Samples repository. The samples repository has the base Nez repository as a submodule so to fully download everything you need add the `--recursive` paramter when cloning:

`git clone --recursive https://github.com/prime31/Nez-Samples.git`

Visual Studio/Xamarin *should* download the NuGet packages for you when opening the solution. There appears to be a bug with Xamarin where it will not properly download NuGet packages. If this happens open the Nez.sln and it should update the packages. You can then close it and reopen the Nez.Samples.sln and it should now compile.


