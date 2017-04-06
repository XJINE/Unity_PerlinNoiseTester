using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PerlinNoise を使ってオブジェクトを生成して配置します。
/// またオブジェクトの色を配置した位置に応じて変化します。
/// </summary>
public class PerlinNoiseObjectGeneratorWithColor : PerlinNoiseObjectGenerator
{
    #region Struct

    /// <summary>
    /// 色の分布を示すデータ。
    /// </summary>
    protected struct ColorDistributionData
    {
        public Color   color;
        public Vector3 position;
    }

    #endregion Struct

    #region Field

    /// <summary>
    /// オブジェクトに設定される色。
    /// </summary>
    public Color[] objectColors;

    /// <summary>
    /// 分布のシードの数。
    /// </summary>
    public int distributionSeedCount = 8;

    /// <summary>
    /// 分布が近いと判定する距離。
    /// </summary>
    public float nearDistributionThreshold = 10;

    /// <summary>
    /// 色の分布を示すデータのリスト。
    /// </summary>
    protected List<ColorDistributionData> colorDistributionDataList = new List<ColorDistributionData>();

    #endregion Field

    #region Method

    /// <summary>
    /// 開始時に呼び出されます。
    /// </summary>
    protected virtual void Start()
    {
        // シード値を生成します。
        GenerateColorDistributionDataSeed();
    }

    /// <summary>
    /// 色の分布のシード値を生成します。
    /// </summary>
    protected virtual void GenerateColorDistributionDataSeed()
    {
        // (1) 最初のデータを追加します。

        this.colorDistributionDataList.Add(new ColorDistributionData()
        {
            color = this.objectColors[Random.Range(0, this.objectColors.Length)]
        });

        // (2) ある程度の分布データが溜まるまで、ランダムな位置をサンプリングして分布データを更新します。

        int forceBreakCount = 10000;
        int forceBreakCounter = 0;

        while (true)
        {
            Vector3 randomPosition = new Vector3()
            {
                x = Random.value,
                y = Random.value,
                z = 0
            };

            UpdateColorDistributionData(randomPosition);

            if (this.distributionSeedCount <= this.colorDistributionDataList.Count)
            {
                break;
            }

            forceBreakCounter += 1;

            if (forceBreakCount <= forceBreakCounter)
            {
                break;
            }
        }

        Debug.Log(this.GetType() + " : Color DistributionData Seed Count : " + this.colorDistributionDataList.Count);
    }

    /// <summary>
    /// オブジェクトを生成します。
    /// </summary>
    protected override void GenerateObjectPerlinNoise()
    {
        float randomValueX = Random.value;
        float randomValueY = Random.value;
        float noiseValue   = Mathf.PerlinNoise(this.perlinNoiseOriginX + randomValueX * this.perlinNoiseScale,
                                               this.perlinNoiseOriginY + randomValueY * this.perlinNoiseScale);

        if (noiseValue < this.generateObjectThreshold)
        {
            return;
        }

        // (1) オブジェクトを生成して追加します。

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


        // (2) オブジェクトを追加する座標をもとに、分布データを更新して、このオブジェクトの色を決定します。
        //     ここではスクリーン座標系で算出している点に注意してください。

        Vector3 randomPosition = new Vector3()
        {
            x = randomValueX,
            y = randomValueY,
            z = 0
        };

        Color objectColor = UpdateColorDistributionData(randomPosition);

        SetColorToObject(gameObject, objectColor);
    }

    /// <summary>
    /// 指定した座標を使って色の分布のデータを更新し、その座標の色を取得します。
    /// </summary>
    /// <param name="position">
    /// 更新に利用する座標。
    /// </param>
    /// <returns>
    /// 指定した座標に分布する色。
    /// </returns>
    protected virtual Color UpdateColorDistributionData(Vector3 position)
    {
        float minlength = Vector3.SqrMagnitude(position - this.colorDistributionDataList[0].position);
        int   minlengthindex = 0;

        // (1) 既存の分布データの中から近い距離にある分布データを探します。

        for (int i = 1; i < this.colorDistributionDataList.Count; i++)
        {
            float length = Vector3.SqrMagnitude(position - this.colorDistributionDataList[i].position);

            if (length < minlength)
            {
                minlength = length;
                minlengthindex = i;
            }
        }

        // (2) 既存の最寄りの分布データと距離が離れていたら新しい分布データとして採用します。

        ColorDistributionData minLengthColorDistributionData = this.colorDistributionDataList[minlengthindex];
        Color objectColor;

        if (this.nearDistributionThreshold < minlength)
        {
            objectColor = this.objectColors[Random.Range(0, this.objectColors.Length)];

            this.colorDistributionDataList.Add(new ColorDistributionData()
            {
                color = objectColor,
                position = position
            });
        }

        // (3) 既存の分布データと距離が近ければ中間の位置を分布データとして書き換えます。

        else
        {
            objectColor = minLengthColorDistributionData.color;

            this.colorDistributionDataList[minlengthindex] = new ColorDistributionData()
            {
                color = objectColor,
                position = (minLengthColorDistributionData.position + position) / 2,
            };
        }

        return objectColor;
    }

    /// <summary>
    /// GameObject に色を付けます。
    /// </summary>
    /// <param name="gameObject">
    /// 色を付ける GameObject.
    /// </param>
    /// <param name="color">
    /// GameObject に付ける色。
    /// </param>
    protected virtual void SetColorToObject(GameObject gameObject, Color color)
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();

        materialPropertyBlock.SetColor("_Color", color);

        gameObject.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
    }

    #endregion Method
}