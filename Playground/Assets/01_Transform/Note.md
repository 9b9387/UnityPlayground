### Define a Camera
基于右手坐标系
1. 相机世界空间的位置 可以直接使用`GetCameraPositionWS()`获取，实际返回`_WorldSpaceCameraPos`
2. 相机方向 `cameraDirection = targetPositionWS - GetCameraPositionWS()`，可以使用`-GetWorldSpaceViewDir()`获取
3. 相机的右轴 定义一个上向量up(0,1,0)，然后叉乘相机方向，获取右侧向量 cameraRight = cross(up, cameraDirection)
4. 相机的上轴 相机方向叉乘相机的右轴 cameraUp = cross(cameraDirection, cameraRight)

### 视图矩阵
视图矩阵，用于从世界空间转换到视图空间。过程为：
1. 通过平移变换，将Camera移到世界坐标原点位置
2. 通过旋转变换，使Camera看向-z方向且自身的y轴与世界坐标y轴重叠
3. 最后对 Z 分量取反（因为世界空间坐标系是左手坐标系，而视觉空间的坐标系是右手坐标系，所以我们需要对Z分量进行取反）
   
视图矩阵由相机的3个轴外加一个平移向量来构成，可以用视图矩阵乘以任何向量来将其变换到那个坐标空间。

$$
M_{view} = 
\begin{bmatrix}
R_x & R_y & R_z & 0 \\
U_x & U_y & R_z & 0\\
-D_x & -D_y & -D_z & 0\\
0 & 0 & 0 & 1\\
\end{bmatrix} \cdot
\begin{bmatrix}
1 & 0 & 0 & -P_x \\
0 & 1 & 0 & -P_y\\
0 & 0 & 1 & -P_z\\
0 & 0 & 0 & 1\\
\end{bmatrix}
$$

其中R,U,D分别是相机的右，上，前向量，P是摄像机的位置向量。

推导过程参考 

[https://zhuanlan.zhihu.com/p/362713511](https://zhuanlan.zhihu.com/p/362713511)

[https://zhuanlan.zhihu.com/p/93022039](https://zhuanlan.zhihu.com/p/93022039)
### 裁剪空间
对 x,y,z分量进行缩放，用 w 分量做范围值。如果x,y,z都在 w 范围内，那么该点在裁剪空间内

### 正交投影变换
过程为：
1. 平移变换，将长方体平移到原点
2. 缩放变换，将长方体的长宽高缩放到2

$$x_{center}=\frac {r+l}2, y_{center}=\frac {t+b}2, z_{center}=\frac {n+f}2$$
$$M_{ortho}=
\begin{bmatrix}
\frac 2{r-l} & 0 & 0 & 0 \\
0 & \frac 2{t-b} & 0 & 0 \\
0 & 0 & \frac 2{n-f} & 0 \\
0 & 0 & 0 & 1
\end{bmatrix} \cdot
\begin{bmatrix}
0 & 0 & 0 & -x_{center} \\
0 & 0 & 0 & -y_{center} \\
0 & 0 & 0 & -z_{center} \\
0 & 0 & 0 & 1
\end{bmatrix}
$$