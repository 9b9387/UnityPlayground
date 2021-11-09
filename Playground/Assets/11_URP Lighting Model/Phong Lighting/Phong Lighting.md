Phong光照模型用来表现表面光滑的物体，光滑物体表面相对于粗糙物体表面会呈现更多的镜面反射，而只有很少的漫反射，会使入射光束在反射后还能保持基本一致的方向，当我们顺这个方向观察过去，就会有大量强烈的光线进入眼睛，也就形成了视觉上的高光效果。

在Phong光照模型中Specular项的计算公式如下：
```
Specular = Ks * pow(saturate(dot(reflect(-lightDir, normal), viewDir)), Gloss)
```

- Ks为高光强度
- Gloss为光滑度