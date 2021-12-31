using UnityEngine;

public class TransformMatrix : MonoBehaviour
{
    public Material material;
    public Transform c;
    private Matrix4x4 cameraInfo;
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        cameraInfo = Matrix4x4.zero;
        camera = c.GetComponent<Camera>();
    }

    [ContextMenu("Test")]
    private void Test()
    {
        Debug.Log(camera.aspect);
        Debug.Log($"camera\n {camera.projectionMatrix}");
        Debug.Log($"GL\n {GL.GetGPUProjectionMatrix(camera.projectionMatrix, true)}");
        //Debug.Log($"Matrix4x4\n {Matrix4x4.Ortho(0, 1, 0, 1/camera.aspect, camera.nearClipPlane, camera.farClipPlane)}");
        //Debug.Log($"OrthoMatrix\n {OrthoMatrix(-camera.orthographicSize * camera.aspect, camera.orthographicSize * camera.aspect, -camera.orthographicSize, camera.orthographicSize, camera.nearClipPlane, camera.farClipPlane) == camera.projectionMatrix}");
        //Debug.Log($"OrthoMatrix\n {OrthoMatrix(0, camera.orthographicSize, 0, camera.orthographicSize / camera.aspect, camera.nearClipPlane, camera.farClipPlane)}");
    }


    // Update is called once per frame
    void Update()
    {
        cameraInfo.m00 = c.right.x;
        cameraInfo.m01 = c.right.y;
        cameraInfo.m02 = c.right.z;
        cameraInfo.m10 = c.up.x;
        cameraInfo.m11 = c.up.y;
        cameraInfo.m12 = c.up.z;
        cameraInfo.m20 = c.forward.x;
        cameraInfo.m21 = c.forward.y;
        cameraInfo.m22 = c.forward.z;
        cameraInfo.m30 = c.position.x;
        cameraInfo.m31 = c.position.y;
        cameraInfo.m32 = c.position.z;
        material.SetMatrix("_CameraInfo", cameraInfo);

        var m = ViewMatrix(c.position, c.position + c.forward, c.up);
        material.SetMatrix("_MyViewMatrix", m);

        //Debug.Log(camera.worldToCameraMatrix == m);
        //Debug.Log(camera.projectionMatrix);
        //var o = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true);
        //var o1 = OrthoMatrix(0, Screen.width, 0, Screen.height, camera.nearClipPlane, camera.farClipPlane);
        //Debug.Log(GL.GetGPUProjectionMatrix(o1, true));
        //Debug.Log(o);
        //Debug.Log(o2);
        var size = camera.orthographicSize;
        var aspect = camera.aspect;
        //var o = OrthoMatrix(-size * aspect, size * aspect, -size, size, camera.nearClipPlane, camera.farClipPlane);
        //material.SetMatrix("_MyOrthoMatrix", GL.GetGPUProjectionMatrix(o, true));
        var o = PerspMatrix(camera.fieldOfView, camera.aspect, camera.nearClipPlane, camera.fieldOfView);

        material.SetMatrix("_MyOrthoMatrix", GL.GetGPUProjectionMatrix(o, true));
    }

    [ContextMenu("TestLookAtMatrix")]
    public void TestLookAtMatrix()
    {
        var positions0 = MockRandomVectorArray(0f, 50f);
        var positions1 = MockRandomVectorArray(0f, 50f);
        var positions2 = MockRandomVectorArray(0f, 50f);

        for (int i = 0; i < positions0.Length; i++)
        {
            var p0 = positions0[i];
            var p1 = positions1[i];
            var p2 = positions2[i];

            var matrix_lookat_1 = Matrix4x4.LookAt(p0, p1, p2.normalized);
            var matrix_lookat_2 = (ViewMatrix(p0, p1, p2.normalized));

            if (matrix_lookat_1 != matrix_lookat_2)
            {
                Debug.LogWarning($"Error, i={i} m1=\n{matrix_lookat_1} m2=\n{matrix_lookat_2}");
            }
            else
            {
                Debug.Log($"{i} Pass!");
            }
        }
    }

    [ContextMenu("TestTranslateMatrix")]
    public void TestTranslateMatrix()
    {
        var positions = MockRandomVectorArray(0f, 50f);

        for (int i = 0; i < positions.Length; i++)
        {
            var p = positions[i];
            var matrix_t_1 = Matrix4x4.Translate(p);
            var matrix_t_2 = TranslateMatrix(p);

            if (matrix_t_1 != matrix_t_2)
            {
                Debug.LogWarning($"Error, i={i} m1={matrix_t_1} m2={matrix_t_2}");
            }
            else
            {
                Debug.Log($"{p} Pass!");
            }
        }
    }

    [ContextMenu("TestScaleMatrix")]
    public void TestScale()
    {
        var scale_array = MockRandomVectorArray(0f, 30f);

        for (int i = 0; i < scale_array.Length; i++)
        {
            var s = scale_array[i];
            var matrix_s_1 = Matrix4x4.Scale(s);
            var matrix_s_2 = ScaleMatrix(s);

            if (matrix_s_1 != matrix_s_2)
            {
                Debug.LogWarning($"Error, i={i} m1={matrix_s_1} m2={matrix_s_2}");
            }
            else
            {
                Debug.Log($"{s} Pass!");
            }
        }
    }

    [ContextMenu("TestRotateMatrix")]
    public void TestRotateMatrix()
    {
        var angle_array = MockRandomVectorArray(0f, 360f);

        for (int i = 0; i < angle_array.Length; i++)
        {
            var s = angle_array[i];
            var q = Quaternion.Euler(s);
            var matrix_r_1 = Matrix4x4.Rotate(q);
            var matrix_r_2 = RotateMatrix(q);
            var matrix_r_3 = RotateMatrix2(s);

            if (matrix_r_1 != matrix_r_2)
            {
                Debug.LogWarning($"Error, i={i} m1={matrix_r_1} m2={matrix_r_2}");
            }
            else if(matrix_r_1 != matrix_r_3)
            {
                Debug.LogWarning($"Error, i={i} m1={matrix_r_1} m3={matrix_r_3}");
            }
            else
            {
                Debug.Log($"{s} Pass!");
            }
        }
    }

    private Matrix4x4 OrthoMatrix(float l, float r, float b, float t, float n, float f)
    {
        // 投影
        var m = Matrix4x4.identity;
        m.m00 = 2 / (r - l);
        m.m11 = 2 / (t - b);
        m.m22 = -2 / (f - n);

        m.m03 = -(r + l) / (r - l);
        m.m13 = -(t + b) / (t - b);
        m.m23 = -(f + n) / (f - n);

        return m;
    }

    private Matrix4x4 PerspMatrix(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
    {
        var m = Matrix4x4.zero;
        var ySscale = 1.0f / Mathf.Tan(fieldOfView / 2f * Mathf.Deg2Rad);
        var xSscale = ySscale / aspectRatio;

        m.m00 = xSscale;
        m.m11 = ySscale;
        m.m22 = -(farPlaneDistance + nearPlaneDistance) / (farPlaneDistance - nearPlaneDistance);
        m.m23 = -(2 * nearPlaneDistance * farPlaneDistance) / (farPlaneDistance - nearPlaneDistance);
        m.m32 = -1;
        return m;
    }


    private Matrix4x4 ViewMatrix(Vector3 from, Vector3 to, Vector3 up)
    {
        // Unity的World Space下物体的forward方向(transform.forward)都是z轴的正方向，
        // 而到了Camera Space下则匹配了OpenGL的传统，使Camera看向了z轴负方向。
        // 之后投影变换到裁剪空间时，还会再做一次z反转，使near在-1，far在1
        // 这样操作的目的是，Unity中的投影矩阵可以和OpenGL一致，
        // OpenGL的投影矩阵是右手坐标系，应用到Unity的左手坐标系，直接就做了z反转。
        // 参考 https://docs.unity.cn/2021.1/Documentation/ScriptReference/Camera-worldToCameraMatrix.html
        // 所以下面计算相机方向的向量基于右手坐标系

        // 摄像机forward向量
        var cameraForward = -Vector3.Normalize(from - to);
        // 摄像机right向量
        var cameraRight = Vector3.Normalize(Vector3.Cross(up, cameraForward));
        // 相机的up向量
        var cameraUp = Vector3.Cross(cameraForward, cameraRight);

        var m_r = Matrix4x4.identity;
        m_r.m00 = cameraRight.x;
        m_r.m01 = cameraRight.y;
        m_r.m02 = cameraRight.z;
        m_r.m10 = cameraUp.x;
        m_r.m11 = cameraUp.y;
        m_r.m12 = cameraUp.z;
        m_r.m20 = -cameraForward.x;
        m_r.m21 = -cameraForward.y;
        m_r.m22 = -cameraForward.z;

        var m_t = Matrix4x4.identity;
        m_t.m03 = -from.x;
        m_t.m13 = -from.y;
        m_t.m23 = -from.z;

        return m_r * m_t;
    }

    /// <summary>
    /// 生成一个长度为8，值随机的Vector3数组
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private Vector3[] MockRandomVectorArray(float from, float to)
    {
        float x = Random.Range(from, to);
        float y = Random.Range(from, to);
        float z = Random.Range(from, to);
        return new Vector3[8]
        {
            new Vector3(-x, y, z),
            new Vector3(x, -y, z),
            new Vector3(x, y, -z),
            new Vector3(-x, -y, z),
            new Vector3(x, -y, -z),
            new Vector3(-x, y, -z),
            new Vector3(x, y, z),
            new Vector3(-x, -y, -z),
        };
    }

    [ContextMenu("TestTRSMatrix")]
    public void TestTRSMatrix()
    {
        var position_array = MockRandomVectorArray(0f, 50f);
        var angle_array = MockRandomVectorArray(0f, 360f);
        var sacle_array = MockRandomVectorArray(0f, 10f);

        for (int i = 0; i < angle_array.Length; i++)
        {
            var p = position_array[i];
            var a = angle_array[i];
            var q = Quaternion.Euler(a);
            var s = sacle_array[i];
            var matrix_trs_1 = Matrix4x4.TRS(p, q, s);
            var matrix_trs_2 = TranslateMatrix(p) * RotateMatrix(q) * ScaleMatrix(s);

            if (matrix_trs_1 != matrix_trs_2)
            {
                Debug.LogWarning($"Error, i={i} m1={matrix_trs_1} m2={matrix_trs_2}");
            }
            else
            {
                Debug.Log($"{i} Pass!");
            }
        }
    }

    /// <summary>
    /// 平移矩阵
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Matrix4x4 TranslateMatrix(Vector3 pos)
    {
        var m = Matrix4x4.identity;
        m.m03 = pos.x;
        m.m13 = pos.y;
        m.m23 = pos.z;
        return m;
    }

    /// <summary>
    /// 缩放矩阵
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    private Matrix4x4 ScaleMatrix(Vector3 scale)
    {
        var m = Matrix4x4.identity;
        m.m00 = scale.x;
        m.m11 = scale.y;
        m.m22 = scale.z;
        return m;
    }

    /// <summary>
    /// 四元数旋转矩阵
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    private Matrix4x4 RotateMatrix(Quaternion q)
    {
        float x = q.x * 2.0F;
        float y = q.y * 2.0F;
        float z = q.z * 2.0F;
        float xx = q.x * x;
        float yy = q.y * y;
        float zz = q.z * z;
        float xy = q.x * y;
        float xz = q.x * z;
        float yz = q.y * z;
        float wx = q.w * x;
        float wy = q.w * y;
        float wz = q.w * z;

        var m = Matrix4x4.zero;
        m.m00 = 1.0f - (yy + zz);
        m.m10 = xy + wz;
        m.m20 = xz - wy;
        m.m01 = xy - wz;
        m.m11 = 1.0f - (xx + zz);
        m.m21 = yz + wx;
        m.m02 = xz + wy;
        m.m12 = yz - wx;
        m.m22 = 1.0f - (xx + yy);
        m.m33 = 1.0F;
        return m;
    }

    /// <summary>
    /// 三角函数的旋转矩阵
    /// </summary>
    /// <param name="angel"></param>
    /// <returns></returns>
    private Matrix4x4 RotateMatrix2(Vector3 angel)
    {
        float radX = Mathf.Deg2Rad * angel.x;
        float radY = Mathf.Deg2Rad * angel.y;
        float radZ = Mathf.Deg2Rad * angel.z;

        float sinX = Mathf.Sin(radX);
        float cosX = Mathf.Cos(radX);
        float sinY = Mathf.Sin(radY);
        float cosY = Mathf.Cos(radY);
        float sinZ = Mathf.Sin(radZ);
        float cosZ = Mathf.Cos(radZ);

        var m = Matrix4x4.zero;
        m.m00 = cosY * cosZ;
        m.m01 = -cosY * sinZ;
        m.m02 = sinY;
        m.m03 = 0.0f;
        m.m10 = cosX * sinZ + sinX * sinY * cosZ;
        m.m11 = cosX * cosZ - sinX * sinY * sinZ;
        m.m12 = -sinX * cosY;
        m.m13 = 0.0f;
        m.m20 = sinX * sinZ - cosX * sinY * cosZ;
        m.m21 = sinX * cosZ + cosX * sinY * sinZ;
        m.m22 = cosX * cosY;
        m.m23 = 0.0f;
        m.m30 = 0.0f;
        m.m31 = 0.0f;
        m.m32 = 0.0f;
        m.m33 = 1.0f;
        return m;
    }
}
