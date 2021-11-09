Blinn-Phong比Phong性能更高

Phong的缺陷：
- 视线方向和反射向量的角度不允许大于90度，否则点乘的结果是负数，镜面贡献值会变为0


Blinn-Phong放弃使用反射向量，改为基于半程向量（单位向量），它在实现方向和光线方向的中间。
半程向量和表面法线越接近，镜面反射成份就越大。

半程向量是将光线方向和视线向量相加后归一化，计算公式：
```
halfwayDir = normalize(lightDir + viewDir)
```

Blinn-Phong高光部分计算公式
```
Specular = pow(saturate(dot(normal, halfwayDir)), shininess);
```