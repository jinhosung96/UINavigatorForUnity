#if UNIRX_SUPPORT
using UniRx;
using UniRx.Triggers;
using Library.UniRxCustom;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif
using UnityEngine.UI;
using JHS.Library.Lifecycle;

namespace JHS.Library.Extension
{
    /// <summary>
    /// 확장 메소드 정의
    /// </summary>
    public static class Extensions
    {
        #region For Component

        public static T AddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (!component.TryGetComponent(out T result)) result = component.AddComponent<T>();

            return result;
        }

        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        #endregion

        #region For GameObjects

        //컴포넌트를 갖고 오고 없으면 추가해서 갖고 온다.
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go.TryGetComponent<T>(out var component) ? component : go.AddComponent<T>();
        }

        public static bool HasComponent<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() != null;
        }

        public static void DestroyChildren(this Transform trans)
        {
            for (var i = trans.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(trans.GetChild(i).gameObject);
            }
        }

        #endregion

        #region For Rigidbody

        public static void ChangeDirection(this Rigidbody rigidbody, Vector3 direction)
        {
            rigidbody.velocity = direction * rigidbody.velocity.magnitude;
        }

        #endregion

        #region For Transform

        public static void AddChildren(this Transform transform, GameObject gameObject)
        {
            gameObject.transform.parent = transform;
        }

        public static void AddChildren(this Transform transform, GameObject[] gameObjects)
        {
            Array.ForEach(gameObjects, child => child.transform.parent = transform);
        }

        public static void AddChildren(this Transform transform, Component[] components)
        {
            Array.ForEach(components, child => child.transform.parent = transform);
        }

        public static void ResetChildPositions(this Transform transform, bool recursive = false)
        {
            foreach (Transform child in transform)
            {
                child.position = Vector3.zero;

                if (recursive)
                {
                    child.ResetChildPositions(true);
                }
            }
        }

        public static void SetChildLayer(this Transform transform, string layerName, bool recursive = false)
        {
            var layer = LayerMask.NameToLayer(layerName);
            SetChildLayersHelper(transform, layer, recursive);
        }

        static void SetChildLayersHelper(Transform transform, int layer, bool recursive)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = layer;

                if (recursive)
                {
                    SetChildLayersHelper(child, layer, true);
                }
            }
        }


        //transform Initialize
        public static void InitializeTransform(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        //transform change X Value
        public static void SetX(this Transform transform, float x)
        {
            var position = transform.position;
            position.x = x;
            transform.position = position;
        }

        //transform change X Value
        public static void SetY(this Transform transform, float y)
        {
            var position = transform.position;
            position.y = y;
            transform.position = position;
        }

        //transform change X Value
        public static void SetZ(this Transform transform, float z)
        {
            var position = transform.position;
            position.z = z;
            transform.position = position;
        }

        public static Vector2 SetX(this Vector2 vec, float x)
        {
            return new Vector2(x, vec.y);
        }

        public static Vector2 SetY(this Vector2 vec, float y)
        {
            return new Vector2(vec.x, y);
        }

        #endregion

        #region For RectTransform

#if UNITASK_SUPPORT
        public static async UniTask Stretch(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
        }
#endif

        #endregion

        #region For Vector3

        public static Vector3 GetCloset(this Vector3 position, IEnumerable<Vector3> otherPosition)
        {
            var closet = Vector3.zero;
            var shortestDistance = Mathf.Infinity;

            foreach (var otherPos in otherPosition)
            {
                var distance = (position - otherPos).sqrMagnitude;

                if (distance < shortestDistance)
                {
                    closet = otherPos;
                    shortestDistance = distance;
                }
            }

            return closet;
        }

        public static Vector3 Offset(this Vector3 position, Vector3 offset) => position + offset;
        public static Vector3 XYToXZ(this Vector2 position) => new(position.x, 0, position.y);
        public static Vector3Int ToVector3Int(this Vector3 position) => new((int)position.x, (int)position.y, (int)position.z);
        public static Vector3 DropY(this Vector3 position) => new(position.x, 0, position.z);

        // 두 Vector3 간에 sqrMagnitude를 이용한 거리 구하기
        public static float DistanceSqr(this Vector3 position, Vector3 otherPosition) => (position - otherPosition).sqrMagnitude;

        // 두 Vector3 간에 sqrMagnitude를 이용하여 거리를 구하고 거리가 지정된 거리보다 작으면 true를 반환한다.
        public static bool IsClose(this Vector3 position, Vector3 otherPosition, float distance) => position.DistanceSqr(otherPosition) < distance * distance;

        // Vector3의 x or y or z 값을 새로운 값으로 대체
        public static Vector3 SetX(this Vector3 position, float x) => new(x, position.y, position.z);
        public static Vector3 SetY(this Vector3 position, float y) => new(position.x, y, position.z);
        public static Vector3 SetZ(this Vector3 position, float z) => new(position.x, position.y, z);

        #endregion

        #region For Choose Or Shuffle

        public static T GetRandomItem<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = list.Count - 1; i > 1; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        public static int Choose(this float[] probs)
        {
            float total = 0;

            foreach (float elem in probs)
            {
                total += elem;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i])
                {
                    return i;
                }
                else
                {
                    randomPoint -= probs[i];
                }
            }

            return probs.Length - 1;
        }

        #endregion

        #region For Animator

        /// <summary>
        /// 현재 재생중인 애니메이션이 종료하였는가?
        /// </summary>
        /// <param name="self">애니메이터 자신</param>
        /// <returns>애니메이션 종료되었는지 여부</returns>
        public static bool IsCompleted(this Animator self)
        {
            return self.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - self.GetAnimatorTransitionInfo(0).duration;
        }

        /// <summary>
        /// 현재 재생중인 애니메이션이 지정한 스테이트에서 종료되었는지 확인
        /// </summary>
        /// <param name="self">애니메이터 자신</param>
        /// <param name="stateHash">설정 스테이트의 해쉬 </param>
        /// <returns>지정된 해쉬 도달 여부</returns>
        public static bool IsCompleted(this Animator self, int stateHash)
        {
            return self.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash && self.IsCompleted();
        }

        /// <summary>
        /// 현재 재생중인 애니메이션 지정비율을 지나쳤는가? normalizeTime이기 떄문에
        /// 비율로 생각해야함.
        /// </summary>
        /// <param name="self">애니메이터 자신</param>
        /// <param name="normalizeTime">지정 비율 시간</param>
        /// <returns>애니메이션이 현재 지정된 구간을 지나가는지 여부</returns>
        public static bool IsPassed(this Animator self, float normalizeTime)
        {
            return self.GetCurrentAnimatorStateInfo(0).normalizedTime > normalizeTime;
        }

        /// <summary>
        /// 애니메이션을 최초부터 재생
        /// </summary>
        /// <param name="self">애니메이터 자신</param>
        /// <param name="shortNameHash">애니메이션의 해쉬</param>
        public static void PlayBegin(this Animator self, int shortNameHash)
        {
            self.Play(shortNameHash, 0, 0.0f);
        }

        #endregion

        #region For Linq

        // 모든 요소에 접근하는 기능을 Linq에 추가
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> collection, Action<TSource> action)
        {
            for (int i = collection.Count() - 1; i >= 0; i--)
            {
                action(collection.ElementAtOrDefault(i));
            }

            return collection;
        }

        // 모든 요소에 접근 하는 기능을 Linq에 추가(index 포함)
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> collection, Action<TSource, int> action)
        {
            for (int i = 0; i < collection.Count(); i++)
            {
                action(collection.ElementAtOrDefault(i), i);
            }

            return collection;
        }

        // 랜덤으로 하나 선택하는 기능을 Linq에 추가
        public static TSource Choose<TSource>(this IEnumerable<TSource> collection)
        {
            return collection.ElementAtOrDefault(Random.Range(0, collection.Count()));
        }

        // 가중치 배열을 활용해 랜덤으로 하나 선택하는 기능을 Linq에 추가
        public static TSource Choose<TSource>(this IEnumerable<TSource> collection, params float[] weights)
        {
            collection = collection.Take(weights.Length);
            var total = weights.Take(collection.Count()).Sum();
            var rand = Random.Range(0.0f, total);

            for (int i = 0; i < collection.Count(); i++)
            {
                if (rand < weights[i]) return collection.ElementAtOrDefault(i);
                else rand -= weights[i];
            }

            return collection.LastOrDefault();
        }

        // Animation Curve를 활용해 가중치 배열을 만들어 랜덤으로 하나 선택하는 기능을 Linq에 추가
        public static TSource Choose<TSource>(this IEnumerable<TSource> collection, AnimationCurve curve)
        {
            var weights = new float[collection.Count()];
            for (int i = 0; i < collection.Count(); i++)
            {
                weights[i] = curve.Evaluate(i / (float)collection.Count());
            }

            return collection.Choose(weights);
        }

        // 중복 없이 여러개의 요소를 랜덤으로 선택하는 기능을 Linq에 추가
        public static IEnumerable<TSource> ChooseSet<TSource>(this IEnumerable<TSource> collection, int count)
        {
            for (int numLeft = collection.Count(); numLeft > 0; numLeft--)
            {
                float prob = (float)count / (float)numLeft;

                if (Random.value <= prob)
                {
                    count--;
                    yield return collection.ElementAtOrDefault(numLeft - 1);

                    if (count == 0) break;
                }
            }
        }

        // 랜덤으로 섞기 기능을 Linq에 추가
        public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> collection)
        {
            var list = collection.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                int j = Random.Range(0, list.Count());
                yield return list[j];
                list[j] = list[i];
            }
        }

        // 가장 가까운 오브젝트 찾기 기능을 Linq에 추가
        public static TSource Closest<TSource>(this IEnumerable<TSource> collection, Vector3 position) where TSource : MonoBehaviour
        {
            var closest = collection.FirstOrDefault();
            float closestDistance = float.MaxValue;
            foreach (var item in collection)
            {
                float distance = Vector3.Distance(position, item.transform.position);
                if (distance < closestDistance)
                {
                    closest = item;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        // Add element to collection using Linq
        // 요소를 컬렉션에 추가하는 기능을 Linq에 추가
        public static IEnumerable<TSource> Add<TSource>(this IEnumerable<TSource> collection, TSource element)
        {
            return collection.Concat(new[] { element });
        }

        // Print all elements of collection using Linq
        // 컬렉션에 대하여 모든 요소를 한 줄로 출력하는 기능을 Linq에 추가
        public static void PrintCollection<TSource>(this IEnumerable<TSource> collection)
        {
            Debug.Log(string.Join(", ", collection.Select(x => x.ToString())));
        }
        public static void PrintCollection<TSource>(this IEnumerable<TSource> collection, string prefix)
        {
            Debug.Log(prefix + string.Join(", ", collection.Select(x => x.ToString())));
        }

        // 컬렉션에 대하여 주어진 기준에 따른 가장 큰 요소를 반환하는 기능을 Linq에 추가
        public static TSource MaxBy<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, TResult> selector) where TResult : IComparable<TResult>
        {
            if (!collection.Any()) return default;
            return collection.Aggregate((x, y) => selector(x).CompareTo(selector(y)) > 0 ? x : y);
        }

        // 컬렉션에 대하여 주어진 기준에 따른 가장 작은 요소를 반환하는 기능을 Linq에 추가
        public static TSource MinBy<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, TResult> selector) where TResult : IComparable<TResult>
        {
            if (!collection.Any()) return default;
            return collection.Aggregate((x, y) => selector(x).CompareTo(selector(y)) < 0 ? x : y);
        }

        #endregion

        #region For UniRx

#if UNIRX_SUPPORT

        public static IObservable<Unit> OnDrawGizmosAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();

            return GetOrAddComponent<ObservableOnDrawGizmosTrigger>(component.gameObject).OnDrawGizmosAsObservable();
        }

        public static IObservable<Unit> OnGUIAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();

            return GetOrAddComponent<ObservableOnGUITrigger>(component.gameObject).OnGUIAsObservable();
        }
        
        public static IObservable<Unit> OnMouseDownAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return GetOrAddComponent<ObservableOnMouseDownTrigger>(component.gameObject).OnMouseDownAsObservable();
        }
        
        public static IObservable<Unit> OnMouseDragAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return GetOrAddComponent<ObservableOnMouseDragTrigger>(component.gameObject).OnMouseDragAsObservable();
        }
        
        public static IObservable<Unit> OnMouseEnterAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return GetOrAddComponent<ObservableOnMouseEnterTrigger>(component.gameObject).OnMouseEnterAsObservable();
        }
        
        public static IObservable<Unit> OnMouseExitAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return GetOrAddComponent<ObservableOnMouseExitTrigger>(component.gameObject).OnMouseExitAsObservable();
        }
        
        public static IObservable<Unit> OnMouseOverAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return GetOrAddComponent<ObservableOnMouseOverTrigger>(component.gameObject).OnMouseOverAsObservable();
        }
        
        public static IObservable<Unit> OnMouseUpAsButtonAsObservable(this GameObject gameObject)
        {
            if (!gameObject) return Observable.Empty<Unit>();
            
            return GetOrAddComponent<ObservableOnMouseUpAsButtonTrigger>(gameObject).OnMouseUpAsButtonAsObservable();
        }
        
        public static IObservable<Unit> OnMouseUpAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return GetOrAddComponent<ObservableOnMouseUpTrigger>(component.gameObject).OnMouseUpAsObservable();
        }

        // 일정 시간 내 Double Click을 감지하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnDoubleClick(this Button button, float interval = 0.5f)
        {
            return button.OnClickAsObservable().Buffer(TimeSpan.FromSeconds(interval)).Where(xs => xs.Count >= 2).AsUnitObservable().Share();
        }

        // 버튼을 터치하고 있는 중인 것을 감지하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnTouchingAsObservable(this Button button)
        {
            return Observable.EveryUpdate().SkipUntil(button.OnPointerDownAsObservable()).TakeUntil(button.OnPointerUpAsObservable()).RepeatUntilDestroy(button).AsUnitObservable().Share();
        }

        // 현재 진행 중인 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator)
        {
            return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration).AsUnitObservable().FirstOrDefault();
        }

        // 지정된 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator, string animationName)
        {
            return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)).Where(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration)
                .AsUnitObservable().FirstOrDefault();
        }

        // 지정된 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator, int animationHash)
        {
            return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == animationHash).Where(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration)
                .AsUnitObservable().FirstOrDefault();
        }

        // 사운드 재생이 종료 시 이벤트를 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAudioCompleteAsObservable(this AudioSource audioSource)
        {
            return Observable.EveryUpdate().Where(_ => !audioSource.isPlaying).AsUnitObservable().FirstOrDefault();
        }

        // 파티클 재생 종료 시 이벤트를 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnParticleCompleteAsObservable(this ParticleSystem particleSystem)
        {
            return Observable.EveryUpdate().TakeUntilDestroy(particleSystem).Where(_ => !particleSystem.IsAlive()).AsUnitObservable().FirstOrDefault();
        }

        public static IDisposable AddTo<T>(this T disposable) where T : IDisposable
            => disposable.AddTo(SceneLife.In.gameObject);
        
