URP边缘检测

实现方式是：检测深度和法线的连续性，来判断物体边缘。
- 获取摄像机深度贴图

    在URP中UniversalRenderPipelineAsset.asset中开启Depth Texture，就可以在Shader中通过访问`_CameraDepthTexture`获取（另外还可以获取到`_CameraColorTexture`）。
    深度贴图用红色通道表示，物体在场景中的深度值，所以深度贴图一般表现为红色。

- 获取摄像机的法线贴图

    URP获取法线贴图比较麻烦，需要实现一个Scriptable Renderer Feature来生成一个depth+normals的贴图。这个是利用`Hidden/Internal-DepthNormalsTexture`这个Unity内置Shader来实现。

    Feature内部原理是：

    用使用这个Shader的材质，来生成一张RT，然后用`cmd.SetGlobalTexture("_CameraDepthNormalsTexture", depthAttachmentHandle.id);`设置到全局Texture后，释放掉RT。
    
    这样就可以在Shader中，访问`_CameraDepthNormalsTexture`贴图，来获取深度加法线贴图。

    还需要在这个贴图中解析出法线来。在`UnityCG.cginc`文件中的`DecodeViewNormalStereo`包含了具体实现。
    最终就可以获取到法线贴图。

- 连续性检测

    利用屏幕UV，进行相邻四个方向的采样。然后计算法线和深度的变化情况。如果变化超过阈值，则可以判断是边缘位置，返回`max(edgeDepth, edgeNormal)`