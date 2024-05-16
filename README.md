# This is for the Hololens 2 (HARP)

---

## Installation of Necessary Softwares/Toolkits/Packages/Dependencies/SDK

### Getting the required Assets
Git Clone / Pull or Download as Zip in the following GitHub Repository

When opening the project in Unity Hub, it will auto generate the library folder. If there are building errors, download the link below. This link contains a library folder for Unity. Delete the auto generated library folder and move the downloaded library folder into the project folder. Use 7ZIP to extract and move/copy.
[Unity Library](https://myrp-my.sharepoint.com/:u:/g/personal/21045050_myrp_edu_sg/ERu7rjuOp2VOn-WiwVLQV74B5lfmrl5WbHt3mKrMlPSsCg?e=Z8eaRc)

### Installation of Window SDK
Install the Window SDK (Version 10.0.19041.0) needed for Hololens 2. Click [here](https://go.microsoft.com/fwlink/?linkid=2120843) to download if not find the references [here](https://developer.microsoft.com/en-us/windows/downloads/sdk-archive/)

### Installation of Unity Hub
Unity Hub is required to open or run any of the Unity Projects. You can install them by clicking [here](https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe) if not visit the official website [here](https://unity3d.com/get-unity/download)

### Choose your appropriate Unity Version that you need.
For this case we're using **2020.3.33f1** where you can find all the archive [here](https://unity3d.com/get-unity/download/archive)

Modules needed for Unity 2020.3.33f1 to Develop in Hololens 2
- Universal Windows Platform Build Support
- Windows Build Support
- Visual Studio (VS Community 2022 / 2019)

![image](https://user-images.githubusercontent.com/25051402/201806064-b90d99e9-ae9a-4ba3-bff0-f3c956019f6e.png)
![image](https://user-images.githubusercontent.com/25051402/201806166-feb51ed6-af68-427a-b5cd-2b279643137e.png)
![image](https://user-images.githubusercontent.com/25051402/201806250-29ca0947-71f9-409d-9ca0-0ff73720eaf7.png)

### Choose the Visual Studio you prefer to use
Tested with VS Community 2022 and VS Commmunity 2019. You can find these [here](https://visualstudio.microsoft.com/downloads/)

Go to Visual Studio Installer and Download the Necessary Workloads Stated Below

Under Desktop & Mobile Section
- Universal Windows Platform Development
- .NET Desktop Development
- Desktop Development with C++
![image](https://user-images.githubusercontent.com/25051402/201803875-bfa8e8e3-a7d1-469f-b146-a69d337741cd.png)

Under Gaming
- Game Development with Unity
- Game development with C++ 
![image](https://user-images.githubusercontent.com/25051402/201804092-12f338fd-ff86-4305-af80-c1b1605f9223.png)

## Accessing Microsoft Azure
- Click [here](https://drive.google.com/file/d/15F57orJm0PNBIxsntp9Dd3twqtgMlJdz/view?usp=sharing) on how to access the Face API.
- Click [here](https://drive.google.com/file/d/1ZVLyjyVyvmeKURhUbUVCdf1oX0-e8kCY/view?usp=sharing) on Azure Cognitive Services.
- Click [here](https://drive.google.com/file/d/118_0dGlBzryx5-Re-XkdDwV0dGcGtC2R/view?usp=sharing) on Object Detection.

## Object Detection Modular Link
- Go to OneDrive login as lta-holotest@outlook.com.
- Click into the Object Detection Folder.
- Click the XML file and change the link within the tag, text.
![image](https://raw.githubusercontent.com/Vernwalker/Azure/main/ObjectFolder.png)
![image](https://raw.githubusercontent.com/Vernwalker/Azure/main/XMLFile.png)
![image](https://raw.githubusercontent.com/Vernwalker/Azure/main/XMLEditor.png)


### Install MRTK

[MRTK-Unity](https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/?view=mrtkunity-2022-05) is a Microsoft-driven project that provides a set of components and features, used to accelerate cross-platform MR app development in Unity.

You can download the Feature Tool [here](https://www.microsoft.com/en-us/download/details.aspx?id=102778)

To know about the MRTK Toolkit you can read it up [here](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool)

### Unity 3rd Party Packages

- [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity/releases/download/v3.0.5/NugetForUnity.3.0.5.unitypackage) 
- [Spatial Anchor Asset 1](https://github.com/microsoft/MixedRealityLearning/releases/download/getting-started-v2.4.0/MRTK.HoloLens2.Unity.Tutorials.Assets.GettingStarted.2.4.0.unitypackage)
- [Spatial Anchor Asset 2](https://github.com/microsoft/MixedRealityLearning/releases/download/azure-spatial-anchors-v2.5.3.1/MRTK.HoloLens2.Unity.Tutorials.Assets.AzureSpatialAnchors.XRplugginManagement.2.5.3.unitypackage)

### Asset from Unity Store
- [PUN 2 (Photon Engine)](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922)
- [Photon Voice 2](https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518)

---

## Setting up the Working Environment

### Importing from Asset Store
1) Navigate to Window -> Package Manager

![image](https://user-images.githubusercontent.com/25051402/202076991-a4a89df1-87ba-4923-a074-391a836b5b9c.png)

2) Under the Packages, Select 'My Assets' -> Import / Reimport. For this instance is [PUN2](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922) and [Photon Voice 2](https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518).
(If you do not have the assets, go to [Unity Asset Store](https://assetstore.unity.com/) and add it into your asset)

![image](https://user-images.githubusercontent.com/25051402/202077240-07a0d32d-91c3-4084-9060-75ff654c01d1.png)

### Mixed Reality Feature Tool
1) Open up the Mixed Reality Feature Tool
![image](https://user-images.githubusercontent.com/25051402/201810112-bb0d181c-d9a8-4479-b645-c73509794277.png)
2) Select the Github Clone / Pull File
![image](https://user-images.githubusercontent.com/25051402/201810046-ff299027-b204-4fec-9d6e-f1d2adf9b390.png)
3) Discover Feature and Ensure the Following is Installed

 **Azure Mixed Reality Services**
 - Microsoft Azure Object Anchors        (Version 0.22.0)
 - Azure Spatial Anchors SDK Core        (Version 2.13.0)
 - Azure Spatial Anchors SDK for Windows (Version 2.13.0)

 **Mixed Reality Toolkit**
 - Mixed Reality Toolkit Examples        (Version 2.8.0)
 - Mixed Reality Toolkit Extensions      (Version 2.8.0)
 - Mixed Reality Toolkit Foundation      (Version 2.8.0)
 - Mixed Reality Toolkit Standard Assets (Version 2.8.0)
 - Mixed Reality Toolkit Test Utilities  (Version 2.8.0)
 - Mixed Reality Toolkit Tools           (Version 2.8.0)

 **MRTK3**
 - MRTK Graphics Tools                   (Version 0.4.0)

 **Platform Support**
 - Mixed Reality OpenXR Plugin           (Version 1.4.0)
 - Mixed Reality Scene Understanding     (Version 0.6.0)

 **Spatial Audio**
 - Microsoft Spatializer                 (Version 1.0.246)

![image](https://user-images.githubusercontent.com/25051402/201811577-f0673574-dc0e-48be-ad18-c9d46d15b771.png)

5) Follow the remaining steps to install the necessary packages

### Installing and Importing Packages in Unity

Ensure you have NuGet in Unity

![image](https://user-images.githubusercontent.com/25051402/201816239-0c8e1cbd-954e-4f4a-a26a-61c2901c4c41.png)

If not, 
Install the NuGet for Unity by Assets -> Import Package -> Custom Package -> Select [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity/releases/download/v3.0.5/NugetForUnity.3.0.5.unitypackage) you've previously installed 

Go to NuGet -> Manage Nuget Packages -> **Installed** Tab

Make sure the following is there if not navigate back to the **Online** Tab and install it
- Microsoft.MixedReality.QR
- Microsoft.Rest.ClientRuntime
- Microsoft.VCRTForwarders.140
- Microsoft.Windows.MixedReality.DotNetWINRT
- Newtonsoft.Json
- OpenCV

### Linking up Visual Studio and Unity for Auto Complete statements when coding / developing in Visual Studio

Go to Edit -> Preferences -> External Tools

Select the Visual Studio Version you prefer for the External Script Editor 

Under the 'Generate .csproj files for' 
Check the following
- Embedded packages
- Local packages
- Registry packages
- Git packages
- Built-in packages
- Local tarball
- Packages from unknown source
- Player projects

![image](https://user-images.githubusercontent.com/25051402/201814555-b883820b-f0c9-43b9-8ba7-52a8ad66a7fb.png)
![image](https://user-images.githubusercontent.com/25051402/201815209-163efeb2-6fe6-4a0c-a076-237235f14db8.png)

### Setting up the Build Settings in Unity

Go to Files -> Build Settings -> Select Universal Windows Platform (Switch Platform if you haven't)

Follow the Configuration 
- Target Device:            **Hololens**
- Architecture:             **ARM64**
- Target SDK Version:       **Choose the one installed, for this case is 10.0.19041.0**
- Minimum Platform Version: **Choose the lowest, for this case is 10.0.10240.0**
- Visual Studio Version:    **Your Preference, for this case is Latest Installed**
- Build and Run on:         **USB Device**
- Build Configuration:      **Release**

Check Development Build

Rest just leave it as it is

![image](https://user-images.githubusercontent.com/25051402/201820544-46ac0db9-c367-45b2-97c9-173487debf05.png)

---

## Steps to Build / Upload App

1) Go to the Build folder
2) Open up the solution(.sln) folder. Eg. LTA-HARP.sln

![image](https://user-images.githubusercontent.com/25051402/201843278-766819e6-b719-4df4-8fe6-1f93e863ba07.png)

3) Right Click LTA-Harp in the **Solution Explorer**

![image](https://user-images.githubusercontent.com/25051402/201844529-c71a17d6-de34-4928-8cb7-a3f5bd70d713.png)

4) Publish -> Create App Packages -> Sideloading -> Yes, use the current certificate -> Check ARM64 (**Release**) -> Create

![image](https://user-images.githubusercontent.com/25051402/201844823-bb1debc4-b329-4475-8a12-375f5011f5ec.png)

![image](https://user-images.githubusercontent.com/25051402/201845244-cc4c79ce-7a56-41b7-b3b2-16c876c9cc64.png)

![image](https://user-images.githubusercontent.com/25051402/201845311-ba985bf0-113f-4877-87f5-908f6ffdd4bd.png)

![image](https://user-images.githubusercontent.com/25051402/201845629-c312676a-8c55-42fb-a919-57988f1de691.png)

![image](https://user-images.githubusercontent.com/25051402/201845691-aff5e0b4-0b7c-4aa5-a233-02f6335476f2.png)

5) Debug -> <App Name> Debug Properties

![image](https://user-images.githubusercontent.com/25051402/201846071-cea5eb8b-3fba-432b-8971-a42681ed8b1f.png)

 6) Debugging -> Machine Name -> IP Address of Hololens

![image](https://user-images.githubusercontent.com/25051402/201846372-32787c26-420b-4cb3-bb65-f0de5832fb62.png)
 
 7) Configure the Setting to be **Release**, **ARM64** -> Run **Remote Machine** to upload

![image](https://user-images.githubusercontent.com/25051402/201847420-04d22c40-9e90-42ac-94bb-2cee3659e238.png)

 **Please ensure Hololens and the uploading Computer/PC is on the same Network before uploading**

**Takes around 5-15min per Upload**

**Note SilverGate might not be able to communicate with each other even though it is in the same network due to security reasons (Probably)**

