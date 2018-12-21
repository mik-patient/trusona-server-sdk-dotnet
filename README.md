# Trusona Server SDK

The Trusona Server SDK allows simplified interaction with the Trusona API.


## Table of Contents

1. [Prerequisites](#prerequisites)
   1. [Server SDK API Credentials](#server-sdk-api-credentials)
1. [NuGet Artifactory Setup](#nuget-artifactory-setup)
   1. [Adding the Trusona repository](#adding-the-trusona-repository)
   1. [Installing the Trusona Package](#installing-the-trusona-package)
1. [Integrating the API into a project](#integrating-the-api-into-a-project)
   1. [Creating a Trusona object](#creating-a-trusona-object)
   1. [Registering devices with Trusona](#registering-devices-with-trusona)
      1. [Binding a device to a user](#binding-a-device-to-a-user)
      1. [Activating a device](#activating-a-device)
   1. [Creating Trusonafications](#creating-trusonafications)
      1. [Creating an Essential Trusonafication](#creating-an-essential-trusonafication)
      1. [Creating an Essential Trusonafication, without user presence or a prompt](#creating-an-essential-trusonafication-without-user-presence-or-a-prompt)
      1. [Creating an Essential Trusonafication, with a TruCode](#creating-an-essential-trusonafication-with-a-trucode)
      1. [Creating an Essential Trusonafication, with the user's identifier](#creating-an-essential-trusonafication-with-the-users-identifier)
      1. [Creating an Essential Trusonafication, with the user's email](#creating-an-essential-trusonafication-with-the-users-email)
      1. [Creating an Executive Trusonafication](#creating-an-executive-trusonafication)
   1. [Using TruCode for device discovery](#using-trucode-for-device-discovery)
   1. [Retrieving identity documents](#retrieving-identity-documents)
      1. [Retrieving all identity documents for a user](#retrieving-all-identity-documents-for-a-user)
      1. [Retrieving a specific identity document](#retrieving-a-specific-identity-document)
      1. [Identity document verification statuses](#identity-document-verification-statuses)
      1. [Identity document types](#identity-document-types)
   1. [Retrieving a device](#retrieving-a-device)
   1. [Handling errors](#handling-errors)
   1. [Using a specific Trusona region](#using-a-specific-trusona-region)


## Prerequisites

### Minimum versions
The Trusona SDK targets .NET Standard 2.0. For a comprehensive list of framework compatabilty, please reference the [.NET Standard Compatability Matrix](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)


### Server SDK API Credentials

The Server SDK requires API credentials that are used by the SDK to identify and authenticate requests from your application to the Trusona APIs.

The two credentials required by the SDK include a `token` and `secret`. Both are strings generated and distributed by Trusona.

*NOTE:* The `token` and `secret` should not be shared with anyone. They are how you authenticate to the Trusona services, and you should not check them into source control.


## NuGet Artifactory Setup

The Trusona SDK packages are hosted on the Trusona Artifactory server. Artifactory acts as a NuGet repository that requires credentials to access it. In order to access the Trusona Server SDK, you will need to add the Trusona NuGet repository to your project and configure it to use the Artifactory username and password provided by Trusona. Here is an example of how to configure NuGet to talk to Artifactory.

### Adding the Trusona repository

To configure NuGet to use the Trusona repository, run the following commands. For use in CI envrionments, you may want to configure these as envrioment variables and provide them to your build script at runtime.

```
nuget sources Add -Name Trusona -Source https://trusona.jfrog.io/trusona/api/nuget/nuget-local
```

Alternatively, you may configure NuGet using Visual Studio. To configure the NuGet Visual Studio Extension to use the Trusona repository, you need to add another Package Source under NuGet Package Manager.

1. Access the corresponding repositories in the "Options" window (Options | Tools) and select to add another Package Source.
1. Provide a name for the repository (i.e. Trusona)
1. Paste in https://trusona.jfrog.io/trusona/api/nuget/nuget-local for the URL


### Installing the Trusona Package

In your project, run the following command to install the latest version of the Trusona SDK.

```
nuget install Trusona.SDK
```

Alternatively, you may also search for the NuGet package using Visual Studio. Be sure to select `All Sources` when using the NuGet extension.


## Integrating the API into a project

### Creating a Trusona object

The `Trusona` class is the main class you will interact with to talk to the Trusona APIs. It can be created with the `token` and `secret` provided by [Trusona](#server-sdk-api-credentials).

*NOTE:* The `token` and `secret` should not be shared with anyone. They are how you authenticate to the Trusona services, and you should not check them into source control.

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);
```

You'll also want to make sure the `token` and `secret` values aren't checked in to your project.


### Registering devices with Trusona

To get a device ready to be used with Trusona, there are three main steps:

1.  Create a device
1.  Bind the device to a user
1.  Activate the device

The first step, creating a device, will be handled by the Trusona mobile SDKs on the client. Once a device is created, the Trusona `deviceIdentifier` will need to be sent to your backend which can use the Trusona Server SDK to complete the next steps.


#### Binding a device to a user

When the backend determines which user owns the `deviceIdentifier`, it can bind the `userIdentifier` to the device in Trusona. The `userIdentifier` can be any `String` that allows you to uniquely identify the user in your system. To bind a device to a user, call the `CreateUserDevice` method.

```csharp
var userDevice = await trusona.CreateUserDevice(
  userIdentifier: user.Id,
  deviceIdentifier: "deviceIdentifier"
);
var activationCode = userDevice.ActivationCode;
```

More than one device can be bound to a user and later, when you Trusonafy them, any device bound to that user may accept the Trusonafication. Once the device is bound the user, you'll receive an activation code that can be used later to active the device.


##### Exceptions

|           Exception           |                                                                 Reason                                                                 |
| :---------------------------- | :------------------------------------------------------------------------------------------------------------------------------------- |
| `DeviceNotFoundException`     | Indicates that the request to bind the user to the device failed because the device could not be found.                                |
| `DeviceAlreadyBoundException` | Indicates that the request to bind the user to the device failed because the device is already bound to a different user.              |
| `ValidationException`         | Indicates that the request to bind the user to the device failed because either the `deviceIdentifier` or `userIdentifier` were blank. |
| `TrusonaException`            | Indicates that the request to bind the user to the device failed, check the message to determine the reason.                           |


#### Activating a device

When the device is ready to be activated, call the `ActivateUserDevice` method with the activation code.

```csharp
var result = await trusona.ActivateUserDevice(activationCode);
```

If the request is successful, the device is ready to be Trusonafied.

##### Exceptions

|         Exception         |                                                                     Reason                                                                      |
| :------------------------ | :---------------------------------------------------------------------------------------------------------------------------------------------- |
| `DeviceNotFoundException` | Indicates that the request to activate the device failed because the device could not be found, most likely due to an invalid `activationCode`. |
| `ValidationException`     | Indicates that the request to activate the device failed because the `activationCode` was blank.                                                |
| `TrusonaException`        | Indicates that the request to activate the device failed, check the message to determine the reason.                                            |

### Creating Trusonafications

Once a device is bound to a user, that user can be Trusonafied using the device identifier obtained from the Trusona Mobile SDK.

#### Creating an Essential Trusonafication

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

var request = Trusonafication.Essential()
                             .DeviceIdentifier("PBanKaajTmz_Cq1pDkrRzyeISBSBoGjExzp5r6-UjcI")
                             .Action("login")
                             .Resource("Acme Bank")
                             .Build();

var trusonafication = await trusona.CreateTrusonafication(request);

var result = await trusona.GetTrusonaficationResult(trusonafication.Id);

if(result.IsSuccessful)
{
  // handle successful authentication
}
```

By default, Essential Trusonafications are built such that the user's presence is required and a prompt asking the user to "Accept" or "Reject" the Trusonafication is presented by the Trusona Mobile SDK. A user's presence is determined by their ability to interact with the device's OS Security, usually by using a biometric or entering the device passcode.

#### Creating an Essential Trusonafication, without user presence or a prompt

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

var request = Trusonafication.Essential()
                             .DeviceIdentifier("PBanKaajTmz_Cq1pDkrRzyeISBSBoGjExzp5r6-UjcI")
                             .Action("login")
                             .Resource("Acme Bank")
                             .WithoutUserPresence()
                             .WithoutPrompt()
                             .Build();

var trusonafication = await trusona.CreateTrusonafication(request);

var result = await trusona.GetTrusonaficationResult(trusonafication.Id);

if(result.IsSuccessful)
{
  // handle successful authentication
}
```

In the above example, the addition of `WithoutUserPresence()` and `WithoutPrompt()` on the builder will result in a Trusonafication that can be accepted solely with possession of the device.

#### Creating an Essential Trusonafication, with a TruCode

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

var request = Trusonafication.Essential()
                             .TruCode(Guid.Parse("37086D01-9EF6-4B57-8592-1276C58B1C4D"))
                             .Action("login")
                             .Resource("Acme Bank")
                             .Build();

var trusonafication = await trusona.CreateTrusonafication(request);

var result = await trusona.GetTrusonaficationResult(trusonafication.Id);

if(result.IsSuccessful)
{
  // handle successful authentication
}
```

In this example, instead of specifying a device identifier, you can provide an ID for a TruCode that was scanned by the Trusona Mobile SDK. This will create a Trusonafication for the device that scanned the TruCode. See [Using TruCode for device discovery](#using-trucode-for-device-discovery) below for more information on using TruCodes.

#### Creating an Essential Trusonafication, with the user's identifier

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

var request = Trusonafication.Essential()
                             .UserIdentifier("73CC202D-F866-4C72-9B43-9FCF5AF149BD")
                             .Action("login")
                             .Resource("Acme Bank")
                             .Build();

var trusonafication = await trusona.CreateTrusonafication(request);

var result = await trusona.GetTrusonaficationResult(trusonafication.Id);

if(result.IsSuccessful)
{
  // handle successful authentication
}
```

In some cases you may already know the user's identifier (i.e. in a multi-factor or step-up authentication scenario). This example shows how to issue a Trusonafication using the user's identifier.

#### Creating an Essential Trusonafication, with the user's email

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

var request = Trusonafication.Essential()
                             .EmailAddress("email@example.com")
                             .Action("login")
                             .Resource("Acme Bank")
                             .Build();

var trusonafication = await trusona.CreateTrusonafication(request);

var result = await trusona.GetTrusonaficationResult(trusonafication.Id);

if(result.IsSuccessful)
{
  // handle successful authentication
}
```

In some cases you may be able to send a Trusonafication to a user
by specifying their email address. This is the case if one of the following is true:

- You have verified ownership of a domain through the Trusona Developer's site
- You have an agreement with Trusona allowing you to send Trusonafications to any email address.

Creating a Trusonafication with an email address is similar to the other
use cases, except you use the `EmailAddress()` method rather than `UserIdentifier()` or `DeviceIdentifier()`.

#### Creating an Executive Trusonafication

To create an Executive Trusonafication, call the `Executive` method initially instead of `Essential`.

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

var request = Trusonafication.Executive()
                             .DeviceIdentifier("PBanKaajTmz_Cq1pDkrRzyeISBSBoGjExzp5r6-UjcI")
                             .Action("login")
                             .Resource("Acme Bank")
                             .Build();

var trusonafication = await trusona.CreateTrusonafication(request);

var result = await trusona.GetTrusonaficationResult(trusonafication.Id);

if(result.IsSuccessful)
{
  // handle successful authentication
}
```

Executive Trusonafications require the user to scan an identity document to authenticate. An identity document needs to be registered with the user's account using the Trusona Mobile SDKs before the user can accept an Executive Trusonafication, and they must scan the same document they registered at the time of Trusonafication. Like Essential, both the prompt and user presence features can be used and are enabled by default, but they can be turned off independently by calling `withoutPrompt` or `withoutUserPresence`, respectively.

##### Trusonafication Builder Options

|         Name          | Required | Default |                                           Description                                            |
| :-------------------- | :------: | :-----: | :----------------------------------------------------------------------------------------------- |
| `DeviceIdentifier`    |    N[^1] |  none   | The identifier as generated by the Trusona Mobile SDK.                                           |
| `TruCode`             |    N[^1] |  none   | The ID for a Trucode scanned by the Trusona Mobile SDK.                                          |
| `UserIdentifier`      |    N[^1] |  none   | The identifier of the user that was registered to a device.                                      |
| `EmailAddress`        |    N[^1] |  none   | The email address of the user that was registered to a device.                                   |
| `Action`              |    Y     |  none   | The action being verified by the Trusonafication. (e.g. 'login', 'verify')                       |
| `Resource`            |    Y     |  none   | The resource being acted upon by the user. (e.g. 'website', 'account')                           |
| `ExpiresAt`           |    N     |  null   | An ISO-8601 UTC date that sets the expiration time of the Trusonafication.                       |
| `WithoutUserPresence` |    N     |  false  | Removes the requirement for the user to demonstrate presence when accepting the Trusonafication. |
| `WithoutPrompt`       |    N     |  false  | Removes the requirement for the user to explicityly "Accept" or "Reject" the Trusonafication.    |

[^1]: You must provide at least one field that would allow Trusona to determine which user to authenticate. The identifier fields are `DeviceIdentifier`, `TruCode`, `UserIdentifier`, and `EmailAddress`.

### Using TruCode for device discovery

In the previous section, we demonstrated how to issue a Trusonafication to a specific device using it's `DeviceIdentifier`, but what if the user is trying to login to your website from their desktop computer and you don't know what the user's `DeviceIdentifier` is? That's where TruCode comes in.

#### What is a TruCode?

A TruCode is a short-lived token that can be rendered in the form of a QR code. A Trusona enabled device can scan the QR code and send it's `DeviceIdentifier` to Trusona. Your backend server can then fetch the `DeviceIdentifier` from Trusona and perform a Trusonafication on the device.

#### Rendering a TruCode

To render a TruCode, you can use the Trusona Web SDK. Because TruCodes are short-lived, they need to be refreshed periodically. The Trusona Web SDK will handle the fetching of TruCodes, polling the status to see if they've been paired, refreshing them before they expire, and, when finally paired, return the `TruCodeId` that the backend can use to look up the device identifier.

First get the Web SDK Config for your system from the Server SDK. The Web SDK will need this configuration later when rendering TruCode.

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

var webSdkConfig = trusona.GetWebSdkConfig(); // {"truCodeUrl": "https://example.net", "relyingPartyId": "C97A800D-75E8-43B5-87A5-3282B0DD8576" }
```

Include the trucode.js script tag before the `</body>` of your document

```html
  <!-- existing content -->
  <script type="text/javascript" src="https://static.trusona.net/web-sdk/js/trucode-0.6.13.js"></script>
  </body>
</html>
```

Add an element to your page where you want the TruCode rendered in.

```html
<div id="tru-code"></div>
```

Call the `renderTruCode` function in the Web SDK using the Web SDK Config from the Server SDK.

```html
<script>
  var truCodeConfig = #{webSdkConfig}; // example: {"truCodeUrl": "https://example.net", "relyingPartyId": "C97A800D-75E8-43B5-87A5-3282B0DD8576" }

  Trusona.renderTruCode({
    truCodeConfig: truCodeConfig,
    truCodeElement: document.getElementById('tru-code'),
    onPaired: function(truCodeId) {
      // send the truCodeId to your backend service
    },
    onError: function() {
      // handle if there were errors fetching truCodes
    });
</script>
```

When the TruCode has been scanned by a Trusona enabled device, the `truCodeId` will be passed into the `onPaired` callback where you can relay it to your backend to get the `deviceIdentifier`.


### Retrieving identity documents

Identity documents can be registered using the Trusona Mobile SDK and are required for being able to accept Executive Trusonafications. Depending on your agreement with Trusona, the identity documents may also be verified using a third-party verification system. The Server SDK allows you to get all the identity documents that were registered by a user or to get a specific identity document by ID. This can be useful to see if a user is capable of accepting Executive Trusonafications or to check the result of a third-party verification.

#### Retrieving all identity documents for a user

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

IEnumerable<IdentityDocument> documents = await trusona.FindIdentityDocuments(
  userIdentifier: user.Id
);

if(documents.Any())
{
  // Is capable of accepting Executive Trusonafications
}
else
{
  // Not capable of accepting Executive Trusonafications
}
```

This example shows how to determine if a user is capable of accepting Executive Trusonaficaitons. The call to the `FindIdentityDocuments` method with the `userIdentifier` that was registered with Trusona will return a `IEnumerable<IdentityDocument>`. If the list is empty, then no identity documents have been registered and the user will not be able to scan their document to accept the Trusonafication.

**NOTE:** The `VerificationStatus` of an identity document is not considered during the acceptance of a Trusonafication. If you only want to allow users with `VERIFIED` documents you'll have to check the status prior to issuing the Trusonafication.

#### Retrieving a specific identity document

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

var document = await trusona.GetIdentityDocument(Guid.Parse("4325E135-FA58-4328-8C9C-4F52FC915C21"));
document.VerificationStatus; // UNVERIFIED, UNVERIFIABLE, VERIFIED, or FAILED
```

Here we are getting a specific identity document by ID. Since the ID is generated at the time the document is registered (on the mobile device), you'll have to send the ID to your backend server and then call the `GetIdentityDocument` method in order to check the status. See all [verification statuses](#identity-document-verification-statuses).

#### Identity document properties

|         Name         | Type               |                                           Description                                                                                              |
| :------------------- | :----------------- | :------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Id`                 | Guid               | The ID of the document that was generated when it was registered.                                                                                  |
| `Hash`               | String             | The hash of the raw data of the document that was scanned. Trusona does not store any of the raw information from the original document            |
| `VerificationStatus` | VerificationStatus | The status of the third-party verification that was performed, if any. See all [verification statuses](#identity-document-verification-statuses).  |
| `VerifiedAt`         | Date               | The date when the verification status was determined.                                                                                              |
| `Type`               | String             | The type of the identity document. See all [identity document types](#identity-document-types).                                                    |

#### Identity document verification statuses

| Status           | Description                                                                                                                                                                |
| :--------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `UNVERIFIED `    | Verification of the identity document has not been attempted.                                                                                                              |
| `UNVERIFIABLE `  | Verification of the identity document was attempted, but no verification determination has been made (i.e. the third-party verification was not available in that region). |
| `VERIFIED`       | The document was sucessfully verified.                                                                                                                                     |
| `FAILED`         | The document failed verification.                                                                                                                                          |

#### Identity document types

| Type                     | Description                                        |
| :----------------------- | :------------------------------------------------- |
| `AAMVA_DRIVERS_LICENSE ` | A U.S. or Canada issued driver's license.          |


### Retrieving a device

If you want to check whether or not a device has been activated, or when it was activated, you can look it up in Trusona using the device's identifier.

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret"
);

var device = await trusona.GetDevice("r1ByVyVKJ7TRgU0RPX0-THMTD_CO3VrCSNqLpJFmhms");

if (device.Active) {
  // Device has been activated and can receive/respond to Trusonafications
}
```

### Handling errors

Failed requests get thrown as a `TrusonaException`, which contains a message about what went wrong and what you should do to fix the problem. Some calls may also throw subclasses of `TrusonaException` for scenarios where it might be possible to correct the issue programmatically. It's up to you if you want to handle those specific scenarios or just catch all `TrusonaException`s. If a request fails validation and has error messages for specific fields, a `ValidationException` will get thrown and you can call `getFieldErrors` to inspect the error messages associated with each field that failed.


### Using a specific Trusona region

All users are provisioned in the default region. Unless otherwise noted, you will not need to configure Trusona to use a specific region. If you have been provisioned in a specific region, you will need to point the SDK to use that region. This can be done by passing the appropriate region endpoint to the constructor. For example:

```csharp
var trusona = new Trusona(
  token: "token",
  secret: "secret",
  environment: TrusonaEnvironment.AP_PRODUCTION
);
```


### Need additional help?

Contact us at engineering@trusona.com