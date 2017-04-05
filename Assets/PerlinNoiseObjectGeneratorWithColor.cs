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
    /// 分布のシードの数。
    /// </summary>
    public int distributionSeedCount = 8;

    /// <summary>
    /// 分布が近いと判定する距離。
    /// </summary>
    public float nearDistributionThreshold = 10;

    /// <summary>
    /// テクスチャインデックスの分布を示すデータのリスト。
    /// </summary>
    protected List<ColorDistributionData> colorDistributionDataList = new List<ColorDistributionData>();

    #endregion Field

    #region Method

    /// <summary>
    /// 開始時に呼び出されます。
    /// </summary>
    protected virtual void Start()
    {

    }

    /// <summary>
    /// 更新時に呼び出されます。
    /// </summary>
    protected virtual void Update()
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

//using UnityEngine;
//using System.Collections.Generic;

//namespace Sagaya.Effects.FlowerTable
//{
//    /// <summary>
//    /// テーブルの上に皿が置かれたとき、花が咲くエフェクト(グローバル)。
//    /// </summary>
//    [System.Serializable]
//    public class Effect_FlowerTableGlobal_Tsutuji : Effect_FlowerTableGlobal
//    {
//        #region Struct

//        /// <summary>
//        /// テクスチャインデックスの分布を示すデータ。
//        /// </summary>
//        protected struct TextureIndexDistributionData
//        {
//            public int textureIndex;
//            public Vector3 position;
//        }

//        #endregion Struct

//        #region Field

//        /// <summary>
//        /// 分布のシードの数。
//        /// </summary>
//        public int distributionSeedCount = 8;

//        /// <summary>
//        /// 分布が近いと判定する距離。
//        /// </summary>
//        public float nearDistributionThreshold = 10;

//        /// <summary>
//        /// テクスチャインデックスの分布を示すデータのリスト。
//        /// </summary>
//        protected List<TextureIndexDistributionData> textureIndexDistributionDataList = new List<TextureIndexDistributionData>();

//        #endregion Field

//        #region Method

//        #region EffectBaseStateMachine

//        /// <summary>
//        /// StateEnterCallback メソッドから呼び出され実行される処理です。
//        /// </summary>
//        protected override void StateEnterCallbackFunction()
//        {
//            switch (GetStateID())
//            {
//                case State.c_EffectInit:
//                    {
//                        this.flowerTableEffectManager = FlowerTableEffectManager.Instance;
//                        GenerateTextureIndexDistributionDataSeed();
//                        break;
//                    }
//                case State.f_OutroAnim:
//                    {
//                        // すべての花を散らせます。
//                        this.flowerManager.SetFlowersToDie(this.maxWaitTimeSecWhenFlowerDieInOutro);
//                        break;
//                    }
//            }
//        }

//        #endregion EffectBaseStateMachine

//        /// <summary>
//        /// テクスチャの分布データのシード値を生成します。
//        /// </summary>
//        protected virtual void GenerateTextureIndexDistributionDataSeed()
//        {
//            Vector3 bloomPosition = GetBloomPosition();

//            // (0) 最初のデータを追加します。

//            while (true)
//            {
//                bloomPosition = GetBloomPosition();

//                if (bloomPosition.x == Effect_FlowerTableGlobal.FlowerPositionInvalidValue.x)
//                {
//                    continue;
//                }

//                this.textureIndexDistributionDataList.Add(new TextureIndexDistributionData()
//                {
//                    textureIndex = this.flowerManager.GetRandomMainTextureIndex(),
//                    position = bloomPosition
//                });

//                break;
//            }

//            while (true)
//            {
//                // (1) ランダムに咲く位置を取得します。

//                bloomPosition = GetBloomPosition();

//                if (bloomPosition.x == Effect_FlowerTableGlobal.FlowerPositionInvalidValue.x)
//                {
//                    continue;
//                }

//                // (2) 既存の分布データの中から近い距離にあるデータを探します。

//                float length = 0;
//                float minLength = Vector3.SqrMagnitude
//                    (bloomPosition - this.textureIndexDistributionDataList[0].position);

//                for (int i = 1; i < this.textureIndexDistributionDataList.Count; i++)
//                {
//                    length = Vector3.SqrMagnitude
//                        (bloomPosition - this.textureIndexDistributionDataList[i].position);

//                    if (length < minLength)
//                    {
//                        minLength = length;
//                    }
//                }

//                // (3) ある程度距離が離れていたらシード値として採用します。

//                if (this.nearDistributionThreshold < minLength)
//                {
//                    this.textureIndexDistributionDataList.Add(new TextureIndexDistributionData()
//                    {
//                        textureIndex = this.flowerManager.GetRandomMainTextureIndex(),
//                        position = bloomPosition
//                    });
//                }

//                // (4) ある程度シードが溜まったら終了します。

//                if (this.distributionSeedCount <= this.textureIndexDistributionDataList.Count)
//                {
//                    break;
//                }
//            }
//        }

//        /// <summary>
//        /// 花を咲かせます。
//        /// </summary>
//        protected override void BloomFlower()
//        {
//            // (0) 無効な位置に咲かそうとしていたら終了します。

//            Vector3 bloomPosition = GetBloomPosition();

//            if (bloomPosition.x == Effect_FlowerTableGlobal.FlowerPositionInvalidValue.x)
//            {
//                return;
//            }

//            int textureIndexDistributionDataListCount = this.textureIndexDistributionDataList.Count;
//            int textureIndex;


//            float minLength = Vector3.SqrMagnitude
//                (bloomPosition - this.textureIndexDistributionDataList[0].position);

//            int minLengthIndex = 0;

//            // (1) 既存の分布データの中から最短距離にあるデータを取得します。

//            for (int i = 1; i < textureIndexDistributionDataListCount; i++)
//            {
//                float length = Vector3.SqrMagnitude
//                    (bloomPosition - this.textureIndexDistributionDataList[i].position);

//                if (length < minLength)
//                {
//                    minLength = length;
//                    minLengthIndex = i;
//                }
//            }

//            // (2) 最短距離にあるデータが、一定の距離以下にあるかを検出します。

//            // (2.1) 近い位置に咲いているときは、そのテクスチャインデックスを取得して、分布データを更新します。

//            TextureIndexDistributionData minLengthDistributionData
//                = this.textureIndexDistributionDataList[minLengthIndex];

//            if (minLength < this.nearDistributionThreshold)
//            {
//                textureIndex = minLengthDistributionData.textureIndex;

//                this.textureIndexDistributionDataList[minLengthIndex] = new TextureIndexDistributionData()
//                {
//                    textureIndex = textureIndex,
//                    position = (minLengthDistributionData.position + bloomPosition) / 2,
//                };
//            }

//            // (2.2) 近い位置に咲いていないときは、新たにテクスチャインデックスを取得して、分布データを追加します。

//            else
//            {
//                textureIndex = this.flowerManager.GetRandomMainTextureIndex();

//                this.textureIndexDistributionDataList.Add(new TextureIndexDistributionData()
//                {
//                    textureIndex = textureIndex,
//                    position = bloomPosition
//                });
//            }

//            // (3) 花を咲かせます。

//            this.flowerManager.BloomFlower(bloomPosition, textureIndex);
//        }

//        #endregion Method
//    }
//}