# UINavigator

---

# 목차

- [개요](#-개요)
    * [소개](##-소개)
        + [계층 구조](###-계층-구조)
        + [플랫 구조](###-플랫-구조)
    * [특징](##-특징)
    * [요구 사항](##-요구-사항)
        + [함께 사용된 라이브러리](###-함께-사용된-라이브러리)
        + [Scripting Define Symbols](###-Scripting-Define-Symbols)
- [시작하기](#-시작하기)
    * [기본 개념](##-기본-개념)
        + [UIContext 및 UIContainer](###-UIContext-및-UIContainer)
        + [UIContext의 종류](###-UIContext의-종류)
    * [UI 등록](##-UI-등록)
    * [UI 전환](##-UI-전환)
        + [새로운 UI 활성화](###-새로운-UI-활성화)
        + [이전 UI로 돌아가기](###-이전-UI로-돌아가기)
    * [UIContainer를 구하는 방법](###-UIContainer를-구하는-방법)
- [UI 전환 애니메이션](#-UI-전환-애니메이션)
- [Lifecycle 재정의](#-Lifecycle-재정의)
- [그 외 추가 기능](#-그-외-추가-기능)
    * [UI 전환 시 데이터 넘기기](##-UI-전환-시-데이터-넘기기)
    * [VContainer 연동](##-VContainer-연동)

---

# 개요

---

## 소개

UINavigator는 본인이 직접 개발한 UGUI 기반 UI 관리 프레임워크입니다.

본 프레임워크는 ‘[Unity Korea, Dev Weeks: 작업 효율을 높이기 위한 유니티 UI 제작 프로그래밍 패턴들](https://www.youtube.com/live/_jW_D2vF9J8?feature=share)’에서 소개하는 구분 법에 의하면 계층 및 플랫 구조에 속합니다.

### 계층 구조

![Untitled](https://github.com/jinhosung96/UINavigator/assets/42510802/50e36fcb-70ac-47ac-8218-4e4e8eda588d)

UI들 간에 계층 관계를 이루며 계속해서 뒤로가기 시 Root로 돌아가는 구조

### 플랫 구조

![Untitled 1](https://github.com/jinhosung96/UINavigator/assets/42510802/9482678c-8399-4757-8974-73b3d64ac677)

UI 간 전환이 있을 때, 각 UI의 히스토리가 보존되는 형태의 구조

## 특징

- 쉬운 UI 전환
- UI 전환 히스토리 관리(뒤로가기 기능)
- GUI 기반 UI 전환 연출 설정
- 각 UI들에 대한 Lifecycle 제공
- Addressable 지원(ADDRESSABLE_SUPPORT에 대한 Scripting Define Symbol 추가)
- VContainer 지원(VCONTAINER_SUPPORT에 대한 Scripting Define Symbol 추가)

## 요구 사항

### 함께 사용된 라이브러리

- UniRx
- UniTask
- Dotween
- VContainer (선택)
- Addressable (선택)

### Scripting Define Symbols

- UNIRX_SUPPORT
- UNITASK_SUPPORT
- DOTWEEN_SUPPORT
- UNITASK_DOTWEEN_SUPPORT
- VCONTAINER_SUPPORT (선택)
- ADDRESSABLE_SUPPORT (선택)

# 시작하기

---

## 기본 개념

### UIContext 및 UIContainer

**UIContext**

UINavigator에 의해 관리되며 화면 상에 노출되는 UI 요소, ‘Sheet, ‘Page’, ‘Modal’로 구분됩니다.

UIContext들은 Sheet, Page, Modal 중 하나를 상속 받은 컴포넌트를 구현해야 합니다.

**UIContainer**

UIContext가 생성될 영역, UIScope의 종류에 따라 ‘SheetContainer’, ‘PageContainer’, ‘ModalContainer’가 존재합니다.

UIContext들은 모두 부모 Container와 동일한 Rect로 생성되며 각 Container들은 자신의 자식 UIContext들에 대한 히스토리를 관리합니다.

### UIContext의 종류

**Sheet**

![UnityScreenNavigator의 Sheet 예시](https://github.com/jinhosung96/UINavigator/assets/42510802/f847bd94-18f6-4918-9bf7-6fc8e1424200)

UnityScreenNavigator의 Sheet 예시

**특징**

- 히스토리를 관리하지 않음(뒤로가기 불가)
- Container 내에 한번에 하나만 활성 됨

**Page**

![UnityScreenNavigator의 Page 예시](https://github.com/jinhosung96/UINavigator/assets/42510802/fb1d94c3-084a-4a27-b72b-899e092af468)

UnityScreenNavigator의 Page 예시

**특징**

- 순차적으로 전환되는 화면
- 전환된 순서에 대한 히스토리를 관리함(뒤로가기 가능)
- Container 내에 한번에 하나만 활성 됨

**Modal**

![UnityScreenNavigator의 Modal 예시](https://github.com/jinhosung96/UINavigator/assets/42510802/b2d27fae-044d-4563-9489-74f37b1ac302)

UnityScreenNavigator의 Modal 예시

**특징**

- 중첩 해서 쌓이는 화면
- 생성된 순서대로 히스토리를 관리함(뒤로가기 가능)
- 최상단 Modal을 제외한 모든 상호작용 차단
- Backdrop 클릭으로 해당 Modal을 종료할 수 있음

**UIScope 중첩**

![UnityScreenNavigator의 화면 중첩 예시](https://user-images.githubusercontent.com/47441314/137634860-ae202ce7-5d2d-48b1-a938-358381d16780.gif)

UnityScreenNavigator의 화면 중첩 예시

위와 같은 UIContext들은 모두 중첩이 가능합니다.

## UI 등록

![Untitled 2](https://github.com/jinhosung96/UINavigator/assets/42510802/fec7b861-3891-4d07-868d-665cbdb73f21)

UIContainer의 Initialize Setting에 해당 Container에서 관리할 UIContext들을 미리 등록해둡니다.

ADDRESSABLE_SUPPORT에 대한 Define Symbol을 추가했다면 Prefab을 통한 생성 방식이 아닌 Addressable을 통한 생성 방식을 선택할 수 있습니다.

이 경우 Prefab 대신 AssetReference를 등록합니다.

SheetContainer 및 PageContainer의 경우 기본 활성화 UI 존재 여부를 지정할 수 있으며, HasDefault를 On으로 할 시, 등록된 UI 중 ‘Element 0’이 Default UI가 됩니다.

![Untitled 3](https://github.com/jinhosung96/UINavigator/assets/42510802/9514df50-aaf5-49b7-b1d5-7d7cb5fc99c4)

ModalContainer의 경우 해당 Container에서 사용할 Backdrop을 별도로 지정해줄 수 있습니다.

## UI 전환

### 새로운 UI 활성화

새로운 UI 활성화 시 먼저 적절한 UIContainer를 구해옵니다.

이 후 해당 UIContainer의 NextAsync<T>()를 호출해 새로운 UI를 활성화 합니다.

활성화 되는 UI는 T에 넘겨준 UIContext의 Type으로 결정되며, Sheet나 Page의 경우 이전 UI를 종료하고 새로운 UI를 활성화합니다.

```csharp
PageContainer pageContainer;

await pageContainer.NextAsync<InventoryPageContext>();
```

### 이전 UI로 돌아가기

PageContainer 및 ModalContainer의 경우 이전 UI들에 대한 히스토리를 보관하고 있습니다.

해당 UIContainer의 PrevAsync()를 호출해주는 것으로 해당 Container의 포커싱 중인 UI를 종료하고 이전 UI를 포커싱합니다.

이 때, count를 넘겨주는 것으로 몇 개 이전으로 돌아 갈지도 지정해줄 수 있습니다.

```csharp
PageContainer pageContainer;

await pageContainer.PrevAsync(); // await pageContainer.PrevAsync(3);
```

만약 특정 UIContainer를 특정하고 싶지 않다면, UIContainer의 정적 메소드인 UIContainer.BackAsync()를 호출하는 방법이 있습니다.

이 때, 현재 UI가 Root UI라 뒤로가기를 실패한다면 결과 값으로 false를 반환합니다.

이를 통해 Root UI에서 뒤로가기 시도 시 Setting Modal을 활성화해주는 로직을 구현할 수 있습니다.

```csharp
if (Input.GetKeyDown(KeyCode.Escape))
{
		if (!await UIContainer.BackAsync())
		{
				modalContainer.NextAsync<SettingModalContext>();
		}
}
```

### UIContainer를 구하는 방법

UINavigator에서는 UIContainer를 구하는 특별한 방법을 몇 가지 제공합니다.

```csharp
PageContainer mainPageContainer = PageContainer.Main;
PageContainer findPageContainer = PageContainer.Find("ContainerName");
PageContainer ofPageContainer = PageContainer.Of(transform);
```

UIContainer.Main은 현재 활성화 되어 있는 씬의 Root로부터 가장 가까운 UIContainer를 반환합니다.

UIContainer.Find는 인스펙터에서 지정해준 UIContainer의 Name을 기준으로 UIContainer를 반환합니다.

![Untitled 4](https://github.com/jinhosung96/UINavigator/assets/42510802/8efafe82-43b3-4e7a-baa9-29cd1e68e7cb)

UIContainer들에게는 Container Setting 아래 Container의 이름을 지정해주는 필드가 존재한다.

UIContainer.Of는 인자로 넘겨준 transform과 가장 가까운 부모 UIContainer를 반환합니다.

UIContainer.Of의 경우 기본적으로 넘겨준 transform을 key로 결과 값을 캐싱하며 캐싱된 값을 새로운 값으로 덮어 씌우고 싶을 시, 매개변수 useCach에 대한 인자 값으로 false를 넘기면 됩니다.

# UI 전환 애니메이션

---

![Untitled 5](https://github.com/jinhosung96/UINavigator/assets/42510802/8c25f546-c589-4fdf-9af3-d1290ce29036)

UIContainer 및 UIContext에는 화면 전환 연출 애니메이션을 설정할 수 있는 필드를 제공합니다.

기본적으로 Move, Rotate, Scale, Fade 연출 방식을 지정할 수 있으며, 섞어서도 사용이 가능합니다.

각 항목에 대해서 연출 지정 방식은 드랍다운을 통해 설정할 수 있으며, None으로 설정 시 연출이 생략됩니다.

연출 지정 방식에 대해서는 MoveShowAnimation 등을 상속 받은 사용자 정의 클래스를 구현함으로써 추가해줄 수 있습니다.

< 예시 코드 - MoveByAlignmentShowAnimation >

```csharp
public enum Alignment
{
    None,
    Left,
    Top,
    Right,
    Bottom
}
    
public class MoveByAlignmentShowAnimation : MoveShowAnimation
{
    [SerializeField] Alignment from;
    [SerializeField] float startDelay;
    [SerializeField] float duration = 0.25f;
    [SerializeField] Ease ease = Ease.Linear;

    public override async UniTask AnimateAsync(RectTransform rectTransform)
    {
        rectTransform.anchoredPosition = PositionFromAlignment(rectTransform, from);
        await rectTransform.DOAnchorPos(PositionFromAlignment(rectTransform, Alignment.None), duration).SetDelay(startDelay).SetEase(ease).SetUpdate(true).ToUniTask();
    }

    Vector2 PositionFromAlignment(RectTransform rectTransform, Alignment alignment)
    {
        var rect = rectTransform.rect;
        return alignment switch
        {
            Alignment.Left => Vector2.left * rect.width,
            Alignment.Top => Vector2.up * rect.height,
            Alignment.Right => Vector2.right * rect.width,
            Alignment.Bottom => Vector2.down * rect.height,
            _ => Vector2.zero
        };
    }
}
```

일반적으로 모든 UIContext는 부모 UIContainer의 연출 방식을 따르지만 필요에 따라 Custom 연출을 지정해줄 수 있습니다.

![Untitled 6](https://github.com/jinhosung96/UINavigator/assets/42510802/2001e139-47a5-4f9c-ae8a-b42e43f0b567)

# Lifecycle 재정의

---

UIContext에는 독립적인 라이프사이클을 제공합니다.

```csharp
/// <summary>
/// Awake보다 먼저 발생되는 이벤트
/// </summary>
public IObservable<Unit> OnPreInitialize => preInitializeEvent.Share();
        
/// <summary>
/// Awake 호출 직후 발생되는 이벤트
/// </summary>
public IObservable<Unit> OnPostInitialize => postInitializeEvent.Share();
        
/// <summary>
/// UI가 활성화를 시작할 때 발생하는 이벤트
/// </summary>
public IObservable<Unit> OnAppear => appearEvent.Share();

/// <summary>
/// UI가 활성화 애니메이션이 진행 중일 때 매 프레임 발생하는 이벤트
/// </summary>
public IObservable<Unit> OnAppearing => OnChangingVisibleState(OnAppear, OnAppeared);

/// <summary>
/// UI가 활성화가 완전히 끝났을 때 발생하는 이벤트
/// </summary>
public IObservable<Unit> OnAppeared => appearedEvent.Share();

/// <summary>
/// UI가 활성화 되어 있는동안 매 프레임 발생하는 이벤트
/// </summary>
public IObservable<Unit> OnUpdate => OnChangingVisibleState(OnAppeared, OnDisappear);

/// <summary>
/// UI가 비활성화를 시작할 때 발생하는 이벤트
/// </summary>
public IObservable<Unit> OnDisappear => disappearEvent.Share();

/// <summary>
/// UI가 비활성화 애니메이션이 진행 중일 때 매 프레임 발생하는 이벤트
/// </summary>
public IObservable<Unit> OnDisappearing => OnChangingVisibleState(OnDisappear, OnDisappeared);

/// <summary>
/// UI가 비활성화가 완전히 끝났을 때 발생하는 이벤트
/// </summary>
public IObservable<Unit> OnDisappeared => disappearedEvent.Share();

/// <summary>
/// UI가 활성화되기 전에 호출
/// 처리가 완전히 완료되고 난 후 UI 활성화
/// </summary>
protected virtual UniTask WhenPreAppearAsync() => UniTask.CompletedTask;

/// <summary>
/// UI가 활성화된 직후 호출
/// 처리가 완전히 완료되고 난 후 OnAppeared 이벤트 호출
/// </summary>
protected virtual UniTask WhenPostAppearAsync() => UniTask.CompletedTask;

/// <summary>
/// UI가 비활성화되기 전에 호출
/// 처리가 완전히 완료되고 난 후 UI 비활성화
/// </summary>
protected virtual UniTask WhenPreDisappearAsync() => UniTask.CompletedTask;

/// <summary>
/// UI가 비활성화된 직후 호출
/// 처리가 완전히 완료되고 난 후 OnDisappeared 이벤트 호출
/// </summary>
protected virtual UniTask WhenPostDisappearAsync() => UniTask.CompletedTask;
```

# 그 외 추가 기능

---

## UI 전환 시 데이터 넘기기

UIContainer의 NextAsync 호출 시 인자 값으로 onPreInitialize 혹은 onPostInitialize를 지정해줄 수 있습니다.

이는 각각 Awake 호출 직전 및 직후에 대한 콜백으로 생성된 UIContext를 인자로 넘겨줍니다.

```csharp
ModalContainer modalContainer;

await modalContainer.NextAsync<ConfirmBoxModalContext>(onPreInitialize: modal =>
{
		modal.TitleText.text = "변경 확인";
    modal.MessageText.text = "정말 변경 하시겠습니까?";
    modal.LeftButtonText.text = "취소";
    modal.RightButtonText.text = "변경";
    modal.OnRightButtonClick.Subscribe(_ =>
    {
		    userData.UserName.Value = view.NicknameInputField.text;
        modalContainer.PrevAsync().Forget();
    });
    modal.OnLeftButtonClick.Subscribe(_ => modalContainer.PrevAsync().Forget());
});
```

## VContainer 연동

VCONTAINER_SUPPORT에 대한 Define Symbol을 추가하는 것으로 UINavigator가 VContainer 기반으로 돌아가도록 설정해줄 수 있습니다.

위와 같이 설정해줄 시 UIContext는 전부 LifetimeScope를 상속 받게 되며 생성 시 자동으로 Inject 처리가 됩니다.