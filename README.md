# UINavigator

UINavigator는 Unity를 위한 강력하고 유연한 UI 내비게이션 시스템으로, UI 스크린, 페이지, 모달을 부드러운 전환과 애니메이션으로 구조적으로 관리할 수 있게 해줍니다.

## 주요 기능

- **다양한 UI 컨테이너 타입**
  - `SheetContainer`: 단일 뷰 표시로 개별 UI 시트 관리
  - `PageContainer`: 내비게이션 히스토리를 지원하는 UI 페이지 처리
  - `ModalContainer`: 백드롭을 지원하는 모달 다이얼로그 제어

- **애니메이션 지원**
  - 내장된 표시/숨김 애니메이션
  - 컨테이너 또는 뷰별 커스텀 애니메이션 설정
  - UI 상태 간 부드러운 전환

- **(선택사항) 의존성 주입 지원**
  - VContainer 통합

- **(선택사항) Addressable Asset 지원**
  - Unity의 Addressable Asset System을 통한 UI 프리팹 로드
  - 동적 UI 로딩 및 인스턴스화

- **이벤트 시스템**
  - 풍부한 라이프사이클 이벤트 (PreInitialize, PostInitialize, Appear, Appeared 등)
  - R3를 사용한 옵저버블 패턴

## 요구사항

- Unity 2021.3 이상
- 의존성:
  - UniTask
  - DOTween
  - R3
  - VContainer (선택사항)
  - Addressables (선택사항)

## 설치 방법

1. 모든 필수 의존성이 프로젝트에 설치되어 있는지 확인
2. UINavigator 패키지 임포트
3. 프로젝트에 필요한 define 심볼 추가:
   - `UNITASK_SUPPORT`
   - `DOTWEEN_SUPPORT`
   - `UNITASK_DOTWEEN_SUPPORT`
   - `R3_SUPPORT`
   - `ADDRESSABLE_SUPPORT` (Addressables 사용 시)
   - `VCONTAINER_SUPPORT` (VContainer 사용 시)

### Package Manager를 통한 설치

Unity 2019.3.4f1 이상 버전에서는 Package Manager에서 직접 Git URL을 통해 설치할 수 있습니다.

1. Package Manager 창을 엽니다 (Window > Package Manager)
2. '+' 버튼을 클릭하고 "Add package from git URL"을 선택합니다
3. 다음 URL을 입력합니다:
```
https://github.com/jinhosung96/UINavigatorForUnity.git
```

또는 `Packages/manifest.json` 파일에 직접 추가할 수 있습니다:
```json
{
  "dependencies": {
    "com.jhs-library.auto-path-generator": "https://github.com/jinhosung96/UINavigatorForUnity.git"
  }
}
```

특정 버전을 설치하려면 URL 뒤에 #{version} 태그를 추가하면 됩니다:
```
https://github.com/jinhosung96/UINavigatorForUnity.git#1.0.0
```

## 기본 사용법

### UIContainer
![image](https://github.com/user-attachments/assets/9fe65ebd-980c-4742-b927-4f554b5e4587)

- AnimationSetting
  - Move, Rotate, Scale, Fade에 대한 Show & Hide 애니메이션 설정
- ContainerSetting
  - 검색을 위한 이름 지정 및 DontDestoryOnLoad 처리 가능
- InitializeSetting
  - Container에서 관리한 UIContext 지정(Resources 혹은 Addressable 방식 중에 지정 가능)
  - HasDefault를 On으로 설정 시 첫번째로 등록한 UIContext를 Default View로 지정(SheetContainer & PageContainer)
    - Default View는 처음부터 활성화된 상태로 시작하며 History에 포함되지 않는다

### UIContext
![image](https://github.com/user-attachments/assets/e2fd0a46-3b7a-48ab-9055-2a25cef6abd2)

- AnimationSetting
  - Move, Rotate, Scale, Fade에 대한 Show & Hide 애니메이션 설정
  - Container의 설정 값을 따를지 따로 설정해줄지 지정 가능
- IsRecycle(Sheet & Page)
  - Show 및 Hide 시 풀링 방식을 사용할지 선택 가능
- Enable Backdrop Button(Modal)
  - Modal의 Backdrop에 뒤로가기 기능을 활성화할지 선택 가능

### Sheet 컨테이너

```csharp
// Sheet 컨테이너 인스턴스 가져오기
var container = SheetContainer.Main;

// 새로운 시트로 이동
await container.NextAsync<MainMenuSheet>();

// 초기화와 함께 이동
await container.NextAsync<GameSheet>(sheet => {
    sheet.Initialize(gameData);
});
```

### Page 컨테이너

```csharp
// 새로운 페이지로 이동
await PageContainer.Main.NextAsync<HomePageView>();

// 이전 페이지로 돌아가기
await PageContainer.Main.PrevAsync();

// 초기 페이지로 리셋
await PageContainer.Main.ResetAsync();
```

### Modal 컨테이너

```csharp
// 모달 다이얼로그 표시
await ModalContainer.Main.NextAsync<ConfirmationDialog>();

// 현재 모달 닫기
await ModalContainer.Main.PrevAsync();
```

## 핵심 개념

### UI 컨테이너

컨테이너는 다양한 타입의 UI 뷰를 관리합니다:
- **Sheet**: 한 번에 하나의 시트만 표시되는 단일 뷰 디스플레이
- **Page**: 히스토리를 지원하는 스택 기반 내비게이션
- **Modal**: 선택적 백드롭이 있는 오버레이 다이얼로그

### 뷰 라이프사이클

각 UI 뷰는 다음 상태를 거칩니다:
1. PreInitialize
2. PostInitialize
3. Appearing
4. Appeared
5. Disappearing
6. Disappeared

### 애니메이션 설정

두 가지 타입의 애니메이션 설정:
- **컨테이너**: 컨테이너 레벨 애니메이션 설정 사용
- **커스텀**: 뷰별 애니메이션 설정 사용

## 고급 기능

### 컨테이너 이름 지정

```csharp
// 이름으로 컨테이너 찾기
var namedContainer = PageContainer.Find("MainPageContainer");
```

### DontDestroyOnLoad 지원

컨테이너를 씬 로드 간에 유지되도록 설정할 수 있습니다:

```csharp
// 인스펙터나 코드에서 설정
container.isDontDestroyOnLoad = true;
```

### 뒤로 가기 내비게이션

```csharp
// 전역적으로 뒤로 가기 내비게이션 처리
if (await UIContainer.BackAsync())
{
    // 뒤로 가기 내비게이션 처리됨
}
```

## 모범 사례

1. **컨테이너 구성**
   - UI의 다른 섹션에 대해 별도의 컨테이너 사용
   - 쉬운 참조를 위해 컨테이너 이름을 적절히 지정

2. **뷰 라이프사이클**
   - PreInitialize에서 뷰 데이터 초기화
   - PostInitialize에서 UI 요소 설정
   - WhenPostDisappearAsync에서 리소스 정리

3. **애니메이션**
   - 일관된 전환을 위해 컨테이너 애니메이션 사용
   - 필요한 경우에만 커스텀 애니메이션 구현

4. **메모리 관리**
   - 자주 사용되는 뷰에 대해 적절한 재활용 설정
   - 정리 메서드에서 리소스 적절히 해제

## 라이선스

이 프로젝트는 MIT 라이선스를 따릅니다 - 자세한 내용은 LICENSE 파일을 참조하세요.
