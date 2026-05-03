using System;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 프리팹 및 리소스 기반 객체 풀을 관리하고 생성/반환 기능을 제공하는 인터페이스입니다.
    /// </summary>
    public interface IObjectPool
    {
        /// <summary>
        /// 지정된 프리팹에 대한 풀 정보를 가져옵니다.
        /// </summary>
        IPoolInfoReadOnly GetInfo(GameObject prefab);

        /// <summary>
        /// 지정된 컴포넌트 프리팹에 대한 풀 정보를 가져옵니다.
        /// </summary>
        IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component;

        /// <summary>
        /// Resources 경로로 관리되는 풀 정보를 가져옵니다.
        /// </summary>
        IPoolInfoReadOnly GetResourcesInfo(string resources);

        /// <summary>
        /// 지정된 프리팹에 대해 미리 생성할 객체 수를 설정합니다.
        /// </summary>
        IPoolInfoReadOnly SetPreload(GameObject prefab, int count);

        /// <summary>
        /// 지정된 컴포넌트 프리팹에 대해 미리 생성할 객체 수를 설정합니다.
        /// </summary>
        IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component;

        /// <summary>
        /// Resources 경로로 로드되는 객체에 대해 미리 생성할 수를 설정합니다.
        /// </summary>
        IPoolInfoReadOnly SetResourcesPreload(string resources, int count);

        /// <summary>
        /// 지정된 프리팹과 연결된 풀을 모두 비웁니다.
        /// </summary>
        IPoolInfoReadOnly ClearPool(GameObject prefab);

        /// <summary>
        /// 지정된 컴포넌트 프리팹과 연결된 풀을 모두 비웁니다.
        /// </summary>
        IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component;

        /// <summary>
        /// 지정된 Resources 경로와 연결된 풀을 모두 비웁니다.
        /// </summary>
        IPoolInfoReadOnly ClearResourcesPool(string resources);

        /// <summary>
        /// 프리팹을 기반으로 풀에서 GameObject 인스턴스를 가져옵니다.
        /// </summary>
        GameObject Get(GameObject prefab);

        /// <summary>
        /// 프리팹을 기반으로 풀에서 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay);

        /// <summary>
        /// 프리팹을 기반으로 풀에서 인스턴스를 가져와 지정된 위치와 회전에 배치합니다.
        /// </summary>
        GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot);

        /// <summary>
        /// 컴포넌트 프리팹을 기반으로 풀에서 인스턴스를 가져옵니다.
        /// </summary>
        T Get<T>(T prefab) where T : Component;

        /// <summary>
        /// 컴포넌트 프리팹을 기반으로 풀에서 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        T Get<T>(T prefab, Transform transform, bool worldPositionStay) where T : Component;

        /// <summary>
        /// 컴포넌트 프리팹을 기반으로 풀에서 인스턴스를 가져와 지정된 위치와 회전에 배치합니다.
        /// </summary>
        T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component;

        /// <summary>
        /// Resources 경로를 사용하여 풀에서 GameObject 인스턴스를 가져옵니다.
        /// </summary>
        GameObject ResourcesGet(string resouces);

        /// <summary>
        /// Resources 경로를 사용하여 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        GameObject ResourcesGet(string resouces, Transform transform, bool worldPositionStay);

        /// <summary>
        /// Resources 경로를 사용하여 인스턴스를 가져와 지정된 위치와 회전에 배치합니다.
        /// </summary>
        GameObject ResourcesGet(string resouces, Vector3 pos, Quaternion rot);

        /// <summary>
        /// Resources 경로를 사용하여 컴포넌트 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        T ResourcesGet<T>(string resouces) where T : Component;

        /// <summary>
        /// Resources 경로를 사용하여 컴포넌트 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        T ResourcesGet<T>(string resouces, Transform transform, bool worldPositionStay) where T : Component;

        /// <summary>
        /// Resources 경로를 사용하여 컴포넌트 인스턴스를 가져와 지정된 위치와 회전에 배치합니다.
        /// </summary>
        T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component;

        /// <summary>
        /// 제네릭 풀에서 타입 <typeparamref name="T"/> 인스턴스를 가져옵니다.
        /// </summary>
        T GenericPool<T>() where T : class, IPoolGeneric, new();

        /// <summary>
        /// GameObject 인스턴스를 풀에 반환합니다.
        /// </summary>
        IPoolInfoReadOnly Return(GameObject instance);

        /// <summary>
        /// 컴포넌트 인스턴스를 풀에 반환합니다.
        /// </summary>
        IPoolInfoReadOnly Return<T>(T instance) where T : Component;

        /// <summary>
        /// 지정된 지연 시간 후 GameObject 인스턴스를 풀에 반환합니다.
        /// </summary>
        void Return(GameObject instance, float delay);

        /// <summary>
        /// 지정된 지연 시간 후 컴포넌트 인스턴스를 풀에 반환합니다.
        /// </summary>
        void Return<T>(T instance, float delay) where T : Component;

        /// <summary>
        /// 제네릭 풀 인스턴스를 반환하고 해당 풀 정보를 가져옵니다.
        /// </summary>
        IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new();

        /// <summary>
        /// 지정된 지연 시간 후 제네릭 풀 인스턴스를 반환합니다.
        /// </summary>
        void GenericReturn<T>(T instance, float delay) where T : class, IPoolGeneric, new();
    }
}