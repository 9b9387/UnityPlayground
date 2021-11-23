VRoid对Unity只支持了Built-in渲染管线，并没有支持到URP，这里使用了GitHub的一个第三方仓库：[ShaderGraphsMToonForURPVR](https://github.com/simplestargame/ShaderGraphsMToonForURPVR)

这个仓库使用Shader Graph制作，看了一下实现细节，大致记录一下。
- 基于兰伯特光照 + Step/Smoothstep函数制作明暗交界线
- 用两张贴图（光照，阴影）+兰伯特光照，做明暗表现
- 边缘光使用贴图实现，混合法线贴图和模型法线，点乘视角矩阵，生成范围在\[-1，1\]内UV
- 支持自发光贴图
- 使用深度法线的描边

特点：
- 这个方案只有兰伯特光照，并没有实现高光
- 明暗部分完全通过贴图乘以灯光颜色表现
- 边缘光与光线方向无关，只与视角方向有关
- 虽然Shader中有多光源相关的代码，但实际并没有影响最终效果