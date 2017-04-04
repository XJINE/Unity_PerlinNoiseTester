using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PerlinNoise を使ってオブジェクトを生成して配置します。
/// </summary>
public class PerlinNoiseObjectGenerator : MonoBehaviour
{
    #region Field

    /// <summary>
    /// 生成するオブジェクトの最大数。
    /// </summary>
    public int objectCountMax = 500;

    /// <summary>
    /// 生成したオブジェクトのリスト。
    /// </summary>
    public List<GameObject> objectList = new List<GameObject>();

    /// <summary>
    /// すべてのオブジェクトを削除するとき true.
    /// </summary>
    public bool removeAllObject = false;

    /// <summary>
    /// 生成されたオブジェクトの大きさ。
    /// </summary>
    public float objectScale = 0.2f;

    /// <summary>
    /// PerlinNoise の原点 X (Seed)。
    /// </summary>
    public float perlinNoiseOriginX = 0;

    /// <summary>
    /// PerlinNoise の原点 Y (Seed)。
    /// </summary>
    public float perlinNoiseOriginY = 0;

    /// <summary>
    /// PerlinNoise のスケール。
    /// </summary>
    public float perlinNoiseScale = 10;

    /// <summary>
    /// オブジェクトを生成する閾値。
    /// PerlinNoise でこの値を超えたときにオブジェクトを生成する。
    /// </summary>
    public float generateObjectThreshold = 0.7f;

    #endregion Field

    #region Method

    /// <summary>
    /// 更新時に呼び出されます。
    /// </summary>
    protected virtual void Update ()
    {
        if (this.objectCountMax <= this.objectList.Count)
        {
            GameObject.Destroy(this.objectList[0]);
            this.objectList.RemoveAt(0);
        }

        if (this.removeAllObject)
        {
            RemoveAllObject();
            this.removeAllObject = false;
        }

        //GenerateObjectRandom();
        GenerateObjectPerlinNoise();
    }

    /// <summary>
    /// すべてのオブジェクトを削除します。
    /// </summary>
    protected virtual void RemoveAllObject()
    {
        for (int i = this.objectList.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(this.objectList[i]);
            this.objectList.RemoveAt(i);
        }
    }

    /// <summary>
    /// オブジェクトを生成します。
    /// </summary>
    protected virtual void GenerateObjectRandom()
    {
        Camera camera      = Camera.main;
        float randomValueX = Random.value;
        float randomValueY = Random.value;

        Vector3 randomPoint  = new Vector3(randomValueX * camera.pixelWidth,
                                           randomValueY * camera.pixelHeight,
                                           0);
        Vector3 worldPoint   = Camera.main.ScreenToWorldPoint(randomPoint);
                worldPoint.z = camera.transform.position.z + (camera.farClipPlane - camera.nearClipPlane) / 2;

        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        gameObject.transform.position = worldPoint;
        gameObject.transform.localScale *= this.objectScale;
        gameObject.transform.SetParent(base.gameObject.transform);

        this.objectList.Add(gameObject);
    }

    /// <summary>
    /// オブジェクトを生成します。
    /// </summary>
    protected virtual void GenerateObjectPerlinNoise()
    {
        float randomValueX = Random.value;
        float randomValueY = Random.value;
        float noiseValue   = Mathf.PerlinNoise(this.perlinNoiseOriginX + randomValueX * this.perlinNoiseScale,
                                               this.perlinNoiseOriginY + randomValueY * this.perlinNoiseScale);

        if (noiseValue < this.generateObjectThreshold)
        {
            return;
        }

        Camera camera = Camera.main;

        Vector3 randomPoint  = new Vector3(randomValueX * camera.pixelWidth,
                                           randomValueY * camera.pixelHeight,
                                           0);
        Vector3 worldPoint   = Camera.main.ScreenToWorldPoint(randomPoint);
                worldPoint.z = camera.transform.position.z + (camera.farClipPlane - camera.nearClipPlane) / 2;

        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        gameObject.transform.position = worldPoint;
        gameObject.transform.localScale *= this.objectScale;
        gameObject.transform.SetParent(base.gameObject.transform);

        this.objectList.Add(gameObject);
    }

    #endregion Method
}