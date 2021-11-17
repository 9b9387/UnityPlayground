### 渲染方程（The Rendering Equation）
渲染方程的物理基础是能量守恒定律。在一个特定的位置和方向，出射光 Lo 是自发光 Le 与反射光线之和，反射光线本身是各个方向的入射光 Li 之和乘以表面反射率及入射角。

$$L_{out} = L_{emission} + \int_{\Omega}f_r \cdot L_i \cdot (w_i \cdot n) \cdot dw_i $$

$f_r$是p点入射方向到出射方向光的反射比例，一般为BRDF。

在实时渲染中，常用到的反射方程（The Reflectance Equation）是渲染方程的简化版本:
$$L_0 = \int_{\Omega}f_r \cdot L_i \cdot (w_i \cdot n) \cdot dw_i$$


### Microfacet Cook-Torrance BRDF反射模型

游戏业界目前最主流的基于物理的镜面反射BRDF模型是基于微平面理论（microfacet theory）的`Microfacet Cook-Torrance BRDF`。

因为游戏和电影中的大多数物体都是不透明的，用BRDF就完全足够，BRDF最为简单，也最为常用。而BSDF、BTDF、BSSRDF往往更多用于半透明材质和次表面散射材质。

Cook-Torrance BRDF公式为：
$$f_r = k_df_{diffuse}+k_sf_{sepcular}$$
其中
$$k_d+k_s=1$$
$f_{specular}$表示高光反射部分，其公式为：
$$f_{specular}=\frac {FDG}{4(n \cdot l)(n \cdot v)}$$

`F`表示菲涅尔方程（Fresnel Equation），通常使用`Fresnel-Schlick`算法获取近似解，不过介质间相对IOR接近1时，Schlick近似误差较大，这时可以考虑使用精确的菲涅尔方程。
$$F \approx F_{schlick}$$
$$F_{schlick} = F_0+(1-F_0)(1-(n \cdot v))^5$$
其中，$F_0$为基础反射率，是一个常数，Unity URP中定义为0.04。

`D`表示法线分布函数（Normal Distribution Function, NDF）主流的法线分布函数是`Trowbridge-Reitz`，因为具有更好的高光长尾。
$$D=\frac {\alpha^2}{\pi((n \cdot h)^2(\alpha ^2-1)+1)^2}$$

`G`表示几何函数（Geometry Function）描述微平面自成阴影的属性，G分为两个独立的部分：光线方向（light）和视线方向（view），并对两者用相同的分布函数来描述。流行的算法为`Schlick-GGX`用较低的性能消耗获取接近的表现。
$$G \approx G_{schlickGGX}$$
$$G_{schlickGGX}=G_1(l)G_1(v)$$
$$G_1(v) = \frac {(n \cdot v)}{(n \cdot v)(1-k)+k}$$ 
其中
$$k=\frac {\alpha}{2}, \alpha=(\frac {roughness+1}{2})^2$$

### URP针对BRDF的优化
在翻看Unity UPR(v10.5.1)的源码时，发现URP在实现BRDF时，针对移动设备做了一些优化，最主要的是将$G \cdot F$函数简化为：
$$GF = \frac {1}{(n \cdot h)^2(roughness + 0.5)}$$
具体细节查看参考资料`Optimizing PBR for Mobile`, Siggraph 2015，对应的简化方程也有一个名字叫`Minimalist CookTorrance BRDF`。

另外，Fresnel-Schlick方程也将Pow5改为Pow4进行了一个小优化。

#### 参考资料
- [基于物理的渲染（PBR）白皮书](https://zhuanlan.zhihu.com/p/53086060)
- [理解PBR：从原理到实现](https://neil3d.github.io/unreal/pbr-theory.html)
- [A Reflectance Model for Computer Graphics](https://graphics.pixar.com/library/ReflectanceModel/paper.pdf)
- [PBR Diffuse Lighting for GGX+Smith Microsurfaces](https://ubm-twvideo01.s3.amazonaws.com/o1/vault/gdc2017/Presentations/Hammon_Earl_PBR_Diffuse_Lighting.pdf)
- [Optimizing PBR for Mobile](https://community.arm.com/events/1155)