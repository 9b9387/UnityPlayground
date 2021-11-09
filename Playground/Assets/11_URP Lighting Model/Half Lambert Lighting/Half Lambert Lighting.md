Lambert光照模型有一个很大的缺点——物体的背面不受光的地方是全黑的。

Half Lamber光照模型是对Lamber光照模型做一次Remap运算，将光照亮度[0, 1]提升到[a, 1](1 > a > 0)之间。

Half Lambert光照模型的通用公式就是：

```
DiffuseLight = Kd * I * (dot(N, L) * a + (1 - a)) (0<a<1)
```

