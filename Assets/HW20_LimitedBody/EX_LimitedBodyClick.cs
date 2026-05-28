using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class EX_LimitedBodyClick : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        // ARCamera 또는 MainCamera를 자동으로 찾아옵니다.
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. 클릭/터치 입력 감지 (New Input System 방식) 
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            // 2. UI를 클릭한 경우 게임 로직은 실행하지 않고 종료
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UI 클릭됨");
                return;
            }

            // 3. 게임 오브젝트 클릭 처리 (Raycast) 
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 클릭된 오브젝트가 '나 자신(이 스크립트가 붙은 큐브)'일 때만 작동
                if (hit.transform == this.transform)
                {
                    Debug.Log($"오브젝트 클릭됨: {hit.transform.name} -> 복사본(Instance) 생성!");

                    // [교수님 구식 코드의 핵심 로직 이식]
                    // 나 자신(gameObject)을 똑같이 복사(Instantiate)합니다.
                    GameObject Clone = Instantiate(gameObject, transform.position, transform.rotation);

                    // 복사본의 Collider를 trigger로 바꾸어 서로 밀려나지 않게 합니다.
                    if (Clone.GetComponent<Collider>() != null)
                    {
                        Clone.GetComponent<Collider>().isTrigger = true;
                    }

                    // 복사본의 중력(Gravity)을 켜서 떨어지게 만듭니다.
                    Rigidbody rb = Clone.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.useGravity = true;
                        // 랜덤한 회전(관성)을 주어 떨어질 때 생동감을 줍니다.
                        rb.angularVelocity = new Vector3(Random.value * 100, Random.value * 100, Random.value * 100);
                    }

                    // 이 스크립트가 복사본에도 붙어있으면 무한 클릭 오류가 나므로 복사본에서는 이 스크립트를 지워줍니다.
                    Destroy(Clone.GetComponent<EX_LimitedBodyClick>());

                    // 3초 뒤에 복사본을 메모리에서 삭제합니다.
                    Destroy(Clone, 3f);
                }
            }
        }
    }
}