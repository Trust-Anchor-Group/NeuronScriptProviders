NeuronScriptProviders
=========================

Installable module that allows you to different types of service providers on a Neuron. Service providers you can define in 
[script](https://lab.tagroot.io/Script.md) using this module include providers that:

* let users buy eDaler(R), 
* sell eDaler(R), 
* perform currency conversion, 
* perform full KyC on identity applications, 
* or partial, via the peer review process.

## Projects

The solution contains the following C# projects:

| Project               | Framework         | Description |
|:----------------------|:------------------|:------------|
| `TAG.Payments.Script` | .NET Standard 2.0 | Service module for the [TAG Neuron](https://lab.tagroot.io/Documentation/Index.md), permitting developers to develop integrations with service providers using [script](https://lab.tagroot.io/Script.md). |

## Nugets

The following nugets external are used. They faciliate common programming tasks, and
enables the libraries to be hosted on an [IoT Gateway](https://github.com/PeterWaher/IoTGateway).
This includes hosting the bridge on the [TAG Neuron](https://lab.tagroot.io/Documentation/Index.md).
They can also be used standalone.

| Nuget                                                                              | Description |
|:-----------------------------------------------------------------------------------|:------------|
| [Paiwise](https://www.nuget.org/packages/Paiwise)                                  | Contains services for integration of financial services into Neurons. |
| [Waher.Events](https://www.nuget.org/packages/Waher.Events/)                       | An extensible architecture for event logging in the application. |
| [Waher.IoTGateway](https://www.nuget.org/packages/Waher.IoTGateway/)               | Contains the [IoT Gateway](https://github.com/PeterWaher/IoTGateway) hosting environment. |
| [Waher.Runtime.Inventory](https://www.nuget.org/packages/Waher.Runtime.Inventory/) | Maintains an inventory of type definitions in the runtime environment, and permits easy instantiation of suitable classes, and inversion of control (IoC). |
| [Waher.Script](https://www.nuget.org/packages/Waher.Script/)                       | Defines an extensible [script language](https://lab.tagroot.io/Script.md) and processor. The installable package extends the script language with functions that make it possible to publish integrations with service providers using only [script](https://lab.tagroot.io/Script.md). |

## Installable Package

The `TAG.Payments.Script` project has been made into a package that can be downloaded and installed on any 
[TAG Neuron](https://lab.tagroot.io/Documentation/Index.md).
To create a package, that can be distributed or installed, you begin by creating a *manifest file*. The
`TAG.Payments.Script` project has a manifest file called `TAG.Payments.Script.manifest`. It defines the
assemblies and content files included in the package. You then use the `Waher.Utility.Install` and `Waher.Utility.Sign` command-line
tools in the [IoT Gateway](https://github.com/PeterWaher/IoTGateway) repository, to create a package file and cryptographically
sign it for secure distribution across the Neuron network.

The service is published as a package on TAG Neurons. If your neuron is connected to this network, you can install the
package using the following information:

| Package information ||
|:-----------------|:----|
| Package          | `TAG.ScriptProviders.package` |
| Installation key | TBD |
| More Information | TBD |

## Building, Compiling & Debugging

The repository assumes you have the [IoT Gateway](https://github.com/PeterWaher/IoTGateway) repository cloned in a folder called
`C:\My Projects\IoT Gateway`, and that this repository is placed in `C:\My Projects\NeuronScriptProviders`. You can place the
repositories in different folders, but you need to update the build events accordingly. To run the application, you select the
`TAG.Payments.Script` project as your startup project. It will execute the console version of the
[IoT Gateway](https://github.com/PeterWaher/IoTGateway), and make sure the compiled files of the `NeuronScriptProviders` solution
is run with it.
