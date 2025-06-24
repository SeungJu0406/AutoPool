# NSJ\_EasyPoolKit

![NSJ_EasyPoolKit Image](https://github.com/user-attachments/assets/a248efbc-c2c5-4d43-8ac0-d81c91271fa6)


A lightweight and easy-to-use object pooling utility for Unity.

Object pooling in Unity often requires manual prefab registration, initialization logic, and tedious return handling. This tool is designed to streamline all of that, letting you implement pooling logic in just a few lines.

NSJ\_EasyPoolKit is built with simplicity and reusability in mind, and can be dropped into most Unity projects with zero customization.

---

## Features

* **Extremely simple usage**

  * Just `ObjectPool.Get()` and `ObjectPool.Return()`
  * No need to pre-register prefabs
  * `Get()` supports same arguments as `Instantiate()`
  * Generic return type for `Component` is supported

* **Automatic Return Support**

  * Chainable method for scheduling return after delay
  * `ObjectPool.Get().ReturnAfter(seconds)`

* **Safe handling of Rigidbody objects**

  * Automatically resets velocity and toggles `WakeUp` / `Sleep` states

* **Resources-based loading**

  * Use `ResourcesPool.Get("path")` with string-based prefab paths
  * Addressables support under consideration

* **Real-time pool state debugging (Editor supported)**

  * Chainable logging: `OnDebug()`, `OnDebugReturn()`, etc.
  * Inspector interface shows active pools, supports search & sorting

* **Zero Garbage Allocations (GC 0B)**

  * Verified under high-frequency spawn/return (1000 objects/sec)

---

## Installation

### 1. **.unitypackage**

* Download `.unitypackage` from [Releases](https://github.com/SeungJu0406/NSJ_EasyPoolKit/releases)
* Import into your Unity project via `Assets > Import Package > Custom Package`

### 2. **Zip File**

* Download ZIP via GitHub Code menu
* Extract and place in your project's `Assets/` folder

---

## Core APIs

### ✨ Manual Pool Control

Provides API to manually preload, clear, or inspect specific pools.

#### SetPreload

```csharp
ObjectPool.SetPreload(prefab, count);
ResourcesPool.SetPreload("PrefabPath", count);
```

* Preloads up to `count` instances of the specified prefab
* If the pool already contains more than `count`, this call is ignored
* All objects are inactive and stored in pool

#### ClearPool

```csharp
ObjectPool.ClearPool(prefab);
ResourcesPool.ClearPreload("PrefabPath");
```

* Empties the pool for the given prefab
* All pooled objects are destroyed
* Pool will automatically recreate on next usage

#### GetInfo

```csharp
var info = ObjectPool.GetInfo(prefab);
var info = ResourcesPool.GetInfo("PrefabPath");
```

* Retrieves pool status for the specified prefab

Key properties:

```csharp
info.PoolCount     // Total objects in pool
info.ActiveCount   // Currently active objects
info.Name          // Prefab name
```

---

### ✨ ObjectPool.Get API

Retrieves objects from the pool based on prefab. Supports position, rotation, transform parent, and generic type return.

#### 1. Basic GameObject return

```csharp
var instance = ObjectPool.Get(prefab);
```

* Pulls from pool or instantiates new if empty

#### 2. With Transform parent

```csharp
var instance = ObjectPool.Get(prefab, transform, worldPositionStay);
```

* Assigns as child of given transform
* Maintains world position if `worldPositionStay` is true

#### 3. With position and rotation

```csharp
var instance = ObjectPool.Get(prefab, position, rotation);
```

* Places object at specified location

#### 4. Return as Component

```csharp
var component = ObjectPool.Get<MyComponent>(prefab);
```

* Returns specified component from the pooled instance

#### 5. Component + Transform

```csharp
var component = ObjectPool.Get<MyComponent>(prefab, transform, worldPositionStay);
```

#### 6. Component + position/rotation

```csharp
var component = ObjectPool.Get<MyComponent>(prefab, position, rotation);
```

---

### ⟳ ObjectPool.Return API

Returns pooled objects back for reuse.

#### 1. Return GameObject

```csharp
ObjectPool.Return(instance);
```

* Deactivates and resets the object
* Resets transform, scale, parent

#### 2. Return Component

```csharp
ObjectPool.Return<MyComponent>(instance);
```

* Returns component and internally accesses its GameObject

---

### ✨ ResourcesPool API

Load prefabs from Resources folder and pool them with string-based keys.

#### 1. Get GameObject

```csharp
var instance = ResourcesPool.Get("Prefabs/MyBullet");
```

#### 2. Get Component

```csharp
var bullet = ResourcesPool.Get<MyComponent>("Prefabs/MyBullet");
```

#### 3. Additional overloads

Same as ObjectPool.Get:

```csharp
ResourcesPool.Get(name, Transform, bool)
ResourcesPool.Get(name, Vector3, Quaternion)
ResourcesPool.Get<T>(name, Transform, bool)
ResourcesPool.Get<T>(name, Vector3, Quaternion)
```

---

### ⟳ ResourcesPool.Return API

```csharp
ResourcesPool.Return(instance);
ResourcesPool.Return<MyComponent>(instance);
```

Same behavior as `ObjectPool.Return`

---

## 🔧 IPooledObject Interface

Objects can implement `IPooledObject` to receive lifecycle callbacks.

```csharp
public interface IPooledObject {
  void OnCreateFromPool();
  void OnReturnToPool();
}
```

Example:

```csharp
public class Bullet : MonoBehaviour, IPooledObject {
  public void OnCreateFromPool() {
    health = maxHealth;
  }

  public void OnReturnToPool() {
    StopAllCoroutines();
    trailRenderer.Clear();
  }
}
```

---

## 🌟 PoolExtensions (Utility)

Extension methods to simplify pooling behavior and debugging.

#### ReturnAfter

```csharp
ObjectPool.Get(prefab).ReturnAfter(3f);
```

#### OnDebug()

```csharp
ObjectPool.Get(prefab).OnDebug("Created bullet");
```

```
[Pool] Bullet (Active: 1 / 10)
[Log]: Created bullet
```

#### OnDebugReturn()

```csharp
ObjectPool.Get(prefab).ReturnAfter(1.5f).OnDebugReturn("Bullet expired");
```

#### OnDebug(IPoolInfoReadOnly)

```csharp
var info = ObjectPool.Return(instance);
info.OnDebug("Returned bullet");
```

---

## 🤖 MockObjectPool for Testing

Use mock pool for testing/debugging without spawning real GameObjects.

#### Enable Mock Mode

```csharp
ObjectPool.SetMock();
```

#### Restore Real Pool

```csharp
ObjectPool.SetReal();
```

Mock mode outputs log statements instead of instantiating real objects. All core APIs work identically.

---

Feel free to contribute or open issues on GitHub!


---



# NSJ_EasyPoolKit

유니티에서 오브젝트 풀링을 간단하고 빠르게 구현하기 위해 만든 툴

유니티에서 오브젝트 풀링 쓸 때,  
매번 프리팹 등록하고, 따로 관리하고, 자동 반환 구현하는 게 너무 귀찮아서  
아예 구조 자체를 내가 다시 만들었음.

풀링이 어렵지 않았으면 좋겠다는 생각으로 설계했고,  
어지간한 프로젝트에선 커스터마이징 없이 바로 쓸 수 있도록 만들어져 있음.

## 주요 특징
- **매우 간단한 사용법**
  - `ObjectPool.Get()` / `Return()` 딱 두 줄로 끝
  - 풀 자동 생성으로 사전 설정 필요 없음
  - `Get()` 메서드는 `Instantiate()` 와 동일한 매개변수 지원
  - 컴포넌트 반환형 지원
- **자동 반환 지원**
  - 체이닝 메서드로 풀링 이후 반환 예약 가능
  - `ObjectPool.Get().ReturnAfter(float)`
- **Rigidbody 물리 오브젝트도 안전하게 처리**
  - velocity 초기화 + WakeUp,Sleep 까지 자동 관리
- **Resources 기반 로딩 지원**
  - `ResourcesPool.Get(string)` 처럼 문자열 키로 로드 가능
  - 어드레서블 연동도 고려 중
- **풀 상태 실시간 디버깅(에디터 지원)**
  - 체이닝 메서드를 통해 실시간 로그 확인 가능
  - `Get().OnDebug(string)`, `Get().ReturnAfter(float)_.OnDebugReturn(string)`, `Return().OnDebug(string)` 
  - 인스펙터에서 풀 상태 확인가능, 검색 지원
- **GC 0B 유지**
  - 초당 1000개 생성/반환 테스트에서도 GarbageCollector 안뜸

## 설치 방법

### 1. **.unitypackage**
- Release 에서 첨부된 `.unitypackage` 파일 다운로드 후 Unity에서 `Import Package > Custom Package` 로 설치
- https://github.com/SeungJu0406/NSJ_EasyPoolKit/releases

### 2. **Zip**
- Code → Dounload Zip 으로 파일 다운로드 이 후 프로젝트 Assets 폴더에 적용

## 주요 함수
### ✨ 풀 수동 제어 API
풀을 사전에 생성하거나, 비우거나, 상태를 조회할 수 있는 수동 제어 API

#### SetPreload
```cs
ObjectPool.SerPreload(prefab, count);
ResourcesPool.SerPreload("Prefab", count);
```
- 지정된 프리팹에 대한 `count` 개수 만큼 미리 생성
- 현재 풀에 존재하는 수보다 적을 경우 무시 됨
- 오브젝트는 비활성화 된 상태로 풀에 저장
---
#### ClearPool
```cs
ObjectPool.ClearPool(prefab);
ResourcesPool.ClearPreload("Prefab");
```
- 지정 프리팹에 대한 풀을 비움
- 내부 오브젝트는 자동으로 `Destroy()` 처리
- 해당 프리팹이 다시 사용되면 자동으로 재생성

---
#### GetInfo
```cs
IPoolInfoReadOnly info = ObjectPool.GetInfo(prefab);
IPoolInfoReadOnly info = ResourcesPool.GetInfo("Prefab);
```
- 특정 프리팹에 대한 현재 풀 상태 조회
**주요 속성**
```cs
info.PoolCount    // 총 보유 중인 오브젝트 수
info.ActiveCount  // 현재 활성화된 오브젝트 수
info.Name         // 프리팹 이름
```

---
### ✨ ObjectPool.Get API
오브젝트 풀에서 프리팹을 기반으로 오브젝트를 가져오는 API
컴포넌트 반환이나 위치 지정 등 다양한 형태를 지원함.

#### 1. 기본 GameObject 반환
```cs
GameObject instance = ObjectPool.Get(prefab)
```
- 지정한 프리팹으로부터 오브젝트를 풀에서 가져옴
- 기존 풀에 오브젝트가 없으면 새로 생성
---
#### 2. 위치 지정: Transform 기반
```cs
GameObject instance = ObjectPool.Get(prefab, transform, worldPositionStay);
```
- 오브젝트를 풀에서 가져온 뒤, 지정한 트랜스폼을 부모로 설정
- worldPositionStay == true인 경우, 월드 좌표 유지, 그렇지 않으면 transform의 위치와 회전을 따라감
- worldPositionStay는 매개변수로 지정하지 않으면 default값 false로 자동 지정
---
#### 3. 위치 지정 : Vector3 + Quaternion
```cs
GameObject instance = ObjectPool.Get(prefab, position, rotation);
```
- 오브젝트를 지정한 위치와 회전으로 배치하여 풀에서 가져옴
---
#### 4. 컴포넌트 타입 반환
```cs
T instance = ObjectPool.Get<T>(prefab);
```
- 프리팹에서 GameObject를 가져온 후 GetComponent<T>()를 호출하여 반환
- 사용자는 직접 오브젝트를 캐스팅하지 않아도 됨
---
#### 5. Transform 지정 + 컴포넌트 반환
```cs
T instance = ObjectPool.Get<T>(prefab, transform, worldPositionStay);
```
- 오브젝트를 transform 기준으로 생성하고, T 타입 컴포넌트 반환
- worldPositionStay에 따라 위치 유지 여부 결정
---
#### 6. 위치/회전 지정 + 컴포넌트 반환
```cs
T instance = ObjectPool.Get<T>(prefab, position, rotation);
```
- 위치와 회전을 직접 지정하여 오브젝트 생성
- T 타입 컴포넌트를 반환
---
### ⟳ ObjectPool.Return API
풀링된 오브젝트를 반환하는 API
오브젝트는 비활성화되며 풀에 다시 보관됨

#### 1. 기본 반환(GameObject)
```cs
ObjectPool.Return(instance);
```
- 오브젝트를 즉시 반환
- 오브젝트는 비활성화되고 풀에 복원
- 반환 시 위치, 회전, 스케일, 부모 등 초기 상태 복구
- 반환된 풀의 상태를 담은 `IPoolInfoReadOnly` 반환
---
#### 2. 컴포넌트 타입 반환
```cs
ObjectPool.Return<T>(instance);
```
- `GameObject` 타입을 따로 꺼내지 않고 컴포넌트 그대로 반환 가능
---
### ✨ ResourcesPool.Get API
`Resources.Load`를 통해 리소스에서 프리팹을 로드하고, 풀링하여 오브젝트를 가져오는 API
기본 `Get`계열과 동일하지만, 프리팹을 코드에 직접 참조하지 않고 문자열로 지정 가능

#### 1. GameObject 반환
```cs
GameObject instance = ResourcesPool.Get(string);
```
- `Resources/` 경로에서 프리팹을 로드하여 풀에서 가져옴
- 리소스 경로는 확장자 없이 작성 ("Prefab/Cube" 등)
---
#### 2. 컴포넌트 타입 반환
```cs
T instance = ResourcesPool.Get(string);
```
- 로드된 프리팹에서 지정한 T 타입 컴포넌트 반환
---
#### 3. 기타 오버로드
다음 형태도 모두 동일하게 지원
```cs
ResourcesPool.Get(string, Transform, bool)
ResourcesPool.Get(string, Vector3, Quaternion)
ResourcesPool.Get<T>(string, Transform, bool)
ResourcesPool.Get<T>(string, Vector3, Quaternion)
```
- `Get` 계열과 동일한 방식으로 동작
---
### ⟳ ResourcesPool.Return API
풀링된 Resources 오브젝트를 반환하는 API
기본 오브젝트 풀 반환과 동일하게 동작

#### 1. 기본 반환(GameObject)
```cs
ResourcesPool.Return(instance);
```
- 오브젝트 반환
---
#### 2. 컴포넌트 타입 반환
```cs
ObjectPool.Return<T>(instance);
```
- `GameObject` 타입을 따로 꺼내지 않고 컴포넌트 그대로 반환 가능

### 🔧 IPooledObject 인터페이스
풀링된 오브젝트가 풀에 의해 생성 또는 반환될 때 자동으로 호출되는 콜백을 정의하는 인터페이스
오브젝트 초기화 및 상태 정리를 자동화할 수 있음
```cs
public interface IPooledObject
{
  void OnCreateFromPool();
  void OnReturnToPool();
}
```
---
#### OnCreateFromPool
- 오브젝트가 풀에서 꺼내질 때 호출
- 생성 직후 초기화, UI리셋, 트리거 복원 등의 용도로 사용
```cs
void IPooledObject.OnCreateFromPool()
{
  //example
  health = maxHealth;
}
```
---
#### OnReturnToPool
- 오브젝트가 풀에 반환되기 직전 호출
- 파티클 중단, 타이머 초기화, 오디오 정지 등 상태 정리 용도로 사용
```cs
void IPooledObject.OnReturnPool()
{
  //example
  StopAllCoroutine();
  trailRenderer.Clear();
}
```
---
#### 사용 방법
- 풀링하려는 대상 컴포넌트에 IPooledObject 인터페이스를 추가
```cs
    public class SampleObject : MonoBehaviour, IPooledObject
    {
        public void OnCreateFromPool() {}
        public void OnReturnToPool() {}
    }
```
---
### 🌟 PoolExtensions Utility
풀링된 오브젝트를 더 쉽게 사용하고, 디버깅을 도와주는 확장 메서드 모음.
자동 반환 메서드를 통해 코드를 더욱 간결하게 만들고, 풀 상태를 로그로 출력 가능
체이닝 메서드를 통해 사용 가능
#### ReturnAfter(float)
일정 시간 지난 후 자동으로 풀에 반환. 코루틴 없이도 간단하게 지연 반환 가능
```cs
ObjectPool.Get(prefab).ReturnAfter(3f);
```
---
#### OnDebug()
풀에서 꺼낼 때 풀 상태 로그 출력
일반모드에서는 [GetPool], Mock 모드에서는 [MockGetPool] 로그 출력
매개변수로 string을 입력할 시 [Log] 에 입력한 string 값 로그 출력
```cs
ObjectPool.Get(prefab).OnDebug("GetPool Test");
```
출력 예시
```
[GetPool] SampleObject (Active : 1 / 1) 
[Log] : GetPool Test
```
---
#### OnDebugReturn()
`ReturnAfter()` 이후에 풀 상태 로그 출력
```cs
ObjectPool.Get(prefab).ReturnAfter(1f).OnDebugReturn("Return Test");
```
출력 예시
```
[ReturnPool] SampleObject (Active : 0 / 1) 
[Log] : Return Test
```
---
#### OnDebug(this IPoolInfoReadOnly()
`Return` 이후에 풀 상태 출력
```cs
ObjectPool.Return(instance).OnDebug("ReturnPool Test");
```
출력 예시
```
[ReturnPool] SampleObject (Active : 0 / 1) 
[Log] : ReturnPool Test
```
---
### 🤖 테스트용 MockObjectPool
풀링 시스템을 사용하지 않고, 테스트용으로 가짜 오브젝트를 생성해주는 모드
실제 Instantiate 대신 간단한 GameObject를 만들고 Debug.Log()를 출력

#### 활성화 : SetMock()
```cs
ObjectPool.SetMock();
```
- 내부 풀을 `MockObjectPool`로 교채
- 실제 게임 오브젝트를 풀링하징 않고, 임의의 오브젝트를 새로 생성
- 디버깅, 확장 메서드 등 대부분 정상 동작
---
#### 복원 : SetReal()
```cs
ObjectPool.SetReal()
```
- 풀을 다시 실제 `EasyObjectPool` 기반으로 복원
---
