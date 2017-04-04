using UnityEngine;

/// <summary>
/// PerlinNoise を使って上下に動きます。
/// </summary>
public class PerlinNoiseShakeMoverVertical : MonoBehaviour
{
    #region Field

    /// <summary>
    /// PerlinNoise の拡大率。
    /// </summary>
    public float perlinNoiseScale = 3;

    #endregion Field

    /// <summary>
    /// 更新時に呼び出されます。
    /// </summary>
    protected virtual void Update()
    {
        float perlinNoiseValue = Mathf.PerlinNoise(Time.timeSinceLevelLoad, 0) * this.perlinNoiseScale;

        base.transform.position = new Vector3()
        {
            x = 0,
            y = perlinNoiseValue,
            z = 0
        };
    }
}