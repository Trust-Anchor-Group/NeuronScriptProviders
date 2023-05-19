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
| [Waher.Script.Graph](https://www.nuget.org/packages/Waher.Script.Graph/)           | Defines script extensions for graphs and images. |

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

## Creating script-based services

Script-based services are created by calling any of the following script functions. You only need to call the function once. The
service will be available to users during the runtime of the Neuron(R), or until a new service with the same ID is created. 

| Function                       | Description |
|:-------------------------------|:------------|
| `BuyEDalerService(Definition)` | Creates a script-based service for buying eDaler. |

The `Definition` argument available in all calls to the functions above, contain an object definition with a set of properties
and methods (i.e. lambda expressions). Some of these are required, some are optional. Following tables list the properties and
methods that are recognized by the different functions. Names are case-insensitive.

| BuyEDalerService(Definition)                             ||||
| Name                 | Use      | Type                | Description |
|:---------------------|:--------:|:--------------------|:------------|
| `Id`                 | Required | String              | Machine-readable ID of service |
| `Name`               | Required | String              | Human-readable name of service. |
| `IconUrl`            | Optional | String              | URL to icon representing service. |
| `IconWidth`          | Optional | 0 <= Number <= 2048 | Width, in pixels, of icon representing service. |
| `IconHeight`         | Optional | 0 <= Number <= 2048 | Height, in pixels, of icon representing service. |
| `Icon`               | Optional | Image, Graph        | Alternative to providing `IconUrl`, `IconWidth` or `IconHeight`. |
| `ContractTemplateId` | Optional | String              | Contract ID of template to use, if a smart contract is required to collect infomration for the operation. |
| `TemplateId`         |          |                     | Alias for `ContractTemplateId` |
| `ContractId`         |          |                     | Alias for `ContractTemplateId` |
| `Supports`           | Optional | λ(String)           | How well the service supports a given currency. Result can be a boolean value or a Grade enumerated value. |
| `Support`            |          |                     | Alias for `Supports` |
| `CanBuyEDaler`       | Optional | λ(String)           | If a given account (given the acclunt name) is allowed to buy eDaler using the service. Result can be a boolean value or a Grade enumerated value. |
| `CanBuy`             |          |                     | Alias for `CanBuyEDaler` |
| `GetOptions`         | Optional | λ(Request)          | Gets payment options to present to the user. The `Reqeust` argument is of type [`OptionsRequest`](TAG.Payments.Script/Providers/BuyEDaler/OptionsRequest.cs). The result is an array of objects ex-nihilo, each object representing an option, and each field or property in the object represents a contract parameter and value. |
| `BuyEDaler`		   | Required | λ(Request)          | Performs the requested task to buy eDaler using the service. The `Reqeust` argument is of type [`BuyRequest`](TAG.Payments.Script/Providers/BuyEDaler/BuyRequest.cs). Result can be number with the amount actually bought, or a physical quantity, with the unit representing the currency used, and the magnitude the amount. If a string is returned, it is considered an error, and no eDaler is assumed to have been bought. |
| `Buy`				   |          |                     | Alias for `BuyEDaler` |

## .config-files

`.config`-files are XML files that are loaded when the server starts. It can include, apart from service configurations, various 
types of script. `InitializationScript` are run once, when the service is installed, and then once, every time the service is
updated. `StartupScript` on the other hand, contain script that is executed every time the Neuron(R) starts, or when the service
is updated. This makes it a good place to define script-based services.

Following, is a basic example showing the structure of a `.config` file containing startup script:

```
<?xml version="1.0" encoding="utf-8"?>
<ServiceConfiguration xmlns="http://waher.se/Schema/ServiceConfiguration.xsd">
	<StartupScript>
		<![CDATA[
		BuyEDalerService(
		{
			"Id":"Mock",
			"Name":"Mock payment service",
			"IconUrl":"https://upload.wikimedia.org/wikipedia/en/6/62/Kermit_the_Frog.jpg",
			"IconWidth":282,
			"IconHeight":353,
			"ContractTemplateId":"2bfab592-1c24-d090-7419-d51e36f8377f@legal.lab.tagroot.io",
			"Supports":(Currency)->true,
			"CanBuyEDaler":(AccountName)->true,
			"GetOptions":(Request)->
			(
				Sleep(2000),
				[
					{
						"Account":"Account 1",
						"Message":"First"
					},
					{
						"Account":"Account 2",
						"Message":"Second"
					},
					{
						"Account":"Account 1",
						"Message":"Third"
					}
				]
			),
			"BuyEDaler":(Request)->
			(
				Sleep(5000);
				Error("Kermit took all the money and ran.")
			)
		});
		]]>
	</StartupScript>
</ServiceConfiguration>
```

## Examples

The solution contains a set of examples to illustrate how different services can be created. The examples reside in the
[Examples](Examples) folder. The following examples are availsble:

| File Name                                               | Description |
|:--------------------------------------------------------|:------------|
| [Mock.BuyEDaler.config](Examples/Mock.BuyEDaler.config) | Mock service that publishes a simple service for buying eDaler, including payment options. |
| [Mock.BuyEDaler.xml](Examples/Mock.BuyEDaler.xml)       | Mock smart contract used by the example service defined in [Mock.BuyEDaler.config](Examples/Mock.BuyEDaler.config). |
