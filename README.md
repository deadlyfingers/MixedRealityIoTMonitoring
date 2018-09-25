# Mixed Reality IoT Monitoring Unity sample for HoloLens

## Dependencies
- [HoloToolkit 2017.4.1.0](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/download/2017.4.1.0/HoloToolkit-Unity-2017.4.1.0.unitypackage) (import Unity Package)
- [Azure Function Toolkit](https://github.com/Unity3dAzure/AzureFunctionToolkit)
  (install as git submodule)  
  `git submodule update --init --recursive`
- VuForia (built-in to Unity 2017)  
  Open Unity project and select **GameObject > VuForia > VuMark**

## Setup

1. Install Unity project dependencies listed above
2. Create VuForia VuMarks database and add to Unity project
3. Setup IoT Remote Monitoring sample
4. Deploy Azure Functions backend

*For steps 2-4 please refer to the [developer blog post](http://www.deadlyfingers.net/gamedev/unity3d/custom-vuforia-vumarks-to-identify-iot-devices-with-hololens//)