#endif

        #endregion

        #region For UniTask

        #if UNITASK_SUPPORT
        
        public static async UniTask IgnoreError(this UniTask task)
        {
            try
            {
                await task;
            }
            catch
            {
                // ignored
            }
        }

#endif

        #endregion

        #region For Debug

        // ReSharper disable once HeapView.PossibleBoxingAllocation
        public static void Print<T>(this T obj) => Debug.Log(obj.ToString());
        public static void Print<T>(this T obj, string prefix) => Debug.Log($"{prefix} : {obj}");

        #endregion

        #region For Json

        public static string ToJson(this Object obj) => JsonUtility.ToJson(obj);
        public static T FromJson<T>(this string json) => JsonUtility.FromJson<T>(json);

        #endregion

        #region For String

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
            => enumerable == null || !enumerable.Any();

        #endregion

        #region For BytesConvert

        public static byte[] GetClipData(this AudioClip clip)
        {
            var floatData = new float[clip.samples * clip.channels];
            clip.GetData(floatData, 0);
        
            var byteData = new byte[floatData.Length * 4];
            Buffer.BlockCopy(floatData, 0, byteData, 0, byteData.Length);

            return byteData;
        }

        public static float[] ToFloatArray(this IEnumerable<byte> byteArray)
        {
            var enumerable = byteArray as byte[] ?? byteArray.ToArray();
            var floatArray = new float[enumerable.Count() / 4];
            Buffer.BlockCopy(enumerable.ToArray(), 0, floatArray, 0, enumerable.Count());
            return floatArray;
        }
        
        public static byte[] ObjectToByte(this object obj)
        {
            try
            {
                using var stream = new MemoryStream();
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
            catch (Exception e)
            {
                e.Print();
            }

            return null;
        }

        #endregion

        #region For Canvas
    
        public static Vector3 GetWorldPosFor(this Canvas self, Component comp,  Vector2 screenOffset = default,  Vector3 worldOffset = default)
        {
            var position = self.worldCamera.WorldToScreenPoint(comp.transform.position + worldOffset) + (Vector3)screenOffset;
            position.z = (self.transform.position - self.worldCamera.transform.position).magnitude;
            return self.worldCamera.ScreenToWorldPoint(position);
        }

        #endregion
    }
}