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

### 2. **Zip**
- Code → Dounload Zip 으로 파일 다운로드 이 후 프로젝트 Assets 폴더에 적용

## 주요 함수
## ObjectPool.Get API
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
### ObjectPool.Return API
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
### ResourcesPool.Get API
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
### ResourcesPool.Return API
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

### IPooledObject 인터페이스
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
### PoolExtensions Utility
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
### 테스트용 MockObjectPool
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
