Tools 
=====

In this section you can find details about various tools that are used during the development process. To work with the app you will need to know how to setup Android Emulators and also how to configure your IDE. 


### Android Virtual Device (Emulator) Configuration

Android's Emulators are a lot faster than they were a few years ago. There has been various optimisations that you should take advantage of to save time during development. You will need to configure multiple emulators during development: normally at least one supporting the lowest API version required by the App, and one supporting the highest. You also need to test on various screen sizes. The screenshot below shows how to configure a Nexus 5 Emulator that is running API version 21 (Lollipop). 


Emulator Config:

![alt text](avd_config.png "Emulator Config")

Things to note:

- Don't use more than 768m of RAM or a larger heap than 64m. This is the minimum requirement for the application and should be enough to get a responsive emulator. Using more than 768m of RAM in Windows is also known to be problematic. 
- Make sure "Use Host GPU" is ticked. This offloads some of the graphics processing to your workstation's GPU. 
- Use the Intel Atom CPU and also see the tip below regarding Intel's virtualization. This allows you to run Android with your workstations CPU, rather than have it emulate the ARM CPU normally found on Android devices. This make things are lot quicker. When using the Intel X86 or X86_64 CPU, you also need to make sure you build the APK so that it supports this architecture. *You should only use this in the dev or debug project configurations* - for production (ie real Android device), only the ARM architecture is currently supported. This is explained more below in the Visual Studio configuration section. 

#### Installing HAX Optimistation

You can insall a small tool from Intel (if you are running an Intel chip on your workstation). You do this through the SDK Manager as shown below. Once downloaded it will be stored in <sdk home>\extras\intel\Hardware_Accelerated_Execution_Manager. Double click the installer and follow the instructions. You may also need to change a setting in your BIOS to enable this optimisation. With HAX installed, you can run Android emulators with the Intel Atom CPU and they will be a lot quicker than 
emulating the ARM CPUs. 

![alt text](sdk_manager_hax.png "HAX App")

