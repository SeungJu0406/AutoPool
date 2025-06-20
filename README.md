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
  - `Get().OnDebug(string)`, `Get().ReturnAfter.OnDebugReturn(string)`, `Return().OnDebug(string)` 
  - 인스펙터에서 풀 상태 확인가능, 검색 지원
- **GC 0B 유지**
  - 초당 1000개 생성/반환 테스트에서도 GarbageCollector 안뜸

## 설치 방법

### 1. **.unitypackage**
- Release 에서 첨부된 `.unitypackage` 파일 다운로드 후 Unity에서 `Import Package > Custom Package` 로 설치

### 2. **Zip**
- Code → Dounload Zip 으로 파일 다운로드 이 후 프로젝트 Assets 폴더에 적용

## 주요 함수
