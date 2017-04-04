using UnityEngine;

/// <summary>
/// PerlinNoise のテクスチャを生成し、オブジェクトに設定します。
/// </summary>
public class PerlinNoiseTextureSetter : MonoBehaviour
{
    #region Field

    /// <summary>
    /// 生成するテクスチャの解像度 Width.
    /// </summary>
    public int pixelWidth = 100;

    /// <summary>
    /// 生成するテクスチャの解像度 Height.
    /// </summary>
    public int pixelHeight = 100;

    /// <summary>
    /// PerlinNoise の原点 X (Seed)。
    /// </summary>
    public float perlinNoiseOriginX = 0;

    /// <summary>
    /// PerlinNoise の原点 Y (Seed)。
    /// </summary>
    public float perlinNoiseOriginY = 0;

    /// <summary>
    /// PerlinNoise の拡大率。
    /// </summary>
    public float perlinNoiseScale = 1.0F;

    /// <summary>
    /// PerlinNoise を描画するレンダラ。
    /// </summary>
    private Renderer noiseRenderer;

    /// <summary>
    /// PerlinNoise を描画するテクスチャ。
    /// </summary>
    private Texture2D noiseTexture;

    /// <summary>
    /// テクスチャに設定される色。
    /// </summary>
    private Color[] pixelColors;

    #endregion Field

    /// <summary>
    /// 開始時に呼び出されます。
    /// </summary>
    protected virtual void Start()
    {
        this.noiseRenderer = GetComponent<Renderer>();
        this.noiseTexture  = new Texture2D(this.pixelWidth, this.pixelHeight);
        this.pixelColors   = new Color[this.pixelWidth * this.pixelHeight];

        this.noiseRenderer.material.mainTexture = this.noiseTexture;
    }

    /// <summary>
    /// 更新時に呼び出されます。
    /// </summary>
    protected virtual void Update()
    {
        UpdateTexture();
    }

    /// <summary>
    /// テクスチャを更新します。
    /// </summary>
    protected void UpdateTexture()
    {
        for (float y = 0; y < this.noiseTexture.height; y++)
        {
            for (float x = 0; x < this.noiseTexture.width; x++)
            {
                float xCoord = this.perlinNoiseOriginX + x / noiseTexture.width * this.perlinNoiseScale;
                float yCoord = this.perlinNoiseOriginY + y / noiseTexture.height * this.perlinNoiseScale;
                float value  = Mathf.PerlinNoise(xCoord, yCoord);

                this.pixelColors[(int)(y * this.noiseTexture.width + x)] = new Color(value, value, value);
            }
        }

        this.noiseTexture.SetPixels(this.pixelColors);
        this.noiseTexture.Apply();
    }
}