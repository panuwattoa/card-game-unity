using System;
using System.Collections;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace WarpGate.UniRx
{
	public interface IDummyAssetBundle
	{
		CachedAssetBundle GetCachedAssetBundle();
	}
	
    public static class ObservableUnityWebRequest
    {
        public static IEnumerator Get(string url, IObserver<string> observer, CancellationToken cancellationToken) {
            using (UnityWebRequest uwp = UnityWebRequest.Get(url)) {
                uwp.SendWebRequest();

                while (!uwp.isDone && !cancellationToken.IsCancellationRequested) {
                    yield return null;
                }

                if (cancellationToken.IsCancellationRequested) {
                    observer.OnError(new Exception("request was canceled"));
                    yield break;
                }


                if (uwp.isNetworkError || uwp.isHttpError) {
                    observer.OnError(new Exception(uwp.error));
                    yield break;
                }

                observer.OnNext(uwp.downloadHandler.text);
                observer.OnCompleted();
            }
        }

        public static IEnumerator Post(string url, WWWForm formData, IObserver<string> observer, CancellationToken cancellationToken) {
            using (UnityWebRequest uwp = UnityWebRequest.Post(url, formData)) {
                uwp.SendWebRequest();

                while (!uwp.isDone && !cancellationToken.IsCancellationRequested) {
                    yield return null;
                }

                if (cancellationToken.IsCancellationRequested) {
                    observer.OnError(new Exception("request was canceled"));
                    yield break;
                }

                if (uwp.isNetworkError || uwp.isHttpError) {
                    observer.OnError(new Exception(uwp.error));
                    yield break;
                }

                observer.OnNext(uwp.downloadHandler.text);
                observer.OnCompleted();
            }
        }

        public static IObservable<AssetBundle> GetAssetBundle(string url, IDummyAssetBundle dummyAssetBundle, IProgress<float> progress = null)
    	{
    		return Observable.FromCoroutine<AssetBundle>((observer, cancellationToken) =>
    			FetchAssetBundle(url, dummyAssetBundle, observer, progress, cancellationToken));
    	}
    
    	private static IEnumerator FetchAssetBundle(string url, IDummyAssetBundle dummyAssetBundle, IObserver<AssetBundle> observer, IProgress<float> reportProgress,
    		CancellationToken cancel)
    	{
    		using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, dummyAssetBundle.GetCachedAssetBundle()))
    		{
    			uwr.SendWebRequest();
    			
    			while (!uwr.isDone && !cancel.IsCancellationRequested)
    			{
    				if (reportProgress != null)
    				{
    					try
    					{
    						reportProgress.Report(uwr.downloadProgress);
    					}
    					catch (Exception ex)
    					{
    						observer.OnError(ex);
    						yield break;
    					}
    				}
    				
    				yield return null;
    			}
    
    			if (cancel.IsCancellationRequested)
    			{
    				yield break;
    			}
    			
    			if (reportProgress != null)
    			{
    				try
    				{
    					reportProgress.Report(uwr.downloadProgress);
    				}
    				catch (Exception ex)
    				{
    					observer.OnError(ex);
    					yield break;
    				}
    			}
    
    			if (uwr.isNetworkError || uwr.isHttpError)
    			{
    				observer.OnError(new Exception(uwr.error));
    				yield break;
    			}

                observer.OnNext(DownloadHandlerAssetBundle.GetContent(uwr));
    			observer.OnCompleted();
    		}
    	}
    }
}