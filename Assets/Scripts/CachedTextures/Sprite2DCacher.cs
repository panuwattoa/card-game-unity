//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UniRx;
//using UnityEngine;
//using UnityEngine.Networking;
//using WarpGate.Functional;

//public class Sprite2DCacher 
//{
//    private static readonly Dictionary<string, Sprite> m_cacheSprites = new Dictionary<string, Sprite>();

//    public static async UniTask<Result<Sprite>> GetSprite(string fileName)
//    {

//        if (m_cacheSprites.TryGetValue(fileName, out var sprite))
//            return Result<Sprite>.Ok(sprite);
//        var result = await FetchTextureAsync("masterdata");
//        Debug.Log("Got result");
//        result.MatchOk(sp =>
//        {
//            if (!m_cacheSprites.ContainsKey(fileName))
//                m_cacheSprites.Add(fileName, sp);
//        });

//        return result;
//    }

//    private static async UniTask<Result<Sprite>> FetchTextureAsync(string url)
//    {
//        using (var uwp = UnityWebRequestTexture.GetTexture(url))
//        {
//            try
//            {
//                await uwp.SendWebRequest();
//                var tx = DownloadHandlerTexture.GetContent(uwp);
//                var pivot = new Vector2(0.5f, 0.5f);
//                var rect = new Rect(Vector2.zero, new Vector2(tx.width, tx.height));
//                return Result<Sprite>.Ok(Sprite.Create(tx, rect, pivot, 100f));
//            }
//            catch (Exception)
//            {
//                var load = Resources.Load<Texture2D>("Tile/"+ key);
//                //  Debug.LogError("FetchTextureAsync err" + ex);
//                if (load)
//                {
//                    var pivot = new Vector2(0.5f, 0.5f);
//                    var rect = new Rect(Vector2.zero, new Vector2(load.width, load.height));
//                    return Result<Sprite>.Ok(Sprite.Create(load, rect, pivot, 100f));
//                }
//                return Result<Sprite>.Err(null);
//            }
//        }
//    }

//    public static IObservable<Texture2D> Fetch(string url, int screenWidth, int screenHeight)
//    {
//        var stream = Observable.FromMicroCoroutine<Texture2D>((observer) =>
//            GetTextureFromDevice(url, screenWidth, screenHeight, observer)
//        );
//        return stream;
//    }

//    private static IEnumerator GetTextureFromDevice(string path, int screenWidth, int screenHeight, IObserver<Texture2D> observer)
//    {
//        Debug.Log(path);
//        var data = File.ReadAllBytes(path);
//        var tex2D = new Texture2D(screenWidth, screenHeight, TextureFormat.RGBA32, false);
//        tex2D.LoadRawTextureData(data);
//        tex2D.Apply();
//        observer.OnNext(tex2D);
//        observer.OnCompleted();
//        yield break;
//    }
//}
