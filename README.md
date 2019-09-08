# Nez-Samples

Samples and demos of various [Nez](https://github.com/prime31/Nez) features. The Samples repo is built against MonoGame 3.5.


Setup
----
- Pull a fresh copy of the Nez-Samples repository. The samples repository has the base Nez repository as a submodule so to fully download everything you need add the `--recursive` parameter when cloning:

`git clone --recursive https://github.com/prime31/Nez-Samples.git`

- open Nez/Nez.sln and build that first. This is required so that the NuGet packages are downloaded and the pipeline tool DLL is generated.
- now you can open the Nez.Samples.sln and run any of the projects in there



Setup FNA
----
- download the latest release of FNA from [here](https://github.com/FNA-XNA/FNA/releases/tag/19.09) and put it into a folder named `FNA` in the Nez-Samples folder
- download the native libs FNA requires from [here](http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2) into the `fnalibs` folder in the Nez-Samples folder
- open the `Nez.FNA.Samples.sln`


Assets License
----
Unless otherwise noted, the assets in the Nez Samples repo project are not MIT licensed. They should not be used in any project. Most are of unknown copyright/origin so assume they are all off limits and use them only for your own personal amusement.
