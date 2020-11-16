
using UnityEngine;
using UnityEngine.Rendering;

public class CustomPipeline : RenderPipeline
{
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        //全部相机逐次渲染
        foreach (var camera in cameras)
        {
            //设置渲染相关相机参数,包含相机的各个矩阵和剪裁平面等
            context.SetupCameraProperties(camera);

            //1.剪裁：这边应该是相机的视锥剪裁相关。
            //自定义一个剪裁参数，cullParam类里有很多可以设置的东西。我们先简单采用相机的默认剪裁参数。
            ScriptableCullingParameters cullParam = new ScriptableCullingParameters();
            //直接使用相机默认剪裁参数
            camera.TryGetCullingParameters(out cullParam);
            //非正交相机
            cullParam.isOrthographic = false;
            //获取剪裁之后的全部结果(其中不仅有渲染物体，还有相关的其他渲染要素)
            CullingResults cullResults = context.Cull(ref cullParam);

            //2.渲染设置：渲染时，会牵扯到渲染排序，所以先要进行一个相机的排序设置，这里Unity内置了一些默认的排序可以调用
            SortingSettings sortSet = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
            //这边进行渲染的相关设置，需要指定渲染的shader的光照模式(就是这里，如果shader中没有标注LightMode的
            //话，使用该shader的物体就没法进行渲染了)和上面的排序设置两个参数
            DrawingSettings drawSet = new DrawingSettings(new ShaderTagId("Always"), sortSet);

            //3.过滤：这边是指定渲染的种类(对应shader中的Rendertype)和相关Layer的设置(-1表示全部layer)
            FilteringSettings filtSet = new FilteringSettings(RenderQueueRange.opaque, -1);
            context.DrawRenderers(cullResults, ref drawSet, ref filtSet);

            //绘制天空球
            context.DrawSkybox(camera);
            //开始执行上下文
            context.Submit();
        }
    }
}
