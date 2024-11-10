using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.Dynamics;

// using VRC.Dynamics;

public partial class Yuukirin_BatchAddCollision : EditorWindow {

    [MenuItem("YUUKIRIN/コリジョン一括追加")]
    private static void ShowWindow()
    {
        var window = GetWindow<Yuukirin_BatchAddCollision>("UIElements");
        window.titleContent = new GUIContent("ゆうきりん/コリジョン一括追加");
        window.Show();
    }
    private ObjectField YUUKIRINBatchAddCollisionTargetField;
    private ObjectField YUUKIRINBatchAddCollisionTargetCollisionField;
    private Button button;
    private Label YUUKIRINBatchAddCollisionTargetErrorElement;
    private Label YUUKIRINBatchAddCollisionTargetCollisionErrorElement;
    
    private void CreateGUI() {
        VisualElement root = rootVisualElement;
        Label Label1 = new(){
            text="コリジョンを適用したいルートボーン"
        };
        YUUKIRINBatchAddCollisionTargetField = new(){
            objectType = typeof(GameObject),
        };
        YUUKIRINBatchAddCollisionTargetErrorElement = new(){
            style = {
                color = Color.red
            }
        };
        Label Label2 = new(){
            text="コリジョン"
        };
        YUUKIRINBatchAddCollisionTargetCollisionField = new(){
            objectType = typeof(VRCPhysBoneCollider)
        };
        YUUKIRINBatchAddCollisionTargetCollisionErrorElement = new(){
            style = {
                color = Color.red
            }
        };
        button = new(){
            text = "適用"
        };
        YUUKIRINBatchAddCollisionTargetField.RegisterValueChangedCallback(_ => ValidateRequired());
        YUUKIRINBatchAddCollisionTargetCollisionField.RegisterValueChangedCallback(_ => ValidateRequired());

        root.Add(Label1);
        root.Add(YUUKIRINBatchAddCollisionTargetField);
        root.Add(YUUKIRINBatchAddCollisionTargetErrorElement);
        root.Add(Label2);
        root.Add(YUUKIRINBatchAddCollisionTargetCollisionField);
        root.Add(YUUKIRINBatchAddCollisionTargetCollisionErrorElement);
        root.Add(button);
        button.clicked += () => {
            BatchAddCollision();
        };
    }

    private void ValidateRequired() {
        GameObject target = (GameObject)YUUKIRINBatchAddCollisionTargetField.value;
        VRCPhysBoneCollider collider = (VRCPhysBoneCollider)YUUKIRINBatchAddCollisionTargetCollisionField.value;
        if (!target) {
            YUUKIRINBatchAddCollisionTargetErrorElement.text = "コリジョンを適用したいルートボーンが指定されていません。";
            YUUKIRINBatchAddCollisionTargetField.style.borderBottomColor = Color.red;
            button.SetEnabled(false);
        }
        if (!collider) {
            YUUKIRINBatchAddCollisionTargetCollisionErrorElement.text = "コリジョンが指定されていません。";
            YUUKIRINBatchAddCollisionTargetCollisionField.style.borderBottomColor = Color.red;
            button.SetEnabled(false);
        }
        if (target) {
            YUUKIRINBatchAddCollisionTargetErrorElement.text = "";
            YUUKIRINBatchAddCollisionTargetField.style.borderBottomColor = Color.black;
        }
        if (collider) {
            YUUKIRINBatchAddCollisionTargetCollisionErrorElement.text = "";
            YUUKIRINBatchAddCollisionTargetCollisionField.style.borderBottomColor = Color.black;
        }
        if (target && collider) {
            button.SetEnabled(true);
        }
    }

    private void BatchAddCollision() {
        GameObject target = (GameObject)YUUKIRINBatchAddCollisionTargetField.value;
        VRCPhysBoneColliderBase collider = (VRCPhysBoneCollider)YUUKIRINBatchAddCollisionTargetCollisionField.value;
        if (!target || !collider) {
            return;
        }
        AddCollision(target, collider);
    }

    private void AddCollision(GameObject target, VRCPhysBoneColliderBase collider) {
        int childCount = target.transform.childCount;
        if (childCount > 0) {
            for (var i = 0; i < childCount; ++i)
            {
                var child = target.transform.GetChild(i).gameObject;
                AddCollision(child, collider);
            }
        }
        if (!target.GetComponent<VRCPhysBone>()) {
            return;
        }
        VRCPhysBone vrcPhysBone = target.GetComponent<VRCPhysBone>();
        vrcPhysBone.allowCollision = VRCPhysBoneBase.AdvancedBool.True;
        bool isSameCol(VRCPhysBoneColliderBase col) {
            return col == collider;
        }

        if (!vrcPhysBone.colliders.Exists(isSameCol)) {
            vrcPhysBone.colliders.Add(collider);
        }
    }

}