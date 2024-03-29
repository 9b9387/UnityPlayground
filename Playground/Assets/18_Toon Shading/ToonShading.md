## 卡通渲染

### 描边

《Real-Time Rendering, Fourth Edition》15.2 Outline Rendering中定义的边的种类：
- 边界或边缘，不会被两个三角形共用
- 折痕，特征边缘，由两个三角形共享的边
- 材质边，由两个不同的材质造成的阴影变化产生的边缘
- 等高线边缘(contour)，两个相邻三角形相对于视线方向朝向不同的方向，由视图方向定义
- 轮廓边缘(silhouette)，用于图像平面中将物体从背景中分离出来，由视图方向定义

#### 1. 基于视角的描边
使用法线和视线方向的点积来检测边缘。

$$Normal \cdot View$$

如果这个值接近于零，那么法线方向就和视线方向接近垂直，因此很可能就是轮廓边缘。这种方法的缺点是，轮廓线宽度取决于曲面的曲率，所以描边的粗细很难单独控制。

这种算法有很多局限性，而且表现并不理想。

#### 2. 基于几何的描边
这个算法使用两个Pass，先正常渲染对象，然后再做一次正面剔除来渲染背面，产生边缘。
渲染边缘通比较常用的做法：
- 三角形壳技术（triangle shell technique）将顶点沿着法线方向向外移动，产生一个外壳。
- 三角形膨胀（Triangle fattening）将边缘向外移动一定距离，然后连接这些边，可以避免顶点远离原始三角形。
- z-bias 把背面顶点的Z值稍微向前偏移一点点，使得背面的些许部分显示出来形成描边效果。

```
Demo: Back Facing Outline

实现方式使用三角形壳技术（triangle shell technique），几个关键点
- position.w值表示深度，用于减小远处物体描边的效果
- 使用_ScreenParams.x，_ScreenParams.y参数调节宽高比例
```

#### 3. 基于图片后处理
这种算法是利用深度图和法线图，对周围坐标进行采样，如果采样结果间有明显的变化，说明这里边缘位置。

```
Demo: DepthNormal Outline

由于URP没有办法直接获取屏幕法线贴图，这里编写了一个`DepthNormalsFeature`来获取屏幕法线贴图。
然后，同时检查深度和法线的连续性，判断边缘。
```

### 光照
卡通光照的特点是：
- 减少色阶数量
- 有明显的明暗交界线（Terminator Line）
- 边缘光（Rim Light）

#### 减少色阶数量，通常有两种常用方法：
- Step/Smoothstep函数
- RampMap贴图
  
#### 边缘光
- Fresnel
- Depth Offset

#### 明暗交界线
通过两个Smoothstep，一正一反相乘，就可以找到明暗交界线的位置。

```
Demo: Toon Diffuse Lighting
简单的卡通光照的实现
```

#### 高光
参考论文《Stylized Highlights for Cartoon Rendering》
